using ChristmasPastryShop.Models.Booths.Contracts;
using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using ChristmasPastryShop.Repositories.Contracts;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Booths
{
    public class Booth : IBooth
    {
        private int boothId;
        private int capacity;
        private readonly IRepository<IDelicacy> delicacies;
        private readonly IRepository<ICocktail> cocktails;
        private double currentBill;
        private double turnover;

        public Booth(int boothId, int capacity)
        {
            BoothId = boothId;
            Capacity = capacity;
            delicacies = new DelicacyRepository();
            cocktails = new CocktailRepository();
            turnover = 0;
            currentBill = 0;
        }

        public int BoothId
        {
            get { return boothId; }
            private set { boothId = value; }
        }

        public int Capacity
        {
            get { return capacity; }
            private set
            {
                if (value < 1)
                {
                    throw new ArgumentException(String.Format(ExceptionMessages.CapacityLessThanOne));
                }
                capacity = value;
            }
        }


        public IRepository<IDelicacy> DelicacyMenu
        {
            get { return delicacies; }
        }

        public IRepository<ICocktail> CocktailMenu
        {
            get { return cocktails; }
        }

        public double CurrentBill
        {
            get { return currentBill; }
        }

        public double Turnover
        {
            get { return turnover; }
        }

        public bool IsReserved { get; private set; }

        public void UpdateCurrentBill(double amount)
        {
            currentBill += amount;
        }
        public void Charge()
        {
            turnover += currentBill;
            currentBill = 0;
        }

        public void ChangeStatus()
        {
            if (IsReserved)
            {
                IsReserved = false;
                return;
            }
            
                IsReserved = true;
            
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Booth: {boothId}");
            sb.AppendLine($"Capacity: {Capacity}");
            sb.AppendLine($"Turnover: {Turnover:f2} lv");
            sb.AppendLine("-Cocktail menu:");
              
            foreach (ICocktail cocktail in CocktailMenu.Models)
            {
                sb.AppendLine($"--{cocktail}");
            }

            sb.AppendLine("-Delicacy menu:");
            foreach (IDelicacy delicacy in DelicacyMenu.Models)
            {
                sb.AppendLine($"--{delicacy}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
