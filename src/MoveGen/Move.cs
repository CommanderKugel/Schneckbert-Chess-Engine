
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

    public static Move nullMove => new Move(0, 0, 0);


    public static bool operator ==(Move x, Move y) 
        => x.from == y.from && x.to == y.to && x.flag == y.flag;

    public static bool operator !=(Move x, Move y) 
        => !(x==y);


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

    public Move (string str, moveFlag flag)
    {
        from = NotationHelper.strToStartsquare(str);
        to = NotationHelper.strToEndsquare(str);
        this.flag = flag;
    }

    public Move (string str, Board board)
    {
        from = NotationHelper.strToStartsquare(str);
        to = NotationHelper.strToEndsquare(str);
        flag = board.pieceLookup[to]==PieceType.None ? moveFlag.quietMove : moveFlag.capture;

        if (NotationHelper.boardNotation[from]=="e1" && NotationHelper.boardNotation[to]=="g1" && board.pieceLookup[from]==PieceType.King ||
            NotationHelper.boardNotation[from]=="e8" && NotationHelper.boardNotation[to]=="g8" && board.pieceLookup[from]==PieceType.King)
                flag = moveFlag.kingsideCastle;
        else if (NotationHelper.boardNotation[from]=="e1" && NotationHelper.boardNotation[to]=="c1" && board.pieceLookup[from]==PieceType.King ||
            NotationHelper.boardNotation[from]=="e8" && NotationHelper.boardNotation[to]=="c8" && board.pieceLookup[from]==PieceType.King)
                flag = moveFlag.queensideCastle;
        else if (board.pieceLookup[from]==PieceType.Pawn && (to<8 || to>55))
        {
            flag = board.pieceLookup[to]==PieceType.None ? moveFlag.queenPromotion : 
                                                           moveFlag.queenPromotionCapture;
        }
    }

    public int toInt ()
    {
        return from | (to << 6) | ((int) flag << 12);
    }
    
}