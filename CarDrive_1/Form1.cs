using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CarDrive_1
{
    public partial class Form1 : Form
    {
        WinFormlib.DoubleBuffering DoubleBuffering = null;
        WinFormlib.Timer_State Timer_State = null;
        WinFormlib.Form_input Form_input = null;

        public Form1()
        {
            InitializeComponent();
            
            Start();
            
        }

        public void Start()
        {
            Timer_State = WinFormlib.Timer_State.getinstance(this);
            DoubleBuffering = WinFormlib.DoubleBuffering.getinstance();
            DoubleBuffering.setInstance(this);
            Form_input = new WinFormlib.Form_input();
            Form_input.binding(this);
            MessageBox.Show("시작했어요!!");
            DoubleBuffering.callback_work += GraphicEvent;
        }

        public void GraphicEvent()
        {
            DoubleBuffering.getGraphics.Clear(Color.LightBlue);
        }
    }
}
