
using System.Numerics;

public static class simpleEval
{

    private static readonly int[] pieceValues = { 100, 320, 330, 500, 900, 0 };

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

        return eval * (board.isWhiteToMove ? -1 : 1);
    }



    public static int kingEval (Board board)
    {
 
        int score = 0;

        ulong bb;
        int pieceIndex;
        int tableIndex;

        ulong pawnBlocker = board.allBitboards[0][1] | board.allBitboards[1][1];
        ulong mask;

        ulong enemyKingZone;
        int valueOfAttack;
        int attackingPieceCount;
        int attackScore;

        for (int piece=1; piece<=6; piece++)
        {
            for (int color=0; color<2; color++)
            {
                valueOfAttack = 0;
                attackingPieceCount = 0;
                enemyKingZone = board.allBitboards[1 - color][(int) PieceType.King];

                // piece in [1, 6]
                tableIndex = piece-1;
                bb = board.allBitboards[color][piece];
                while (bb != 0)
                {
                    pieceIndex = color==1 ? Helper.popLSB(ref bb) : Helper.popLSB(ref bb) ^ 56;
                    
                    switch (piece)
                    {
                        case 2: mask = PrecomputedData.KnightAttacks[pieceIndex] & enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 2;
                                }
                                break;

                        case 3: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker) & enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 2;
                                }
                                break;

                        case 4: mask = MoveGenLegal.orthogonalAttacks(piece, pawnBlocker) & enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 4;
                                }
                                break;

                        case 5: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker) |
                                       MoveGenLegal.orthogonalAttacks(piece, pawnBlocker);
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 8;
                                }
                                break;                                
                    }
                    
                    score += simpleTables.PSqT[tableIndex][pieceIndex];
                }

                if (attackingPieceCount > 7) attackingPieceCount = 7;
                attackScore = valueOfAttack * attackWeight[attackingPieceCount] / 100;
                score += attackScore;

                score = -score;
            }
        }

        return score * (board.isWhiteToMove ? -1 : 1);
    }
    private static readonly int[] attackWeight = { 0, 0, 50, 75, 88, 94, 97, 99 };

}