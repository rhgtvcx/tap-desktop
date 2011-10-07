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
using TheAirlineV2.Model.AirlineModel;
using TheAirlineV2.GraphicsModel.PageModel.GeneralModel;
using TheAirlineV2.Model.GeneralModel;
using System.Globalization;
using System.Threading;
using TheAirlineV2.GraphicsModel.UserControlModel.PopUpWindowsModel;
using TheAirlineV2.GraphicsModel.UserControlModel.MessageBoxModel;
using System.Windows.Markup;

namespace TheAirlineV2.GraphicsModel.PageModel.PageAirlineModel.PanelAirlineModel
{
    /// <summary>
    /// Interaction logic for PageAirlineFinances.xaml
    /// </summary>
    public partial class PageAirlineFinances : Page
    {
        private Airline Airline;
        private TextBlock txtCurrentMoney, txtBalance;
        private ListBox lbSpecifications, lbLoans;
        public PageAirlineFinances(Airline airline)
        {
            this.Language = XmlLanguage.GetLanguage(new CultureInfo(GameObject.GetInstance().getLanguage().CultureInfo, true).IetfLanguageTag); 

            this.Airline = airline;
            
            InitializeComponent();

            StackPanel panelFinances = new StackPanel();
            panelFinances.Margin = new Thickness(0, 10, 50, 0);

            TextBlock txtHeader = new TextBlock();
            txtHeader.Margin = new Thickness(0, 0, 0, 0);
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Airline Finances";
            panelFinances.Children.Add(txtHeader);

            TextBlock txtSummary  =new TextBlock();
            txtSummary.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtSummary.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush");
            txtSummary.FontWeight = FontWeights.Bold;
            txtSummary.Text = "Financial Summary";
            panelFinances.Children.Add(txtSummary);

            ListBox lbSummary = new ListBox();
            lbSummary.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbSummary.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");
            panelFinances.Children.Add(lbSummary);

            txtCurrentMoney = UICreator.CreateTextBlock(string.Format("{0:c}", this.Airline.Money));
            txtCurrentMoney.Foreground = new Converters.ValueIsMinusConverter().Convert(this.Airline.Money, null, null, null) as Brush;            
          
            lbSummary.Items.Add(new QuickInfoValue("Current cash", txtCurrentMoney));

            txtBalance = UICreator.CreateTextBlock(string.Format("{0:c}", this.Airline.Money - GameObject.GetInstance().StartMoney));
            txtBalance.Foreground = new Converters.ValueIsMinusConverter().Convert(this.Airline.Money - GameObject.GetInstance().StartMoney, null, null, null) as Brush;
            lbSummary.Items.Add(new QuickInfoValue("Total balance", txtBalance));

            ContentControl txtSpecifications = new ContentControl();
            txtSpecifications.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtSpecifications.ContentTemplate = this.Resources["SpecsHeader"] as DataTemplate;
            txtSpecifications.Margin = new Thickness(0, 5, 0, 0);

            panelFinances.Children.Add(txtSpecifications);

            lbSpecifications = new ListBox();
            lbSpecifications.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbSpecifications.ItemTemplate = this.Resources["SpecsItem"] as DataTemplate;

            panelFinances.Children.Add(lbSpecifications);


            showSpecifications();

            StackPanel panelLoans = new StackPanel();
            panelLoans.Visibility = this.Airline.IsHuman ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            panelLoans.Margin = new Thickness(0,5,0,0);

            panelFinances.Children.Add(panelLoans);

            TextBlock txtLoans= new TextBlock();
            txtLoans.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtLoans.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtLoans.FontWeight = FontWeights.Bold;
            txtLoans.Text = "Loans";
            panelLoans.Children.Add(txtLoans);

            ContentControl ccLoans = new ContentControl();
            ccLoans.ContentTemplate = this.Resources["LoansHeader"] as DataTemplate;

            panelLoans.Children.Add(ccLoans);

            lbLoans = new ListBox();
            lbLoans.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbLoans.ItemTemplate = this.Resources["LoanItem"] as DataTemplate;

            panelLoans.Children.Add(lbLoans);

            Button btnLoan = new Button();
            btnLoan.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnLoan.Height = 20;
            btnLoan.Width = 100;
            //btnLoan.Visibility = this.Airline.IsHuman ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnLoan.Content = "Apply for loan";
            btnLoan.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            btnLoan.Margin = new Thickness(0, 10, 0, 0);
            btnLoan.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnLoan.Click += new RoutedEventHandler(btnLoan_Click);

            panelLoans.Children.Add(btnLoan);

            showLoans(false);

            
            this.Content = panelFinances;

            GameTimer.GetInstance().OnTimeChanged += new GameTimer.TimeChanged(PageAirlineFinances_OnTimeChanged);
        }

