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
        public enum RepeatType { Round, Once, Hit, Wound };

        public string Type { get; set; }
        public TestType Bet { get; set; }
        public RepeatType Repeat { get; set; } 


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
                    Repeat = param.Repeat,
                };

                newParams.Add(newParamTest);
            }

            return newParams;
        }

        public static void Tests(ref Unit unit, Unit opponent, int round, Param.RepeatType context)
        {
            bool onceUseContext = (context == RepeatType.Round) && (round == 1);

            foreach (Param param in opponent.ParamTests)
                if ((param.Repeat == context) || ((param.Repeat == Param.RepeatType.Once) && onceUseContext))
                    ParamTest(ref unit, param.Type, opponent, param.Bet, context);
        }

        private static void ParamTest(ref Unit unit, string param, Unit opponent, Test.Param.TestType test, Param.RepeatType context)
        {
            bool roundFormat = ((context == RepeatType.Hit) || (context == RepeatType.Wound));

            Test.Data.Console(Test.Data.text, (roundFormat ? " --> " : "\n\n") + "{0} must pass {1} test ", unit.Name, param);

            int paramValue = (int)typeof(Unit).GetProperty(param).GetValue(unit);
            int diceNum = ((param == "Leadership") ? 2 : 1);

            if (Test.Dice.Roll(unit, param, opponent, paramValue, diceNum, paramTest: true))
                Test.Data.Console(Test.Data.goodText, " --> passed");
            else
                switch (test)
                {
                    case Test.Param.TestType.Pass:
                        Test.Data.Console(Test.Data.badText, " --> pass this round");
                        unit.PassThisRound = true;
                        break;
                    case Test.Param.TestType.Wound:
                        Test.Data.Console(Test.Data.badText, " --> WOUND");
                        unit.Wounds -= 1;
                        break;
                    case Test.Param.TestType.Death:
                        Test.Data.Console(Test.Data.badText, " --> SLAIN");
                        unit.Wounds = 0;
                        break;
                }
        }
    }
}
