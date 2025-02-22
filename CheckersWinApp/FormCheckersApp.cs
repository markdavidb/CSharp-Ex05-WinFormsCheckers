using System;
using System.Drawing;
using System.Windows.Forms;
using CheckersWinApp.Logic;
using CheckersWinApp.Properties;

namespace CheckersWinApp
{
    public class FormCheckersApp : Form
    {
        private readonly CheckersLogic r_Logic;
        private Button[,] m_BoardButtons;
        private Button m_SelectedButton;
        private Button m_CapturedButtonForAnimation;
        private Label m_LabelPlayer1Score;
        private Label m_LabelPlayer2Score;
        private Label m_CurrentPlayer;
        private Timer m_ComputerTimer;
        private readonly Image r_BlackPieceImage = Resource.blackpiece;
        private readonly Image r_WhitePieceImage = Resource.whitepiece;
        private readonly Image r_BlackKingPieceImage = Resource.blackkingpiece;
        private readonly Image r_WhiteKingPieceImage = Resource.whitekingpiece;

        public FormCheckersApp(CheckersLogic i_Logic)
        {
            r_Logic = i_Logic;
            initializeForm();
            buildBoardUi();
            updateScores();
        }

        private void initializeForm()
        {
            this.Text = "Damka";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormClosing += formCheckers_FormClosing;
        }

        private void buildBoardUi()
        {
            int boardSize = r_Logic.BoardSize;
            int buttonSize = 55;
            int startX = 15;
            int startY = 40;

            m_BoardButtons = new Button[boardSize, boardSize];

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    Button button = new Button
                    {
                        Size = new Size(buttonSize, buttonSize),
                        Location = new Point(startX + col * buttonSize, startY + row * buttonSize),
                        FlatStyle = FlatStyle.Flat,
                        Tag = new Point(row, col)
                    };

                    button.FlatAppearance.BorderSize = 0;
                    button.Enabled = ((row + col) % 2 != 0);
                    updateButtonAppearance(button, row, col);
                    button.Click += boardButton_Click;
                    this.Controls.Add(button);
                    m_BoardButtons[row, col] = button;
                }
            }

            int formWidth = startX + (boardSize * buttonSize) + 15;
            int formHeight = startY + (boardSize * buttonSize) + 50;

