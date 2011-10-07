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
using TheAirlineV2.GraphicsModel.PageModel.GeneralModel;
using TheAirlineV2.Model.AirlineModel;
using TheAirlineV2.GraphicsModel.PageModel.PageAirlineModel;
using TheAirlineV2.GraphicsModel.PageModel.PageAirlinesModel.PanelAirlinesModel;

namespace TheAirlineV2.GraphicsModel.PageModel.PageAirlinesModel
{
    /// <summary>
    /// Interaction logic for PageAirlines.xaml
    /// </summary>
    public partial class PageAirlines : StandardPage
    {
        public PageAirlines()
        {
            InitializeComponent();

            StackPanel airlinesPanel = new StackPanel();
            airlinesPanel.Margin = new Thickness(10, 0, 10, 0);

            ContentControl txtHeader = new ContentControl();
            txtHeader.ContentTemplate = this.Resources["AirlinesHeader"] as DataTemplate;
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //txtHeader.SetResourceReference(Label.BackgroundProperty, "HeaderBackgroundBrush");

            airlinesPanel.Children.Add(txtHeader);


            ListBox lbAirlines = new ListBox();
            lbAirlines.ItemTemplate = this.Resources["AirlineItem"] as DataTemplate;
            lbAirlines.Height = 500;
            lbAirlines.ItemContainerStyleSelector = new ListBoxItemStyleSelector();

            List<Airline> airlines = Airlines.GetAirlines();
            airlines.Sort((delegate(Airline a1, Airline a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); }));

            foreach (Airline airline in airlines)
                lbAirlines.Items.Add(airline);

            airlinesPanel.Children.Add(lbAirlines);

            StandardContentPanel panelContent = new StandardContentPanel();

            panelContent.setContentPage(airlinesPanel, StandardContentPanel.ContentLocation.Left);


            StackPanel panelSideMenu = new PanelAirlines();

            panelContent.setContentPage(panelSideMenu, StandardContentPanel.ContentLocation.Right);



            base.setContent(panelContent);

            base.setHeaderContent("Airlines");


            showPage(this);
        }
        private void LnkAirline_Click(object sender, RoutedEventArgs e)
        {
            Airline airline = (Airline)((Hyperlink)sender).Tag;

            PageNavigator.NavigateTo(new PageAirline(airline));

            // PageNavigator.NavigateTo(new PagePlayerProfile(player));
        }
    }
}
