using PlanetWars.Models.MilitaryUnits.Contracts;
using PlanetWars.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace PlanetWars.Repositories
{
    public class UnitRepository : IRepository<IMilitaryUnit>
    {
        private readonly List<IMilitaryUnit> models;

        public UnitRepository()
        {
            this.models = new List<IMilitaryUnit>();
        }

        public IReadOnlyCollection<IMilitaryUnit> Models => models.AsReadOnly();
        public IMilitaryUnit FindByName(string name)
        {
            return models.FirstOrDefault(w => w.GetType().Name == name);
        }

        public void AddItem(IMilitaryUnit model)
        {
            models.Add(model);
        }


        public bool RemoveItem(string name)
        {
            return models.Remove(models.FirstOrDefault(w => w.GetType().Name == name));
        }
    }
}

