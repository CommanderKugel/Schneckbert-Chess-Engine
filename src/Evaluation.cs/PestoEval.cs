

using System.Globalization;

public static class Eval
{
    static int[] gamephaseInc = { 0, 1, 1, 2, 4, 0 };
    static Random rng = new Random();

    public static int PestoEval (Board board)
    {
        int eg = 0;
        int mg = 0;
        int gamephase = 0;

        ulong bb;
        int pieceIndex;

        for (int piece=0; piece<6; piece++)
        {
            bb = board.allBitboards[0][piece];
            while (bb != 0)
            {
                pieceIndex = Helper.popLSB(ref bb);
                mg -= PSqT.mgTables[0][piece][pieceIndex];
                eg -= PSqT.egTables[0][piece][pieceIndex];
                gamephase += gamephaseInc[piece];
            }
            bb = board.allBitboards[1][piece];
            while (bb != 0)
            {
                pieceIndex = Helper.popLSB(ref bb);
                mg += PSqT.mgTables[1][piece][pieceIndex ^ 56];
                eg += PSqT.egTables[1][piece][pieceIndex ^ 56];
                gamephase += gamephaseInc[piece];
            }
        }

        gamephase = Math.Max(gamephase, 24);

        int mobility = Mobility.knightMovesCount(board) +
                       Mobility.diagonalMovesCount(board) +
                       Mobility.orthogonalMovesCount(board);

        return mobility + (mg * gamephase + eg * (24 - gamephase)) / (board.isWhiteToMove ? 24 : -24);
    }

}
