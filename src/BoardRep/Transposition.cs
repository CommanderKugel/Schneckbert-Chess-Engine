
public struct Transposition
{
    public ulong zobristKey;    // 64 bit
    public Move move;           // 16 bit
    public short depth;         // 16 bit
    public byte score;          // 8 bit
    public byte flag;           // 8  bit
                                // -> 112 bit
                                // 0xFFFFF ~14.6 MB

    public Transposition(ulong zobristKey, Move move, short depth, byte score, byte flag)
    {
        this.zobristKey = zobristKey;
        this.move  = move;
        this.depth = depth;
        this.score = score;
        this.flag  = flag;
    }
}


