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
using TheAirlineV2.GraphicsModel.SkinsModel;
using TheAirlineV2.Model.GeneralModel;

namespace TheAirlineV2.GraphicsModel.PageModel.GeneralModel
{
    /// <summary>
    /// Interaction logic for PageSettings.xaml
    /// </summary>
    public partial class PageSettings : StandardPage
    {
        private ComboBox cbSkin;
        private Slider slGameSpeed;
        private TextBlock txtGameSpeed;
        private ComboBox cbLanguage;
        private CheckBox cbMailOnLandings;
        public PageSettings()
        {
            InitializeComponent();

            StackPanel settingsPanel = new StackPanel();
            settingsPanel.Margin = new Thickness(10, 0, 10, 0);

            // airportPanel.Children.Add(createQuickInfoPanel());

            StandardContentPanel panelContent = new StandardContentPanel();

            panelContent.setContentPage(settingsPanel, StandardContentPanel.ContentLocation.Left);

            //Panel panelSideMenu = createSidePanel();



            // panelContent.setContentPage(panelSideMenu, StandardContentPanel.ContentLocation.Right);
            ListBox lbSettings = new ListBox();
            lbSettings.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbSettings.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");

            settingsPanel.Children.Add(lbSettings);

            WrapPanel panelSpeed = new WrapPanel();

            slGameSpeed = new Slider();
            slGameSpeed.Minimum = (int)GeneralHelpers.GameSpeedValue.Fastest;
            slGameSpeed.Maximum = (int)GeneralHelpers.GameSpeedValue.Slowest;
            slGameSpeed.Width = 100;
            slGameSpeed.IsDirectionReversed = true;
            slGameSpeed.TickFrequency = 150;
            slGameSpeed.IsSnapToTickEnabled = true;
            slGameSpeed.IsMoveToPointEnabled = true;
            slGameSpeed.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slGameSpeed_ValueChanged);

             panelSpeed.Children.Add(slGameSpeed);
            
            txtGameSpeed = UICreator.CreateTextBlock(GameTimer.GetInstance().GameSpeed.ToString());
            txtGameSpeed.Margin = new Thickness(5, 0, 0, 0);
            txtGameSpeed.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            panelSpeed.Children.Add(txtGameSpeed);

            slGameSpeed.Value = (int)GameTimer.GetInstance().GameSpeed;
            lbSettings.Items.Add(new QuickInfoValue("Game speed", panelSpeed));

            cbLanguage = new ComboBox();
            cbLanguage.Width = 200;
            cbLanguage.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbLanguage.SelectedValuePath = "Name";
            cbLanguage.DisplayMemberPath = "Name";
            foreach (Language language in Languages.GetLanguages())
                cbLanguage.Items.Add(language);

            cbLanguage.SelectedItem = GameObject.GetInstance().getLanguage();

            lbSettings.Items.Add(new QuickInfoValue("Language", cbLanguage));


            cbSkin = new ComboBox();
            cbSkin.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbSkin.Width = 200;
            cbSkin.SelectedValuePath = "Name";
            cbSkin.DisplayMemberPath = "Name";

            foreach (Skin skin in Skins.GetSkins())
                cbSkin.Items.Add(skin);

            cbSkin.SelectedItem = SkinObject.GetInstance().CurrentSkin;

            lbSettings.Items.Add(new QuickInfoValue("Skin", cbSkin));

            cbMailOnLandings = new CheckBox();
            cbMailOnLandings.IsChecked = GameObject.GetInstance().NewsBox.MailsOnLandings;
      
            lbSettings.Items.Add(new QuickInfoValue("Mail on landings", cbMailOnLandings));
            
            WrapPanel buttonsPanel = new WrapPanel();
            buttonsPanel.Margin = new Thickness(0, 10, 0, 0);
            settingsPanel.Children.Add(buttonsPanel);

            Button btnOk = new Button();
            btnOk.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnOk.Height = 20;
            btnOk.Width = 80;
            btnOk.Content = "OK";
            //btnOk.Click += new RoutedEventHandler(btnOk_Click);
            btnOk.Click += new RoutedEventHandler(btnOk_Click);
            btnOk.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");

            buttonsPanel.Children.Add(btnOk);

            Button btnUndo = new Button();
            btnUndo.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnUndo.Height = 20;
            btnUndo.Margin = new Thickness(5, 0, 0, 0);
            btnUndo.Width = 80;
            // btnCancel.Visibility = this.Route.Airliner.Airliner.Airline.IsHuman ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnUndo.Click += new RoutedEventHandler(btnUndo_Click);
            btnUndo.Content = "Undo";
            btnUndo.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");

            buttonsPanel.Children.Add(btnUndo);

            base.setContent(panelContent);

            base.setHeaderContent("Settings");

            

            showPage(this);
        }

        private void slGameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GeneralHelpers.GameSpeedValue speed = (GeneralHelpers.GameSpeedValue)Enum.ToObject(typeof(GeneralHelpers.GameSpeedValue), (int)slGameSpeed.Value);
            txtGameSpeed.Text = speed.ToString();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            cbSkin.SelectedItem = SkinObject.GetInstance().CurrentSkin;
            slGameSpeed.Value = (int)GameTimer.GetInstance().GameSpeed;
            cbLanguage.SelectedItem = GameObject.GetInstance().getLanguage();
            cbMailOnLandings.IsChecked = GameObject.GetInstance().NewsBox.MailsOnLandings;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Skin selectedSkin = (Skin)cbSkin.SelectedItem;

            SkinObject.GetInstance().setCurrentSkin(selectedSkin);
          //FindResource("BackgroundImage") = img;
          //  FindResource("Bac
           // x:Key="MyImageSource" UriSource="../Media/Image.png" 
           //SetResourceReference(BitmapImage.UriSourceProperty,new Uri(AppDomain.CurrentDomain.BaseDirectory + "data\\graphics\\background.jpg"));
            GeneralHelpers.GameSpeedValue speed = (GeneralHelpers.GameSpeedValue)Enum.ToObject(typeof(GeneralHelpers.GameSpeedValue), (int)slGameSpeed.Value);
            GameTimer.GetInstance().setGameSpeed(speed);

            Language language = (Language)cbLanguage.SelectedItem;
            GameObject.GetInstance().setLanguage(language);
            GameObject.GetInstance().NewsBox.MailsOnLandings = cbMailOnLandings.IsChecked.Value;

          PageNavigator.NavigateTo(new PageSettings()); 
         
        }
    }
}
