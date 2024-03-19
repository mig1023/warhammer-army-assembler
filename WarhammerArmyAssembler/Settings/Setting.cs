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

        public Types? _type;
        public Types? Type
        {
            get => _type == null ? Types.checkbox : _type;
            set { _type = value; }
        }

        private string _default;
        public string Default
        {
            get => String.IsNullOrEmpty(_default) ? "True" : _default;
            set { _default = value; }
        }
    }
}
