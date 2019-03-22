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
        int total_reward = 0;
        object Carlist_locker = new object();
        public static System.Drawing.Font font = new System.Drawing.Font("휴먼편지체", 15);
        public static System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

        public delegate void work_thread_Handler(Car car);
        public static work_thread_Handler callback_worker;

        void Draw_totalReward()
        {
            WinFormlib.DoubleBuffering.getinstance().getGraphics.DrawString(
                "Reset to 'R'", font, brush, 300, 300);
            WinFormlib.DoubleBuffering.getinstance().getGraphics.DrawString(
                "Reward : " + total_reward, font, brush, 550, 300); 
        }

        public MainProgram()
        {
            Carlist = new List<Car>();
            
            makeMap();
            WinFormlib.DoubleBuffering.getinstance().callback_work += Draw_totalReward;

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
            lock (Carlist_locker)
            {
                for (int i = 0; i < Carnum; i++)
                {
                    Car car = new Car();
                    Carlist.Add(car);
                    car.setLocation(map.getStartPoint());
                    car.setDegree(map.getStartDegree());
                    car.Show();
                    car.setSize();
                }
            }
        }
        public void Reset(int Carnum = 1)
        {
            lock (Carlist_locker)
            {
                foreach (Car c in Carlist)
                {
                    c.unShow();
                }
                Carlist.Clear();
            }
            total_reward = 0;
            map.Reset();

            SetCar(Carnum);
        }

        //파이선 스레드에서 move후에 threadworker실행 XX
        //나중에 파이썬 쓰레드에서 실행, 또는 C# 테스트용 스레드에서 실행
        //취소 -> 안쓸예정
        //public void Thread_worker()
        //{
        //    foreach (Car car in Carlist)
        //    {
        //        //callback_worker(car);
                
        //    }

        //}

        //이 프로그램을 C#으로 테스트할때 실행
        public void TestwithKeyinput()
        {
            SetCar(1);
            WinFormlib.Key_input.Key_in += delegate (System.Windows.Forms.Keys key)
            {
                if(key == System.Windows.Forms.Keys.R)
                {
                    Reset();
                }
            };
            bindKey();
        }

        //키보드 입력과 연결시키는 함수
        void bindKey()
        {
            worker = new WinFormlib.Threading_Timer_v0();
            worker.setInterval(10);
            worker.setCallback(reactKey);
            worker.Start();
        }

        //눌린 키에 반응하는 함수
        void reactKey()//Car car)
        {
            
            WinFormlib.Key_input key = WinFormlib.Key_input.getinstance();
            int keyinput = 4;
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

            //car.move(keyinput);
            /*int[] onehot = new int[9];
            for(int i = 0;i < 9; i++)
            {
                onehot[i] = 0;
            }
            onehot[keyinput] = 1;
            Request_Move(onehot);*/
            Request_Move(keyinput);
        }

        public Tuple<double[], int, bool> Request_Move(int moveno)//int[] move_onehot)
        {
            //속도, 각도, 거리1, 2, 3, 4, 5, reward, done;
            /*int moveno = 0;
            for(int i = 0; i < 9;i++)
            {
                if(move_onehot[i] == 1)
                {
                    moveno = i;
                    break;
                }
            }*/
            //for(int i = 0;i < move_onehot.Length;i++)

            double v;
            double degree;
            double[] distance;
            int reward;
            bool done;

            lock (Carlist_locker)
            {
                if (Carlist.Count > 0)
                {
                    Carlist[0].move(moveno);
                    callback_worker(Carlist[0]);


                    v = Carlist[0].getv();
                    degree = Carlist[0].getdegree();
                    distance = Carlist[0].getdistances();
                    reward = Carlist[0].getreward();
                    done = Carlist[0].done;


                    double[] t = new double[7];
                    t[0] = v;
                    t[1] = degree;
                    distance.CopyTo(t, 2);
                    this.total_reward += reward;
                    Tuple<double[], int, bool> ans = new Tuple<double[], int, bool>(t, reward, done);
                    return ans;
                }
            }
            return null;
        }


    }
}
