

public class main
{
    public static Board board = new Board();


    public static void Main()
    {
        Helper.init();

        

        if (true)
        {
            const long timeControl = 100;


            BotMatch matchUp_12v11 = new BotMatch(new Search_12(), new Search_11());
            matchUp_12v11.match(timeControl, threadName: "12vs11");

            BotMatch matchUp_13v11 = new BotMatch(new Search_13(), new Search_11());
            matchUp_13v11.match(timeControl, threadName: "13vs11");


        } 
        else
        {
            Search search = new Search_11();

            string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - ";
            NotationHelper.initFen(board, fen);
            Draw.drawBoard(board);

            Move move = search.Think(board, 500);
            Console.WriteLine("MOVE: "+move);
            board.makeMove(move);
            Draw.drawBoard(board);

        }
              
    }

}

