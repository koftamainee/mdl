using System;
using System.Globalization;
using MDL.Core;

namespace MDL.Parser;

/// <summary>
/// Recursive-descent parser for the MDL language. Produces an
/// <see cref="MDLDocument"/> matching the grammar defined in <c>grammar.js</c>.
/// </summary>
public sealed class MDLParser
{
    private string _src = string.Empty;
    private int _pos;
    private int _line = 1;
    private int _col = 1;

    /// <summary>
    /// Parses <paramref name="source"/> into a complete MDL document.
    /// </summary>
    /// <exception cref="MDLParserException">When the source is not valid MDL.</exception>
    public MDLDocument Parse(string source)
    {
        _src = source ?? throw new ArgumentNullException(nameof(source));
        _pos = 0;
        _line = 1;
        _col = 1;

        SkipTrivia();
        var root = ParseObjectBody(terminator: '\0', "end of input");

        return new MDLDocument(root);
    }

    /// <summary>
    /// Parses a single value (not a full document) from <paramref name="source"/>.
    /// </summary>
    public MDLValue ParseValue(string source)
    {
        _src = source ?? throw new ArgumentNullException(nameof(source));
        _pos = 0;
        _line = 1;
        _col = 1;

        SkipTrivia();
        MDLValue? value = ParseValueCore(allowObject: true);
        SkipTrivia();
        if (_pos < _src.Length)
            throw Error("unexpected trailing content");
        if (value == null)
            throw Error("expected a value");
        return value;
    }

    private MDLObject ParseObjectBody(char terminator, string terminatorDesc)
    {
        var obj = new MDLObject();
        while (true)
        {
            SkipTrivia();
            if (_pos >= _src.Length)
            {
                if (terminator != '\0')
                    throw Error($"expected '}}' before {terminatorDesc}");
                return obj;
            }

            char c = _src[_pos];
            if (terminator != '\0' && c == terminator)
                return obj;
            if (terminator == '\0' && (c == '}' || c == ']'))
                throw Error($"unexpected '{c}'");

            obj.Add(ParsePair());
        }
    }

    private MDLPair ParsePair()
    {
        var key = ParseKey();
        SkipTrivia();
        MDLValue? value = ParseValueCore(allowObject: true);
        if (value == null)
            throw Error($"expected a value after key \"{key}\"");
        return new MDLPair(key, value);
    }

    private string ParseKey()
    {
        SkipTrivia();
        int start = _pos;
        while (_pos < _src.Length)
        {
            char c = _src[_pos];
            if (c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                Advance();
            }
            else if (_pos > start && c >= '0' && c <= '9')
            {
                Advance();
            }
            else
            {
                break;
            }
        }
        if (_pos == start)
            throw Error("expected a key");
        return _src.Substring(start, _pos - start);
    }

    private MDLValue? ParseValueCore(bool allowObject)
    {
        SkipTrivia();
        if (_pos >= _src.Length)
            return null;

        char c = _src[_pos];
        switch (c)
        {
            case '{':
                if (!allowObject)
                    return null;
                Advance();
                var obj = ParseObjectBody('}', "'}'");
                SkipTrivia();
                Expect('}');
                return obj;

            case '[':
                return ParseList();

            case '"':
                return new MDLString(ParseQuotedString());

            case '`':
                return new MDLString(ParseRawString());

            case '#':
                return null;
        }

        return ParseBareValue();
    }

    private MDLList ParseList()
    {
        Expect('[');
        var list = new MDLList();
        while (true)
        {
            SkipTrivia();
            if (_pos >= _src.Length)
                throw Error("expected ']'");
            if (_src[_pos] == ']')
            {
                Advance();
                return list;
            }
            var item = ParseValueCore(allowObject: true);
            if (item == null)
                throw Error("expected a value in list");
            list.Add(item);
        }
    }

