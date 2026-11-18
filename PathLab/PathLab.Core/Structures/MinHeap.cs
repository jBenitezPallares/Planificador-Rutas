namespace PathLab.Core.Structures;

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
        _elements.Add(element);
        int position = _elements.Count - 1;
        
        while (position > 0 &&  _comparer.Compare(_elements[Parent(position)], _elements[position]) > 0)
        {
            position = Swap(Parent(position), position);
        }
    }
    
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
