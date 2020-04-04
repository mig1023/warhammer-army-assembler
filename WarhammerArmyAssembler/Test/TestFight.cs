﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    class TestFight
    {
        private enum DiceType { M, WS, BS, S, T, W, I, A, LD, AS, WARD, REGENERATION };

        static List<string> testConsole = new List<string>();

        static Random rand = new Random();

        static int lastDice = 0;
        static int round = 0;

        static bool attackWithKillingBlow = false;
        static bool attackIsPoisoned = false;

        static Brush text = Brushes.Black;
        static Brush supplText = Brushes.Gray;
        static Brush goodText = Brushes.Green;
        static Brush badText = Brushes.Red;

        public static void StatisticTest(Unit unit, Unit unitMount, Unit enemy, Unit enemyMount)
        {
            int[] result = new int[3];

            InterfaceTestUnit.PreventConsoleOutput(prevent: true);

            for (int i = 0; i < 1000; i++)
                result[FullTest(unit, unitMount, enemy, enemyMount)] += 1;

            InterfaceTestUnit.PreventConsoleOutput(prevent: false);

            Console(text, "{0} win: {1} / 1000\n{2} win: {3} / 1000", unit.Name, result[1], enemy.Name, result[2]);

            if (result[0] > 0)
                Console(text, "\nNobody win: {0} / 1000", result[0]);
        }

        public static string ThisIsUnit(Unit unit)
        {
            return (unit.IsUnit() ? " (unit)" : String.Empty);
        }

        private static void InitRoundWounds(List<Unit> opponents, ref Dictionary<int, int> roundWounds)
        {
            foreach (Unit unit in opponents)
                roundWounds[unit.ID] = 0;
        }

        private static bool BothOpponentsAreAlive(List<Unit> opponents)
        {
            Dictionary<Unit.TestTypeTypes, int> opponentsWounds = new Dictionary<Unit.TestTypeTypes, int>();

            foreach (Unit unit in opponents)
                if (opponentsWounds.ContainsKey(unit.TestType))
                    opponentsWounds[unit.TestType] += unit.Wounds;
                else
                    opponentsWounds.Add(unit.TestType, unit.Wounds);

            return (opponentsWounds[Unit.TestTypeTypes.Unit] > 0) && (opponentsWounds[Unit.TestTypeTypes.Enemy] > 0);
        }

        public static int FullTest(Unit originalUnit, Unit originalUnitMount, Unit originalEnemy, Unit originalEnemyMount)
        {
            testConsole.Clear();

            round = 0;

            Unit unit = originalUnit.Clone().SetTestType(Unit.TestTypeTypes.Unit);
            Unit enemy = originalEnemy.Clone().SetTestType(Unit.TestTypeTypes.Enemy);

            Unit enemyMount = null;
            Unit unitMount = null;

            List<Unit> participants = new List<Unit>() { unit, enemy };

            if (originalUnitMount != null)
            {
                unitMount = originalUnitMount.Clone().SetTestType(Unit.TestTypeTypes.Unit);
                participants.Add(unitMount);
            }

            if (originalEnemyMount != null)
            {
                enemyMount = originalEnemyMount.Clone().SetTestType(Unit.TestTypeTypes.Enemy);
                participants.Add(enemyMount);
            }

            Console(text, "{0}{1} vs {2}{3}", unit.Name, ThisIsUnit(unit), enemy.Name, ThisIsUnit(enemy));

            Dictionary<int, int> roundWounds = new Dictionary<int, int>();
            InitRoundWounds(participants, ref roundWounds);

            CheckTerror(ref unit, unitMount, enemy);
            CheckTerror(ref enemy, enemyMount, unit);

            if (unitMount != null)
                CheckTerror(ref unitMount, unit, enemy);

            if (enemyMount != null)
                CheckTerror(ref enemyMount, enemy, enemy);

            while (BothOpponentsAreAlive(participants) && (round < 100))
            {
                round += 1;

                string unitMountLine = (unitMount != null ? String.Format(" + {0}: {1}W", unitMount.Name, unitMount.Wounds) : String.Empty);
                string enemyMountLine = (enemyMount != null ? String.Format(" + {0}: {1}W", enemyMount.Name, enemyMount.Wounds) : String.Empty);

                Console(supplText, "\n\nround: {0}", round);
                Console(supplText, "\n{0}: {1}W{2}, {3}: {4}W{5}", unit.Name, unit.Wounds, unitMountLine, enemy.Name, enemy.Wounds, enemyMountLine);

                participants.Sort((a, b) => a.CompareTo(b));

                if (round == 1)
                    participants.Sort((a, b) => a.CompareTo(b));

                ShowRoundOrder(participants);

                Dictionary<int, int> attacksRound = new Dictionary<int, int>();

                foreach(Unit u in participants)
                    attacksRound[u.ID] = PrintAttack(u, u.Attacks, roundWounds[u.ID]);

                InitRoundWounds(participants, ref roundWounds);

                foreach (Unit u in participants)
                    if (BothOpponentsAreAlive(participants))
                    {
                        Unit opponent = SelectOpponent(participants, u);
                        roundWounds[opponent.ID] += Round(u, ref opponent, attacksRound[u.ID], round);
                    }

                int unitRoundWounds = roundWounds[unit.ID] + (unitMount != null ? roundWounds[unitMount.ID] : 0);
                int enemyRoundWounds = roundWounds[enemy.ID] + (enemyMount != null ? roundWounds[enemyMount.ID] : 0);

                if ((enemy.Wounds > 0) && (enemyRoundWounds > unitRoundWounds))
                    enemy.Wounds = BreakTest(enemy, enemyMount, unit, unitMount, roundWounds[enemy.ID]);

                if ((enemyMount != null) && (enemyMount.Wounds > 0) && (enemyRoundWounds > unitRoundWounds))
                    enemyMount.Wounds = BreakTest(enemyMount, enemy, unit, unitMount, roundWounds[enemyMount.ID]);

                if ((unit.Wounds > 0) && (unitRoundWounds > enemyRoundWounds))
                    unit.Wounds = BreakTest(unit, unitMount, enemy, enemyMount, roundWounds[unit.ID]);

                if ((unitMount != null) && (unitMount.Wounds > 0) && (unitRoundWounds > enemyRoundWounds))
                    unitMount.Wounds = BreakTest(unitMount, unit, enemy, enemyMount, roundWounds[unitMount.ID]);
            }

            Console(text, "\n\nEnd: ");

            if (enemy.Wounds + (enemyMount == null ? 0 : enemyMount.Wounds) <= 0)
            {
                Console(text, "{0} win", unit.Name);
                return 1;
            }
            else if (unit.Wounds + (unitMount == null ? 0 : unitMount.Wounds) <= 0)
            {
                Console(text, "{0} win", enemy.Name);
                return 2;
            }
            else
            {
                Console(text, "{0} and {1} failed to kill each other", unit.Name, enemy.Name);
                return 0;
            }
        }

        private static Unit SelectOpponent(List<Unit> participants, Unit unit)
        {
            Unit randomOpponent = null;

            do
                randomOpponent = participants[rand.Next(participants.Count)];
            while (randomOpponent.TestType == unit.TestType || randomOpponent.Wounds <= 0);

            return randomOpponent;
        }

        private static void Console(Brush color, string line)
        {
            InterfaceTestUnit.LineToConsole(line, color);
        }

        private static void Console(Brush color, string line, params object[] p)
        {
            Console(color, String.Format(line, p));
        }

        private static int PrintAttack(Unit unit, int attackNum, int deathInRound)
        {
            if (unit.Frenzy)
                Console(supplText, "\n{0} --> is frenzy", unit.Name);

            if (unit.IsUnit() && (deathInRound > 0))
            {
                attackNum -= deathInRound;
                Console(supplText, "\n-{0} attack {1}", deathInRound, unit.Name);
            }

            if (unit.IsUnit() && ((unit.Wounds * unit.OriginalAttacks) < attackNum))
                attackNum = unit.Wounds * unit.OriginalAttacks;

            return attackNum;
        }

        private static void CheckLostFrenzy(ref Unit unit)
        {
            if (unit.Frenzy && (unit.Wounds > 0))
            {
                unit.Frenzy = false;
                Console(supplText, "\n{0} lost his frenzy", unit.Name);
            }
        }

        private static void CheckTerror(ref Unit unit, Unit friend, Unit enemy)
        {
            bool friendTerrorOrFear = (friend != null ? (friend.Terror || friend.Fear) : false);

            if (!enemy.Terror || unit.Terror || unit.Fear || friendTerrorOrFear)
                return;

            Console(text, "\n{0} try to resist of terror by {1} ", unit.Name, enemy.Name);

            if (RollDice(DiceType.LD, unit, unit.Leadership, 2))
                Console(goodText, " --> passed");
            else
            {
                unit.Wounds = 0;
                Console(badText, " --> fail");
            }
        }

        private static int Round(Unit unit, ref Unit enemy, int attackNumber, int round)
        {
            int roundWounds = 0;

            if (unit.Frenzy)
                attackNumber *= 2;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0))
                Console(text, "\n");

            for (int i = 0; i < attackNumber; i++)
            {
                int wounded = Attack(unit, enemy, round);
                roundWounds += wounded;
                enemy.Wounds -= wounded;
            }

            if (enemy.Regeneration && (roundWounds > 0) && !attackWithKillingBlow)
            {
                Console(text, "\n");

                for (int i = 0; i < roundWounds; i++)
                {
                    Console(text, "\n{0} --> regeneration ", enemy.Name);

                    if (RollDice(DiceType.REGENERATION, enemy, 4))
                    {
                        Console(goodText, " --> success");
                        enemy.Wounds += 1;
                        roundWounds -= 1;
                    }
                    else
                        Console(badText, " --> fail");
                }
            }

            if (enemy.Wounds < 0)
                enemy.Wounds = 0;

            return roundWounds;
        }

        private static void ShowRoundOrder(List<Unit> allParticipants)
        {
            Console(supplText, "\nround fight order");

            foreach (Unit u in allParticipants)
                Console(supplText, " --> {0}", u.Name);
        }

        public static bool CheckInitiative(Unit unit, Unit enemy)
        {
            Unit.TestTypeTypes unitType = Unit.TestTypeTypes.Unit;
            Unit.TestTypeTypes enemyType = Unit.TestTypeTypes.Enemy;

            if ((round == 1) && (unit.TestType == unitType) && (enemy.TestType == enemyType) && (!enemy.HitFirst))
                return true;
            else if ((round == 1) && (unit.TestType == enemyType) && (enemy.TestType == unitType) && (!unit.HitFirst))
                return true;
            else if (unit.HitFirst && !enemy.HitFirst)
                return true;
            else if (!unit.HitFirst && enemy.HitFirst)
                return false;
            else if (unit.HitLast && !enemy.HitLast)
                return false;
            else if (!unit.HitLast && enemy.HitLast)
                return true;
            else if (unit.Initiative > enemy.Initiative)
                return true;
            else if (unit.Initiative < enemy.Initiative)
                return false;
            else
            {
                if (RollDice(DiceType.I, unit, 4, hiddenDice: true))
                    return true;
                else
                    return false;
            }
        }

        private static int BreakTest(Unit unit, Unit unitFriend, Unit enemy, Unit enemyFriend, int woundInRound)
        {
            Console(text, "\n\n{0} break test --> ", unit.Name);

            int temoraryLeadership = unit.Leadership;

            if (unit.Stubborn)
                Console(text, "stubborn --> ");
            else
                temoraryLeadership -= woundInRound;

            if (temoraryLeadership < 0)
                temoraryLeadership = 0;

            bool enemyFearOrTerror = ((enemy.Wounds > 0) && (enemy.Terror || enemy.Fear));
            bool enemyMountFearOrTerror = ((enemyFriend != null) && (enemyFriend.Wounds > 0) ? (enemyFriend.Terror || enemyFriend.Fear) : false);

            bool unitFearOrTerror = ((unit.Wounds > 0) && (unit.Terror || unit.Fear));
            bool unitMountFearOrTerror = ((unitFriend != null) && (unitFriend.Wounds > 0) ? (unitFriend.Terror || unitFriend.Fear) : false);

            bool thereAreMoreOfThem = (
                (unit.UnitStrength * unit.Size) + (unitFriend != null ? (unitFriend.UnitStrength * unitFriend.Size) : 0) <
                (enemy.UnitStrength * enemy.Size) + (enemyFriend != null ? (enemyFriend.UnitStrength * enemyFriend.Size) : 0)
            );

            if (unit.Unbreakable)
                Console(text, "unbreakable");
            else if (
                thereAreMoreOfThem
                &&
                (enemyFearOrTerror || enemyMountFearOrTerror)
                &&
                !(unit.ImmuneToPsychology || unitFearOrTerror || unitMountFearOrTerror))
            {
                Console(badText, "autobreak by {0} fear", (enemyFearOrTerror ? enemy.Name : enemyFriend.Name));
                return 0;
            }
            else
            {
                if (RollDice(DiceType.LD, unit, temoraryLeadership, diceNum: 2, breakTest: true))
                    Console(goodText, " --> passed");
                else
                {
                    Console(badText, " --> fail");
                    return 0;
                }
            }

            CheckLostFrenzy(ref unit);

            return unit.Wounds;
        }

        private static int Attack(Unit unit, Unit enemy, int round)
        {
            attackIsPoisoned = false;
            attackWithKillingBlow = false;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0))
            {
                Console(text, "\n{0} --> hit ", unit.Name);

                if (Hit(unit, enemy, round))
                {
                    Console(text, " --> wound ");

                    if ((PoisonedAttack(unit) || Wound(unit, enemy)) && (KillingAttack(unit, enemy) || NotAS(unit, enemy)) && (NotWard(enemy)))
                    {
                        if (attackWithKillingBlow && enemy.IsHeroOrHisMount())
                        {
                            Console(badText, " --> {0} SLAIN", enemy.Name);
                            return enemy.Wounds;
                        }
                        else
                        {
                            Console(badText, " --> {0} {1}", enemy.Name, ((enemy.Wounds <= 1) && !enemy.IsUnit() ? "SLAIN" : "WOUND"));
                            return WoundsNumbers(unit, enemy);
                        }
                    }
                }
                Console(goodText, " --> fail");
            }
            return 0;
        }

        private static int WoundsNumbers(Unit unit, Unit enemy)
        {
            if (String.IsNullOrEmpty(unit.MultiWounds))
                return 1;

            int multiwounds = 0;

            if (unit.MultiWounds.Contains("D"))
                multiwounds = rand.Next(int.Parse(unit.MultiWounds.Replace("D", String.Empty))) + 1;
            else
                multiwounds = int.Parse(unit.MultiWounds);

            Console(text, " <-- {0} multiple wounds", multiwounds);

            return multiwounds;
        }

        private static bool PoisonedAttack(Unit unit)
        {
            if (unit.PoisonAttack && (lastDice == 6))
            {
                attackIsPoisoned = true;
                Console(text, "(poison)");
                return true;
            }
            else
                return false;
        }

        private static bool KillingAttack(Unit unit, Unit enemy)
        {
            if (unit.KillingBlow && !attackIsPoisoned && (lastDice == 6) && (enemy.UnitStrength <= 1))
            {
                attackWithKillingBlow = true;
                Console(text, " --> killing blow");
                return true;
            }

            if (enemy.Armour != null)
                Console(text, " --> AS ");

            return false;
        }

        private static bool Hit(Unit unit, Unit enemy, int round)
        {
            // autohit

            int chance = 4;

            if (unit.WeaponSkill > enemy.WeaponSkill)
                chance = 3;
            else if ((unit.WeaponSkill * 2) < enemy.WeaponSkill)
                chance = 5;

            return RollDice(DiceType.WS, enemy, chance, round: round);
        }

        private static bool Wound(Unit unit, Unit enemy)
        {
            int chance = 4;

            if (unit.Strength == (enemy.Toughness + 1))
                chance = 3;
            else if (unit.Strength > (enemy.Toughness + 1))
                chance = 2;
            else if ((unit.Strength + 1) == enemy.Toughness)
                chance = 5;
            else if ((unit.Strength + 2) == enemy.Toughness)
                chance = 6;
            else if ((unit.Strength + 2) < enemy.Toughness)
            {
                Console(text, "(impossible)");
                return false;
            }

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

            return RollDice(DiceType.AS, enemy, chance);
        }

        private static bool NotWard(Unit enemy)
        {
            if (enemy.Ward == null)
                return true;

            Console(text, " --> ward ");

            return RollDice(DiceType.WARD, enemy, enemy.Ward);
        }

        private static bool RollDice(DiceType diceType, Unit unit, int? conditionParam,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false)
        {
            if (conditionParam == null)
                return false;

            bool restoreConsoleOutput = (hiddenDice && InterfaceTestUnit.PreventConsoleOutputStatus());

            if (hiddenDice)
                InterfaceTestUnit.PreventConsoleOutput(prevent: true);

            int condition = conditionParam ?? 0;

            Console(supplText, "({0}{1}, ", condition, (diceType == DiceType.LD ? " LD" : "+"));

            int result = RollAllDice(diceType, unit, diceNum);

            bool testPassed = TestPassedByDice(result, condition, diceType, breakTest);

            Console(supplText, "{0}", result);

            if (!testPassed && unit.Hate && (diceType == DiceType.WS))      // +reroll
            {
                result = RollAllDice(diceType, unit, diceNum);
                Console(supplText, ", {0}", result);

                testPassed = TestPassedByDice(result, condition, diceType, breakTest);
            }
            else if ((diceType == DiceType.AS) && (condition > 6) && (condition < 10) && (result == 6))
            {
                int supplCondition = condition - 3;
                result = RollAllDice(diceType, unit, 1);
                Console(supplText, " --> {0}+, {1}", supplCondition, result);

                testPassed = TestPassedByDice(result, supplCondition, diceType, breakTest);
            }

            Console(supplText, ")");

            if (restoreConsoleOutput)
                InterfaceTestUnit.PreventConsoleOutput(prevent: false);

            return testPassed;
        }

        private static bool TestPassedByDice(int result, int condition, DiceType diceType, bool breakTest = false)
        {
            bool reversCheck = (diceType == DiceType.AS) || (diceType == DiceType.WARD);

            if (breakTest && (result == 2))
            {
                Console(supplText, "insane courage! --> ");
                return true;
            }

            if (((result < condition) || (result == 1)) && reversCheck)
                return true;

            if ((result <= condition) && (diceType == DiceType.LD))
                return true;

            if ((result >= condition) && ((diceType != DiceType.LD) && !reversCheck))
                return true;

            return false;
        }

        private static int RollAllDice(DiceType diceType, Unit unit, int diceNum)
        {
            int maxRoll = 0;
            int result = 0;

            if ((diceType == DiceType.LD) && unit.ColdBlooded)
            {
                Console(supplText, "cold-blooded, ");
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
