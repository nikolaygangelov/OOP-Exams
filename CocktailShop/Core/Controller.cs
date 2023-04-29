using ChristmasPastryShop.Core.Contracts;
using ChristmasPastryShop.Models.Booths;
using ChristmasPastryShop.Models.Delicacies;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ChristmasPastryShop.Models.Booths.Contracts;
using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Models.Cocktails;

namespace ChristmasPastryShop.Core
{
    public class Controller : IController
    {
        private BoothRepository booths;
        public Controller()
        {
            booths = new BoothRepository();
        }
        public string AddBooth(int capacity)
        {
            int boothId = this.booths.Models.Count + 1;

            Booth booth = new Booth(boothId, capacity);
            booths.AddModel(booth);
            return String.Format(OutputMessages.NewBoothAdded, boothId, capacity);
        }
        public string AddDelicacy(int boothId, string delicacyTypeName, string delicacyName)
        {
            IBooth booth = booths.Models
               .FirstOrDefault(b => b.BoothId == boothId);
            IDelicacy delicacy = booth.DelicacyMenu.Models.FirstOrDefault(d => d.Name == delicacyName);

            if (delicacy != null)
            {
                return String.Format(OutputMessages.DelicacyAlreadyAdded, delicacyName);
            }

            switch (delicacyTypeName)
            {
                case nameof(Gingerbread):
                    delicacy = new Gingerbread(delicacyName);
                    break;
                case nameof(Stolen):
                    delicacy = new Stolen(delicacyName);
                    break;
                default:
                    return String.Format(OutputMessages.InvalidDelicacyType, delicacyTypeName);
            }

            booth.DelicacyMenu.AddModel(delicacy);
            return String.Format(OutputMessages.NewDelicacyAdded, delicacyTypeName, delicacyName);
        }

        public string AddCocktail(int boothId, string cocktailTypeName, string cocktailName, string size)
        {
            
            IBooth booth = booths.Models
               .FirstOrDefault(b => b.BoothId == boothId);
            ICocktail cocktail = booth.CocktailMenu.Models.FirstOrDefault(c => c.Name == cocktailName && c.Size == size);

            if (cocktail != null)
            {
                return String.Format(OutputMessages.CocktailAlreadyAdded, size, cocktailName);
            }

            if (size != "Small" && size != "Middle" && size != "Large")
            {
                return String.Format(OutputMessages.InvalidCocktailSize, size);
            }

            switch (cocktailTypeName)
            {
                case nameof(Hibernation):
                    cocktail = new Hibernation(cocktailName, size);
                    break;
                case nameof(MulledWine):
                    cocktail = new MulledWine(cocktailName, size);
                    break;
                default:
                    return String.Format(OutputMessages.InvalidCocktailType, cocktailTypeName);
            }

            booth.CocktailMenu.AddModel(cocktail);
            return String.Format(OutputMessages.NewCocktailAdded, size, cocktailName, cocktailTypeName);
        }

        public string ReserveBooth(int countOfPeople)
        {
            IBooth booth = booths.Models
                .OrderBy(b => b.Capacity)
                .ThenByDescending(b => b.BoothId)
                .FirstOrDefault(b => b.IsReserved == false && b.Capacity >= countOfPeople);
            if (booth == null)
            {
                return String.Format(OutputMessages.NoAvailableBooth, countOfPeople);
            }

            booth.ChangeStatus();
            return String.Format(OutputMessages.BoothReservedSuccessfully, booth.BoothId, countOfPeople);
        }

        public string TryOrder(int boothId, string order)
        {
            string[] ordersInfo = order.Split("/", StringSplitOptions.RemoveEmptyEntries);
            string itemTypeName = ordersInfo[0];
            string itemName = ordersInfo[1];
            string countOfPieces = ordersInfo[2];

            IBooth booth = booths.Models
               .FirstOrDefault(b => b.BoothId == boothId);//!!!!!!!!!!

            ICocktail cocktail = booth.CocktailMenu.Models
                .FirstOrDefault(c => c.Name == itemName && c.GetType().Name == itemTypeName);

            IDelicacy delicacy = booth.DelicacyMenu.Models
                .FirstOrDefault(c => c.Name == itemName && c.GetType().Name == itemTypeName);

            switch (itemTypeName)
            {
                case nameof(Hibernation):
                case nameof(MulledWine):
                    if (!booth.CocktailMenu.Models.Contains(cocktail))
                    {
                        return String.Format(OutputMessages.CocktailStillNotAdded, itemTypeName, itemName);
                    }

                    string sizeOfCocktail = ordersInfo[3];

                    if (cocktail.Size != sizeOfCocktail)
                    {
                        return String.Format(OutputMessages.NotRecognizedItemName, sizeOfCocktail, itemName);
                    }

                    double amountForCocktail = cocktail.Price * double.Parse(countOfPieces);
                    booth.UpdateCurrentBill(amountForCocktail);

                    return String.Format(OutputMessages.SuccessfullyOrdered, booth.BoothId, countOfPieces, itemName);
                case nameof(Gingerbread):
                case nameof(Stolen):
                    if (!booth.DelicacyMenu.Models.Contains(delicacy))
                    {
                        return String.Format(OutputMessages.DelicacyStillNotAdded, itemTypeName, itemName);
                    }

                    if (delicacy == null)
                    {
                        return String.Format(OutputMessages.NotRecognizedItemName, itemTypeName, itemName);
                    }

                    double amountForDelicacy = delicacy.Price * double.Parse(countOfPieces);
                    booth.UpdateCurrentBill(amountForDelicacy);

                    return String.Format(OutputMessages.SuccessfullyOrdered, booth.BoothId, countOfPieces, itemName);
                default:
                    return String.Format(OutputMessages.NotRecognizedType, itemTypeName);

            }
        }
        public string LeaveBooth(int boothId)
        {
            IBooth booth = booths
                .Models
                .FirstOrDefault(b => b.BoothId == boothId);

            booth.ChangeStatus();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Bill {booth.CurrentBill:f2} lv");
            sb.AppendLine($"Booth {boothId} is now available!");
            
            booth.Charge();

            return sb.ToString().TrimEnd();
        }

        public string BoothReport(int boothId)
        {
            return booths
                .Models
                .FirstOrDefault(b => b.BoothId == boothId).ToString();
        }



    }
}
