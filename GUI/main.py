from chess import *
import pygame
import sys
import time
import subprocess


# ===== INIT FEN =====

def getSquareArray(board):
    squareArray = [ "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-" ]

    for color in [WHITE, BLACK]:
        for piece in [PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING]:

            indexList = board.pieces(piece, color)
            for index in indexList:
                squareArray[index] = piece_symbol(piece) if color==BLACK else piece_symbol(piece).upper()
    
    return squareArray


# ===== DRAW THE BOARD =====

TILE_SIZE = 75
OFFSET_X = TILE_SIZE * 3
OFFSET_Y = TILE_SIZE

screen = pygame.display.set_mode((12 * TILE_SIZE, 10 * TILE_SIZE))
screen.fill((30, 30, 30))
clock = pygame.time.Clock()
pieceImage = pygame.image.load("C:\\Users\\nikol\\Desktop\\VS Code Dateien\\.vscode\\Pieces.png")


def getImage(x, y):
    image = pygame.Surface([100, 100], pygame.SRCALPHA).convert_alpha()
    image.blit(pieceImage, (0, 0), (x * 100, y * 100, 100, 100))
    image = pygame.transform.scale(image, (TILE_SIZE, TILE_SIZE))
    return image

piecePics = {
    "K": getImage(0, 0), "Q": getImage(1, 0), "B": getImage(2, 0),
    "N": getImage(3, 0), "R": getImage(4, 0), "P": getImage(5, 0),
    "k": getImage(0, 1), "q": getImage(1, 1), "b": getImage(2, 1),
    "n": getImage(3, 1), "r": getImage(4, 1), "p": getImage(5, 1),
    "-": getImage(6, 2)
}

def drawSquare(i, squareArray):
    file = i % 8
    rank = 7 - int(i / 8)
    color = (181, 136, 99) if (file + rank) % 2 != 0 else (240, 217, 181)

    x = file * TILE_SIZE + OFFSET_X
    y = rank * TILE_SIZE + OFFSET_Y

    pygame.draw.rect(screen, color, (x, y, TILE_SIZE, TILE_SIZE))
    try:
        if squareArray[i] != "-":
            screen.blit(piecePics[squareArray[i]], (x, y))
    except:
        pass

def drawBoard(squareArray):
    for i in range(0, 64):
        drawSquare(i, squareArray)
    pygame.display.flip()

displayFenPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\displayFen.txt"
def readFen():
    with open(displayFenPath, "r") as txt:
        fen = txt.read().replace('\n', '')
    return fen


# ===== BUTTON & EVAL BAR =====

buttonRect = pygame.Rect((TILE_SIZE / 2, 4.5 * TILE_SIZE, 2 * TILE_SIZE, TILE_SIZE))
pygame.draw.rect(screen, "darkgrey", buttonRect)


def drawEvalBar(eval):
    x = 8 * TILE_SIZE + OFFSET_X
    y = OFFSET_Y

    width = TILE_SIZE / 4
    evalHeight = y + TILE_SIZE * 3 + eval / 250 * TILE_SIZE

    pygame.draw.rect(screen, "white", (x, y, width, TILE_SIZE * 8))
    pygame.draw.rect(screen, "black", (x, y, width, evalHeight))
    pygame.draw.rect(screen, (30, 30, 30), (x, y + TILE_SIZE * 8, width, TILE_SIZE))
    pygame.display.flip()


# ===== MAKE MOVES =====

def getMoveDict(board=Board):
    moves = list(board.legal_moves)
    moveDict = {}
    fullMoveDict = {}

    for move in moves:
        # start and end are both an index
        start = move.from_square
        dest = move.to_square

        if start not in moveDict.keys():
            moveDict[start] = []
            fullMoveDict[start] = []
        moveDict[start].append(dest)
        fullMoveDict[start].append(move)

    return moveDict, fullMoveDict


def getMouseIndex(mouse):
    rank = 7 - int((mouse[1] - OFFSET_Y) / TILE_SIZE)
    file =     int((mouse[0] - OFFSET_X) / TILE_SIZE)
    return rank * 8 + file


def getLegalMovesClicked(index, dict):

    if index in dict.keys():
        return dict[index]
    return []


