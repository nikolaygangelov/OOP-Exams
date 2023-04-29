using BookingApp.Models.Bookings.Contracts;
using BookingApp.Models.Hotels.Contacts;
using BookingApp.Models.Rooms.Contracts;
using BookingApp.Repositories.Contracts;
using BookingApp.Utilities.Messages;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BookingApp.Repositories;

namespace BookingApp.Models.Hotels
{
    public class Hotel : IHotel
    {

        public Hotel(string fullName, int category)
        {
            FullName = fullName;
            Category = category;
            Rooms = new RoomRepository();
            Bookings = new BookingRepository();
        }


        private string fullName;

        public string FullName
        {
            get { return fullName; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(ExceptionMessages.HotelNameNullOrEmpty);
                }
                fullName = value;
            }
        }

        private int category;

        public int Category
        {
            get { return category; }
            private set
            {
                if (value < 1 || value > 5)
                {
                    throw new ArgumentException(ExceptionMessages.InvalidCategory);
                }
                category = value;
            }
        }


        public double Turnover
        {
            get { return Math.Round(Bookings.All().Sum(b => b.ResidenceDuration * b.Room.PricePerNight), 2); }
        }

        public IRepository<IRoom> Rooms { get; set; }
        public IRepository<IBooking> Bookings { get; set; }

    }
}
