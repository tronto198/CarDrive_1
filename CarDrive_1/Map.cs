using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using WinFormlib;

namespace CarDrive_1
{
    class Map
    {
        ///해야할것들
        ///트랙에 따라서 차와 충돌되는지 계산 o
        ///트랙을 따라서 세이브 포인트를 만들고 이 역시 충돌계산
        ///트랙, 세이브 포인트에 충돌될때 각각 실행되는 함수 생성
        ///원점은 맵 가운데 o
        ///왼쪽 위 구석에 반복수 표시?
        ///차를 여기에 넣을까?

        int x, y;
        int TrackWidth, TrackHeight;
        const int TrackSize = 100;
        DoubleBuffering Screen;
        Line CenterLine;
        Pen thispen = new Pen(new SolidBrush(Color.Black));
        List<CheckLine> Linelist = new List<CheckLine>();

        public Map()
        {
            Screen = DoubleBuffering.getinstance();
            CenterLine = new Line();
        }

        /// <summary>
        /// 트랙의 위치와 크기를 정함, 좌표는 가운데가 기준
        /// </summary>
        /// <param name="_x"> 중점의 x좌표 </param>
        /// <param name="_y"> 중점의 y좌표 </param>
        /// <param name="width"> 트랙의 안쪽 크기(x축) </param>
        /// <param name="height"> 트랙의 안쪽 크기(y축) </param>
        public void set(int _x, int _y, int width, int height)
        {
            x = _x;
            y = _y;
            TrackWidth = width;

            this.TrackHeight = height;
            int half = TrackHeight / 2;

            //정가운데에 가로 선하나를 저장
            CenterLine.setPoint1(x - width / 2 - half, y);
            CenterLine.setPoint2(x + width / 2 - half, y);

            //차는 센터라인의 x범위 내에서 y가 일정 범위 내에 잇어야함
            //나머지는 센터라인 양끝점에서 원범위 내에 있는지로 판별

            //센터라인 양끝과 중앙, 반원형 1/3, 2/3 지점에 체크라인
            CheckLine make(CheckLine frontline, Point p1, Point p2, bool plus)
            {
                int size = TrackSize;
                if (!plus)
                {
                    size = -TrackSize;
                }

                CheckLine c0 = new CheckLine();
                c0.setPoint1((p1.X + p2.X) / 2, p1.Y);
                c0.setPoint2((p1.X + p2.X) / 2, p1.Y + size);
                frontline.Link(c0);
                c0.setreward(100);

                CheckLine c1 = new CheckLine();
                c1.setPoint1(p2);
                c1.setPoint2(p2.X , p2.Y + size);
                c0.Link(c1);

                Linelist.Add(c0);
                Linelist.Add(c1);

                return c1;
            }

            CheckLine make_round(CheckLine frontline, Point center, int startdegree, int enddegree)
            {
                double radian_1 = m.to_radian * (startdegree + (enddegree - startdegree) / 3 * 2);
                double radian_2 = m.to_radian * (startdegree + (enddegree - startdegree) / 3);
                CheckLine c0 = new CheckLine();
                c0.setPoint1((int)(center.X + Math.Sin(radian_1) * half), 
                    (int)(center.Y + Math.Cos(radian_1) * half));
                c0.setPoint2((int)(center.X + Math.Sin(radian_1) * (half + TrackSize)), 
                    (int)(center.Y + Math.Cos(radian_1) * (half + TrackSize)));
                frontline.Link(frontline);

                CheckLine c1 = new CheckLine();
                c1.setPoint1((int)(center.X + Math.Sin(radian_2) * half), 
                    (int)(center.Y + Math.Cos(radian_2) * half));
                c1.setPoint2((int)(center.X + Math.Sin(radian_2) * (half + TrackSize)),
                    (int)(center.Y + Math.Cos(radian_2) * (half + TrackSize)));
                c0.Link(c1);

                Linelist.Add(c0);
                Linelist.Add(c1);

                return c1;
        }


            CheckLine lastline = new CheckLine();
            lastline.setPoint1(CenterLine.point1.X, CenterLine.point1.Y + half);
            lastline.setPoint2(CenterLine.point1.X, CenterLine.point1.Y + half + TrackSize);



            CheckLine firstline = make(lastline, new Point(CenterLine.point1.X, CenterLine.point1.Y + half),
                 new Point(CenterLine.point2.X, CenterLine.point2.Y + half), true);

            CheckLine line2 = make_round(firstline, CenterLine.point2, 0, 180);
            CheckLine line3 = new CheckLine();
            line3.setPoint1(CenterLine.point2.X, CenterLine.point2.Y - half);
            line3.setPoint2(CenterLine.point2.X, CenterLine.point2.Y - half - TrackSize);
            line2.Link(line3);
            Linelist.Add(line3);

            CheckLine line4 = make(line3, new Point(CenterLine.point2.X, CenterLine.point2.Y - half),
                new Point(CenterLine.point1.X, CenterLine.point1.Y - half), false);

            CheckLine line5 = make_round(line4, CenterLine.point1, 180, 360);
            line5.Link(lastline);


            firstline.Activate();

            Linelist.Add(lastline);

            foreach (CheckLine c in Linelist)
            {
                c.Show();
            }
        }

