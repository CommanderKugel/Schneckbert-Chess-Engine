
public struct Transposition
{

    public ulong zobristKey;
    public Move move;

    public int depth;
    public int score;
    public byte flag;

    public Transposition(ulong zobristKey, Move move, int depth, int score, int flag)
    {
        this.zobristKey = zobristKey;
        this.move = move;
        this.depth = depth;
        this.score = score;
        this.flag = (byte) flag;
    }
}


