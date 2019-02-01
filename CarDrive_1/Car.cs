using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;


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
        
       
        /// <summary>
        /// 속도, 가속도 설정
        /// </summary>
        /// <param name="">
        /// 
        /// </param>
        public void input()
        {
          
            double accel = 9.8;         
            double velocity= 0.0;
            velocity= velocity+accel;

            //velocity = velocity_0 + accel * duration;
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
