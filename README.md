# MDL - Mantle Data Language
Mantle Data language is a language for declaring data, primary used in Mantle Game Engine.
Syntax is like JSON, but without all of it verbosity.

## File example
``` mdl
# No starting {
# "" required only for string with special symbols
#   like spaces or various escape sequences 

shader pbr
stages {
  vertex {
    src shaders/pbr.slang
    entry vs_main
  }

  fragment {
    src shaders/pbr.slang
    entry fs_main
  }
}

pipeline {
  depth_test true
  depth_write true
  blend opaque
  cull_mode back
  polygon_mode fill
}

# Lists can enumerate on a single line
some_list [ Monday Tuesday ]

string_with_spaces "Hello world!"

# requires "" for # symbol
black_color "#FFFFFF"

# raw string for \
windows_path `C:\Projects\Mantle`

# raw string with \` symbol
hello2 `Hello \` World!`
```

## Features
- No nil or null as a separate type
- String is primary type. All values can be parsed like strings
- Numbers like like 123 or 123.5 can be parsed like Integer, Floats and Strings. Otherwise - error.
- Booleans like true or false can be parsed like Bools and Strings. Otherwise - error.

## Parser
Example parser in C# available in `parser/`
