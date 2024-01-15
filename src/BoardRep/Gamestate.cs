
public struct Gamestate
{
    public byte capturedPiece;
    public byte enPassantFile;
    public byte castlingRights;
    public ulong zobristKey;
    public byte fiftyMoveCounter;


    public Gamestate(byte capturedPiece, byte enPassantFile, byte castlingRights, ulong zobristKey, byte fiftyMoveCounter) 
    {
        this.capturedPiece = capturedPiece;
        this.enPassantFile = enPassantFile;
        this.castlingRights = castlingRights;
        this.zobristKey = zobristKey;
        this.fiftyMoveCounter = fiftyMoveCounter;
    }


    

    public bool hasCastlingRights(int us) => (castlingRights & (0b1100 >> (us+us))) != 0;

    public bool hasKingsideRights(int us) => (castlingRights & (0b0100 >> (us+us))) != 0;
    
    public bool hasQueensideRights(int us) => (castlingRights & (0b1000 >> (us+us))) != 0;



    public override string ToString()
    {
        string str = "";
        if ((castlingRights & 0b0001) != 0) str+="K";
        if ((castlingRights & 0b0010) != 0) str+="Q";
        if ((castlingRights & 0b0100) != 0) str+="k";
        if ((castlingRights & 0b1000) != 0) str+="q";
        return "captured Piece:  "+capturedPiece+"\n"+
               "en Passant:      "+(enPassantFile > 7 ? "none" : enPassantFile)+"\n"+
               "castling Rights: "+str;
    }

}
