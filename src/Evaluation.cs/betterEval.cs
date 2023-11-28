using System.Collections;
using System.Drawing;
using System.Numerics;


public static class betterEval
{
    private static readonly int[] pieceValues = { 0, 100, 320, 330, 500, 900, 20_000 };
    private static readonly int[] attackValue = { 0, 0, 50, 75, 88, 94, 97, 99, 100 };

    public static int eval(Board board)
    {
        int score = 0;

        ulong blocker = board.allBitboards[0][1] | board.allBitboards[1][1];

        for (int us=0; us<2; us++, score=-score)
        {
            ulong mask;
            ulong bb;
            int them = 1-us;

            int pieceIndex = BitOperations.TrailingZeroCount(board.allBitboards[them][(int)PieceType.King]);
            ulong enemyKingZone = PrecomputedData.KingAttacks[pieceIndex];
            int attackerCount = 0;
            int attackWeight = 0;

            bool noEnemyQueen = BitOperations.PopCount(board.allBitboards[them][5]) == 0;

            for (int piece=1; piece<7; piece++)
            {
                bb = board.allBitboards[us][piece];
                while (bb!=0)
                {
                    pieceIndex = (us==1) ? Helper.popLSB(ref bb) : 63 - Helper.popLSB(ref bb);
                    score += pieceValues[piece];
                    score += simpleTables.PSqT[piece][pieceIndex];

                    switch (piece)
                    {
                        case 1: 
                                break;

                        case 2: mask = PrecomputedData.KnightAttacks[pieceIndex];
                                score += BitOperations.PopCount(mask & ~blocker);
                                mask &= enemyKingZone;
                                if (mask!=0)
                                {
                                    attackerCount++;
                                    attackWeight += BitOperations.PopCount(mask) * 5;
                                }
                                break;

                        case 3: mask = MoveGenLegal.diagonalAttacks(pieceIndex, blocker);
                                score += BitOperations.PopCount(mask);
                                mask &= enemyKingZone;
                                if (mask!=0)
                                {
                                    attackerCount++;
                                    attackWeight += BitOperations.PopCount(mask) * 5;
                                }
                                break;

                        case 4: mask = MoveGenLegal.orthogonalAttacks(pieceIndex, blocker);
                                score += BitOperations.PopCount(mask);
                                mask &= enemyKingZone;
                                if (mask!=0)
                                {
                                    attackerCount++;
                                    attackWeight += BitOperations.PopCount(mask) * 10;
                                }
                                break;

                        case 5: mask = MoveGenLegal.orthogonalAttacks(pieceIndex, blocker) |
                                       MoveGenLegal.diagonalAttacks(pieceIndex, blocker);
                                score += BitOperations.PopCount(mask);
                                mask &= enemyKingZone;
                                if (mask!=0)
                                {
                                    attackerCount++;
                                    attackWeight += BitOperations.PopCount(mask) * 15;
                                }
                                break;

                        case 6: if (noEnemyQueen) score += simpleTables.PSqT[6][pieceIndex]-simpleTables.PSqT[5][pieceIndex];
                                score += BitOperations.PopCount(PrecomputedData.KingAttacks[pieceIndex]);
                                break;
                    }
                }
            }
            if (attackerCount>8) attackerCount=8;
            score += attackWeight * attackValue[attackerCount] / 100;
        }

        return score * (board.isWhiteToMove ? -1 : 1);
    }

}