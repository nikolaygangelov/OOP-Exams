using PlanetWars.Models.Weapons.Contracts;
using PlanetWars.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetWars.Repositories
{
    public class WeaponRepository : IRepository<IWeapon>
    {
        private readonly List<IWeapon> models;

        public WeaponRepository()
        {
            this.models = new List<IWeapon>();
        }

        public IReadOnlyCollection<IWeapon> Models => models.AsReadOnly();
        public IWeapon FindByName(string name)
        {
            return models.FirstOrDefault(w => w.GetType().Name == name);
        }

        public void AddItem(IWeapon model)
        {
            models.Add(model);
        }


        public bool RemoveItem(string name)
        {
            return models.Remove(models.FirstOrDefault(w => w.GetType().Name == name));
        }
    }
}
