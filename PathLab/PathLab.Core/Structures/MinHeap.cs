namespace PathLab.Core.Structures;

/// <summary>
/// A binary min-heap: it always hands out the smallest element first.
/// </summary>
/// <typeparam name="T">The type of the elements stored in the heap.</typeparam>
/// <remarks>
/// It is the priority queue behind Dijkstra and A*, written by hand on top of a list.
/// Inserting and extracting both take O(log n), and checking whether it is empty is O(1).
/// The order is not stable: elements that compare as equal can come out in any order.
/// </remarks>
public class MinHeap<T>
{
    private readonly List<T> _elements;
    private readonly IComparer<T> _comparer;

    /// <summary>
    /// Creates an empty heap.
    /// </summary>
    /// <param name="comparer">
    /// Decides which element is the smallest: the one for which <c>Compare</c> returns the
    /// lower value comes out first. A* uses it to order its cells by cost.
    /// </param>
    public MinHeap(IComparer<T> comparer)
    {
        _elements = new List<T>();
        _comparer = comparer;
    }

    /// <summary>Whether the heap holds no elements.</summary>
    public bool IsEmpty => _elements.Count == 0;

    /// <summary>
    /// Adds an element to the heap.
    /// </summary>
    /// <param name="element">The element to add. Repeated elements are allowed.</param>
    public void Insert(T element)
    {
        _elements.Add(element);
        int position = _elements.Count - 1;

        while (position > 0 &&  _comparer.Compare(_elements[Parent(position)], _elements[position]) > 0)
        {
            position = Swap(Parent(position), position);
        }
    }

    /// <summary>
    /// Removes the smallest element and returns it.
    /// </summary>
    /// <returns>The smallest element, according to the comparer.</returns>
    /// <exception cref="InvalidOperationException">
    /// The heap is empty. Check <see cref="IsEmpty"/> first when that can happen.
    /// </exception>
    public T ExtractMin()
    {
        if (IsEmpty)
            throw new InvalidOperationException("El heap está vacío");

        int position = 0;
        T min = _elements[position];

        if (_elements.Count == 1)
        {
            _elements.RemoveAt(position);
            return  min;
        }

        _elements[position] = _elements[_elements.Count - 1];
        _elements.RemoveAt(_elements.Count - 1);

        while (true)
        {
            int smallest = position;
            int left = LeftChild(position);
            int right = RightChild(position);

            if (left < _elements.Count && _comparer.Compare(_elements[left], _elements[smallest]) < 0)
                smallest = left;
            if (right < _elements.Count && _comparer.Compare(_elements[right], _elements[smallest]) < 0)
                smallest = right;

            if (smallest == position) break;

            position = Swap(smallest, position);
        }

        return min;
    }

    private int Swap(int otherPosition, int actualPosition)
    {
        T aux = _elements[actualPosition];
        _elements[actualPosition] = _elements[otherPosition];
        _elements[otherPosition] = aux;
        return otherPosition;
    }
    private int LeftChild(int i) => 2 * i + 1;
    private int RightChild(int i) => 2 * i + 2;
    private int Parent(int i) => (i - 1) / 2;
}
