

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
    
    public void push(ulong zobristKey)
    {
        table[index & hashMod] = zobristKey;
        index++;
    }

    public bool isRepeatedPosition()
    {
        int counter = 0;
        try
        {
            int i = index-2;
            while (i>=0)
            {
                if (table[i & hashMod] == table[index & hashMod]) counter++;
                i-=2;
            }
        }
        catch {
            Console.WriteLine(index);
        }
        return counter==2;
    }

    public void resetIndex(ulong zobristKey)
    {
        index = 0;
        table[index++] = zobristKey;
    }

}
