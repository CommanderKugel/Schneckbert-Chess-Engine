
using System.Diagnostics;

//
// negamaxframework with alpha beta pruning
// Repitition, check- & stalemate detection
// Basic Eval Function + basic mobility
// basic q-search

// iterative deepening
// return when time is up
// return best move of incomplete iterations
// TT Move - Move ordering
//
// NEW STUFF:
// TT move cutoff (& Move ordering)
//
// WDL vs. Search6: 218+ 638= 144-
// time: 100
//

public class Search_8 : Search
{
    public override string ToString() { return "Search_8"; }
    
    int CHECKMATE = 30_000_000;
    Stopwatch watch = new Stopwatch();
    Board board;

    int startPly;
    int globalBestScore;
    Move globalBestMove;
    long timeControl;

    bool timeIsUp;

    Transposition[] transpositionTable = new Transposition[0xFFFF];

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

        for (int depth=1; depth<10 && watch.ElapsedMilliseconds<timeControl; depth++)
        {            
            score = -negaMax(-CHECKMATE, CHECKMATE, depth);
        }

        watch.Stop();

        return globalBestMove;
    }

    private int negaMax(int alpha, int beta, int depth)
    {
        bool isRoot = board.plyCount == startPly;

        if (board.repititionTable.isRepeatedPosition()) return -10;
        if (depth == 0) return qSearch(alpha, beta, depth);


        Transposition entry = transpositionTable[board.currentGamestate.zobristKey % 0xFFFF];

        if (!isRoot && entry.zobristKey==board.currentGamestate.zobristKey && entry.depth >= depth && (
            entry.flag==2 && entry.score>=beta ||
            entry.flag==1 && entry.score<=alpha ||
            entry.flag==3
            )) return entry.score;


        Move[] moves = board.generateLegalMoves();
        if (moves.Length == 0) return board.isInCheck ? board.plyCount - CHECKMATE : 0;
        if (isRoot || entry.move is not null) moves = moveOrdering(moves, isRoot, entry);

        int score;
        int startAlpha = alpha;
        int localBestScore = -CHECKMATE;
        Move localBestMove = Move.nullMove;
        foreach (Move move in moves)
        {
            if (watch.ElapsedMilliseconds > timeControl && globalBestMove != Move.nullMove)
                return CHECKMATE;

            board.makeMove(move);
            score = -negaMax(-beta, -alpha, depth-1);
            board.undoMove(move);
            
            if (score > localBestScore)
            {
                localBestScore = score;
                localBestMove = move;

                if (score > alpha) 
                {
                    if (score >= beta) break;
                    
                    alpha = score;
                    if (isRoot)
                    {
                        globalBestScore = score;
                        globalBestMove = move;
                    }
                }
            }
        }

        int ttFlag = localBestScore >= beta ? 2 : localBestScore > startAlpha ? 3 : 1;

        transpositionTable[board.currentGamestate.zobristKey % 0xFFFF] 
            = new Transposition(board.currentGamestate.zobristKey,
                                localBestMove,
                                depth,
                                localBestScore,
                                ttFlag);

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

    private Move[] moveOrdering (Move[] moves, bool isRoot, Transposition entry)
    {
        Move firstMove = isRoot ? globalBestMove : entry.move;

        for (int i=0; i<moves.Length; i++)
        {
            if (moves[i] == firstMove)
            {
                Move copy = moves[0];
                moves[0] = moves[i];
                moves[i] = copy;
            }
        }
        return moves;
    }

}
