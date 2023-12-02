using System.Numerics;

public static class PestoEval
{
    private static readonly int[] gamephaseInc = { 0, 1, 1, 2, 4, 0 };

    public static int Eval (Board board)
    {
        int eg = 0;
        int mg = 0;
        int gamephase = 0;

        ulong bb;
        int pieceIndex;
        int tablePieceIndex;

        for (int color=0; color<2; color++)
        {
            // convention: 0=none, 1=pawn, 2=knight, 3=bishop, 4=rook, 5=queen, 6=king
            for (int piece=1; piece<=6; piece++)
            {
                tablePieceIndex = piece-1;
                bb = board.allBitboards[color][piece];
                while (bb != 0)
                {
                    pieceIndex = color==1 ? Helper.popLSB(ref bb) : Helper.popLSB(ref bb) ^ 56;
                    mg += PestoTables.mgTables[tablePieceIndex][pieceIndex];
                    eg += PestoTables.egTables[tablePieceIndex][pieceIndex];
                    gamephase += gamephaseInc[tablePieceIndex];
                }
            }
            mg=-mg;
            eg=-eg;
        }
        gamephase = Math.Max(gamephase, 24);
        return (mg * gamephase + eg * (24 - gamephase)) / (board.isWhiteToMove ? -24 : 24);
    }




    public static int EvalMobility (Board board)
    {
        int eg = 0;
        int mg = 0;
        int gamephase = 0;

        ulong bb;
        int pieceIndex;
        int tableIndex;

        ulong mask;
        int mobility;
        ulong pawnBlocker = board.allBitboards[0][1] | board.allBitboards[1][1];

        for (int color=0; color<2; color++)
        {
            mobility = 0;

            for (int piece=1; piece<=6; piece++)
            {
                tableIndex = piece-1;
                bb = board.allBitboards[color][piece];
                while (bb != 0)
                {
                    pieceIndex = color==1 ? Helper.popLSB(ref bb) : Helper.popLSB(ref bb) ^ 56;
                    mg += PestoTables.mgTables[tableIndex][pieceIndex];
                    eg += PestoTables.egTables[tableIndex][pieceIndex];
                    gamephase += gamephaseInc[tableIndex];

                    switch (piece)
                    {
                        case 2: mask = PrecomputedData.KnightAttacks[pieceIndex] & ~pawnBlocker;
                                mobility += BitOperations.PopCount(mask);
                                
                                break;
                        case 3: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker);
                                mobility += BitOperations.PopCount(mask);
                                break;

                        case 4: mask = MoveGenLegal.orthogonalAttacks(piece, pawnBlocker);
                                mobility += BitOperations.PopCount(mask);
                                break;

                        case 5: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker) |
                                       MoveGenLegal.orthogonalAttacks(piece, pawnBlocker);
                                mobility += BitOperations.PopCount(mask);
                                break;

                        case 6: mask = PrecomputedData.KingAttacks[pieceIndex];
                                mobility += BitOperations.PopCount(mask);
                                break;      
                    }
                }
            }
            mg += mobility * 2;
            eg += mobility;

