using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckersWinApp.Logic
{
    public class CheckersLogic
    {
        private readonly Board r_Board;
        private readonly Player r_Player1;
        private readonly Player r_Player2;
        private readonly Random r_RandForComputer;
        private Player m_CurrentPlayer;
        private eGameStatus m_GameStatus;
        private Move m_LastMove = null;
        private Player m_LastPlayer = null;
        private Position? m_ForcedCapturePiecePosition = null;

        public Player LastPlayer
        {
            get
            {
                return m_LastPlayer;
            }
        }

        public Player CurrentPlayer
        {
            get
            {
                return m_CurrentPlayer;
            }
        }

        public Player Player1
        {
            get
            {
                return r_Player1;
            }
        }

        public Player Player2
        {
            get
            {
                return r_Player2;
            }
        }

        public Move LastMove
        {
            get
            {
                return m_LastMove;
            }
        }

        public int BoardSize
        {
            get
            {
                return r_Board.Size;
            }
        }

        public eGameStatus GameStatus
        {
            get
            {
                return m_GameStatus;
            }
        }

        public Player Winner
        {
            get
            {
                Player winner;

                switch (m_GameStatus)
                {
                    case eGameStatus.Player1Won:
                        winner = r_Player1;
                        break;
                    case eGameStatus.Player2Won:
                        winner = r_Player2;
                        break;
                    default:
                        winner = null;
                        break;
                }

                return winner;
            }
        }

        public CheckersLogic(int i_BoardSize, string i_Player1Name, string i_Player2Name, bool i_IsVsComputer)
        {
            r_Board = new Board(i_BoardSize);
            r_RandForComputer = new Random();
            r_Player1 = new Player(ePlayerSide.Bottom, i_Player1Name);
            r_Player2 = new Player(ePlayerSide.Top, i_Player2Name, i_IsVsComputer);
            m_CurrentPlayer = r_Player1;
            m_GameStatus = eGameStatus.InProgress;
            initializePlayersPieces();
        }

        public ePieceType GetPieceAt(int i_Row, int i_Col)
        {
            return r_Board.GetPieceAt(i_Row, i_Col);
        }

        private void initializePlayersPieces()
        {
            for (int row = 0; row < r_Board.Size; row++)
            {
                for (int col = 0; col < r_Board.Size; col++)
                {
                    ePieceType piece = r_Board.GetPieceAt(row, col);

                    if (piece == r_Player1.RegularPiece || piece == r_Player1.KingPiece)
                    {
                        r_Player1.AddPiece(row, col, piece);
                    }
                    else if (piece == r_Player2.RegularPiece || piece == r_Player2.KingPiece)
                    {
                        r_Player2.AddPiece(row, col, piece);
                    }
                }
            }
        }

        public bool MakeMove(Move i_Move, out string o_ErrorMsg)
        {
            o_ErrorMsg = string.Empty;
            bool moveSucceeded = true;
            bool isCaptureMove = i_Move.IsJump();
            List<Move> captureMoves = generateMovesForPlayer(m_CurrentPlayer, true);

            if (m_ForcedCapturePiecePosition.HasValue)
            {
                Position forcedPos = m_ForcedCapturePiecePosition.Value;

                if (i_Move.From.Row != forcedPos.Row || i_Move.From.Column != forcedPos.Column)
                {
                    o_ErrorMsg = "You must continue capturing with the same piece!";
                    moveSucceeded = false;
                }
            }

            if (moveSucceeded)
            {
                if (captureMoves.Count > 0 && !isCaptureMove)
                {
                    o_ErrorMsg = "You must make a capture move if available!";
                    moveSucceeded = false;
                }
            }

            if (moveSucceeded)
            {
                if (!isMoveLegal(m_CurrentPlayer, i_Move, isCaptureMove, out o_ErrorMsg))
                {
                    moveSucceeded = false;
                }
            }

            if (moveSucceeded)
            {
                performMove(i_Move, isCaptureMove);
                m_LastPlayer = m_CurrentPlayer;

                if (isCaptureMove && m_ForcedCapturePiecePosition == null)
                {
                    m_ForcedCapturePiecePosition = new Position(i_Move.To.Row, i_Move.To.Column);
                }

                if (isCaptureMove && hasCaptureMoveFromCell(i_Move.To.Row, i_Move.To.Column))
                {
                    m_CurrentPlayer = m_LastPlayer;
                    m_ForcedCapturePiecePosition = new Position(i_Move.To.Row, i_Move.To.Column);
                }
                else
                {
                    m_ForcedCapturePiecePosition = null;
                    bool gameEnded = updateGameStatusIfOver();

                    if (!gameEnded)
                    {
                        switchTurn();
                    }
                }
            }

            return moveSucceeded;
        }

        public bool MakeComputerMove(out Move o_ChosenMove, out string o_ErrorMsg)
        {
            o_ChosenMove = null;
            o_ErrorMsg = string.Empty;

            // שלב 1: בדיקה אם יש למחשב מהלכים
            List<Move> possibleMoves = getAllPossibleMovesForPlayer(m_CurrentPlayer);
            if (possibleMoves.Count == 0)
            {
                // אם אין מהלכים זמינים, חוזרים false
                o_ErrorMsg = "No available moves for the computer.";
                return false;
            }

            // שלב 2: בחירת מהלך אקראי
            int index = r_RandForComputer.Next(possibleMoves.Count);
            Move chosenMove = possibleMoves[index];

            // שלב 3: ביצוע המהלך
            bool moveSucceeded = MakeMove(chosenMove, out string errorMsg);
            if (!moveSucceeded)
            {
                // אם המהלך נכשל מסיבה כלשהי
                o_ErrorMsg = errorMsg;
                return false;
            }

            // שלב 4: אם הצלחנו לבצע מהלך, נעדכן את הערך ב־out
            o_ChosenMove = chosenMove;
            // אין הודעת שגיאה
            o_ErrorMsg = string.Empty;

            return true;
        }

        public int ComputePoints(Player i_Player)
        {
            return computePiecesValue(i_Player);
        }

        public void QuitGame()
        {
            m_GameStatus = m_CurrentPlayer == r_Player1 ? eGameStatus.Player2Won : eGameStatus.Player1Won;
        }

        public void Reset(int i_BoardSize)
        {
            r_Board.InitializeBoard(i_BoardSize);
            r_Player1.ClearPieces();
            r_Player2.ClearPieces();
            initializePlayersPieces();
            m_CurrentPlayer = r_Player1;
            m_GameStatus = eGameStatus.InProgress;
            m_LastMove = null;
            m_LastPlayer = null;
            m_ForcedCapturePiecePosition = null;
        }

        private bool isPieceOwnedByPlayer(Player i_Player, int i_Row, int i_Col)
        {
            return i_Player.FindPiece(i_Row, i_Col) != null;
        }

        private bool validateBasicMoveConditions(Player i_Player, Move i_Move, out string o_ErrorMsg)
        {
            o_ErrorMsg = string.Empty;
            bool isLegal = true;

            if (!isPieceOwnedByPlayer(i_Player, i_Move.From.Row, i_Move.From.Column))
            {
                isLegal = false;
                o_ErrorMsg = "You can only move your own pieces.";
            }
            else if (!isInRange(i_Move.From.Row, i_Move.From.Column) || !isInRange(i_Move.To.Row, i_Move.To.Column))
            {
                isLegal = false;
                o_ErrorMsg = "One or both positions are out of range.";
            }
            else
            {
                int rowDiff = i_Move.To.Row - i_Move.From.Row;
                ePieceType movingPiece = r_Board.GetPieceAt(i_Move.From.Row, i_Move.From.Column);

                if (!isDirectionValid(movingPiece, rowDiff))
                {
                    isLegal = false;
                    o_ErrorMsg = "Invalid move direction.";
                }
                else if (r_Board.GetPieceAt(i_Move.To.Row, i_Move.To.Column) != ePieceType.None)
                {
                    isLegal = false;
                    o_ErrorMsg = "Destination position is not empty.";
                }
            }

            return isLegal;
        }

        private bool isRegularMoveLegal(Move i_Move, out string o_ErrorMsg)
        {
            o_ErrorMsg = string.Empty;
            bool isLegal = true;
            int rowDiff = Math.Abs(i_Move.To.Row - i_Move.From.Row);
            int colDiff = Math.Abs(i_Move.To.Column - i_Move.From.Column);

            if (rowDiff != 1 || colDiff != 1)
            {
                isLegal = false;
                o_ErrorMsg = "Regular move must be exactly 1 square diagonally.";
            }

            return isLegal;
        }

        private bool isCaptureMoveLegal(Move i_Move, out string o_ErrorMsg)
        {
            o_ErrorMsg = string.Empty;
            bool isLegal = true;
            int rowDiff = i_Move.To.Row - i_Move.From.Row;
            int colDiff = i_Move.To.Column - i_Move.From.Column;

            if (Math.Abs(rowDiff) != 2 || Math.Abs(colDiff) != 2)
            {
                isLegal = false;
                o_ErrorMsg = "Capture move must jump exactly 2 squares diagonally.";
            }
            else
            {
                int midRow = (i_Move.From.Row + i_Move.To.Row) / 2;
                int midCol = (i_Move.From.Column + i_Move.To.Column) / 2;
                ePieceType movingPiece = r_Board.GetPieceAt(i_Move.From.Row, i_Move.From.Column);
                ePieceType midPiece = r_Board.GetPieceAt(midRow, midCol);

                if (!isOpponentPiece(movingPiece, midPiece))
                {
                    isLegal = false;
                    o_ErrorMsg = "No opponent piece to capture in the middle.";
                }
            }

            return isLegal;
        }

        private bool isMoveLegal(Player i_Player, Move i_Move, bool i_IsCapture, out string o_ErrorMsg)
        {
            o_ErrorMsg = string.Empty;
            bool isLegal = true;

            if (!validateBasicMoveConditions(i_Player, i_Move, out o_ErrorMsg))
            {
                isLegal = false;
            }
            else
            {
                if (i_IsCapture)
                {
                    bool captureLegal = isCaptureMoveLegal(i_Move, out string captureError);

                    if (!captureLegal)
                    {
                        isLegal = false;
                        o_ErrorMsg = captureError;
                    }
                }
                else
                {
                    bool regularLegal = isRegularMoveLegal(i_Move, out string regularError);

                    if (!regularLegal)
                    {
                        isLegal = false;
                        o_ErrorMsg = regularError;
                    }
                }
            }

            return isLegal;
        }

        private static bool isDirectionValid(ePieceType i_Piece, int i_RowDiff)
        {
            bool isValid = false;

            if (i_Piece == ePieceType.U || i_Piece == ePieceType.K)
            {
                isValid = true;
            }
            else if (i_Piece == ePieceType.O && i_RowDiff > 0)
            {
                isValid = true;
            }
            else if (i_Piece == ePieceType.X && i_RowDiff < 0)
            {
                isValid = true;
            }

            return isValid;
        }

        private static bool isOpponentPiece(ePieceType i_MyPiece, ePieceType i_OtherPiece)
        {
            bool isOpponent = false;

            if (i_OtherPiece != ePieceType.None)
            {
                bool myTop = (i_MyPiece == ePieceType.O || i_MyPiece == ePieceType.U);
                bool otherTop = (i_OtherPiece == ePieceType.O || i_OtherPiece == ePieceType.U);

                isOpponent = (myTop != otherTop);
            }

            return isOpponent;
        }

        private bool isInRange(int i_Row, int i_Col)
        {
            return i_Row >= 0 && i_Row < r_Board.Size && i_Col >= 0 && i_Col < r_Board.Size;
        }

        public List<Move> getAllPossibleMovesForPlayer(Player i_Player)
        {
            List<Move> resultMoves = new List<Move>();
            List<Move> captureMoves = generateMovesForPlayer(i_Player, true);

            if (captureMoves.Count > 0)
            {
                resultMoves = captureMoves;
            }
            else
            {
                resultMoves = generateMovesForPlayer(i_Player, false);
            }

            return resultMoves;
        }

        private List<Move> generateMovesForPlayer(Player i_Player, bool i_IsCapture)
        {
            List<Move> moves = new List<Move>();
            int stepSize = i_IsCapture ? 2 : 1;
            int[] steps = { -stepSize, stepSize };

            foreach (PlayerPiece currentPiece in i_Player.Pieces)
            {
                Position from = new Position(currentPiece.Row, currentPiece.Col);

                foreach (int directionRow in steps)
                {
                    foreach (int directionCol in steps)
                    {
                        Position to = new Position(currentPiece.Row + directionRow, currentPiece.Col + directionCol);
                        Move move = new Move(from, to);

                        if (isMoveLegal(i_Player, move, i_IsCapture, out string _))
                        {
                            moves.Add(move);
                        }
                    }
                }
            }

            return moves;
        }

        private bool hasCaptureMoveFromCell(int i_Row, int i_Col)
        {
            bool hasCapture = false;
            ePieceType piece = r_Board.GetPieceAt(i_Row, i_Col);

            if (piece != ePieceType.None)
            {
                Position from = new Position(i_Row, i_Col);
                int[] steps = { -2, 2 };

                foreach (int directionRow in steps)
                {
                    foreach (int directionCol in steps)
                    {
                        Position to = new Position(i_Row + directionRow, i_Col + directionCol);
                        Move move = new Move(from, to);

                        if (isMoveLegal(m_CurrentPlayer, move, true, out string errorMsg))
                        {
                            hasCapture = true;
                            break;
                        }
                    }

                    if (hasCapture)
                    {
                        break;
                    }
                }
            }

            return hasCapture;
        }

        private void performMove(Move i_Move, bool i_IsCapture)
        {
            ePieceType movingPiece = r_Board.GetPieceAt(i_Move.From.Row, i_Move.From.Column);
            Player currentPlayer = m_CurrentPlayer;
            Player opponent = null;

            r_Board.SetPieceAt(i_Move.To.Row, i_Move.To.Column, movingPiece);
            r_Board.SetPieceAt(i_Move.From.Row, i_Move.From.Column, ePieceType.None);
            m_LastMove = i_Move;
            currentPlayer.UpdatePiecePosition(i_Move.From.Row, i_Move.From.Column, i_Move.To.Row, i_Move.To.Column);

            if (i_IsCapture)
            {
                int midRow = (i_Move.From.Row + i_Move.To.Row) / 2;
                int midCol = (i_Move.From.Column + i_Move.To.Column) / 2;

                if (currentPlayer == r_Player1)
                {
                    opponent = r_Player2;
                }
                else
                {
                    opponent = r_Player1;
                }

                r_Board.SetPieceAt(midRow, midCol, ePieceType.None);
                opponent.RemovePiece(midRow, midCol);
            }

            ePieceType newType = promoteIfNeeded(i_Move.To.Row, i_Move.To.Column);

            if (newType != movingPiece)
            {
                r_Board.SetPieceAt(i_Move.To.Row, i_Move.To.Column, newType);
                currentPlayer.UpdatePieceType(i_Move.To.Row, i_Move.To.Column, newType);
            }
        }

        private ePieceType promoteIfNeeded(int i_Row, int i_Col)
        {
            ePieceType piece = r_Board.GetPieceAt(i_Row, i_Col);
            ePieceType newType = piece;

            if (piece == ePieceType.O && i_Row == r_Board.Size - 1)
            {
                newType = ePieceType.U;
                r_Board.SetPieceAt(i_Row, i_Col, newType);
            }
            else if (piece == ePieceType.X && i_Row == 0)
            {
                newType = ePieceType.K;
                r_Board.SetPieceAt(i_Row, i_Col, newType);
            }

            return newType;
        }

        private void switchTurn()
        {
            m_CurrentPlayer = m_CurrentPlayer == r_Player1 ? r_Player2 : r_Player1;
        }

        private bool updateGameStatusIfOver()
        {
            bool isOver = false;
            eGameStatus newStatus = determineGameStatus();

            if (newStatus != eGameStatus.InProgress)
            {
                m_GameStatus = newStatus;
                isOver = true;
            }

            return isOver;
        }

        private eGameStatus determineGameStatus()
        {
            eGameStatus status = eGameStatus.InProgress;

            if (bothPlayersStuck())
            {
                status = eGameStatus.Tie;
            }
            else if (isPlayerLoser(r_Player2))
            {
                status = eGameStatus.Player1Won;
            }
            else if (isPlayerLoser(r_Player1))
            {
                status = eGameStatus.Player2Won;
            }

            return status;
        }

        private bool isPlayerLoser(Player i_Player)
        {
            return (!hasAnyPieces(i_Player) || !playerHasAnyMove(i_Player));
        }

        private bool bothPlayersStuck()
        {
            return (!playerHasAnyMove(r_Player1) && !playerHasAnyMove(r_Player2));
        }

        private bool hasAnyPieces(Player i_Player)
        {
            return (i_Player.Pieces.Count > 0);
        }

        private bool playerHasAnyMove(Player i_Player)
        {
            List<Move> allMoves = getAllPossibleMovesForPlayer(i_Player);

            return (allMoves.Count > 0);
        }

        private int computePiecesValue(Player i_Player)
        {
            int value = 0;

            foreach (PlayerPiece currentPiece in i_Player.Pieces)
            {
                if (currentPiece.PieceType == i_Player.RegularPiece)
                {
                    value += 1;
                }
                else if (currentPiece.PieceType == i_Player.KingPiece)
                {
                    value += 4;
                }
            }

            return value;
        }

        public ePieceType GetPieceTypeAtPosition(Player i_Player, int i_Row, int i_Col)
        {
            ePieceType pieceType = ePieceType.None;
            PlayerPiece piece = i_Player.FindPiece(i_Row, i_Col);

            if (piece != null)
            {
                pieceType = piece.PieceType;
            }

            return pieceType;
        }

        public static bool ValidatePlayerName(string i_Name, out string o_ErrorMsg)
        {
            o_ErrorMsg = string.Empty;
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(i_Name))
            {
                isValid = false;
                o_ErrorMsg = "Name cannot be empty or consist solely of whitespace.";
            }
            else if (i_Name.Length > 20)
            {
                isValid = false;
                o_ErrorMsg = "Name cannot exceed 20 characters.";
            }
            else if (i_Name.Contains(" "))
            {
                isValid = false;
                o_ErrorMsg = "Name cannot contain spaces.";
            }
            else if (!i_Name.All(char.IsLetter))
            {
                isValid = false;
                o_ErrorMsg = "Name must contain only letters.";
            }

            return isValid;
        }

        public static bool ValidateBoardSize(int i_BoardSize, out string o_ErrorMsg)
        {
            o_ErrorMsg = string.Empty;
            bool isValid = true;

            if (i_BoardSize != 6 && i_BoardSize != 8 && i_BoardSize != 10)
            {
                isValid = false;
                o_ErrorMsg = "Invalid board size. Allowed sizes are 6, 8, or 10.";
            }

            return isValid;
        }

        public static bool TryParseMove(string i_Input, int i_BoardSize, out Move o_Move, out string o_ErrorMsg)
        {
            o_Move = null;
            o_ErrorMsg = string.Empty;
            bool isSuccess = true;
            bool isValid = tryValidateAndSplitMove(i_Input, out string fromStr, out string toStr, out string moveErrorMsg);

            if (!isValid)
            {
                o_ErrorMsg = moveErrorMsg;
                isSuccess = false;
            }
            else
            {
                bool positionsAreValid = validatePositions(fromStr, toStr, i_BoardSize, out Position? fromPos, out Position? toPos, out string positionErrorMessage);

                if (!positionsAreValid || !fromPos.HasValue || !toPos.HasValue)
                {
                    o_ErrorMsg = positionErrorMessage;
                    isSuccess = false;
                }
                else
                {
                    o_Move = new Move(fromPos.Value, toPos.Value);
                }
            }

            return isSuccess;
        }

        private static bool tryValidateAndSplitMove(string i_Input, out string o_FromStr, out string o_ToStr, out string o_ErrorMsg)
        {
            o_FromStr = string.Empty;
            o_ToStr = string.Empty;
            o_ErrorMsg = string.Empty;
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(i_Input))
            {
                o_ErrorMsg = "The input cannot be empty.";
                isValid = false;
            }
            else
            {
                string[] parts = i_Input.Split('>');

                if (parts.Length != 2)
                {
                    o_ErrorMsg = "Invalid move format. Please use the format 'ROWcol>ROWcol'.";
                    isValid = false;
                }
                else
                {
                    o_FromStr = parts[0].Trim();
                    o_ToStr = parts[1].Trim();
                }
            }

            return isValid;
        }

        private static bool validatePositions(string i_FromStr, string i_ToStr, int i_BoardSize, out Position? io_FromPos, out Position? io_ToPos, out string o_ErrorMsg)
        {
            io_FromPos = null;
            io_ToPos = null;
            o_ErrorMsg = string.Empty;
            bool arePositionsValid = true;
            bool isFromValid = Position.TryParse(i_FromStr, i_BoardSize, out io_FromPos);
            bool isToValid = Position.TryParse(i_ToStr, i_BoardSize, out io_ToPos);

            if (!isFromValid && !isToValid)
            {
                arePositionsValid = false;
                o_ErrorMsg = $"Both source '{i_FromStr}' and destination '{i_ToStr}' positions are invalid.";
            }
            else if (!isFromValid)
            {
                arePositionsValid = false;
                o_ErrorMsg = $"Source position '{i_FromStr}' is invalid.";
            }
            else if (!isToValid)
            {
                arePositionsValid = false;
                o_ErrorMsg = $"Destination position '{i_ToStr}' is invalid.";
            }

            return arePositionsValid;
        }
    }
}
