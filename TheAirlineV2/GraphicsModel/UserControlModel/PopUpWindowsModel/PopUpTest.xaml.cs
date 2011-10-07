﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TheAirlineV2.GraphicsModel.UserControlModel.PopUpWindowsModel
{
    /// <summary>
    /// Interaction logic for PopUpTest.xaml
    /// </summary>
    public partial class PopUpTest : Window
    {
        public static void ShowPopUp()
        {
            PopUpTest window = new PopUpTest();

            window.ShowDialog();
        }
        public PopUpTest()
        {
            //this.SetResourceReference(Window.BackgroundProperty, "HeaderBackgroundBrush2");

            //this.ResizeMode = System.Windows.ResizeMode.NoResize;

            //this.WindowStyle = System.Windows.WindowStyle.ToolWindow;

            InitializeComponent();
        }
    }
}
