using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WinFormlib
{
    public class DoubleBuffering
    {
        private static DoubleBuffering oInstance = null;
        private BufferedGraphics g;

        public Graphics getGraphics { get { return g.Graphics; } }
        public BufferedGraphics getBuffered { get { return g; } }
        public delegate void ClearEventHandler();
        public ClearEventHandler callback_work = null;

        private DoubleBuffering()
        {
            //BufferedGraphics graphics
            //g = graphics;
        }

        public void setInstance(System.Windows.Forms.Form form)
        {
            Graphics gg = form.CreateGraphics();
            g = BufferedGraphicsManager.Current.Allocate(gg, form.ClientRectangle);
            gg.Dispose();

            void Render()
            {
                try
                {
                    Work();

                    form.Invoke(new Action(delegate ()
                    {
                        try
                        {
                            Graphics g = form.CreateGraphics();
                            DoubleBuffering.getinstance().getBuffered.Render(g);
                            g.Dispose();
                        }
                        catch (Exception e)
                        {

                        }
                    }));

                }
                catch (Exception e)
                {

                }
            }

            Threading_Timer thread_FrameRender = new Threading_Timer();
            thread_FrameRender.setCallback(new Action(delegate () {
                //callback_Draw();
                Render();
            }));
            thread_FrameRender.setInterval(8);
            thread_FrameRender.Start();
            
        }

        public static DoubleBuffering getinstance()
        {
            try
            {
                if (oInstance == null)
                {
                    oInstance = new DoubleBuffering();
                }
                return oInstance;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void Work()
        {
            if(callback_work != null)
            {
                callback_work();
            }
        }
    }
}
