using BookingApp.Models.Rooms.Contracts;
using BookingApp.Repositories.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BookingApp.Repositories
{
    public class RoomRepository : IRepository<IRoom>
    {
        private readonly List<IRoom> rooms;
        public RoomRepository()
        {
            rooms = new List<IRoom>();
        }

        public void AddNew(IRoom model)
        {
            rooms.Add(model);
        }

        public IReadOnlyCollection<IRoom> All()
        {
            IReadOnlyCollection<IRoom> models = rooms.AsReadOnly();
            return models;
        }

        public IRoom Select(string roomTypeName)
        {
            return rooms.FirstOrDefault(r => r.GetType().Name == roomTypeName);
        }
    }
}
