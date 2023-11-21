using System;
using System.Data.SqlTypes;
using System.Diagnostics;

public class NotationHelper
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
        board.repititionTable.Clear();
        board.stateHistory.Clear();
        board.moveHistory.Clear();
        
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
                case "P": board.setPiece(sq, 1, PieceType.Pawn);   continue;                          
                case "p": board.setPiece(sq, 0, PieceType.Pawn);   continue;                         
                case "N": board.setPiece(sq, 1, PieceType.Knight); continue;                        
                case "n": board.setPiece(sq, 0, PieceType.Knight); continue;                         
                case "B": board.setPiece(sq, 1, PieceType.Bishop); continue;
                case "b": board.setPiece(sq, 0, PieceType.Bishop); continue;                         
                case "R": board.setPiece(sq, 1, PieceType.Rook);   continue;                         
                case "r": board.setPiece(sq, 0, PieceType.Rook);   continue;                         
                case "Q": board.setPiece(sq, 1, PieceType.Queen);  continue;
                case "q": board.setPiece(sq, 0, PieceType.Queen);  continue;                         
                case "K": board.setPiece(sq, 1, PieceType.King);   continue;
                case "k": board.setPiece(sq, 0, PieceType.King);   continue;    
                case " ": break;  
            }
        }

        // side to move
        if (fen[stringIndex++].ToString() == "w") {
            board.isWhiteToMove = true;
        } else {
            board.isWhiteToMove = false;
        }

        bool[,] castlingRights = { { false, false }, { false, false } };

        for (entry = fen[++stringIndex].ToString(); entry!=" "; entry = fen[++stringIndex].ToString())
        {
            switch (entry) {
                case "K": castlingRights[1, (int) castle.kingSide ] = true; continue;
                case "Q": castlingRights[1, (int) castle.queenSide] = true; continue;
                case "k": castlingRights[0, (int) castle.kingSide ] = true; continue;
                case "q": castlingRights[0, (int) castle.queenSide] = true; continue;
                case "-": break;
            }
        }

        int caslingRightNum = 0;
        if (castlingRights[1,0]) caslingRightNum |= Gamestate.whiteKingside;
        if (castlingRights[1,1]) caslingRightNum |= Gamestate.whiteQueenside;
        if (castlingRights[0,0]) caslingRightNum |= Gamestate.blackKingside;
        if (castlingRights[0,1]) caslingRightNum |= Gamestate.blackQueenside;
        

        entry = fen[++stringIndex].ToString();

        int enPassant = 0;
        if (entry != "-")
        {
            entry += fen[++stringIndex].ToString();
            int index = stringToIndex(entry);
            int dir = board.isWhiteToMove ? -8 : 8;
            enPassant = index + dir;
        }

        
        Gamestate newState = new Gamestate(PieceType.None, enPassant, caslingRightNum, 0, 0);
        board.currentGamestate = newState;
        ulong newZobrist = Zobrist.calcZobrist(board);
        board.currentGamestate.zobristKey = newZobrist;
        board.stateHistory.Push(newState);

        board.repititionTable.push(newZobrist);

        board.plyCount = 0;
        board.fullMoveCount = 0;
    }

}
