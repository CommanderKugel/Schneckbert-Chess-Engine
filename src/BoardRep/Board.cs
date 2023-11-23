

using System.Net;

public class Board
{
    // universal stuff
    public bool isWhiteToMove;
    public bool isInCheck;
    public int plyCount;
    public int fullMoveCount;
    public MoveGenPseudo genPseudo;
    public MoveGenLegal genLegal;

    // Data moves dont store
    public Stack<Move> moveHistory = new Stack<Move>();
    public Stack<Gamestate> stateHistory = new Stack<Gamestate>();
    public Gamestate currentGamestate;
    public RepititionTable repititionTable;

    // actual piece representation
    public ulong[][] allBitboards = new ulong[2][];
    public PieceType[] pieceLookup = new PieceType[64];


    public Board ()
    {
        isWhiteToMove = true;
        isInCheck = false;
        plyCount = 0;
        fullMoveCount = 0;
        genPseudo = new MoveGenPseudo();
        genLegal = new MoveGenLegal();

        // bitboards & piece lookup
        allBitboards[0] = new ulong[7];
        allBitboards[1] = new ulong[7];
        clearBoard();
        moveHistory.Clear();
        stateHistory.Clear();

        repititionTable = new RepititionTable();
        currentGamestate = new Gamestate(PieceType.None, 0, 0, 0, 0);
        stateHistory.Push(currentGamestate);
    }
    


