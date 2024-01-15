
using System.Diagnostics;

public class Standard : Search
{
    public override void Reset()
    {
        return;
    }

    Stopwatch watch = new Stopwatch();
    const int CHECKMATE = 2_000_000_000;

    Board board;
    long maxTime;

    long nodeCount;
    int startPly;
    Move globalBestMove;

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
        if (board.isFiftyMoveDraw || board.onlyKings)
            return 0;


        if (depth <= 0)
            return Pesto.Eval(board);


        bool isRoot = board.plyCount == startPly;



        Span<Move> moves = stackalloc Move[218];
        int moveCount = board.generateLegalMoves(ref moves);
        
        
        int bestScore = -CHECKMATE;
        for (int i=0; i<moveCount; i++)
        {
            Move move = moves[i];

            board.makeMove(move);
            nodeCount++;
            int score = -Search(-beta, -alpha, depth-1);
            board.undoMove(move);


            if (watch.ElapsedMilliseconds > maxTime)
                return CHECKMATE;


            if (score > bestScore)
            {
                bestScore = score;
                if (isRoot)
                    globalBestMove = move;

                if (score > alpha)
                    alpha = score;
                
                if (score >= beta)
                    return score;
            }
        }
        return bestScore;
    }
}
