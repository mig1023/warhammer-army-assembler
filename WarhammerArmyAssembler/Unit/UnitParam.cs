using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    public class UnitParam
    {
        public int Value { get; set; }
        public string View { get; set; }
        public int Random { get; set; }
        public bool Empty { get; set; }

        public static UnitParam SetValue(int value, bool random = false, bool empty = false)
        {
            if (empty)
                return new UnitParam { Empty = true };
            else if (random)
                return new UnitParam { Random = value };
            else
                return new UnitParam { Value = value };
        }

        public void SetView(string view)
        {
            View = view;
        }

        public UnitParam Clone()
        {
            UnitParam newParam = new UnitParam();

            newParam.Value = this.Value;
            newParam.View = this.View;
            newParam.Random = this.Random;
            newParam.Empty = this.Empty;

            return newParam;
        }
    }
}
