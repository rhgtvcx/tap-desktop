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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheAirlineV2.Model.GeneralModel;
using TheAirlineV2.GraphicsModel.PageModel.GeneralModel;
using TheAirlineV2.GraphicsModel.PageModel.PageAirportsModel;
using TheAirlineV2.GraphicsModel.PageModel.PageAirlineModel;
using TheAirlineV2.GraphicsModel.PageModel.PageGameModel;
using System.Threading;
using System.Globalization;

namespace TheAirlineV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Frame frameMain;
   
        public MainWindow()
        {
            
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB", true);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB", true);

            Setup.SetupGame();

            PageNavigator.MainWindow = this;

            InitializeComponent();

          
            //this.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;



            Canvas mainPanel = new Canvas();

            frameMain = new Frame();
            //frameMain.Width = this.Width;
            //frameMain.Height = this.Height;
            frameMain.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            //frameMain.Navigate(new PageAirline(GameObject.GetInstance().HumanAirline));
            //frameMain.Navigate(new PageFrontMenu());
            frameMain.Navigate(new PageNewGame());

            Canvas.SetTop(frameMain, 0);
            Canvas.SetLeft(frameMain, 0);

            mainPanel.Children.Add(frameMain);

            this.Content = mainPanel;

           
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
        
          if (e.Key == Key.Escape)
                this.Close();
        }
        //clears the navigator
        public void clearNavigator()
        {
            frameMain.NavigationService.LoadCompleted += new LoadCompletedEventHandler(NavigationService_LoadCompleted);
 
            // Remove back entries
            while (frameMain.NavigationService.CanGoBack)
                frameMain.NavigationService.RemoveBackEntry();

           

         
        }
       
        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            frameMain.NavigationService.RemoveBackEntry();

            frameMain.NavigationService.LoadCompleted -= new LoadCompletedEventHandler(NavigationService_LoadCompleted);
 
        }
        //returns if navigator can go forward
        public Boolean canGoForward()
        {
            return frameMain.NavigationService.CanGoForward;
        }
        //returns if navigator can go back
        public Boolean canGoBack()
        {
            return frameMain.NavigationService.CanGoBack;
        }
        //navigates to a new page
        public void navigateTo(Page page)
        {
            frameMain.Navigate(page);

        }
        //moves the navigator forward
        public void navigateForward()
        {
            if (frameMain.NavigationService.CanGoForward) 
                frameMain.NavigationService.GoForward();
        }
        //moves the navigator back
        public void navigateBack()
        {
            if (frameMain.NavigationService.CanGoBack)
                frameMain.NavigationService.GoBack();
        }
        
    }
}
