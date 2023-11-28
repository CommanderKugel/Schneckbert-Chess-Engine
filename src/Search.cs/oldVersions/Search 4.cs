
using System.Diagnostics;

//
// fixed depth search
// negamaxframework with alpha beta pruning
// Repitition, check- & stalemate detection
// Basic Eval Function
// basic q-search
//
// NEW STUFF:
// iterative Deepening (but still fixed depth)
//
// WDL vs. Search2: 171+ 658= 171-
// time: 500
//

public class Search_4 : Search
{
    public override string ToString() { return "Search_4"; }
    
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

        startPly = board.plyCount;
        int score;
        
        globalBestScore = -CHECKMATE;
        globalBestMove = Move.nullMove;

        for (int depth=1; depth<3; depth++)
        {
            
            score = -negaMax(-CHECKMATE, CHECKMATE, depth);

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

                if (score > alpha) 
                {
                    alpha = score;
                    if (isRoot)
                    {
                        globalBestScore = score;
                        globalBestMove = move;
                    }
                }
            }
        }
        return localBestScore;
    }

    private int qSearch(int alpha, int beta, int depth)
    {
        int standPat = simpleEval.Eval(board);
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
