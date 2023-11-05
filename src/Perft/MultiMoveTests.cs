
using System.Diagnostics;
using System.Diagnostics.Metrics;

public static class Perft
{

    public static Stopwatch watch = new Stopwatch();


    private static ulong recursionTT (Board board, int depth)
    {
        ulong nodes = 0;
        ulong comparer = 0;
        if (TT.ContainsKey(board.currentGamestate.zobristKey)) 
        {
            hit++;
            comparer = TT[board.currentGamestate.zobristKey];
        }
        Move[] moves = board.generateLegalMoves();
        if (depth <= 1) return (ulong) moves.Length;

        foreach (Move move in moves)
        {
            board.makeMove(move);
            nodes += recursionTT(board, depth-1);
            board.undoMove(move);
        }
        if (!TT.ContainsKey(board.currentGamestate.zobristKey))
            TT.Add(board.currentGamestate.zobristKey, nodes);
        else TT[board.currentGamestate.zobristKey] = nodes;
        if (comparer != 0 && comparer != nodes) fails++;
        return nodes;
    }

    
    static int fails = 0;
    static int hit = 0;
    static Dictionary<ulong, ulong> TT = new Dictionary<ulong, ulong>();

    public static void perftTT (Board board, int depth, int seed)
    {
        int sum = 0;
        string[] fens = { "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 ", "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ", "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8  ", "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10 " };

        foreach (string fen in fens)
        {
            NotationHelper.initFen(board, fen);
            for (int i=depth; i<=depth; i++)
            {   
                fails = 0;
                hit = 0;

                TT = new Dictionary<ulong, ulong>() ;
                watch.Restart();
                ulong nodes = recursionTT(board, i);
                watch.Stop();

                sum += hit / (fails + 1);
                Console.WriteLine(hit / (fails + 1));
            }
            
        }
        Console.WriteLine("seed: "+seed+", acc: "+(sum/6));
    }







    private static ulong recursionPseudo (Board board, int depth)
    {
        ulong nodes = 0;
        Move[] moves = board.generateMovesPseudo();

        if (depth <= 1) return (ulong) moves.Length;

        foreach (Move move in moves)
        {
            board.makeMove(move);
            nodes += recursionPseudo (board, depth-1);
            board.undoMove(move);
        }

        return nodes;
    }

    private static ulong recursionLegal (Board board, int depth)
    {
        ulong nodes = 0;
        Move[] moves = board.generateLegalMoves();

        if (depth <= 1) return (ulong) moves.Length;

        foreach (Move move in moves)
        {
            board.makeMove(move);
            nodes += recursionLegal (board, depth-1);
            board.undoMove(move);
        }

        return nodes;
    } 


    public static void perftPseudo (Board board, int depth)
    {
        try
        {
            for (int i=1; i<=depth; i++) 
                Console.WriteLine("depth: "+i+", nodes: "+recursionPseudo(board, i));
        }
        catch
        {
            Draw.drawBoard(board);
            board.printMovehistory();
        }
    }

    public static void perftLegal (Board board, int depth)
    {
        try
        {
            for (int i=1; i<=depth; i++) 
                Console.WriteLine("depth: "+i+", nodes: "+recursionLegal(board, i));
        }
        catch
        {
            Draw.drawBoard(board);
            board.printMovehistory();
        }
    }


    public static void perftPerMovePseudo (Board board, int depth)
    {
        Move[] moves = board.generateMovesPseudo();
        Console.WriteLine("moves: "+moves.Length);
        int total=0;

        for (int i=0; i<moves.Length; i++)
        {
            Move move = moves[i];
            board.makeMove(move);

            int res = (int) recursionPseudo(board, depth-1);
            Console.WriteLine(i+": "+move+" - "+res);
            total += res;

            board.undoMove(move);

        }
        Console.WriteLine("total: "+total); 
    }

        public static void perftPerMoveLegal (Board board, int depth)
    {
        Move[] moves = board.generateLegalMoves();
        Console.WriteLine("moves: "+moves.Length);
        int total=0;

        for (int i=0; i<moves.Length; i++)
        {
            Move move = moves[i];
            board.makeMove(move);

            int res = (int) recursionLegal(board, depth-1);
            Console.WriteLine(i+": "+move+" - "+res);
            total += res;

            board.undoMove(move);

        }
        Console.WriteLine("total: "+total); 
    }


