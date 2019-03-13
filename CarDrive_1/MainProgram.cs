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
        WinFormlib.Threading_Timer_v0 worker;

        public MainProgram()
        {
            map = new Map();
            Carlist = new List<Car>();

            makeMap();
        }

        public void makeCar()
        {
            Car car = new Car();
            Carlist.Add(car);
            car.Start();
        }


        public void makeMap()
        {
            //맵 만드는 곳
            Map Map = new Map();
            Map.set(600, 300, 400, 250);
            Map.Show();
        }

        public void Set(int Carnum = 1)
        {
            for(int i = 0;i < Carnum; i++)
            {
                makeCar();
            }
        }

        public void bindKey()
        {
            Set();
            worker = new WinFormlib.Threading_Timer_v0();
            worker.setInterval(10);
            worker.setCallback(reactKey);
            worker.Start();
        }

        void reactKey()
        {
            WinFormlib.Key_input key = WinFormlib.Key_input.getinstance();
            int keyinput = 0;
            /// 4  1  6
            /// 3  0  7
            /// 8  5  2
            /// 
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

            if (key.get_up)
            {
                keyinput = 1;
                lr();
            }
            else if (key.get_down)
            {
                keyinput = 7;
                lr();
            }
            else
            {
                keyinput = 4;
                lr();
            }

            Carlist[0].move(keyinput);
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
