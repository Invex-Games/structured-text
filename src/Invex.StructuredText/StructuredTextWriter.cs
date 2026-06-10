namespace Invex.StructuredText;

/// <summary>
///     An indentation-aware text writer for producing structured text such as YAML.
///     Indentation is managed through disposable scopes returned by <see cref="WriteSection" />,
///     so the nesting of the generating code mirrors the nesting of the generated output.
/// </summary>
/// <param name="indentSize">The number of spaces written per indent level. Defaults to 2.</param>
/// <example>
///     <code>
///         var writer = new StructuredTextWriter();
///
///         using (writer.WriteSection("jobs:"))
///         using (writer.WriteSection("build:"))
///             writer.WriteLine("runs-on: ubuntu-latest");
///
///         var yaml = writer.ToString();
///     </code>
/// </example>
[PublicAPI]
public sealed class StructuredTextWriter(int indentSize = 2)
{
    /// <summary>
    ///     The underlying buffer that accumulates the written text.
    /// </summary>
    public StringBuilder StringBuilder { get; init; } = new();

    /// <summary>
    ///     A counter incremented by every <see cref="Reset" />.
    ///     Section scopes created before a reset compare against this value and become no-ops on dispose,
    ///     so stale scopes cannot corrupt the indentation of content written after the reset.
    /// </summary>
    public int Version { get; private set; } = 1;

    /// <summary>
    ///     The current indent level (number of nested sections), not the number of spaces.
    /// </summary>
    public int Indent { get; private set; }

    /// <summary>
    ///     The number of spaces written per indent level.
    /// </summary>
    public int IndentSize { get; set; } = indentSize;

    /// <summary>
    ///     Appends <paramref name="text" /> without a trailing newline.
    /// </summary>
    /// <param name="text">The text to append. When <c>null</c>, nothing is written.</param>
    /// <param name="indent">
    ///     Whether to prefix the text with the current indentation.
    ///     Pass <c>false</c> when continuing a line that has already been started.
    /// </param>
    public void Write(string? text = null, bool indent = true) =>
        StringBuilder.Append(text is not null
            ? $"{new(' ', indent ? Indent * IndentSize : 0)}{text}"
            : string.Empty);

    /// <summary>
    ///     Appends <paramref name="text" /> as a full line at the current indent level,
    ///     or a blank line when <paramref name="text" /> is <c>null</c>.
    /// </summary>
    /// <param name="text">The text to append, or <c>null</c> to write an empty line.</param>
    public void WriteLine(string? text = null) =>
        StringBuilder.AppendLine(text is not null
            ? $"{new(' ', Indent * IndentSize)}{text}"
            : string.Empty);

    /// <summary>
    ///     Optionally writes a header line, then increases the indent level by one.
    /// </summary>
    /// <param name="text">
    ///     An optional header line (e.g. <c>"steps:"</c>) written at the current indent level
    ///     before the indent is increased. When <c>null</c> or empty, only the indent changes.
    /// </param>
    /// <returns>
    ///     A scope that restores the previous indent level when disposed.
    ///     Dispose it (typically via <c>using</c>) to close the section.
    /// </returns>
    /// <remarks>
    ///     If <see cref="Reset" /> is called while a scope is still open, disposing that scope
    ///     does nothing — the scope is tied to the <see cref="Version" /> it was created under.
    /// </remarks>
    public IDisposable WriteSection(string? text = null)
    {
        if (text is { Length: > 0 })
            StringBuilder.AppendLine($"{new(' ', Indent * IndentSize)}{text}");

        Indent++;

        var version = Version;

        return new ActionScope(() =>
        {
            if (Version == version)
                Indent--;
        });
    }

    /// <summary>
    ///     Clears the buffer, resets the indent level to zero, and invalidates any open section scopes
    ///     by incrementing <see cref="Version" />. Use this to reuse one writer for multiple documents.
    /// </summary>
    /// <param name="indentSize">The indent size to use from this point on. Defaults to 2.</param>
    public void Reset(int indentSize = 2)
    {
        StringBuilder.Clear();
        Indent = 0;
        Version++;
        IndentSize = indentSize;
    }

    /// <summary>
    ///     Returns the accumulated text.
    /// </summary>
    public override string ToString() =>
        StringBuilder.ToString();

    /// <summary>
    ///     A disposable that invokes an action exactly when disposed.
    /// </summary>
    private sealed record ActionScope(Action? OnDispose = null) : IDisposable
    {
        /// <summary>
        ///     Executes the disposal action if it is not null.
        /// </summary>
        public void Dispose() =>
            OnDispose?.Invoke();
    }
}
