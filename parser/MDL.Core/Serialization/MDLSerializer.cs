using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MDL.Core.Model;
using MDL.Core.Parsing;

namespace MDL.Core.Serialization
{
    /// <summary>
    /// Populates CLR objects from parsed MDL documents. Maps MDL object keys to
    /// members of the target type by name (overridable with <see cref="MDLNameAttribute"/>,
    /// excludable with <see cref="MDLIgnoreAttribute"/>).
    /// </summary>
    public static class MDLSerializer
    {
        private static readonly System.Threading.ThreadLocal<MdlParser> Parser =
            new System.Threading.ThreadLocal<MdlParser>(() => new MdlParser());
        private static readonly ConcurrentDictionary<Type, ObjectMapper> Mappers =
            new ConcurrentDictionary<Type, ObjectMapper>();

        private sealed class ObjectMapper
        {
            public MemberMapper[] Members = Array.Empty<MemberMapper>();
            public Func<object>? Factory;
        }

        private sealed class MemberMapper
        {
            public string Key = string.Empty;
            public Type TargetType = typeof(object);
            public Action<object, object?>? Setter;
        }

        /// <summary>
        /// Converts a parsed <paramref name="document"/> into an instance of type
        /// <typeparamref name="T"/>.
        /// </summary>
        public static T Deserialize<T>(MDLDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            return (T)FromValue(typeof(T), document.Root)!;
        }

        /// <summary>
        /// Converts a parsed <paramref name="value"/> into an instance of type
        /// <typeparamref name="T"/>.
        /// </summary>
        public static T Deserialize<T>(MDLValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return (T)FromValue(typeof(T), value)!;
        }

        /// <summary>
        /// Parses <paramref name="source"/> and converts it into an instance of
        /// type <typeparamref name="T"/>.
        /// </summary>
        public static T Deserialize<T>(string source) => (T)FromValue(typeof(T), Parser.Value.Parse(source).Root)!;

        /// <summary>
        /// Parses the content of <paramref name="stream"/> and converts it into an
        /// instance of type <typeparamref name="T"/>.
        /// </summary>
        public static T Deserialize<T>(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using var reader = new StreamReader(stream);
            return (T)FromValue(typeof(T), Parser.Value.Parse(reader.ReadToEnd()).Root)!;
        }

        private static object? FromValue(Type type, MDLValue? value)
        {
            if (value == null)
                return null;

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (value is MDLObject obj)
                return FromObject(type, obj);

            if (value is MDLList list)
                return FromList(type, list);

            if (value is MDLInteger integer)
            {
                long l = integer.Value;
                if (type == typeof(long)) return l;
                if (type == typeof(int)) return (int)l;
                if (type == typeof(float)) return (float)l;
                if (type == typeof(double)) return (double)l;
                if (type == typeof(short)) return (short)l;
                if (type == typeof(byte)) return (byte)l;
                return Convert.ChangeType(l, type);
            }

            if (value is MDLFloat @float)
            {
                double d = @float.Value;
                if (type == typeof(double)) return d;
                if (type == typeof(float)) return (float)d;
                if (type == typeof(long)) return (long)d;
                if (type == typeof(int)) return (int)d;
                if (type == typeof(short)) return (short)d;
                if (type == typeof(byte)) return (byte)d;
                return Convert.ChangeType(d, type);
            }

            if (value is MDLBoolean boolean)
            {
                if (type == typeof(bool)) return boolean.Value;
                return Convert.ChangeType(boolean.Value, type);
            }

            if (value is MDLString str)
                return FromString(type, str.Value);

            return null;
        }

        private static object FromObject(Type type, MDLObject obj)
        {
            if (type == typeof(MDLObject)) return obj;
            if (type == typeof(MDLValue)) return obj;
            if (type == typeof(object)) return obj;
            if (type == typeof(MDLDocument)) return new MDLDocument(obj);

            if (typeof(IDictionary).IsAssignableFrom(type) || IsGenericDictionary(type))
                return FromDictionary(type, obj);
            var mapper = GetMapper(type);
            if (mapper.Factory == null)
                throw new InvalidOperationException($"Type {type} cannot be instantiated.");

            object instance = mapper.Factory();
            for (int i = 0; i < mapper.Members.Length; i++)
            {
                var m = mapper.Members[i];
                var v = FindValue(obj, m.Key);
                if (v == null)
                    continue;
                m.Setter?.Invoke(instance, FromValue(m.TargetType, v));
            }
            return instance;
        }

        private static MDLValue? FindValue(MDLObject obj, string key)
        {
            var pairs = obj.Pairs;
            for (int i = 0; i < pairs.Count; i++)
            {
                if (string.Equals(pairs[i].Key, key, StringComparison.OrdinalIgnoreCase))
                    return pairs[i].Value;
            }
            return null;
        }