            this.ClientSize = new Size(formWidth, formHeight);
            m_LabelPlayer1Score = new Label
            {
                Text = $"{r_Logic.Player1.Name}: {r_Logic.Player1.Score}",
                Font = new Font("Arial", 9, FontStyle.Bold),
                AutoSize = true,
            };
            m_LabelPlayer2Score = new Label
            {
                Text = $"{r_Logic.Player2.Name}: {r_Logic.Player2.Score}",
                Font = new Font("Arial", 9, FontStyle.Bold),
                AutoSize = true,
            };
            m_CurrentPlayer = new Label
            {
                Text = $"Current Player: {r_Logic.CurrentPlayer.Name}",
                Font = new Font("Arial", 9, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(m_LabelPlayer1Score);
            this.Controls.Add(m_LabelPlayer2Score);
            this.Controls.Add(m_CurrentPlayer);

            int label1X = startX + 20;
            int label1Y = startY - m_LabelPlayer1Score.Height - 10;

            m_LabelPlayer1Score.Location = new Point(label1X, label1Y);

            int lastButtonX = startX + (boardSize - 1) * buttonSize;
            int label2X = lastButtonX + buttonSize - m_LabelPlayer2Score.Width - 20;

            m_LabelPlayer2Score.Location = new Point(label2X, label1Y);

            int currentPlayerX = (this.ClientSize.Width - m_CurrentPlayer.Width) / 2;
            int currentPlayerY = this.ClientSize.Height - 10 - m_CurrentPlayer.Height;

            m_CurrentPlayer.Location = new Point(currentPlayerX, currentPlayerY);
        }

        private Image pieceToImage(ePieceType i_Piece)
        {
            Image result = null;

            switch (i_Piece)
            {
                case ePieceType.X:
                    result = r_BlackPieceImage;
                    break;
                case ePieceType.O:
                    result = r_WhitePieceImage;
                    break;
                case ePieceType.K:
                    result = r_BlackKingPieceImage;
                    break;
                case ePieceType.U:
                    result = r_WhiteKingPieceImage;
                    break;
            }

            return result;
        }

        private void boardButton_Click(object sender, EventArgs e)
        {
            if (!r_Logic.CurrentPlayer.IsComputer)
            {
                Button clickedButton = sender as Button;

                if (clickedButton != null)
                {
                    Point position = (Point)clickedButton.Tag;

                    if (m_SelectedButton == null)
                    {
                        if (isCurrentPlayersPiece(position))
                        {
                            selectPiece(clickedButton);
                        }
                    }
                    else
                    {
                        if (m_SelectedButton == clickedButton)
                        {
                            deselectPiece(clickedButton);
                        }
                        else
                        {
                            performPlayerMove(m_SelectedButton, clickedButton);
                            deselectPiece(m_SelectedButton);
                        }
                    }
                }
            }
        }

        private void selectPiece(Button i_Button)
        {
            m_SelectedButton = i_Button;
            i_Button.BackColor = Color.LightBlue;
        }

        private void deselectPiece(Button i_Button)
        {
            Point pos = (Point)i_Button.Tag;

            updateButtonAppearance(i_Button, pos.X, pos.Y);
            m_SelectedButton = null;
        }

        private void performPlayerMove(Button i_FromButton, Button i_ToButton)
        {
            Point fromPos = (Point)i_FromButton.Tag;
            Point toPos = (Point)i_ToButton.Tag;
            Position from = new Position(fromPos.X, fromPos.Y);
            Position to = new Position(toPos.X, toPos.Y);
            Move move = new Move(from, to);
            bool isCapture = Math.Abs(to.Row - from.Row) == 2;

            performMoveWithAnimation(move, isCapture);
        }

        private bool isCurrentPlayersPiece(Point i_Position)
        {
            int row = i_Position.X;
            int col = i_Position.Y;
            ePieceType piece = r_Logic.GetPieceAt(row, col);

            return piece == r_Logic.CurrentPlayer.RegularPiece || piece == r_Logic.CurrentPlayer.KingPiece;
        }

        private void refreshBoardUi()
        {
            int boardSize = r_Logic.BoardSize;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    updateButtonAppearance(m_BoardButtons[row, col], row, col);
                }
            }

