public class HumanPlayer : Search
{
    public override string ToString() { return "Human"; }

    public override Move Think(Board board, long timeControl)
    {
        Console.Write("human color: "+(board.isWhiteToMove?"White ":"Black "));
        string move = Console.ReadLine();
        
        Move humanMove = Move.nullMove;
        try { humanMove = new Move(move, board); }
        catch { Console.WriteLine("ERROR OCCURED"); }
        
        return humanMove;

    }
}