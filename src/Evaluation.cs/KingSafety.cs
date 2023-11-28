
using System.Numerics;

public static class KingSafety
{
    const int white = 1;
    const int black = 0;
    const ulong notCentralFiles = 0xE7E7E7E7E7E7E7E7;

    public static int pawnShield (Board board)
    {
        int score = 0;
        int kingIndex;
        ulong pawnMask;
        // white Pawn shield
        kingIndex = BitOperations.TrailingZeroCount(board.allBitboards[white][(int) PieceType.King]);
        pawnMask =  PrecomputedData.KingAttacks[kingIndex] << 8 &
                    PrecomputedData.KingAttacks[kingIndex] << 16 &
                    board.allBitboards[white][(int) PieceType.Pawn] &
                    notCentralFiles;
        score += BitOperations.PopCount(pawnMask) * 30 - 45;
        // black Pawn shield
        kingIndex = BitOperations.TrailingZeroCount(board.allBitboards[black][(int) PieceType.King]);
        pawnMask =  PrecomputedData.KingAttacks[kingIndex] >> 8 &
                    PrecomputedData.KingAttacks[kingIndex] >> 16 &
                    board.allBitboards[black][(int) PieceType.Pawn] &
                    notCentralFiles;
        score -= BitOperations.PopCount(pawnMask) * 30 - 45;

        return score;
    }


    // https://www.chessprogramming.org/King_Safety -> Attacking King Zone
    public static int attackKingZone (int us, int kingIndex, Board board)
    {
        int attackingPieceCount=0;
        int valueOfAttack=0;

        // us is attacking them -> enemy king zone
        ulong kingZone = PrecomputedData.KingAttacks[kingIndex];
        kingZone |= us==0 ? kingZone << 8 : kingZone >> 8;

        ulong relevantPieces;
        ulong mask;
        int pieceIndex;
        // knight attacks
        relevantPieces = board.allBitboards[us][(int) PieceType.Knight];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = PrecomputedData.KnightAttacks[pieceIndex] & kingZone;
            if (mask != 0)
            {
                attackingPieceCount++;
                valueOfAttack += BitOperations.PopCount(mask) * 20;
            }
        }
        
        ulong blocker = board.allBitboards[0][(int) PieceType.Pawn] | board.allBitboards[1][(int) PieceType.Pawn];
       
        // bishop attacks
        relevantPieces = board.allBitboards[us][(int) PieceType.Bishop];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = MoveGenLegal.diagonalAttacks(pieceIndex, blocker) & kingZone;
            if (mask != 0)
            {
                attackingPieceCount++;
                valueOfAttack += BitOperations.PopCount(mask) * 20;
            }
        }
        // rook attacks
        relevantPieces = board.allBitboards[us][(int) PieceType.Rook];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = MoveGenLegal.orthogonalAttacks(pieceIndex, blocker) & kingZone;
            if (mask != 0)
            {
                attackingPieceCount++;
                valueOfAttack += BitOperations.PopCount(mask) * 40;
            }
        }
        // queen attacks
        relevantPieces = board.allBitboards[us][(int) PieceType.Queen];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = (MoveGenLegal.orthogonalAttacks(pieceIndex, blocker) | 
                    MoveGenLegal.diagonalAttacks(pieceIndex, blocker)) & 
                    kingZone;
            if (mask != 0)
            {
                attackingPieceCount++;
                valueOfAttack += BitOperations.PopCount(mask) * 80;
            }
        }
        if (attackingPieceCount > 7) attackingPieceCount=7;

        return valueOfAttack * attackWeight[attackingPieceCount] / 100;
    }
    private static readonly int[] attackWeight = { 0, 50, 75, 88, 94, 97, 99, 100 };

}
