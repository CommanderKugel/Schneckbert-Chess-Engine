using System.ComponentModel;
using System.Linq.Expressions;
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

public enum castle
{
    kingSide,
    queenSide
}

public enum dir
{
    left,
    up,
    right
}

public static class Helper
{

    public static int popLSB(ref ulong num) {
        int index = BitOperations.TrailingZeroCount(num);
        num &= num - 1;
        return index;
    } 

    public class init
    {
        public init()
        {
            Magic.initMagic();
            PrecomputedData.initCheckMasks();
            Zobrist.initZobrist(1);
            PSqT.initTables();
        }
    }




   
    

}
