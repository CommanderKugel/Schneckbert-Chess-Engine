using System.Numerics;

public enum PieceType
{
    None,
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum nodeType
{
    lowerBound,
    exact,
    upperBound
}


public enum gameResult 
{
    ongoing,
    blackWon,
    draw,
    whiteWon
}

public static class Helper
{
    public static int popLSB(ref ulong num) 
    {
        int index = BitOperations.TrailingZeroCount(num);
        num &= num - 1;
        return index;
    }

    public static void init()
    {
        Magic.initMagic();
        PrecomputedData.initCheckMasks();
        Zobrist.initZobrist(1);
        PestoTables.initTables();
    }
}
