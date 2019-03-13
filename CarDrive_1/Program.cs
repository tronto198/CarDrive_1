using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CarDrive_1
{
    public static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        //[STAThread]
        public static void Main()
        {
            //Application.Run(new Form1());
            ExMain();
        }

        public static Form1 ExMain()
        {
            //ApplicationSetting();
            Form1 form = null;
            bool b = true;
            Thread t = new Thread(delegate (){
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                form = new Form1();
                b = false;
                Application.Run(form);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (b);
            return form;
        }

        public static void ApplicationSetting()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }

}
