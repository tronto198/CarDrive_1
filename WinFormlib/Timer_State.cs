using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WinFormlib
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
            if (instance == null)
            {
                instance = new Timer_State();
            }

            return instance;
        }

        /// <summary>
        /// 윈폼과 이 클래스를 연결, 연결하면 윈폼이 종료될때
        /// 이 클래스에 저장된 모든 스레드들을 먼저 종료 후 윈폼 종료
        /// </summary>
        /// <param name="form">연결할 윈폼</param>
        /// <returns></returns>
        public static Timer_State getinstance(System.Windows.Forms.Form form)
        {
            if (instance == null)
            {
                instance = new Timer_State();
            }
            form.FormClosing += new System.Windows.Forms.FormClosingEventHandler(instance.formclosing);
            return instance;
        }

        internal void Add(myThread item)
        {
            list.Add(item);
        }
        internal void Remove(myThread item)
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
                //e.Cancel = true;
            }
        }

        public void Stop()
        {
            Stopping = true;
            //Thread th = new Thread(delegate ()
            //{
            List<myThread> p_list = new List<myThread>();
            for (int i = 0; i < list.Count(); i++)
            {
                p_list.Add(list[i]);
            }
            foreach (myThread i in p_list)
            {
                i.Stop();
            }

            if (callback_TimerAllStopped != null)
            {
                callback_TimerAllStopped();
            }

            //});
            //th.Start();
        }
    }

    //분리해보려다가 실패 이건 아마 안쓰겠지..
    public abstract class myThread
    {
        private static Timer_State Timer_State = Timer_State.getinstance();

        //스레드 종료 완료시 콜백
        public delegate void TimerStopHandler();
        public TimerStopHandler Callback_Timerstop;

        public myThread()
        {
            Callback_Timerstop = null;
        }
        public virtual void Start()
        {
            Timer_State.Add(this);
        }
        public virtual void Stop()
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
        /// <summary>
        /// 콜백이 실행되기까지 기다리는 시간(ms)
        /// </summary>
        public int interval = 10;

        private Action action = null;
        Timer timer = null;

        /// <summary>
        /// 타이머가 만료될때마다 실행되는 함수 지정
        /// </summary>
        /// <param name="target">함수</param>
        public void setCallback(Action target)
        {
            action = target;
        }

        /// <summary>
        /// 타이머의 간격 설정
        /// </summary>
        /// <param name="peried">밀리초 단위로 설정</param>
        public void setInterval(int peried)
        {
            this.interval = peried;
        }

        public override void Start()
        {
            timer = new Timer(Task, action, 100, interval);

            base.Start();
        }

        private void Task(object obj)
        {
            Action action = obj as Action;
            action();
        }




        public override void Stop()
        {
            timer.Dispose();

            base.Stop();
        }


    }

    //예전버전 타이머, 성능이 좀더 좋은것 같더라?
    public class Threading_Timer_v0 : myThread
    {
        private WaitCallback WC = null;
        Action target = null;
        private Thread Thread = null;
        private bool Running = false;

        public int interval = 10;

        /// <summary>
        /// 타이머가 만료될때마다 실행되는 함수 지정
        /// </summary>
        /// <param name="target">함수</param>
        public void setCallback(Action target)
        {
            this.target = target;
            void action(object obj)
            {
                if (!Running)
                {
                    Running = true;
                    target();
                    Running = false;
                }
            }
            WC = new WaitCallback(action);
        }

        /// <summary>
        /// 타이머의 간격 설정
        /// </summary>
        /// <param name="peried">밀리초 단위로 설정</param>
        public void setInterval(int peried)
        {
            this.interval = peried;
        }

        public override void Start()
        {
            if (WC != null)
            {
                Thread = new Thread(new ThreadStart(T));
                Thread.Start();
                base.Start();
            }

        }

        void T()
        {
            while (!Timer_State.getinstance().Stopping)
            {
                ThreadPool.QueueUserWorkItem(WC);
                Thread.SpinWait(interval);
            }
        }

        public override void Stop()
        {
            try
            {
                Thread.Join();
            }
            catch (Exception e)
            {

            }
            base.Stop();
        }
    }
}
