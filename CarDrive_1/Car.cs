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
        const double accel = 0.09;
        const double max_velocity = 15.5;
        double velocity = 0.0;
        double turn_max_velocity = 2;
        const double stop_friction = 0.1;
        const double run_friction = 0.05;
        double degree = 0;
        const double turn = 3;

        //Threading_Timer timer = new Threading_Timer();
        //Rectangle car = new Rectangle();
        double x = 0, y = 100;
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
        
        //차를 실제로 움직임
        public void go()
        {
            if (velocity > 0)
            {
                velocity -= run_friction;
                if (velocity < 0)
                {
                    velocity = 0;
                }
                else if(velocity > max_velocity)
                {
                    velocity = max_velocity;
                }
            }
            else
            {
                velocity += run_friction;
                if (velocity > 0)
                {
                    velocity = 0;
                }
                else if(velocity < -max_velocity)
                {
                    velocity = -max_velocity;
                }
            }
            

            double v_x = 0;
            double v_y = 0;

            v_y = (Math.Cos(degree * m.to_radian) * velocity);
            v_x = (Math.Sin(degree * m.to_radian) * velocity);

            x += v_x;
            y -= v_y;
        }

        public void Draw()
        {
            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush);

            //Bitmap b = RotateImage(img, center, (float)degree);

            Graphics g = d.getGraphics;

            //회전했을때 좌표

            g.TranslateTransform((float)x + center.X, (float)y + center.Y);

            g.RotateTransform((float)degree);
            //g.TranslateTransform(-(x + center.X), -(y + center.Y));
            d.getGraphics.DrawImage(img, -center.X, -center.Y);
            g.ResetTransform();
        }
        
        //차를 어떻게 움직일지 입력
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
                double f_turn = turn;
                double abs_v = Math.Abs(velocity);
                if (turn_max_velocity > abs_v)
                    f_turn *= abs_v / turn_max_velocity;

                if (right)
                    degree += f_turn;
                else
                    degree -= f_turn;
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

        public void car_vertex()//차의 네개의 꼭짓점
        {

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
