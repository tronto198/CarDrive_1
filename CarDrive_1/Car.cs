using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using WinFormlib;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CarDrive_1
{
    //class TimerExample
    //{
    //    public void Timer()
    //    {
    //        var autoEvent = new AutoResetEvent(false);
    //        var statusChecker = new StatusChecker(10);
    //        var stateTimer = new Timer(statusChecker.CheckStatus, autoEvent, 1000, 250);

    //    }
    //}
    //class StatusChecker
    //{
    //    private int invokeCount;
    //    private int maxCount;
    //    public StatusChecker(int count)
    //    {
    //        invokeCount = 0;
    //        maxCount = count;
    //    }
    //    public void CheckStatus(object stateInfo)
    //    {
    //        AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            
    //    }
    //}
    public class Car
    {
        const double accel = 0.09;
        const double max_velocity = 15.5;
        double velocity = 0.0;
        double turn_max_velocity = 3;
        const double stop_friction = 0.1;
        const double run_friction = 0.05;
        double degree = 0;
        const double turn = 3;
        Line[] lines = new Line[5];
        double[] distances = new double[5];
        public double reward = 0;
        public bool done = false;
        Line left = new Line();
        Line right = new Line();


        //Rectangle car = new Rectangle();
        double x = 0, y = 100;
        DoubleBuffering d = DoubleBuffering.getinstance();


        Image img = Image.FromFile("car.png");
        
        Bitmap bitmap = null;
        Pen DrawingPen = new Pen(new SolidBrush(Color.Black));
        

        Point center;

        public Line[] getlines() { return lines; }
        public void setdistance(double[] distances)
        {
            this.distances = distances;
        }
        public double[] getdistances() { return distances; }
        public double getdegree() { return degree; }
        public double getv() { return velocity; }
        public double getreward()
        {
            double r = reward;
            reward = 0;
            return r;
        }

        public void setSize()
        {
            Size resize = new Size(27, 60);
            center = new Point((int)(img.Width / 2), (int)(img.Height / 2));
        }
        public Car()
        {
            for(int i = 0;i < 5; i++)
            {
                lines[i] = new Line();
            }
            center = new Point((int)(img.Width / 2), (int)(img.Height / 2));
            bitmap = new Bitmap(img);
        }
        

        public void Show()
        {
            d.callback_work += Draw;
            foreach (var l in lines)
            {
                l.Show();
            }
            left.Show();
            right.Show();
        }
        public void unShow()
        {
            d.callback_work -= Draw;
            foreach(var l in lines)
            {
                l.unShow();
            }

            left.unShow();
            right.unShow();
        }
        
        public void setLocation(Point p)
        {
            setLocation(p.X, p.Y);
        }
        public void setLocation(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public void setDegree(int _degree)
        {
            degree = (double)_degree;
        }


        //그리기
        void Draw()
        {
            //Bitmap b = RotateImage(img, center, (float)degree);

            Graphics g = d.getGraphics;

            //회전했을때 좌표

            g.TranslateTransform((float)x, (float)y);

            g.RotateTransform((float)degree);
            //g.TranslateTransform(-(x + center.X), -(y + center.Y));
            d.getGraphics.DrawImage(img, -center.X, -center.Y);
            g.ResetTransform();

            Font font = new Font("휴먼편지체", 8);
            for(int i = 0;i < 5; i++)
            {
                g.DrawString(distances[i].ToString("##. ###"), font, MainProgram.brush,
                    (float)x, (float)y - 15 * 4 + 15 * i);

            }
        }
        
        //차를 어떻게 움직일지 입력
        public void move(int moveno)
        {
            if (done) return;
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
                {
                    degree += f_turn;
                    if (degree > 360) degree -= 360;
                }
                else
                {
                    degree -= f_turn;
                    if (degree < 0) degree += 360;
                }
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
            cal_distance();
            
        }
        //차를 실제로 움직임
        void go()
        {
            if (velocity > 0)
            {
                velocity -= run_friction;
                if (velocity < 0)
                {
                    velocity = 0;
                }
                else if (velocity > max_velocity)
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
                else if (velocity < -max_velocity)
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

        void cal_distance()
        {
            double bet = 15;
            double thisdegree = degree - 90 - bet * 2;
            int distance = 15 * 20;
            
            for(int i = 0;i < 5; i++) 
            {
                double d = thisdegree + i * bet;
                lines[i].setPoint1((int)x, (int)y);
                lines[i].setPoint2((int)(x + distance * Math.Cos(m.to_radian * d)), 
                    (int)(y + distance * Math.Sin(m.to_radian * d)));
            }

            
        }


        public void car_vertex(out Line _left, out Line _right)// 앞부분 가운데, 뒷부분 가운데 점의 좌표 **검증필요**
        {

            /*
            double x_f, y_f, x_b, y_b;//실제 사용할 좌표
            double x_f_i, y_f_i, x_b_i, y_b_i;//초기값
            Point real_center = new Point((int)x, (int)y);
            double a, b;//일차함수의 기울기, y절편
            x_f_i = real_center.X;
            y_f_i = real_center.Y + 30.0;


            x_b_i = real_center.X;
            y_b_i = real_center.X - 30.0;

            x_f = (x_f_i - real_center.X) * Math.Cos(degree) - (y_f_i - real_center.Y) * Math.Sin(degree) + real_center.X;

            y_f = (x_f_i - real_center.X) * Math.Sin(degree) + (y_f_i - real_center.Y) * Math.Cos(degree) + real_center.Y;

            x_b = (x_b_i - real_center.X) * Math.Cos(degree) - (y_b_i - real_center.Y) * Math.Sin(degree) + real_center.X;
            y_b = (x_b_i - real_center.X) * Math.Sin(degree) + (y_b_i - real_center.Y) * Math.Cos(degree) + real_center.Y;


            // a = (y_f - y_b) / (x_f - x_b);//기울기
            // b = y_f - a * x_f;//y절편


            double x_1, y_1, x_2, y_2, x_3, y_3, x_4, y_4;
            //초기값 설정
            x_1 = real_center.X - 13.5;
            y_1 = real_center.Y + 30.0;
            x_2 = real_center.X - 13.5;
            y_2 = real_center.Y - 30.0;
            x_3 = real_center.X + 13.5;
            y_3 = real_center.Y + 30.0;
            x_4 = real_center.X + 13.5;
            y_4 = real_center.Y - 30.0;

            //구하려는 값
            x_1 = (x_1 - real_center.X) * Math.Cos(degree) - (y_1 - real_center.Y) * Math.Sin(degree) + real_center.X;
            y_1 = (x_1 - real_center.X) * Math.Sin(degree) + (y_1 - real_center.Y) * Math.Cos(degree) + real_center.Y;
            x_2 = (x_2 - real_center.X) * Math.Cos(degree) - (y_2 - real_center.Y) * Math.Sin(degree) + real_center.X;
            y_2 = (x_2 - real_center.X) * Math.Sin(degree) + (y_2 - real_center.Y) * Math.Cos(degree) + real_center.Y;
            x_3 = (x_3 - real_center.X) * Math.Cos(degree) - (y_3 - real_center.Y) * Math.Sin(degree) + real_center.X;
            y_3 = (x_3 - real_center.X) * Math.Sin(degree) + (y_3 - real_center.Y) * Math.Cos(degree) + real_center.Y;
            x_4 = (x_4 - real_center.X) * Math.Cos(degree) - (y_4 - real_center.Y) * Math.Sin(degree) + real_center.X;
            y_4 = (x_4 - real_center.X) * Math.Sin(degree) + (y_4 - real_center.Y) * Math.Cos(degree) + real_center.Y;

            */

            //
            //1   3
            //
            //2   4
            //
            double length = Math.Sqrt(center.X * center.X + center.Y * center.Y) - 2;
            double degree = this.degree + 90;
            double d_fr = Math.Atan(center.Y / center.X) * m.to_degree + 90;
            double d_fl = -d_fr;
            double d_br = 180 + d_fl;
            double d_bl = 180 + d_fr;

            double x_1 = x + length * Math.Cos((d_fl + degree) * m.to_radian);
            double y_1 = y + length * Math.Sin((d_fl + degree) * m.to_radian);
            double x_2 = x + length * Math.Cos((d_bl + degree) * m.to_radian);
            double y_2 = y + length * Math.Sin((d_bl + degree) * m.to_radian);
            double x_3 = x + length * Math.Cos((d_fr + degree) * m.to_radian);
            double y_3 = y + length * Math.Sin((d_fr + degree) * m.to_radian);
            double x_4 = x + length * Math.Cos((d_br + degree) * m.to_radian);
            double y_4 = y + length * Math.Sin((d_br + degree) * m.to_radian);

            left.setPoint1(x_1, y_1);
            left.setPoint2(x_2, y_2);
            right.setPoint1(x_3, y_3);
            right.setPoint2(x_4, y_4);
            _left = this.left;
            _right = this.right;
            
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
