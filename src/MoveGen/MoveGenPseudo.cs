
using System.Diagnostics;
using System.Numerics;

public class MoveGenPseudo
{


    const int PAWN   = (int) PieceType.Pawn;
    const int KNIGHT = (int) PieceType.Knight;
    const int BISHOP = (int) PieceType.Bishop;
    const int ROOK   = (int) PieceType.Rook;
    const int QUEEN  = (int) PieceType.Queen;
    const int KING   = (int) PieceType.King;

    const int rookShift = 52;
    const int bishopShift = 55;

    private ulong kingAttacks (int index)
    {
        return PrecomputedData.KingAttacks[index];
    }

    private ulong knightAttacks (int index)
    {
        return PrecomputedData.KnightAttacks[index];
    }

    private ulong orthogonalAttacks (int index, ulong blocker)
    {
        blocker &= Magic.rookMasks[index];
        blocker *= Magic.rookMagics[index];
        blocker >>= rookShift;
        return Magic.rookAttacks[index][blocker];
    }

    private ulong diagonalAttacks (int index, ulong blocker)
    {
        blocker &= Magic.bishopMasks[index];
        blocker *= Magic.bishopMagics[index];
        blocker >>= bishopShift;
        return Magic.bishopAttacks[index][blocker];
    }


    private void extract (int from, ulong mask, moveFlag flag, ref Span<Move> moves)
    {
        int to;
        while (mask != 0)   
        {
            to = Helper.popLSB(ref mask);
            moves[moveIndex++] = new Move(from, to, flag);
        }
    }


    private void extractPromotions (int from, int to, ref Span<Move> moves)
    {
        moves[moveIndex++] = new Move(from, to, moveFlag.queenPromotion);
        moves[moveIndex++] = new Move(from, to, moveFlag.rookPromotion);
        moves[moveIndex++] = new Move(from, to, moveFlag.bishopPromotion);
        moves[moveIndex++] = new Move(from, to, moveFlag.knightPromotion);
    }

    private void exreactCapturePromotions (int from, int to, ref Span<Move> moves)
    {
        moves[moveIndex++] = new Move(from, to, moveFlag.queenPromotionCapture);
        moves[moveIndex++] = new Move(from, to, moveFlag.rookPromotionCapture);
        moves[moveIndex++] = new Move(from, to, moveFlag.bishopPromotionCapture);
        moves[moveIndex++] = new Move(from, to, moveFlag.knightPromotionCapture);
    }


    // constants
    public const ulong notAFile    = 0xFEFEFEFEFEFEFEFE;
    public const ulong notHFile    = 0x7F7F7F7F7F7F7F7F;

    const int whiteRight = -9;
    const int whiteUp    = -8;
    const int whiteLeft  = -7;
    const int blackRight =  7;
    const int blackDown  =  8;
    const int blackLeft  =  9;
    private static readonly int[] whiteDirs = { whiteRight, whiteUp,   whiteLeft };
    private static readonly int[] blackDirs = { blackRight, blackDown, blackLeft };
    private static readonly int[][] pawnDirs = { blackDirs, whiteDirs };
    const ulong secondRank  = 0x000000000000FF00;
    const ulong seventhRank = 0x00FF000000000000;
    public static readonly ulong[] preBaseRanks = new ulong[] { seventhRank, secondRank };



    const byte blackKingside  = 1 << 0;
    const byte blackQueenside = 1 << 1;
    const byte whiteKingside  = 1 << 2;
    const byte whiteQueenside = 1 << 3;
    public static byte[] castlingConstants = { blackKingside, blackQueenside, whiteKingside, whiteQueenside };

    private static ulong[] whiteCastlingSquares = { 0x60,               0x0C,                 0xE };
    private static ulong[] blackCastlingSquares = { 0x6000000000000000, 0x0C00000000000000, 0x0E00000000000000 };
    public static ulong[][] castlingSquares = { blackCastlingSquares, whiteCastlingSquares };

    public const ulong whiteRookKingside  = 1ul << 7;
    public const ulong whiteRookQueenside = 1ul << 0;
    public const ulong blackRookKingside  = 1ul << 63;
    public const ulong blackRookQueenside = 1ul << 56;
    public static readonly ulong[] whiteRookBoards = new ulong[] { whiteRookKingside, whiteRookQueenside };
    public static readonly ulong[] blackRookBoards = new ulong[] { blackRookKingside, blackRookQueenside };
    public static readonly ulong[][] rookCastlingSquares = { blackRookBoards, whiteRookBoards };



    int us;
    int them;
    Board board;