        private static object FromList(Type type, MDLList list)
        {
            if (type == typeof(MDLList)) return list;
            if (type == typeof(MDLValue)) return list;
            if (type == typeof(object)) return list;

            Type elementType = typeof(object);
            Type listType = type;

            if (type.IsArray)
            {
                elementType = type.GetElementType() ?? typeof(object);
                var result = Array.CreateInstance(elementType, list.Count);
                for (int i = 0; i < list.Count; i++)
                    result.SetValue(FromValue(elementType, list.Items[i]), i);
                return result;
            }

            if (IsGenericList(type, out elementType))
            {
                var concrete = typeof(List<>).MakeGenericType(elementType);
                var result = Activator.CreateInstance(concrete);
                var add = concrete.GetMethod("Add");
                foreach (var item in list.Items)
                    add.Invoke(result, new[] { FromValue(elementType, item) });
                return result;
            }

            throw new InvalidOperationException($"Type {type} is not a supported list target.");
        }

        private static object FromDictionary(Type type, MDLObject obj)
        {
            var (kvType, kvValue) = GetDictionaryTypes(type);
            Type keyType = kvType;
            Type valueType = kvValue ?? typeof(object);

            var concrete = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var result = Activator.CreateInstance(concrete);
            var add = concrete.GetMethod("Add") ?? throw new InvalidOperationException("Dictionary cannot be built.");

            foreach (var pair in obj.Pairs)
            {
                var key = FromString(keyType, pair.Key);
                var val = FromValue(valueType, pair.Value);
                add.Invoke(result, new[] { key, val });
            }
            return result;
        }

        private static object FromString(Type type, string text)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string)) return text;
            if (type == typeof(char) && text.Length == 1) return text[0];
            if (type.IsEnum)
            {
                if (Enum.IsDefined(type, text))
                    return Enum.Parse(type, text);
                long num;
                if (long.TryParse(text, out num))
                    return Enum.ToObject(type, num);
                throw new InvalidOperationException($"'{text}' is not a valid {type.Name} value.");
            }
            return Convert.ChangeType(text, type);
        }

        private static ObjectMapper GetMapper(Type type)
        {
            return Mappers.GetOrAdd(type, BuildMapper);
        }

        private static ObjectMapper BuildMapper(Type type)
        {
            var mapper = new ObjectMapper();

            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
                mapper.Factory = () => ctor.Invoke(null);

            var members = new List<MemberMapper>();
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            foreach (var prop in type.GetProperties(flags))
            {
                if (!prop.CanWrite || prop.GetIndexParameters().Length != 0)
                    continue;
                if (prop.GetCustomAttribute<MDLIgnoreAttribute>() != null)
                    continue;

                string key = ResolveKey(prop);
                var setter = prop.SetMethod;
                members.Add(new MemberMapper
                {
                    Key = key,
                    TargetType = prop.PropertyType,
                    Setter = (inst, val) => setter.Invoke(inst, new[] { val }),
                });
            }

            foreach (var field in type.GetFields(flags))
            {
                if (field.IsInitOnly || field.IsLiteral)
                    continue;
                if (field.GetCustomAttribute<MDLIgnoreAttribute>() != null)
                    continue;

                string key = ResolveKey(field);
                members.Add(new MemberMapper
                {
                    Key = key,
                    TargetType = field.FieldType,
                    Setter = (inst, val) => field.SetValue(inst, val),
                });
            }

            mapper.Members = members.ToArray();
            return mapper;
        }

        private static string ResolveKey(MemberInfo member)
        {
            var attr = member.GetCustomAttribute<MDLNameAttribute>();
            return attr != null ? attr.Name : member.Name;
        }

        private static bool IsGenericList(Type type, out Type elementType)
        {
            elementType = typeof(object);
            if (!type.IsGenericType)
                return false;
            var def = type.GetGenericTypeDefinition();
            if (def == typeof(List<>) || def == typeof(IList<>) || def == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            return false;
        }

        private static bool IsGenericDictionary(Type type)
        {
            if (!type.IsGenericType)
                return false;
            var def = type.GetGenericTypeDefinition();
            return def == typeof(Dictionary<,>) || def == typeof(IDictionary<,>) || def == typeof(IReadOnlyDictionary<,>);
        }

        private static (Type, Type?) GetDictionaryTypes(Type type)
        {
            if (type.IsGenericType && IsGenericDictionary(type))
            {
                var args = type.GetGenericArguments();
                return (args[0], args.Length > 1 ? args[1] : typeof(object));
            }
            return (typeof(string), typeof(object));
        }
    }
}
