using Heroes.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Heroes.Models
{
    public class Mace : Weapon
    {
        public Mace(string name, int durability) : 
            base(name, durability)
        {
        }

        public override int DoDamage()
        {
            int damage = 25;
           
            return damage;
        }
    }
}
