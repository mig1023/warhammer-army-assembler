using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Interface
{
    public class SizeList : List<string>
    {
        public SizeList()
        {
            this.Add("10");
            this.Add("16");
            this.Add("20");
            this.Add("24");
            this.Add("26");
            this.Add("30");
        }
    }
}
