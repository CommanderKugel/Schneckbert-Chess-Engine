

using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

public class BotMatch
{
    // constants to change rooks
    const ushort whiteKingside =   7 |  5 << 6;
    const ushort blackKingside =  63 | 61 << 6;
    const ushort whiteQueenside =  0 |  3 << 6;
    const ushort blackQueenside =  7 | 59 << 6;

    public Search searcher1;
    public Search searcher2;
    public long timeControl;
    public Board board;

    public BotMatch(Search searcher1, Search searcher2)
    {
        this.searcher1 = searcher1;
        this.searcher2 = searcher2;
        board = new Board();
    }

    private const string fensPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\Fens.txt";
    private const string folderPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\";

    public void match(long timeControl, int rounds=500, string threadName="", bool doPrinting=false)
    {   
        if (rounds > 500) rounds = 500;

        int[] WDL = { 0, 0, 0 };
        
        int res;
        for (int matchNum=0; matchNum<rounds; matchNum++)
        {
            string fen = File.ReadLines(fensPath).Skip(matchNum).Take(1).First();

            res = go(timeControl, false, fen, doPrinting);
            WDL[res]++;

            res = go(timeControl, true, fen, doPrinting);
            WDL[res]++;

            Console.WriteLine(threadName+", "+matchNum+": "+WDL[0]+"+ "+WDL[1]+"= "+WDL[2]+"-");
        }
        
        using (StreamWriter writer = new StreamWriter(folderPath+threadName)) writer.WriteLine(
            searcher1.ToString()+" vs "+searcher2.ToString()+": "+WDL[0]+"+ "+WDL[1]+"= "+WDL[2]+"-"
        );
        Console.WriteLine("FINAL RESULT "+threadName+": "+searcher1+" "+WDL[0]+"+ "+WDL[1]+"= "+WDL[2]+"- "+searcher2);
    }


    public int go(long timeControl, bool flip, string fen, bool doPrinting=false)
    {
        Search white = flip ? searcher2 : searcher1;
        Search black = flip ? searcher1 : searcher2;
        
        NotationHelper.initFen(board, fen);

        if (doPrinting) 
        {
            Console.Clear();
            Draw.drawBoard(board);
        }

        gameResult res = gameResult.ongoing;
        while (res == gameResult.ongoing)
        {
            bool player = board.isWhiteToMove;
            Search playerToMove = player ? white : black;
            Move move = Move.nullMove;

            if (doPrinting)
            {
                Console.Clear();
                Draw.drawBoard(board);
            }

            try
            {
                move = playerToMove.Think(board, timeControl);

                try
                {
                    board.makeMove(move);           
                }
                catch
                {
                    Console.WriteLine("ERROR when making a move occured. Player: "+playerToMove);
                    Console.WriteLine("ply: "+board.plyCount+", fen: "+fen+", move: "+move);
                    res = player ? gameResult.blackWon : gameResult.whiteWon;
                }

                res = checkResult(board); 
            }
            catch
            {
                Console.WriteLine("ERROR while thinking occured. Player: "+playerToMove);
                Console.WriteLine("ply: "+board.plyCount+", fen: "+fen);
                res = player ? gameResult.blackWon : gameResult.whiteWon;
            }
            if (doPrinting)
            {
                Draw.drawBoard(board);
                Console.WriteLine(move);
            }
          
        }

        //if (res == gameResult.draw) Console.WriteLine("Draw");
        //else if (res == gameResult.blackWon) Console.WriteLine(black+" won");
        //else if (res == gameResult.whiteWon) Console.WriteLine(white+" won"); 
            
        
        int ret = 1;
        if (res != gameResult.draw)
        {
            ret = res==gameResult.whiteWon ? (flip ? 2 : 0) 
                                           : (flip ? 0 : 2);
        }
        return ret;
    }



    public void writeMove (Move move, string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
            writer.WriteLine(move.ToString());
    }


    public static gameResult checkResult (Board board)
    {
        if (board.generateLegalMoves().Length == 0)
        {
            if (board.isInCheck && board.isWhiteToMove) return gameResult.blackWon;
            if (board.isInCheck && board.isWhiteToMove) return gameResult.whiteWon;
            return gameResult.draw;
        }
        if (board.repititionTable.isRepeatedPosition()) 
        {
            //Console.WriteLine("Repitition Draw! ply: "+board.plyCount+" table index: "+board.repititionTable.index);
            //board.repititionTable.printTable();
            return gameResult.draw;
        }
        if (board.currentGamestate.fiftyMoveCounter == 50)
        {
            //Console.WriteLine("50 move draw!");
            return gameResult.draw;
        }
        return gameResult.ongoing;
    }

}
