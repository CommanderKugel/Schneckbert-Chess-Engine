

public class main
{
    public static Board board = new Board();


    public static void Main()
    {
        Helper.init();

        

        if (true)
        {
            const long timeControl = 50;


            BotMatch matchUp = new BotMatch(new Search_18(), new Search_17());
            matchUp.match(timeControl, threadName: "18vs17");


        } 
        else
        {
            Search search = new Search_14();

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