def drawClickedMoves(moves=list, squareArray=list):

    drawBoard(squareArray)

    overlay = pygame.Surface((TILE_SIZE, TILE_SIZE))
    overlay.set_alpha(150)
    overlay.fill(((52, 161, 235)))

    for dest in moves:
        rank = 7 - int(dest / 8)
        file = dest % 8
        screen.blit(overlay, (file * TILE_SIZE + OFFSET_X, rank * TILE_SIZE + OFFSET_Y))
    
    pygame.display.flip()

writeMovePath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\moveFromPython.txt"
def makeMove(start, dest, fullMoveDict, board=Board):

    if start in fullMoveDict.keys() and len(fullMoveDict[start]) > 0:
        moves = fullMoveDict[start]
        for move in moves:
            if move.to_square == dest:
                board.push(move)
                with open(writeMovePath, "w") as txt:
                    txt.write(str(move))
        return True
    
    else:
        False

    

# ===== MAIN LOOP =====


readMovePath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\moveFromCS.txt"
exePath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\bin\\Debug\\net7.0\\Schneckbert 0.2.exe"
evalPath = "C:\\Users\\nikol\\Desktop\\VS Code Dateien\\Schneckbert 0.2\\Schneckbert 0.2\\resources\\eval.txt";


def play(fenIndex):
    
    while True:
        with open(writeMovePath, "w") as txt:
            txt.write(str(0))
        with open(readMovePath, "w") as txt:
            txt.write(str(0))
        with open(evalPath, "w") as txt:
            txt.write(str(0))

        p = subprocess.Popen([exePath, str(fenIndex)])
        fenIndex += 1

        time.sleep(0.5)

        fen = readFen()
        board = Board(fen)
        squareArray = getSquareArray(board)
        drawBoard(squareArray)
        drawEvalBar(0)

        moveDict, fullMoveDict = getMoveDict(board)
        movesClicked = []

        moveStart = None
        lastMove = None

        currentEval = 0

        doInnerLoop = True
        while doInnerLoop:

            for event in pygame.event.get():

                """
                handle quitting the window
                """
                if event.type == pygame.QUIT:
                    with open(writeMovePath, "w") as txt:
                        txt.write(str(0))
                    with open(readMovePath, "w") as txt:
                        txt.write(str(0))
                    pygame.quit()
                    sys.exit()
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_ESCAPE:
                        with open(writeMovePath, "w") as txt:
                            txt.write(str(0))
                        with open(readMovePath, "w") as txt:
                            txt.write(str(0))
                        pygame.quit()
                        sys.exit()

                """
                handle making a human move, always moves as white
                """
                if event.type == pygame.MOUSEBUTTONDOWN:
                    mouse = pygame.mouse.get_pos()
                    mouseIndex = getMouseIndex(mouse)

                    if buttonRect.collidepoint(mouse):
                        p.kill()
                        doInnerLoop = False

                    if board.turn == WHITE and mouseIndex in movesClicked:
                        if makeMove(moveStart, mouseIndex, fullMoveDict, board):

                            squareArray = getSquareArray(board)
                            drawBoard(squareArray)

                            moveStart = None
                            moveDict, fullMoveDict = getMoveDict(board)

                    elif board.turn == WHITE:
                        movesClicked = getLegalMovesClicked(mouseIndex, moveDict)
                        moveStart = mouseIndex
                        drawClickedMoves(movesClicked, squareArray)
            """
            Make Bot Move, always moves as black
            """
            if board.turn == BLACK:
                with open(readMovePath, "r") as txt:
                    move = txt.read().replace('\n', '')
                    if move != lastMove and move != "0":
                        try:
                            board.push_uci(move)
                            lastMove = move

                            squareArray = getSquareArray(board)
                            drawBoard(squareArray)

                            moveStart = None
                            moveDict, fullMoveDict = getMoveDict(board)
                        except:
                            print("ERROR OCCURED! ITS IN PYTHON")
                            print(move)


            """
            Draw eval bar
            """
            with open(evalPath, "r") as txt:
                eval = txt.read()
                if currentEval != eval:
                    currentEval = eval
                    try:
                        drawEvalBar(int(eval))
                    except:
                        pass

            clock.tick(15)

play(120)
