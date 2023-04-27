using Heroes.Models.Contracts;
using Heroes.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Heroes.Models
{
    public abstract class Hero : IHero
    {
        private string name;
        private int health;
        private int armour;
        private IWeapon weapon;

        public Hero(string name, int health, int armour)
        {
            Name = name;
            Health = health;
            Armour = armour;
        }
        public string Name
        {
            get { return name; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(ExceptionMessages.HeroNameNull);
                }
                name = value;
            }
        }

        public int Health
        {
            get { return health; }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ExceptionMessages.HeroHealthBelowZero);
                }
                health = value;
            }
        }

        public int Armour
        {
            get { return armour; }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ExceptionMessages.HeroArmourBelowZero);
                }
                armour = value;
            }
        }

        public bool IsAlive => Health > 0;

        public IWeapon Weapon
        {
            get { return weapon; }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentException(ExceptionMessages.WeaponNull);
                }
                weapon = value;
            }
        }


        public void AddWeapon(IWeapon weapon)
        {
            Weapon = weapon;
        }

        public void TakeDamage(int points)
        {
            int differrence = points - Armour;
            if (Armour <= points)
            {
                Armour = 0;
                if (Health <= differrence)
                {
                    Health = 0;
                }
                else
                {
                    Health -= differrence;
                }
            }
            else
            {
                Armour -= points;
                //if (Health <= points)
                //{
                //    Health = 0;
                //}
                //else
                //{
                //    Health -= points;
                //}
            }

            
        }
    }
}
