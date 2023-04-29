using PlanetWars.Models.MilitaryUnits.Contracts;
using PlanetWars.Models.Planets.Contracts;
using PlanetWars.Models.Weapons.Contracts;
using PlanetWars.Repositories;
using PlanetWars.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PlanetWars.Models.MilitaryUnits;
using PlanetWars.Models.Weapons;

namespace PlanetWars.Models.Planets
{
    public class Planet : IPlanet
    {
        private string name;
        private double budget;
        private readonly UnitRepository units;
        private readonly WeaponRepository weapons;

        public Planet(string name, double budget)
        {
            Name = name;
            Budget = budget;
            units = new UnitRepository();
            weapons = new WeaponRepository();
        }

        public string Name
        {
            get { return name; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(ExceptionMessages.InvalidPlanetName);
                }
                name = value;
            }
        }


        public double Budget
        {
            get { return budget; }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ExceptionMessages.InvalidBudgetAmount);
                }
                budget = value;
            }
        }

        public double MilitaryPower
        {
            get
            {
                double result = units.Models.Sum(u => u.EnduranceLevel) + weapons.Models.Sum(d => d.DestructionLevel);
                if (units.Models.Any(u => u.GetType().Name == nameof(AnonymousImpactUnit)))
                {
                    result *= 1.3;
                }
                if (weapons.Models.Any(w => w.GetType().Name == nameof(NuclearWeapon)))
                {
                    result *= 1.45;
                }
                double militaryPower = Math.Round(result, 3);
                return militaryPower;
            }
        }


        public IReadOnlyCollection<IMilitaryUnit> Army => units.Models;

        public IReadOnlyCollection<IWeapon> Weapons => weapons.Models;

        public void AddUnit(IMilitaryUnit unit)
        {
            units.AddItem(unit);
        }

        public void AddWeapon(IWeapon weapon)
        {
            weapons.AddItem(weapon);
        }
        public void TrainArmy()
        {
            foreach (var unit in units.Models)
            {
                unit.IncreaseEndurance();
            }
        }
        public void Spend(double amount)
        {
            if (Budget < amount)
            {
                throw new InvalidOperationException(ExceptionMessages.UnsufficientBudget);
            }
            Budget -= amount;
        }
        public void Profit(double amount)
        {
            Budget += amount;
        }

        public string PlanetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Planet: {Name}");
            sb.AppendLine($"--Budget: {Budget} billion QUID");

            if (units.Models.Count > 0)
            {
                var stringUnits = new Queue<string>();

                foreach (var item in Army)
                {
                    stringUnits.Enqueue(item.GetType().Name);
                }
                sb.AppendLine($"--Forces: {string.Join(", ", stringUnits)}");
            }
            else
            {
                sb.AppendLine("--Forces: No units");
            }

            if (weapons.Models.Count > 0)
            {
                var equipment = new Queue<string>();

                foreach (var item in Weapons)
                {
                    equipment.Enqueue(item.GetType().Name);
                }
                sb.AppendLine($"--Combat equipment: {string.Join(", ", equipment)}");
            }
            else
            {
                sb.AppendLine("--Combat equipment: No weapons");
            }

            sb.AppendLine($"--Military Power: {MilitaryPower}");

            return sb.ToString().TrimEnd();

        }


    }
}

