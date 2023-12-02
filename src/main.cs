

public class main
{
    public static Board board = new Board();


    public static void Main(string[] args)
    {
        Helper.init();
        
        const long timeControl = 1000;
        BotMatch matchUp;

        // call from python GUI -> args passed
        if (args.Length > 0) 
        {
            int num = Int32.Parse(args[0]);         
        

            matchUp = new BotMatch(new HumanPlayerPy(), new Search_17_forHuman());
            matchUp.humanVsBot(timeControl, num);

        }
        // if not called from python GUI
        else 
        {
            matchUp = new BotMatch(new Search_17_LMR(), new Search_17());
            matchUp.match(timeControl, entryRound: 0, endRound: 1000);
            
            matchUp = new BotMatch(new Search_17_FP(), new Search_17());
            matchUp.match(timeControl, entryRound: 0, endRound: 500);
            

        }
    }
}
