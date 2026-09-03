/**
 * @file Data definition language for Mantle Game Engine
 * @author koftamainee <dev@koftamainee.ru>
 * @license MIT
 */

/// <reference types="tree-sitter-cli/dsl" />
// @ts-check

export default grammar({
  name: 'mdl',

  extras: $ => [
    /\s/,
    $.comment,
  ],

  rules: {
    source_file: $ => repeat($.pair),

    pair: $ => seq($.key, $._value),

    key: $ => /[A-Za-z_][A-Za-z0-9_-]*/,

    _value: $ => choice(
      $.object,
      $.list,
      $.number,
      $.boolean,
      $.string,
      $.raw_string,
      $.bare_string,
    ),

    object: $ => seq('{', repeat($.pair), '}'),

    list: $ => seq('[', repeat($._value), ']'),

    // integers and floats, with optional exponent:
    // 123, -123, 123.5, -123.5, 1e10, 1.5e-10, -2E+3
    number: $ => token(prec(2, /-?\d+(\.\d+)?([eE][+-]?\d+)?/)),

    boolean: $ => token(prec(2, choice('true', 'false'))),

    // "quoted string, with \" escapes"
    // A single atomic token (like raw_string below): extras such as
    // `comment` are only ever tried *between* grammar tokens, so if
    // the whole string were built from several sub-tokens, a `#`
    // inside "..." could be lexed as a comment that swallows the
    // closing quote. Keeping it atomic rules that out entirely.
    string: $ => token(seq(
      '"',
      repeat(choice(
        /\\./,
        /[^"\\\n]/,
      )),
      '"',
    )),

    // `raw string`, backslash has no special meaning
    // except immediately before a backtick: \` is an escaped backtick
    raw_string: $ => token(/`(\\`|[^`])*`/),

    // anything else: bare/unquoted string. Cannot contain whitespace or
    // any of the structural/quoting characters.
    bare_string: $ => token(prec(1, /[^\s{}\[\]"`#]+/)),

    comment: $ => token(seq('#', /[^\n]*/)),
  },
});
