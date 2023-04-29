using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Cocktails
{
    public abstract class Cocktail : ICocktail
    {
        private string name;
        private string size;
        private double price;

        protected Cocktail(string cocktailName, string size, double price)
        {
            Name = cocktailName;
            Size = size;
            Price = price;
        }

        public string Name
        {
            get { return name; }
            private set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(String.Format(ExceptionMessages.NameNullOrWhitespace));
                }
                name = value;
            }
        }


        public string Size
        {
            get { return size; }
            private set { size = value; }
        }


        public double Price
        {
            get { return price; }
            private set
            {
                if (Size == "Middle")
                {
                    value = 2 * (value / 3);
                }
                else if (Size == "Small")
                {
                    value = value / 3;
                }
                price = value;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Size}) - {Price:f2} lv";
        }

    }
}
