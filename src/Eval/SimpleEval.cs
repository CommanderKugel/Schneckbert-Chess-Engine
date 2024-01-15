

public class Simple
{
    public static int Eval(Board board)
    {
        int[] pVal = { 0, 100, 300, 320, 500, 900, 20_000 };

        int eval = 0;
        ulong bb = board.allBitboards[1][0];
        while (bb != 0)
        {
            int sq = Helper.popLSB(ref bb);
            int piece = board.pieceLookup[sq];
            eval += pVal[piece];
        }
        bb = board.allBitboards[0][0];
        while (bb != 0)
        {
            int sq = Helper.popLSB(ref bb);
            int piece = board.pieceLookup[sq];
            eval -= pVal[piece];
        }
        return eval;
    }
}
