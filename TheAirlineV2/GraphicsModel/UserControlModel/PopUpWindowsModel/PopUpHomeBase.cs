﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TheAirlineV2.Model.AirlinerModel;
using System.Windows.Controls;
using TheAirlineV2.Model.AirportModel;
using System.Windows.Media;

namespace TheAirlineV2.GraphicsModel.UserControlModel.PopUpWindowsModel
{
    public class PopUpHomeBase : PopUpWindow
    {
        private FleetAirliner Airliner;
        private ComboBox cbAirport;
        public static object ShowPopUp(FleetAirliner airliner)
        {
            PopUpWindow window = new PopUpHomeBase(airliner);
            window.ShowDialog();

            return window.Selected == null ? null : window.Selected;
        }
        public PopUpHomeBase(FleetAirliner airliner)
        {
            this.Airliner = airliner;

            this.Title = "Select Homebase";

            this.Width = 300;

            this.Height = 100;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            StackPanel mainPanel = new StackPanel();
            mainPanel.Margin = new Thickness(10, 10, 10, 10);
            cbAirport = new ComboBox();
            cbAirport.SetResourceReference(ComboBox.ItemTemplateProperty, "AirportCountryItem");
            //cbAirport.SetResourceReference(ComboBox.ItemTemplateProperty, "CountryFlagLongItem");
            cbAirport.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbAirport.IsSynchronizedWithCurrentItem = true;
            cbAirport.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            List<Airport> airports = this.Airliner.Airline.Airports.FindAll((delegate(Airport airport) { return airport.getAirportFacility(this.Airliner.Airline,AirportFacility.FacilityType.Service).TypeLevel>0; }));
            airports.Sort(delegate(Airport a1, Airport a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); });

            //vælg kun med basis service + sæt ved 

            foreach (Airport airport in airports)
                cbAirport.Items.Add(airport);

            cbAirport.SelectedItem = this.Airliner.Homebase;

            mainPanel.Children.Add(cbAirport);

            mainPanel.Children.Add(createButtonsPanel());

                

            this.Content = mainPanel;

            
        }
        //creates the button panel
        private WrapPanel createButtonsPanel()
        {
            WrapPanel panelButtons = new WrapPanel();
            panelButtons.Margin = new Thickness(0, 5, 0, 0);

            Button btnOk = new Button();
            btnOk.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnOk.Height = 20;
            btnOk.Width = 80;
            btnOk.Content = "OK";
            btnOk.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnOk.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            btnOk.Click += new RoutedEventHandler(btnOk_Click);
            panelButtons.Children.Add(btnOk);

            Button btnCancel = new Button();
            btnCancel.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnCancel.Height = 20;
            btnCancel.Width = 80;
            btnCancel.Content = "Cancel";
            btnCancel.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnCancel.Margin = new Thickness(5, 0, 0, 0);
            btnCancel.Click += new RoutedEventHandler(btnCancel_Click);

            panelButtons.Children.Add(btnCancel);

            return panelButtons;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Selected = null;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Selected = cbAirport.SelectedItem;
            this.Close();
        }

    }
}
