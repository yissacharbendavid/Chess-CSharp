using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new ChessGame().playGame();
        }
    }
    class ChessGame
    {
        Board board;
        string[] boardHistory;
        public ChessGame()
        {
            board = new Board();
            board.setBoard();
            boardHistory = new string[50];
            resetBoardHistory();
        }
        public void playGame()
        {
            Console.WriteLine(board);
            int BoardHistoryIndex = 0;
            while (true)
            {
                BoardHistoryIndex = addBoardToHistoryAndUpdateIndex(BoardHistoryIndex);
                if (!isStartTurnLegal( true))
                    break;
                if (!makeMoveOrTie(true))
                    break;
                Console.WriteLine(board);
                BoardHistoryIndex = addBoardToHistoryAndUpdateIndex( BoardHistoryIndex);
                if (!isStartTurnLegal(false))
                    break;
                if (!makeMoveOrTie( false))
                    break;
                Console.WriteLine(board);
            }
        }
        bool isStartTurnLegal( bool isWhite)
        {
            if (isCheckmate(isWhite))
            {
                Console.WriteLine("Checkmate!!!");
                return false;
            }
            if (isStalemate(isWhite))
            {
                Console.WriteLine("It's a tie!");
                return false;
            }
            if (board.isCheck(isWhite))
                Console.WriteLine("\nCheck!!!\n");
            return true;
        }
        bool isStalemate(bool isWhite)
        {
            if (!board.isPlayerHaveLegalMoves(isWhite) || board.getMovesCountFor50MovesStalemate() == 50 || isBoardThreeTimesPlayed())
                return true;
            if (board.piecesCount(new Rook(true)) == 0 && board.piecesCount(new Pawn(true)) == 0 && board.piecesCount(new Queen(true)) == 0)
            {
                if ((board.piecesCount(new Bishop(true)) < 2 && board.piecesCount(new Knight(true)) == 0) || (board.piecesCount(new Bishop(true)) == 0 && board.piecesCount(new Knight(true)) < 2))
                    return true;
                if (board.piecesCount(new Bishop(true)) == 2 && board.piecesCount(new Knight(true)) == 0 && board.isBishopsOnSameColor())
                    return true;
            }
            return false;
        }
        bool isCheckmate(bool isWhite)
        {
            if (board.isCheck(isWhite) && !board.isPlayerHaveLegalMoves(isWhite))
                return true;
            return false;
        }
        bool makeMoveOrTie(bool isWhite)
        {
            string input;
            int currentFile, currentRank, moveToFile, moveToRank;
            bool movePlayed;
            do
            {
                input = askForMove(isWhite);
                while (input == "tie")
                {
                    if (isTieApproved())
                    {
                        Console.WriteLine("It's a tie!");
                        return false;
                    }
                    else
                        input = askForMove(isWhite);
                }
                currentFile = convertLetterToNumber(input[0]);
                currentRank = int.Parse("" + input[1]) - 1;
                moveToFile = convertLetterToNumber(input[2]);
                moveToRank = int.Parse("" + input[3]) - 1;
                movePlayed = board.movePiece(currentFile, currentRank, moveToFile, moveToRank, isWhite);
                if (!movePlayed)
                    Console.WriteLine("Illegal move. Try again.");
            } while (!movePlayed);
            if (board.getMovesCountFor50MovesStalemate() == 0)
                resetBoardHistory();
            return true;
        }
        bool isTieApproved()
        {
            Console.WriteLine("Your opponent asked for tie. Do you approve it? (yes or no)");
            string input = Console.ReadLine().Trim();
            while (true)
            {
                if (input.ToLower() == "no")
                    return false;
                if (input.ToLower() == "yes")
                    return true;
                Console.WriteLine("invalid input. Try again, answer with yes or no.");
                input = Console.ReadLine().Trim();
            };
        }
        string askForMove(bool isWhite)
        {
            bool isValid;
            string input;
            do
            {
                isValid = true;
                Console.WriteLine("Player " + (isWhite? "white":"black") + ", please enter your move end press ENTER (current position and desired position), or tie to ask for tie.");
                input = Console.ReadLine().Trim();
                if (input.Length != 4 && input.ToLower() != "tie")
                {
                    isValid = false;
                    Console.WriteLine("Invalid input. Try again.");
                    continue;
                }
                if (input.Length == 4)
                {
                    if (!Char.IsNumber(input, 1) || !Char.IsNumber(input, 3))
                            isValid = false;
                    else
                        if (int.Parse("" + input[1]) < 1 || int.Parse("" + input[1]) > 8 || int.Parse("" + input[3]) < 1 || int.Parse("" + input[3]) > 8)
                            isValid = false;
                    if (convertLetterToNumber(input[0]) < 0 || convertLetterToNumber(input[2]) < 0)
                        isValid = false;
                    if (!isValid)
                        Console.WriteLine("Invalid input. Try again.");
                }
            } while (!isValid);
            return input;
        }
        int convertLetterToNumber(char letter)
        {
            string letters = "abcdefgh";
            letter = Char.ToLower(letter);
            int lettersIndex = -1;
            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == letter)
                    lettersIndex = i;
            }
            return lettersIndex;
        }
        int getBoardIndex()
        {
            string BoardWithoutLastChar;
            for (int i = 0; i < boardHistory.Length; i++)
            {
                BoardWithoutLastChar = boardHistory[i].Remove(boardHistory[i].Length-1,1);
                if (BoardWithoutLastChar == board.boardDetailsAsString())
                    return i;
            }
            return -1;
        }
        int addBoardToHistoryAndUpdateIndex(int boardsIndex)
        {
            if (getBoardIndex() >= 0)
                setHowManyTimesBoardPlayed();
            else
            {
                boardHistory[boardsIndex] = board.boardDetailsAsString() + 1;
                boardsIndex++;
            }
            return boardsIndex;
        }
        void setHowManyTimesBoardPlayed()
        {
            int boardIndex = getBoardIndex();
            int timesPlayed = int.Parse("" + boardHistory[boardIndex][boardHistory[boardIndex].Length-1]);
            boardHistory[boardIndex] =  boardHistory[boardIndex].Remove(boardHistory[boardIndex].Length - 1, 1) + (timesPlayed + 1);
        }
        bool isBoardThreeTimesPlayed()
        {
            for (int i = 0; i < boardHistory.Length; i++)
                if (boardHistory[i][boardHistory[i].Length-1] == '3')
                    return true;
            return false;
        }
        void resetBoardHistory()
        {
            for (int i = 0; i < boardHistory.Length; i++)
                boardHistory[i] = " ";
        }
    }
    class Board
    {
        Piece[,] board;
        int movedRooksAndKings;
        int movesCountFor50MovesStalemate;
        bool isWhiteTurn;
        public Board()
        {
            board = new Piece[8, 8];
            movedRooksAndKings = 0;
            movesCountFor50MovesStalemate = 0;
            isWhiteTurn = true;
        }
        public Piece[,] getBoard()
        {
            return board;
        }
        public void setBoard()
        {
            this.board[0, 0] = new Rook(false);
            this.board[0, 1] = new Knight(false);
            this.board[0, 2] = new Bishop(false);
            this.board[0, 3] = new Queen(false);
            this.board[0, 4] = new King(false);
            this.board[0, 5] = new Bishop(false);
            this.board[0, 6] = new Knight(false);
            this.board[0, 7] = new Rook(false);
            this.board[1, 0] = new Pawn(false);
            this.board[1, 1] = new Pawn(false);
            this.board[1, 2] = new Pawn(false);
            this.board[1, 3] = new Pawn(false);
            this.board[1, 4] = new Pawn(false);
            this.board[1, 5] = new Pawn(false);
            this.board[1, 6] = new Pawn(false);
            this.board[1, 7] = new Pawn(false);
            this.board[6, 0] = new Pawn(true);
            this.board[6, 1] = new Pawn(true);
            this.board[6, 2] = new Pawn(true);
            this.board[6, 3] = new Pawn(true);
            this.board[6, 4] = new Pawn(true);
            this.board[6, 5] = new Pawn(true);
            this.board[6, 6] = new Pawn(true);
            this.board[6, 7] = new Pawn(true);
            this.board[7, 0] = new Rook(true);
            this.board[7, 1] = new Knight(true);
            this.board[7, 2] = new Bishop(true);
            this.board[7, 3] = new Queen(true);
            this.board[7, 4] = new King(true);
            this.board[7, 5] = new Bishop(true);
            this.board[7, 6] = new Knight(true);
            this.board[7, 7] = new Rook(true);
            for (int file = 0; file < board.GetLength(0); file++)
                for (int rank = 0; rank < board.GetLength(1); rank++)
                    if (board[file, rank] == null)
                        board[file, rank] = new EmptyPiece();
        }
        //public void setNewGame()
        //{
        //    this.board[0, 2] = new King("B");
        //    this.board[4, 3] = new Queen("W");
        //    this.board[3, 4] = new Bishop("W");
        //    this.board[5, 4] = new King("W");
        //    for (int i = 0; i < board.GetLength(0); i++)
        //        for (int j = 0; j < board.GetLength(1); j++)
        //            if (board[i, j] == null)
        //                board[i, j] = new EmptyPiece();
        //}
        public bool movePiece(int currentRank, int currentFile, int moveToRank, int moveToFile, bool isWhite)
        {
            if (board[currentFile, currentRank].isMoveLegal(this, currentRank, currentFile, moveToRank, moveToFile, isWhite))
            {
                if (board[currentFile,currentRank] is Pawn || !(board[moveToFile,moveToRank] is EmptyPiece))
                    movesCountFor50MovesStalemate = 0;
                else
                    movesCountFor50MovesStalemate++;
                board[moveToFile, moveToRank] = board[currentFile, currentRank];
                board[currentFile, currentRank] = new EmptyPiece();
                if (board[moveToFile, moveToRank] is Pawn && currentFile != moveToFile && currentRank != moveToRank && board[currentFile, moveToRank] is Pawn)
                    board[currentFile, moveToRank] = new EmptyPiece();
                if (!board[moveToFile, moveToRank].getIsMoved())
                {
                    board[moveToFile, moveToRank].setIsMoved();
                    if (board[moveToFile, moveToRank] is Rook || board[moveToFile, moveToRank] is King)
                        movedRooksAndKings++;
                }
                if (board[moveToFile, moveToRank] is Pawn && (moveToFile == 0 || moveToFile == 7))
                    promotion(moveToFile, moveToRank);
                cancelAllEnPassants(isWhite);
                isWhiteTurn = !isWhiteTurn;
                return true;
            }
            return false;
        }
        public bool isCheck(bool isWhite)
        {
            int kingfile = -1, kingRank = -1;
            for (int rank = 0; rank < board.GetLength(0); rank++)
                for (int file = 0; file < board.GetLength(1); file++)
                {
                    if (board[rank, file] is King && board[rank, file].getIsWhite() == isWhite)
                    {
                        kingfile = rank;
                        kingRank = file;
                    }
                }
            if (kingfile < 0)
                return false;
            for (int rank = 0; rank < board.GetLength(0); rank++)
                for (int file = 0; file < board.GetLength(1); file++)
                    if (board[rank, file].getIsWhite() != isWhite)
                        if (board[rank, file].isMoveLegal(this, file, rank, kingRank, kingfile, board[rank, file].getIsWhite()))
                            return true;
            return false;


        }
        public override string ToString()
        {
            string output = "   A  B  C  D  E  F  G  H \n";
            for (int file = 0; file < board.GetLength(0); file++)
            {
                output += (file + 1) + " ";
                for (int rank = 0; rank < board.GetLength(1); rank++)
                    output += " " + board[file, rank];
                output += "\n";
            }
            return output;

        }
        public void copyBoard(Board boardToCopy)
        {
            for (int file = 0; file < board.GetLength(0); file++)
                for (int rank = 0; rank < board.GetLength(1); rank++)
                    this.board[file, rank] = boardToCopy.board[file, rank];
            movedRooksAndKings = boardToCopy.movedRooksAndKings;
            isWhiteTurn = boardToCopy.isWhiteTurn;
        }
        public void promotion(int file, int rank)
        {
            string input;
            bool isIpnutValid = false;
            while (!isIpnutValid)
            {
                Console.WriteLine("To what piece do you want to promote your pawn? (Enter q for queen, n for knight, r for rook or b for bishop.)");
                input = Console.ReadLine().Trim().ToLower();
                isIpnutValid = true;
                switch (input)
                {
                    case "q": board[file, rank] = new Queen(board[file, rank].getIsWhite()? true : false); break;
                    case "n": board[file, rank] = new Knight(board[file, rank].getIsWhite() ? true : false); break;
                    case "r": board[file, rank] = new Rook(board[file, rank].getIsWhite() ? true : false); break;
                    case "b": board[file, rank] = new Bishop(board[file, rank].getIsWhite() ? true : false); break;
                    default: Console.WriteLine("Invalid input. Try again."); isIpnutValid = false; break;
                }
            }
        }
        public bool isPlayerHaveLegalMoves(bool isWhite)
        {
            for (int file = 0; file < board.GetLength(0); file++)
                for (int rank = 0; rank < board.GetLength(1); rank++)
                    if (board[file, rank].getIsWhite() == isWhite && board[file, rank].isHaveLegalMove(this, file, rank))
                        return true;
            return false;
        }
        public void cancelAllEnPassants(bool isWhite)
        {
            for (int file = 0; file < board.GetLength(0); file++)
                for (int rank = 0; rank < board.GetLength(1); rank++)
                    if (board[file, rank].getIsWhite() == isWhite && board[file, rank] is Pawn)
                    {
                        Pawn pawn = (Pawn)board[file, rank];
                        pawn.setCanEnpassant(false);
                    }
        }
        public bool isBishopsOnSameColor()
        {
            int bishop1File = -1, bishop1Rank = -1, bishop2file = -1, bishop2Rank =-1;
            bool isPlaceColor1White, isPlaceColor2White;
            for (int file = 0 ; file < board.GetLength(0) && bishop2file > 0 ; file++)
                for (int rank = 0 ; rank < board.GetLength(1) && bishop2file > 0 ; rank++)
                {
                    if(board[file, rank] is Bishop)
                        if (bishop1File < 0)
                        {
                            bishop1File = file; bishop1Rank = rank;
                        }
                        else
                        {
                            bishop2file = file; bishop2Rank = rank;
                        }
                }
            isPlaceColor1White = bishop1File%2 == bishop1Rank%2;
            isPlaceColor2White = bishop2file%2 == bishop2Rank%2;
            return isPlaceColor1White == isPlaceColor2White;
        }
        public int getMovesCountFor50MovesStalemate()
            { return movesCountFor50MovesStalemate ; }
        public int piecesCount(object piece)
        {
            int count = 0;
            for (int file = 0; file < board.GetLength(0); file++)
                for (int rank = 0; rank < board.GetLength(1); rank++)
                    if (board[file, rank].GetType() == piece.GetType())
                        count++;
            return count;
        }
        public int possibleEnPassantCount()
        {
            int count = 0;
            for(int file = 0; file < board.GetLength(0); file++)
                for(int rank = 0; rank < board.GetLength(1); rank++)
                    if (board[file, rank] is Pawn)
                    {
                        Pawn pawn = (Pawn)board[file, rank];
                        if (pawn.getCanEnPassant())
                            count++;
                    }
            return count;
        }
        public string boardDetailsAsString()
        {
            return ToString() + isWhiteTurn + movedRooksAndKings + possibleEnPassantCount();
        }
    }
    class Piece
    {
        protected bool isMoved;
        bool isWhite;
        char type;
        public Piece(char type, bool isWhite)
        {
            this.type = type;
            this.isWhite = isWhite;
            isMoved = false;
        }
        public void setIsMoved()
            { this.isMoved = true; }
        public bool getIsMoved()
            { return isMoved; }
        public bool getIsWhite()
        { return isWhite; }
        public override string ToString()
        {
            return (isWhite?"W":"B") + Char.ToUpper(type);
        }
        public virtual bool isMoveLegal(Board gameboard, int currentFile, int currentRank, int moveToFile, int moveToRank, bool isWhite)
        {
            Piece[,] board = gameboard.getBoard();
            if (!(board[moveToRank, moveToFile] is EmptyPiece))
                if (board[moveToRank, moveToFile].getIsWhite() == isWhite)
                    return false;
            if (isWhite != board[currentRank, currentFile].getIsWhite())
                return false;
                    if (board[currentRank, currentFile] is EmptyPiece)
                return false;
            if (currentRank == moveToRank && currentFile == moveToFile)
                return false;
            Board copy = new Board();
            Piece[,] boardCopy = copy.getBoard();
            copy.copyBoard(gameboard);
            boardCopy[moveToRank, moveToFile] = boardCopy[currentRank, currentFile];
            boardCopy[currentRank, currentFile] = new EmptyPiece();
            if (copy.isCheck(board[currentRank, currentFile].getIsWhite()))
            {
                if (copy.piecesCount(new King(true)) != 2)
                    return true;
                return false;
            }
            return true;
        }
        public bool isHaveLegalMove(Board gameboard, int currentFile, int currentRank)
        {
            Piece[,] board = gameboard.getBoard();
            for(int file = 0; file < board.GetLength(0); file++)
                for(int rank = 0; rank < board.GetLength(1); rank++)
                    if(isMoveLegal(gameboard,currentRank,currentFile,file,rank,board[currentFile,currentRank].getIsWhite()))
                        return true;
            return false;
        }
        public int distance(int num1, int num2)
        { return num1 > num2 ? num1 - num2 : num2 - num1; }
    }
    class Pawn : Piece
    {
        bool canEnPassant;
        public Pawn(bool isWhite):base ('p',isWhite) { }
        public override bool isMoveLegal(Board gameBoard, int currentFile, int currentRank, int moveToFile, int moveToRank, bool isWhite)
        {
            if (!base.isMoveLegal(gameBoard, currentFile, currentRank, moveToFile, moveToRank, isWhite))
                return false;
            if (!isDirectionLegal(currentRank,moveToRank) )
                return false;
            Piece[,] board = gameBoard.getBoard();
            if (currentFile != moveToFile)
            {
                if (distance(currentFile, moveToFile) != 1 || distance(currentRank, moveToRank) != 1)
                    return false;
                if(canEnPassant)
                {
                    if(!(board[currentRank,moveToFile] is Pawn))
                        return false;
                }
                else if (board[moveToRank, moveToFile] is EmptyPiece)
                    return false;
            }
            else if (!(board[moveToRank, moveToFile] is EmptyPiece))
                return false;
            if (!getIsMoved())
            {
                if (distance(currentRank,moveToRank) == 2)
                    setCanBeEatenByEnPassant(gameBoard,moveToFile,moveToRank);
                if (distance(currentRank, moveToRank) >2)
                    return false;
            }
            else if (distance(currentRank,moveToRank) > 1)
                return false;
            return true;
        }
        public bool isDirectionLegal(int currentRank,  int moveToRank)
        {
            if (getIsWhite())
            {
                if (currentRank < moveToRank)
                    return false;
            }
            else if (currentRank > moveToRank)
                return false;
            return true;
        }
        public void setCanEnpassant(bool canEnPassant)
        { this.canEnPassant = canEnPassant; }
        public bool getCanEnPassant()
            { return canEnPassant; }
        public void setCanBeEatenByEnPassant(Board gameBoard, int moveToRank, int moveToFile)
        {
            Piece[,] board = gameBoard.getBoard();
            if (moveToRank != 7)
                if (board[moveToFile, moveToRank + 1] is Pawn && board[moveToFile, moveToRank + 1].getIsWhite() != getIsWhite())
                {
                    Pawn pawn = (Pawn)board[moveToFile, moveToRank + 1];
                    pawn.setCanEnpassant(true);
                }
            if (moveToRank != 0)
                if (board[moveToFile, moveToRank - 1] is Pawn && board[moveToFile, moveToRank - 1].getIsWhite() != getIsWhite())
                {
                    Pawn pawn = (Pawn)board[moveToFile, moveToRank - 1];
                    pawn.setCanEnpassant(true);
                }
        }
    }
    class Rook: Piece
    {
        public Rook(bool isWhite) : base('r', isWhite) { }
        public override bool isMoveLegal(Board gameBoard, int currentFile, int currentRank, int moveToFile, int moveToRank, bool isWhite)
        {
            if (!base.isMoveLegal(gameBoard, currentFile, currentRank, moveToFile, moveToRank, isWhite))
                return false;
            if (moveToRank != currentRank && moveToFile != currentFile)
                return false;
            if (moveToRank != currentRank)
            {
                if (!isMoveLegalOnFile(gameBoard, currentFile, currentRank, moveToRank))
                    return false;
            }
            else
            {
                if (!isMoveLegalOnRank(gameBoard, currentFile, currentRank, moveToFile))
                    return false;
            }
            return true;
        }
        public bool isMoveLegalOnFile(Board gameBoard, int currentFile, int currentRank, int moveToRank)
        {
            Piece[,] board = gameBoard.getBoard();
            if (moveToRank > currentRank)
            {
                for (int rank = currentRank + 1; rank < moveToRank; rank++)
                    if (!(board[rank, currentFile] is EmptyPiece))
                        return false;
            }
            else
                for (int rank = moveToRank +1; rank < currentRank; rank++)
                    if (!(board[rank, currentFile] is EmptyPiece))
                        return false;
            return true;
        }
        public bool isMoveLegalOnRank(Board gameBoard, int currentFile, int currentRank, int moveToFile)
        {
            Piece[,] board = gameBoard.getBoard();
            if (moveToFile > currentFile)
            {
                for (int file = currentFile + 1; file < moveToFile; file++)
                    if (!(board[currentRank, file] is EmptyPiece))
                        return false;
            }
            else
                for (int file = moveToFile +1; file < currentFile; file++)
                    if (!(board[currentRank, file] is EmptyPiece))
                        return false;
            return true;
        }
    }
    class Knight : Piece
    {
        public Knight(bool isWhite) : base('n', isWhite) { }
        public override bool isMoveLegal(Board gameBoard, int currentFile, int currentRank, int moveToFile, int moveToRank, bool isWhite)
        {
            if (!base.isMoveLegal(gameBoard, currentFile, currentRank, moveToFile, moveToRank, isWhite))
                return false;
            if (currentRank - moveToRank == 2 || moveToRank - currentRank == 2)
                if (currentFile - moveToFile == 1 || moveToFile - currentFile == 1)
                    return true;
            if (currentFile - moveToFile == 2 || moveToFile - currentFile == 2)
                if (currentRank - moveToRank == 1 || moveToRank - currentRank == 1)
                    return true;
            return false;
        }
    }
    class Queen : Piece
    {
        public Queen(bool isWhite) : base('q', isWhite) { }
        public override bool isMoveLegal(Board gameBoard, int currentFile, int currentRank, int moveToFile, int moveToRank, bool isWhite)
        {
            if (base.isMoveLegal(gameBoard, currentFile, currentRank, moveToFile, moveToRank, isWhite) &&
                ( canMoveLikeRook(gameBoard, currentFile, currentRank, moveToFile, moveToRank)|| canMoveLikeBishop(gameBoard, currentFile, currentRank, moveToFile, moveToRank)))
                return true;
            return false;          
        }
        public bool canMoveLikeBishop(Board gameBoard, int currentFile, int currentRank, int moveToFile, int moveToRank)
        {
            Piece[,] board = gameBoard.getBoard();
            if (distance(currentFile, moveToFile) != distance(currentRank, moveToRank))
                return false;
            for (int index = 1, rank = currentRank, file = currentFile; index < distance(currentFile, moveToFile); index++)
            {
                if (rank < moveToRank)
                    rank++;
                else
                    rank--;
                if (file < moveToFile)
                    file++;
                else
                    file--;
                if (!(board[rank, file] is EmptyPiece))
                    return false;
            }
            return true;
        }
        public bool canMoveLikeRook(Board gameBoard, int currentFile, int currentRank, int moveToFile, int moveToRank)
        {
            if (moveToRank != currentRank && moveToFile != currentFile)
                return false;
            if (moveToRank != currentRank)
            {
                if (!isMoveLegalOnRank(gameBoard, currentFile, currentRank, moveToRank))
                    return false;
            }
            else
            {
                if (!isMoveLegalOnFile(gameBoard, currentFile, currentRank, moveToFile))
                    return false;
            }
            return true;
        }
        public bool isMoveLegalOnRank(Board gameBoard, int currentFile, int currentRank, int moveToRank)
        {
            Piece[,] board = gameBoard.getBoard();
            if (moveToRank > currentRank)
            {
                for (int file = currentRank + 1; file < moveToRank; file++)
                    if (!(board[file, currentFile] is EmptyPiece))
                        return false;
            }
            else
                for (int file = moveToRank +1; file < currentRank; file++)
                    if (!(board[file, currentFile] is EmptyPiece))
                        return false;
            return true;
        }
        public bool isMoveLegalOnFile(Board gameBoard, int currentFile, int currentRank, int moveToFile)
        {
            Piece[,] board = gameBoard.getBoard();
            if (moveToFile > currentFile)
            {
                for (int rank = currentFile + 1; rank < moveToFile; rank++)
                    if (!(board[currentRank, rank] is EmptyPiece))
                        return false;
            }
            else
                for (int rank = moveToFile +1; rank < currentFile; rank++)
                    if (!(board[currentRank, rank] is EmptyPiece))
                        return false;
            return true;
        }
    }
    class King : Piece
    {
        public King(bool isWhite) : base('k', isWhite) { }
        public override bool isMoveLegal(Board gameboard, int currentFile, int currentRank, int moveToFile, int moveToRank, bool isWhite)
        {
            bool isLegal = true;
            if(! base.isMoveLegal(gameboard, currentFile, currentRank, moveToFile, moveToRank, isWhite))
                isLegal =  false;
            if (isMoved == false && (moveToFile == 6 || moveToFile == 2) && (currentRank == 0 || currentRank == 7) && distance(moveToFile,currentFile) > 1)
            {
                    if (!isCastelingLegal(gameboard, currentRank, moveToFile, moveToRank))
                        isLegal = false;
            }
            else if (distance(moveToRank, currentRank) > 1 || distance(moveToFile, currentFile) > 1)
                isLegal = false;
            return isLegal;
        }
        public bool isCastelingLegal(Board gameboard, int currentRank, int moveToFile, int moveToRank)
        {
            Piece[,] board = gameboard.getBoard();
            Board copy = new Board();
            copy.copyBoard(gameboard);
            if (moveToFile == 6)
                if (!board[currentRank,7].getIsMoved() && !gameboard.isCheck(getIsWhite()))
                    if (isMoveLegal(gameboard,4, currentRank,5, moveToRank, getIsWhite()))
                    {
                        copy.movePiece(4, currentRank, 5, moveToRank, getIsWhite());
                        if (isMoveLegal(copy, 5, currentRank, 6, moveToRank, getIsWhite()))
                        {
                            gameboard.movePiece(7, currentRank, 5, currentRank,getIsWhite());
                            return true;
                        }
                    }
            if (moveToFile == 2)
                if (!board[currentRank,0].getIsMoved() && !gameboard.isCheck(getIsWhite()))
                    if (isMoveLegal(gameboard, 4, currentRank, 3, moveToRank, getIsWhite()))
                    {
                        copy.movePiece(4, currentRank, 3, moveToRank, getIsWhite());
                        if (isMoveLegal(copy, 3, currentRank, 2, moveToRank, getIsWhite()))
                        {
                            gameboard.movePiece(0, currentRank, 3, currentRank, getIsWhite());
                            return true;
                        }
                    }
            return false;
        }
    }
    class Bishop : Piece
    {
        public Bishop(bool isWhite) : base('b', isWhite) { }
        public override bool isMoveLegal(Board gameBoard, int currentFile, int currentRank, int moveToFile, int moveToRank, bool isWhite)
        {
            if (!base.isMoveLegal(gameBoard, currentFile, currentRank, moveToFile, moveToRank, isWhite))
                return false;
            Piece[,] board = gameBoard.getBoard();
            if (distance(currentFile, moveToFile) != distance(currentRank, moveToRank))
                return false;
            for (int index = 1, Rank = currentRank, File = currentFile ; index < distance(currentFile, moveToFile); index++)
            {
                if (Rank < moveToRank)
                    Rank++;
                else
                    Rank--;
                if (File < moveToFile)
                    File++;
                else
                    File--;
                if (!(board[Rank,File] is EmptyPiece))
                    return false;
            }
            return true;
        }
    }
    class EmptyPiece:Piece
    {
        public EmptyPiece():base('e', true) { isMoved = true; }
        public override string ToString() { return "--"; }
    }
}