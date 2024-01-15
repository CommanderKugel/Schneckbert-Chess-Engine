

public static class Zobrist
{

    //
    // Big thanks to Sebleague from who i stole this code
    //
    // he did a chess coding challenge and provided a lot of code so its public and i think im allowed to steal it :)
    //



    public static ulong sideToMove;
    public static ulong[] castlingRights = new ulong[16];

    public static ulong[][] whitePieces  = new ulong[7][];
    public static ulong[][] blackPieces  = new ulong[7][];
    public static ulong[][][] pieceArray = new ulong[2][][];

    public static ulong[] enPassant = new ulong[8];



    public static void initZobrist (int seed = 541364163)
    {
        Random rng = new Random(seed);

        pieceArray[1] = whitePieces;
        pieceArray[0] = blackPieces;
        for (int piece=0; piece<7; piece++) 
        {
            pieceArray[0][piece] = new ulong[64];
            pieceArray[1][piece] = new ulong[64];
            for (int sq=0; sq<64; sq++) {
                pieceArray[0][piece][sq] = RandomUnsignedLong(rng);
                pieceArray[1][piece][sq] = RandomUnsignedLong(rng);
            }
        }

        for (int i=0; i<castlingRights.Length; i++) {
            castlingRights[i] = RandomUnsignedLong(rng);
        }
        for (int i=0; i<enPassant.Length; i++) {
            enPassant[i] = RandomUnsignedLong(rng);
        }

        sideToMove = RandomUnsignedLong(rng);
    }


    public static ulong calcZobrist (Board board)
    {
        ulong zobristKey = 0;

        try
        {
            if (board.isWhiteToMove) 
                zobristKey ^= sideToMove;

            if (board.currentGamestate.enPassantFile < 8)
                zobristKey ^= enPassant[board.currentGamestate.enPassantFile];
            
            zobristKey ^= castlingRights[board.currentGamestate.castlingRights];

            for (int i=0; i<64; i++)
            {
                if (board.pieceLookup[i] != 0)
                {
                    ulong piece = 1ul << i;
                    if      ((board.allBitboards[0][0] & piece) != 0) 
                        zobristKey ^= pieceArray[0][board.pieceLookup[i] - 1][i];

                    else if ((board.allBitboards[1][0] & piece) != 0) 
                        zobristKey ^= pieceArray[1][board.pieceLookup[i] - 1][i];
                }
            }
        }
        catch
        {
            Console.WriteLine("ERROR INITIALIZING ZOBRIST KEYS!");
        }

        return zobristKey;
    }


    private static ulong RandomUnsignedLong (Random rng)
    {
        byte[] buffer = new byte[8];
        rng.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }
}