            m_CurrentPlayer.Text = $"Current Player: {r_Logic.CurrentPlayer.Name}";
        }

        private void doComputerMoveIfNeeded()
        {
            if (r_Logic.CurrentPlayer.IsComputer)
            {
                if (m_ComputerTimer == null)
                {
                    m_ComputerTimer = new Timer();
                    m_ComputerTimer.Interval = 600;
                    m_ComputerTimer.Tick += computerTimer_Tick;
                }

                m_ComputerTimer.Start();
            }
        }

        private void computerTimer_Tick(object sender, EventArgs e)
        {
            m_ComputerTimer.Stop();

            if (r_Logic.GameStatus == eGameStatus.InProgress && r_Logic.CurrentPlayer.IsComputer)
            {
                bool isMoveSucceeded = r_Logic.MakeComputerMove(out Move chosenMove, out string errorMsg);

                if (isMoveSucceeded)
                {
                    bool isCapture = Math.Abs(chosenMove.To.Row - chosenMove.From.Row) == 2;

                    performMoveWithAnimation(chosenMove, isCapture, true);
                }
            }
        }

        private void performMoveWithAnimation(Move i_Move, bool i_IsCapture, bool i_MoveAlreadyDone = false)
        {
            bool isCanProceed = true;

            if (!i_MoveAlreadyDone)
            {
                bool isMoveSucceeded = r_Logic.MakeMove(i_Move, out string errorMsg);

                if (!isMoveSucceeded)
                {
                    MessageBox.Show(errorMsg, "Illegal Move", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isCanProceed = false;
                }
            }

            if (isCanProceed)
            {
                if (i_IsCapture)
                {
                    int midRow = (i_Move.From.Row + i_Move.To.Row) / 2;
                    int midCol = (i_Move.From.Column + i_Move.To.Column) / 2;
                    Button capturedButton = m_BoardButtons[midRow, midCol];
                    Image capturedImage = capturedButton.Image;

                    refreshBoardUi();
                    capturedButton.Image = capturedImage;
                    m_CapturedButtonForAnimation = capturedButton;

                    FadeOutAnimator animator = new FadeOutAnimator(capturedButton);

                    animator.AnimationCompleted += animator_AnimationCompleted;
                    animator.Start();
                }
                else
                {
                    refreshBoardUi();
                    checkGameStatus();

                    if (r_Logic.GameStatus == eGameStatus.InProgress)
                    {
                        doComputerMoveIfNeeded();
                    }
                }
            }
        }

        private void animator_AnimationCompleted()
        {
            if (m_CapturedButtonForAnimation != null)
            {
                m_CapturedButtonForAnimation.Image = null;
                refreshBoardUi();
                checkGameStatus();

                if (r_Logic.GameStatus == eGameStatus.InProgress)
                {
                    doComputerMoveIfNeeded();
                }

                m_CapturedButtonForAnimation = null;
            }
        }

        private void checkGameStatus()
        {
            if (r_Logic.GameStatus != eGameStatus.InProgress)
            {
                if (r_Logic.GameStatus == eGameStatus.Tie)
                {
                    handleEndOfGame("Tie! Another Round?");
                }
                else
                {
                    string winnerName = r_Logic.Winner == r_Logic.Player1 ? r_Logic.Player1.Name : r_Logic.Player2.Name;

                    handleEndOfGame($"{winnerName} Won! Another Round?");
                }
            }
        }

        private void handleEndOfGame(string i_Message)
        {
            bool isTie = (r_Logic.GameStatus == eGameStatus.Tie);

            if (!isTie)
            {
                Player winner = r_Logic.Winner;
                Player loser = (winner == r_Logic.Player1) ? r_Logic.Player2 : r_Logic.Player1;
                int winnerPoints = r_Logic.ComputePoints(winner);
                int loserPoints = r_Logic.ComputePoints(loser);
                int difference = Math.Abs(winnerPoints - loserPoints);
                winner.Score += difference;
            }

            DialogResult dialogResult = MessageBox.Show(i_Message, "Damka", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                r_Logic.Reset(r_Logic.BoardSize);
                refreshBoardUi();
                updateScores();
            }
            else
            {
                this.Close();
            }
        }

        private void formCheckers_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit?", "Quit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                if (r_Logic.GameStatus == eGameStatus.InProgress)
                {
                    r_Logic.QuitGame();
                    Player opponent = (r_Logic.CurrentPlayer == r_Logic.Player1) ? r_Logic.Player2 : r_Logic.Player1;

                    MessageBox.Show($"You lost, and {opponent.Name} won!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void updateScores()
        {
            m_LabelPlayer1Score.Text = $"{r_Logic.Player1.Name}: {r_Logic.Player1.Score}";
            m_LabelPlayer2Score.Text = $"{r_Logic.Player2.Name}: {r_Logic.Player2.Score}";
        }

        private Color getCellBackgroundColor(int i_Row, int i_Col)
        {
            return ((i_Row + i_Col) % 2 == 0) ? Color.LightGray : Color.White;
        }

        private void updateButtonContent(Button i_Button, int i_Row, int i_Col)
        {
            ePieceType piece = r_Logic.GetPieceAt(i_Row, i_Col);
            i_Button.Image = pieceToImage(piece);
            i_Button.Text = string.Empty;
            i_Button.ImageAlign = ContentAlignment.MiddleCenter;
        }

        private void updateButtonAppearance(Button i_Button, int i_Row, int i_Col)
        {
            updateButtonContent(i_Button, i_Row, i_Col);
            i_Button.BackColor = getCellBackgroundColor(i_Row, i_Col);
        }
    }
}
