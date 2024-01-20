using System;

namespace WarhammerArmyAssembler.Settings
{
    class Setting
    {
        public enum Types { checkbox, combobox, input }

        public string ID { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public string Options { get; set; }
        
        public Types Type { get; set; }

        public string Default { get; set; }
    }
}
