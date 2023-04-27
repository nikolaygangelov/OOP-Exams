using Heroes.Models.Contracts;
using Heroes.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heroes.Repositories
{
    public class WeaponRepository : IRepository<IWeapon>
    {
        private readonly List<IWeapon> models;

        public WeaponRepository()
        {
            models = new List<IWeapon>();
        }
        public IReadOnlyCollection<IWeapon> Models => models.AsReadOnly();

        public void Add(IWeapon model)
        {
            models.Add(model);
        }

        public IWeapon FindByName(string name)
        {
            return models.FirstOrDefault(h => h.Name == name);
        }

        public bool Remove(IWeapon model)
        {
            return models.Remove(model);
        }
    }
}
