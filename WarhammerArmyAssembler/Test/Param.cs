using System;
using System.Collections.Generic;

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
        public bool MountsOnly { get; set; }

        public bool UsedAlready { get; set; }


        public static List<Param> Clone(List<Param> allParams)
        {
            if (allParams == null)
                return new List<Param> { };

            List<Param> newParams = new List<Param>();

            foreach (Param param in allParams)
            {
                Param newParamTest = new Param
                {
                    Type = param.Type,
                    Bet = param.Bet,
                    Context = param.Context,
                    Repeat = param.Repeat,
                    MountsOnly = param.MountsOnly,
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
                int testsCount = 1;

                if (unit.IsUnit() && (context == ContextType.Round))
                    testsCount = unit.GetFront();

                bool canBeApplied = (param.Repeat == RepeatType.Normal) || ((param.Repeat == RepeatType.Once) && !param.UsedAlready);

                if (param.MountsOnly && (unit.Type != Unit.UnitType.Mount))
                    canBeApplied = false;

                if ((param.Context == context) && canBeApplied)
                {
                    for (int i = 0; i < testsCount; i++)
                        ParamTest(ref unit, param.Type, opponent, param.Bet, context, i);

                    if (param.Repeat == RepeatType.Once)
                        param.UsedAlready = true;
                }
            }
        }

        private static void ParamTest(ref Unit unit, string param, Unit opponent, TestType test,
            ContextType context, int testCount)
        {
            bool roundFormat = ((context == ContextType.Hit) || (context == ContextType.Wound));

            string newLine = (testCount >= 1 ? "\n" : "\n\n");
            Test.Data.Console(Test.Data.text, (roundFormat ? " --> " : newLine) + "{0} must pass {1} test ", unit.Name, param);

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

        public static void Describe(List<Param> param, ref List<string> rules)
        {
            foreach (Param p in param)
            {
                Dictionary<ContextType, string> contextType = new Dictionary<ContextType, string>
                {
                    [ContextType.Round] = "At the beginning of each round",
                    [ContextType.Hit] = "hit",
                    [ContextType.Wound] = "wound",
                    [ContextType.ArmourSave] = "successful armour save",
                    [ContextType.WardSave] = "successful ward save",
                };

                Dictionary<TestType, string> betType = new Dictionary<TestType, string>
                {
                    [TestType.Wound] = "get a wound",
                    [TestType.Death] = "will be slain",
                    [TestType.Pass] = "skip a round without attack",
                };

                string head = String.Format("After {0} ", (p.Repeat == RepeatType.Once ? "first" : "each"));

                if (p.Context == ContextType.Round)
                    head = String.Empty;

                string opponent = (p.MountsOnly ? "opponent's mount" : "opponent");

                rules.Add(String.Format("{0}{1} {2} must pass {3} test or {4}", head, contextType[p.Context], opponent, p.Type, betType[p.Bet]));
            }
        }
    }
}
