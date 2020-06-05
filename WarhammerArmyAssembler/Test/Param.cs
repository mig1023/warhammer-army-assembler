using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Test
{
    public class Param
    {
        public enum TestType { Pass, Wound, Death };
        public enum ContextType { Round, Hit, Wound, ArmourSave, WardSave };
        public enum RepeatType { Normal, Once };

        public string Type { get; set; }
        public TestType Bet { get; set; }
        public ContextType Context { get; set; }
        public RepeatType Repeat { get; set; }

        public bool UsedAlready { get; set; }


        public static List<Param> Clone(List<Param> allParams)
        {
            if (allParams == null)
                return new List<Param> { };

            List<Param> newParams = new List<Param>();

            foreach(Param param in allParams)
            {
                Param newParamTest = new Param
                {
                    Type = param.Type,
                    Bet = param.Bet,
                    Context = param.Context,
                    Repeat = param.Repeat,
                    UsedAlready = false,
                };

                newParams.Add(newParamTest);
            }

            return newParams;
        }

        public static void Tests(ref Unit unit, Unit opponent, ContextType context)
        {
            foreach (Param param in opponent.ParamTests)
            {
                bool canBeApplied = (param.Repeat == RepeatType.Normal) || ((param.Repeat == RepeatType.Once) && !param.UsedAlready);

                if ((param.Context == context) && canBeApplied)
                {
                    ParamTest(ref unit, param.Type, opponent, param.Bet, context);

                    if (param.Repeat == RepeatType.Once)
                        param.UsedAlready = true;
                }
            }
        }

        private static void ParamTest(ref Unit unit, string param, Unit opponent, TestType test, ContextType context)
        {
            bool roundFormat = ((context == ContextType.Hit) || (context == ContextType.Wound));

            Test.Data.Console(Test.Data.text, (roundFormat ? " --> " : "\n\n") + "{0} must pass {1} test ", unit.Name, param);

            int paramValue = (int)typeof(Unit).GetProperty(param).GetValue(unit);
            int diceNum = ((param == "Leadership") ? 2 : 1);

            if (Test.Dice.Roll(unit, param, opponent, paramValue, diceNum, paramTest: true))
                Test.Data.Console(Test.Data.goodText, " --> passed");
            else
                switch (test)
                {
                    case TestType.Pass:
                        Test.Data.Console(Test.Data.badText, " --> pass this round");
                        unit.PassThisRound = true;
                        break;
                    case TestType.Wound:
                        Test.Data.Console(Test.Data.badText, " --> WOUND");
                        unit.Wounds -= 1;
                        break;
                    case TestType.Death:
                        Test.Data.Console(Test.Data.badText, " --> SLAIN");
                        unit.Wounds = 0;
                        break;
                }
        }
    }
}
