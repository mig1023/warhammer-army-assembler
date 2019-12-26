using System.Collections.Generic;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public class ArmyBook
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
        public static Dictionary<int, Unit> Mounts = new Dictionary<int, Unit>();
        public static Dictionary<int, Option> Artefact = new Dictionary<int, Option>();

        public static int MaxIDindex = 0;

        public static Brush MainColor = null;
        public static Brush AdditionalColor = null;
        public static Brush BackgroundColor = null;
    }
}
