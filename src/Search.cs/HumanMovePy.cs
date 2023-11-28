using System.Diagnostics;

public class HumanPlayerPy : Search
{
    public override string ToString() { return "Human_Py"; }

    private const string moveReadPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\moveFromPython.txt";
    private string move = "";

    public override Move Think(Board board, long timeControl)
    {
        Console.Write("human color: "+(board.isWhiteToMove?"White ":"Black "));

        while (true)
        {
            string newMove = File.ReadLines(moveReadPath).Take(1).First();
            if (move != newMove && newMove != "0")
            {
                move = newMove;
                break;
            }
            Thread.Sleep(100);
        }
        
        Move humanMove = Move.nullMove;
        try { humanMove = new Move(move, board); }
        catch { Console.WriteLine("ERROR OCCURED"); }

        Console.WriteLine("move: "+humanMove);
        
        return humanMove;
    }

    
}