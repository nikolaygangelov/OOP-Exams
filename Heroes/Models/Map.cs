using Heroes.Models.Contracts;
using Heroes.Repositories;
using Heroes.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heroes.Models
{
    public class Map : IMap
    {
        private readonly List<IHero> knights;
        private readonly List<IHero> barbarians;


        public Map()
        {
            knights = new List<IHero>();
            barbarians = new List<IHero>();
        }
        public string Fight(ICollection<IHero> players)
        {

            foreach (IHero hero in players)
            {
                if (hero.GetType().Name == nameof(Knight))
                {
                    knights.Add(hero);
                }
                else if (hero.GetType().Name == nameof(Barbarian))
                {
                    barbarians.Add(hero);
                }
            }

            int sumOfKnightsHealth = knights.Sum(k => k.Health);
            int sumOfBarbarianssHealth = barbarians.Sum(b => b.Health);
            int fakeDurability = 0;
            while (sumOfBarbarianssHealth > 0 && sumOfKnightsHealth > 0)
            {
                foreach (IHero knight in knights)
                {
                    if (knight.IsAlive)
                    {
                        fakeDurability = knight.Weapon.Durability;
                        foreach (Hero barberian in barbarians)
                        {
                            fakeDurability--;

                            if (fakeDurability > 0)
                            {

                                barberian.TakeDamage(knight.Weapon.DoDamage());
                            }
                            else
                            {
                                barberian.TakeDamage(0);
                            }

                        }
                    }
                }
                int a = 0;
                foreach (IHero barberian in barbarians)
                {
                    if (barberian.IsAlive)
                    {
                        fakeDurability = barberian.Weapon.Durability;
                        foreach (Hero knight in knights)
                        {
                            fakeDurability--;

                            if (fakeDurability > 0)
                            {

                                knight.TakeDamage(barberian.Weapon.DoDamage());
                            }
                            else
                            {
                                knight.TakeDamage(0);
                            }

                        }
                    }
                }

                sumOfKnightsHealth = knights.Sum(k => k.Health);
                sumOfBarbarianssHealth = barbarians.Sum(b => b.Health);
            }

            if (sumOfBarbarianssHealth == 0)
            {
                return String.Format(OutputMessages.MapFightKnightsWin, knights.Where(k => k.IsAlive == false).Count());
            }
            else
            {
                return String.Format(OutputMessages.MapFigthBarbariansWin, barbarians.Where(b => b.Health == 0).Count());
            }


        }
    }
}
