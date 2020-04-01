using System;
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

        public static void StatisticTest(Unit unit, Unit mount, Unit enemy)
        {
            int[] result = new int[3];

            InterfaceTestUnit.PreventConsoleOutput(prevent: true);

            for (int i = 0; i < 1000; i++)
                result[FullTest(unit, mount, enemy)] += 1;

            InterfaceTestUnit.PreventConsoleOutput(prevent: false);

            Console(text, "{0} win: {1} / 1000\n{2} win: {3} / 1000", unit.Name, result[1], enemy.Name, result[2]);

            if (result[0] > 0)
                Console(text, "\nNobody win: {0} / 1000", result[0]);
        }

        public static string ThisIsUnit(Unit unit)
        {
            return (unit.IsUnit() ? " (unit)" : String.Empty);
        }

        private static void InitRoundWounds(Dictionary<int, Unit> opponents, ref Dictionary<int, int> roundWounds)
        {
            foreach (int unitID in opponents.Keys.ToArray())
                roundWounds[unitID] = 0;
        }

        public static int FullTest(Unit originalUnit, Unit originalMount, Unit originalEnemy)
        {
            testConsole.Clear();

            round = 0;

            Unit unit = originalUnit.Clone().SetTestType(Unit.TestTypeTypes.Unit);
            Unit mount = (originalMount == null ? null : originalMount.Clone().SetTestType(Unit.TestTypeTypes.Mount));
            Unit enemy = originalEnemy.Clone().SetTestType(Unit.TestTypeTypes.Enemy);

            Dictionary<int, Unit> participants = new Dictionary<int, Unit>() {[unit.ID] = unit, [enemy.ID] = enemy };
            Dictionary<int, Unit> opponents = new Dictionary<int, Unit>() { [unit.ID] = enemy, [enemy.ID] = unit };

            if (mount != null)
            {
                participants.Add(mount.ID, mount);
                opponents.Add(mount.ID, enemy);
            }

            Console(text, "{0}{1} vs {2}{3}", unit.Name, ThisIsUnit(unit), enemy.Name, ThisIsUnit(enemy));

            Dictionary<int, int> roundWounds = new Dictionary<int, int>();
            InitRoundWounds(opponents, ref roundWounds);

            CheckTerror(ref unit, enemy);
            CheckTerror(ref enemy, unit);

            while((unit.Wounds + (mount == null ? 0 : mount.Wounds) > 0) && (enemy.Wounds > 0) && (round < 100))
            {
                round += 1;

                string mountLine = (mount != null ? String.Format(" + {0}: {1}W", mount.Name, mount.Wounds) : String.Empty);

                Console(supplText, "\n\nround: {0}", round);
                Console(supplText, "\n{0}: {1}W{2}, {3}: {4}W", unit.Name, unit.Wounds, mountLine, enemy.Name, enemy.Wounds);

                List<Unit> allParticipants = new List<Unit> { unit, enemy };

                if (mount != null)
                    allParticipants.Add(mount);

                allParticipants.Sort((a, b) => a.CompareTo(b));
                ShowRoundOrder(allParticipants);

                Dictionary<int, int> attacksRound = new Dictionary<int, int>();

                foreach(Unit u in allParticipants)
                    attacksRound[u.ID] = PrintAttack(u, u.Attacks, roundWounds[u.ID]);

                InitRoundWounds(opponents, ref roundWounds);

                foreach (Unit u in allParticipants)
                {
                    Unit opponent = SelectOpponent(opponents, participants, u);
                    roundWounds[opponent.ID] += Round(u, ref opponent, attacksRound[u.ID], round);
                }

                int unitRoundWounds = roundWounds[unit.ID] + (mount != null ? roundWounds[mount.ID] : 0);

                if ((enemy.Wounds > 0) && (roundWounds[enemy.ID] > unitRoundWounds))
                    enemy.Wounds = BreakTest(enemy, unit, mount, roundWounds[enemy.ID]);

                if ((unit.Wounds > 0) && (unitRoundWounds > roundWounds[enemy.ID]))
                    unit.Wounds = BreakTest(unit, enemy, null, roundWounds[unit.ID]);

                if ((mount != null)  && (mount.Wounds > 0) && (unitRoundWounds > roundWounds[enemy.ID]))
                    mount.Wounds = BreakTest(mount, enemy, null, roundWounds[mount.ID]);
            }

            Console(text, "\n\nEnd: ");

            if (enemy.Wounds <= 0)
            {
                Console(text, "{0} win", unit.Name);
                return 1;
            }
            else if (unit.Wounds + (mount == null ? 0 : mount.Wounds) <= 0)
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

        private static Unit SelectOpponent(Dictionary<int, Unit> opponents, Dictionary<int, Unit> participants, Unit unit)
        {
            if (unit.TestType == Unit.TestTypeTypes.Unit || unit.TestType == Unit.TestTypeTypes.Mount)
                return opponents[unit.ID];
            else
            {
                List<int> allUnits = participants.Keys.ToList();
                int randomOpponent;

                do
                    randomOpponent = allUnits[rand.Next(allUnits.Count)];
                while (participants[randomOpponent].ID == unit.ID || participants[randomOpponent].Wounds <= 0);

                return participants[randomOpponent];
            }
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

        private static void CheckTerror(ref Unit unit, Unit enemy)
        {
            if (!enemy.Terror || unit.Terror)
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
            if ((round == 1) && (unit.TestType != Unit.TestTypeTypes.Enemy) && (!enemy.HitFirst))
                return true;
            else if (unit.HitFirst && !enemy.HitFirst)
                return true;
            else if (!unit.HitFirst && enemy.HitFirst)
                return false;
            else if (unit.Initiative > enemy.Initiative)
                return true;
            else if (unit.Initiative < enemy.Initiative)
                return true;
            else
            {
                if (RollDice(DiceType.I, unit, 4))
                    return true;
                else
                    return false;
            }
        }

        private static int BreakTest(Unit unit, Unit enemy, Unit mount, int woundInRound)
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
            bool enemyMountFearOrTerror = ((mount != null) && (mount.Wounds > 0) ? (mount.Terror || mount.Fear) : false);

            if (unit.Unbreakable)
                Console(text, "unbreakable");
            else if ((enemyFearOrTerror) && !(unit.ImmuneToPsychology || unit.Terror || unit.Fear))
            {
                Console(badText, "autobreak by {0} fear", enemy.Name);
                return 0;
            }
            else if (enemyMountFearOrTerror && !(unit.ImmuneToPsychology || unit.Terror || unit.Fear))
            {
                Console(badText, "autobreak by {0} fear", mount.Name);
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

            Console(text, " --> {0} multiple wounds", multiwounds);

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
            int diceNum = 1, int round = 2, bool breakTest = false)
        {
            if (conditionParam == null)
                return false;

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