        public void Show()
        {
            Screen.callback_work += Draw;
        }

        void Draw()
        {
            //트랙을 그리는 작업을 함수로 지정
            void work(int Size)     // Size = 양끝 원의 크기(지름 이면서 트랙의 y축 크기)
            {
                int half = Size / 2;    //반지름, 센터라인에서 위아래의 직선라인까지의 거리

                int x1 = CenterLine.point1.X;
                int y1 = CenterLine.point1.Y;
                int x2 = CenterLine.point2.X;
                int y2 = CenterLine.point2.Y;
                

                //위아래 직선라인 그리기
                Screen.getGraphics.DrawLine(thispen, x1, y1 - half, x2, y2 - half);
                Screen.getGraphics.DrawLine(thispen, x1, y1 + half, x2, y2 + half);

                //양끝 반원들 그리기
                Screen.getGraphics.DrawArc(thispen, x1 - half, y1 - half, Size, Size, 90, 180);
                Screen.getGraphics.DrawArc(thispen, x2 - half, y2 - half, Size, Size, 270, 180);
                
            }

            //안쪽 트랙 그리기
            work(TrackHeight);

            //바깥쪽 트랙 그리기
            work(TrackHeight + TrackSize * 2);
            
        }

        
        /// <summary>
        /// 차가 트랙에 닿는지, 포인트를 지났는지 검사
        /// </summary>
        /// <param name="car">검사할 차</param>
        public void check(Car car)
        {

            //트랙에 충돌되는지
            //세이브 포인트에 도달했는지
            //선으로 얼마나 남앗는지?
            //각각 다른 함수로 연결

            //차의 모서리 넷 필요
            //모서리 넷이 각각 안에 있는지로 판별
            

            //한 점이 트랙 안에 정상적으로 있는지 판별 (참이면 정상, 거짓이면 충돌)
            bool checkCrash(Point p)
            {
                //센터라인에서부터 half만큼 떨어진 곳이 트랙
                //추가로 TrackSize만큼 떨어진곳이 트랙의 바깥쪽
                int half = TrackHeight / 2; 
                

                ///충돌을 감지
                bool CatchCrash(double num)
                {
                    if(num < half || half + TrackSize < num) //충돌 했을때
                    {
                        return false;
                    }
                    else  //통과!
                    {
                        return true;
                    }
                }


                //점의 위치에 따라 기준을 다르게 잡음
                if(p.X < CenterLine.point1.X)  //왼쪽 곡선 라인일때
                {
                    double length = m.getLength(p, CenterLine.point1);  //왼쪽 CenterLine 끝점과의 거리

                    return CatchCrash(length);
                }
                else if (CenterLine.point2.X < p.X) //오른쪽 곡선 라인일때
                {
                    double length = m.getLength(p, CenterLine.point2);  //오른쪽 CenterLine 끝점과의 거리

                    return CatchCrash(length);
                }
                else //직선 라인일때
                {
                    int abs = Math.Abs(p.Y - y);  //CenterLine과 점과의 거리
                    return CatchCrash(abs);
                }

            }

            //차가 세이브 포인트를 넘겼는지 판별 차의 왼쪽 앞점, 차의 오른쪽 앞점
            //bool checkBonus(Point Lp, Point Rp)
            {

            }

            //checkCrash 4번, 보너스 한번 실행


        }

        void Crashed(Car car)
        {
            //차에 함수 만들기
        }

        void Bonus(Car car)
        {
            //차에 함수 만들기
        }


    }

