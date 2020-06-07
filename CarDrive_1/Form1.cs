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
        WinFormlib.DoubleBuffering Screen = null;
        WinFormlib.Timer_State Timer_State = null;
        WinFormlib.Form_input Form_input = null;
        Pen thispen = new Pen(new SolidBrush(Color.Black));

        MainProgram Main_Program = null;

        public Form1()
        {
            InitializeComponent();
            
            FirstSetting();

            Main_Program = new MainProgram(this);
                                    

            //ShowHelp();

        }

        void FirstSetting()
        {
            Timer_State = WinFormlib.Timer_State.getinstance(this);
            Screen = WinFormlib.DoubleBuffering.getinstance();
            Screen.setInstance(this);
            Form_input = new WinFormlib.Form_input();
            Form_input.binding(this);

            Screen.callback_work += GraphicClearEvent;
            

        }


        public MainProgram getMainProgram()
        {
            return Main_Program;
        }


        void GraphicClearEvent()
        {
            //여기는 화면에 그려진 것들을 지우는 곳
            //이 함수가 실행된 후에 그려야 제대로 화면에 나옴
            Screen.getGraphics.Clear(Color.LightBlue);
        }

        public void Ringing(string str)
        {
            this.Invoke(new Action(delegate ()
            {
                MessageBox.Show(str);

            }));
        }
        




        int testint = 1;

        void ShowHelp()
        {
            //메세지 박스 사용 가능 (mbox 치고 탭탭 하면 자동, 위에 using System.Windows.Form 해야 가능)
            MessageBox.Show("시작했어요!!");
            
            //이런식으로 그리기 함수 연결 가능
            Screen.callback_work += ShowDraw;

            //이런식으로 스레드형 타이머 설정 가능 (위에 using WinFormlib 하면 앞에꺼 띄고 가능)
            WinFormlib.Threading_Timer test_timer = new WinFormlib.Threading_Timer();
            test_timer.setInterval(100);                    //타이머의 실행 간격 (100ms)
            test_timer.setCallback(ShowTimercallback);      //타이머가 실행시키는 함수
            test_timer.Start();                             //타이머 시작
            //0.1초 간격으로 timercallback 함수 실행
        }
        
        void ShowDraw()
        {
            //이런식으로 다양한 Draw가능
            Font thisfont = new Font("AR CENA", 10);
            Brush thisBrush = new SolidBrush(Color.Black);
            
            Screen.getGraphics.DrawEllipse(new Pen(thisBrush), new Rectangle(50, 50, 400, 250));
            Screen.getGraphics.DrawString(testint + "", thisfont, thisBrush, 50, 50);
        }

        void ShowTimercallback()
        {
            if (testint++ % 10000 == 0)
                testint -= 5000;
        }
        public void link(object obj)
        {
            Action a = obj as Action;
            this.FormClosed += delegate(object sender, FormClosedEventArgs e) { a(); };
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main_Program.Formclose();
        }
        
    }
}