    public void makeMove (Move move)
    {
        int us = isWhiteToMove ? 1 : 0;
        int them = 1 - us;

        PieceType movingPiece   = pieceLookup[move.from];
        PieceType capturedPiece = pieceLookup[move.to];

        int newEnPassant = 0;
        int newCastlingRights = currentGamestate.castlingRights;

        // update biboards of moved piece
        XORbitboards(1ul << move.to | 1ul << move.from, movingPiece, us);
        // update piece lookup
        pieceLookup[move.from] = PieceType.None;
        pieceLookup[move.to  ] = movingPiece;

        // update castlin rights if King or Rook are moved
        if ((newCastlingRights & (us==1 ? Gamestate.whiteBoth : Gamestate.blackBoth)) != 0)
        {
            if (movingPiece == PieceType.King) newCastlingRights &= us==1 ? 0xC : 0x3;
            
            else if (movingPiece == PieceType.Rook)
            {   
                // queen rook
                if      (move.from == (us==1 ? 0 : 56)) newCastlingRights &= Gamestate.castlingMasks[us][1];
                // king rook
                else if (move.from == (us==1 ? 7 : 63)) newCastlingRights &= Gamestate.castlingMasks[us][0];
            }

            if (capturedPiece == PieceType.Rook)
            {
                // queen rook
                if      (move.to == (them==1 ? 0 : 56)) newCastlingRights &= Gamestate.castlingMasks[them][1];
                // king rook
                else if (move.to == (them==1 ? 7 : 63)) newCastlingRights &= Gamestate.castlingMasks[them][0];
            }
        }


        switch (move.flag)
        {
            case moveFlag.quietMove:        break;
 
            case moveFlag.capture:          XORbitboards(move.to, capturedPiece, them);
                                            if (currentGamestate.hasCastlingRight(them) && capturedPiece == PieceType.Rook)
                                            {
                                                // queen rook
                                                if      (move.to == (them==1 ? 0 : 56)) newCastlingRights &= Gamestate.castlingMasks[them][1];
                                                // king rook
                                                else if (move.to == (them==1 ? 7 : 63)) newCastlingRights &= Gamestate.castlingMasks[them][0];
                                            }
                                            break;

            case moveFlag.doublePawnPush:   newEnPassant = move.to;
                                            break;

            case moveFlag.kingsideCastle:   int start = us==1 ? 7 : 63;
                                            int end   = us==1 ? 5 : 61;
                                            XORbitboards(1ul << start | 1ul << end, PieceType.Rook, us);
                                            pieceLookup[start] = PieceType.None;
                                            pieceLookup[end  ] = PieceType.Rook;
                                            newCastlingRights &= Gamestate.castlingMasks[us][0] & Gamestate.castlingMasks[us][1];
                                            break;
            
            case moveFlag.queensideCastle:  start = us==1 ? 0 : 56;
                                            end   = us==1 ? 3 : 59;
                                            XORbitboards(1ul << start | 1ul << end, PieceType.Rook, us);
                                            pieceLookup[start] = PieceType.None;
                                            pieceLookup[end  ] = PieceType.Rook;
                                            newCastlingRights &= Gamestate.castlingMasks[us][0] & Gamestate.castlingMasks[us][1];
                                            break;

            case moveFlag.enPassantCapture: capturedPiece = PieceType.Pawn;
                                            XORbitboards(currentGamestate.enPassant, PieceType.Pawn, them);
                                            pieceLookup[currentGamestate.enPassant] = PieceType.None;
                                            break;

            case moveFlag.queenPromotion:   makePromotion(move.to, PieceType.Queen, us);
                                            break;
            case moveFlag.rookPromotion:    makePromotion(move.to, PieceType.Rook, us);
                                            break;            
            case moveFlag.bishopPromotion:  makePromotion(move.to, PieceType.Bishop, us);
                                            break;
            case moveFlag.knightPromotion:  makePromotion(move.to, PieceType.Knight, us);
                                            break;

            case moveFlag.queenPromotionCapture:    makePromotion(move.to, PieceType.Queen, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;
            case moveFlag.rookPromotionCapture:     makePromotion(move.to, PieceType.Rook, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;            
            case moveFlag.bishopPromotionCapture:   makePromotion(move.to, PieceType.Bishop, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;
            case moveFlag.knightPromotionCapture:   makePromotion(move.to, PieceType.Knight, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;
        }

        isWhiteToMove = !isWhiteToMove;
        plyCount++;
        fullMoveCount += them;
        isInCheck = false;

        
        ulong newZobristKey = currentGamestate.zobristKey;
        newZobristKey ^= Zobrist.sideToMove;
        if (newEnPassant != 0) newZobristKey ^= Zobrist.enPassant[newEnPassant & 7];
        newZobristKey ^= Zobrist.pieceArray[us][(int) movingPiece - 1][move.to];
        
        newZobristKey ^= Zobrist.pieceArray[us][(int) movingPiece - 1][move.from];
        if (capturedPiece != PieceType.None) newZobristKey ^= Zobrist.pieceArray[them][(int) capturedPiece - 1][move.to];
        
        if (currentGamestate.castlingRights != newCastlingRights) 
        {
            newZobristKey ^= Zobrist.castlingRights[currentGamestate.castlingRights];
            newZobristKey ^= Zobrist.castlingRights[newCastlingRights];
        }
        
        repititionTable.push(newZobristKey);
        int newFiftyMoveCounter = 0;
        if (capturedPiece == PieceType.None && movingPiece != PieceType.Pawn)
            newFiftyMoveCounter = currentGamestate.fiftyMoveCounter + 1;    

        Gamestate newGamestate = new Gamestate(capturedPiece, newEnPassant, newCastlingRights, newZobristKey, newFiftyMoveCounter);
        currentGamestate = newGamestate;
        stateHistory.Push(newGamestate);

        moveHistory.Push(move);

    }

    public void makeNullMove ()
    {
        int us = isWhiteToMove ? 1 : 0;
        int them = 1 - us;

        int newEnPassant = 0;
        int newCastlingRights = currentGamestate.castlingRights;
        ulong newZobristKey = currentGamestate.zobristKey ^ Zobrist.sideToMove;
        int newFiftyMoveCounter = currentGamestate.fiftyMoveCounter + 1;

        isWhiteToMove = !isWhiteToMove;
        plyCount++;
        fullMoveCount += them;
        isInCheck = false;

        Gamestate newGamestate = new Gamestate(PieceType.None, newEnPassant, newCastlingRights, newZobristKey, newFiftyMoveCounter);
        currentGamestate = newGamestate;
        stateHistory.Push(newGamestate);
        moveHistory.Push(Move.nullMove);
        repititionTable.push(newZobristKey);
    }

    private void makePromotion (int to, PieceType type, int color)
    {
        XORbitboards(to, PieceType.Pawn, color);
        XORbitboards(to, type, color);
        pieceLookup[to] = type;
    }

    public void undoMove (Move move)
    {
        isWhiteToMove = !isWhiteToMove;
        int us = isWhiteToMove ? 1 : 0;
        int them = 1 - us;
        plyCount--;
        fullMoveCount -= them;
 

        PieceType movingPiece = pieceLookup[move.to];
        PieceType capturedPiece = currentGamestate.capturedPiece;


        // already discard the current gamestate to get en passant square
        stateHistory.Pop();
        currentGamestate = stateHistory.Peek();

        moveHistory.Pop();
        repititionTable.pop();

        // undo moving piece updates
        XORbitboards(1ul << move.to | 1ul << move.from, movingPiece, us);
        // undo piece lookup
        pieceLookup[move.from] = movingPiece;
        pieceLookup[move.to  ] = capturedPiece;

        switch (move.flag)
        {
            case moveFlag.quietMove:        break;
 
            case moveFlag.capture:          XORbitboards(move.to, capturedPiece, them);
                                            break;

            case moveFlag.doublePawnPush:   break;

            case moveFlag.kingsideCastle:   int start = us==1 ? 7 : 63;
                                            int end   = us==1 ? 5 : 61;
                                            XORbitboards(1ul << start | 1ul << end, PieceType.Rook, us);
                                            pieceLookup[start] = PieceType.Rook;
                                            pieceLookup[end  ] = PieceType.None;
                                            break;
            
            case moveFlag.queensideCastle:  start = us==1 ? 0 : 56;
                                            end   = us==1 ? 3 : 59;
                                            XORbitboards(1ul << start | 1ul << end, PieceType.Rook, us);
                                            pieceLookup[start] = PieceType.Rook;
                                            pieceLookup[end  ] = PieceType.None;
                                            break;

            case moveFlag.enPassantCapture: XORbitboards(currentGamestate.enPassant, capturedPiece, them);
                                            pieceLookup[move.to] = PieceType.None;
                                            pieceLookup[currentGamestate.enPassant] = capturedPiece;
                                            break;

            case moveFlag.queenPromotion:   undoPromotion(move.from, PieceType.Queen, us);
                                            break;
            case moveFlag.rookPromotion:    undoPromotion(move.from, PieceType.Rook, us);
                                            break;
            case moveFlag.bishopPromotion:  undoPromotion(move.from, PieceType.Bishop, us);
                                            break;
            case moveFlag.knightPromotion:  undoPromotion(move.from, PieceType.Knight, us);
                                            break;                                        

            case moveFlag.queenPromotionCapture:    undoPromotion(move.from, PieceType.Queen, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;
            case moveFlag.rookPromotionCapture:     undoPromotion(move.from, PieceType.Rook, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;
            case moveFlag.bishopPromotionCapture:   undoPromotion(move.from, PieceType.Bishop, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;
            case moveFlag.knightPromotionCapture:   undoPromotion(move.from, PieceType.Knight, us);
                                                    XORbitboards(move.to, capturedPiece, them);
                                                    break;                                                          
        }
    
    }

    public void undoNullMove()
    {
        isWhiteToMove = !isWhiteToMove;
        int us = isWhiteToMove ? 1 : 0;
        int them = 1 - us;
        plyCount--;
        fullMoveCount -= them;

        // already discard the current gamestate to get en passant square
        stateHistory.Pop();
        currentGamestate = stateHistory.Peek();

        moveHistory.Pop();
        repititionTable.pop();
    }

    private void undoPromotion (int from, PieceType type, int color)
    {
        // moving piece (aka promoted type) already on from square
        XORbitboards(from, type, color);
        XORbitboards(from, PieceType.Pawn, color);
        pieceLookup[from] = PieceType.Pawn;
    }


    private void XORbitboards (ulong mask, PieceType type, int color)
    {
        allBitboards[color][(int) type] ^= mask;
        allBitboards[color][         0] ^= mask;        
    }

    private void XORbitboards (int to, PieceType type, int color)
    {
        allBitboards[color][(int) type] ^= 1ul << to;
        allBitboards[color][         0] ^= 1ul << to;
    }








    public Move[] generateMovesPseudo (bool onlyCaptures=false)
    {
        return genPseudo.moveGen(this, onlyCaptures);
    }

    public Move[] generateLegalMoves (bool onlyCaptures=false)
    {
        return genLegal.moveGen(this, onlyCaptures);
    }



    public void printMoves (bool onlyCaptures = false)
    {
        Move[] moves = genPseudo.moveGen(this, onlyCaptures);
        Console.WriteLine("moves: "+moves.Length);
        for (int i=0; i<moves.Length; i++)
        {
            Console.WriteLine(i+": "+moves[i]);
        }
        
    }

    public void printMovehistory()
    {
        int len = moveHistory.Count;
        Move[] newHistory = new Move[len];
        for (int i=len-1; i>=0; i--) newHistory[i] = moveHistory.Pop();
        foreach (Move move in newHistory) Console.WriteLine(move);
    }

    public void clearBoard ()
    {
        for (int i=0; i<allBitboards.Length; i++)
        {
            for (int j=0; j<allBitboards[i].Length; j++) allBitboards[i][j]=0;
        }
        for (int i=0; i<64; i++) pieceLookup[i]=PieceType.None;
    }

    public void setPiece (int square, int color, PieceType piece)
    {
        allBitboards[color][(int) piece] |= 1ul << square;
        allBitboards[color][          0] |= 1ul << square;
        pieceLookup[square] = piece;
    }
}