    private string ParseQuotedString()
    {
        Expect('"');
        var sb = new System.Text.StringBuilder();
        while (true)
        {
            if (_pos >= _src.Length)
                throw Error("unterminated string");
            char c = _src[_pos];
            if (c == '"')
            {
                Advance();
                return sb.ToString();
            }
            if (c == '\n')
                throw Error("newline in string");
            if (c == '\\')
            {
                Advance();
                if (_pos >= _src.Length)
                    throw Error("unterminated escape");
                char e = _src[_pos];
                Advance();
                switch (e)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case 'r': sb.Append('\r'); break;
                    case '0': sb.Append('\0'); break;
                    default: sb.Append(e); break;
                }
            }
            else
            {
                sb.Append(c);
                Advance();
            }
        }
    }

    private string ParseRawString()
    {
        Expect('`');
        var sb = new System.Text.StringBuilder();
        while (true)
        {
            if (_pos >= _src.Length)
                throw Error("unterminated raw string");
            char c = _src[_pos];
            if (c == '`')
            {
                Advance();
                return sb.ToString();
            }
            if (c == '\\' && _pos + 1 < _src.Length && _src[_pos + 1] == '`')
            {
                Advance();
                Advance();
                sb.Append('`');
                continue;
            }
            sb.Append(c);
            Advance();
        }
    }

    private MDLValue ParseBareValue()
    {
        int start = _pos;
        while (_pos < _src.Length && !IsTrivia(_src[_pos]) && !IsStructural(_src[_pos]))
            Advance();

        string text = _src.Substring(start, _pos - start);
        if (text.Length == 0)
            throw Error("expected a value");

        if (text == "true") return new MDLBoolean(true);
        if (text == "false") return new MDLBoolean(false);

        if (IsNumber(text))
        {
            if (HasDecimalOrExponent(text))
            {
                double d;
                if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                    return new MDLFloat(d);
            }
            else
            {
                long l;
                if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out l))
                    return new MDLInteger(l);
                throw Error($"integer overflow: '{text}' does not fit in a 64-bit signed integer");
            }
        }

        return new MDLString(text);
    }

    private static bool HasDecimalOrExponent(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '.' || c == 'e' || c == 'E')
                return true;
        }
        return false;
    }

    private static bool IsNumber(string text)
    {
        int i = 0;
        int n = text.Length;
        if (i < n && text[i] == '-') i++;
        int digits = 0;
        while (i < n && char.IsDigit(text[i])) { i++; digits++; }
        if (i < n && text[i] == '.')
        {
            i++;
            while (i < n && char.IsDigit(text[i])) { i++; digits++; }
        }
        if (digits == 0) return false;
        if (i < n && (text[i] == 'e' || text[i] == 'E'))
        {
            i++;
            if (i < n && (text[i] == '+' || text[i] == '-')) i++;
            int expDigits = 0;
            while (i < n && char.IsDigit(text[i])) { i++; expDigits++; }
            if (expDigits == 0) return false;
        }
        return i == n;
    }

    private bool IsTrivia(char c) => c == ' ' || c == '\t' || c == '\n' || c == '\r';

    private static bool IsStructural(char c)
        => c == '{' || c == '}' || c == '[' || c == ']' || c == '"' || c == '`' || c == '#';

    private void SkipTrivia()
    {
        while (_pos < _src.Length)
        {
            char c = _src[_pos];
            if (IsTrivia(c))
            {
                Advance();
            }
            else if (c == '#')
            {
                while (_pos < _src.Length && _src[_pos] != '\n')
                    Advance();
            }
            else
            {
                break;
            }
        }
    }

    private void Expect(char expected)
    {
        if (_pos >= _src.Length || _src[_pos] != expected)
            throw Error($"expected '{expected}'");
        Advance();
    }

    private void Advance()
    {
        if (_pos < _src.Length)
        {
            if (_src[_pos] == '\n')
            {
                _line++;
                _col = 1;
            }
            else
            {
                _col++;
            }
            _pos++;
        }
    }

    private MDLParserException Error(string message)
        => new MDLParserException(message, _line, _col);
}
