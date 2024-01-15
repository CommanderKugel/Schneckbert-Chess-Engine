
using System.Diagnostics;


public class main
{
    public static Board board = new Board();
    public static Stopwatch watch = new Stopwatch();



    public static void Main()
    {
        Helper.init(); 
        
        Botmatch match = new Botmatch(new Q_Search(), new TT_Move());
        match.go(timeControl: 1_000, increment: 10);


        match = new Botmatch(new Q_Search(), new TT_cutoff());
        match.go(timeControl: 1_000, increment: 10);


        match = new Botmatch(new TT_Move(), new TT_cutoff());
        match.go(timeControl: 1_000, increment: 10);
    }
}
