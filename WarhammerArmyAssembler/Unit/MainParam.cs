using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    public class MainParam
    {
        public int Value { get; set; }

        public string View
        {
            get
            {
                return Value.ToString();
            }
        }

        public MainParam(int value)
        {
            this.Value = value;
        }
    }
}
