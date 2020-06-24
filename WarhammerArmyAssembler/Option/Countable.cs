using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    public class Countable
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Value { get; set; }

        public string Dependency { get; set; }
        public int Ratio { get; set; }

        public Countable Clone()
        {
            Countable newParam = new Countable
            {
                Min = this.Min,
                Max = this.Max,
                Value = this.Value,
                Dependency = this.Dependency,
                Ratio = this.Ratio,
            };

            return newParam;
        } 
    }
}
