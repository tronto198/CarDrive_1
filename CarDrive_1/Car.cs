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
        double accel = 0.15;
        double velocity = 0.0;
        double degree = 90;
        double turn = 3;
        //Threading_Timer timer = new Threading_Timer();
        //Rectangle car = new Rectangle();
        int x = 0, y = 100;
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

            //timer.setinterval(100);
            //timer.setcallback(go);
            //timer.start();
            d = DoubleBuffering.getinstance();
            d.callback_work += Draw;
            //velocity = velocity_0 + accel * duration;


            center = new Point((int)(img.Width / 2),(int)(img.Height / 2));

            bitmap = new Bitmap(img);
        }
        
        public void go()
        {
            int v_x = 0;
            int v_y = 0;

            v_y = (int)(Math.Cos(degree) * velocity);
            v_x = (int)(Math.Sin(degree) * velocity);

            x += v_x;
            y -= v_y;
        }

        public void Draw()
        {
            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush);

            //Bitmap b = RotateImage(img, center, (float)degree);

            Graphics g = d.getGraphics;
            g.TranslateTransform(x + center.X, y + center.Y);
            g.RotateTransform((float)degree);
            //g.TranslateTransform(-(x + center.X), -(y + center.Y));
            d.getGraphics.DrawImage(img, -center.X, -center.Y);
            g.ResetTransform();
        }
        
        public void move(int moveno)
        {
            /// 0  1  2
            /// 3  4  5
            /// 6  7  8
            /// 
            void acceling(bool front)
            {
                if (front)
                    velocity += accel;
                else
                    velocity -= accel;
            }
            void turning(bool right)
            {
                if (right)
                    degree += turn;
                else
                    degree -= turn;
            }

            void lrcheck(int no)
            {
                if(moveno < no)
                {
                    turning(false);
                }
                else if(moveno > no)
                {
                    turning(true);
                }
            }

            if(moveno > 5)
            {
                acceling(false);
                lrcheck(7);
            }
            else if(moveno > 2)
            {
                lrcheck(4);
            }
            else
            {
                acceling(true);
                lrcheck(1);
            }

            go();
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
