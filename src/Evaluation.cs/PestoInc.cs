
using System.ComponentModel.DataAnnotations;

public class PestoInc
{
    private static readonly int[] gamephaseInc = { 0, 1, 1, 2, 4, 0 };
    private const int Length = 256;
    private pestoEvalPiece[] evalStack = new pestoEvalPiece[Length];

    public PestoInc(Board board)
    {
        init(board);
    }

    public void init(Board board)
    {
        evalStack =  new pestoEvalPiece[Length];

        int eg = 0;
        int mg = 0;
        int gamephase = 0;

        ulong bb;
        int pieceIndex;
        int tableIndex;

        for (int color=0; color<2; color++)
        {
            // convention: 0=none, 1=pawn, 2=knight, 3=bishop, 4=rook, 5=queen, 6=king
            for (int piece=1; piece<=6; piece++)
            {
                tableIndex = piece-1;
                bb = board.allBitboards[color][piece];
                while (bb != 0)
                {
                    pieceIndex = color==1 ? Helper.popLSB(ref bb) : Helper.popLSB(ref bb) ^ 56;
                    mg += PestoTables.mgTables[color][tableIndex][pieceIndex];
                    eg += PestoTables.egTables[color][tableIndex][pieceIndex];
                    gamephase += gamephaseInc[tableIndex];
                }
            }
            mg=-mg;
            eg=-eg;
        }

        pestoEvalPiece first = new pestoEvalPiece(mg, eg, gamephase);
        evalStack[board.plyCount % Length] = first;
    }
    
    public void push(Board board, Move move)
    {
        int us = board.isWhiteToMove ? 1 : 0;
        int them = board.isWhiteToMove ? 1 : 0;

        int movingPiece = (int) board.pieceLookup[move.from];
        int capturedPiece = (int) board.pieceLookup[move.to];
        pestoEvalPiece oldEval = evalStack[board.plyCount % Length];
        
        int mg_psqtVal;
        int eg_psqtVal;
        int phaseWeight = oldEval.phaseWeight;

        mg_psqtVal = PestoTables.mgTables[us][movingPiece-1][move.to];
        mg_psqtVal -= PestoTables.mgTables[us][movingPiece-1][move.from];
        eg_psqtVal = PestoTables.egTables[us][movingPiece-1][move.to];
        eg_psqtVal -= PestoTables.egTables[us][movingPiece-1][move.from];

        if (capturedPiece != 0) 
        {
            mg_psqtVal += PestoTables.mgTables[them][capturedPiece-1][move.to];
            eg_psqtVal += PestoTables.egTables[them][capturedPiece-1][move.from];
            phaseWeight -= gamephaseInc[capturedPiece];
        }

        if (board.isWhiteToMove)
        {
            mg_psqtVal = oldEval.mg_psqtVal - mg_psqtVal;
            eg_psqtVal = oldEval.eg_psqtVal - eg_psqtVal;
        }
        else
        {
            mg_psqtVal += oldEval.mg_psqtVal;
            eg_psqtVal += oldEval.eg_psqtVal;
        }

        /*
        !!! INCREMENT PHASEWEIGHT IN CASE OF PROMOTION !!!
        */

        pestoEvalPiece newEval = new pestoEvalPiece(mg_psqtVal, eg_psqtVal, phaseWeight);
        evalStack[board.plyCount % Length] = newEval;
    }  


    public int Eval(Board board)
    {
        pestoEvalPiece currentEvalPiece = evalStack[board.plyCount % Length];
        int phase = (currentEvalPiece.phaseWeight < 24) ? currentEvalPiece.phaseWeight : 24;
        return (currentEvalPiece.mg_psqtVal * phase + currentEvalPiece.eg_psqtVal * (24 - phase)) / (board.isWhiteToMove ? -24 : 24);
    }

}

public struct pestoEvalPiece
{
    public int mg_psqtVal;
    public int eg_psqtVal;
    public int phaseWeight;

    public pestoEvalPiece(int mg_psqtVal, int eg_psqtVal, int phaseWeight)
    {
        this.mg_psqtVal = mg_psqtVal;
        this.eg_psqtVal = eg_psqtVal;
        this.phaseWeight = phaseWeight;
    }    
}
