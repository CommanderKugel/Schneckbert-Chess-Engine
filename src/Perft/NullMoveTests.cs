
public static class NullMoveTests
{

    public static void depth_null_tests (Board board, bool doLegal)
    {
        Console.WriteLine("generating enemy attack Masks & King Moves");
        nt1.test(board, doLegal);
        nt2.test(board, doLegal);
        nt3.test(board, doLegal);
        nt4.test(board, doLegal);
        nt5.test(board, doLegal);
        nt6.test(board, doLegal);

        Console.WriteLine("enemy attack Mask & King Moves with X-Rays");
        nt7.test(board, doLegal);
        nt8.test(board, doLegal);
        nt9.test(board, doLegal);
        nt10.test(board, doLegal);

        Console.WriteLine("basic quiet Piece Movements");
        nt11.test(board, doLegal);
        nt12.test(board, doLegal);
        nt13.test(board, doLegal);
        nt14.test(board, doLegal);
        nt15.test(board, doLegal);

        Console.WriteLine("block all slider rays");
        nt16.test(board, doLegal);
        nt17.test(board, doLegal);
        nt18.test(board, doLegal);

        Console.WriteLine("Rook Checks");
        nt19.test(board, doLegal);
        nt20.test(board, doLegal);
        nt21.test(board, doLegal);
        nt22.test(board, doLegal);
        nt23.test(board, doLegal);

        Console.WriteLine("Bishop Checks");
        nt24.test(board, doLegal);
        nt25.test(board, doLegal);
        nt26.test(board, doLegal);
        nt27.test(board, doLegal);
        nt28.test(board, doLegal);

        Console.WriteLine("double check");
        nt29.test(board, doLegal);

        Console.WriteLine("basic pawn pushes");
        nt30.test(board, doLegal);
        nt31.test(board, doLegal);
        nt32.test(board, doLegal);
        nt33.test(board, doLegal);
        nt34.test(board, doLegal);

        Console.WriteLine("basic pawn captures");
        nt35.test(board, doLegal, true);
        nt36.test(board, doLegal, true);

        Console.WriteLine("pawn check evasions");
        nt37.test(board, doLegal);
        nt38.test(board, doLegal, true);
        nt39.test(board, doLegal, true);
        nt40.test(board, doLegal);

        Console.WriteLine("basic en Passant");
        nt41.test(board, doLegal);
        nt42.test(board, doLegal);
        nt43.test(board, doLegal);

        Console.WriteLine("basic promotion");
        nt44.test(board, doLegal, true);
        nt45.test(board, doLegal, true);
        nt46.test(board, doLegal, true);

        Console.WriteLine("promotion check evasions");
        nt47.test(board, doLegal);
        nt48.test(board, doLegal);
        nt49.test(board, doLegal);

        Console.WriteLine("piece pins");
        nt50.test(board, doLegal);
        nt51.test(board, doLegal);
        nt52.test(board, doLegal);
        nt53.test(board, doLegal);
        nt54.test(board, doLegal);
        nt55.test(board, doLegal);
        nt56.test(board, doLegal);
        nt57.test(board, doLegal);

        Console.WriteLine("pawn pins & pushes");
        nt58.test(board, doLegal);
        nt59.test(board, doLegal);
        nt60.test(board, doLegal);
        nt61.test(board, doLegal);
        nt62.test(board, doLegal);
        nt63.test(board, doLegal);
        nt64.test(board, doLegal);
        nt65.test(board, doLegal);
        nt66.test(board, doLegal);
        nt67.test(board, doLegal);
        nt68.test(board, doLegal);
        nt69.test(board, doLegal);

        Console.WriteLine("pawn pins & captures");
        nt70.test(board, doLegal, true);
        nt71.test(board, doLegal, true);
        nt72.test(board, doLegal);
        nt73.test(board, doLegal);
        nt74.test(board, doLegal);
        nt75.test(board, doLegal);

        nt76.test(board, doLegal, true);
        nt77.test(board, doLegal, true);
        nt78.test(board, doLegal, true);
        nt79.test(board, doLegal, true);

        Console.WriteLine("pinned en passant");
        nt80.test(board, doLegal, true);
        nt81.test(board, doLegal, true);
        nt82.test(board, doLegal, true);
        nt83.test(board, doLegal, true);
        nt84.test(board, doLegal, true);
        nt85.test(board, doLegal, true);
        nt86.test(board, doLegal, true);

        Console.WriteLine("check & en passant");
        nt87.test(board, doLegal);
        nt88.test(board, doLegal);
        nt89.test(board, doLegal);
        nt90.test(board, doLegal);

        Console.WriteLine("castling");
        nt91.test(board, doLegal);
        nt92.test(board, doLegal);
        nt93.test(board, doLegal);
        nt94.test(board, doLegal);
        nt95.test(board, doLegal);
        nt96.test(board, doLegal);
        nt97.test(board, doLegal);
        nt98.test(board, doLegal);
        nt99.test(board, doLegal);
        nt100.test(board, doLegal);
        nt101.test(board, doLegal);
        nt102.test(board, doLegal);
        nt103.test(board, doLegal);


    }






