using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDrive_1
{
    public class MainProgram
    {
        Form1 form = null;
        Map map = null;
        List<Car> Full_Carlist = null;
        List<Car> Active_Carlist = null;
        WinFormlib.Threading_Timer_v0 worker = null;
        double[] total_reward;
        int play_count = 0;
        bool running = false;
        public static int carnum = 0;
        object Carlist_locker = new object();


        public static System.Drawing.Font font = new System.Drawing.Font("휴먼편지체", 15);
        public static System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        

        public delegate void work_thread_Handler(Car car, int carnum);
        public static work_thread_Handler callback_worker;

        void Draw_totalReward()
        {
            WinFormlib.DoubleBuffering.getinstance().getGraphics.DrawString(
                "Reset to 'R'", font, brush, 250, 300);
            WinFormlib.DoubleBuffering.getinstance().getGraphics.DrawString(
                "play : " + play_count, font, brush, 380, 250);

            WinFormlib.DoubleBuffering.getinstance().getGraphics.DrawString(
                "Reward : ", font, brush, 490, 300);

            int height = 300 - carnum / 2 * 17;
            for(int i =0;i < carnum; i++)
            {
                WinFormlib.DoubleBuffering.getinstance().getGraphics.DrawString(
                     total_reward[i].ToString("###. ##"), font, LineColors.brushes[i], 650, height + i * 17);
            }
        }

        public MainProgram(Form1 form)
        {
            this.form = form;
            running = true;
            Active_Carlist = new List<Car>();
            Full_Carlist = new List<Car>();
            
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

        //프로그램에 차 추가
        public void AddCar(int Carnum = 1)
        {
            carnum = Carnum;
            total_reward = new double[carnum];
            map.setCarnum(carnum);
            lock (Carlist_locker)
            {
                for (int i = 0; i < Carnum; i++)
                {
                    Car car = new Car();
                    Active_Carlist.Add(car);
                    Full_Carlist.Add(car);
                    car.setcarnum(i);
                    car.Show();
                    car.setSize();
                }
            }
            SetCar();
        }

        //프로그램에 차 세팅
        void SetCar()
        {
            lock (Carlist_locker)
            {
                for (int i = 0; i < Active_Carlist.Count; i++)
                {
                    Car car = Active_Carlist[i];
                    car.setLocation(map.getStartPoint());
                    car.setDegree(map.getStartDegree());
                }
            }
            for (int i = 0; i < carnum; i++)
            {
                total_reward[i] = 0;
            }
        }
        public void playcount(int count)
        {
            play_count = count;
        }
        public bool Reset()
        {
            if (!running) return false;
            lock (Carlist_locker)
            {
                Active_Carlist.Clear();
                foreach (Car c in Full_Carlist)
                {
                    Active_Carlist.Add(c);
                }
            }
            
            SetCar();
            map.Reset();

            return true;
        }
        public bool check()
        {
            return running;
        }


        //이 프로그램을 C#으로 테스트할때 실행
        public void TestwithKeyinput()
        {
            AddCar(1);
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
            Request_Move(new int[1] { keyinput });
        }

        public void Formclose()
        {
            running = false;
        }
        


        public Tuple<double[], double, bool>[] Request_Move(int[] moveno)//int[] move_onehot)
        {
            //속도, 거리1, 2, 3, 4, 5, reward, done;
            //위치, 각도

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

            //double v;
            //double degree;
            //double max_reward = 0;

            double normalization_v(double v)
            {
                return v / Car.max_velocity * 10;
            }

            double normalization_dis(double dis)
            {
                return dis / 300 * 10;
            }

            Tuple<double[], double, bool>[] anslist;
            int n = 0;
            lock (Carlist_locker)
            {
                anslist = new Tuple<double[], double, bool>[Active_Carlist.Count];

                if (Active_Carlist.Count < 1)
                {
                    form.Ringing("1");
                    return null;
                }
                for (int i = 0; i < moveno.Length; i++)
                {
                    //System.Windows.Forms.MessageBox.Show(i+" " + Active_Carlist.Count);

                    double[] dbs = new double[6];
                    int no = i - n;
                    double reward;
                    bool done;
                    double[] distance;
                        
                    Active_Carlist[no].move(moveno[i]);
                    callback_worker(Active_Carlist[no], Active_Carlist[no].carnum);


                    dbs[0] = normalization_v( Active_Carlist[no].getv() );
                    //dbs[1] = Active_Carlist[no].getx();
                    //dbs[2] = Active_Carlist[no].gety();
                    //dbs[3] = Active_Carlist[0].getdegree();
                    distance = Active_Carlist[no].getdistances();
                    reward = Active_Carlist[no].getreward();
                    done = Active_Carlist[no].done;

                    for(int j = 0; j < 5; j++)
                    {
                        distance[j] = normalization_dis(distance[j]);
                    }
                    distance.CopyTo(dbs, 1);
                    total_reward[Active_Carlist[no].carnum] += reward;

                    if (done)
                    {
                        n++;
                        Active_Carlist.RemoveAt(no);
                    }

                    Tuple<double[], double, bool> ans = new Tuple<double[], double, bool>(dbs, reward, done);
                    anslist[i] = ans;
                }
                //this.total_reward += max_reward;
                
                
            }
            if (anslist == null) form.Ringing("2");
            return anslist;
        }

    }


}
