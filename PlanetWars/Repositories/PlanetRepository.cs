using PlanetWars.Models.Planets.Contracts;
using PlanetWars.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace PlanetWars.Repositories
{
    public class PlanetRepository : IRepository<IPlanet>
    {
        private readonly List<IPlanet> models;

        public PlanetRepository()
        {
            this.models = new List<IPlanet>();
        }

        public IReadOnlyCollection<IPlanet> Models => models.AsReadOnly();
        public IPlanet FindByName(string name)
        {
            return models.FirstOrDefault(w => w.Name == name);
        }

        public void AddItem(IPlanet model)
        {
            models.Add(model);
        }


        public bool RemoveItem(string name)
        {
            return models.Remove(models.FirstOrDefault(w => w.Name == name));
        }
    }
}
