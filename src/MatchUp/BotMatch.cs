

using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

public class BotMatch
{

    public Search searcher1;
    public Search searcher2;
    public long timeControl;
    public string name;

    public BotMatch(Search searcher1, Search searcher2)
    {
        this.searcher1 = searcher1;
        this.searcher2 = searcher2;
        name = searcher1.ToString()+" vs "+searcher2.ToString();
    }

    private const string fensPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\UHO_2022\\UHO_2022_+110_+119\\UHO_2022_6mvs_+110_+119.epd";
    private const string folderPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\testResults\\";
    private const string moveWritePath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\moveFromCS.txt";
    private const string displayFenPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\displayFen.txt";

    public void humanVsBot (long timeControl, int fenIndex)
    {
        string fen = File.ReadLines(fensPath).Skip(fenIndex).Take(1).First();
        using (StreamWriter writer = new StreamWriter(displayFenPath)) 
        {
            writer.WriteLine( fen );
        }
        Console.WriteLine(fen);

        int res = go(timeControl, false, fen, false);
        Console.WriteLine(res==0 ? "Human won" : res == 1 ? "Draw" : "Computer Won");
    }


    public void match(long timeControl, int entryRound=0, int endRound=500, bool doPrinting=false)
    {   
        if (endRound > 3317) endRound = 3317;
        if (entryRound > endRound) entryRound=endRound;

        int[] WDL = { 0, 0, 0 };
        
        int res;
        for (int matchNum=entryRound; matchNum<endRound; matchNum++)
        {
            string fen = File.ReadLines(fensPath).Skip(matchNum).Take(1).First();

            res = go(timeControl, false, fen, doPrinting);
            WDL[res]++;
            Console.WriteLine(name+", "+matchNum+": "+WDL[0]+"+ "+WDL[1]+"= "+WDL[2]+"-");

            res = go(timeControl, true, fen, doPrinting);
            WDL[res]++;
            Console.WriteLine(name+", "+matchNum+": "+WDL[0]+"+ "+WDL[1]+"= "+WDL[2]+"-");
        }
        
        using (StreamWriter writer = new StreamWriter(folderPath+name)) writer.WriteLine(
            searcher1.ToString()+" vs "+searcher2.ToString()+": "+WDL[0]+"+ "+WDL[1]+"= "+WDL[2]+"-"
        );
        Console.WriteLine("FINAL RESULT "+name+": "+searcher1+" "+WDL[0]+"+ "+WDL[1]+"= "+WDL[2]+"- "+searcher2);
    }


    public int go(long timeControl, bool flip, string fen, bool doPrinting=false)
    {
        if (searcher2 is Search_17_forHuman) searcher2 = Search_17_forHuman.reset();
        Search white = flip ? searcher2 : searcher1;
        Search black = flip ? searcher1 : searcher2;
        
        Board board = new Board();
        NotationHelper.initFen(board, fen);

        if (false && doPrinting) 
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


            try
            {
                move = playerToMove.Think(board, timeControl);

                try
                {
                    board.makeMove(move);

                    if (playerToMove is not HumanPlayerPy)
                    {
                        writeMove(move, moveWritePath);
                    }
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

            if (false && doPrinting)
            {
                Console.Clear();
                Draw.drawBoard(board);
                Console.WriteLine("last move: "+move);
            }
                      
        }          
        
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
