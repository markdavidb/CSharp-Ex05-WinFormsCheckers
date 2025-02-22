namespace CheckersWinApp.Logic
{
    public struct Position
    {
        private readonly int r_Row;
        private readonly int r_Column;

        public int Row
        {
            get
            {
                return r_Row;
            }
        }

        public int Column
        {
            get
            {
                return r_Column;
            }
        }

        public Position(int i_Row, int i_Column)
        {
            r_Row = i_Row;
            r_Column = i_Column;
        }

        public static bool TryParse(string i_Input, int i_BoardSize, out Position? o_Position)
        {
            o_Position = null;
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(i_Input) || i_Input.Length != 2)
            {
                isValid = false;
            }
            else
            {
                char rowChar = i_Input[0];
                char colChar = i_Input[1];

                if (!char.IsUpper(rowChar) || !char.IsLower(colChar))
                {
                    isValid = false;
                }
                else
                {
                    int row = rowChar - 'A';
                    int col = colChar - 'a';

                    if (col < 0 || col >= i_BoardSize || row < 0 || row >= i_BoardSize)
                    {
                        isValid = false;
                    }
                    else
                    {
                        o_Position = new Position(row, col);
                    }
                }
            }

            return isValid;
        }

        public string GetPositionAsString()
        {
            char rowChar = (char)('A' + Row);
            char colChar = (char)('a' + Column);

            return $"{rowChar}{colChar}";
        }
    }
}