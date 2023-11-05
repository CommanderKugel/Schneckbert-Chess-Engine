
public struct Gamestate
{
    public PieceType capturedPiece;
    public int enPassant;
    public int castlingRights;
    public ulong zobristKey;
    public int fiftyMoveCounter;


    public Gamestate(PieceType capturedPiece, int enPassant, int castlingRights, ulong zobristKey, int fiftyMoveCounter) 
    {
        this.capturedPiece = capturedPiece;
        this.enPassant = enPassant;
        this.castlingRights = castlingRights;
        this.zobristKey = zobristKey;
        this.fiftyMoveCounter = fiftyMoveCounter;
    }


    public const byte whiteKingside  = 1;
    public const byte whiteQueenside = 2;
    public const byte whiteBoth      = whiteKingside | whiteQueenside;

    public const byte blackKingside  = 4;
    public const byte blackQueenside = 8;
    public const byte blackBoth      = blackKingside | blackQueenside;

    public static readonly byte[] whiteMasks = new byte[] { byte.MaxValue ^ whiteKingside, byte.MaxValue ^ whiteQueenside };
    public static readonly byte[] blackMasks = new byte[] { byte.MaxValue ^ blackKingside, byte.MaxValue ^ blackQueenside };
    public static readonly byte[][] castlingMasks = { blackMasks, whiteMasks };
    public static readonly byte[] anyMask = new byte[] { blackBoth, whiteBoth };

    public static readonly byte[] whiteInvertedMasks = new byte[] { whiteKingside, whiteQueenside };
    public static readonly byte[] blackInvertedMasks = new byte[] { blackKingside, blackQueenside };
    public static readonly byte[][] invertedCastlingMasks = { blackInvertedMasks, whiteInvertedMasks };
    

    public bool hasCastlingRight(int us) {
        return (castlingRights & anyMask[us]) != 0;
    }

    public bool hasKingsideRights(int us) {
        return (castlingRights & invertedCastlingMasks[us][0]) != 0;
    } 
   
    public bool hasQueensideRights(int us) {
        return (castlingRights & invertedCastlingMasks[us][1]) != 0;
    }





    public override string ToString()
    {
        string str = "";
        if ((castlingRights & whiteKingside ) != 0) str+="K";
        if ((castlingRights & whiteQueenside) != 0) str+="Q";
        if ((castlingRights & blackKingside ) != 0) str+="k";
        if ((castlingRights & blackQueenside) != 0) str+="q";
        return "captured Piece:  "+capturedPiece+"\n"+
               "en Passant:      "+NotationHelper.boardNotation[enPassant]+"\n"+
               "castling Rights: "+str;
    }

}
