
using System.Diagnostics;

//
// negamaxframework with alpha beta pruning
// Repitition, check- & stalemate detection
// Basic Eval Function
// basic q-search
// iterative deepening
// return when time is up
//
// NEW STUFF:
// always returning global best move (also for incomplete iterations)
//
// WDL vs. Search5: 121+ 392= 73-
// time: 100
//

public class Search_6 : Search
{
    public override string ToString() { return "Search_6"; }
    
    int CHECKMATE = 30_000_000;
    Stopwatch watch = new Stopwatch();
    Board board;

    int startPly;
    int globalBestScore;
    Move globalBestMove;
    long timeControl;

    bool timeIsUp;

    public override Move Think(Board board, long timeControl)
    {
        watch.Reset();
        watch.Start();

        this.board = board;
        this.timeControl = timeControl;

        timeIsUp = false;
        startPly = board.plyCount;
        int score;
        
        globalBestScore = -CHECKMATE;
        globalBestMove = Move.nullMove;
        
        int depth=1;
        for (; depth<10; depth++)
        {            
            score = -negaMax(-CHECKMATE, CHECKMATE, depth);
            if (watch.ElapsedMilliseconds > timeControl) break;
        }

        watch.Stop();
        //Console.WriteLine("depth: "+depth);

        return globalBestMove;
    }

    private int negaMax(int alpha, int beta, int depth)
    {
        bool isRoot = board.plyCount == startPly;

        if (board.repititionTable.isRepeatedPosition()) return -10;

        if (depth == 0) return qSearch(alpha, beta, depth);

        Move[] moves = board.generateLegalMoves();
        if (moves.Length == 0) return board.isInCheck ? board.plyCount - CHECKMATE : 0;

        if (isRoot && globalBestMove != Move.nullMove) moves = moveOrdering(moves);

        int score;
        int localBestScore = -CHECKMATE;
        foreach (Move move in moves)
        {
            if (watch.ElapsedMilliseconds > timeControl && globalBestMove != Move.nullMove)
                return CHECKMATE;

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
        
        int score;
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

    private Move[] moveOrdering (Move[] moves)
    {
        for (int i=0; i<moves.Length; i++)
        {
            if (moves[i] == globalBestMove)
            {
                Move copy = moves[0];
                moves[0] = moves[i];
                moves[i] = copy;
            }
        }
        return moves;
    }

}
