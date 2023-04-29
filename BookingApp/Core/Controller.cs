using BookingApp.Core.Contracts;
using BookingApp.Models.Hotels;
using BookingApp.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BookingApp.Models.Hotels.Contacts;
using BookingApp.Utilities.Messages;
using BookingApp.Models.Rooms.Contracts;
using BookingApp.Models.Bookings.Contracts;
using BookingApp.Models.Bookings;
using BookingApp.Repositories.Contracts;
using BookingApp.Models.Rooms;

namespace BookingApp.Core
{
    public class Controller : IController
    {
        private readonly IRepository<IHotel> hotels;

        public Controller()
        {
            hotels = new HotelRepository();
        }


        public string AddHotel(string hotelName, int category)
        {
            IHotel hotel = hotels.Select(hotelName);
            if (hotel != null)
            {
                return String.Format(OutputMessages.HotelAlreadyRegistered, hotelName);
            }

            hotel = new Hotel(hotelName, category);
            hotels.AddNew(hotel);
            return String.Format(OutputMessages.HotelSuccessfullyRegistered, category, hotelName);
        }
        public string UploadRoomTypes(string hotelName, string roomTypeName)
        {
            IHotel hotel = hotels.Select(hotelName);
            if (hotel == null)
            {
                return String.Format(OutputMessages.HotelNameInvalid, hotelName);
            }
            IRoom room = hotel.Rooms.Select(roomTypeName);
            if (room != null)
            {
                String.Format(OutputMessages.RoomTypeAlreadyCreated);
            }

            switch (roomTypeName)
            {
                case nameof(DoubleBed):
                    room = new DoubleBed();
                    break;
                case nameof(Studio):
                    room = new Studio();
                    break;
                case nameof(Apartment):
                    room = new Apartment();
                    break;
                default:
                    throw new ArgumentException(ExceptionMessages.RoomTypeIncorrect);
            }

            hotel.Rooms.AddNew(room);
            return String.Format(OutputMessages.RoomTypeAdded, roomTypeName, hotelName);
        }
        public string SetRoomPrices(string hotelName, string roomTypeName, double price)
        {
            string[] allowedCategories = new string[] { "Apartment", "Studio", "DoubleBed" };

            IHotel hotel = hotels.Select(hotelName);
            if (hotel == null)
            {
                return String.Format(OutputMessages.HotelNameInvalid, hotelName);
            }
            if (!allowedCategories.Contains(roomTypeName))
            {
                throw new ArgumentException(ExceptionMessages.RoomTypeIncorrect);
            }
            IRoom room = hotel.Rooms.Select(roomTypeName);
            if (room == null)
            {
                String.Format(OutputMessages.RoomTypeNotCreated);
            }
            if (room.PricePerNight > 0)
            {
                throw new InvalidOperationException(ExceptionMessages.CannotResetInitialPrice);
            }

            room.SetPrice(price);
            return String.Format(OutputMessages.PriceSetSuccessfully, roomTypeName, hotelName);
        }

        public string BookAvailableRoom(int adults, int children, int duration, int category)
        {
            if (!hotels.All().Any(h => h.Category == category))
            {
                String.Format(OutputMessages.CategoryInvalid, category);
            }

            var orderedHotels = hotels.All()
                .Where(h => h.Category == category)
                .OrderBy(h => h.Turnover)
                .ThenBy(h => h.FullName);
                

            foreach (IHotel hotel in orderedHotels)
            {

                IRoom room = hotel.Rooms
                    .All()
                    .Where(r => r.PricePerNight > 0)
                    .Where(r => r.BedCapacity >= adults + children)
                    .OrderBy(r => r.BedCapacity)
                    .FirstOrDefault();

                if (room != null)
                {

                    int bookingNumber = this.hotels.All().Sum(x => x.Bookings.All().Count) + 1;
                    IBooking booking = new Booking(room, duration, adults, children, bookingNumber);
                    hotel.Bookings.AddNew(booking);

                    return String.Format(OutputMessages.BookingSuccessful, bookingNumber, hotel.FullName);
                }

            }

            return String.Format(OutputMessages.RoomNotAppropriate);

        }

        public string HotelReport(string hotelName)
        {
            IHotel hotel = hotels.Select(hotelName);
            if (hotel == null)
            {
                return String.Format(OutputMessages.HotelNameInvalid, hotelName);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Hotel name: {hotel.FullName}");
            sb.AppendLine($"--{hotel.Category} star hotel");
            sb.AppendLine($"--Turnover: {hotel.Turnover:F2} $");
            sb.AppendLine($"--Bookings:");

            sb.AppendLine();

            if (hotel.Bookings.All().Count == 0)
            {
                sb.AppendLine("none");
            }
            else
            {

                foreach (IBooking booking in hotel.Bookings.All())
                {
                    sb.AppendLine($"{booking.BookingSummary()}");
                    sb.AppendLine();
                }
            }

            return sb.ToString().TrimEnd();

        }



    }
}
