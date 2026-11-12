namespace PathLab.Core;

public class MinHeap<T>
{
    private readonly List<T> _elements;
    private readonly IComparer<T> _comparer;

    public MinHeap(IComparer<T> comparer)
    {
        _elements = new List<T>();
        _comparer = comparer;
    }

    public bool IsEmpty => _elements.Count == 0;

    public void Insert(T element)
    {
        throw new NotImplementedException();
    }

    public T ExtractMin()
    {
        throw new NotImplementedException();
    }
}
