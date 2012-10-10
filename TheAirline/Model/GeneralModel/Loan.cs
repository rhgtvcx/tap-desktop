﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheAirline.Model.GeneralModel
{
    //the class for a loan
    public class Loan
    {
        public double Amount { get; set; }
        public double Rate { get; set; }
        public int Length { get; set; }
        public DateTime Date { get; set; }
        public double PaymentLeft { get; set; }
        public Boolean IsActive { get { return hasPaymentLeft(); } set { ;} }
        public double MonthlyPayment { get { return getMonthlyPayment(); } set { ;} }
        public int MonthsLeft { get { return getMonthsLeft(); } set { ;} }
        public Loan(DateTime date, double amount, int length, double rate)
        {
            this.Amount = amount;
            this.Rate = rate;
            this.Length = length;
            this.Date = date;
            this.PaymentLeft = amount;
            
        }
        //returns the monthly payment for the loan
       
        public double getMonthlyPayment()
        {
           double basePayment = MathHelpers.GetMonthlyPayment(this.Amount, this.Rate, this.Length);
            {
                if (GameObject.GetInstance().Difficulty == GameObject.DifficultyLevel.Easy)
                { return (basePayment * 0.75); }
                if (GameObject.GetInstance().Difficulty == GameObject.DifficultyLevel.Normal)
                { return (basePayment * 1.0); }
                if (GameObject.GetInstance().Difficulty == GameObject.DifficultyLevel.Hard)
                { return (basePayment * 1.25); }
                else
                { return MathHelpers.GetMonthlyPayment(this.Amount, this.Rate, this.Length); }
            }
        }
        //checks if there is still payment left on the loan
        private Boolean hasPaymentLeft()
        {
            return this.PaymentLeft > 0;
        }
        //returns the amount of months left on the loan
        private int getMonthsLeft()
        {
            return (int)Math.Ceiling(this.PaymentLeft / this.MonthlyPayment);
        }
    }
}
