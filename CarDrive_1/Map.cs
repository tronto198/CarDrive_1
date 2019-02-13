using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        
        int Width, Height;

        public void setSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            //타원 크기 두개 정함
        }

        public void Crashcheck(Car car)
        {
            //트랙에 충돌되는지
            //세이브 포인트에 도달했는지
            //선으로 얼마나 남앗는지?
            //각각 다른 함수로 연결
        }




    }
}
