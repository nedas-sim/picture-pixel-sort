namespace Models.Array;

public class ArrayRAM<T> : IArrayHandler<T>
{
    T[] _array;
    int _count;

    public ArrayRAM(int size)
    {
        _array = new T[size];
        _count = 0;
    }

    public void Add(T data)
    {
        if (_count < _array.Length)
        {
            _array[_count] = data;
            _count++;
        }
    }

    public T Get(int index)
    {
        return _array[index];
    }

    public T[] GetArray()
    {
        return _array;
    }

    public void Replace(int index, T data)
    {
        _array[index] = data;
    }

    public void SetArray(T[] array)
    {
        _array = array;
    }

    public int Size()
    {
        return _array.Length;
    }

    public void Swap(int index1, int index2)
    {
        (_array[index1], _array[index2]) = (_array[index2], _array[index1]);
    }
}