        private void btnLoan_Click(object sender, RoutedEventArgs e)
        {
            Loan loan = (Loan)PopUpLoan.ShowPopUp();

            if (loan != null)
            {
                this.Airline.addLoan(loan);

                this.Airline.addInvoice(new Invoice(loan.Date, Invoice.InvoiceType.Loans, loan.Amount));
            }
        }
        //shows the loans
        private void showLoans(Boolean forceShow)
        {
            if (this.Airline.Loans.Count != lbLoans.Items.Count || forceShow)
            {
                lbLoans.Items.Clear();

                foreach (Loan loan in this.Airline.Loans.FindAll((delegate(Loan l) { return l.IsActive; })))
                    lbLoans.Items.Add(loan);
            }
        }
        //shows the specifications
        private void showSpecifications()
        {
            lbSpecifications.Items.Clear();

            foreach (Invoice.InvoiceType type in Enum.GetValues(typeof(Invoice.InvoiceType)))
            {
                lbSpecifications.Items.Add(new SpecsItem(this.Airline, type));
              
            }
        }
        private void PageAirlineFinances_OnTimeChanged()
        {
           
          
            if (this.IsLoaded)
            {
                txtBalance.Text = string.Format("{0:c}", this.Airline.Money - GameObject.GetInstance().StartMoney);
                txtBalance.Foreground = new Converters.ValueIsMinusConverter().Convert(this.Airline.Money - GameObject.GetInstance().StartMoney, null, null, null) as Brush;

                txtCurrentMoney.Text = string.Format("{0:c}", this.Airline.Money);
                txtCurrentMoney.Foreground = new Converters.ValueIsMinusConverter().Convert(this.Airline.Money, null, null, null) as Brush;            
                showSpecifications();
                showLoans(false);
            }
        }
        private void btnPayLoan_Click(object sender, RoutedEventArgs e)
        {
            Loan loan = (Loan)((Button)sender).Tag;

            TextBox txtboxLoan=(TextBox)((Panel)((Button)sender).Parent).FindName("txtboxLoan");

            if (txtboxLoan.Text.Length > 0)
            {
                double amount = Double.Parse(txtboxLoan.Text);

                if (amount > loan.PaymentLeft || amount <= 0 || amount> this.Airline.Money)
                {
                    WPFMessageBox.Show("Invalid amount", "Please enter a valid amount to pay back on the loan", WPFMessageBoxButtons.Ok);
                }
                else
                {
                    WPFMessageBoxResult result = WPFMessageBox.Show("Pay on loan", string.Format("Are you sure you want to pay {0:c} on the loan", amount), WPFMessageBoxButtons.YesNo);

                    if (result == WPFMessageBoxResult.Yes)
                    {
                        this.Airline.addInvoice(new Invoice(GameObject.GetInstance().GameTime, Invoice.InvoiceType.Loans, Math.Max(-amount, -loan.PaymentLeft)));

                        loan.PaymentLeft -= Math.Min(amount, loan.PaymentLeft);

                        showLoans(true);
                    }
                }
            }
            
        }
        private void txtboxLoan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToDouble(e.Text);

                
            }
            catch
            {
                e.Handled = true;
            }
        }
        //the class for a category specification
        private class SpecsItem
        {
            public Invoice.InvoiceType InvoiceType { get; set; }
            public Airline Airline { get; set; }
            public double CurrentMonth { get { return getCurrentMonthTotal(); } set { ;} }
            public double LastMonth { get { return getLastMonthTotal(); } set { ;} }
            public double YearToDate { get { return getYearToDateTotal(); } set { ;} }
            public SpecsItem(Airline airline,Invoice.InvoiceType type)
            {
                this.InvoiceType = type;
                this.Airline = airline;
            }
            //returns the total amount for the current month
            public double getCurrentMonthTotal()
            {
                DateTime startDate = new DateTime(GameObject.GetInstance().GameTime.Year,GameObject.GetInstance().GameTime.Month,1);
                return this.Airline.getInvoicesAmount(startDate, GameObject.GetInstance().GameTime, this.InvoiceType);
            }
            //returns the total amount for the last month
            public double getLastMonthTotal()
            {
                DateTime tDate = GameObject.GetInstance().GameTime.AddMonths(-1);
              
                return this.Airline.getInvoicesAmountMonth(tDate.Month, this.InvoiceType);
        
            }
            //returns the total amount for the year to date
            public double getYearToDateTotal()
            {
                
                return  this.Airline.getInvoicesAmountYear(GameObject.GetInstance().GameTime.Year, this.InvoiceType);
                
            }
 
        }

    

       

      
      

        
    }
    
}