    private static depthNullTest nt1 = new depthNullTest("8/8/8/2K1k3/8/8/8/8 w - - 0 1",     5, "attackMask - King  ");
    private static depthNullTest nt2 = new depthNullTest("8/1p6/8/1K3k2/8/8/8/8 w - - 0 1",   6, "attackMask - Pawn  ");
    private static depthNullTest nt3 = new depthNullTest("8/8/8/1K3k2/8/1n6/8/8 w - - 0 1",   6, "attackMask - Knight");
    private static depthNullTest nt4 = new depthNullTest("8/8/8/1K3k2/3b4/8/8/8 w - - 0 1",   6, "attackMask - Bishop");
    private static depthNullTest nt5 = new depthNullTest("2r5/8/4r3/1K3k2/8/8/8/8 w - - 0 1", 3, "attackMask - Rook  ");
    private static depthNullTest nt6 = new depthNullTest("8/8/8/1K3k2/8/2q5/8/8 w - - 0 1",   3, "attackMask - Queen ");

    private static depthNullTest nt7  = new depthNullTest("8/8/8/1K2r1k1/8/8/8/8 w - - 0 1",   6, "attackMask - xRay rook 1  ");
    private static depthNullTest nt8  = new depthNullTest("8/8/8/1K4k1/8/8/1r6/8 w - - 0 1",   6, "attackMask - xRay rook 2  ");
    private static depthNullTest nt9  = new depthNullTest("8/3b4/8/1K4k1/8/8/8/8 w - - 0 1",   6, "attackMask - xRay bishop 1");
    private static depthNullTest nt10 = new depthNullTest("8/8/8/1K4k1/8/3b4/8/8 w - - 0 1",   6, "attackMask - xRay bishop 2");

    private static depthNullTest nt11 = new depthNullTest("8/1K4k1/8/8/8/8/8/8 w - - 0 1",    8, "Piece Movement - King  ");
    private static depthNullTest nt12 = new depthNullTest("8/1K4k1/8/8/8/2N5/8/8 w - - 0 1", 16, "Piece Movement - Knight");
    private static depthNullTest nt13 = new depthNullTest("8/1K6/6k1/8/8/2B5/8/8 w - - 0 1", 19, "Piece Movement - Bishop");
    private static depthNullTest nt14 = new depthNullTest("8/1K4k1/8/8/8/3R4/8/8 w - - 0 1", 22, "Piece Movement - Rook  ");
    private static depthNullTest nt15 = new depthNullTest("8/1K4k1/8/8/8/3Q4/8/8 w - - 0 1", 33, "Piece Movement - Queen ");

