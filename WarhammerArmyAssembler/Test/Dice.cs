using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Dice
    {
        public enum Types { M, WS, BS, S, T, W, I, A, LD, AS, WARD, REGENERATION, OTHER };

        public static int lastDice = 0;

        public static bool CheckReroll(Dictionary<string, Types> unitRerolls, Unit unit, Types diceType)
        {
            if (String.IsNullOrEmpty(unit.Reroll))
                return false;

            string[] allReroll = unit.Reroll.Split(';');

            foreach (string unitReroll in allReroll)
                foreach (KeyValuePair<string, Types> reroll in unitRerolls)
                    if ((unitReroll.Trim() == reroll.Key) && (reroll.Value == diceType))
                        return true;

            return false;
        }

        public static bool MustBeRerolled(Types diceType, Unit unit, Unit enemy)
        {
            Dictionary<string, Types> unitRerolls = new Dictionary<string, Types>
            {
                ["OpponentToHit"] = Types.WS,
                ["OpponentToWound"] = Types.S,
                ["OpponentToWard"] = Types.WARD,
            };

            if (CheckReroll(unitRerolls, enemy, diceType))
                return true;

            return false;
        }

        public static bool CanBeRerolled(Types diceType, Unit unit, Unit enemy)
        {
            Dictionary<string, Types> unitRerolls = new Dictionary<string, Types>
            {
                ["ToHit"] = Types.WS,
                ["ToShoot"] = Types.BS,
                ["ToWound"] = Types.S,
                ["ToLeadership"] = Types.LD,
            };

            Dictionary<string, Types> enemyRerolls = new Dictionary<string, Types>
            {
                ["ToArmour"] = Types.AS,
                ["ToWard"] = Types.WARD,
            };

            if (unit.Reroll == "All")
                return true;

            if (CheckReroll(unitRerolls, unit, diceType))
                return true;

            if (CheckReroll(enemyRerolls, enemy, diceType))
                return true;

            return false;
        }

        public static bool Roll(Unit unit, Types diceType, Unit enemy, int? conditionParam,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false)
        {
            return Roll(unit, diceType, enemy, conditionParam, out int _, diceNum, round, breakTest, hiddenDice);
        }

        public static int GetRankBonus(Unit unit)
        {
            if (!unit.StrengthInNumbers)
                return 0;
            else
                return Unit.ParamNormalization((unit.GetRank() - 1), onlyZeroCheck: true);
        }

        public static bool Roll(Unit unit, Types diceType, Unit enemy, int? conditionParam, out int dice,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false)
        {
            dice = 0;

            if (conditionParam == null)
                return false;

            bool restoreConsoleOutput = (hiddenDice && InterfaceTestUnit.PreventConsoleOutputStatus());

            Unit unitTestPassed = (diceType == Types.LD ? unit : enemy);

            if (hiddenDice)
                InterfaceTestUnit.PreventConsoleOutput(prevent: true);

            int condition = conditionParam ?? 0;

            if (diceType == Types.LD)
            {
                int rankBonus = GetRankBonus(unit);

                if (rankBonus <= 0)
                    Test.Console(Test.supplText, "({0} LD, ", condition);
                else
                {
                    condition += rankBonus;
                    condition = Unit.ParamNormalization(condition);

                    Test.Console(Test.supplText, "({0} LD with rank bonus, ", condition);
                }
            }
            else
                Test.Console(Test.supplText, "({0}+, ", condition);

            int result = RollAll(diceType, unitTestPassed, diceNum);

            bool testPassed = TestPassedByDice(result, condition, diceType, breakTest);

            Test.Console(Test.supplText, "{0}", result);

            bool hateHitReroll = unit.Hate && (diceType == Types.WS);

            if ((diceType == Types.AS) && (condition > 6) && (condition < 10) && (result == 6))
            {
                int supplCondition = condition - 3;
                result = RollAll(diceType, unitTestPassed, 1);
                Test.Console(Test.supplText, " --> {0}+, {1}", supplCondition, result);

                testPassed = TestPassedByDice(result, supplCondition, diceType, breakTest);
            }
            else if (
                (!testPassed && (hateHitReroll || CanBeRerolled(diceType, unit, enemy)))
                ||
                (testPassed && MustBeRerolled(diceType, unit, enemy))
            )
            {
                result = RollAll(diceType, unitTestPassed, diceNum);
                Test.Console(Test.supplText, ", reroll --> {0}", result);
                testPassed = TestPassedByDice(result, condition, diceType, breakTest);
            }

            dice = result;

            Test.Console(Test.supplText, ")");

            if (restoreConsoleOutput)
                InterfaceTestUnit.PreventConsoleOutput(prevent: false);

            return testPassed;
        }

        public static bool TestPassedByDice(int result, int condition, Types diceType, bool breakTest = false)
        {
            bool reversCheck = (diceType == Types.AS) || (diceType == Types.WARD);

            if (breakTest && (result == 2))
            {
                Test.Console(Test.supplText, "insane courage! --> ");
                return true;
            }

            if (((result < condition) || (result == 1)) && reversCheck)
                return true;

            if ((result <= condition) && (diceType == Types.LD))
                return true;

            if ((result >= condition) && ((diceType != Types.LD) && !reversCheck))
                return true;

            return false;
        }

        public static int RollAll(Types diceType, Unit unit, int diceNum, bool hiddenDice = false)
        {
            int maxRoll = 0;
            int result = 0;

            if ((diceType == Types.LD) && unit.ColdBlooded)
            {
                if (!hiddenDice)
                    Test.Console(Test.supplText, "cold-blooded, ");

                diceNum += 1;
            }

            for (int i = 0; i < diceNum; i++)
            {
                int roll = Test.rand.Next(6) + 1;
                result += roll;

                if (roll > maxRoll)
                    maxRoll = roll;
            }

            if ((diceType == Types.LD) && unit.ColdBlooded)
                result -= maxRoll;

            lastDice = result;

            return result;
        }
    }
}
