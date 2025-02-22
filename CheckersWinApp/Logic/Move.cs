using System;

namespace CheckersWinApp.Logic
{
    public class Move
    {
        private readonly Position r_From;
        private readonly Position r_To;

        public Position From
        {
            get
            {
                return r_From;
            }
        }

        public Position To
        {
            get
            {
                return r_To;
            }
        }

        public Move(Position i_From, Position i_To)
        {
            r_From = i_From;
            r_To = i_To;
        }

        public bool IsJump()
        {
            return (Math.Abs(To.Row - From.Row) == 2 && Math.Abs(To.Column - From.Column) == 2);
        }

        public string GetMoveAsString()
        {
            return $"{From.GetPositionAsString()}>{To.GetPositionAsString()}";
        }
    }
}