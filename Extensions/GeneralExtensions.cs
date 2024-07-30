using System.Linq.Expressions;
using System.Text.Json;

namespace easy_core;

/// <summary>
/// Extension methods.
/// </summary>
public static class GeneralExtensions
{
	/// <summary>
	/// Clones the provided options into a new object.
	/// </summary>
	/// <param name="value">The options to clone.</param>
	/// <param name="includeConverters">Specifies whether to include converts in the resulting object.</param>
	/// <remarks>
	/// Does not clone properties that are only available in .NET 8.0.
	/// </remarks>
	public static JsonSerializerOptions Clone(this JsonSerializerOptions value, bool includeConverters = true)
	{
		var cloned = new JsonSerializerOptions()
		{
			AllowTrailingCommas = value.AllowTrailingCommas,
			DefaultBufferSize = value.DefaultBufferSize,
			DefaultIgnoreCondition = value.DefaultIgnoreCondition,
			DictionaryKeyPolicy = value.DictionaryKeyPolicy,
			Encoder = value.Encoder,
			IgnoreReadOnlyFields = value.IgnoreReadOnlyFields,
			IgnoreReadOnlyProperties = value.IgnoreReadOnlyProperties,
			IncludeFields = value.IncludeFields,
			MaxDepth = value.MaxDepth,
			NumberHandling = value.NumberHandling,
			PropertyNamingPolicy = value.PropertyNamingPolicy,
			PropertyNameCaseInsensitive = value.PropertyNameCaseInsensitive,
			ReadCommentHandling = value.ReadCommentHandling,
			ReferenceHandler = value.ReferenceHandler,
			UnknownTypeHandling = value.UnknownTypeHandling,
			WriteIndented = value.WriteIndented
		};

		if (includeConverters)
			foreach (var converter in value.Converters)
				cloned.Converters.Add(converter);

		return cloned;
	}

	/// <summary>
	/// Creates a lambda expression for the provided property.
	/// </summary>
	/// <typeparam name="TEntity">The type the property exists within.</typeparam>
	/// <param name="propertyName">The name of the property to return.</param>
	/// <remarks>
	/// Use a dot '.' notation to indicate nested properties (ex. ParentClass.ChildClass.PropertyName).
	/// </remarks>
	public static Expression<Func<TEntity, object>> ToLambda<TEntity>(this string propertyName)
	{
		var parameter = Expression.Parameter(typeof(TEntity));
		var current = parameter as Expression;

		foreach (var child in propertyName.Split('.'))
			current = Expression.Property(current, child);

		return Expression.Lambda<Func<TEntity, object>>(Expression.Convert(current, typeof(object)), parameter);
	}

	/// <summary>
	/// Returns the name of the property in the provided expression when possible.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="expression">The expression to find the property name of.</param>
	public static string GetExpressionPropertyName<T, TResult>(this Expression<Func<T, TResult>> expression)
	{
		string name = string.Empty;

		if (expression.Body is MemberExpression m)
			name = m.Member.Name;
		else if (expression.Body is UnaryExpression u)
			name = ((MemberExpression)u.Operand).Member.Name;

		return name;
	}

	/// <summary>
	/// Updates the destination model instance using values from the source model.
	/// </summary>
	/// <typeparam name="TModel">The type of model to update.</typeparam>
	/// <param name="source">The model containing the values to use.</param>
	/// <param name="destination">The model to update.</param>
	/// <param name="properties">The properties of the model to update.</param>
	/// <remarks>
	/// This method can be used to bypass restrictions on private setters or init only properties.
	/// </remarks>
	public static bool TryUpdateModel<TModel>(this TModel source, TModel destination, params Expression<Func<TModel, object?>>[] properties) where TModel : class
	{
		var ok = true;

		foreach (var expression in properties)
		{
			try
			{
				var name = expression.GetExpressionPropertyName();

				if (string.IsNullOrWhiteSpace(name))
				{
					ok = false;
					continue;
				}

				var property = typeof(TModel).GetProperty(name);

				if (property != null)
					property.SetValue(destination, property.GetValue(source, null));
				else
					ok = false;
			}
			catch
			{
				ok = false;
			}
		}

		return ok;
	}

	/// <summary>
	/// Updates the destination model instance using values from the source model.
	/// </summary>
	/// <typeparam name="TModel">The type of model to update.</typeparam>
	/// <param name="source">The model containing the values to use.</param>
	/// <param name="destination">The model to update.</param>
	/// <param name="updateNull">Specifies whether to update null values.</param>
	/// <param name="properties">The properties of the model to update.</param>
	[Obsolete("Use other overload.")]
	public static bool TryUpdateModel<TModel>(this TModel source, TModel destination, bool updateNull, params Expression<Func<TModel, object?>>[] properties) where TModel : class
	{
		var ok = true;

		foreach (var prop in properties)
		{
			try
			{
				var propertyName = prop.GetExpressionPropertyName();

				if (string.IsNullOrWhiteSpace(propertyName))
				{
					ok = false;
					continue;
				}

				var input = typeof(TModel).GetProperty(propertyName)?.GetValue(source, null);

				if (input != null || updateNull)
					typeof(TModel).GetProperty(propertyName)!.SetValue(destination, input);
			}
			catch
			{
				ok = false;
			}
		}

		return ok;
	}
}
