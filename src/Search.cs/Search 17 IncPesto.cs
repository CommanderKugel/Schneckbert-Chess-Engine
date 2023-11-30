
using System.Diagnostics;

//
// negamax-PVS-framework with alpha beta pruning
// Repitition, check- & stalemate detection
// Basic Eval Function + basic mobility
// basic q-search
//
// iterative deepening
// return when time is up
// return best move of incomplete iterations
//
// Move ordering in main & Q Search
//  -> TTmove, MVV-LVA, Killer Moves, Move history tables
//
// NEW STUFF:
// Incrementally updated Pesto Evaluation
// 
//
// WDL vs. normal Pesto Eval: + = -
// time: 100
//

public class Search_17_IncPesto : Search
{
    public override string ToString() { return "17_IncPesto"; }
    
    int CHECKMATE = 30_000_000;
    Stopwatch watch = new Stopwatch();
    Board board;

    int startPly;
    int globalBestScore;
    Move globalBestMove;
    long timeControl;


    Transposition[] transpositionTable = new Transposition[0xFFFF];
    Move[] killerMoves1 = new Move[0xFF];
    Move[] killerMoves2 = new Move[0xFF];
    int[,,] moveHistory;

    PestoInc incEval;

    public override Move Think(Board board, long timeControl)
    {
        watch.Reset();
        watch.Start();

        this.board = board;
        this.timeControl = timeControl;

        startPly = board.plyCount;
        moveHistory = new int[2, 64, 64];
        incEval = new PestoInc(board);
        incEval.init(board);
        int score;
        
        globalBestScore = -CHECKMATE;
        globalBestMove = Move.nullMove;

        for (int depth=1; depth<10 && watch.ElapsedMilliseconds<timeControl; depth++)
        {            

            negaMax(-CHECKMATE, CHECKMATE, depth);
            
        }

        watch.Stop();

        return globalBestMove;
    }


    private int negaMax(int alpha, int beta, int depth)
    {
        bool isRoot = board.plyCount == startPly;

        if (board.repititionTable.isRepeatedPosition()) return -10;
        if (depth == 0) return qSearch(alpha, beta, depth);


        Transposition entry = transpositionTable[board.currentGamestate.zobristKey % 0xFFFF];

        if (!isRoot && entry.zobristKey==board.currentGamestate.zobristKey && entry.depth >= depth && (
            entry.flag==2 && entry.score>=beta ||
            entry.flag==1 && entry.score<=alpha ||
            entry.flag==3
            )) return entry.score;


        // check for (stale-) mate before scoring
        Move[] moves = board.generateLegalMoves();
        if (moves.Length == 0) return board.isInCheck ? board.plyCount - CHECKMATE : 0;

        // Move Sorting:
        // initialize relevant variables
        // then loop over all moves to determine the moveScore & save that score
        int[] moveScores = new int[moves.Length];
        int us = board.isWhiteToMove ? 1 : 0;

        Move ttMove = entry.move is not null ? entry.move : Move.nullMove;
        Move killerMove1 = killerMoves1[board.plyCount % 0xFF] is not null ? killerMoves1[board.plyCount % 0xFF] : Move.nullMove;
        Move killerMove2 = killerMoves2[board.plyCount % 0xFF] is not null ? killerMoves2[board.plyCount % 0xFF] : Move.nullMove;
        Move move;

        for (int i=0; i<moveScores.Length; i++)
        {
            move = moves[i];
            moveScores[i] = 
                move==ttMove ? int.MaxValue :
                board.pieceLookup[move.to]!=PieceType.None ? (2_000_000_000 + 100*(int)board.pieceLookup[move.to]-(int)board.pieceLookup[move.from]) :
                (move==killerMove1 || move==killerMove2) ? 2_000_000_000 :
                moveHistory[us, move.from, move.to]--;
        }

        // now the essential alpha-beta-make-unmake-move part of the search
        int score;
        int localBestScore = -CHECKMATE;
        int startAlpha = alpha;
        Move localBestMove = Move.nullMove;

        for (int i=0; i<moves.Length; i++)
        {
            // Cutoff if time is up
            // return checkmate instead of 0 in case of best move having negative value
            if (watch.ElapsedMilliseconds > timeControl && globalBestMove != Move.nullMove)
                return CHECKMATE;

            // incrementally sorting moves with moveScore as keys
            // one-iteration selection sort
            int bestScore = -1;
            int bestIndex = i;
            // parsing all moveScores to find best score & index of best score
            for (int j=i; j<moves.Length; j++)
            {
                if (moveScores[j] > bestScore)
                {
                    bestScore = moveScores[j];
                    bestIndex = j;
                }
            }
            // update next Move to make
            // "swapping" bestIndex and i
            // actually just move objects at "i" to "bestIndex" and never go back to i
            move = moves[bestIndex];
            moves[bestIndex] = moves[i];
            moveScores[bestIndex] = moveScores[i];

            // update psqt every time a move is made/unmade
            incEval.push(board, move);
            board.makeMove(move);

            // PV-Search Part
            // full window search until Alpha can be rised
            // then continue with null window search
            if (i==0)
            {
                score = -negaMax(-beta, -alpha, depth-1);
            }
            else // if not first Move
            {
                // use null window for cheaper cutoffs & prove that PV is best
                // only bounds are needed, if bigger than alpha do full research for exact score
                score = -negaMax(-alpha-1, -alpha, depth-1);

                // if null window search can raise alpha without beta-cutoff
                // -> research with full window for exact score
                if (score > alpha && score < beta) 
                    score = -negaMax(-beta, -alpha, depth-1);
            }

            board.undoMove(move);
            
            // cutoffs
            if (score > localBestScore)
            {
                localBestScore = score;
                localBestMove = move;

                if (score > alpha) 
                {
                    // because beta > alpha
                    // cause cutoff before doing the work to increase alpha
                    if (score >= beta) 
                    {
                        // if quiet move:
                        // update killer Moves and move history
                        // always replace older killer move (overwrite array2 with array1 and overwrite array1)
                        // does it make a difference if instead (capturedPiece == piecetype.None)?
                        if (move.flag == moveFlag.quietMove)
                        {
                            int hash = board.plyCount % 0xFF;
                            killerMoves2[hash] = killerMoves1[hash];
                            killerMoves1[hash] = move;

                            moveHistory[us, move.from, move.to] += depth * depth * 70;
                        }
                        break;
                    }
                    
                    // if not cutoff: update alpha
                    // in case of root Move: update global best move and score
                    alpha = score;
                    if (isRoot)
                    {   
                        globalBestScore = score;
                        globalBestMove = move;
                        Console.WriteLine(depth+": "+move);

                        using (StreamWriter writer = new StreamWriter("C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\eval.txt"))
                            { writer.WriteLine(globalBestScore.ToString()); };
                    }
                }
            }
        }

        // update Transposition Table
        // exact Score (flag = 1), beta cutoff (flag = 2), alpha fail soft (flag 3)
        int ttFlag = localBestScore >= beta ? 2 : localBestScore > startAlpha ? 3 : 1;
        transpositionTable[board.currentGamestate.zobristKey % 0xFFFF] 
            = new Transposition(board.currentGamestate.zobristKey,
                                localBestMove,
                                depth,
                                localBestScore,
                                ttFlag);

        return localBestScore;
    }

