﻿using System;
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
        public Form1()
        {
            InitializeComponent();

            WinFormlib.DoubleBuffering.setInstance(this);
            Start();
            
            //브랜치 테스트
        }

        public static void Start()
        {
            MessageBox.Show("시작했어요!!");
        }
    }
}
