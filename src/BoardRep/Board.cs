
using System.Numerics;

public class Board
{
    const byte PAWN = 1;
    const byte KNIGHT = 2;
    const byte BISHOP = 3;
    const byte ROOK = 4;
    const byte QUEEN = 5;
    const byte KING = 6;



    // universal stuff
    public bool isWhiteToMove;
    public bool isInCheck;
    public int plyCount;

    // Data moves dont store
    public Stack<Gamestate> stateStack = new Stack<Gamestate>();
    public Gamestate currentGamestate;
    public RepititionTable repTable = new RepititionTable();

    public Stack<Move> moveStack = new Stack<Move>();

    // actual piece representation
    public ulong[][] allBitboards = new ulong[2][];
    public byte[] pieceLookup = new byte[64];

    

    public ulong zobristKey => currentGamestate.zobristKey;
    public bool isRepeatedPosition => repTable.isRepeatedPosition();
    public bool isFiftyMoveDraw => currentGamestate.fiftyMoveCounter == 100;
    public bool onlyKings => BitOperations.PopCount(allBitboards[0][0] | allBitboards[1][0]) == 2;


    public Board ()
    {
        isWhiteToMove = true;
        isInCheck = false;
        plyCount = 0;

        // bitboards & piece lookup
        allBitboards[0] = new ulong[7];
        allBitboards[1] = new ulong[7];
        clearBoard();
        stateStack.Clear();

        currentGamestate = new Gamestate(0, 8, 0, 0, 0);
        stateStack.Push(currentGamestate);
    }

    private static readonly int[] rookCastleSquares = { 63, 61, 7, 5 , 59, 56, 0, 3 };


    public bool hasCastlingRights(int us) => currentGamestate.hasCastlingRights(us);


