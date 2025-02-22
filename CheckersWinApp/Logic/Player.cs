using System.Collections.Generic;

namespace CheckersWinApp.Logic
{
    public class Player
    {
        private int m_Score;
        private readonly string r_Name;
        private readonly bool r_IsComputer;
        private readonly ePieceType r_RegularPiece;
        private readonly ePieceType r_KingPiece;
        private readonly ePlayerSide r_Side;
        private List<PlayerPiece> m_Pieces;

        public string Name
        {
            get
            {
                return r_Name;
            }
        }

        public int Score
        {
            get
            {
                return m_Score;
            }
            set
            {
                if (value >= 0)
                {
                    m_Score = value;
                }
                else
                {
                    m_Score = 0;
                }
            }
        }

        public bool IsComputer
        {
            get
            {
                return r_IsComputer;
            }
        }

        public ePieceType RegularPiece
        {
            get
            {
                return r_RegularPiece;
            }
        }

        public ePieceType KingPiece
        {
            get
            {
                return r_KingPiece;
            }
        }

        public ePlayerSide Side
        {
            get
            {
                return r_Side;
            }
        }

        public List<PlayerPiece> Pieces
        {
            get
            {
                return m_Pieces;
            }
            set
            {
                m_Pieces = value;
            }
        }

        public Player(ePlayerSide i_Side, string i_Name, bool i_IsComputer = false)
        {
            r_Side = i_Side;
            r_Name = i_Name;
            r_IsComputer = i_IsComputer;
            m_Score = 0;
            m_Pieces = new List<PlayerPiece>();

            if (r_Side == ePlayerSide.Top)
            {
                r_RegularPiece = ePieceType.O;
                r_KingPiece = ePieceType.U;
            }
            else
            {
                r_RegularPiece = ePieceType.X;
                r_KingPiece = ePieceType.K;
            }
        }

        public void AddPiece(int i_Row, int i_Column, ePieceType i_Type)
        {
            PlayerPiece newPiece = new PlayerPiece(i_Row, i_Column, i_Type);

            m_Pieces.Add(newPiece);
        }

        public void RemovePiece(int i_Row, int i_Column)
        {
            PlayerPiece pieceToRemove = FindPiece(i_Row, i_Column);

            if (pieceToRemove != null)
            {
                m_Pieces.Remove(pieceToRemove);
            }
        }

        public void UpdatePiecePosition(int i_OldRow, int i_OldColumn, int i_NewRow, int i_NewColumn)
        {
            PlayerPiece pieceToUpdate = FindPiece(i_OldRow, i_OldColumn);

            if (pieceToUpdate != null)
            {
                pieceToUpdate.Row = i_NewRow;
                pieceToUpdate.Col = i_NewColumn;
            }
        }

        public void UpdatePieceType(int i_Row, int i_Column, ePieceType i_NewType)
        {
            PlayerPiece pieceToUpdate = FindPiece(i_Row, i_Column);

            if (pieceToUpdate != null)
            {
                pieceToUpdate.PieceType = i_NewType;
            }
        }

        public void ClearPieces()
        {
            m_Pieces.Clear();
        }

        public PlayerPiece FindPiece(int i_Row, int i_Column)
        {
            PlayerPiece foundPiece = null;

            foreach (PlayerPiece currentPiece in m_Pieces)
            {
                if (currentPiece.Row == i_Row && currentPiece.Col == i_Column)
                {
                    foundPiece = currentPiece;
                    break;
                }
            }

            return foundPiece;
        }
    }
}
