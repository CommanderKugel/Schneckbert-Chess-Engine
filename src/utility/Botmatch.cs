
using System.Diagnostics;
using System.Numerics;

public class Botmatch
{
    Search x;
    Search y;

    public Botmatch(Search x, Search y)
    {
        this.x = x;
        this.y = y;
    }


    public void go(long timeControl, long increment=0, int entryRound=0, int endRound=500, bool print=false)
    {
        if (endRound > 3317) 
            endRound = 3317;
        if (entryRound > endRound) 
            return;

        //      WIN x, DRAW, WIN y, ERROR //
        int[] WDL = { 0, 0, 0, 0 };

        const string fenPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.3\\Schneckbert 0.3\\resources\\UHO_2022\\UHO_2022_+110_+119\\UHO_2022_6mvs_+110_+119.epd";
        increment /= 2;

        for (int matchNum=entryRound; matchNum < endRound; matchNum++)
        {
            string fen = File.ReadLines(fenPath).Skip(matchNum).Take(1).First();

            switch (go(timeControl, increment, fen, false))
            {
                case DRAW:      WDL[1]++; break;
                case WHITE_WON: WDL[0]++; break;
                case BLACK_WON: WDL[2]++; break;
                case ERROR:     WDL[3]++; break;
            }
            Console.WriteLine($"{matchNum*2}: {WDL[0]},{WDL[1]},{WDL[2]}, ERRORS: {WDL[3]}");

            switch (go(timeControl, increment, fen, true))
            {
                case DRAW:      WDL[1]++; break;
                case WHITE_WON: WDL[2]++; break;
                case BLACK_WON: WDL[0]++; break;
                case ERROR:     WDL[3]++; break;
            }
            Console.WriteLine($"{matchNum*2+1}: {WDL[0]},{WDL[1]},{WDL[2]}, ERRORS: {WDL[3]}");
        
        }
    }


    public int go (long timeControl, long increment, string fen, bool flip, bool print=false)
    {
        // RESET BLACK AND WHITE
        Search white = flip ? y : x;
        Search black = flip ? x : y;
        var whiteClock = new Stopwatch();
        var blackClock = new Stopwatch();
        white.Reset();
        black.Reset();

        Board board = new Board();
        NotationHelper.initFen(board, fen);

        int result = ONGOING;
        while (result == ONGOING)
        {
            Search playerToMove = board.isWhiteToMove ? white : black;
            Move move = Move.nullMove;

            var clock = board.isWhiteToMove ? whiteClock : blackClock;
            long timeLeft = timeControl + board.plyCount * increment - clock.ElapsedMilliseconds;
            if (timeLeft < 0) return board.isWhiteToMove ? BLACK_WON : WHITE_WON;

            clock.Start();
            try
            {
                move = playerToMove.Think(board, timeLeft, increment);
            }
            catch
            {
                Console.Error.WriteLine($"Search.Think thew an error: {fen}");
                return ERROR;
            }
            clock.Stop();

            if (move == Move.nullMove)
            {
                Console.WriteLine("RETURNED NULL MOVE");
                return ERROR;
            }

            board.makeMove(move);

            result = checkResult(board);
        }

        return result;
    }


    public int checkResult (Board board)
    {
        Span<Move> moves = stackalloc Move[218];
        int moveCount = board.generateLegalMoves(ref moves);

        if (moveCount == 0)
        {
            if (!board.isInCheck)
                return DRAW;

            return board.isWhiteToMove ? BLACK_WON : WHITE_WON;
        }

        if (board.isFiftyMoveDraw || 
            board.isRepeatedPosition ||
            board.onlyKings)
            return DRAW;

        return ONGOING;
    }




    const int ONGOING = 100;
    const int BLACK_WON = -1;
    const int DRAW = 0;
    const int WHITE_WON = 1;
    const int ERROR = -100;
}
