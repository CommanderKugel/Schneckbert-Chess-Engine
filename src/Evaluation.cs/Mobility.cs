

using System.Numerics;

public static class Mobility
{

    public static int knightMovesCount (Board board)
    {
        int score = 0;
        ulong pawns = board.allBitboards[0][(int) PieceType.Pawn] | board.allBitboards[1][(int) PieceType.Pawn];

        for (int us=0; us<2; us++, score = -score)
        {
            ulong bb = board.allBitboards[us][(int) PieceType.Knight];

            while (bb != 0)
            {
                int pieceIndex = Helper.popLSB(ref bb);
                ulong mask = PrecomputedData.KnightAttacks[pieceIndex];
                score += BitOperations.PopCount(mask & ~pawns);
            }
        }
        return score;
    }

    public static int diagonalMovesCount (Board board)
    {
        int score = 0;
        ulong pawns = board.allBitboards[0][(int) PieceType.Pawn] | board.allBitboards[1][(int) PieceType.Pawn];
        for (int us=0; us<2; us++, score = -score)
        {
            ulong bb = board.allBitboards[us][(int) PieceType.Bishop] | board.allBitboards[us][(int) PieceType.Queen];

            while (bb != 0)
            {
                int pieceIndex = Helper.popLSB(ref bb);
                ulong mask = MoveGenLegal.diagonalAttacks(pieceIndex, pawns);
                score += BitOperations.PopCount(mask);
            }
        }
        return score;
    }

    public static int orthogonalMovesCount (Board board)
    {
        int score = 0;
        ulong pawns = board.allBitboards[0][(int) PieceType.Pawn] | board.allBitboards[1][(int) PieceType.Pawn];
        for (int us=0; us<2; us++, score = -score)
        {
            ulong bb = board.allBitboards[us][(int) PieceType.Rook] | board.allBitboards[us][(int) PieceType.Queen];

            while (bb != 0)
            {
                int pieceIndex = Helper.popLSB(ref bb);
                ulong mask = MoveGenLegal.orthogonalAttacks(pieceIndex, pawns);
                score += BitOperations.PopCount(mask);
            }
        }
        return score;
    }


}
