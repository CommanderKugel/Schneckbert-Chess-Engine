
using static System.Math;

public static class Pesto
{

    public static int Eval(Board board)
    {
        int[] gamephaseInc = { 0, 1, 1, 2, 4, 0 };

        int mg = 0;
        int eg = 0;
        int gp = 0;

        for (int piece=0; piece<6; piece++)
        {
            ulong bb = board.allBitboards[1][piece+1];
            while (bb != 0)
            {
                int sq = Helper.popLSB(ref bb);
                mg += PestoTables.mgTables[piece][sq];
                eg += PestoTables.egTables[piece][sq];            
                gp += gamephaseInc[piece];
            }

            bb = board.allBitboards[0][piece+1];
            while (bb != 0)
            {
                int sq = Helper.popLSB(ref bb);
                sq ^= 56;   // flip verticly because blacks point of view
                mg -= PestoTables.mgTables[piece][sq];
                eg -= PestoTables.egTables[piece][sq];
                gp += gamephaseInc[piece];
            }
        }
        gp = Max(gp, 24);

        return (mg * gp + eg * (24 - gp)) / (board.isWhiteToMove ? 24 : -24);
    }

}
