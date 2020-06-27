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

        private int _Value;
        public int Value
        {
            set
            {
                _Value = value;
            }

            get
            {
                return _Value;
            }
        }

        public string View
        {
            get
            {
                return AddFromAnyOption(_Name);
            }
        }

        public MainParam(int value, Unit unit, string name)
        {
            this.Value = value;
            this._Unit = unit;
            this._Name = name;
        }

        private string AddFromAnyOption(string name)
        {
            bool reversParam = ((name == "Armour" || name == "Ward") ? true : false);
            bool mountParam = (name == "Armour" ? true : false);
            bool doNotCombine = (name == "Ward" ? true : false);

            int paramValue = Value;

            if (paramValue == 16)
                return "D6";
            else if ((paramValue > 10) && ((paramValue % 6) == 0))
                return ((int)(paramValue / 6)).ToString() + "D6";
            else if (paramValue < 0)
                return "-";

            string paramModView = String.Empty;

            List<Option> allOption = new List<Option>(_Unit.Options);

            if (mountParam && (_Unit.MountOn > 0))
                allOption.AddRange(Army.Data.Units[_Unit.MountOn].Options);

            bool alreadyArmour = false;
            bool alreadyShield = false;

            foreach (Option option in allOption)
            {
                if (!option.IsActual())
                    continue;

                if (_Unit.OptionTypeAlreadyUsed(option, ref alreadyArmour, ref alreadyShield))
                    continue;

                PropertyInfo optionToParam = typeof(Option).GetProperty(String.Format("{0}To", name));
                if (optionToParam != null)
                {
                    object optionToObject = optionToParam.GetValue(option);
                    int optionToValue = (int)optionToObject;

                    if (optionToValue > 0)
                        return optionToValue.ToString() + (reversParam ? "+" : "*");
                }

                PropertyInfo optionParam = typeof(Option).GetProperty(String.Format("AddTo{0}", name));
                object optionObject = optionParam.GetValue(option);
                int optionValue = (int)optionObject;

                if (optionValue > 0 && reversParam)
                {
                    if (paramValue == null)
                        paramValue = 7;

                    if (doNotCombine)
                    {
                        if (optionValue < paramValue)
                            paramValue = optionValue;
                    }
                    else
                        paramValue -= (7 - optionValue);
                }
                else if (optionValue > 0)
                {
                    paramModView += '*';
                    paramValue += optionValue;
                    paramValue = Unit.ParamNormalization(paramValue);
                }
            }

            if (reversParam && (paramValue != null))
                paramModView += '+';

            return paramValue.ToString() + paramModView;
        }
    }
}
