using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormlib
{
    /*
     * Form.Designer.cs에 복붙하고 시작    또는    밑의 binding 실행
     
        this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(Key_input.Key_Preview);
        this.KeyDown += new System.Windows.Forms.KeyEventHandler(Key_input.Key_down);
        this.KeyUp += new System.Windows.Forms.KeyEventHandler(Key_input.Key_up);
        this.MouseDown += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_down);
        this.MouseMove += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_move);
        this.MouseUp += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_up);

    */
    public class Form_input
    {
        bool bind = false;

        public static Mouse_input Mouse_input = null;
        public static Key_input Key_input = null;

        /// <summary>
        /// 윈폼의 마우스와 키보드 인풋에 이 클래스들 연결
        /// </summary>
        /// <param name="form">연결할 윈폼 지정</param>
        public void binding(Form form)
        {
            
            if (bind)
            {
                throw new Exception("이미 바인딩되었습니다.");
            }
            else
            {
                Mouse_input = new Mouse_input();
                Key_input = new Key_input();
                form.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(Key_input.Key_Preview);
                form.KeyDown += new System.Windows.Forms.KeyEventHandler(Key_input.Key_down);
                form.KeyUp += new System.Windows.Forms.KeyEventHandler(Key_input.Key_up);
                form.MouseDown += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_down);
                form.MouseMove += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_move);
                form.MouseUp += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_up);
                
            }
            bind = true;
        }
    }
    
    public class Mouse_input
    {
        //이것저것 실험하느라 쓸데없는 것들이 많으니 생략
        //드래그로 박스 만드는 기능이 있지만 일단 꺼져있음
        //아마 이 프로젝트에선 안쓰지 않을까..?
        static Mouse_input oInstance = null;

        bool Drag = false;
        Point first_click = new Point();
        Rectangle rectangle = new Rectangle();
        Point Mouse_point = new Point();

        Brush thisbrush = new SolidBrush(Color.FromArgb(50, Color.LightGreen));

        Pen greenpen = new Pen(Color.Green, 0.02f);


        public bool Draw { get; set; }

        public static Mouse_input getinstance()
        {
            if(oInstance == null)
            {
                throw new Exception("Form_input.binding() 필요");
            }
            else
            {
                return oInstance;
            }
        }

        internal Mouse_input()
        {
            oInstance = this;
            //Draw = true;
        }

        public bool Dragging { get { return Drag; } }
        public Point point { get { return Mouse_point; } }

        public void Mouse_down(object sender, MouseEventArgs e)
        {
            rectangle.Location = e.Location;
            first_click = e.Location;
            rectangle.Width = 0;
            rectangle.Height = 0;
            if (!Drag)
            {
                if (Draw)
                    //Main_Program.Draw_last += Drawing;
                    Drag = true;
            }

        }
        public void Mouse_up(object sender, MouseEventArgs e)
        {
            if (Drag)
            {
                if (Draw)
                    //Main_Program.Draw_last -= Drawing;
                    Drag = false;
            }
        }
        public void Mouse_move(object sender, MouseEventArgs e)
        {
            Mouse_point = e.Location;
            if (Drag)
            {
                if (e.X > first_click.X)
                {
                    rectangle.X = first_click.X;
                    rectangle.Width = e.X - rectangle.X;
                }
                else
                {
                    rectangle.X = e.X;
                    rectangle.Width = first_click.X - e.X;
                }

                if (e.Y > first_click.Y)
                {
                    rectangle.Y = first_click.Y;
                    rectangle.Height = e.Y - rectangle.Y;
                }
                else
                {
                    rectangle.Y = e.Y;
                    rectangle.Height = first_click.Y - e.Y;
                }

            }
        }

        private void Drawing()
        {
            DoubleBuffering.getinstance().getGraphics.FillRectangle(thisbrush, rectangle);
            DoubleBuffering.getinstance().getGraphics.DrawRectangle(greenpen, rectangle);
        }
    }


    public class Key_input
    {
        static Key_input oInstance = null;

        private bool Shift = false;

        private bool Up = false;
        private bool Down = false;
        private bool Left = false;
        private bool Right = false;
        private bool Space = false;


        public bool get_shift { get { return Shift; } }

        public bool get_up { get { return Up; } }
        public bool get_down { get { return Down; } }
        public bool get_left { get { return Left; } }
        public bool get_right { get { return Right; } }
        public bool get_space { get { return Space; } }

        //키보드로 입력이 들어가면 Key_in 이벤트 발생
        //이 이벤트에 연결하여 키보드입력을 받을 수 있음
        public delegate void key_down(Keys keys);
        public static event key_down Key_in;

        public static Key_input getinstance()
        {
            if(oInstance == null)
            {
                throw new Exception("Form_input.binding() 필요");
            }
            else
            {
                return oInstance;
            }
        }

        internal Key_input() {
            oInstance = this;
        }

        //가끔씩 화살표 버튼들이 입력으로 안되는 경우가 있어서 이게 필요
        public void Key_Preview(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        public void Key_down(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Up = true;
                    Down = false;
                    break;

                case Keys.Down:
                    Down = true;
                    Up = false;
                    break;

                case Keys.Left:
                    Left = true;
                    Right = false;
                    break;

                case Keys.Right:
                    Right = true;
                    Left = false;
                    break;

                case Keys.Space:
                    Space = true;
                    break;

                case Keys.Shift:
                    Shift = true;
                    break;
            }
            if(Key_in != null)
                Key_in(e.KeyCode);

        }

        public void Key_up(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Up = false;
                    break;

                case Keys.Down:
                    Down = false;
                    break;

                case Keys.Left:
                    Left = false;
                    break;

                case Keys.Right:
                    Right = false;
                    break;

                case Keys.Space:
                    Space = false;
                    break;

                case Keys.Shift:
                    Shift = false;
                    break;
            }
        }
    }

}
