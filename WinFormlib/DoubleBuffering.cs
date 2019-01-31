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

        //컴퓨터 내부적으로 미리 화면을 그려놓고(getGraphics로 가져온 그래픽 인스턴스에 미리 그려놓음)
        //진짜 화면에 그릴때 이를 한꺼번에 그려서 속도를 빠르게함
        //(getBuffered에 저장되어 있는 미리 그려놓은 그래픽을 진짜 화면에 그림)

        /// <summary>
        /// 인스턴스의 그래픽을 가져옵니다
        /// </summary>
        public Graphics getGraphics { get { return g.Graphics; } }
        public BufferedGraphics getBuffered { get { return g; } }

        /// <summary>
        /// callback_work에 메서드를 연결
        /// </summary>
        public delegate void ClearEventHandler();
        public ClearEventHandler callback_work = null;

        //인스턴스 하나만으로 고정
        private DoubleBuffering()
        {
            //BufferedGraphics graphics
            //g = graphics;
        }

        /// <summary>
        /// 윈폼에 맞게 인스턴스 설정
        /// </summary>
        /// <param name="form">이 더블버퍼링을 사용할 윈폼 지정</param>
        public void setInstance(System.Windows.Forms.Form form)
        {
            Graphics gg = form.CreateGraphics();
            g = BufferedGraphicsManager.Current.Allocate(gg, form.ClientRectangle);
            gg.Dispose();

            //렌더링(화면에 진짜로 그리기)
            void Render()
            {
                try
                {
                    
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

                    Work();
                }
                catch (Exception e)
                {

                }
            }

            //여기서부터 타이머 설정
            //8ms마다 Render라는 함수 실행
            Threading_Timer thread_FrameRender = new Threading_Timer();
            thread_FrameRender.setCallback(new Action(delegate () {
                //callback_Draw();
                Render();
            }));
            thread_FrameRender.setInterval(8);
            thread_FrameRender.Start();
            
        }

        /// <summary>
        /// 더블 버퍼링의 인스턴스를 가져옴, 없으면 새로 생성
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 여기서 callback_work 이벤트 발생, 이 이벤트에 연결된 그리기 메소드들 실행
        /// </summary>
        public void Work()
        {
            if(callback_work != null)
            {
                callback_work();
            }
        }
    }
}
