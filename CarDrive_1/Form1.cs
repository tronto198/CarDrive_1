using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CarDrive_1
{
    public partial class Form1 : Form
    {
        WinFormlib.DoubleBuffering DoubleBuffering = null;
        WinFormlib.Timer_State Timer_State = null;
        WinFormlib.Form_input Form_input = null;
        Pen thispen = new Pen(new SolidBrush(Color.Black));

        int testint = 1;

        public Form1()
        {
            InitializeComponent();
            
            Setting();

            makeCar();

            test();
        }

        public void Setting()
        {
            Timer_State = WinFormlib.Timer_State.getinstance(this);
            DoubleBuffering = WinFormlib.DoubleBuffering.getinstance();
            DoubleBuffering.setInstance(this);
            Form_input = new WinFormlib.Form_input();
            Form_input.binding(this);
            MessageBox.Show("시작했어요!!");
            DoubleBuffering.callback_work += GraphicEvent;
            
            //이런식으로 그리기 함수 연결 가능
            DoubleBuffering.callback_work += testevent;

            //이런식으로 스레드형 타이머 설정 가능
            WinFormlib.Threading_Timer test_timer = new WinFormlib.Threading_Timer();
            test_timer.interval = 100; 
            test_timer.setCallback(timercallback);
            test_timer.Start();
            //0.1초 간격으로 timercallback 함수 실행




        }

        public void makeCar()
        {
            Car car = new Car();
            car.Start();
        }

        public void test()
        {
            void drawing()
            {
                DoubleBuffering.getGraphics.DrawArc(thispen, new Rectangle(100, 100, 500, 500), 180, 180);
            }

            DoubleBuffering.callback_work += drawing;
        }


        public void GraphicEvent()
        {
            //여기는 화면에 그려진 것들을 지우는 곳
            //이 함수가 실행된 후에 그려야 제대로 화면에 나옴
            DoubleBuffering.getGraphics.Clear(Color.LightBlue);
        }

        public void testevent()
        {
            //이런식으로 다양한 Draw가능
            Font thisfont = new Font("AR CENA", 10);
            Brush thisBrush = new SolidBrush(Color.Black);
            
            DoubleBuffering.getGraphics.DrawEllipse(new Pen(thisBrush), new Rectangle(50, 50, 400, 250));
            DoubleBuffering.getGraphics.DrawString(testint + "", thisfont, thisBrush, 50, 50);
        }

        public void timercallback()
        {
            if (testint++ % 10000 == 0)
                testint -= 5000;
        }
    }
}
