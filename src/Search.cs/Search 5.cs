
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
// iterative Deepening (no fixed depth)
// research best move last iter first
// always return best move from last full iteration
//
// WDL vs. Search4: 418+ 546= 36-
// time: 300
//

public class Search_5 : Search
{
    public override string ToString() { return "Search_5"; }
    
    int CHECKMATE = 30_000_000;
    Stopwatch watch = new Stopwatch();
    Board board;

    int startPly;
    int globalBestScore;
    Move globalBestMove;
    long timeControl;

    public override Move Think(Board board, long timeControl)
    {
        watch.Reset();
        watch.Start();

        this.board = board;
        this.timeControl = timeControl;

        startPly = board.plyCount;
        int score;
        
        globalBestScore = -CHECKMATE;
        globalBestMove = Move.nullMove;
        Move bestMoveLastIteration = Move.nullMove;

        for (int depth=1; depth<10; depth++)
        {
            bestMoveLastIteration = globalBestMove;
            
            score = -negaMax(-CHECKMATE, CHECKMATE, depth);
            if (watch.ElapsedMilliseconds > timeControl) break;
        }

        watch.Stop();

        return bestMoveLastIteration;
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
