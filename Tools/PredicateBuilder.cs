using System.Linq.Expressions;

namespace easy_core;

/// <summary>
/// Enables the efficient, dynamic composition of query predicates.
/// </summary>
public static class PredicateBuilder
{
	/// <summary>
	/// Creates a predicate that evaluates to true.
	/// </summary>
	public static Expression<Func<T, bool>> True<T>() { return param => true; }

	/// <summary>
	/// Creates a predicate that evaluates to false.
	/// </summary>
	public static Expression<Func<T, bool>> False<T>() { return param => false; }

	/// <summary>
	/// Creates a predicate expression from the specified lambda expression.
	/// </summary>
	public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

	/// <summary>
	/// Returns a <see langword="null"/> predicate to use with methods in this class.
	/// </summary>
	public static Expression<Func<T, bool>>? Create<T>() { return null; }

	/// <summary>
	/// Combines the first predicate with the second using the logical "and".
	/// </summary>
	public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>>? first, Expression<Func<T, bool>> second)
	{
		if (first == null)
			return second;
		else
			return first.Compose(second, Expression.AndAlso);
	}

	/// <summary>
	/// Combines the first predicate with the second using the logical "or".
	/// </summary>
	public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>>? first, Expression<Func<T, bool>> second)
	{
		if (first == null)
			return second;
		else
			return first.Compose(second, Expression.OrElse);
	}

	/// <summary>
	/// Negates the predicate.
	/// </summary>
	public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
	{
		var negated = Expression.Not(expression.Body);
		return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
	}

	/// <summary>
	/// Combines the first expression with the second using the specified merge function.
	/// </summary>
	private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
	{
		var map = first.Parameters
			.Select((first, index) => new { first, second = second.Parameters[index] })
			.ToDictionary(p => p.second, p => p.first);

		var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

		if (secondBody != null)
			return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
		else
			return Expression.Lambda<T>(first.Body, first.Parameters);
	}

	private class ParameterRebinder : ExpressionVisitor
	{
		private readonly Dictionary<ParameterExpression, ParameterExpression> map;

		public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression>? map)
		{
			this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
		}

		public static Expression? ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression expression)
		{
			return new ParameterRebinder(map).Visit(expression);
		}

		/// <inheritdoc/>
		protected override Expression VisitParameter(ParameterExpression parameter)
		{
			if (map.TryGetValue(parameter, out ParameterExpression? replacement))
				parameter = replacement;

			return base.VisitParameter(parameter);
		}
	}
}