            mg=-mg;
            eg=-eg;
        }
        gamephase = Math.Max(gamephase, 24);
        return (mg * gamephase + eg * (24 - gamephase)) / (board.isWhiteToMove ? -24 : 24);
    }

    

    public static int kingEval (Board board)
    {
        int eg = 0;
        int mg = 0;
        int gamephase = 0;

        ulong bb;
        int pieceIndex;
        int tableIndex;

        ulong pawnBlocker = board.allBitboards[0][1] | board.allBitboards[1][1];
        ulong mask;

        ulong enemyKingZone;
        int valueOfAttack;
        int attackingPieceCount;
        int attackScore;

        for (int piece=1; piece<=6; piece++)
        {
            for (int color=0; color<2; color++)
            {
                valueOfAttack = 0;
                attackingPieceCount = 0;
                enemyKingZone = board.allBitboards[1 - color][(int) PieceType.King];

                // piece in [1, 6]
                tableIndex = piece-1;
                bb = board.allBitboards[color][piece];
                while (bb != 0)
                {
                    pieceIndex = color==1 ? Helper.popLSB(ref bb) : Helper.popLSB(ref bb) ^ 56;
                    
                    switch (piece)
                    {
                        case 2: mask = PrecomputedData.KnightAttacks[pieceIndex] & enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 2;
                                }
                                break;

                        case 3: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker) & enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 2;
                                }
                                break;

                        case 4: mask = MoveGenLegal.orthogonalAttacks(piece, pawnBlocker) & enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 4;
                                }
                                break;

                        case 5: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker) |
                                       MoveGenLegal.orthogonalAttacks(piece, pawnBlocker);
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 8;
                                }
                                break;                                
                    }
                    
                    mg += PestoTables.mgTables[tableIndex][pieceIndex];
                    eg += PestoTables.egTables[tableIndex][pieceIndex];
                    gamephase += gamephaseInc[tableIndex];
                }

                if (attackingPieceCount > 7) attackingPieceCount = 7;
                attackScore = valueOfAttack * attackWeight[attackingPieceCount] / 100;

                mg += attackScore;
                eg += attackScore / 2;

                mg = -mg;
                eg = -eg;
            }
        }

        // if many pieces + early promotion
        gamephase = Math.Max(gamephase, 24);

        return (mg * gamephase + eg * (24 - gamephase)) / (board.isWhiteToMove ? -24 : 24);
    }
    private static readonly int[] attackWeight = { 0, 0, 50, 75, 88, 94, 97, 99 };

   



    public static int FullEval (Board board)
    {
        int eg = 0;
        int mg = 0;
        int gamephase = 0;

        ulong bb;
        int pieceIndex;
        int tableIndex;

        ulong pawnBlocker = board.allBitboards[0][1] | board.allBitboards[1][1];
        ulong mask;

        ulong enemyKingZone;
        int valueOfAttack;
        int attackingPieceCount;
        int attackScore;

        int mobility;

        for (int piece=1; piece<=6; piece++)
        {
            for (int color=0; color<2; color++)
            {
                valueOfAttack = 0;
                attackingPieceCount = 0;
                enemyKingZone = board.allBitboards[1 - color][(int) PieceType.King];

                mobility = 0;

                // piece in [1, 6]
                tableIndex = piece-1;
                bb = board.allBitboards[color][piece];
                while (bb != 0)
                {
                    pieceIndex = color==1 ? Helper.popLSB(ref bb) : Helper.popLSB(ref bb) ^ 56;
                    
                    switch (piece)
                    {   
                        // knights
                        case 2: mask = PrecomputedData.KnightAttacks[pieceIndex];
                                mobility += BitOperations.PopCount(mask & ~pawnBlocker);
                                mask &= enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 2;
                                }
                                break;

                        // bishops
                        case 3: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker);
                                mobility += BitOperations.PopCount(mask);
                                mask &= enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 2;
                                }
                                break;

                        // rooks
                        case 4: mask = MoveGenLegal.orthogonalAttacks(piece, pawnBlocker);
                                mobility += BitOperations.PopCount(mask) / 2;
                                mask &= enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 4;
                                }
                                break;

                        // queen
                        case 5: mask = MoveGenLegal.diagonalAttacks(piece, pawnBlocker) |
                                       MoveGenLegal.orthogonalAttacks(piece, pawnBlocker);
                                mobility += BitOperations.PopCount(mask) / 4;
                                mask &= enemyKingZone;
                                if (mask != 0)                               
                                {
                                    attackingPieceCount++;
                                    valueOfAttack += BitOperations.PopCount(mask) * 8;
                                }
                                break;                                
                    }
                    
                    mg += PestoTables.mgTables[tableIndex][pieceIndex];
                    eg += PestoTables.egTables[tableIndex][pieceIndex];
                    gamephase += gamephaseInc[tableIndex];
                }

                if (attackingPieceCount > 7) attackingPieceCount = 7;
                attackScore = valueOfAttack * attackWeight[attackingPieceCount] / 100;

                mg += attackScore + mobility;
                eg += attackScore / 2 + mobility;

                mg = -mg;
                eg = -eg;
            }
        }

        // if many pieces + early promotion
        gamephase = Math.Max(gamephase, 24);

        return (mg * gamephase + eg * (24 - gamephase)) / (board.isWhiteToMove ? -24 : 24);
    }
    
}
