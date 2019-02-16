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
        ///맵 크기 정하고 그에 맞게 트랙 생성
        ///트랙에 따라서 차와 충돌되는지 계산
        ///트랙을 따라서 세이브 포인트를 만들고 이 역시 충돌계산
        ///트랙, 세이브 포인트에 충돌될때 각각 실행되는 함수 생성
        ///원점은 맵 가운데
        ///왼쪽 위 구석에 반복수 표시
        ///차를 여기에 넣을까? 차는 스레드로 한꺼번에 돌려야하나?
        ///타원말고 운동장 트랙처럼으로 변경  그럼 양끝은 원형으로, 위아래는 선으로 가능

        int x, y;
        int TrackWidth, TrackHeight;
        int TrackSize = 100;
        DoubleBuffering Screen;
        Line CenterLine;

        public Map()
        {
            Screen = DoubleBuffering.getinstance();
            CenterLine = new Line();
        }

        public void set(int _x, int _y, int width, int height)
        {
            x = _x;
            y = _y;
            TrackWidth = width;
            TrackHeight = height;
            //크기 두개 정함
            CenterLine.setPoint1(x - width / 2, y);
            CenterLine.setPoint2(x + width / 2, y);
            //센터라인의 x범위 내에서 y가 일정 범위 내에 잇어야함
        }

        public void Draw()
        {
            //Screen.getGraphics.DrawArc(new Pen(new SolidBrush(Color.Black)), new Rectangle())
        }

        public void Crashcheck(Car car)
        {
            //트랙에 충돌되는지
            //세이브 포인트에 도달했는지
            //선으로 얼마나 남앗는지?
            //각각 다른 함수로 연결
        }




    }

    class Line
    {
        public int x1, y1, x2, y2;

        public void setPoint1(int _x1, int _y1)
        {
            x1 = _x1;
            y1 = _y1;
        }

        public void setPoint2(int _x2, int _y2)
        {
            x2 = _x2;
            y2 = _y2;
        }
    }
}
