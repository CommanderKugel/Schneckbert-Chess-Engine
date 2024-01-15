
public static class moveFlag
{
    /*
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
    */

    public const byte quietMove              = 0b0000;
    public const byte doublePawnPush         = 0b0001;
    public const byte kingsideCastle         = 0b0010;
    public const byte queensideCastle        = 0b0011;
    public const byte capture                = 0b0100;
    public const byte enPassantCapture       = 0b0101;
    public const byte knightPromotion        = 0b1000;
    public const byte bishopPromotion        = 0b1001;
    public const byte rookPromotion          = 0b1010;
    public const byte queenPromotion         = 0b1011;
    public const byte knightPromotionCapture = 0b1100;
    public const byte bishopPromotionCapture = 0b1101;
    public const byte rookPromotionCapture   = 0b1110;
    public const byte queenPromotionCapture  = 0b1111;
}

public struct Move
{

    public int from;
    public int to;
    public byte flag;

    public Move (int from, int to, byte flag)
    {
        this.from = from;
        this.to = to;
        this.flag = flag;
    }

    public static Move nullMove => new Move(0, 0, 0);

    public bool isCapture => (flag & 0b0100) != 0;
    public bool isPromotion => (flag & 0b1000) != 0;


    public static bool operator ==(Move x, Move y) => x.from == y.from && x.to == y.to && x.flag == y.flag;
    public static bool operator !=(Move x, Move y) => !(x==y);



    public override string ToString()
    {
        return flag switch
        {
            moveFlag.queenPromotion or moveFlag.queenPromotionCapture => NotationHelper.boardNotation[from] + NotationHelper.boardNotation[to] + "q",
            moveFlag.rookPromotion or moveFlag.rookPromotionCapture => NotationHelper.boardNotation[from] + NotationHelper.boardNotation[to] + "r",
            moveFlag.bishopPromotion or moveFlag.bishopPromotionCapture => NotationHelper.boardNotation[from] + NotationHelper.boardNotation[to] + "b",
            moveFlag.knightPromotion or moveFlag.knightPromotionCapture => NotationHelper.boardNotation[from] + NotationHelper.boardNotation[to] + "k",
            _ => NotationHelper.boardNotation[from] + NotationHelper.boardNotation[to],
        };
    }

    public Move (string str, byte flag)
    {
        from = NotationHelper.strToStartsquare(str);
        to = NotationHelper.strToEndsquare(str);
        this.flag = flag;
    }

    public Move (string str, Board board)
    {
        from = NotationHelper.strToStartsquare(str);
        to = NotationHelper.strToEndsquare(str);
        flag = board.pieceLookup[to]==0 ? moveFlag.quietMove : moveFlag.capture;

        if (NotationHelper.boardNotation[from]=="e1" && NotationHelper.boardNotation[to]=="g1" && board.pieceLookup[from]==6 ||
            NotationHelper.boardNotation[from]=="e8" && NotationHelper.boardNotation[to]=="g8" && board.pieceLookup[from]==6)
                flag = moveFlag.kingsideCastle;
        else if (NotationHelper.boardNotation[from]=="e1" && NotationHelper.boardNotation[to]=="c1" && board.pieceLookup[from]==6 ||
            NotationHelper.boardNotation[from]=="e8" && NotationHelper.boardNotation[to]=="c8" && board.pieceLookup[from]==6)
                flag = moveFlag.queensideCastle;
        else if (board.pieceLookup[from]==1 && (to<8 || to>55))
        {
            flag = board.pieceLookup[to]==1 ? moveFlag.queenPromotion : 
                                                           moveFlag.queenPromotionCapture;
        }
    }

    public int toInt ()
    {
        return from | (to << 6) | (flag << 12);
    }
    
}