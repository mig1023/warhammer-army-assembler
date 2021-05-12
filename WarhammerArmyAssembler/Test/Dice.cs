using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Test
{
    class Dice
    {
        public enum Types { M, WS, BS, S, T, W, I, A, LD, AS, WARD, REGENERATION, OTHER };

        public static int lastDice = 0;

        public static bool CheckReroll(Dictionary<string, Types> unitRerolls, Unit unit, Types diceType, int dice)
        {
            if (String.IsNullOrEmpty(unit.Reroll))
                return false;

            string[] allRerolls = unit.Reroll.Split(';');

            foreach (string unitReroll in allRerolls)
            {
                string strParam = String.Empty;

                if (unitReroll.Contains("("))
                {
                    strParam = unitReroll.Substring(unitReroll.IndexOf("(") + 1, 1);
                    bool parseParamOk = int.TryParse(strParam, out int param);

                    if (!parseParamOk || (param != dice))
                        continue;
                }

                string rerollType = unitReroll.Trim().Replace(String.Format("({0})", strParam), String.Empty);

                foreach (string reroll in unitRerolls.Keys.ToList())
                    if ((rerollType == reroll) && (unitRerolls[reroll] == diceType))
                        return true;
            }

            return false;
        }

        public static bool MustBeRerolled(Types diceType, Unit unit, Unit enemy, int dice)
        {
            Dictionary<string, Types> enemyRerolls = new Dictionary<string, Types>
            {
                ["OpponentToHit"] = Types.WS,
                ["OpponentToWound"] = Types.S,
                ["ToArmour"] = Types.AS,
                ["ToWard"] = Types.WARD,
            };

            return CheckReroll(enemyRerolls, enemy, diceType, dice);
        }

        public static bool CanBeRerolled(Types diceType, Unit unit, Unit enemy, int dice)
        {
            Dictionary<string, Types> unitRerolls = new Dictionary<string, Types>
            {
                ["ToHit"] = Types.WS,
                ["ToShoot"] = Types.BS,
                ["ToWound"] = Types.S,
                ["ToLeadership"] = Types.LD,
                ["OpponentToArmour"] = Types.AS,
                ["OpponentToWard"] = Types.WARD,
            };

            if (unit.Reroll == "All")
                return true;

            if (unit.MurderousProwess && (lastDice == 1))
                return true;

            return CheckReroll(unitRerolls, unit, diceType, dice);
        }

        public static int GetRankBonus(Unit unit) => (!unit.StrengthInNumbers ? 0 : Unit.ParamNormalization((unit.GetRank() - 1), onlyZeroCheck: true));

        public static bool Roll(Unit unit, Types diceType, Unit enemy, int? conditionParam,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false, bool paramTest = false)
        {
            return Roll(unit, diceType, enemy, conditionParam, out int _, diceNum, round, breakTest, hiddenDice, paramTest);
        }

        public static bool Roll(Unit unit, string lineDiceType, Unit enemy, int? conditionParam,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false, bool paramTest = false)
        {
            Dictionary<string, Types> toDiceType = new Dictionary<string, Types>
            {
                ["WeaponSkill"] = Types.WS,
                ["Strength"] = Types.S,
                ["Toughness"] = Types.T,
                ["Wounds"] = Types.W,
                ["Initiative"] = Types.I,
                ["Leadership"] = Types.LD,
            };

            return Roll(unit, toDiceType[lineDiceType], enemy, conditionParam, out int _, diceNum, round, breakTest, hiddenDice, paramTest);
        }

        public static bool Roll(Unit unit, Types diceType, Unit enemy, int? conditionParam, out int dice,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false, bool paramTest = false)
        {
            dice = 0;

            if (conditionParam == null)
                return false;

            bool restoreConsoleOutput = (hiddenDice && Interface.TestUnit.PreventConsoleOutputStatus());

            Unit unitTestPassed = ((diceType == Types.LD) || paramTest ? unit : enemy);
            Unit hisOpponent = ((diceType == Types.LD) || paramTest ? enemy : unit);

            if (hiddenDice)
                Interface.TestUnit.PreventConsoleOutput(prevent: true);

            int condition = conditionParam ?? 0;

            if (diceType == Types.LD)
            {
                int rankBonus = GetRankBonus(unit);

                if (rankBonus <= 0)
                    Test.Data.Console(Test.Data.supplText, "({0} LD, ", condition);
                else
                {
                    condition += rankBonus;
                    condition = Unit.ParamNormalization(condition);

                    Test.Data.Console(Test.Data.supplText, "({0} LD with rank bonus, ", condition);
                }
            }
            else if (paramTest)
                Test.Data.Console(Test.Data.supplText, "({0} {1}, ", condition, diceType.ToString());
            else
                Test.Data.Console(Test.Data.supplText, "({0}+, ", condition);

            int result = RollAll(diceType, unitTestPassed, diceNum, enemy: hisOpponent);

            bool testPassed = TestPassedByDice(unit, enemy, result, condition, diceType, out string addResult, breakTest, paramTest);

            Test.Data.Console(Test.Data.supplText, "{0}{1}", result, addResult);

            if ((diceType == Types.AS) && (result == 1))
           
            if ((diceType == Types.WS) && !paramTest)
            {
                if (unit.AddToHit > 0)
                    Test.Data.Console(Test.Data.supplText, ", +{0} bonus", unit.AddToHit);

                if (enemy.SubOpponentToHit > 0)
                    Test.Data.Console(Test.Data.supplText, ", -{0} penalty", enemy.SubOpponentToHit);
            }

            if ((diceType == Types.S) && !paramTest)
            {
                if (unit.AddToWound > 0)
                    Test.Data.Console(Test.Data.supplText, ", +{0} bonus", unit.AddToWound);

                if (enemy.SubOpponentToWound > 0)
                    Test.Data.Console(Test.Data.supplText, ", -{0} penalty", enemy.SubOpponentToWound);
            }

            bool hateHitReroll = unit.Hate && (diceType == Types.WS);
            bool canBeRerolled = !testPassed && (hateHitReroll || CanBeRerolled(diceType, unit, enemy, result));
            bool mustBeRerolled = testPassed && MustBeRerolled(diceType, unit, enemy, result);

            if ((diceType == Types.AS) && (condition > 6) && (condition < 10) && (result == 6))
            {
                int supplCondition = condition - 3;
                result = RollAll(diceType, unitTestPassed, 1, enemy: hisOpponent);
                testPassed = TestPassedByDice(unit, enemy, result, supplCondition, diceType, out string addAddRoll, breakTest, paramTest);

                Test.Data.Console(Test.Data.supplText, " --> {0}+, {1}{2}", supplCondition, result, addAddRoll);
            }
            else if (canBeRerolled || mustBeRerolled)
            {
                result = RollAll(diceType, unitTestPassed, diceNum, enemy: hisOpponent);
                testPassed = TestPassedByDice(unit, enemy, result, condition, diceType, out string addReroll, breakTest, paramTest);

                Test.Data.Console(Test.Data.supplText, ", reroll --> {0}{1}", result, addReroll);
            }

            dice = result;

            Test.Data.Console(Test.Data.supplText, ")");

            if (restoreConsoleOutput)
                Interface.TestUnit.PreventConsoleOutput(prevent: false);

            return testPassed;
        }

        public static bool TestPassedByDice(Unit unit, Unit enemy, int result, int condition,
            Types diceType, out string addResult, bool breakTest = false, bool paramTest = false)
        {
            addResult = String.Empty;

            bool reversCheck = (diceType == Types.AS) || (diceType == Types.WARD);

            if (breakTest && (result == 2))
            {
                Test.Data.Console(Test.Data.supplText, "insane courage! --> ");
                return true;
            }

            if ((diceType == Types.WS) && !paramTest)
            {
                if (result == 6)
                    return true;

                if (result == 1)
                    return false;

                if (unit.AddToHit > 0)
                    result += unit.AddToHit;

                if (enemy.SubOpponentToHit > 0)
                    result -= enemy.SubOpponentToHit;
            }

            if ((diceType == Types.S) && !paramTest)
            {
                if (unit.AddToWound > 0)
                    result += unit.AddToWound;

                if (enemy.SubOpponentToWound > 0)
                    result -= enemy.SubOpponentToWound;
            }

            if ((result == 6) && paramTest)
            {
                addResult = " - always fail";
                return false;
            }

            if (((result <= condition) || (result == 1)) && paramTest)
                return true;

            if ((result < condition) && reversCheck)
                return true;

            if ((result == 1) && reversCheck)
            {
                addResult = " - always fail";
                return true;
            }
            
            if ((result <= condition) && (diceType == Types.LD))
                return true;

            if ((result >= condition) && ((diceType != Types.LD) && !reversCheck && !paramTest))
                return true;

            return false;
        }

        public static int RollAll(Types diceType, Unit unit, int diceNum, bool hiddenDice = false, Unit enemy = null)
        {
            int maxRoll = 0, minRoll = 6, result = 0;

            bool bloodroar = ((enemy != null) && enemy.Bloodroar);

            if ((diceType == Types.LD) && (unit.ColdBlooded || bloodroar))
            {
                if (!hiddenDice)
                    Test.Data.Console(Test.Data.supplText, (unit.ColdBlooded ? "cold-blooded, " : "bloodroar, "));

                diceNum += 1;
            }

            for (int i = 0; i < diceNum; i++)
            {
                int roll = Test.Data.rand.Next(6) + 1;
                result += roll;

                if (roll > maxRoll)
                    maxRoll = roll;

                if (roll < minRoll)
                    minRoll = roll;
            }

            if ((diceType == Types.LD) && unit.ColdBlooded)
                result -= maxRoll;

            if ((diceType == Types.LD) && bloodroar)
                result -= minRoll;

            lastDice = result;

            return result;
        }
    }
}
