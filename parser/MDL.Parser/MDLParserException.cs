using System;

namespace MDL.Parser;

/// <summary>
/// Thrown when MDL source text cannot be parsed. Carries the position of
/// the problem in line/column form to aid diagnostics.
/// </summary>
public sealed class MDLParserException : Exception
{
    /// <summary>Creates a new parser exception.</summary>
    public MDLParserException(string message, int line, int column)
        : base(message)
    {
        Line = line;
        Column = column;
    }

    /// <summary>One-based line of the error.</summary>
    public int Line { get; }

    /// <summary>One-based column of the error.</summary>
    public int Column { get; }
}
