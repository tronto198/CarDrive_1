using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDrive_1
{
    public class MainProgram
    {
        Map map = null;
        List<Car> Carlist = null;
        WinFormlib.Threading_Timer_v0 worker = null;

        public delegate void work_thread_Handler(Car car);
        public static work_thread_Handler callback_worker;

        public MainProgram()
        {
            Carlist = new List<Car>();
            
            makeMap();

            callback_worker += map.check;
        }
        
        void makeMap()
        {
            //맵 만드는 곳
            map = new Map();
            map.set(600, 300, 400, 250);
            map.Show();
        }

        //프로그램에 차 세팅
        public void SetCar(int Carnum = 1)
        {
            for(int i = 0;i < Carnum; i++)
            {
                Car car = new Car();
                Carlist.Add(car);
                car.setLocation(map.getStartPoint());
                car.setDegree(0);
                car.Show();
                car.setSize();
            }
        }

        //나중에 파이썬 쓰레드에서 실행, 또는 C# 테스트용 스레드에서 실행
        public void Thread_worker()
        {
            foreach (Car car in Carlist)
                callback_worker(car);
        }

        //이 프로그램을 C#으로 테스트할때 실행
        public void TestwithKeyinput()
        {
            SetCar(1);
            bindKey();
            worker = new WinFormlib.Threading_Timer_v0();
            worker.setInterval(10);
            worker.setCallback(Thread_worker);

            worker.Start();
        }

        //키보드 입력과 연결시키는 함수
        void bindKey()
        {
            callback_worker += reactKey;
        }

        //눌린 키에 반응하는 함수
        void reactKey(Car car)
        {
            WinFormlib.Key_input key = WinFormlib.Key_input.getinstance();
            int keyinput = 0;
            /// 0  1  2
            /// 3  4  5
            /// 6  7  8
            /// 

            void lr()
            {
                if (key.get_left)
                {
                    keyinput--;
                }
                else if (key.get_right)
                {
                    keyinput++;
                }
            }

            if (key.get_up) keyinput = 1;
            else if (key.get_down) keyinput = 7;
            else keyinput = 4;
            lr();

            car.move(keyinput);
        }

        /*public Tuple<double[][], int[], bool[]> Request_Move(int[] move_onehot)
        {
            
            for(int i = 0;i < move_onehot.Length;i++)
            {
                //Car.move(move_onehot[i]);
            }

            
        }*/
    }
}
