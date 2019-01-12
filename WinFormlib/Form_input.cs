using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Paper_io_Client
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
    class Form_input
    {
        public static void binding(Form form)
        {

            form.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(Key_input.Key_Preview);
            form.KeyDown += new System.Windows.Forms.KeyEventHandler(Key_input.Key_down);
            form.KeyUp += new System.Windows.Forms.KeyEventHandler(Key_input.Key_up);
            form.MouseDown += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_down);
            form.MouseMove += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_move);
            form.MouseUp += new System.Windows.Forms.MouseEventHandler(Mouse_input.Mouse_up);
        }
    }
    
    static class Mouse_input
    {
        static bool Drag = false;
        static Point first_click = new Point();
        static Rectangle rectangle = new Rectangle();
        static Point Mouse_point = new Point();

        static Brush thisbrush = null;

        static Pen greenpen = new Pen(Color.Green, 0.02f);


        public static bool Draw { get; set; }

        static Mouse_input()
        {
            thisbrush = new SolidBrush(Color.FromArgb(50, Color.LightGreen));
            Draw = true;
        }

        public static bool Dragging { get { return Drag; } }
        public static Point point { get { return Mouse_point; } }

        public static void Mouse_down(object sender, MouseEventArgs e)
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
        public static void Mouse_up(object sender, MouseEventArgs e)
        {
            if (Drag)
            {
                if (Draw)
                    //Main_Program.Draw_last -= Drawing;
                    Drag = false;
            }
        }
        public static void Mouse_move(object sender, MouseEventArgs e)
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

        private static void Drawing()
        {
            DoubleBuffering.Instance().getGraphics.FillRectangle(thisbrush, rectangle);
            DoubleBuffering.Instance().getGraphics.DrawRectangle(greenpen, rectangle);
        }
    }


    static class Key_input
    {
        private static bool Shift = false;

        private static bool Up = false;
        private static bool Down = false;
        private static bool Left = false;
        private static bool Right = false;
        private static bool Space = false;


        public static bool get_shift { get { return Shift; } }

        public static bool get_up { get { return Up; } }
        public static bool get_down { get { return Down; } }
        public static bool get_left { get { return Left; } }
        public static bool get_right { get { return Right; } }
        public static bool get_space { get { return Space; } }

        public delegate void key_down(Keys keys);
        public static event key_down Key_in;

        public static void Key_Preview(object sender, PreviewKeyDownEventArgs e)
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

        public static void Key_down(object sender, KeyEventArgs e)
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

        public static void Key_up(object sender, KeyEventArgs e)
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
