using System.Windows.Forms;
using CheckersWinApp.Logic;

namespace CheckersWinApp
{
    public class CheckersApplication
    {
        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormGameSettings formSettings = new FormGameSettings();
            DialogResult dialogResult = formSettings.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                CheckersLogic checkersLogic = new CheckersLogic(formSettings.BoardSize, formSettings.Player1Name, formSettings.Player2Name, !formSettings.IsTwoPlayers);
                FormCheckersApp formCheckers = new FormCheckersApp(checkersLogic);

                Application.Run(formCheckers);
            }
        }
    }
}