    private static depthNullTest nt16 = new depthNullTest("8/1K4k1/8/2p1p3/3B4/2p1p3/8/8 w - - 0 1",   12, "blocked Slider - Bishop");
    private static depthNullTest nt17 = new depthNullTest("8/1K4k1/8/3p4/2pRp3/3p4/8/8 w - - 0 1",     12, "blocked Slider - Rook  ");
    private static depthNullTest nt18 = new depthNullTest("8/1K4k1/8/2ppp3/2pQp3/2ppp3/8/8 w - - 0 1", 16, "blocked Slider - Queen ");
    
    private static depthNullTest nt19 = new depthNullTest("8/8/1K3rk1/8/4N3/8/8/8 w - - 0 1", 8, "Rook Check - Knight");
    private static depthNullTest nt20 = new depthNullTest("8/8/1K3rk1/4B3/8/8/8/8 w - - 0 1", 8, "Rook Check - Bishop");
    private static depthNullTest nt21 = new depthNullTest("8/8/1K3rk1/8/8/3R4/8/8 w - - 0 1", 7, "Rook Check - Rook 1");
    private static depthNullTest nt22 = new depthNullTest("8/8/1K3rk1/8/8/5R2/8/8 w - - 0 1", 7, "Rook Check - Rook 2");
    private static depthNullTest nt23 = new depthNullTest("8/8/1K3rk1/4Q3/8/8/8/8 w - - 0 1", 9, "Rook Check - Queen ");

    private static depthNullTest nt24 = new depthNullTest("8/6k1/5b2/8/6N1/2K5/8/8 w - - 0 1", 8, "Bishop Check - Knight  ");
    private static depthNullTest nt25 = new depthNullTest("8/6k1/5b2/8/5B2/2K5/8/8 w - - 0 1", 7, "Bishop Check - Bishop 1");
    private static depthNullTest nt26 = new depthNullTest("8/6k1/5b2/6B1/8/2K5/8/8 w - - 0 1", 7, "Bishop Check - Bishop 2");
    private static depthNullTest nt27 = new depthNullTest("8/6k1/3R1b2/8/8/2K5/8/8 w - - 0 1", 8, "Bishop Check - Rook    ");
    private static depthNullTest nt28 = new depthNullTest("8/6k1/3Q1b2/8/8/2K5/8/8 w - - 0 1", 9, "Bishop Check - Queen   ");

    private static depthNullTest nt29 = new depthNullTest("8/8/1K4k1/8/3b4/1r2Q3/8/8 w - - 0 1", 4, "double check ");

    private static depthNullTest nt30 = new depthNullTest("8/1K4k1/8/8/8/3P4/8/8 w - - 0 1",    9, "single pawn push          ");
    private static depthNullTest nt31 = new depthNullTest("8/1K4k1/8/8/3p4/3P4/8/8 w - - 0 1",  8, "single pawn blocked       ");
    private static depthNullTest nt32 = new depthNullTest("8/1K4k1/8/8/8/8/3P4/8 w - - 0 1",   10, "double pawn push          ");
    private static depthNullTest nt33 = new depthNullTest("8/1K4k1/8/8/3p4/8/3P4/8 w - - 0 1",  9, "double pawn push blocked 1");
    private static depthNullTest nt34 = new depthNullTest("8/1K4k1/8/8/8/3p4/3P4/8 w - - 0 1",  8, "double pawn push blocked 2");

    private static depthNullTest nt35 = new depthNullTest("8/1K4k1/8/4p3/3P4/8/8/8 w - - 0 1",  1, "pawn capture right");
    private static depthNullTest nt36 = new depthNullTest("8/1K4k1/8/2p5/3P4/8/8/8 w - - 0 1",  1, "pawn capture left ");
    
    private static depthNullTest nt37 = new depthNullTest("8/6k1/8/8/1K3r2/3P4/8/8 w - - 0 1",  7, "evasion single push  ");
    private static depthNullTest nt38 = new depthNullTest("8/6k1/8/8/1K3r2/8/3P4/8 w - - 0 1",  7, "evasion double push  ");
    private static depthNullTest nt39 = new depthNullTest("8/6k1/8/8/1K3r2/4P3/8/8 w - - 0 1",  8, "evasion capture right");
    private static depthNullTest nt40 = new depthNullTest("8/6k1/8/8/1K1r4/4P3/8/8 w - - 0 1",  7, "evasion capture left ");

