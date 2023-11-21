
using System.Numerics;

public static class simpleEval
{

    public static readonly int[] pieceValues = { 100, 320, 330, 500, 900, 0 };

    public static int Eval(Board board)
    {
        int eval = 0;

        bool noQueens = BitOperations.PopCount(board.allBitboards[0][5] | board.allBitboards[1][5]) == 0;

        for (int color=0; color<2; color++, eval = -eval)
        {
            for (int piece=0; piece<6; piece++)
            {
                ulong bb = board.allBitboards[color][piece+1];
                while (bb != 0)
                {
                    int index = color==1 ? Helper.popLSB(ref bb) : 63 - Helper.popLSB(ref bb);
                    
                    // piece Value
                    eval += pieceValues[piece];
                    
                    // PSqT Value
                    if (piece == 5) 
                    {
                        if (noQueens) eval += simpleTables.PSqT[6][index];  // King eg table
                        else          eval += simpleTables.PSqT[5][index];  // king mg table
                    }
                    else eval += simpleTables.PSqT[piece][index];
                    
                }
            }
        }

        eval += Mobility.knightMovesCount(board);
        eval += Mobility.diagonalMovesCount(board);
        eval += Mobility.orthogonalMovesCount(board);

        return eval * (board.isWhiteToMove ? -1 : 1);
    }

}