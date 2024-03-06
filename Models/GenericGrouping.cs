using System.Collections;

namespace easy_core;

/// <summary>
/// Generic implementation of <see cref="IGrouping{TKey, TElement}"/> to facilitate serialization.
/// </summary>
public class GenericGrouping<TKey, TElement> : IGrouping<TKey, TElement>
{
	public GenericGrouping() { }

	/// <exception cref="ArgumentNullException"></exception>
	public GenericGrouping(IGrouping<TKey, TElement> grouping)
	{
		if (grouping == null)
			throw new ArgumentNullException(nameof(grouping));

		Key = grouping.Key;
		Elements = grouping.ToList();
	}

	/// <inheritdoc/>
	public TKey Key { get; set; }

	/// <summary>
	/// Contains the grouped objects.
	/// </summary>
	public List<TElement> Elements { get; set; }

	/// <inheritdoc/>
	public IEnumerator<TElement> GetEnumerator()
	{
		return Elements.GetEnumerator();
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