    public void makeMove (Move move)
    {
        int us = isWhiteToMove ? 1 : 0;
        int them = 1-us;
        int from = move.from;
        int to = move.to;

        // newState vars
        int newEPFile = 8;
        int newCastlingRights = currentGamestate.castlingRights;
        ulong newZobrist = currentGamestate.zobristKey;

        byte movingPiece = pieceLookup[from];
        byte capturedPiece = pieceLookup[to];

        // make quiet move
        ulong bb = 1ul << from | 1ul << to;
        allBitboards[us][movingPiece] ^= bb;
        allBitboards[us][0] ^= bb;

        pieceLookup[from] = 0;
        pieceLookup[to] = movingPiece;

        newZobrist ^= Zobrist.pieceArray[us][movingPiece][from] ^ Zobrist.pieceArray[us][movingPiece][to];


        if (move.isCapture)
        {
            bb = 1ul << to;
            allBitboards[them][capturedPiece] ^= bb;
            allBitboards[them][0] ^= bb;

            newZobrist ^= Zobrist.pieceArray[them][capturedPiece][to];

            if (hasCastlingRights(them) && capturedPiece == ROOK)
            {
                if (to == (!isWhiteToMove ? 7 : 63))
                    newCastlingRights &= ~(0b0100 >> (them+them));

                if (to == (!isWhiteToMove ? 0 : 56))
                    newCastlingRights &= ~(0b1000 >> (them+them));
            }
        }

        if (hasCastlingRights(us))
        {
            if (movingPiece == KING)
                newCastlingRights &= ~(0b1100 >> (us+us));

            if (movingPiece == ROOK)
            {
                if (from == (isWhiteToMove ? 7 : 63))
                    newCastlingRights &= ~(0b0100 >> (us+us));

                if (from == (isWhiteToMove ? 0 : 56))
                    newCastlingRights &= ~(0b1000 >> (us+us));
            }
        }

        switch (move.flag)
        {
            case moveFlag.quietMove:
            case moveFlag.capture:
                break;
            case moveFlag.doublePawnPush:
                newEPFile = to & 7;
                newZobrist ^= Zobrist.enPassant[newEPFile];
                break;
            case moveFlag.enPassantCapture:
                capturedPiece = PAWN;

                int epPawn = currentGamestate.enPassantFile + (4 - them) * 8;
                bb = 1ul << epPawn;
                allBitboards[them][PAWN] ^= bb;
                allBitboards[them][0] ^= bb;
                pieceLookup[epPawn] = 0;
                break;

            case moveFlag.kingsideCastle:
                int start = isWhiteToMove ? 7 : 63;
                int end = isWhiteToMove ? 5 : 61;
                pieceLookup[start] = 0;
                pieceLookup[end] = ROOK;

                bb = 1ul << start | 1ul << end;
                allBitboards[us][ROOK] ^= bb;
                allBitboards[us][0] ^= bb;
                break;

            case moveFlag.queensideCastle:
                start = isWhiteToMove ? 0 : 56;
                end = isWhiteToMove ? 3 : 59;
                pieceLookup[start] = 0;
                pieceLookup[end] = ROOK;

                bb = 1ul << start | 1ul << end;
                allBitboards[us][ROOK] ^= bb;
                allBitboards[us][0] ^= bb;
                break;

            case moveFlag.queenPromotion:
            case moveFlag.queenPromotionCapture:
                bb = 1ul << to;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][QUEEN] ^= bb;
                pieceLookup[to] = QUEEN;
                break;
            case moveFlag.rookPromotion:
            case moveFlag.rookPromotionCapture:
                bb = 1ul << to;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][ROOK] ^= bb;
                pieceLookup[to] = ROOK;
                break;
            case moveFlag.bishopPromotion:
            case moveFlag.bishopPromotionCapture:
                bb = 1ul << to;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][BISHOP] ^= bb;
                pieceLookup[to] = BISHOP;
                break;    
            case moveFlag.knightPromotion:
            case moveFlag.knightPromotionCapture:
                bb = 1ul << to;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][KNIGHT] ^= bb;
                pieceLookup[to] = KNIGHT;
                break;
        }

        if (currentGamestate.enPassantFile < 8)
            newZobrist ^= Zobrist.enPassant[currentGamestate.enPassantFile];
        if (newCastlingRights != currentGamestate.castlingRights)
            newZobrist ^= Zobrist.castlingRights[currentGamestate.castlingRights] ^ Zobrist.castlingRights[newCastlingRights];
        newZobrist ^= Zobrist.sideToMove;

        
        isWhiteToMove = !isWhiteToMove;
        plyCount++;

        byte newFiftyMoveCounter = (byte) ((movingPiece == 1 || move.isCapture) ? 0 :
                                           1 + currentGamestate.fiftyMoveCounter);
        Gamestate newState = new Gamestate(capturedPiece, (byte) newEPFile, (byte) newCastlingRights, newZobrist, newFiftyMoveCounter);
        currentGamestate = newState;
        stateStack.Push(newState);
        repTable.push(newZobrist);

        moveStack.Push(move);
    }

  
    public void undoMove (Move move)
    {
        int us = isWhiteToMove ? 0 : 1;
        int them = 1-us;


        int from = move.from;
        int to = move.to;

        byte movingPiece = pieceLookup[to];
        byte capturedPiece = currentGamestate.capturedPiece;
        
        // undo quiet move
        ulong bb = 1ul << from | 1ul << to;
        allBitboards[us][movingPiece] ^= bb;
        allBitboards[us][0] ^= bb;

        pieceLookup[from] = movingPiece;
        pieceLookup[to] = capturedPiece;

        if (move.isCapture)
        {
            bb = 1ul << to;
            allBitboards[them][capturedPiece] ^= bb;
            allBitboards[them][0] ^= bb;
        }

        switch (move.flag)
        {
            case moveFlag.quietMove:
            case moveFlag.capture:
            case moveFlag.doublePawnPush:
                break;
            case moveFlag.enPassantCapture:
                int epPawn = (to & 7) + (isWhiteToMove ? 24 : 32);
                bb = 1ul << epPawn | 1ul << to;
                allBitboards[them][PAWN] ^= bb;
                allBitboards[them][0] ^= bb;

                pieceLookup[to] = 0;
                pieceLookup[epPawn] = PAWN;
                break;

            case moveFlag.kingsideCastle:
                int start = !isWhiteToMove ? 7 : 63;
                int end = !isWhiteToMove ? 5 : 61;
                pieceLookup[start] = ROOK;
                pieceLookup[end] = 0;

                bb = 1ul << start | 1ul << end;
                allBitboards[us][ROOK] ^= bb;
                allBitboards[us][0] ^= bb;
                break;

            case moveFlag.queensideCastle:
                start = !isWhiteToMove ? 0 : 56;
                end = !isWhiteToMove ? 3 : 59;
                pieceLookup[start] = ROOK;
                pieceLookup[end] = 0;

                bb = 1ul << start | 1ul << end;
                allBitboards[us][ROOK] ^= bb;
                allBitboards[us][0] ^= bb;
                break;

            case moveFlag.queenPromotion:
            case moveFlag.queenPromotionCapture:
                bb = 1ul << from;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][QUEEN] ^= bb;
                pieceLookup[from] = PAWN;
                break;
            case moveFlag.rookPromotion:
            case moveFlag.rookPromotionCapture:
                bb = 1ul << from;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][ROOK] ^= bb;
                pieceLookup[from] = PAWN;
                break;
            case moveFlag.bishopPromotion:
            case moveFlag.bishopPromotionCapture:
                bb = 1ul << from;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][BISHOP] ^= bb;
                pieceLookup[from] = PAWN;
                break;
            case moveFlag.knightPromotion:
            case moveFlag.knightPromotionCapture:
                bb = 1ul << from;
                allBitboards[us][PAWN] ^= bb;
                allBitboards[us][KNIGHT] ^= bb;
                pieceLookup[from] = PAWN;
                break;
        }


        isWhiteToMove = !isWhiteToMove;
        plyCount--;

        repTable.pop();
        stateStack.Pop();
        currentGamestate = stateStack.Peek();

        moveStack.Pop();
    }




    public void makeMove (string move)
    {
        Move newMove = new Move(move, this);
        makeMove(newMove);
    }

    public void undoMove (string move)
    {
        Move newMove = new Move(move, this);
        undoMove(newMove);
    }
    
    
    public int generateLegalMoves (ref Span<Move> moves)
    {
        return MoveGenLegal.moveGen(ref moves, this);
    }

    public int generateCaptures (ref Span<Move> moves)
    {
        return CaptureGenLegal.captureGen(ref moves, this);
    }

    public void clearBoard ()
    {
        for (int i=0; i<allBitboards.Length; i++)
        {
            for (int j=0; j<allBitboards[i].Length; j++) allBitboards[i][j]=0;
        }
        for (int i=0; i<64; i++) pieceLookup[i] = 0;
    }

    public void setPiece (int square, int color, byte piece)
    {
        allBitboards[color][piece] |= 1ul << square;
        allBitboards[color][0] |= 1ul << square;
        pieceLookup[square] = piece;
    }

    public void initFen (string fen=Perft.startingPos)
    {
        NotationHelper.initFen(this, fen);
    }
    
    public void printMoveHist()
    {
        foreach (Move move in moveStack)
            Console.WriteLine(move);
    }

    public void printLegalMoves()
    {
        Span<Move> moves = stackalloc Move[218];
        int moveCount = MoveGenLegal.moveGen(ref moves, this);
        Console.WriteLine($"moveCount: {moveCount}");

        for (int i=0; i<moveCount; i++)
            Console.WriteLine(moves[i]);
    }
}