    private static depthNullTest nt41 = new depthNullTest("8/1K4k1/8/3Pp3/8/8/8/8 w - e6 0 1",  10, "en passant right");
    private static depthNullTest nt42 = new depthNullTest("8/1K4k1/8/4pP2/8/8/8/8 w - e6 0 1",  10, "en passant left ");
    private static depthNullTest nt43 = new depthNullTest("8/1K4k1/8/3PpP2/8/8/8/8 w - e6 0 1", 12, "en passant both ");

    private static depthNullTest nt44 = new depthNullTest("8/3P4/8/8/8/8/1K4k1/8 w - - 0 1",    4, "promotion push         ");
    private static depthNullTest nt45 = new depthNullTest("3nn3/3P4/8/8/8/8/1K4k1/8 w - - 0 1", 4, "promotion right capture");
    private static depthNullTest nt46 = new depthNullTest("2nn4/3P4/8/8/8/8/1K4k1/8 w - - 0 1", 4, "promotion left capture ");

    private static depthNullTest nt47 = new depthNullTest("2n5/3P4/1K6/8/8/8/6k1/8 w - - 0 1", 11, "knight check promo capture");
    private static depthNullTest nt48 = new depthNullTest("2b5/3P4/K7/8/8/8/6k1/8 w - - 0 1",   8, "bishop check promo capture");
    private static depthNullTest nt49 = new depthNullTest("2n5/3P4/1K6/8/8/8/6k1/8 w - - 0 1", 11, "rook check promo capture  ");

    private static depthNullTest nt50 = new depthNullTest("8/8/1KN2rk1/8/8/8/8/8 w - - 0 1",  7, "rook pin + knight");
    private static depthNullTest nt51 = new depthNullTest("8/8/1KB2rk1/8/8/8/8/8 w - - 0 1",  7, "rook pin + bishop");
    private static depthNullTest nt52 = new depthNullTest("8/8/1KR2rk1/8/8/8/8/8 w - - 0 1", 10, "rook pin + rook  ");
    private static depthNullTest nt53 = new depthNullTest("8/8/1KQ2rk1/8/8/8/8/8 w - - 0 1", 10, "rook pin + queen ");

    private static depthNullTest nt54 = new depthNullTest("8/6k1/5b2/8/8/2N5/1K6/8 w - - 0 1",  7, "bishp pin + knight");
    private static depthNullTest nt55 = new depthNullTest("8/6k1/5b2/8/8/2B5/1K6/8 w - - 0 1", 10, "bishp pin + bishop");
    private static depthNullTest nt56 = new depthNullTest("8/6k1/5b2/8/8/2R5/1K6/8 w - - 0 1",  7, "bishp pin + rook  ");
    private static depthNullTest nt57 = new depthNullTest("8/6k1/5b2/8/8/2Q5/1K6/8 w - - 0 1", 10, "bishp pin + queen ");

    private static depthNullTest nt58 = new depthNullTest("8/8/8/8/8/KP4rk/8/8 w - - 0 1", 4, "rook pin hor + pawn push  ");
    private static depthNullTest nt59 = new depthNullTest("8/8/8/8/8/8/KP4rk/8 w - - 0 1", 4, "rook pin hor + double push");
    private static depthNullTest nt60 = new depthNullTest("8/KP4rk/8/8/8/8/8/8 w - - 0 1", 4, "rook pin hor + promotion  ");
    private static depthNullTest nt61 = new depthNullTest("8/8/1K6/2P5/8/4b3/5k2/8 w - - 0 1", 7, "bishop pin up + pawn push  ");
    private static depthNullTest nt62 = new depthNullTest("8/8/8/8/8/1K6/2P2k2/3b4 w - - 0 1", 7, "bishop pin up + double push");
    private static depthNullTest nt63 = new depthNullTest("K7/1P6/8/8/8/8/6b1/7k w - - 0 1",   2, "bishop pin up + promotion  ");

