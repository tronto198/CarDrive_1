using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using WinFormlib;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CarDrive_1
{
    class TimerExample
    {
        public void Timer()
        {
            var autoEvent = new AutoResetEvent(false);
            var statusChecker = new StatusChecker(10);

            var stateTimer = new Timer(statusChecker.CheckStatus, autoEvent, 1000, 250);

        }
    }
    class StatusChecker
    {
        private int invokeCount;
        private int maxCount;
        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }
        public void CheckStatus(object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            
        }
    }
    class Car
    {
        double accel = 0.5;
        double velocity = 0.0;
        double degree = 0;
        Threading_Timer timer = new Threading_Timer();
        Rectangle car = new Rectangle();
        DoubleBuffering d = null;

        Image img = Image.FromFile("car.png");
        Bitmap bitmap = null;
        Point center;

        /// <summary>
        /// 속도, 가속도 설정
        /// </summary>
        /// <param name="">
        /// 
        /// </param>
        public void Start()
        {
            bitmap = new Bitmap(img);
            car.Size = new Size(30, 20);
            car.X = 0;
            car.Y = 100;
            timer.setInterval(100);
            timer.setCallback(go);
            timer.Start();
            d = DoubleBuffering.getinstance();
            d.callback_work += Draw;
            //velocity = velocity_0 + accel * duration;


            Matrix mat = new Matrix();
            center = new Point();
        }
        
        public void go()
        {
            //위아래 = 가속도 +-
            //좌우 = 각도?

            Key_input key_Input = Key_input.getinstance();

            if (key_Input.get_up == true)
            {
                velocity += accel;
            }
            

            car.X += (int)velocity;

            //---> 
            //30도 속도 x = cos(30)*velocity y = sin(30)* v
        }

        public void Draw()
        {
            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush);

            d.getGraphics.DrawImage(bitmap, car);
            
        }

        
        //방향키 입력으로 위치이동
        public void Key(string[] args)
        {
            ConsoleKeyInfo cki;
            int x = 10, y = 10;
            while(true)
            {
                Console.Clear();
                Console.SetCursorPosition(x, y);
               
                cki = Console.ReadKey(true);
                switch(cki.Key)
                {
                    case ConsoleKey.LeftArrow:
                        x--;
                        break;
                    case ConsoleKey.RightArrow:
                        x++;
                        break;
                    case ConsoleKey.UpArrow:
                        y--;
                        break;
                    case ConsoleKey.DownArrow:
                        y++;
                        break;
                    case ConsoleKey.Q:
                        return;
                   
                       
                }
            }
        }

    }
}
