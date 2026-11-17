namespace PathLab.Core.Tests;

[TestClass]
public class MinHeapTests
{
    [TestMethod]
    public void ExtractMin_WithRandomValues_ReturnsThemSorted()
    {
        Random random = new Random(42);

        MinHeap<int> heap = new MinHeap<int>(Comparer<int>.Default);
        List<int> expected = new List<int>();

        for (int i = 0; i < 100; i++)
        {
            int value = random.Next();
            expected.Add(value);
            heap.Insert(value);
        }

        expected.Sort();

        List<int> actual = new List<int>();
        while (!heap.IsEmpty)
        {
            actual.Add(heap.ExtractMin());
        }

        CollectionAssert.AreEqual(expected, actual);
    }
    
    [TestMethod]
    public void ExtractMin_OnEmptyHeap_ThrowsException()
    {
        MinHeap<int> heap = new MinHeap<int>(Comparer<int>.Default);
        Assert.ThrowsExactly<InvalidOperationException>(() => heap.ExtractMin());
    }

    [TestMethod]
    public void IsEmpty_BeforeAndAfterInsert()
    {
        MinHeap<int> heap = new MinHeap<int>(Comparer<int>.Default);
        Assert.IsTrue(heap.IsEmpty);
        
        heap.Insert(1);
        Assert.IsFalse(heap.IsEmpty);

        heap.ExtractMin();
        Assert.IsTrue(heap.IsEmpty);
    }
}
