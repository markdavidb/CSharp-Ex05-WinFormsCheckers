using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CheckersWinApp
{
    public class FadeOutAnimator
    {
        private float m_CurrentOpacity;
        private readonly Button r_Button;
        private readonly Timer r_Timer;
        private readonly float r_OpacityStep;
        private readonly Image r_OriginalImage;

        public event Action AnimationCompleted;

        public FadeOutAnimator(Button i_Button, int i_Duration = 160, int i_Steps = 10)
        {
            r_Button = i_Button;
            r_OriginalImage = i_Button.Image;
            r_OpacityStep = 1.0f / i_Steps;
            m_CurrentOpacity = 1.0f;
            r_Timer = new Timer();
            r_Timer.Interval = i_Duration / i_Steps;
            r_Timer.Tick += timer_Tick;
        }

        public void Start()
        {
            if (r_Button.Image != null)
            {
                r_Timer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            m_CurrentOpacity -= r_OpacityStep;

            if (m_CurrentOpacity <= 0)
            {
                r_Timer.Stop();
                r_Button.Image = null;
                AnimationCompleted?.Invoke();
            }
            else
            {
                r_Button.Image = setImageOpacity(r_OriginalImage, m_CurrentOpacity);
            }
        }

        private Image setImageOpacity(Image i_Image, float i_Opacity)
        {
            Bitmap resultBitmap = new Bitmap(i_Image.Width, i_Image.Height);
            Graphics graphics = Graphics.FromImage(resultBitmap);

            try
            {
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix33 = i_Opacity;
                ImageAttributes imageAttributes = new ImageAttributes();

                try
                {
                    imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    graphics.DrawImage(i_Image, new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                        0, 0, i_Image.Width, i_Image.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                finally
                {
                    imageAttributes.Dispose();
                }
            }
            finally
            {
                graphics.Dispose();
            }

            return resultBitmap;
        }
    }
}