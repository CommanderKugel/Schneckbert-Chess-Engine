
public static class NotationHelper
{
    public static readonly string[] boardNotation = new string[] 
    {
        "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1", 
        "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2", 
        "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3", 
        "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4", 
        "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5", 
        "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6", 
        "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7", 
        "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8"
    };

    
    public static PieceType strToPiece (string str)
    {
        switch (str[0].ToString())
        {
            case "P": return PieceType.Pawn;
            case "N": return PieceType.Knight;
            case "B": return PieceType.Bishop;
            case "R": return PieceType.Rook;
            case "Q": return PieceType.Queen;
            case "K": return PieceType.King;
            default:  return PieceType.None;
        }
    }

    public static int strToStartsquare (string str) {
        return stringToIndex(str[0].ToString() + str[1]);
    }

    public static int strToEndsquare (string str) {
        return stringToIndex(str[2].ToString() + str[3]);
    }

    public static int stringToIndex (string str) {
        for (int i=0; i<64; i++) if (boardNotation[i] == str) return i;
        Console.WriteLine("ERROR - BAD SQUARE STRING!");
        return 0;
    }


    public static string[] nums = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" };
    public static void initFen (Board board, string fen)
    {
        board.clearBoard();
        //board.repititionTable.Clear();
        board.stateStack.Clear();
        
        string entry=fen[0].ToString();

        int rank=7;
        int file=0;
        int sq, stringIndex;

        // putting down the pieces
        for (stringIndex=0; entry!=" "; stringIndex++, file++)
        {
            entry=fen[stringIndex].ToString();
            sq=8*rank + file;

            switch (entry) {
                case "1": continue;
                case "2": file++; continue;
                case "3": file+=2; continue;
                case "4": file+=3; continue;
                case "5": file+=4; continue;
                case "6": file+=5; continue;
                case "7": file+=6; continue;
                case "8": file=-1; continue;
                case "/": file=-1; rank--; continue;
                case "P": board.setPiece(sq, 1, 1); continue;                          
                case "p": board.setPiece(sq, 0, 1); continue;                         
                case "N": board.setPiece(sq, 1, 2); continue;                        
                case "n": board.setPiece(sq, 0, 2); continue;                         
                case "B": board.setPiece(sq, 1, 3); continue;
                case "b": board.setPiece(sq, 0, 3); continue;                         
                case "R": board.setPiece(sq, 1, 4); continue;                         
                case "r": board.setPiece(sq, 0, 4); continue;                         
                case "Q": board.setPiece(sq, 1, 5); continue;
                case "q": board.setPiece(sq, 0, 5); continue;                         
                case "K": board.setPiece(sq, 1, 6); continue;
                case "k": board.setPiece(sq, 0, 6); continue;    
                case " ": break;  
            }
        }

        // side to move
        if (fen[stringIndex++].ToString() == "w") {
            board.isWhiteToMove = true;
        } else {
            board.isWhiteToMove = false;
        }

        byte castlingRightsNum = 0;
        for (entry = fen[++stringIndex].ToString(); entry!=" "; entry = fen[++stringIndex].ToString())
        {
            switch (entry) {
                case "K": castlingRightsNum |= 0b0001; continue;
                case "Q": castlingRightsNum |= 0b0010; continue;
                case "k": castlingRightsNum |= 0b0100; continue;
                case "q": castlingRightsNum |= 0b1000; continue;
                case "-": break;
            }
        }

        entry = fen[++stringIndex].ToString();

        int enPassant = 8;
        if (entry != "-")
        {
            entry += fen[++stringIndex].ToString();
            int index = stringToIndex(entry);
            int dir = board.isWhiteToMove ? -8 : 8;
            enPassant = index + dir;
        }
        
        
        ulong newZobrist = Zobrist.calcZobrist(board);
        board.repTable.push(newZobrist);
        
        Gamestate newState = new Gamestate(0, (byte) enPassant, castlingRightsNum, newZobrist, 0);
        board.currentGamestate = newState;

        board.stateStack.Push(newState);

        board.plyCount = 0;
    }

}
