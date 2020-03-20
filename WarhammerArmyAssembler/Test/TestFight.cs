using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class TestFight
    {
        private enum DiceType { M, WS, BS, S, T, W, I, A, LD, AS, WARD };

        static List<string> testConsole = new List<string>();

        static Random rand = new Random();

        static int lastDice = 0;

        public static string Test(Unit unit, Unit enemy)
        {
            testConsole.Clear();

            Console("{0}{1} vs {1}{2}\n", unit.Name, IsUnit(unit), enemy.Name, IsUnit(enemy));

            CheckTerror(unit, enemy);
            CheckTerror(enemy, unit);

            return String.Join(String.Empty, testConsole.ToArray());
        }

        private static void Console(string line)
        {
            testConsole.Add(line);
        }

        private static void Console(string line, params object[] p)
        {
            Console(String.Format(line, p));
        }

        private static string IsUnit(Unit unit)
        {
            return (unit.Type == Unit.UnitType.Core ? " (unit)" : String.Empty);
        }

        private static void CheckTerror(Unit unit, Unit enemy)
        {
            if (!unit.Terror || enemy.Terror)
                return;

            Console("{0} try to resist of terror by {1}", enemy.Name, unit.Name);

            if (rollDice(DiceType.LD, unit, String.Format("{0}+", unit.Leadership), 2))
            {
                unit.Wounds = 0;
                Console(" --> fail");
            }
            else
                Console(" --> passed");
        }

        private static bool rollDice(DiceType diceType, Unit unit, string diceLine, int num = 1, int round = 2)
        {
            if ((diceType == DiceType.LD) && unit.ColdBlooded)
            {
                Console("cold-blooded");
                num += 1;
            }

            Console("({0}, ", diceLine);

            string diceForParse = diceLine.Replace("+", String.Empty);

            int dice = int.Parse(diceForParse);

            int result = rollAllDice(diceType, unit, num);

            bool testPassed = testPassedByDice(result, dice, diceType);

            Console("{0}", result);

            if (!testPassed && unit.Hate && (diceType == DiceType.WS))      // +reroll
            {
                result = rollAllDice(diceType, unit, num);
                Console(", {0}", result);
            }

            Console(")");

            return testPassedByDice(result, dice, diceType);
        }

        private static bool testPassedByDice(int result, int dice, DiceType diceType)
        {
            return (
                ((result < dice) && (diceType == DiceType.LD))
                ||
                ((result < dice) && (diceType != DiceType.LD))
                ||
                ((result == 1) && (diceType != DiceType.LD))
            );
        }

        private static int rollAllDice(DiceType diceType, Unit unit, int num)
        {
            int maxRoll = 0;
            int result = 0;

            for(int i = 0; i < num; i++)
            {
                int roll = rand.Next(6) + 1;
                result += roll;

                if (roll > maxRoll)
                    maxRoll = roll;
            }

            if ((diceType == DiceType.LD) && unit.ColdBlooded)
                result -= maxRoll;

            lastDice = result;

            return result;
        }
    }
}
