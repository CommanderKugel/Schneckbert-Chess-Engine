
using System.Collections;
using System.Text.RegularExpressions;

public class main
{
    public static Helper.init init = new Helper.init();
    public static Board board = new Board();
    public static Search search = new Search();


    const long timeControl = 1000;


    public static void Main()
    {
        
        NotationHelper.initFen(board, "8/8/8/8/8/5K2/1Q6/7k w - - 0 1");
        Draw.drawBoard(board);

        board.makeMove(new Move("b2f2", moveFlag.quietMove));

        //board.makeMove(search.Think(board, timeControl));
        Draw.drawBoard(board);
        Console.WriteLine(board.generateLegalMoves().Length);
        Draw.drawMask(board.allBitboards[1][5]);

    }
}

