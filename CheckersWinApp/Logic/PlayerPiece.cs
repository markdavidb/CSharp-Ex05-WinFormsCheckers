namespace CheckersWinApp.Logic
{
    public class PlayerPiece
    {
        private int m_Row;
        private int m_Col;
        private ePieceType m_PieceType;

        public PlayerPiece(int i_Row, int i_Col, ePieceType i_PieceType)
        {
            m_Row = i_Row;
            m_Col = i_Col;
            m_PieceType = i_PieceType;
        }

        public int Row
        {
            get
            {
                return m_Row;
            }
            set
            {
                m_Row = value;
            }
        }

        public int Col
        {
            get
            {
                return m_Col;
            }
            set
            {
                m_Col = value;
            }
        }

        public ePieceType PieceType
        {
            get
            {
                return m_PieceType;
            }
            set
            {
                m_PieceType = value;
            }
        }
    }
}