    private int qSearch(int alpha, int beta, int depth)
    {
        int standPat = incEval.Eval(board);
        
        // check for (stale)mate before scoring moves
        Move[] moves = board.generateLegalMoves(true);
        if (moves.Length == 0) return board.isInCheck ? board.plyCount - CHECKMATE : 
                                                        standPat;
            

        // Move Sorting:
        // initialize relevant variables
        // then loop over all moves to determine the moveScore & save that score
        int[] moveScores = new int[moves.Length];
        for (int i=0; i<moveScores.Length; i++)
        {   
            // only MVV-LVA
            moveScores[i] = (int) board.pieceLookup[moves[i].to]*10 - (int) board.pieceLookup[moves[i].from];
        }
        
        if (standPat >= beta) return beta;
        if (standPat > alpha) alpha = standPat;
        
        if (depth <= -8) return standPat;
        
        int score;
        for (int i=0; i<moves.Length; i++)
        {
            // incrementally sorting moves with moveScore as keys
            // one-iteration selection sort
            int bestScore=-1;
            int bestIndex=i;
            
            // parsing all moveScores to find best score & index of best score
            for (int j=i ; j<moves.Length; j++)
            {
                if (moveScores[j] > bestScore)
                {
                    bestScore = moveScores[j];
                    bestIndex = j;
                }
            }
            // update next move to play
            // "swapping" bestIndex and i
            // actually just move objects at "i" to "bestIndex" and never go back to i
            Move move = moves[bestIndex];
            moves[bestIndex] = moves[i];
            moveScores[bestIndex] = moveScores[i];


            // classical alpha-beta part
            incEval.push(board, move);
            board.makeMove(move);

            score = -qSearch(-beta, -alpha, depth-1);

            board.undoMove(move);

            // update bounds & do cutoffs
            if (score > alpha) 
            {
                if (score >= beta) return beta;
                alpha = score;
            }
        }

        // return alpha, because player can always stop capturing pieces
        return alpha;
    }

}