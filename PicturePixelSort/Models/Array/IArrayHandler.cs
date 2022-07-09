namespace Models.Array;

public interface IArrayHandler<T>
{
    void Add(T data);
    T Get(int index);
    void Replace(int index, T data);
    void Swap(int index1, int index2);
    T[] GetArray();
    void SetArray(T[] array);
    int Size();
}
