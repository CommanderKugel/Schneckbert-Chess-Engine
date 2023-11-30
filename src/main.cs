

public class main
{
    public static Board board = new Board();


    public static void Main(string[] args)
    {
        Helper.init();
        
        const long timeControl = 75;
        BotMatch matchUp;

        // call from python GUI -> args passed down
        if (args.Length > 0) 
        {
            int num = Int32.Parse(args[0]);         
        

            matchUp = new BotMatch(new HumanPlayerPy(), new Search_17_IncPesto());
            matchUp.humanVsBot(timeControl, num);

        }
        // if not called from python GUI
        else 
        {
            matchUp = new BotMatch(new Search_17_Pesto(), new Search_17());
            matchUp.match(timeControl, entryRound: 0, endRound: 1000);
            // Simlpe Eval + Mobility vs. Pesto: 269+ 835= 266-
            
            matchUp = new BotMatch(new Search_17_KS(), new Search_17_Pesto());
            matchUp.match(timeControl, entryRound: 0, endRound: 1000);
            

        }
    }
}
