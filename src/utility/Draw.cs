
public class Draw
{
    public static void drawBoard (Board board)
    {
        string[] coords = new string[] {"1 |", "2 |", "3 |", "4 |", "5 |", "6 |", "7 |", "8 |"};
        Console.WriteLine("    a b c d e f g h");
        Console.WriteLine("  +-----------------+");

        ulong one=1;
        int index;
        for (int y=7; y>=0; y--) {
            Console.Write(coords[y]);
            for (int x=0; x<8; x++) 
            {
                index=y*8 + x;
                ulong i= one<<index;

                string tile;
                     if ((board.allBitboards[1][1] & i) != 0)   tile = "\u265F";
                else if ((board.allBitboards[1][2] & i) != 0)   tile = "\u265E";
                else if ((board.allBitboards[1][3] & i) != 0)   tile = "\u265D";
                else if ((board.allBitboards[1][4] & i) != 0)   tile = "\u265C";
                else if ((board.allBitboards[1][5] & i) != 0)   tile = "\u265B";
                else if ((board.allBitboards[1][6] & i) != 0)   tile = "\u265A";
                else if ((board.allBitboards[0][1] & i) != 0)   tile = "\u2659";
                else if ((board.allBitboards[0][2] & i) != 0)   tile = "\u2658";
                else if ((board.allBitboards[0][3] & i) != 0)   tile = "\u2657";
                else if ((board.allBitboards[0][4] & i) != 0)   tile = "\u2656";
                else if ((board.allBitboards[0][5] & i) != 0)   tile = "\u2655";
                else if ((board.allBitboards[0][6] & i) != 0)   tile = "\u2654";
                else tile = "-";

                Console.OutputEncoding = System.Text.Encoding.Unicode;
                Console.Write(" "+tile);
            }
            Console.WriteLine(" |");
        }
        Console.WriteLine("  +-----------------+");
    }


    public static void drawMask(ulong moveMask)
    {
        string[] coords = new string[] {"1 |", "2 |", "3 |", "4 |", "5 |", "6 |", "7 |", "8 |"};
        Console.WriteLine("    a b c d e f g h");
        Console.WriteLine("  +-----------------+");

        ulong one=1;
        int index;
        for (int y=7; y>=0; y--) {
            Console.Write(coords[y]);
            for (int x=0; x<8; x++) 
            {
                index=y*8 + x;
                ulong i= one<<index;

                string tile;

                if ((moveMask & i) != 0) tile = "X";
                else tile = "-";

                Console.Write(" "+tile);
            }
            Console.WriteLine(" |");
        }
        Console.WriteLine("  +-----------------+");
    }


    private static void drawBoardAndMask(Board board, ulong mask)
    {
        string[] coords = new string[] {"1 |", "2 |", "3 |", "4 |", "5 |", "6 |", "7 |", "8 |"};
        Console.WriteLine("    a b c d e f g h");
        Console.WriteLine("  +-----------------+");

        ulong one=1;
        int index;
        for (int y=7; y>=0; y--) {
            Console.Write(coords[y]);
            for (int x=0; x<8; x++) 
            {
                index=y*8 + x;
                ulong i= one<<index;

                string tile;
                     if ((board.allBitboards[1][1] & i) != 0) tile = "P";
                else if ((board.allBitboards[1][2] & i) != 0) tile = "N";
                else if ((board.allBitboards[1][3] & i) != 0) tile = "B";
                else if ((board.allBitboards[1][4] & i) != 0) tile = "R";
                else if ((board.allBitboards[1][5] & i) != 0) tile = "Q";
                else if ((board.allBitboards[1][6] & i) != 0) tile = "K";
                else if ((board.allBitboards[0][1] & i) != 0) tile = "p";
                else if ((board.allBitboards[0][2] & i) != 0) tile = "n";
                else if ((board.allBitboards[0][3] & i) != 0) tile = "b";
                else if ((board.allBitboards[0][4] & i) != 0) tile = "r";
                else if ((board.allBitboards[0][5] & i) != 0) tile = "q";
                else if ((board.allBitboards[0][6] & i) != 0) tile = "k";

                else if ((mask & i) != 0) tile = "X";
                else tile = "-";
                 Console.Write(" "+tile);
            }
            Console.WriteLine(" |");
        }
        Console.WriteLine("  +-----------------+");
    }


}
