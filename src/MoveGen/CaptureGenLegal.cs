
using System.Numerics;

public static class CaptureGenLegal
{

    const byte PAWN   = 1;
    const byte KNIGHT = 2;
    const byte BISHOP = 3;
    const byte ROOK   = 4;
    const byte QUEEN  = 5;
    const byte KING   = 6;

    const int rookShift = 52;
    const int bishopShift = 55;

    public const ulong notAFile    = 0xFEFEFEFEFEFEFEFE;
    public const ulong notHFile    = 0x7F7F7F7F7F7F7F7F;

    public const ulong firstRank   = 0x0000_0000_0000_00FF;
    public const ulong secondRank  = 0x0000_0000_0000_FF00;
    public const ulong thirdRank   = 0x0000_0000_00FF_0000;
    public const ulong fourthRank  = 0x0000_0000_FF00_0000;
    public const ulong sixthRank   = 0x0000_FF00_0000_0000;
    public const ulong seventhRank = 0x00FF_0000_0000_0000;
    public const ulong eighthRank  = 0xFF00_0000_0000_0000;



    private static ulong[] pinMaskArray = new ulong[64];

    public static int captureGen (ref Span<Move> moves, Board board)
    {
        int us = board.isWhiteToMove ? 1 : 0;
        int them = 1-us;

        int moveCount = 0;
        ulong[][] allBitboards = board.allBitboards;

        ulong enemy = allBitboards[them][0];
        ulong friendly = allBitboards[us][0];
        ulong blocker = enemy | friendly;
        ulong empty = ~blocker;

        ulong enemyAttackMask = 0;
        ulong checkMask = ulong.MaxValue;
        ulong pinnedPieces = 0;


        int kingIndex = BitOperations.TrailingZeroCount(allBitboards[us][KING]);
        if (allBitboards[us][KING] == 0 || allBitboards[them][KING] == 0)
        {
            Console.WriteLine($"ERROR found in MoveGeneration! A King is Missing");
            Draw.drawBoard(board);
            board.printMoveHist();
            throw null;
        }
        

        initEnemyAttackMask();
        initCheckAndPin();

        if (board.isInCheck)
            return MoveGenLegal.moveGen(ref moves, board);

        generateKingMoves(ref moves);
        generateKnightMoves(ref moves);
        generateDiagMoves(ref moves);
        generateOrthoMoves(ref moves);
        generatePawnPushPromotions(ref moves);
        generatePawnCaptures(ref moves);
        generateEPCaptures(ref moves);

        return moveCount;



        /*
        / LOCAL METHODS
        */

        void generateEPCaptures(ref Span<Move> moves)
        {
            if (board.currentGamestate.enPassantFile > 7)
                return;


            int epFile = board.currentGamestate.enPassantFile;
            int epTarget = epFile + (5 - 3*them) * 8;
            int epPawn = epFile + (4 - them) * 8;

            ulong attackers = (1ul << (epPawn+1)) & notAFile | (1ul << (epPawn-1)) & notHFile;
            attackers &= allBitboards[us][PAWN];

            while (attackers != 0)
            {
                int from = Helper.popLSB(ref attackers);
                ulong bb = 1ul << epPawn | 1ul << from | 1ul << epTarget;
                ulong newBlocker = blocker ^ bb;

                ulong diagChecks = diagonalAttacks(kingIndex, newBlocker) & (allBitboards[them][BISHOP] | allBitboards[them][QUEEN]);
                ulong orthoChecks = orthogonalAttacks(kingIndex, newBlocker) & (allBitboards[them][ROOK] | allBitboards[them][QUEEN]);

                if (diagChecks == 0 && orthoChecks == 0)
                    moves[moveCount++] = new Move(from, epTarget, moveFlag.enPassantCapture);
            }
        }

        void generatePawnCaptures(ref Span<Move> moves)
        {
            ulong pawns = allBitboards[us][PAWN];
            int right;
            int left;
            ulong rights;
            ulong lefts;
            ulong rightPinned;
            ulong leftPinned;
            ulong rightPromos;
            ulong leftPromos;


            if (board.isWhiteToMove)
            {
                rightPromos = ((pawns & ~pinnedPieces & seventhRank) << 9) & notAFile & enemy & checkMask;
                leftPromos = ((pawns & ~pinnedPieces & seventhRank) << 7) & notHFile & enemy & checkMask;
                pawns &= ~seventhRank;
                right = 9;
                left = 7;
                rights = ((pawns & ~pinnedPieces) << 9) & notAFile & enemy & checkMask;
                lefts = ((pawns & ~pinnedPieces) << 7) & notHFile & enemy & checkMask;
                rightPinned = ((pawns & pinnedPieces) << 9) & notAFile & enemy & checkMask;
                leftPinned = ((pawns & pinnedPieces) << 7) & notHFile & enemy & checkMask;
            }
            else
            {
                rightPromos = ((pawns & ~pinnedPieces & secondRank) >> 7) & notAFile & enemy & checkMask;
                leftPromos = ((pawns & ~pinnedPieces & secondRank) >> 9) & notHFile & enemy & checkMask;
                pawns &= ~secondRank;
                right = -7;
                left = -9;
                rights = ((pawns & ~pinnedPieces) >> 7) & notAFile & enemy & checkMask;
                lefts = ((pawns & ~pinnedPieces) >> 9) & notHFile & enemy & checkMask;
                rightPinned = ((pawns & pinnedPieces) >> 7) & notAFile & enemy & checkMask;
                leftPinned = ((pawns & pinnedPieces) >> 9) & notHFile & enemy & checkMask;
            }


            while (rights != 0)
            {
                int to = Helper.popLSB(ref rights);
                moves[moveCount++] = new Move(to-right, to, moveFlag.capture);
            }
            while (lefts != 0)
            {
                int to = Helper.popLSB(ref lefts);
                moves[moveCount++] = new Move(to-left, to, moveFlag.capture);
            }
            while (rightPinned != 0)
            {
                int to = Helper.popLSB(ref rightPinned);
                int from = to-right;
                if (((1ul << to) & pinMaskArray[from]) != 0)
                    moves[moveCount++] = new Move(from, to, moveFlag.capture);
            }
            while (leftPinned != 0)
            {
                int to = Helper.popLSB(ref leftPinned);
                int from = to-left;
                if (((1ul << to) & pinMaskArray[from]) != 0)
                    moves[moveCount++] = new Move(from, to, moveFlag.capture);
            }
            while (rightPromos != 0)
            {
                int to = Helper.popLSB(ref rightPromos);
                int from = to-right;
                if (((1ul << from) & pinnedPieces) == 0 || ((1ul << to) & pinMaskArray[from]) != 0)
                {
                    moves[moveCount++] = new Move(from, to, moveFlag.queenPromotionCapture);
                    moves[moveCount++] = new Move(from, to, moveFlag.rookPromotionCapture);
                    moves[moveCount++] = new Move(from, to, moveFlag.bishopPromotionCapture);
                    moves[moveCount++] = new Move(from, to, moveFlag.knightPromotionCapture);
                }
            }
            while (leftPromos != 0)
            {
                int to = Helper.popLSB(ref leftPromos);
                int from = to-left;
                if (((1ul << from) & pinnedPieces) == 0 || ((1ul << to) & pinMaskArray[from]) != 0)
                {
                    moves[moveCount++] = new Move(from, to, moveFlag.queenPromotionCapture);
                    moves[moveCount++] = new Move(from, to, moveFlag.rookPromotionCapture);
                    moves[moveCount++] = new Move(from, to, moveFlag.bishopPromotionCapture);
                    moves[moveCount++] = new Move(from, to, moveFlag.knightPromotionCapture);
                }
            }
        }


        void generatePawnPushPromotions(ref Span<Move> moves)
        {
            ulong pawns = allBitboards[us][PAWN];
            int dir;
            ulong promos;
            ulong pinnedPromos;

            if (board.isWhiteToMove)
            {
                dir = -8;
                promos = ((pawns & seventhRank & ~pinnedPieces) << 8) & empty & checkMask;
                pinnedPromos = ((pawns & seventhRank & pinnedPieces) << 8) & empty & checkMask;
            }
            else
            {
                dir = 8;
                promos = ((pawns & secondRank & ~pinnedPieces) >> 8) & empty & checkMask;
                pinnedPromos = ((pawns & secondRank & pinnedPieces) >> 8) & empty & checkMask;
            }

            while (promos != 0)
            {
                int to = Helper.popLSB(ref promos);
                int from = to + dir;
                moves[moveCount++] = new Move(from, to, moveFlag.queenPromotion);
                moves[moveCount++] = new Move(from, to, moveFlag.rookPromotion);
                moves[moveCount++] = new Move(from, to, moveFlag.bishopPromotion);
                moves[moveCount++] = new Move(from, to, moveFlag.knightPromotion);
            }
            while (pinnedPromos != 0)
            {
                int to = Helper.popLSB(ref pinnedPromos);
                int from = to + dir;
                if (((1ul << to) & pinMaskArray[from]) != 0)
                {
                    moves[moveCount++] = new Move(from, to, moveFlag.queenPromotion);
                    moves[moveCount++] = new Move(from, to, moveFlag.rookPromotion);
                    moves[moveCount++] = new Move(from, to, moveFlag.bishopPromotion);
                    moves[moveCount++] = new Move(from, to, moveFlag.knightPromotion);
                }
            }

        }

        void generateKingMoves(ref Span<Move> moves)
        {
            ulong mask = PrecomputedData.KingAttacks[kingIndex] & ~enemyAttackMask; 
            extract(ref moves, kingIndex, mask);
        }

        void generateKnightMoves(ref Span<Move> moves)
        {
            ulong knights = allBitboards[us][KNIGHT] & ~pinnedPieces;
            while (knights != 0)
            {
                int sq = Helper.popLSB(ref knights);
                ulong mask = PrecomputedData.KnightAttacks[sq] & checkMask;
                extract(ref moves, sq, mask);
            }
        }

        void generateDiagMoves(ref Span<Move> moves)
        {
            ulong pieces = (allBitboards[us][BISHOP] | allBitboards[us][QUEEN]) & ~pinnedPieces;
            while (pieces != 0)
            {
                int sq = Helper.popLSB(ref pieces);
                ulong mask = diagonalAttacks(sq, blocker) & checkMask;
                extract(ref moves, sq, mask);
            }
            pieces = (allBitboards[us][BISHOP] | allBitboards[us][QUEEN]) & pinnedPieces;
            while (pieces != 0)
            {
                int sq = Helper.popLSB(ref pieces);
                ulong mask = diagonalAttacks(sq, blocker) & checkMask & pinMaskArray[sq];
                extract(ref moves, sq, mask);
            }
        }

        void generateOrthoMoves(ref Span<Move> moves)
        {
            ulong bishops = (allBitboards[us][ROOK] | allBitboards[us][QUEEN]) & ~pinnedPieces;
            while (bishops != 0)
            {
                int sq = Helper.popLSB(ref bishops);
                ulong mask = orthogonalAttacks(sq, blocker) & checkMask;
                extract(ref moves, sq, mask);
            }
            bishops = (allBitboards[us][ROOK] | allBitboards[us][QUEEN]) & pinnedPieces;
            while (bishops != 0)
            {
                int sq = Helper.popLSB(ref bishops);
                ulong mask = orthogonalAttacks(sq, blocker) & checkMask & pinMaskArray[sq];
                extract(ref moves, sq, mask);
            }
        }

        ulong orthogonalAttacks (int index, ulong blocker)
        {
            ulong key = blocker & Magic.rookMasks[index];
            key *= Magic.rookMagics[index];
            key >>= rookShift;
            return Magic.rookAttacks[index][key];
        }

        ulong diagonalAttacks (int index, ulong blocker)
        {
            ulong key = blocker & Magic.bishopMasks[index];
            key *= Magic.bishopMagics[index];
            key >>= bishopShift;
            return Magic.bishopAttacks[index][key];
        }

        void extract(ref Span<Move> moves, int from, ulong mask)
        {
            ulong captures = mask & enemy;
            while (captures != 0)
            {
                int to = Helper.popLSB(ref captures);
                moves[moveCount++] = new Move(from, to, moveFlag.capture);
            }
        }

        void initEnemyAttackMask()
        {
            //enemyAttackMask = 0;

            int sq = BitOperations.TrailingZeroCount(allBitboards[them][KING]);
            enemyAttackMask |= PrecomputedData.KingAttacks[sq];

            ulong pieces = allBitboards[them][KNIGHT];
            while (pieces != 0)
            {
                sq = Helper.popLSB(ref pieces);
                enemyAttackMask |= PrecomputedData.KnightAttacks[sq];
            }

            ulong xRayBlocker = blocker ^ (1ul << kingIndex);
            pieces = allBitboards[them][BISHOP] | allBitboards[them][QUEEN];
            while (pieces != 0)
            {
                sq = Helper.popLSB(ref pieces);
                enemyAttackMask |= diagonalAttacks(sq, xRayBlocker);
            }

            pieces = allBitboards[them][ROOK] | allBitboards[them][QUEEN];
            while (pieces != 0)
            {
                sq = Helper.popLSB(ref pieces);
                enemyAttackMask |= orthogonalAttacks(sq, xRayBlocker);
            }

            pieces = allBitboards[them][PAWN];
            if (board.isWhiteToMove)
            {
                enemyAttackMask |= (pieces >> 7) & notAFile;
                enemyAttackMask |= (pieces >> 9) & notHFile;
            }
            else
            {   
                enemyAttackMask |= (pieces << 9) & notAFile;
                enemyAttackMask |= (pieces << 7) & notHFile;
            }
        }


        void initCheckAndPin()
        {
            ulong enemyDiags = allBitboards[them][BISHOP] | allBitboards[them][QUEEN];
            ulong enemyOrthos = allBitboards[them][ROOK] | allBitboards[them][QUEEN];

            ulong kingDiagVision = diagonalAttacks(kingIndex, blocker);
            ulong kingOrthoVision = orthogonalAttacks(kingIndex, blocker);

                                                            // right                               left
            ulong pawnChecks = board.isWhiteToMove ? (1ul << (kingIndex+9)) & notAFile | (1ul << (kingIndex+7)) & notHFile :
                                                     (1ul << (kingIndex-7)) & notAFile | (1ul << (kingIndex-9)) & notHFile;
            pawnChecks &= allBitboards[them][PAWN];
            ulong knightChecks = PrecomputedData.KnightAttacks[kingIndex] & allBitboards[them][KNIGHT];
            ulong diagChecks = kingDiagVision & enemyDiags;
            ulong orthoChecks = kingOrthoVision & enemyOrthos;
            

            // pinnedPieces = 0;
            ulong diagSnipers = diagonalAttacks(kingIndex, 0) & enemyDiags & ~diagChecks;
            ulong orthoSnipers = orthogonalAttacks(kingIndex, 0) & enemyOrthos & ~orthoChecks;
            while (diagSnipers != 0)
            {
                int sq = Helper.popLSB(ref diagSnipers);
                ulong mask = diagonalAttacks(sq, blocker) & kingDiagVision & friendly;
                if (mask != 0)
                {
                    pinnedPieces |= mask;
                    pinMaskArray[BitOperations.TrailingZeroCount(mask)] = PrecomputedData.checkMasks[kingIndex][sq];
                }
            }
            while (orthoSnipers != 0)
            {
                int sq = Helper.popLSB(ref orthoSnipers);
                ulong mask = orthogonalAttacks(sq, blocker) & kingOrthoVision & friendly;
                if (mask != 0)
                {
                    pinnedPieces |= mask;
                    pinMaskArray[BitOperations.TrailingZeroCount(mask)] = PrecomputedData.checkMasks[kingIndex][sq];
                }
            }


            int checkCount = BitOperations.PopCount(pawnChecks | knightChecks | diagChecks | orthoChecks);

            if (checkCount == 0)
            {
                checkMask = ulong.MaxValue;
                board.isInCheck = false;
            }

            else if(checkCount == 1)
            {
                if (knightChecks != 0)
                    checkMask = knightChecks;

                else  
                {
                    int sq = BitOperations.TrailingZeroCount(diagChecks | orthoChecks | pawnChecks);
                    checkMask = PrecomputedData.checkMasks[kingIndex][sq];
                }
                board.isInCheck = true;
            }
                 
            else if (checkCount == 2)
            {
                checkMask = 0;
                board.isInCheck = true;
            }
        }
    } 
}
