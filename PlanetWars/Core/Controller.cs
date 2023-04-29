using PlanetWars.Core.Contracts;
using PlanetWars.Models.MilitaryUnits;
using PlanetWars.Models.MilitaryUnits.Contracts;
using PlanetWars.Models.Planets;
using PlanetWars.Models.Planets.Contracts;
using PlanetWars.Models.Weapons;
using PlanetWars.Models.Weapons.Contracts;
using PlanetWars.Repositories;
using PlanetWars.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetWars.Core
{
    public class Controller : IController
    {
        private readonly PlanetRepository planets;
        public Controller()
        {
            planets = new PlanetRepository();
        }
        public string CreatePlanet(string name, double budget)
        {
            IPlanet planet = planets.FindByName(name);
            if (planet != null)
            {
                return String.Format(OutputMessages.ExistingPlanet, name);
            }

            Planet planet1 = new Planet(name, budget);
            planets.AddItem(planet1);
            return String.Format(OutputMessages.NewPlanet, name);

        }
        public string AddUnit(string unitTypeName, string planetName)
        {
            IPlanet planet = planets.FindByName(planetName);
            if (planet == null)
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.UnexistingPlanet, planetName));
            }
            IMilitaryUnit unit = planet.Army.FirstOrDefault(a => a.GetType().Name == unitTypeName);

            if (unit != null)
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.UnitAlreadyAdded, unitTypeName, planetName));
            }

            switch (unitTypeName)
            {
                case nameof(SpaceForces):
                    unit = new SpaceForces();
                    break;
                case nameof(StormTroopers):
                    unit = new StormTroopers();
                    break;
                case nameof(AnonymousImpactUnit):
                    unit = new AnonymousImpactUnit();
                    break;
                default:
                    throw new InvalidOperationException(String.Format(ExceptionMessages.ItemNotAvailable, unitTypeName));
            }

            if (planet.Budget >= unit.Cost)
            {
                planet.AddUnit(unit);
            }
            planet.Spend(unit.Cost);
            return String.Format(OutputMessages.UnitAdded, unitTypeName, planetName);
        }

        public string AddWeapon(string planetName, string weaponTypeName, int destructionLevel)
        {
            IPlanet planet = planets.FindByName(planetName);
            if (planet == null)
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.UnexistingPlanet, planetName));
            }

            IWeapon weapon = planet.Weapons.FirstOrDefault(a => a.GetType().Name == weaponTypeName);

            if (weapon != null)
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.WeaponAlreadyAdded, weaponTypeName, planetName));
            }

            switch (weaponTypeName)
            {
                case nameof(BioChemicalWeapon):
                    weapon = new BioChemicalWeapon(destructionLevel);
                    break;
                case nameof(NuclearWeapon):
                    weapon = new NuclearWeapon(destructionLevel);
                    break;
                case nameof(SpaceMissiles):
                    weapon = new SpaceMissiles(destructionLevel);
                    break;
                default:
                    throw new InvalidOperationException(String.Format(ExceptionMessages.ItemNotAvailable, weaponTypeName));
            }

            if (planet.Budget >= weapon.Price)
            {
                planet.AddWeapon(weapon);
            }
            planet.Spend(weapon.Price);
            return String.Format(OutputMessages.WeaponAdded, planetName, weaponTypeName);
        }
        public string SpecializeForces(string planetName)
        {
            IPlanet planet = planets.Models.FirstOrDefault(p => p.Name == planetName);
            if (planet == null)
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.UnexistingPlanet, planetName));
            }

            if (planet.Army.Count == 0)
            {
                throw new InvalidOperationException(ExceptionMessages.NoUnitsFound);
            }

            planet.TrainArmy();
            planet.Spend(1.25);
            return String.Format(OutputMessages.ForcesUpgraded, planetName);

        }
        public string SpaceCombat(string planetOne, string planetTwo)
        {
            IPlanet planet1 = planets.Models.FirstOrDefault(p => p.Name == planetOne);
            IPlanet planet2 = planets.Models.FirstOrDefault(p => p.Name == planetTwo);
            
            double sumOfMil1 = planet1.MilitaryPower;
            double sumOfMil2 = planet2.MilitaryPower;
            string winningPlanetName = "";
            string losingPlanetName = "";
            double sumOfCostAndPricesOfPlanetOne = planet1.Army.Sum(u => u.Cost) + planet1.Weapons.Sum(w => w.Price);
            double sumOfCostAndPricesOfPlanetTwo = planet2.Army.Sum(u => u.Cost) + planet2.Weapons.Sum(w => w.Price);

            if (planet1.MilitaryPower > planet2.MilitaryPower)
            {
                winningPlanetName = planet1.Name;               
            }
            else if (planet1.MilitaryPower < planet2.MilitaryPower)
            {
                winningPlanetName = planet2.Name;                
            }
            else
            {
                if (planet1.GetType().Name == nameof(NuclearWeapon) && planet2.GetType().Name != nameof(NuclearWeapon))
                {
                    winningPlanetName = planet1.Name;
                }
                else if (planet1.GetType().Name != nameof(NuclearWeapon) && planet2.GetType().Name == nameof(NuclearWeapon))
                {
                    winningPlanetName = planet2.Name;
                }
                else if ( (planet1.GetType().Name == nameof(NuclearWeapon) && planet2.GetType().Name == nameof(NuclearWeapon)) ||
                  (planet1.GetType().Name != nameof(NuclearWeapon) && planet2.GetType().Name != nameof(NuclearWeapon)) )
                {
                    planet1.Spend(planet1.Budget / 2);
                    planet2.Spend(planet2.Budget / 2);
                    return OutputMessages.NoWinner;
                }
            }
            if (planet1.Name == winningPlanetName)
            {
                losingPlanetName = planet2.Name;
                planet1.Spend(planet1.Budget / 2);
                planet1.Profit(planet2.Budget / 2);
                planet1.Profit(sumOfCostAndPricesOfPlanetTwo);
            }

            if (planet2.Name == winningPlanetName)
            {
                losingPlanetName = planet1.Name;
                planet2.Spend(planet2.Budget / 2);
                planet2.Profit(planet1.Budget / 2);
                planet2.Profit(sumOfCostAndPricesOfPlanetOne);             
            }

            planets.RemoveItem(losingPlanetName);
            return String.Format(OutputMessages.WinnigTheWar, winningPlanetName, losingPlanetName);
        }

        public string ForcesReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("***UNIVERSE PLANET MILITARY REPORT***");

            foreach (IPlanet planet in planets.Models.OrderByDescending(p => p.MilitaryPower).ThenBy(p => p.Name))
            {
                sb.AppendLine(planet.PlanetInfo());
            }

            return sb.ToString().TrimEnd();
        }

        public void Peace()
        {
            return;
        }
    }
}
