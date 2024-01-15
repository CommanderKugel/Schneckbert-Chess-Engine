
using System.Diagnostics;

public class TT_Move : Search
{
    public override void Reset()
    {
        transpositionTable = new Transposition[0xF_FFFF];
    }

    Stopwatch watch = new Stopwatch();
    const int CHECKMATE = 2_000_000_000;

    Board board;
    long maxTime;

    long nodeCount;
    int startPly;
    Move globalBestMove;

    Transposition[] transpositionTable;

    public override Move Think(Board board, long timeLeft, long increment)
    {
        this.board = board;
        maxTime = timeLeft / 30;
        startPly = board.plyCount;

        watch.Restart();
        nodeCount = 0;

        for (int depth=1; watch.ElapsedMilliseconds < maxTime; depth++)
        {
            Search(-CHECKMATE, CHECKMATE, depth);
        }

        return globalBestMove;
    }

    public int Search(int alpha, int beta, int depth)
    {
        bool isRoot = board.plyCount == startPly;

        if (!isRoot && (board.isFiftyMoveDraw || board.onlyKings || board.isRepeatedPosition))
            return 0;


        var entry = transpositionTable[board.zobristKey & 0x7FFFF];
        

        if (depth <= 0)
            return QSearch(alpha, beta);


        Span<Move> moves = stackalloc Move[218];
        int moveCount = board.generateLegalMoves(ref moves);


        Move ttMove = entry.move;
        int[] moveScores = new int[moveCount];
        for (int i=0; i<moveCount; i++)
        {
            Move move = moves[i];
            if (move == ttMove)
                moveScores[i] = 2_000_000_000;
            else if (move.isCapture)
            {
                int victim = board.pieceLookup[move.to];
                int attacker = board.pieceLookup[move.from];
                moveScores[i] = 100 * pVal[victim] - pVal[attacker];
            }
            else
                moveScores[i] = 0;
        }
        
        int startAlpha = alpha;
        int bestScore = 1-CHECKMATE;
        Move bestMove = Move.nullMove;
        for (int i=0; i<moveCount; i++)
        {
            if (watch.ElapsedMilliseconds > maxTime)
                return CHECKMATE;

            int bestIndex = i;
            int highScore = moveScores[i];
            for (int j=i+1; j<moveCount; j++)
            {
                if (moveScores[j] > highScore)
                {
                    highScore = moveScores[j];
                    bestIndex = j;
                }
            }
            Move move = moves[bestIndex];
            moves[bestIndex] = moves[i];
            moveScores[bestIndex] = moveScores[i];


            board.makeMove(move);
            nodeCount++;
            int score = -Search(-beta, -alpha, depth-1);
            board.undoMove(move);


            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
                if (isRoot)
                    globalBestMove = move;

                if (score > alpha)
                    alpha = score;
                
                if (score >= beta)
                    break;
            }
        }
        nodeType flag = (bestScore < alpha) ? nodeType.lowerBound : 
                        (bestScore >= beta) ? nodeType.upperBound 
                                            : nodeType.exact;
        transpositionTable[board.plyCount & 0x7FFFF] = new Transposition(board.zobristKey, bestMove, (byte) depth, (byte) bestScore, (byte) flag);

        return bestScore;
    }


    public int QSearch (int alpha, int beta)
    {
        int standPat = Pesto.Eval(board);

        if (watch.ElapsedMilliseconds > maxTime)
            return standPat;

        if (standPat >= beta)
            return standPat;
        if (standPat > alpha)
            alpha = standPat;

        Span<Move> moves = stackalloc Move[218];
        int moveCount = board.generateCaptures(ref moves);

        int[] moveScores = new int[moveCount];
        for (int i=0; i<moveCount; i++)
        {
            Move move = moves[i];
            int victim = board.pieceLookup[move.to];
            int attacker = board.pieceLookup[move.from];
            moveScores[i] = 100 * pVal[victim] - pVal[attacker];
        }
        
        
        int bestScore = alpha;
        for (int i=0; i<moveCount; i++)
        {
            int bestIndex = i;
            int highScore = moveScores[i];
            for (int j=i+1; j<moveCount; j++)
            {
                if (moveScores[j] > highScore)
                {
                    highScore = moveScores[j];
                    bestIndex = j;
                }
            }
            Move move = moves[bestIndex];
            moves[bestIndex] = moves[i];
            moveScores[bestIndex] = moveScores[i];


            board.makeMove(move);
            nodeCount++;
            int score = -QSearch(-beta, -alpha);
            board.undoMove(move);

            if (score > bestScore)
            {
                bestScore = score;

                if (score >= beta)
                    return score;
                if (score > alpha)
                    alpha = score;
            }
        }    

        return bestScore;
    }

    private static readonly int[] pVal = { 0, 100, 300, 320, 500, 900, 20_000 };

}
