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
using TheAirlineV2.Model.GeneralModel.StatisticsModel;
using TheAirlineV2.Model.AirlineModel;
using TheAirlineV2.GraphicsModel.PageModel.GeneralModel;
using TheAirlineV2.GraphicsModel.PageModel.PageAirlineModel;

namespace TheAirlineV2.GraphicsModel.PageModel.PageAirlinesModel.PanelAirlinesModel
{
    /// <summary>
    /// Interaction logic for PageAirlinesStatistics.xaml
    /// </summary>
    public partial class PageAirlinesStatistics : Page
    {
        private StackPanel panelStats;
        private int StatWidth = 200;
        public PageAirlinesStatistics()
        {

            InitializeComponent();

            StackPanel panelStatistics = new StackPanel();
            panelStatistics.Margin = new Thickness(0, 0, 50, 0);

            TextBlock txtHeader = new TextBlock();
            txtHeader.Margin = new Thickness(0, 0, 0, 0);
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Statistics";

            panelStatistics.Children.Add(txtHeader);

            panelStats = new StackPanel();


            panelStatistics.Children.Add(panelStats);

            GameTimer.GetInstance().OnTimeChanged += new GameTimer.TimeChanged(PageAirlinesStatistics_OnTimeChanged);

            this.Content = panelStatistics;

            showStats();
        }

        //shows the stats
        private void showStats()
        {
            panelStats.Children.Clear();

            // panelStats.Children.Add(createStatisticsPanel(StatisticsTypes.GetStatisticsType("Arrivals")));
            //panelStats.Children.Add(createHappinessPanel());
            panelStats.Children.Add(createStatisticsPanel(StatisticsTypes.GetStatisticsType("Departures")));
            panelStats.Children.Add(createStatisticsPanel(StatisticsTypes.GetStatisticsType("Passengers")));
            panelStats.Children.Add(createStatisticsPanel(StatisticsTypes.GetStatisticsType("Passengers%")));

        }
        private void PageAirlinesStatistics_OnTimeChanged()
        {
            if (this.IsLoaded)
                showStats();

        }
        private void LnkAirline_Click(object sender, RoutedEventArgs e)
        {
            Airline airline = (Airline)((Hyperlink)sender).Tag;

            PageNavigator.NavigateTo(new PageAirline(airline));


        }
        //creates the statistics for happiness
        private StackPanel createHappinessPanel()
        {
            StackPanel panelStatistics = new StackPanel();
            panelStatistics.Margin = new Thickness(0, 0, 0, 5);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Passenger happiness in %";

            panelStatistics.Children.Add(txtHeader);

            ListBox lbStatistics = new ListBox();
            lbStatistics.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            //lbStatistics.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");
            lbStatistics.ItemTemplate = this.Resources["AirlineStatItem"] as DataTemplate;

            panelStatistics.Children.Add(lbStatistics);

            List<Airline> airlines = Airlines.GetAirlines();
            airlines.Sort((delegate(Airline a1, Airline a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); }));


            foreach (Airline airline in airlines)
                lbStatistics.Items.Add(new AirlineStatisticsItem(airline, (int)PassengerHelpers.GetPassengersHappiness(airline), Convert.ToInt16(this.StatWidth * PassengerHelpers.GetPassengersHappiness(airline) / 100)));

            return panelStatistics;


        }
        //creates the airlines statistics
        private StackPanel createStatisticsPanel(StatisticsType type)
        {
            StackPanel panelStatistics = new StackPanel();
            panelStatistics.Margin = new Thickness(0, 0, 0, 5);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = type.Name;

            panelStatistics.Children.Add(txtHeader);

            ListBox lbStatistics = new ListBox();
            lbStatistics.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbStatistics.ItemTemplate = this.Resources["AirlineStatItem"] as DataTemplate;


            double maxValue = getMaxValue(type);

            double coff = this.StatWidth / maxValue;

            List<Airline> airlines = Airlines.GetAirlines();
            airlines.Sort((delegate(Airline a1, Airline a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); }));


            foreach (Airline airline in airlines)
                lbStatistics.Items.Add(new AirlineStatisticsItem(airline, airline.Statistics.getStatisticsValue(type), Math.Max(1, (int)Convert.ToDouble(airline.Statistics.getStatisticsValue(type) * coff))));


            panelStatistics.Children.Add(lbStatistics);

            return panelStatistics;

        }
        //finds the maximum value for a statistics type
        private double getMaxValue(StatisticsType type)
        {
            double value = 1;
            foreach (Airline airline in Airlines.GetAirlines())
            {
                int aValue = airline.Statistics.getStatisticsValue(type);
                if (aValue > value)
                    value = aValue;
            }

            return value;
        }
        private class AirlineStatisticsItem
        {
            public Airline Airline { get; set; }
            public int Value { get; set; }
            public int Width { get; set; }
            public AirlineStatisticsItem(Airline airline, int value, int Width)
            {
                this.Airline = airline;
                this.Value = value;
                this.Width = Width;
            }
        }
    }

}
