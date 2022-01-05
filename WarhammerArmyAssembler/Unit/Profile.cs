using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public class Profile
    {
        public int Value { get; set; }

        public int Original { get; set; }

        public string View { get; set; }

        public SolidColorBrush Color { get; set; }

        public bool Null { get; set; }

        public Profile Clone() => new Profile
        {
            Value = this.Value,
            Original = this.Original,
            View = this.View,
            Color = this.Color,
            Null = this.Null,
        };
    }
}
