using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SharedLibaray
{
    public class Timer_State
    {
        private static Timer_State instance = null;

        //모든 타이머 종료 후 콜백
        public delegate void TimerAllStopHandler();
        public TimerAllStopHandler callback_TimerAllStopped;

        private List<myThread> list;

        public bool Stopping = false;
        

        private Timer_State()
        {
            callback_TimerAllStopped = null;
            list = new List<myThread>();
        }

        public static Timer_State getinstance()
        {
            if(instance == null)
            {
                instance = new Timer_State();
            }

            return instance;
        }

        public static Timer_State getinstance(System.Windows.Forms.Form form)
        {
            if (instance == null)
            {
                instance = new Timer_State();
            }
            form.FormClosing += new System.Windows.Forms.FormClosingEventHandler(instance.formclosing);
            return instance;
        }

        public void Add(myThread item)
        {
            list.Add(item);
        }
        public void Remove(myThread item)
        {
            list.Remove(item);
        }

        public int count() { return list.Count(); }

        private void formclosing(object obj, System.Windows.Forms.FormClosingEventArgs e)
        {
            Stop(e);
        }

        public void Stop(System.ComponentModel.CancelEventArgs e)
        {
            if (!Stopping)
            {
                Stop();
                e.Cancel = true;
            }
        }

        public void Stop()
        {
            Stopping = true;
            Thread th = new Thread(delegate ()
            {
                List<myThread> p_list = new List<myThread>();
                for (int i = 0; i < list.Count(); i++)
                {
                    p_list.Add(list[i]);
                }
                foreach (Threading_Timer i in p_list)
                {
                    i.Stop();
                }

                if (callback_TimerAllStopped != null)
                {
                    callback_TimerAllStopped();
                }
            });
            th.Start();
        }
    }

    public class myThread
    {
        private static Timer_State Timer_State = Timer_State.getinstance();

        //스레드 종료 완료시 콜백
        public delegate void TimerStopHandler();
        public TimerStopHandler Callback_Timerstop;

        public myThread()
        {
            Callback_Timerstop = null;
        }
        public void Start()
        {
            Timer_State.Add(this);
        }
        public void Stop()
        {
            if (Callback_Timerstop != null)
            {
                Callback_Timerstop();
            }
            Timer_State.Remove(this);
        }
    }

    public class Threading_Timer : myThread
    {

        public int interval = 10;
        
        private Action action = null;
        Timer timer = null;

        public void setCallback(Action target) 
        {
            action = target;
        }
        public void setInterval(int peried)
        {
            this.interval = peried;
        }
        public void Start()
        {
            timer = new Timer(Task, action, 100, interval);
            
            base.Start();
        }

        private void Task(object obj)
        {
            Action action = obj as Action;
            action();
        }

        


        public void Stop()
        {
            timer.Dispose();

            base.Stop();
        }

    }
    
}
