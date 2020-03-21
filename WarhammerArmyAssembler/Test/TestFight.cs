using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class TestFight
    {
        private enum DiceType { M, WS, BS, S, T, W, I, A, LD, AS, WARD, REGENERATION };

        static List<string> testConsole = new List<string>();

        static Random rand = new Random();

        static int lastDice = 0;

        static bool attackWithKillingBlow = false;
        static bool attackIsPoisoned = false;
        

        public static string Test(Unit unit, Unit enemy)
        {
            testConsole.Clear();

            Console("{0}{1} vs {1}{2}\n", unit.Name, IsUnit(unit), enemy.Name, IsUnit(enemy));

            int roundWoundsUnit = 0;
            int roundWoundsEnemy = 0;
            int round = 0;
            int deathInRoundUnit = 0;
            int deathInRoundEnemy = 0;

            CheckTerror(unit, enemy);
            CheckTerror(enemy, unit);

            while((unit.Wounds > 0) && (enemy.Wounds > 0))
            {
                round += 1;
                Console("\nround: {0}\n", round);
                Console("{0}: {1}W, {2}: {3}W\n", unit.Name, unit.Wounds, enemy.Name, enemy.Wounds);

                int attacksUnit = unit.Attacks;
                int attackEnemy = enemy.Attacks;

                if (ThisIsUnit(unit))
                    attacksUnit = PrintAttack(unit, attacksUnit, roundWoundsUnit);

                if (ThisIsUnit(enemy))
                    attackEnemy = PrintAttack(enemy, attackEnemy, roundWoundsEnemy);

                if (CheckInitiative(unit, enemy, round))
                {
                    roundWoundsEnemy = Round(unit, ref enemy, attacksUnit, round);
                    roundWoundsUnit = Round(enemy, ref unit, attackEnemy, round);
                }
                else
                {
                    roundWoundsUnit = Round(enemy, ref unit, attackEnemy, round);
                    roundWoundsEnemy = Round(unit, ref enemy, attacksUnit, round);
                }

                if ((unit.Wounds > 0) && (enemy.Wounds > 0))
                {
                    if (roundWoundsUnit > roundWoundsEnemy)
                    {
                        unit.Wounds = BreakTest(unit, enemy, roundWoundsUnit);
                        CheckLostFrenzy(ref unit);
                    }

                    if (roundWoundsUnit < roundWoundsEnemy)
                    {
                        enemy.Wounds = BreakTest(enemy, unit, roundWoundsEnemy);
                        CheckLostFrenzy(ref enemy);
                    }
                }
            }

            Console("End: {0} win\n", (enemy.Wounds <= 0 ? unit.Name : enemy.Name));

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

        private static bool ThisIsUnit(Unit unit)
        {
            return (String.IsNullOrEmpty(IsUnit(unit)) ? false : true);
        }

        private static string IsUnit(Unit unit)
        {
            bool core = (unit.Type == Unit.UnitType.Core);
            bool special = (unit.Type == Unit.UnitType.Special);
            bool rare = (unit.Type == Unit.UnitType.Rare);

            return (core || special || rare ? " (unit)" : String.Empty);
        }

        private static int PrintAttack(Unit unit, int attackNum, int deathInRound)
        {
            if (unit.Frenzy)
                Console("{0} --> is frenzy");

            if ((deathInRound > 0) && ThisIsUnit(unit))
            {
                attackNum -= deathInRound;
                Console("-{0} attack {1}", deathInRound, unit.Name);
            }

            return attackNum;
        }

        private static void CheckLostFrenzy(ref Unit unit)
        {
            if (unit.Frenzy && (unit.Wounds > 0))
            {
                unit.Frenzy = false;
                Console("\n{0} lost his frenzy", unit.Name);
            }
        }

        private static void CheckTerror(Unit unit, Unit enemy)
        {
            if (!unit.Terror || enemy.Terror)
                return;

            Console("{0} try to resist of terror by {1} ", enemy.Name, unit.Name);

            if (RollDice(DiceType.LD, enemy, DiceHigher(enemy.Leadership), 2))
                Console(" --> passed");
            else
            {
                unit.Wounds = 0;
                Console(" --> fail");
            }
        }

        // ref!

        private static int Round(Unit unit, ref Unit enemy, int attackNumber, int round)
        {
            int roundWounds = 0;

            if (unit.Frenzy)
                attackNumber *= 2;

            for (int i = 0; i < attackNumber; i++)
            {
                int wounded = Attack(unit, enemy, round);
                roundWounds += wounded;
                enemy.Wounds -= wounded;
            }

            Console("\n");

            if (enemy.Regeneration && (roundWounds > 0))
            {
                Console("\n");

                for (int i = 0; i < roundWounds; i++)
                {
                    if (i > 1)
                        Console("\n");

                    Console("{0} --> regeneration ", enemy.Name);

                    if (RollDice(DiceType.REGENERATION, enemy, DiceHigher(4)))
                    {
                        Console(" --> success");
                        enemy.Wounds += 1;
                        roundWounds -= 1;
                    }
                    else
                        Console(" --> fail");
                }

                Console("\n");
            }

            return roundWounds;
        }

        private static bool CheckInitiative(Unit unit, Unit enemy, int round)
        {
            if ((round == 1) && !unit.HitFirst && !enemy.HitFirst)
            {
                Console("first round rule\n");
                return true;
            }
            else if (unit.HitFirst && !enemy.HitFirst)
            {
                Console("{0} all time first\n", unit.Name);
                return true;
            }
            else if (!unit.HitFirst && enemy.HitFirst)
            {
                Console("{0} all time first\n", enemy.Name);
                return false;
            }
            else if (unit.Initiative > enemy.Initiative)
            {
                Console("{0} has initiative\n", unit.Name);
                return true;
            }
            else if (unit.Initiative < enemy.Initiative)
            {
                Console("{0} has initiative\n", enemy.Name);
                return true;
            }
            else
            {
                Console("random initiative --> ");

                if (RollDice(DiceType.I, null, DiceHigher(4)))
                {
                    Console("{0}\n", unit.Name);
                    return true;
                }
                else
                {
                    Console("{0}\n", enemy.Name);
                    return false;
                }
            }
        }

        private static int BreakTest(Unit unit, Unit enemy, int woundInRound)
        {
            Console("\n{0} break test --> ");

            int temoraryLeadership = unit.Leadership;

            if (unit.Stubborn)
                Console("stubborn ");
            else
                temoraryLeadership -= woundInRound;

            if (temoraryLeadership < 0)
                temoraryLeadership = 0;
            
            if (unit.Unbreakable)
                Console("unbreakable ");
            else if ((enemy.Terror || enemy.Fear) && !(unit.ImmuneToPsychology || unit.Terror || unit.Fear))
            {
                Console("autobreak by {0} fear", enemy.Name);
                return 0;
            }
            else
            {
                if (RollDice(DiceType.LD, unit, DiceHigher(unit.Leadership), diceNum: 2))
                {
                    Console(" --> fail");
                    return 0;
                }
                else
                    Console(" --> passed");
            }

            return unit.Wounds;
        }

        private static int Attack(Unit unit, Unit enemy, int round)
        {
            attackIsPoisoned = false;
            attackWithKillingBlow = false;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0))
            {
                Console("\n{0} --> hit ", unit.Name);

                if (Hit(unit, enemy, round))
                {
                    Console(" --> wound ");

                    if ((PoisonedAttack(unit) || Wound(unit, enemy)) && (KillingAttack(unit) || NotAS(unit, enemy)) && (NotWard(enemy)))
                    {
                        Console(" --> {0} WOUND", enemy.Name);

                        if (attackWithKillingBlow && enemy.IsHeroOrHisMount())
                            return enemy.Wounds;

                        return 1;
                    }
                }
                Console(" --> fail");
            }
            return 0;
        }

        private static bool PoisonedAttack(Unit unit)
        {
            if (unit.PoisonAttack && (lastDice == 6))
            {
                attackIsPoisoned = true;
                Console("(poison)");
                return true;
            }
            else
                return false;
        }

        private static bool KillingAttack(Unit unit)
        {
            if (unit.KillingBlow && !attackIsPoisoned && (lastDice == 6))
            {
                attackWithKillingBlow = true;
                Console(" --> killing blow");
                return true;
            }

            Console(" --> AS ");
            return false;
        }

        private static bool Hit(Unit unit, Unit enemy, int round)
        {
            // autohit

            string chance = "4+";

            if (unit.WeaponSkill > enemy.WeaponSkill)
                chance = "3+";
            else if ((unit.WeaponSkill * 2) < enemy.WeaponSkill)
                chance = "5+";

            return RollDice(DiceType.WS, enemy, chance, round: round);
        }

        private static bool Wound(Unit unit, Unit enemy)
        {
            string chance = "4+";

            if (unit.Strength == (enemy.Toughness + 1))
                chance = "3+";
            else if (unit.Strength > (enemy.Toughness + 1))
                chance = "2+";
            else if ((unit.Strength + 1) == enemy.Toughness)
                chance = "5+";
            else if ((unit.Strength + 2) == enemy.Toughness)
                chance = "6+";
            else if ((unit.Strength + 2) < enemy.Toughness)
                return false;

            return RollDice(DiceType.S, enemy, chance);
        }

        private static bool NotAS(Unit unit, Unit enemy)
        {
            if (enemy.Armour == null)
                return true;

            int chance = unit.Strength - 3;

            if (chance < 0)
                chance = 0;

            chance += enemy.Armour ?? 0;

            return RollDice(DiceType.AS, enemy, DiceHigher(chance));
        }

        private static bool NotWard(Unit enemy)
        {
            if (enemy.Ward == null)
                return true;

            Console(" --> ward ");

            return RollDice(DiceType.WARD, enemy, DiceHigher(enemy.Ward));
        }

        private static string DiceHigher(int? param)
        {
            return String.Format("{0}+", param);
        }

        private static bool RollDice(DiceType diceType, Unit unit, string conditionLine, int diceNum = 1, int round = 2)
        {
            Console("({0}, ", conditionLine);

            string conditionForParse = conditionLine.Replace("+", String.Empty);

            int condition = int.Parse(conditionForParse);

            int result = RollAllDice(diceType, unit, diceNum);

            bool testPassed = TestPassedByDice(result, condition, diceType);

            Console("{0}", result);

            if (!testPassed && unit.Hate && (diceType == DiceType.WS))      // +reroll
            {
                result = RollAllDice(diceType, unit, diceNum);
                Console(", {0}", result);
            }

            Console(")");

            return TestPassedByDice(result, condition, diceType);
        }

        private static bool TestPassedByDice(int result, int condition, DiceType diceType)
        {
            return (
                ((result <= condition) && (diceType == DiceType.LD))
                ||
                ((result >= condition) && (diceType != DiceType.LD))
                ||
                ((result == 1) && (diceType != DiceType.LD))
            );
        }

        private static int RollAllDice(DiceType diceType, Unit unit, int diceNum)
        {
            int maxRoll = 0;
            int result = 0;

            if ((diceType == DiceType.LD) && unit.ColdBlooded)
            {
                Console("cold-blooded, ");
                diceNum += 1;
            }

            for (int i = 0; i < diceNum; i++)
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