    private static depthNullTest nt64 = new depthNullTest("2k5/2r5/8/8/2P5/2K5/8/8 w - - 0 1",  8, "rook pin vert 1 + pawn push  ");
    private static depthNullTest nt65 = new depthNullTest("2k5/2r5/8/8/8/8/2P5/2K5 w - - 0 1",  6, "rook pin vert 1 + double push");
    private static depthNullTest nt66 = new depthNullTest("8/2K5/8/2P5/8/8/2r5/2k5 w - - 0 1",  9, "rook pin vert 2 + pawn push  ");
    private static depthNullTest nt67 = new depthNullTest("8/2K5/8/8/8/8/2P5/2r1k3 w - - 0 1", 10, "rook pin vert 2 + double push");
    private static depthNullTest nt68 = new depthNullTest("5k2/4b3/8/2P5/1K6/8/8/8 w - - 0 1", 7, "bishop pin down + pawn push  ");
    private static depthNullTest nt69 = new depthNullTest("8/8/6k1/5b2/8/8/2P5/1K6 w - - 0 1", 4, "bishop pin down + double push");

    private static depthNullTest nt70 = new depthNullTest("8/8/8/4p3/K2P2rk/8/8/8 w - - 0 1",    0, "rook pin hor + pawn right   ");
    private static depthNullTest nt71 = new depthNullTest("8/8/8/3p4/K3P1rk/8/8/8 w - - 0 1",    0, "rook pin hor + pawn left    ");
    private static depthNullTest nt72 = new depthNullTest("3k4/3r4/8/4p3/3P4/8/3K4/8 w - - 0 1", 9, "rook pin vert 1 + pawn right");
    private static depthNullTest nt73 = new depthNullTest("3k4/3r4/8/2p5/3P4/8/3K4/8 w - - 0 1", 9, "rook pin vert 1 + pawn left ");
    private static depthNullTest nt74 = new depthNullTest("8/3K4/8/4p3/3P4/8/3r4/3k4 w - - 0 1", 9, "rook pin vert 2 + pawn right");
    private static depthNullTest nt75 = new depthNullTest("8/3K4/8/2p5/3P4/8/3r4/3k4 w - - 0 1", 9, "rook pin vert 2 + pawn left ");

    private static depthNullTest nt76 = new depthNullTest("7k/6b1/8/4p3/3P4/8/1K6/8 w - - 0 1", 1, "bishop pin right + pawn right");
    private static depthNullTest nt77 = new depthNullTest("7k/6b1/8/2p5/3P4/8/1K6/8 w - - 0 1", 0, "bishop pin right + pawn left ");
    private static depthNullTest nt78 = new depthNullTest("7k/8/1b6/4p3/3P4/8/5K2/8 w - - 0 1", 0, "bishop pin left  + pawn right");
    private static depthNullTest nt79 = new depthNullTest("7k/8/1b6/2p5/3P4/8/5K2/8 w - - 0 1", 1, "bishop pin left  + pawn left ");

    private static depthNullTest nt80 = new depthNullTest("7k/8/8/1K1Pp1r1/8/8/8/8 w - e6 0 1", 0, "rook pin hor + EP   ");
    private static depthNullTest nt81 = new depthNullTest("7k/3K4/8/3Pp3/8/8/3r4/8 w - e6 0 1", 0, "rook pin vert 1 + EP");
    private static depthNullTest nt82 = new depthNullTest("7k/3r4/8/3Pp3/8/8/3K4/8 w - e6 0 1", 0, "rook pin vert 2 + EP");
    private static depthNullTest nt83 = new depthNullTest("7k/5K2/8/3Pp3/8/1b6/8/8 w - e6 0 1", 1, "bishop pin up 1 + EP  ");
    private static depthNullTest nt84 = new depthNullTest("7k/1K6/8/3Pp3/8/5b2/8/8 w - e6 0 1", 0, "bishop pin up 2 + EP  ");
    private static depthNullTest nt85 = new depthNullTest("7k/5b2/8/3Pp3/8/1K6/8/8 w - e6 0 1", 1, "bishop pin down 1 + EP");
    private static depthNullTest nt86 = new depthNullTest("7k/1b6/8/3Pp3/8/5K2/8/8 w - e6 0 1", 0, "bishop pin down 2 + EP");