    public const string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 ";
    public static readonly ulong[] startResults = { 20, 400, 8_902, 197_281, 4_865_609, 119_060_324 };

    public static void perftFromStartPseudo (Board board, int depth)
    {
        NotationHelper.initFen(board, startFen);
        
        for (int i=1; i<=depth; i++)
        {   
            watch.Restart();
            ulong nodes = recursionPseudo(board, i);
            watch.Stop();
            Console.WriteLine("depth "+i+"; in "+watch.ElapsedMilliseconds+" ms; nodes: "+nodes);
            Console.WriteLine(nodes / ((ulong) watch.ElapsedMilliseconds + 1)* 1000 + " nps \n");
        }
    }

    public static void perftFromStartLegal (Board board, int depth)
    {
        NotationHelper.initFen(board, startFen);
        
        for (int i=1; i<=depth; i++)
        {   
            watch.Restart();
            ulong nodes = recursionLegal(board, i);
            watch.Stop();
            Console.WriteLine("depth "+i+"; in "+watch.ElapsedMilliseconds+" ms; nodes: "+nodes);
            Console.WriteLine(nodes / ((ulong) watch.ElapsedMilliseconds + 1)* 1000 + " nps \n");
        }
    }
    


    public static void compareMoveGenerators (Board board, int depth, string fen)
    {
        NotationHelper.initFen(board, fen);
        Console.WriteLine("fen: "+fen);

        watch.Reset();
        watch.Start();
        ulong nodes1 = recursionPseudo(board, depth);
        watch.Stop();
        Console.WriteLine("pseudo legal: "+watch.ElapsedMilliseconds+" ms; nodes: "+nodes1+", thats: "+nodes1 / ((ulong) watch.ElapsedMilliseconds + 1)* 1000 + " nps");

        watch.Reset();
        watch.Start();
        ulong nodes2 = recursionLegal(board, depth);
        watch.Stop();
        Console.WriteLine("pin masks:    "+watch.ElapsedMilliseconds+" ms; nodes: "+nodes2+", thats: "+nodes2 / ((ulong) watch.ElapsedMilliseconds + 1)* 1000 + " nps");
    }


    public static void perftPerMoveBoth (Board board, int depth)
    {
        Move[] movesPseudo = board.generateMovesPseudo();
        Move[] movesLegal  = board.generateLegalMoves();

        if (movesPseudo.Length == movesLegal.Length)
        {
            Console.WriteLine("moves: "+movesPseudo.Length);
            int totalPseudo=0;
            int totalLegal =0;
            for (int i=0; i<movesPseudo.Length; i++)
            {
                Move move = movesPseudo[i];
                board.makeMove(move);

                int resPseudo = (int) recursionPseudo(board, depth-1);
                int resLegal  = (int) recursionLegal(board, depth-1);
                Console.WriteLine(i+": "+move+" - "+resLegal+"/"+resPseudo);
                if (resLegal != resPseudo) { Console.WriteLine("ERROR FOUND!"); }
                totalPseudo += resPseudo;
                totalLegal  += resLegal;

                board.undoMove(move);
            }
            Console.WriteLine("total: "+totalLegal+"/"+totalPseudo); 
            Console.WriteLine(totalLegal == totalPseudo ? "✓" : "X"); 
        }

        else if (movesLegal.Length > movesPseudo.Length) 
        {
            for (int i=0; i<movesLegal.Length; i++)
            {
                Console.WriteLine("legal: "+movesLegal[i]+" vs "+movesPseudo[i]+" pseudo");
            }
        }
        else if (movesLegal.Length < movesPseudo.Length) 
        {
            for (int i=0; i<movesLegal.Length; i++)
            {
                Console.WriteLine("legal: "+movesLegal[i]+" vs "+movesPseudo[i]+" pseudo");
            }
        }
    }

    public static void depthOneCompare (Board board)
    {
        Move[] movesPseudo = board.generateMovesPseudo();
        Move[] movesLegal  = board.generateLegalMoves();

        for (int i=0; i<movesLegal.Length; i++)
        {
            Console.WriteLine("legal: "+movesLegal[i]+" vs "+movesPseudo[i]+" pseudo");
        }

    }

}
