
The Unbalanced Human Openings 2022 (UHO 2022)


The idea of the UHO (Unbalanced Human Openings) openings is quite simple. The chess-community is a real conservative group. 
And I recognized, that many of them dont like openings, which were constructed (my Drawkiller openings for example) or changed 
manually (like my NBSC openings, where black is not allowed to castle short). They say, that is no "real chess". I do not agree 
with that personally, but I decided to try building openings-sets, which are working like NBSC (white has a clear advantage) 
and can (not must!) be (because white has a clear advantage at the beginning of the game) rescored with my Advanced Armageddon 
Scoring system for extremly high Elo-spreadings, which makes the rakings in a test/tournament much more reliable with a lower 
number of played games, but are complete unmodified and 100% human.

Whats new in UHO 2022:
The UHO 2022 opening sets, are filtered out of the Megabase 2022 (by ChessBase). And the only filter, I am using, 
is the eval of KomodoDragon 2.6 in the endposition of each opening-line. (UHO V3 used Megabase 2020 and KomodoDragon 1.0
for evaluating). And the UHO 2022 files are around +10% bigger than the UHO V3 files, because the Megabase 2022 contains
more games than the Megabase 2020, of course.

What did I do exactly? 

- sorted the games of the Megabase 2022 by Elo: both players 2700+ Elo, both players 2600+ Elo, both players 2500+ Elo, 
both players 2400+ Elo, both players 2300+ Elo.
- cutted all games after 6/8 moves (12/16 plies).
- removed all Chess960 games
- removed all lines with double endpositons, so each endposition is unique. And because the games are sorted by Elo of 
players, each opening line in the UHO-sets is played by the strongest players (if weaker players played the same opening
line, this game was deleted by pgn-extract, because pgn-extract always keeps the first appearance of a line in a pgn-file)
- removed all lines, were not both queens are still on board in the endposition.
- removed all lines, starting with silly pawn-moves in move 1 and 2 (a4, b4, g4, h4 (same for black (...a5, ...b5, etc.)

After doing these work, there were 111789 6-moves deep opening-lines remaining and 316318 8-moves deep opening-lines
remaining (all with unique endpositions).
Finally, these opening-lines were evaluated (endpositions) with KomodoDragon 2.6 (8.5 seconds per endposition on a 12 core 
AMD Ryzen CPU) and kept all endpositions in an eval interval of [-2.99;+2.99] (this is the UHO 2022 RawData (can be found 
in the download, too)). That took around 40 days of computing...
The UHO 2022 RawData can be very useful for other purposes, because it contains all opening-lines from very good for black 
to very good for white. Feel free to filter these openings as you like...The eval (and search depth) is stored in the
Annotator-Tag of each line:
[Annotator "depth=29 eval=+114"]
You can use ChessBase (for example) for searching. Examples of searching:
"eval=-05" finds all lines in an eval-interval of [-0.50;-0.59]
"eval=+00" finds all lines in an eval-interval of [+0.00;+0.09]
"eval=-1" finds all lines in an eval-interval of [-1.00;-1.99]


So the Unbalanced Human Openings contain:

- 100% moves played by humans, only. Both players had at least 2300 Elo.
- opening lines played by the stronges players are at the beginning of the pgn/epd-files.
- no manually constructed openings (like my Drawkiller openings)
- no manually added moves to make castling impossible (like my NBSC openings)
- no selection of piece-patterns

There are 8 folders with different UHO 2022 openings-sets, with increasing advantage for white:
*** IMPORTANT: *** KomodoDragon 2.6 shows evals, which are around +0.20 higher, than KomodoDragon 1.0 does, 
which I used for evaluating the old UHO V3 openings. So, the Eval-intervals in the new UHO 2022 are +0.20 higher:
UHO 2022 starts with Eval [+1.10;+1.19] instead of Eval [+0.90;+0.99] (UHO V3). But as you can see below in the
testing results, using UHO 2022 with Eval [+1.10;+1.19] gives nearly the same Elo-spreading and draw-rates, UHO V3
with Eval [+0.90;+0.99] gives...

Eval [+1.10;+1.19] 6mvs: 3317 lines  8mvs: 8940 lines  8mvs_big: 27074 lines
Eval [+1.20;+1.29] 6mvs: 2608 lines  8mvs: 7296 lines  8mvs_big: 22336 lines
Eval [+1.30;+1.39] 6mvs: 2214 lines  8mvs: 6100 lines  8mvs_big: 18252 lines
Eval [+1.40;+1.49] 6mvs: 1777 lines  8mvs: 4856 lines  8mvs_big: 15048 lines
Eval [+1.50;+1.59] 6mvs: 1440 lines  8mvs: 4092 lines  8mvs_big: 12130 lines
Eval [+1.60;+1.69] 6mvs: 1098 lines  8mvs: 3182 lines  8mvs_big: 9831 lines
Eval [+1.70;+1.79] 6mvs: 823  lines  8mvs: 2557 lines  8mvs_big: 7787 lines
Eval [+1.80;+1.89] 6mvs: 614  lines  8mvs: 2048 lines  8mvs_big: 6192 lines

Each folder contains 3 UHO openings-sets: 6 moves, 8 moves and a bigger 8 moves file with a bigger eval interval of 
0.29 instead of 0.09.

The idea is to adjust the draw-rate of engine-tests and tournaments in a recommended interval of around 45%-60% by 
using a different UHO 2022 opening set. 
At the moment, it is recommended to use the first UHO 2022 set with [+1.10;+1.19]. But if this set gives a too high
draw-rate, because of a very drawish test-environment (Stockfish vs. another Stockfish for example), you can use a 
higher UHO 2022 set, to shrink the draw-rate.
Same for the computerchess in the future, where faster hardware and stronger engines will lead to increasing
draw-rates, too. Then using a higher UHO 2022 (more advantage for white) set can be useful.
But mention, that the draw-rate should not get too low (below 45%), because this will lead to a lot of 1:1-pairs
(= one opening line is clearly won for white, when Engine A and Engine B plays white in a head-to-head, both Engines
win one game, using that opening-line (=1:1 points/pair)). If the draw-rate in your engine-testings is below 45%, 
you should switch to a lower UHO 2022 set (=smaller advantage for white) otherwise you will shrink the Elo spreading 
of your test-results!!!

Mention, that the UHO 2022 files are getting smaller and smaller, the higher the Eval-interval is, because the games
in the Megabase getting rarer and rarer, giving white such a huge advantage after 6/8 moves.

The UHO openings give white a clear advantage, so these openings can be used for Armageddon rescoring or using
my Gampairs Rescorer Tool (both are included in the UHO 2022 download (Rescoring tools for biased openings)).
But it is also possible, of course, to use these openings with regular chess with classcial scoring system. 


(C) 2022 Stefan Pohl (SPCC)(www.sp-cc.de)


Testing-results:

Stockfish 14.1 avx2 vs. KomodoDragon 2.6 avx2, 5min+3sec, singlethread, no bases, no ponder, 
AMD Ryzen 3900 12-core (24 threads) notebook. Cutechess-cli. 1000 games each testrun. 

Stockfish 14.1 point of view: 

Balsa_v2724 openings:        1000 (+45,=951,-4),   Score: 52.0%, Elo: +14, Draws: 95.1%
Feobos c3 openings:          1000 (+121,=865,-14), Score: 55.4%, Elo: +38, Draws: 86.5%
Hert 500 openings:           1000 (+84,=904,-12),  Score: 53.6%, Elo: +25, Draws: 90.4%
Stockfish 8moves_V3:         1000 (+90,=897,-13),  Score: 53.9%, Elo: +27, Draws: 89.7%

Average result of the 4 classical openings:        Score: 53.7%, Elo: +26, Draws: 90.4%


Re-test of old UHO V3 for comparsion:
(UHO_V3_8mvs_+090_+099:      1000 (+332,=580,-88), Score: 62.2%, Elo: +87, Draws: 58.0%)

Here the results of the new UHO 2022 with recommended Eval-interval of [+1.10;+1.19]:
UHO_2022_6mvs_+110_+119:     1000 (+364,=540,-96), Score: 63.4%, Elo: +96, Draws: 54.0%
UHO_2022_8mvs_+110_+119:     1000 (+340,=575,-85), Score: 62.8%, Elo: +91, Draws: 57.5%
UHO_2022_8mvs_big_+100_+129: 1000 (+327,=599,-74), Score: 62.6%, Elo: +91, Draws: 59.9%

These results are really impressive!
The Elo-spreading, using UHO 2022 openings, is around 3.5x bigger than the classical
openings (more than +90 Elo instead of +26 Elo (!!!))
And the draw-rate is much, much lower (around 57% instead of 90% (!!!))

For the future or games played on extremly fast hardware and/or long thinking-time, here
the testing-results of some UHO 2022 sets with higher Evals. You can see, that the 
Elo-spreading gets worse (=smaller), when the draw-rate is too small (below 45%) - 
because of too many 1:1 gamepairs (the advantage for white is too big)

UHO_2022_6mvs_+120_+129:     1000 (+384,=498,-118),Score: 63.3%, Elo: +96, Draws: 49.8%
UHO_2022_8mvs_+120_+129:     1000 (+387,=481,-132),Score: 62.8%, Elo: +91, Draws: 48.1%
UHO_2022_8mvs_big_+110_+139: 1000 (+388,=492,-120),Score: 63.4%, Elo: +96, Draws: 49.2%

UHO_2022_6mvs_+130_+139:     1000 (+412,=448,-140),Score: 63.6%, Elo: +98, Draws: 44.8%
UHO_2022_8mvs_+130_+139:     1000 (+416,=421,-163),Score: 62.6%, Elo: +91, Draws: 42.1%
UHO_2022_8mvs_big_+120_+149: 1000 (+406,=432,-162),Score: 62.2%, Elo: +87, Draws: 43.2%

UHO_2022_6mvs_+140_+149:     1000 (+436,=365,-199),Score: 61.9%, Elo: +85, Draws: 36.5%
UHO_2022_8mvs_+140_+149:     1000 (+439,=355,-206),Score: 61.6%, Elo: +83, Draws: 35.5%
UHO_2022_8mvs_big_+130_+159: 1000 (+436,=361,-203),Score: 61.6%, Elo: +83, Draws: 36.1%

UHO_2022_8mvs_+150_+159:     1000 (+456,=271,-273),Score: 59.1%, Elo: +65, Draws: 27.1%
