using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CheckersWinApp
{
    public class FormGameSettings : Form
    {
        private RadioButton m_RadioButton6X6;
        private RadioButton m_RadioButton8X8;
        private RadioButton m_RadioButton10X10;
        private Label m_LabelPlayer1;
        private Label m_LabelBoardSize;
        private Label m_LabelPlayers;
        private TextBox m_TextBoxPlayer1;
        private CheckBox m_CheckBoxPlayer2;
        private TextBox m_TextBoxPlayer2;
        private Button m_ButtonDone;
        private int m_BoardSize = 6;
        private string m_Player1Name = "Player 1";
        private string m_Player2Name = "Computer";
        private bool m_IsTwoPlayers = false;

        public int BoardSize
        {
            get
            {
                return m_BoardSize;
            }
            private set
            {
                m_BoardSize = value;
            }
        }

        public string Player1Name
        {
            get
            {
                return m_Player1Name;
            }
            private set
            {
                m_Player1Name = value;
            }
        }

        public string Player2Name
        {
            get
            {
                return m_Player2Name;
            }
            private set
            {
                m_Player2Name = value;
            }
        }

        public bool IsTwoPlayers
        {
            get
            {
                return m_IsTwoPlayers;
            }
            private set
            {
                m_IsTwoPlayers = value;
            }
        }

        public FormGameSettings()
        {
            initializeComponents();
        }

        private void initializeComponents()
        {
            this.Text = "Game Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(250, 230);

            m_LabelBoardSize = new Label { Text = "Board Size:", Location = new Point(20, 10), Width = 80 };
            m_LabelPlayers = new Label { Text = "Players:", Location = new Point(m_LabelBoardSize.Location.X, 60), Width = 80 };
            m_LabelPlayer1 = new Label { Text = "Player 1:", Location = new Point(m_LabelPlayers.Location.X + 10, m_LabelPlayers.Location.Y + 30), Width = 50 };
            m_RadioButton6X6 = new RadioButton { Text = "6 x 6", Location = new Point(40, 30), Checked = true, AutoSize = false, Width = 50 };
            m_RadioButton8X8 = new RadioButton { Text = "8 x 8", Location = new Point(m_RadioButton6X6.Right + 5, 30), AutoSize = false, Width = 50 };
            m_RadioButton10X10 = new RadioButton { Text = "10 x 10", Location = new Point(m_RadioButton8X8.Right + 5, 30), AutoSize = false, Width = 60 };
            m_TextBoxPlayer1 = new TextBox { Location = new Point(m_LabelPlayer1.Location.X + 90, m_LabelPlayer1.Location.Y - 3), Text = "Player1" };
            m_CheckBoxPlayer2 = new CheckBox { Text = "Player 2:", Location = new Point(m_LabelPlayer1.Location.X + 3, 118), Checked = false, Width = 80 };
            m_CheckBoxPlayer2.CheckedChanged += checkBoxPlayer2_CheckedChanged;
            m_TextBoxPlayer2 = new TextBox { Location = new Point(m_TextBoxPlayer1.Location.X, m_CheckBoxPlayer2.Location.Y), Text = "[Computer]", Enabled = false };
            m_ButtonDone = new Button { Text = "Done", Location = new Point(100, 150), Width = 70 };
            m_ButtonDone.Click += buttonDone_Click;

            this.Controls.AddRange(
                new Control[]
                {
            m_LabelBoardSize, m_RadioButton6X6, m_RadioButton8X8, m_RadioButton10X10,
            m_LabelPlayers, m_LabelPlayer1, m_TextBoxPlayer1, m_CheckBoxPlayer2, m_TextBoxPlayer2, m_ButtonDone
                });
        }

        private void checkBoxPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            m_TextBoxPlayer2.Enabled = m_CheckBoxPlayer2.Checked;

            if (!m_CheckBoxPlayer2.Checked)
            {
                m_TextBoxPlayer2.Text = "[Computer]";
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (m_RadioButton6X6.Checked)
            {
                BoardSize = 6;
            }
            else if (m_RadioButton8X8.Checked)
            {
                BoardSize = 8;
            }
            else
            {
                BoardSize = 10;
            }

            IsTwoPlayers = m_CheckBoxPlayer2.Checked;
            Player1Name = m_TextBoxPlayer1.Text.Trim();
            Player2Name = IsTwoPlayers ? m_TextBoxPlayer2.Text.Trim() : "Computer";

            bool isValid = true;
            StringBuilder errorMessage = new StringBuilder();

            if (string.IsNullOrWhiteSpace(Player1Name))
            {
                errorMessage.AppendLine("Player 1 name cannot be empty.");
                isValid = false;
            }

            if (IsTwoPlayers && string.IsNullOrWhiteSpace(Player2Name))
            {
                errorMessage.AppendLine("Player 2 name cannot be empty.");
                isValid = false;
            }

            if (!isValid)
            {
                MessageBox.Show(errorMessage.ToString(), "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}