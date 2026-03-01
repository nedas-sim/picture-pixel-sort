namespace MergeSortFile;

internal class FileClass
{
    private BinaryReader Reader;
    private BinaryWriter Writer;

    public void WriteByte(string file, byte value)
    {
        using (Writer = new BinaryWriter(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write)))
        {
            Writer.Seek(0, SeekOrigin.End);
            Writer.Write(value);
            Writer.Close();
        }
    }

    public byte ReadByte(string file, int index)
    {
        using (Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
        {
            Reader.BaseStream.Seek(index, SeekOrigin.Begin);
            byte value = Reader.ReadByte();
            Reader.Close();
            return value;
        }
    }

    public void Write(string file, int value)
    {
        using (Writer = new BinaryWriter(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write)))
        {
            byte[] values = BitConverter.GetBytes(value);
            Writer.Seek(0, SeekOrigin.End);
            Writer.Write(values, 0, 3);
            Writer.Close();
        }
    }

    public int Read(string file, int index)
    {
        byte r = ReadByte(file, 3 * index + 2);
        byte g = ReadByte(file, 3 * index + 1);
        byte b = ReadByte(file, 3 * index);

        return (((r << 8) + g) << 8) + b;
    }

    public int ByteLength(string file)
    {
        using (Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
        {
            int length = (int)Reader.BaseStream.Length;
            Reader.Close();
            return length;
        }
    }

    public int Length(string file)
    {
        return ByteLength(file) / 3;
    }

    public void Replace(string file, int value, int index)
    {
        using (Writer = new BinaryWriter(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write)))
        {
            byte[] array = BitConverter.GetBytes(value);

            Writer.Seek(3 * index, SeekOrigin.Begin);
            Writer.Write(array, 0, 3);

            Writer.Close();
        }
    }

    public string Clone(string fileToCopy)
    {
        string newFile = Program.saveDir + Path.GetFileNameWithoutExtension(fileToCopy) + "New.txt";
        for (int i = 0; i < ByteLength(fileToCopy); i++)
        {
            byte value = ReadByte(fileToCopy, i);
            WriteByte(newFile, value);
        }

        return newFile;
    }
}
