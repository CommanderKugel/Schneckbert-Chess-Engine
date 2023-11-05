
public enum moveFlag
{
    quietMove,                  //  0
    doublePawnPush,             //  1
    kingsideCastle,             //  2
    queensideCastle,            //  3
    capture,                    //  4
    enPassantCapture,           //  5
    knightPromotion,            //  6
    bishopPromotion,            //  7
    rookPromotion,              //  8
    queenPromotion,             //  9
    knightPromotionCapture,     // 10
    bishopPromotionCapture,     // 11
    rookPromotionCapture,       // 12
    queenPromotionCapture       // 13
}

public class Move
{

    public int from;
    public int to;
    public moveFlag flag;

    public Move (int from, int to, moveFlag flag)
    {
        this.from = from;
        this.to = to;
        this.flag = flag;
    }


    public static bool operator ==(Move x, Move y) 
        => x.from == y.from && x.to == y.to;

    public static bool operator !=(Move x, Move y) 
        => !(x==y);


    public override string ToString()
    {
        if (flag == moveFlag.kingsideCastle) return "O-O";
        if (flag == moveFlag.queensideCastle) return "O-O-O";
        return NotationHelper.boardNotation[from] + NotationHelper.boardNotation[to]; // + ", " + flag;
    }

    public Move (string str, moveFlag flag)
    {
        from = NotationHelper.strToStartsquare(str);
        to = NotationHelper.strToEndsquare(str);
        this.flag = flag;
    }

    public int toInt ()
    {
        return from | to << 8;
    }
    
}