    private static depthNullTest nt87 = new depthNullTest("2b4k/8/8/3Pp3/6K1/8/8/8 w - e6 0 1", 6, "bishop check in  + EP");
    private static depthNullTest nt88 = new depthNullTest("3b3k/8/8/3Pp1K1/8/8/8/8 w - e6 0 1", 5, "bishop check out + EP");
    private static depthNullTest nt89 = new depthNullTest("7k/8/1K4r1/3Pp3/8/8/8/8 w - e6 0 1", 8, "rook   check in  + EP");
    private static depthNullTest nt90 = new depthNullTest("7k/8/8/3Pp3/8/1K4r1/8/8 w - e6 0 1", 6, "rook   check out + EP");

    private static depthNullTest nt91 = new depthNullTest("4k3/8/8/8/8/8/8/4K2R w K - 0 1",   15, "castle kingside ");
    private static depthNullTest nt92 = new depthNullTest("4k3/8/8/8/8/8/8/R3K3 w Q - 0 1",   16, "castle queenside");
    private static depthNullTest nt93 = new depthNullTest("4k3/8/8/8/8/8/8/R3K2R w KQ - 0 1", 26, "castle both ways");

    private static depthNullTest nt94 = new depthNullTest("4k3/8/4r3/8/8/8/8/4K2R w K - 0 1",  4, "rookcheck   - castle kingside ");
    private static depthNullTest nt95 = new depthNullTest("4k3/8/4r3/8/8/8/8/R3K3 w Q - 0 1",  4, "rookcheck   - castle queenside");
    private static depthNullTest nt96 = new depthNullTest("4k3/8/8/8/8/6b1/8/4K2R w K - 0 1",  4, "bishopcheck - castle kingside ");
    private static depthNullTest nt97 = new depthNullTest("4k3/8/8/8/8/6b1/8/R3K3 w Q - 0 1",  4, "bishopcheck - castle queenside");
    private static depthNullTest nt98 = new depthNullTest("4k3/8/8/8/8/3n4/8/4K2R w K - 0 1",  4, "knightcheck - castle kingside ");
    private static depthNullTest nt99 = new depthNullTest("4k3/8/8/8/8/5n2/8/R3K3 w Q - 0 1",  4, "knightcheck - castle queenside");

    private static depthNullTest nt100 = new depthNullTest("4k3/8/8/8/8/8/8/4K1NR w K - 0 1", 15, "blocker - castle kingside ");
    private static depthNullTest nt101 = new depthNullTest("4k3/8/8/8/8/8/8/RN2K3 w Q - 0 1", 15, "blocker - castle queenside");
    private static depthNullTest nt102 = new depthNullTest("4k3/8/5r2/8/8/8/8/4K2R w K - 0 1", 12, "attackMask - castle kingside ");
    private static depthNullTest nt103 = new depthNullTest("4k3/8/3r4/8/8/8/8/R3K3 w Q - 0 1", 13, "attackMask - castle queenside");





    private class depthNullTest
    {
        string fen;
        int nodeCount;
        string description;
        
        public depthNullTest (string fen, int nodeCount, string description)
        {
            this.fen = fen;
            this.nodeCount = nodeCount;
            this.description = description;
        }
        public void test (Board board, bool doLegal, bool onlyCaptures=false)
        {
            NotationHelper.initFen(board, fen);
            int count = doLegal ? board.generateLegalMoves(onlyCaptures).Length : board.generateMovesPseudo(onlyCaptures).Length;
            string res = nodeCount==count ? " ✓" : " X - "+count+"/"+nodeCount;
            Console.WriteLine(" - "+description+": "+res);
        }
    }

}

