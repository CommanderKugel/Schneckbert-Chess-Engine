
using System.Diagnostics;
using System.Numerics;

public static class Perft
{
    private static Stopwatch watch = new Stopwatch();
    public const string startingPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private static long recursion(Board board, int depth)
    {
        if (depth == 0)
            return 1;

        long nodes = 0;    
        Span<Move> moves = stackalloc Move[218];
        int MoveCount = board.generateLegalMoves(ref moves);
            
        for (int i=0; i<MoveCount; i++)
        {
            Move move = moves[i];
            board.makeMove(move);
            nodes += recursion(board, depth-1);
            board.undoMove(move);
        }
        return nodes;
    }   

    static Move x = new Move("c7c5", moveFlag.doublePawnPush);

    public static void go(int depth=6, string fen=startingPos)
    {
        Board board = new Board();
        board.initFen(fen);

        Console.WriteLine($"fen: {fen}");
        for (int i=1; i<=depth; i++)
        {
            watch.Restart();
            long nodes = recursion(board, i);
            Console.WriteLine($"depth: {i}, nodes: {nodes}, nps: {nodes/(watch.ElapsedMilliseconds+1)*1000}");
        }
    }


    public static void goSplit(int depth, Board board)
    {
        Span<Move> moves = stackalloc Move[218];
        int moveCount = board.generateLegalMoves(ref moves);

        long counter = 0;

        for (int i=36; i<37; i++)
        {
            board.makeMove(moves[i]);
            long nodes = recursion(board, depth-1);
            counter += nodes;
            Console.WriteLine($"{moves[i]}: {nodes}");
            board.undoMove(moves[i]);
        }
        Console.WriteLine($"moveCount: {moveCount}, total: {counter}");
    }



}
