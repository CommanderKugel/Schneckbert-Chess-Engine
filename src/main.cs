

public class main
{
    public static Board board = new Board();


    public static void Main()
    {
        Helper.init();

        

        if (true)
        {
            const long timeControl = 100;


            BotMatch matchUp_11v8 = new BotMatch(new Search_11(), new Search_8());
            matchUp_11v8.match(timeControl, threadName: "11vs8");


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

