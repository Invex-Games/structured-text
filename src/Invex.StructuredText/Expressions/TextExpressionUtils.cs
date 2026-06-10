namespace Invex.StructuredText.Expressions;

/// <summary>
///     Utility helpers for working with sequences and trees of <see cref="TextExpression" />.
/// </summary>
[PublicAPI]
public static class TextExpressionUtils
{
    extension(IEnumerable<TextExpression> expressions)
    {
        /// <summary>
        ///     Interleaves <paramref name="separator" /> between consecutive elements of the sequence.
        /// </summary>
        /// <param name="separator">The expression to insert between elements.</param>
        public IEnumerable<TextExpression> Join(TextExpression separator)
        {
            var list = expressions.ToList();

            yield return list[0];

            foreach (var expression in list.Skip(1))
            {
                yield return separator;
                yield return expression;
            }
        }
    }

    /// <summary>
    ///     Recursively flattens an expression tree into its leaf expressions (literals, raw text,
    ///     and workflow-run references), in left-to-right order. Composite nodes such as
    ///     <see cref="AndExpression" /> or <see cref="FormatExpression" /> are traversed into their operands;
    ///     <see cref="CastExpression{TTo}" /> wrappers are unwrapped transparently.
    /// </summary>
    /// <param name="expression">The root of the tree to flatten.</param>
    /// <returns>The leaf expressions of the tree.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for unrecognized expression types.</exception>
    public static IEnumerable<TextExpression> Flatten(TextExpression expression) =>
        expression switch
        {
            AndExpression andExpression => andExpression.Source.SelectMany(Flatten),
            BooleanExpression booleanExpression => [booleanExpression],
            CoalesceExpression coalesceExpression => coalesceExpression.Source.SelectMany(Flatten),
            ConcatExpression concatExpression => concatExpression.Values.SelectMany(Flatten),
            ContainsExpression containsExpression =>
            [
                ..Flatten(containsExpression.Source), ..Flatten(containsExpression.Pattern),
            ],
            EndsWithExpression endsWithExpression =>
            [
                ..Flatten(endsWithExpression.Source), ..Flatten(endsWithExpression.Pattern),
            ],
            EqualExpression equalExpression => [..Flatten(equalExpression.Left), ..Flatten(equalExpression.Right)],
            EvaluateExpression evaluateExpression => Flatten(evaluateExpression.Expression),
            FormatExpression formatExpression =>
            [
                ..Flatten(formatExpression.Source), ..formatExpression.Arguments.SelectMany(Flatten),
            ],
            GreaterThanExpression greaterThanExpression =>
            [
                ..Flatten(greaterThanExpression.Left), ..Flatten(greaterThanExpression.Right),
            ],
            GreaterThanOrEqualToExpression greaterThanOrEqualToExpression =>
            [
                ..Flatten(greaterThanOrEqualToExpression.Left), ..Flatten(greaterThanOrEqualToExpression.Right),
            ],
            HashFilesExpression hashFilesExpression => Flatten(hashFilesExpression.Source),
            IndexAccessExpression indexAccessExpression =>
            [
                ..Flatten(indexAccessExpression.Array), ..Flatten(indexAccessExpression.Index),
            ],
            JoinExpression joinExpression =>
            [
                ..Flatten(joinExpression.Source),
                ..joinExpression.OptionalSeparator is not null
                    ? Flatten(joinExpression.OptionalSeparator)
                    : [],
            ],
            LessThanExpression lessThanExpression =>
            [
                ..Flatten(lessThanExpression.Left), ..Flatten(lessThanExpression.Right),
            ],
            LessThanOrEqualToExpression lessThanOrEqualToExpression =>
            [
                ..Flatten(lessThanOrEqualToExpression.Left), ..Flatten(lessThanOrEqualToExpression.Right),
            ],
            NotEqualExpression notEqualExpression =>
            [
                ..Flatten(notEqualExpression.Left), ..Flatten(notEqualExpression.Right),
            ],
            NotExpression notExpression => Flatten(notExpression.Source),
            NullExpression nullExpression => [nullExpression],
            NumberExpression numberExpression => [numberExpression],
            OrExpression orExpression => orExpression.Source.SelectMany(Flatten),
            PropertyAccessExpression propertyAccessExpression =>
            [
                ..Flatten(propertyAccessExpression.Object), ..Flatten(propertyAccessExpression.Property),
            ],
            RawExpression rawExpression => [rawExpression],
            StartsWithExpression startsWithExpression =>
            [
                ..Flatten(startsWithExpression.Source), ..Flatten(startsWithExpression.Pattern),
            ],
            StepOutcomeExpression stepOutcomeExpression => [stepOutcomeExpression],
            StepOutcomeTypeExpression stepOutcomeTypeExpression => [stepOutcomeTypeExpression],
            StepOutputExpression stepOutputExpression => [stepOutputExpression],
            StringExpression stringExpression => [stringExpression],
            TargetOutcomeExpression targetOutcomeExpression => [targetOutcomeExpression],
            TargetOutcomeTypeExpression targetOutcomeTypeExpression => [targetOutcomeTypeExpression],
            TargetOutputExpression targetOutputExpression => [targetOutputExpression],
            ToJsonExpression toJsonExpression => [toJsonExpression],
            _ when expression.GetType() is { IsGenericType: true } type &&
                   type.GetGenericTypeDefinition() == typeof(CastExpression<>) => Flatten(
                (TextExpression)type.GetProperty(nameof(CastExpression<>.Inner))!.GetValue(expression)!),
            _ => throw new ArgumentOutOfRangeException(nameof(expression)),
        };
}
