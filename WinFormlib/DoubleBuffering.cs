using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Paper_io_Client
{
    class DoubleBuffering
    {
        public static DoubleBuffering oInstance = null;
        private BufferedGraphics g;

        public Graphics getGraphics { get { return g.Graphics; } }
        public BufferedGraphics getBuffered { get { return g; } }
        public delegate void ClearEventHandler();
        public static ClearEventHandler callback_work = null;

        private DoubleBuffering(BufferedGraphics graphics)
        {
            g = graphics;
        }

        public static DoubleBuffering Instance(BufferedGraphics graphics)
        {
            if (oInstance == null)
            {
                oInstance = new DoubleBuffering(graphics);
            }
            else
            {
                oInstance = null;
                oInstance = new DoubleBuffering(graphics);
            }
            return oInstance;
        }

        public static DoubleBuffering Instance()
        {
            try
            {
                if (oInstance == null)
                {
                    throw (new Exception("instance 선언이 되지 않았습니다."));
                }
                return oInstance;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public static void Work()
        {
            if(callback_work != null)
            {
                callback_work();
            }
        }
    }
}