    ulong[][] bitboards;
    ulong checkMask;
    ulong enemyAttackMask;

    ulong blocker;
    ulong enemy;
    ulong empty;
    ulong empytOrEnemy;

    ulong enemyDiags;
    ulong enemyOrthos;

    ulong relevantPieces;
    ulong mask;


    int moveIndex;
    int kingIndex;
    int enemyKingIndex;
    int pieceIndex;



    public Move[] moveGen (Board board, bool onlyCaptures=false)
    {

        // initiate relevant variables
        Span<Move> moves = new Move[218];

        us   = board.isWhiteToMove ? 1 : 0;
        them = board.isWhiteToMove ? 0 : 1;
        this.board = board;

        bitboards = board.allBitboards;
        checkMask = 0;
        enemyAttackMask = 0;

        blocker = bitboards[0][0] | bitboards[1][0];
        enemy = bitboards[them][0];
        empty = ~blocker;
        empytOrEnemy = empty | enemy;

        enemyDiags = bitboards[them][BISHOP] | bitboards[them][QUEEN];
        enemyOrthos = bitboards[them][ROOK] | bitboards[them][QUEEN];

        moveIndex = 0;
        kingIndex = BitOperations.TrailingZeroCount(bitboards[us][KING]);
        enemyKingIndex = BitOperations.TrailingZeroCount(bitboards[them][KING]);


        // 
        //  start with check generation
        //

        ulong knightChecks = knightAttacks(kingIndex) & bitboards[them][KNIGHT];
        ulong diagChecks = diagonalAttacks(kingIndex, blocker) & enemyDiags;
        ulong orthoChecks = orthogonalAttacks(kingIndex, blocker) & enemyOrthos;
        ulong pawnCheks = us==1 ? ((bitboards[us][KING] & notHFile) << 9) & bitboards[them][PAWN] |
                                  ((bitboards[us][KING] & notAFile) << 7) & bitboards[them][PAWN] :
                                  ((bitboards[us][KING] & notHFile) >> 7) & bitboards[them][PAWN] |
                                  ((bitboards[us][KING] & notAFile) >> 9) & bitboards[them][PAWN] ;

        ulong leaperChecks = knightChecks | pawnCheks;
        ulong sliderChecks = orthoChecks | diagChecks;

        switch (BitOperations.PopCount(leaperChecks | sliderChecks))
        {
            case 0: checkMask = ulong.MaxValue;
                    break;
            case 1: if (leaperChecks != 0) checkMask = leaperChecks;
                    else checkMask = PrecomputedData.checkMasks[kingIndex][BitOperations.TrailingZeroCount(sliderChecks)];
                    onlyCaptures = false;
                    break;
            case 2: checkMask = 0;
                    break;
        }


        //
        // generate attack mask for king movement & castling
        //

        // King
        enemyAttackMask |= kingAttacks(enemyKingIndex);
        // Knight
        relevantPieces = bitboards[them][KNIGHT];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            enemyAttackMask |= knightAttacks(pieceIndex);
        }
        // Diagonal sliders
        // blockers dont contain our king because xrays
        // enemy King is a valid blocker tho
        blocker ^= 1ul << kingIndex;
        relevantPieces = bitboards[them][BISHOP] | bitboards[them][QUEEN];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            enemyAttackMask |= diagonalAttacks(pieceIndex, blocker);
        }
        // Orthogonal sliders
        relevantPieces = bitboards[them][ROOK] | bitboards[them][QUEEN];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            enemyAttackMask |= orthogonalAttacks(pieceIndex, blocker);
        }
        // Pawns
        // first right
        relevantPieces = bitboards[them][PAWN] & notHFile;
        enemyAttackMask |= them==1 ? relevantPieces << 9 : relevantPieces >> 7;
        // then left
        relevantPieces = bitboards[them][PAWN] & notAFile;
        enemyAttackMask |= them==1 ? relevantPieces << 7 : relevantPieces >> 9;

        //
        //  return Kingmoves when in double check
        //  else generate Moves normally
        //

        if (checkMask == 0) 
        {
            mask = kingAttacks(kingIndex) & ~enemyAttackMask;
            extract(kingIndex, mask & enemy, moveFlag.capture, ref moves);
            extract(kingIndex, mask & empty, moveFlag.quietMove, ref moves);
            return moves.Slice(0, moveIndex).ToArray();
        }

        //
        //  promotions
        //

        relevantPieces = bitboards[us][PAWN] & preBaseRanks[them];
        if (relevantPieces != 0)        
        {
            // pushes
            mask = us==1 ? relevantPieces << 8 : relevantPieces >> 8;
            mask &= empty & checkMask;
            while (mask != 0)
            {
                pieceIndex = Helper.popLSB(ref mask);
                extractPromotions(pieceIndex+pawnDirs[us][1], pieceIndex, ref moves);
            }
            // right captures
            mask = us==1 ? (relevantPieces & notHFile) << 9 : (relevantPieces & notHFile) >> 7;
            mask &= enemy & checkMask;
            while (mask != 0)
            {
                pieceIndex = Helper.popLSB(ref mask);
                exreactCapturePromotions(pieceIndex+pawnDirs[us][0], pieceIndex, ref moves);
            }
            // left captures
            mask = us==1 ? (relevantPieces & notAFile) << 7 : (relevantPieces & notAFile) >> 9;
            mask &= enemy & checkMask;
            while (mask != 0)
            {
                pieceIndex = Helper.popLSB(ref mask);
                exreactCapturePromotions(pieceIndex+pawnDirs[us][2], pieceIndex, ref moves);
            }
        }

        // 
        //  captures
        //

        // knights
        relevantPieces = bitboards[us][KNIGHT];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = knightAttacks(pieceIndex);
            mask &= checkMask & enemy;
            extract(pieceIndex, mask, moveFlag.capture, ref moves);
        }
        // blocker now contain our king again
        blocker ^= 1ul << kingIndex;

        // diagonal sliders
        relevantPieces = bitboards[us][BISHOP] | bitboards[us][QUEEN];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = diagonalAttacks(pieceIndex, blocker);
            mask &= checkMask & enemy;
            extract(pieceIndex, mask, moveFlag.capture, ref moves);
        }
        // orthogonal sliders
        relevantPieces = bitboards[us][ROOK] | bitboards[us][QUEEN];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = orthogonalAttacks(pieceIndex, blocker);
            mask &= checkMask & enemy;
            extract(pieceIndex, mask, moveFlag.capture, ref moves);
        }

        // pawns
        // first right
        relevantPieces = bitboards[us][PAWN] & notHFile & ~preBaseRanks[them];
        relevantPieces = us==1 ? relevantPieces << 9 : relevantPieces >> 7;
        relevantPieces &= checkMask & enemy;
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            moves[moveIndex++] = new Move(pieceIndex+pawnDirs[us][0], pieceIndex, moveFlag.capture);
        }
        // now left
        relevantPieces = bitboards[us][PAWN] & notAFile & ~preBaseRanks[them];
        relevantPieces = us==1 ? relevantPieces << 7 : relevantPieces >> 9;
        relevantPieces &= checkMask & enemy;
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            moves[moveIndex++] = new Move(pieceIndex+pawnDirs[us][2], pieceIndex, moveFlag.capture);
        }

        // King
        mask = kingAttacks(kingIndex) & enemy & ~enemyAttackMask;
        extract(kingIndex, mask, moveFlag.capture, ref moves);

        //
        //  en passant
        //

        int enPassant = board.currentGamestate.enPassant;
        if (enPassant != 0 && knightChecks == 0)
        {
            ulong targetSquare = us==1 ? 1ul << (enPassant+8) : 1ul << (enPassant-8);
            if (pawnCheks == 0 || 1ul << enPassant == pawnCheks)
            {
                mask = ((1ul << (enPassant+1)) & notAFile) |
                       ((1ul << (enPassant-1)) & notHFile);

                relevantPieces = bitboards[us][PAWN] & mask;

                while (relevantPieces != 0)
                {
                    pieceIndex = Helper.popLSB(ref relevantPieces);
                    // always test move for legality
                    mask = targetSquare | 1ul << pieceIndex | 1ul << enPassant;
                    blocker ^= mask;
                    if ((diagonalAttacks(kingIndex, blocker) & enemyDiags) == 0 &&
                        (orthogonalAttacks(kingIndex, blocker) & enemyOrthos) == 0)
                        {
                            moves[moveIndex++] = new Move(pieceIndex, BitOperations.TrailingZeroCount(targetSquare), moveFlag.enPassantCapture);
                        }
                    blocker ^= mask;
                }
            }
        }

        if (onlyCaptures)
        {
            legalityCheck(ref moves);
            return moves.Slice(0, moveIndex).ToArray();
        }


        //
        //  castling
        //

        if (board.currentGamestate.hasKingsideRights(us) &&
            checkMask == ulong.MaxValue &&
            ((blocker | enemyAttackMask) & castlingSquares[us][0]) == 0)
        {   
            moves[moveIndex++] = new Move(kingIndex, us==1 ? 6 : 62, moveFlag.kingsideCastle);
        }

        if (board.currentGamestate.hasQueensideRights(us) &&
            checkMask == ulong.MaxValue &&
            (enemyAttackMask & castlingSquares[us][1]) == 0 &&
            (blocker & castlingSquares[us][2]) == 0)
        {   
            moves[moveIndex++] = new Move(kingIndex, us==1 ? 2 : 58, moveFlag.queensideCastle);
        }
        

        //
        //  quiet moves
        //

        // knights
        relevantPieces = bitboards[us][KNIGHT];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = knightAttacks(pieceIndex);
            mask &= checkMask & empty;
            extract(pieceIndex, mask, moveFlag.quietMove, ref moves);
        }
        // diagonal sliders
        relevantPieces = bitboards[us][BISHOP] | bitboards[us][QUEEN];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = diagonalAttacks(pieceIndex, blocker);
            mask &= checkMask & empty;
            extract(pieceIndex, mask, moveFlag.quietMove, ref moves);
        }
        // orthogonal sliders
        relevantPieces = bitboards[us][ROOK] | bitboards[us][QUEEN];
        while (relevantPieces != 0)
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            mask = orthogonalAttacks(pieceIndex, blocker);
            mask &= checkMask & empty;
            extract(pieceIndex, mask, moveFlag.quietMove, ref moves);
        }
        // single pawn pushes
        relevantPieces = bitboards[us][PAWN] & ~preBaseRanks[them];
        relevantPieces = us==1 ? relevantPieces << 8 : relevantPieces >> 8;
        relevantPieces &= checkMask & empty;
        while (relevantPieces != 0) 
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            moves[moveIndex++] = new Move(pieceIndex+pawnDirs[us][1], pieceIndex, moveFlag.quietMove);
        }
        // double pawn pushes
        relevantPieces = bitboards[us][PAWN] & preBaseRanks[us];
        relevantPieces = us==1 ? (relevantPieces << 8) & empty : (relevantPieces >> 8) & empty;
        relevantPieces = us==1 ? (relevantPieces << 8) & empty : (relevantPieces >> 8) & empty;
        relevantPieces &= checkMask;
        while (relevantPieces != 0) 
        {
            pieceIndex = Helper.popLSB(ref relevantPieces);
            moves[moveIndex++] = new Move(pieceIndex+pawnDirs[us][1]+pawnDirs[us][1], pieceIndex, moveFlag.doublePawnPush);
        }
        // King
        mask = kingAttacks(kingIndex) & empty & ~enemyAttackMask;
        extract(kingIndex, mask, moveFlag.quietMove, ref moves);



        // now make pseudo legal moves legal moves
        legalityCheck(ref moves);
        return moves.Slice(0, moveIndex).ToArray();
    }


    private void legalityCheck (ref Span<Move> moves)
    {
        int j=0;
        for (int i=0; i<moveIndex; i++)
        {            

            // only try moves that have the potential to be pinned
            // king moves & castling are never checked, because checkMask never overlaps
            if (PrecomputedData.checkMasks[kingIndex][moves[i].from] != 0)
            {
                Move move = moves[i];


                // quiet moves, promotions, etc
                if (board.pieceLookup[move.to] == PieceType.None)
                {
                    if (move.flag != moveFlag.enPassantCapture)
                    {
                        blocker ^= 1ul << move.from | 1ul << move.to;
                        mask = (diagonalAttacks(kingIndex, blocker) & enemyDiags) | 
                            (orthogonalAttacks(kingIndex, blocker) & enemyOrthos);
                        blocker ^= 1ul << move.from | 1ul << move.to;
                    }
                    else 
                    {
                        blocker ^= 1ul << move.from | 1ul << move.to | 1ul << board.currentGamestate.enPassant;
                        mask = (diagonalAttacks(kingIndex, blocker) & enemyDiags) | 
                            (orthogonalAttacks(kingIndex, blocker) & enemyOrthos);
                        blocker ^= 1ul << move.from | 1ul << move.to | 1ul << board.currentGamestate.enPassant;
                    }
                }
                // captures
                else 
                {
                    blocker ^= 1ul << move.from;
                    mask = (diagonalAttacks(kingIndex, blocker) & enemyDiags) | 
                        (orthogonalAttacks(kingIndex, blocker) & enemyOrthos);
                    // capturing the pinning piece is allowed :)
                    mask &= ~(1ul << move.to);
                    blocker ^= 1ul << move.from;  
                }

                if (mask == 0) 
                {
                    moves[j++] = moves[i];
                } 

            }
            else 
            {
                moves[j++] = moves[i];
            }
        }
        moveIndex = j;      
    }

}
