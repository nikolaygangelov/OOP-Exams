using Heroes.Core.Contracts;
using Heroes.Models;
using Heroes.Models.Contracts;
using Heroes.Repositories;
using Heroes.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heroes.Core
{
    public class Controller : IController
    {
        private HeroRepository heroes;
        private WeaponRepository weapons;

        public Controller()
        {
            heroes = new HeroRepository();
            weapons = new WeaponRepository();
        }
        public string CreateHero(string type, string name, int health, int armour)
        {
            IHero hero = heroes.FindByName(name);

            if (hero != null)
            {
                throw new InvalidOperationException(String.Format(OutputMessages.HeroAlreadyExist));
            }

            switch (type)
            {
                case nameof(Knight):
                    hero = new Knight(name, health, armour);
                    break;
                case nameof(Barbarian):
                    hero = new Barbarian(name, health, armour);
                    break;
                default:
                    throw new InvalidOperationException(OutputMessages.HeroTypeIsInvalid);
            }

            heroes.Add(hero);

            if (hero is Knight)
            {
                return String.Format(OutputMessages.SuccessfullyAddedKnight, name);
            }

            return String.Format(OutputMessages.SuccessfullyAddedBarbarian, name);

        }

        public string AddWeaponToHero(string weaponName, string heroName)
        {
            IHero hero = heroes.FindByName(heroName);

            if (hero == null)
            {
                throw new InvalidOperationException(String.Format(OutputMessages.HeroDoesNotExist, heroName));
            }

            IWeapon weapon = weapons.FindByName(weaponName);

            if (weapon == null)
            {
                throw new InvalidOperationException(String.Format(OutputMessages.WeaponDoesNotExist, weaponName));
            }

            if (hero.Weapon != null)
            {
                throw new InvalidOperationException(String.Format(OutputMessages.HeroAlreadyHasWeapon, heroName));
            }

            hero.AddWeapon(weapon);
            weapons.Remove(weapon);

            return String.Format(OutputMessages.WeaponAddedToHero, heroName, weapon.GetType().Name.ToLower());
        }


        public string CreateWeapon(string type, string name, int durability)
        {

            IWeapon weapon = weapons.FindByName(name);

            if (weapon != null)
            {
                throw new InvalidOperationException(String.Format(OutputMessages.WeaponAlreadyExists, name));
            }

            switch (type)
            {
                case nameof(Claymore):
                    weapon = new Claymore(name, durability);
                    break;
                case nameof(Mace):
                    weapon = new Mace(name, durability);
                    break;
                default:
                    throw new InvalidOperationException(OutputMessages.WeaponTypeIsInvalid);
            }

            weapons.Add(weapon);

            if (weapon is Claymore)
            {
                return String.Format(OutputMessages.WeaponAddedSuccessfully, type.ToLower(), name);
            }

            return String.Format(OutputMessages.WeaponAddedSuccessfully, type.ToLower(), name);


        }

        public string HeroReport()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var hero in heroes.Models
                .OrderBy(h => h.GetType().Name)
                .ThenByDescending(h => h.Health)
                .ThenBy(h => h.Name)
                 )
            {
                sb.AppendLine($"{ hero.GetType().Name}: { hero.Name }");
                sb.AppendLine($"--Health: { hero.Health }");
                sb.AppendLine($"--Armour: { hero.Armour }");

                if (hero.Weapon == null)
                {
                    sb.AppendLine("--Weapon: Unarmed");
                }
                else
                {
                    sb.AppendLine($"--Weapon: { hero.Weapon.Name }");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public string StartBattle()
        {
            Map map = new Map();

            return map.Fight(heroes.Models.Where(h => h.IsAlive == true && h.Weapon != null).ToList());
        }
    }
}
