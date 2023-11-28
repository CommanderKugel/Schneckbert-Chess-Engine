
using System.Diagnostics;

//
// fixed depth search
// negamaxframework with alpha beta pruning
// Repitition, check- & stalemate detection
// basic q-search
//
// NEW STUFF:
// replaced basic Eval with Pesto
// added basicmobility to Pesto
//
// Search3 - mobility vs. Search2: 125+ 575= 300-
// Search3 + mobility vs. Search2: 134+ 554= 312-
//
// Simple Eval ftw lol
//

public class Search_3_both : Search
{
    public override string ToString() { return "Search_3_both"; }
    
    int CHECKMATE = 30_000_000;
    Stopwatch watch = new Stopwatch();
    Board board;

    int startPly;
    int globalBestScore;
    Move globalBestMove;
    long timeControl;

    public override Move Think(Board board, long timeControl)
    {
        //watch.Reset();
        //watch.Start();

        this.board = board;
        this.timeControl = timeControl;
        globalBestScore = -CHECKMATE;

        Move[] moves = board.generateLegalMoves();
        globalBestMove = moves[0];

        startPly = board.plyCount;
        int depth = 2;
        int score;

        for (int i=0; i<moves.Length; i++)
        {
            board.makeMove(moves[i]);
            score = -negaMax(-CHECKMATE, CHECKMATE, depth-1);
            board.undoMove(moves[i]);

            if (score > globalBestScore)
            {
                globalBestScore = score;
                globalBestMove = moves[i];
            }
        }

        //while (watch.ElapsedMilliseconds < timeControl);
        //watch.Stop();

        return globalBestMove;
    }

    private int negaMax(int alpha, int beta, int depth)
    {
        bool isRoot = board.plyCount == startPly;

        if (board.repititionTable.isRepeatedPosition()) return -10;

        if (depth == 0) return qSearch(alpha, beta, depth);

        Move[] moves = board.generateLegalMoves();
        if (moves.Length == 0) return board.isInCheck ? board.plyCount-CHECKMATE : 0;

        int score;
        int localBestScore = -CHECKMATE;
        foreach (Move move in moves)
        {
            board.makeMove(move);
            score = -negaMax(-beta, -alpha, depth-1);
            board.undoMove(move);
            
            if (score >= beta) return beta;

            if (score > localBestScore)
            {
                localBestScore = score;
                if (score > alpha) alpha = score;
            }
        }
        return localBestScore;
    }

    private int qSearch(int alpha, int beta, int depth)
    {
        int standPat = PestoEval.FullEval(board);
        Move[] moves = board.generateLegalMoves(true);
        if (moves.Length == 0) 
            return board.isInCheck ? board.plyCount - CHECKMATE : 
                                     standPat;
        
        if (standPat >= beta) return beta;
        if (standPat > alpha) alpha = standPat;
        
        if (depth <= -8) return standPat;
        
        int score = standPat;
        foreach (Move move in moves)
        {
            board.makeMove(move);
            score = -qSearch(-beta, -alpha, depth-1);
            board.undoMove(move);

            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }
        return alpha;
    }

}
