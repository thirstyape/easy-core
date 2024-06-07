using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace easy_core;

/// <summary>
/// Extension methods related to attributes.
/// </summary>
public static class AttributeExtensions
{
	/// <summary>
	/// Returns the display name of a provided type if found.
	/// </summary>
	/// <param name="type">The data type to return the name for.</param>
	public static string GetTypeDisplayName(this Type type)
	{
		return type.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? type.Name;
	}

	/// <summary>
	/// Returns the display description of a provided type if found.
	/// </summary>
	/// <param name="type">The data type to return the description for.</param>
	public static string? GetTypeDisplayDescription(this Type type)
	{
		return type.GetCustomAttribute<DisplayAttribute>()?.GetDescription();
	}

	/// <summary>
	/// Returns the <see cref="DisplayAttribute.Name"/> value of a provided property if found.
	/// </summary>
	/// <param name="property">The property to find the name of.</param>
	public static string GetPropertyDisplayName(this PropertyInfo property)
	{
		return property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? property.Name;
	}

	/// <summary>
	/// Returns the <see cref="DisplayAttribute.Name"/> value of a provided property if found.
	/// </summary>
	/// <typeparam name="TModel">The type of the property.</typeparam>
	/// <param name="property">The property to find the name of.</param>
	public static string GetPropertyDisplayName<TModel>(this Expression<Func<TModel>> property)
	{
		return property.GetPropertyAttribute<TModel, DisplayAttribute>()?.GetName() ?? "";
	}

	/// <summary>
	/// Returns the display name of a provided property if found.
	/// </summary>
	/// <param name="propertyName">The name of the property in the parent class.</param>
	/// <param name="type">The data type of the parent class of the property.</param>
	public static string GetPropertyDisplayName(this Type type, string propertyName)
	{
		var property = type.GetProperty(propertyName);

		if (property == null)
			return propertyName;

		return property.GetPropertyDisplayName();
	}

	/// <summary>
	/// Returns the <see cref="DisplayAttribute.Description"/> value of a provided property if found.
	/// </summary>
	/// <param name="property">The property to find the description of.</param>
	public static string? GetPropertyDisplayDescription(this PropertyInfo property)
	{
		return property.GetCustomAttribute<DisplayAttribute>()?.GetDescription();
	}

	/// <summary>
	/// Returns the <see cref="DisplayAttribute.Description"/> value of a provided property if found.
	/// </summary>
	/// <typeparam name="TModel">The type of the property.</typeparam>
	/// <param name="property">The property to find the description of.</param>
	public static string? GetPropertyDisplayDescription<TModel>(this Expression<Func<TModel>> property)
	{
		return property.GetPropertyAttribute<TModel, DisplayAttribute>()?.GetDescription();
	}

	/// <summary>
	/// Returns the display description of a provided property if found.
	/// </summary>
	/// <param name="propertyName">The description of the property in the parent class.</param>
	/// <param name="type">The data type of the parent class of the property.</param>
	public static string? GetPropertyDisplayDescription(this Type type, string propertyName)
	{
		var property = type.GetProperty(propertyName);

		if (property == null)
			return null;

		return property.GetPropertyDisplayDescription();
	}

	/// <summary>
	/// Returns the <see cref="DisplayAttribute.Name"/> value of a provided property value if found.
	/// </summary>
	/// <typeparam name="TModel">The type of the property.</typeparam>
	/// <param name="value">The value to find the name of.</param>
	public static string GetValueDisplayName<TModel>(this TModel value)
	{
		return value.GetValueAttribute<TModel, DisplayAttribute>()?.GetName() ?? value?.ToString() ?? "";
	}

	/// <summary>
	/// Returns the matching attribute from the specified type if found.
	/// </summary>
	/// <typeparam name="TAttribute">The datatype of the attribute to return.</typeparam>
	/// <param name="type">The type to check.</param>
	public static TAttribute? GetTypeAttribute<TAttribute>(this Type type) where TAttribute : Attribute
	{
		return (TAttribute?)Attribute.GetCustomAttribute(type, typeof(TAttribute));
	}

	/// <summary>
	/// Returns the matching attribute from the specified property if found.
	/// </summary>
	/// <typeparam name="TModel">The datatype of the property to check.</typeparam>
	/// <typeparam name="TAttribute">The datatype of the attribute to return.</typeparam>
	/// <param name="property">The property to check.</param>
	public static TAttribute? GetPropertyAttribute<TModel, TAttribute>(this Expression<Func<TModel>> property) where TAttribute : Attribute
	{
		var expression = (MemberExpression)property.Body;
		var attribute = expression.Member.GetCustomAttribute(typeof(TAttribute)) as TAttribute;

		return attribute;
	}

	/// <summary>
	/// Returns the matching attribute from the specified value if found.
	/// </summary>
	/// <typeparam name="TModel">The datatype of the property to check.</typeparam>
	/// <typeparam name="TAttribute">The datatype of the attribute to return.</typeparam>
	/// <param name="value">The value to check.</param>
	public static TAttribute? GetValueAttribute<TModel, TAttribute>(this TModel value) where TAttribute : Attribute
	{
		var memberName = value?.ToString();

		if (value == null || string.IsNullOrWhiteSpace(memberName))
			return null;

		var attribute = value.GetType()
			.GetMember(memberName)
			.First()
			.GetCustomAttribute<TAttribute>();

		return attribute;
	}
}
