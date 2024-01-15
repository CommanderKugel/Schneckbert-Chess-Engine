
public class RepititionTable
{
    const int length = 0xFF;


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
        table[++index & 0x7F] = zobristKey;
    }

    public ulong pop()
    {
        ulong ret = table[index & 0x7F];
        index--;
        return ret;
    }

    public void Reset()
    {
        index = 0;
    }

    public bool isRepeatedPosition()
    {
        ulong currentZobrist = table[index & 0x7F];
        for (int i=(index-1) & 0x7F; i>=0; i--)
        {
            if (table[i] == currentZobrist) 
                return true;
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
