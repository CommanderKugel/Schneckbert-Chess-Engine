
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;


public class Search
{
    const int CHECKMATE = 0x7FFFFFF;
    const int GONNA_BE_MATE = 0xFFFFF;

    Stopwatch watch = new Stopwatch();

    Move bestMove;
    int globalBestScore;
    Board board;

    long time;
    int startPly;

    public Move Think (Board board, long time)
    {
        watch.Restart();
        this.board = board;
        this.time = time;
        
        startPly = board.plyCount;
        globalBestScore = -CHECKMATE;
        
        Move[] moves = board.generateLegalMoves();
        bestMove = moves[0];

        int depth = 0;
        int score;
        while (time<watch.ElapsedMilliseconds || depth++ <= 2)
        {
            foreach (Move move in moves)
            {
                board.makeMove(move);
                score = -search(-CHECKMATE, CHECKMATE, depth);
                board.undoMove(move);

                Console.WriteLine(move+": "+score);

                if (score > globalBestScore) 
                {
                    globalBestScore = score;
                    bestMove = move;
                }

                if (watch.ElapsedMilliseconds > time ||
                    globalBestScore > GONNA_BE_MATE) return bestMove;
            }            
        }

        return bestMove;
    }



    private int search (int alpha, int beta, int depth)
    {
        if (board.repititionTable.isRepeatedPosition()) return 0;
        if (depth == 0) return Eval.PestoEval(board);

        Move[] moves = board.generateLegalMoves();

        if (moves.Length == 0)
        {
            if (board.genLegal.checkMask == 0) { return 0; }
            else return board.plyCount - CHECKMATE;
        }
        
        int score;
        int localBestScore = -CHECKMATE;
        foreach (Move move in moves)
        {
            board.makeMove(move);
            score = -search(-beta, -alpha, depth-1);
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

}
