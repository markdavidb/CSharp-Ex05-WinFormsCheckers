namespace CheckersWinApp.Logic
{
    public class Board
    {
        private readonly int r_Size;
        private readonly ePieceType[,] r_Cells;

        public int Size
        {
            get
            {
                return r_Size;
            }
        }

        public ePieceType[,] Cells
        {
            get
            {
                return r_Cells;
            }
        }

        public Board(int i_Size)
        {
            r_Size = i_Size;
            r_Cells = new ePieceType[i_Size, i_Size];
            InitializeBoard(i_Size);
        }
        
        public void InitializeBoard(int i_Size)
        {
            int fillRows = 0;

            for (int row = 0; row < r_Size; row++)
            {
                for (int col = 0; col < r_Size; col++)
                {
                    r_Cells[row, col] = ePieceType.None;
                }
            }

            if (i_Size == 6)
            {
                fillRows = 2;
            }
            else if (i_Size == 8)
            {
                fillRows = 3;
            }
            else if (i_Size == 10)
            {
                fillRows = 4;
            }

            for (int row = 0; row < fillRows; row++)
            {
                for (int col = 0; col < r_Size; col++)
                {
                    if ((row + col) % 2 == 1)
                    {
                        r_Cells[row, col] = ePieceType.O;
                    }
                }
            }

            for (int row = r_Size - fillRows; row < r_Size; row++)
            {
                for (int col = 0; col < r_Size; col++)
                {
                    if ((row + col) % 2 == 1)
                    {
                        r_Cells[row, col] = ePieceType.X;
                    }
                }
            }
        }

        public bool IsInRange(int i_Row, int i_Col)
        {
            return (i_Row >= 0 && i_Row < r_Size && i_Col >= 0 && i_Col < r_Size);
        }

        public ePieceType GetPieceAt(int i_Row, int i_Col)
        {
            return (IsInRange(i_Row, i_Col) ? r_Cells[i_Row, i_Col] : ePieceType.None);
        }

        public void SetPieceAt(int i_Row, int i_Col, ePieceType i_Piece)
        {
            if (IsInRange(i_Row, i_Col))
            {
                r_Cells[i_Row, i_Col] = i_Piece;
            }
        }
    }
}
