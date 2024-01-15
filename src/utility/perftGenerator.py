import chess

board = chess.Board("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ")

def recursion(depth: int):

    if depth == 0:
        return 1

    nodes: int = 0
    for move in board.legal_moves:
        board.push(move)
        nodes += recursion(depth-1)
        board.pop()
        
    return nodes

def perft(depth: int):

    for i in range(1, depth+1):
        print(f"depth: {i}, nodes: {recursion(i)}")
        
def perftSplit(depth: int):
    i: int = 0
    j: int = 0
    for move in board.legal_moves:
        board.push(move)
        nodes = recursion(depth-1)
        print(f"{move}: {nodes}")
        board.pop()
        i += 1
        j += nodes
    print(f"moveCount: {i}, total: {j}")
    
    
board.push_uci("f3d3")
board.push_uci("e8c8")
board.push_uci("d3c4")
board.push_uci("c7c5")

perftSplit(2)
