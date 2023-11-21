

public class RepititionTable
{

    const int length = 256;
    const int hashMod = length - 1;

    public ulong[] table;
    public int index;

    public RepititionTable()
    {
        table = new ulong[length];
        index = 0;
    }

    public void Clear()
    {
        table = new ulong[length];
        index = 0;
    }
    
    public void push(ulong zobristKey)
    {
        table[++index % length] = zobristKey;
    }

    public ulong pop()
    {
        ulong ret = table[index % length];
        index--;
        return ret;
    }

    public void Reset()
    {
        index = 0;
    }

    public bool isRepeatedPosition()
    {
        ulong currentZobrist = table[index % length];
        for (int i=(index-1) % length; i>=0; i--)
        {
            if (table[i] == currentZobrist) return true;
        }
        return false;
    }

    public void printTable()
    {
        foreach (ulong elem in table)
        {
            if (elem != 0) Console.Write(elem.ToString("X")+", ");
        }
        Console.WriteLine("");
    }

}
