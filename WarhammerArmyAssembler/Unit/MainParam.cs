using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    public class MainParam
    {
        private Unit _Unit;
        private string _Name;

        public int Value { get; set; }
        public string View { get; set; }

        public MainParam(int value, ref Unit unit, string name)
        {
            this.Value = value;
            this._Unit = unit;
            this._Name = name;
        }
    }
}