    //선
    class Line
    {
        private Point p1, p2;
        private double length;
        protected Pen DrawingPen = new Pen(new SolidBrush(Color.Black));
        protected bool drawing = false;
        //얘네는 숨기고

        public Point point1 { get { return p1; } }
        public Point point2 { get { return p2; } }
        public double Length { get { return length; } }
        //이렇게 하면 함수를 쓰지 않고 읽기 전용으로 가능 
        //함수로 부르지 않고 편하게 쓰고 싶지만 바뀌어서는 안되는 것들을 쓰면 좋은듯?


        /// <summary>
        /// 첫번째 끝점 지정
        /// </summary>
        /// <param name="_x1"></param>
        /// <param name="_y1"></param>
        public void setPoint1(int _x1, int _y1)
        {
            p1.X = _x1;
            p1.Y = _y1;
            getLength();
        }

        /// <summary>
        /// 첫번째 끝점 지정
        /// </summary>
        /// <param name="p"></param>
        public void setPoint1(Point p)
        {
            setPoint1(p.X, p.Y);
        }

        /// <summary>
        /// 두번째 끝점 지정
        /// </summary>
        /// <param name="_x2"></param>
        /// <param name="_y2"></param>
        public void setPoint2(int _x2, int _y2)
        {
            p2.X = _x2;
            p2.Y = _y2;
            getLength();
        }

        /// <summary>
        /// 두번째 끝점 지정
        /// </summary>
        /// <param name="p"></param>
        public void setPoint2(Point p)
        {
            setPoint2(p.X, p.Y);
        }

        /// <summary>
        /// 선의 길이 측정
        /// </summary>
        /// <returns></returns>
        public double getLength()
        {
            length = m.getLength(p1, p2);
            return length;
        }

        //두 선분의 교점 구하기 (여기서는 겹치는지만)
        //출처 : http://www.gisdeveloper.co.kr/?p=89
        public bool CrossLIne(Line l)
        {
            double t;
            double s;
            double under = (l.p2.Y - l.p1.Y) * (p2.X - p1.X) - (l.p2.X - l.p1.X) * (p2.Y - p1.Y);
            if (under == 0) return false;

            double _t = (l.p2.X - l.p1.X) * (p1.Y - l.p1.Y) - (l.p2.Y - l.p1.Y) * (p1.X - l.p1.X);
            double _s = (p2.X - p1.X) * (p1.Y - l.p1.Y) - (p2.Y - p1.Y) * (p1.X - l.p1.X);

            if (_t == 0 && _s == 0)
                return false;

            
            t = _t / under;
            s = _s / under;

            if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0) return false;

            Point crosspoint = new Point();
            crosspoint.X = (int)(p1.X + t * (double)(p2.X - p1.X));
            crosspoint.Y = (int)(p1.Y + t * (double)(p2.Y - p1.Y));

            return true;
        }

        public virtual void Show()
        {
            if (!drawing)
            {
                DoubleBuffering.getinstance().callback_work += delegate ()
                {
                    DoubleBuffering.getinstance().getGraphics.DrawLine(DrawingPen, p1, p2);
                };
            }
        }
    }


    class CheckLine : Line
    {
        int reward = 10;
        bool activation = false;
        Pen ActivatePen = new Pen(new SolidBrush(Color.Green));
        CheckLine nextLine = null;

        
        public void setreward(int _reward)
        {
            reward = _reward;
        }

        public void Link(CheckLine next)
        {
            nextLine = next;
        }

        public void Activate()
        {
            activation = true;
        }

        public int getreward()
        {
            activation = false;
            nextLine.activation = true;
            return reward;
        }

        public override void Show()
        {
            if (!drawing)
            {
                DoubleBuffering.getinstance().callback_work += delegate ()
                {
                    if (activation)
                    {
                        DoubleBuffering.getinstance().getGraphics.DrawLine(ActivatePen, point1, point2);
                    }
                    else
                    {
                        DoubleBuffering.getinstance().getGraphics.DrawLine(DrawingPen, point1, point2);
                    }
                };
            }
        }
    }

    //길이 구하는거 많이 써서 따로 만들어버림
    public static class m
    {
        //degree to radian
        public const double to_radian = Math.PI / 180;
        public static double getLength(Point p1, Point p2)
        {
            return Math.Sqrt((double)((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }
    }
}
