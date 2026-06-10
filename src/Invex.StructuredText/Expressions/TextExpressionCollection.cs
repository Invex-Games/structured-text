namespace Invex.StructuredText.Expressions;

/// <summary>
///     An ordered collection of <see cref="TextExpression" /> instances.
///     Implicit conversions from <see cref="string" />, <c>string[]</c>, <see cref="List{T}" />, and
///     single expressions allow model properties to be assigned naturally,
///     e.g. <c>Run = ["dotnet build"]</c> or <c>Branches = ["main"]</c>.
/// </summary>
[PublicAPI]
public sealed class TextExpressionCollection : IEnumerable<TextExpression>
{
    private readonly List<TextExpression> _items = [];

    /// <summary>
    ///     Creates an empty collection.
    /// </summary>
    public TextExpressionCollection() { }

    /// <summary>
    ///     Creates a collection containing the given expressions.
    /// </summary>
    /// <param name="list">The expressions to copy into the collection.</param>
    public TextExpressionCollection(IEnumerable<TextExpression> list) : this()
    {
        _items.AddRange(list);
    }

    /// <summary>
    ///     The number of expressions in the collection.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    ///     The capacity of the underlying list.
    /// </summary>
    public int Capacity => _items.Capacity;

    /// <summary>
    ///     Gets the expression at the given index.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    public TextExpression this[int index] => _items[index];

    /// <inheritdoc />
    public IEnumerator<TextExpression> GetEnumerator() =>
        _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <summary>
    ///     Appends an expression to the collection.
    /// </summary>
    /// <param name="item">The expression to append.</param>
    public void Add(TextExpression item) =>
        _items.Add(item);

    /// <summary>
    ///     Wraps a single expression in a collection.
    /// </summary>
    public static implicit operator TextExpressionCollection(TextExpression expression) =>
        new([expression]);

    /// <summary>
    ///     Wraps an array of expressions in a collection.
    /// </summary>
    public static implicit operator TextExpressionCollection(TextExpression[] array) =>
        new(array);

    /// <summary>
    ///     Wraps a list of expressions in a collection.
    /// </summary>
    public static implicit operator TextExpressionCollection(List<TextExpression> array) =>
        new(array);

    /// <summary>
    ///     Wraps a single string (as a <see cref="RawExpression" />) in a collection.
    /// </summary>
    public static implicit operator TextExpressionCollection(string value) =>
        new([TextExpressions.Raw(value)]);

    /// <summary>
    ///     Wraps an array of strings (as <see cref="RawExpression" />s) in a collection.
    /// </summary>
    public static implicit operator TextExpressionCollection(string[] array) =>
        new(array.Select(TextExpressions.Raw));

    /// <summary>
    ///     Wraps a list of strings (as <see cref="RawExpression" />s) in a collection.
    /// </summary>
    public static implicit operator TextExpressionCollection(List<string> list) =>
        new(list.Select(TextExpressions.Raw));
}

/// <summary>
///     A typed workflow expression collection that documents the expected element result type.
///     The type parameter serves as a compile-time hint for what each expression resolves to.
/// </summary>
[PublicAPI]
public sealed class WorkflowExpressionCollection<T> : IEnumerable<TextExpression>
{
    private readonly TextExpressionCollection _inner;

    /// <summary>
    ///     Creates an empty collection.
    /// </summary>
    public WorkflowExpressionCollection()
    {
        _inner = [];
    }

    /// <summary>
    ///     Wraps an existing untyped collection.
    /// </summary>
    /// <param name="inner">The collection to wrap (not copied).</param>
    public WorkflowExpressionCollection(TextExpressionCollection inner)
    {
        _inner = inner;
    }

    /// <summary>
    ///     Creates a collection containing the given expressions.
    /// </summary>
    /// <param name="list">The expressions to copy into the collection.</param>
    public WorkflowExpressionCollection(IEnumerable<TextExpression> list)
    {
        _inner = new(list);
    }

    /// <summary>
    ///     The number of expressions in the collection.
    /// </summary>
    public int Count => _inner.Count;

    /// <summary>
    ///     The capacity of the underlying list.
    /// </summary>
    public int Capacity => _inner.Capacity;

    /// <summary>
    ///     Gets the expression at the given index.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    public TextExpression this[int index] => _inner[index];

    /// <inheritdoc />
    public IEnumerator<TextExpression> GetEnumerator() =>
        _inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <summary>
    ///     Appends an expression to the collection.
    /// </summary>
    /// <param name="item">The expression to append.</param>
    public void Add(TextExpression item) =>
        _inner.Add(item);

    /// <summary>
    ///     Wraps an untyped collection in a typed one.
    /// </summary>
    [return: NotNullIfNotNull(nameof(collection))]
    public static implicit operator WorkflowExpressionCollection<T>?(TextExpressionCollection? collection) =>
        collection is not null
            ? new(collection)
            : null;

    /// <summary>
    ///     Unwraps the underlying untyped collection.
    /// </summary>
    [return: NotNullIfNotNull(nameof(typed))]
    public static implicit operator TextExpressionCollection?(WorkflowExpressionCollection<T>? typed) =>
        typed?._inner;

    /// <summary>
    ///     Wraps a single expression in a typed collection.
    /// </summary>
    public static implicit operator WorkflowExpressionCollection<T>(TextExpression expression) =>
        new(new([expression]));

    /// <summary>
    ///     Wraps an array of expressions in a typed collection.
    /// </summary>
    public static implicit operator WorkflowExpressionCollection<T>(TextExpression[] array) =>
        new(new(array));

    /// <summary>
    ///     Wraps a list of expressions in a typed collection.
    /// </summary>
    public static implicit operator WorkflowExpressionCollection<T>(List<TextExpression> list) =>
        new(new(list));

    /// <summary>
    ///     Wraps a single string (as a <see cref="RawExpression" />) in a typed collection.
    /// </summary>
    public static implicit operator WorkflowExpressionCollection<T>(string value) =>
        new(new([TextExpressions.Raw(value)]));

    /// <summary>
    ///     Wraps an array of strings (as <see cref="RawExpression" />s) in a typed collection.
    /// </summary>
    public static implicit operator WorkflowExpressionCollection<T>(string[] array) =>
        new(new(array.Select(TextExpressions.Raw)));

    /// <summary>
    ///     Wraps a list of strings (as <see cref="RawExpression" />s) in a typed collection.
    /// </summary>
    public static implicit operator WorkflowExpressionCollection<T>(List<string> list) =>
        new(new(list.Select(TextExpressions.Raw)));
}
