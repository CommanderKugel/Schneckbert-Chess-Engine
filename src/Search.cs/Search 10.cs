
using System.Diagnostics;

//
// negamaxframework with alpha beta pruning
// Repitition, check- & stalemate detection
// Basic Eval Function + basic mobility
// basic q-search
//
// iterative deepening
// return when time is up
// return best move of incomplete iterations
// Move ordering: TTmove, MVV-LVA
//
// NEW STUFF:
// Killer Moves
// 
//
// WDL vs. Search8: 164+ 622= 214-
// time: 100
//

public class Search_10 : Search
{
    public override string ToString() { return "Search_10"; }
    
    int CHECKMATE = 30_000_000;
    Stopwatch watch = new Stopwatch();
    Board board;

    int startPly;
    int globalBestScore;
    Move globalBestMove;
    long timeControl;

    bool timeIsUp;

    Transposition[] transpositionTable = new Transposition[0xFFFF];
    Move[] killerMoves1 = new Move[0xFF];
    Move[] killerMoves2 = new Move[0xFF];

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
        if (isRoot || entry.move is not null) moves = moveOrdering(moves, isRoot, entry.move);

        int score;
        int localBestScore = -CHECKMATE;
        int startAlpha = alpha;
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
                    if (score >= beta) 
                    {
                        if (board.pieceLookup[move.to]==PieceType.None)
                        {
                            int hash = board.plyCount % 0xFF;
                            killerMoves2[hash] = killerMoves1[hash];
                            killerMoves1[hash] = move;
                        }
                        break;
                    }
                    
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
        moves = moveOrdering(moves);

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

    private Move[] moveOrdering (Move[] moves, bool isRoot=false, Move entryMove=null)
    {
        Move firstMove = isRoot ? globalBestMove : entryMove is not null ? entryMove : Move.nullMove;
        int hash = board.plyCount % 0xFF;
        Move killerMove1 = killerMoves1[hash] is not null ? killerMoves1[hash] : Move.nullMove;
        Move killerMove2 = killerMoves2[hash] is not null ? killerMoves2[hash] : Move.nullMove;
        int[] moveScores = new int[moves.Length];

        for (int i=0; i<moves.Length; i++)
        {
            // TT Move
            // MVV-LVA
            // Killer Move
            // quiet Moves

            moveScores[i] = -(moves[i]==firstMove ? 1000 :
                              board.pieceLookup[moves[i].to]!=PieceType.None ? (100 + (int)board.pieceLookup[moves[i].from]*10-(int)board.pieceLookup[moves[i].to]) :
                              (moves[i]==killerMove1 || moves[i]==killerMove2) ? 90 :
                              0);
        }
        Array.Sort(moveScores, moves);
        return moves;
    }

}
