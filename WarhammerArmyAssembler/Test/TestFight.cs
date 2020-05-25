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

        public static void BattleRoyaleTest(Unit unit, Unit unitMount)
        {
            foreach (string enemyGroupName in Enemy.GetEnemiesGroups())
            {
                string currentText = InterfaceTestUnit.GetFullConsoleText();

                Console(supplText, "{0}\n\n", enemyGroupName.ToUpper());

                if (currentText == String.Empty)
                    Console(text, "\n");

                foreach (Enemy enemy in Enemy.GetEnemiesByGroup(enemyGroupName))
                {
                    Unit currentEnemy = enemy.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
                    Unit currentMount = null;

                    if (currentEnemy.Mount != null)
                        currentMount = enemy.Mount.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

                    StatisticTest(unit, unitMount, currentEnemy, currentMount, royalNotation: true);
                }
            }
        }

        public static void StatisticTest(Unit unit, Unit unitMount, Unit enemy, Unit enemyMount,
            bool royalNotation = false)
        {
            int[] result = new int[3];
            string currentText = InterfaceTestUnit.GetFullConsoleText();

            InterfaceTestUnit.PreventConsoleOutput(prevent: true);

            for (int i = 0; i < 1000; i++)
                result[FullTest(unit, unitMount, enemy, enemyMount)] += 1;

            InterfaceTestUnit.PreventConsoleOutput(prevent: false);

            if (royalNotation)
                Console(text, "vs {0}: win: {1:f1}% defeat: {2:f1}%\n", enemy.TestListName, (double)result[1] / 10, (double)result[2] / 10);                
            else
            {
                Console(text, "{0} win: {1:f1}%\n{2} win: {3:f1}%", unit.Name, (double)result[1] / 10, enemy.TestListName, (double)result[2] / 10);

                if (result[0] > 0)
                    Console(text, "\nNobody win: {0:f1}%", (double)result[0] / 10);
            }

            if (currentText == String.Empty)
                Console(text, "\n");

            WinDefeatScale(result[1], result[2]);
        }

        private static void WinDefeatScale(double win, double defeat)
        {
            Brush scaleColor = goodText;

            foreach(double param in new List<double> { win, defeat })
            {
                int scale = (int)param / 10;

                for (int i = 0; i < scale; i++)
                    Console(scaleColor, "|");

                scaleColor = badText;
            }

            Console(text, "\n\n");
        }

        private static void InitRoundWounds(List<Unit> opponents, ref Dictionary<int, int> roundWounds)
        {
            foreach (Unit unit in opponents)
                roundWounds[unit.ID] = 0;
        }

        private static bool BothOpponentsAreAlive(List<Unit> opponents)
        {
            Dictionary<Unit.TestTypeTypes, int> opponentsWounds = new Dictionary<Unit.TestTypeTypes, int>();

            foreach (Unit u in opponents)
                if (opponentsWounds.ContainsKey(u.TestType))
                    opponentsWounds[u.TestType] += (u.IsNotSimpleMount() ? u.Wounds : 0);
                else
                    opponentsWounds.Add(u.TestType, (u.IsNotSimpleMount() ? u.Wounds : 0));

            return ((opponentsWounds[Unit.TestTypeTypes.Unit] > 0) && (opponentsWounds[Unit.TestTypeTypes.Enemy] > 0));
        }

        public static int FullTest(Unit originalUnit, Unit originalUnitMount, Unit originalEnemy, Unit originalEnemyMount)
        {
            testConsole.Clear();

            round = 0;

            Unit unit = originalUnit.Clone().SetTestType(Unit.TestTypeTypes.Unit);
            Unit enemy = originalEnemy.Clone().SetTestType(Unit.TestTypeTypes.Enemy);

            List<Unit> participants = new List<Unit>() { unit, enemy };

            if (originalUnitMount != null)
                unit.Mount = originalUnitMount.Clone().SetTestType(Unit.TestTypeTypes.Unit);

            if (originalEnemyMount != null)
                enemy.Mount = originalEnemyMount.Clone().SetTestType(Unit.TestTypeTypes.Enemy);

            Dictionary<Unit, List<Unit>> BreakTestOrder = new Dictionary<Unit, List<Unit>>
            {
                [enemy] = new List<Unit> { enemy, enemy.Mount, unit, unit.Mount },
                [unit] = new List<Unit> { unit, unit.Mount, enemy, enemy.Mount },
            };

            if (unit.Name == enemy.Name)
                enemy.Name += " (enemy)";

            if (originalUnitMount != null)
            {
                participants.Add(unit.Mount);
                unit.Mount = unit.Mount;
                BreakTestOrder[unit.Mount] = new List<Unit> { unit.Mount, unit, enemy, enemy.Mount };
            }

            if (originalEnemyMount != null)
            {
                participants.Add(enemy.Mount);
                enemy.Mount = enemy.Mount;
                BreakTestOrder[enemy.Mount] = new List<Unit> { enemy.Mount, enemy, unit, unit.Mount };

                if ((unit.Mount != null) && (unit.Mount.Name == enemy.Mount.Name))
                    enemy.Mount.Name += " (enemy)";
            }

            Console(text, "{0} vs {1}", unit.Name,  enemy.Name);

            Dictionary<int, int> roundWounds = new Dictionary<int, int>();
            InitRoundWounds(participants, ref roundWounds);

            foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                u.Value[0] = CheckTerror(u.Value[0], u.Value[1], u.Value[2], u.Value[3]);

            while (BothOpponentsAreAlive(participants) && (round < 100))
            {
                round += 1;

                Console(supplText, "\n\nround: {0}\n", round);

                foreach (Unit u in new List<Unit> { unit, enemy })
                {
                    bool monstrousMount = (u.Mount != null) && u.Mount.IsNotSimpleMount();
                    string uMount = (monstrousMount ? String.Format(" + {0}: {1}W", u.Mount.Name, u.Mount.Wounds) : String.Empty);
                    Console(supplText, "{0}: {1}W{2}{3}", u.Name, u.Wounds, uMount, (u == unit ? ", " : String.Empty));
                }

                participants.Sort((a, b) => a.CompareTo(b));

                if (round == 1)
                    participants.Sort((a, b) => a.CompareTo(b));

                ShowRoundOrder(participants);

                Dictionary<int, int> attacksRound = new Dictionary<int, int>();

                foreach (Unit u in participants)
                    attacksRound[u.ID] = PrintAttack(u, u.Attacks, roundWounds, unit, enemy, unit.Mount);

                InitRoundWounds(participants, ref roundWounds);

                if ((round == 1) && (!String.IsNullOrEmpty(unit.ImpactHit) || (unit.Mount != null && !String.IsNullOrEmpty(unit.Mount.ImpactHit))))
                {
                    Unit impactUnit = (unit.Mount != null && !String.IsNullOrEmpty(unit.Mount.ImpactHit) ? unit.Mount : unit);
                    Unit opponent = SelectOpponent(participants, impactUnit);

                    string impactOutLine = String.Empty;
                    int attacks = ImpactHitNumer(unit, unit.Mount, out impactOutLine);

                    roundWounds[opponent.ID] += Round(impactUnit, ref opponent, attacks, round, impactHit: true, impactLine: impactOutLine);
                }

                foreach (Unit u in participants)
                    if (BothOpponentsAreAlive(participants))
                    {
                        Unit opponent = SelectOpponent(participants, u);
                        int woundsAtStartOfRound = opponent.Wounds;

                        roundWounds[opponent.ID] += Round(u, ref opponent, attacksRound[u.ID], round);

                        if (opponent.Regeneration && (woundsAtStartOfRound > opponent.Wounds) && !opponent.WoundedWithKillingBlow)
                            Regeneration(opponent, (woundsAtStartOfRound - opponent.Wounds));

                        if (opponent.Wounds <= 0)
                            Console(badText, "\n\n{0} SLAIN", opponent.Name);
                    }

                Console(text, "\n");

                if (BothOpponentsAreAlive(participants))
                {
                    bool draw = true;

                    foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                    {
                        if (u.Key.Wounds <= 0)
                            continue;

                        if (((u.Key == unit.Mount) && (unit.Wounds > 0)) || ((u.Key == enemy.Mount) && (enemy.Wounds > 0)))
                            continue;

                        roundWounds[u.Key.ID] += RoundBonus(u.Value[2], u.Value[3], u.Value[0], u.Value[1]);

                        if (RoundLostBy(u.Value[0], u.Value[1], u.Value[2], u.Value[3], roundWounds))
                        {
                            if (BreakTestFail(u.Value[0], u.Value[1], u.Value[2], u.Value[3], ref roundWounds))
                            {
                                u.Key.Wounds = 0;

                                if (u.Key.Mount != null)
                                    u.Key.Mount.Wounds = 0;

                                Console(text, "\n");
                            }

                            draw = false;
                        }
                    }

                    if (draw)
                        Console(goodText, "\nThe round ended in a draw");
                }
            }

            Console(text, "\nEnd: ");

            if (enemy.Wounds + (enemy.Mount != null && enemy.Mount.IsNotSimpleMount() ? enemy.Mount.Wounds : 0) <= 0)
            {
                Console(text, "{0} win", unit.Name);
                return 1;
            }
            else if (unit.Wounds + (unit.Mount != null && unit.Mount.IsNotSimpleMount() ? unit.Mount.Wounds : 0) <= 0)
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

        private static bool RoundLostBy(Unit unit, Unit unitMount, Unit enemy, Unit enemyMount, Dictionary<int, int> roundWounds)
        {
            if ((unit == null) || (unit.Wounds <= 0) || unit.IsSimpleMount())
                return false;

            int unitRoundWounds = roundWounds[unit.ID] + (unitMount != null ? roundWounds[unitMount.ID] : 0);
            int enemyRoundWounds = roundWounds[enemy.ID] + (enemyMount != null ? roundWounds[enemyMount.ID] : 0);

            return unitRoundWounds > enemyRoundWounds;
        }

        private static int RoundBonus(Unit unit, Unit unitMount, Unit enemy, Unit enemyMount)
        {
            int unitFullSize = (unit.Size * unit.OriginalWounds) + (unitMount != null ? unitMount.Size * unitMount.OriginalWounds : 0);
            int enemyFullSize = (enemy.Size * enemy.OriginalWounds) + (enemyMount != null ? enemyMount.Size * enemyMount.OriginalWounds : 0);

            string unitSide = (((unitMount != null) && (unit.Wounds <= 0)) ? unitMount.Name : unit.Name);

            if (unitFullSize > enemyFullSize)
            {
                Console(supplText, "\n{0} have +1 battle result bonus by outnumber", unitSide);
                return 1;
            }
            else
                return 0;
        }

        private static Unit SelectOpponent(List<Unit> participants, Unit unit)
        {
            Unit randomOpponent = null;
            bool canBeOpponent = false;

            do
            {
                randomOpponent = participants[rand.Next(participants.Count)];

                if ((randomOpponent.TestType != unit.TestType) && (randomOpponent.Wounds > 0))
                    canBeOpponent = true;

                if ((randomOpponent.Type == Unit.UnitType.Mount) && (randomOpponent.OriginalWounds == 1))
                    canBeOpponent = false;
            }
            while (!canBeOpponent);

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

        private static int PrintAttack(Unit unit, int attackNum, Dictionary<int, int> death,
            Unit tUnit, Unit tEnemy, Unit tMount)
        {
            if (unit.Frenzy)
                Console(supplText, "\n{0} --> is frenzy", unit.Name);

            int deathInRound = death[unit.ID];

            if (unit.IsSimpleMount())
                deathInRound = ((tMount != null) && (unit.ID == tMount.ID) ? death[tUnit.ID] : death[tEnemy.ID]);

            if ((!unit.IsHeroOrHisMount()) && (unit.Wounds > 0) && (deathInRound > 0))
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
                unit.Attacks -= 1;
                Console(supplText, "\n{0} lost his frenzy", unit.Name);
            }
        }

        private static Unit CheckTerror(Unit unit, Unit friend, Unit enemy, Unit enemyFriend)
        {
            bool friendTerrorOrFear = (friend != null ? (friend.Terror || friend.Fear) : false);
            bool enemyFriendTerror = (enemyFriend != null ? enemyFriend.Terror : false);

            if (unit.IsSimpleMount())
                return unit;

            if ((!enemy.Terror && !enemyFriendTerror) || unit.Terror || friendTerrorOrFear)
                return unit;

            string terrorSource = (((enemyFriend != null) && !enemy.Terror) ? enemyFriend.Name : enemy.Name);

            Console(text, "\n{0} try to resist of terror by {1} ", unit.Name, terrorSource);

            if (unit.Unbreakable)
                Console(goodText, " --> autopassed (unbreakable)");
            else if (unit.ImmuneToPsychology || unit.Undead)
                Console(goodText, " --> autopassed (imunne to psychology)");
            else if (unit.Frenzy)
                Console(goodText, " --> autopassed (frenzy)");
            else if (RollDice(unit, DiceType.LD, enemy, unit.Leadership, 2))
                Console(goodText, " --> passed");
            else
            {
                unit.Wounds = 0;
                Console(badText, " --> fail");
            }

            return unit;
        }

        private static int Round(Unit unit, ref Unit enemy, int attackNumber, int round,
            bool impactHit = false, string impactLine = "")
        {
            int roundWounds = 0;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0))
                Console(text, "\n");

            for (int i = 0; i < attackNumber; i++)
            {
                int wounded = Attack(unit, enemy, round, impactHit, impactLine);
                roundWounds += wounded;
                enemy.Wounds -= wounded;
            }

            if (enemy.Wounds < 0)
                enemy.Wounds = 0;

            return roundWounds;
        }

        private static void Regeneration(Unit unit, int roundWounds)
        {
            Console(text, "\n");

            for (int i = 0; i < roundWounds; i++)
            {
                Console(text, "\n{0} --> regeneration ", unit.Name);

                if (RollDice(unit, DiceType.REGENERATION, unit, 4))
                {
                    Console(goodText, " --> success");
                    unit.Wounds += 1;
                }
                else
                    Console(badText, " --> fail");
            }
        }

        private static void ShowRoundOrder(List<Unit> allParticipants)
        {
            Console(supplText, "\nround fight order:");

            foreach (Unit u in allParticipants)
                Console(supplText, "{0} {1}", (u == allParticipants[0] ? String.Empty : " -->"), u.Name);
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
                if (RollDice(unit, DiceType.I, enemy, 4, hiddenDice: true))
                    return true;
                else
                    return false;
            }
        }

        private static bool BreakTestFail(Unit unit, Unit unitFriend, Unit enemy, Unit enemyFriend,
            ref Dictionary<int, int> woundInRound)
        {
            Console(text, "\n{0} break test --> ", unit.Name);

            int temoraryLeadership = unit.Leadership;

            if (unit.Stubborn)
                Console(text, "stubborn --> ");
            else
                temoraryLeadership -= woundInRound[unit.ID] + (unitFriend != null ? woundInRound[unitFriend.ID] : 0);

            if (temoraryLeadership < 0)
                temoraryLeadership = 0;

            bool enemyFearOrTerror = ((enemy.Wounds > 0) && enemy.IsFearOrTerror());
            bool enemyMountFearOrTerror = ((enemyFriend != null) && (enemyFriend.Wounds > 0) ? enemyFriend.IsFearOrTerror() : false);

            bool unitFearOrTerror = ((unit.Wounds > 0) && unit.IsFearOrTerror());
            bool unitMountFearOrTerror = ((unitFriend != null) && (unitFriend.Wounds > 0) ? unitFriend.IsFearOrTerror() : false);

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
                !(unit.ImmuneToPsychology || unit.Undead || unitFearOrTerror || unitMountFearOrTerror))
            {
                Console(badText, "autobreak by {0} fear", (enemyFearOrTerror ? enemy.Name : enemyFriend.Name));
                return true;
            }
            else
            {
                if (RollDice(unit, DiceType.LD, enemy, temoraryLeadership, out int dice, diceNum: 2, breakTest: true))
                    Console(goodText, " --> passed");
                else
                {
                    Console(badText, " --> fail");

                    if (unit.Undead)
                    {
                        int additionalWounds = (dice - temoraryLeadership);

                        Console(badText, " --> {0} additional wounds", additionalWounds);

                        if (unit.Wounds < additionalWounds)
                            additionalWounds = unit.Wounds;

                        woundInRound[unit.ID] += additionalWounds;
                        unit.Wounds -= additionalWounds;

                        return false;
                    }
                    else
                        return true;
                }
            }

            CheckLostFrenzy(ref unit);

            return false;
        }

        private static int Attack(Unit unit, Unit enemy, int round, bool impactHit = false, string impactLine = "")
        {
            attackIsPoisoned = false;
            attackWithKillingBlow = false;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0))
            {
                if (!impactHit)
                    Console(text, "\n{0} --> hit ", unit.Name);
                else
                {
                    Console(text, "\n{0} --> hit (", unit.Name);
                    Console(supplText, "{0} impact hit", impactLine);
                    Console(text, ")");
                }

                if (impactHit || Hit(unit, enemy, round))
                {
                    Console(text, " --> wound ");

                    if (
                        (PoisonedAttack(unit, impactHit) || Wound(unit, enemy, round))
                        &&
                        (KillingAttack(unit, enemy) || NotAS(unit, enemy))
                        &&
                        (NotWard(unit, enemy))
                    ) {
                        if (attackWithKillingBlow && enemy.IsHeroOrHisMount())
                        {
                            Console(badText, " --> {0} SLAIN", enemy.Name);
                            enemy.WoundedWithKillingBlow = true;
                            return enemy.Wounds;
                        }
                        else
                        {
                            Console(badText, " --> {0} {1}", enemy.Name, "WOUND");
                            return WoundsNumbers(unit, enemy);
                        }
                    }
                }
                Console(goodText, " --> fail");
            }
            return 0;
        }

        private static void RandomParamValues(string param,
            out int diceNumber, out int diceSize, out int addSomething)
        {
            string[] randParams = param.Split('D');

            bool diceNumberParse = int.TryParse(randParams[0], out diceNumber);

            if (!diceNumberParse)
                diceNumber = 1;

            if (randParams[1].Contains('+'))
            {
                string[] randNumber = randParams[1].Split('+');
                _ = int.TryParse(randNumber[0], out diceSize);
                _ = int.TryParse(randNumber[1], out addSomething);
            }
            else
            {
                _ = int.TryParse(randParams[1], out diceSize);
                addSomething = 0;
            }
        }

        private static int RandomParamParse(string param)
        {
            int randomParam = 0;

            if (!param.Contains("D"))
                randomParam = int.Parse(param);
            else
            {
                RandomParamValues(param, out int diceNumber, out int diceSize, out int addSomething);

                for (int i = 0; i < diceNumber; i++)
                    randomParam += rand.Next(diceSize) + 1 + addSomething;
            }

            return randomParam;
        }

        private static int ImpactHitNumer(Unit unit, Unit unitMount, out string impactOutLine)
        {
            string impactHit = String.Empty;

            if ((unitMount == null) || String.IsNullOrEmpty(unitMount.ImpactHit))
                impactHit = unit.ImpactHit;
            else if (String.IsNullOrEmpty(unit.ImpactHit))
                impactHit = unitMount.ImpactHit;
            else
            {
                int currentImpact = 0, currentAdd = 0;

                foreach(Unit u in new List<Unit> { unit, unitMount })
                {
                    RandomParamValues(u.ImpactHit, out int diceNumber, out int diceSize, out int addSomething);

                    int diceMax = diceNumber * diceSize;

                    if ((diceMax > currentImpact) || ((diceMax == currentImpact) && (addSomething > currentAdd)))
                    {
                        currentImpact = diceMax;
                        currentAdd = addSomething;
                        impactHit = u.ImpactHit;
                    }
                }
            }

            impactOutLine = impactHit;
            return RandomParamParse(impactHit);
        }

        private static int WoundsNumbers(Unit unit, Unit enemy)
        {
            if (String.IsNullOrEmpty(unit.MultiWounds))
                return 1;

            int multiwounds = RandomParamParse(unit.MultiWounds);

            Console(text, " <-- {0} multiple wounds", multiwounds);

            if (enemy.Wounds < multiwounds)
            {
                multiwounds = enemy.Wounds;
                Console(supplText, ", only {0} can be inflicted", multiwounds);
            }
                

            return multiwounds;
        }

        private static bool PoisonedAttack(Unit unit, bool impactHit = false)
        {
            if (!impactHit && unit.PoisonAttack && (lastDice == 6))
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
            bool killingBlow = unit.HeroicKillingBlow || (unit.KillingBlow && (enemy.UnitStrength <= 1));

            if (killingBlow && !attackIsPoisoned && (lastDice == 6))
            {
                attackWithKillingBlow = true;
                Console(text, " --> {0}killing blow", (unit.HeroicKillingBlow ? "heroic " : String.Empty));
                return true;
            }

            if ((enemy.Armour != null) && !unit.NoArmour)
                Console(text, " --> AS ");

            return false;
        }

        private static bool Hit(Unit unit, Unit enemy, int round)
        {
            int chance = 4;

            if (unit.AutoHit)
            {
                Console(text, "(autohit)");
                return true;
            }
            else if (unit.WeaponSkill > enemy.WeaponSkill)
                chance = 3;
            else if ((unit.WeaponSkill * 2) < enemy.WeaponSkill)
                chance = 5;

            return RollDice(unit, DiceType.WS, enemy, chance, round: round);
        }

        private static bool Wound(Unit unit, Unit enemy, int round)
        {
            int chance = 4;
            int strength = unit.Strength;

            if ((unit.Lance || unit.Flail) && (round == 1))
            {
                strength += 2;

                if (strength > 10)
                    strength = 10;
            }

            if (unit.AutoWound)
            {
                Console(text, "(autowound)");
                return true;
            }
            if (strength == (enemy.Toughness + 1))
                chance = 3;
            else if (strength > (enemy.Toughness + 1))
                chance = 2;
            else if ((strength + 1) == enemy.Toughness)
                chance = 5;
            else if ((strength + 2) == enemy.Toughness)
                chance = 6;
            else if ((strength + 2) < enemy.Toughness)
            {
                Console(text, "(impossible)");
                return false;
            }

            return RollDice(unit, DiceType.S, enemy, chance);
        }

        private static bool NotAS(Unit unit, Unit enemy)
        {
            if ((enemy.Armour == null) || unit.NoArmour)
                return true;

            int chance = (unit.Strength + unit.ArmourPiercing) - 3;

            if (chance < 0)
                chance = 0;

            chance += enemy.Armour ?? 0;

            return RollDice(unit, DiceType.AS, enemy, chance);
        }

        private static bool NotWard(Unit unit, Unit enemy)
        {
            if (enemy.Ward == null)
                return true;

            Console(text, " --> ward ");

            return RollDice(unit, DiceType.WARD, enemy, enemy.Ward);
        }

        private static bool CheckReroll(Dictionary<string, DiceType> unitRerolls, Unit unit, DiceType diceType)
        {
            if (String.IsNullOrEmpty(unit.Reroll))
                return false;

            string[] allReroll = unit.Reroll.Split(';');

            foreach(string unitReroll in allReroll)
                foreach (KeyValuePair<string, DiceType> reroll in unitRerolls)
                    if ((unitReroll.Trim() == reroll.Key) && (reroll.Value == diceType))
                        return true;

            return false;
        }

        private static bool MustBeRerolled(DiceType diceType, Unit unit, Unit enemy)
        {
            Dictionary<string, DiceType> unitRerolls = new Dictionary<string, DiceType>
            {
                ["OpponentToHit"] = DiceType.WS,
                ["OpponentToWound"] = DiceType.S,
                ["OpponentToWard"] = DiceType.WARD,
            };

            if (CheckReroll(unitRerolls, enemy, diceType))
                return true;

            return false;
        }

        private static bool CanBeRerolled(DiceType diceType, Unit unit, Unit enemy)
        {
            Dictionary<string, DiceType> unitRerolls = new Dictionary<string, DiceType>
            {
                ["ToHit"] = DiceType.WS,
                ["ToShoot"] = DiceType.BS,
                ["ToWound"] = DiceType.S,
                ["ToLeadership"] = DiceType.LD,
            };

            Dictionary<string, DiceType> enemyRerolls = new Dictionary<string, DiceType>
            {
                ["ToArmour"] = DiceType.AS,
                ["ToWard"] = DiceType.WARD,
            };

            if (unit.Reroll == "All")
                return true;

            if (CheckReroll(unitRerolls, unit, diceType))
                return true;

            if (CheckReroll(enemyRerolls, enemy, diceType))
                return true;

            return false;
        }

        private static bool RollDice(Unit unit, DiceType diceType, Unit enemy, int? conditionParam,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false)
        {
            return RollDice(unit, diceType, enemy, conditionParam, out int _, diceNum, round, breakTest, hiddenDice);
        }

        private static int GetRankBonus(Unit unit)
        {
            if (!unit.StrengthInNumbers)
                return 0;
            else
            {
                int rankBonus = unit.GetRank() - 1;

                if (rankBonus < 0)
                    rankBonus = 0;

                return rankBonus;
            }
        }

        private static bool RollDice(Unit unit, DiceType diceType, Unit enemy, int? conditionParam, out int dice,
            int diceNum = 1, int round = 2, bool breakTest = false, bool hiddenDice = false)
        {
            dice = 0;

            if (conditionParam == null)
                return false;

            bool restoreConsoleOutput = (hiddenDice && InterfaceTestUnit.PreventConsoleOutputStatus());

            Unit unitTestPassed = (diceType == DiceType.LD ? unit : enemy);

            if (hiddenDice)
                InterfaceTestUnit.PreventConsoleOutput(prevent: true);

            int condition = conditionParam ?? 0;

            if (diceType == DiceType.LD)
            {
                int rankBonus = GetRankBonus(unit);

                if (rankBonus <= 0)
                    Console(supplText, "({0} LD, ", condition);
                else
                {
                    condition += rankBonus;

                    if (condition > 10)
                        condition = 10;

                    Console(supplText, "({0} LD with rank bonus, ", condition);
                }
            }
            else
                Console(supplText, "({0}+, ", condition);

            int result = RollAllDice(diceType, unitTestPassed, diceNum);

            bool testPassed = TestPassedByDice(result, condition, diceType, breakTest);

            Console(supplText, "{0}", result);

            bool hateHitReroll = unit.Hate && (diceType == DiceType.WS);

            if ((diceType == DiceType.AS) && (condition > 6) && (condition < 10) && (result == 6))
            {
                int supplCondition = condition - 3;
                result = RollAllDice(diceType, unitTestPassed, 1);
                Console(supplText, " --> {0}+, {1}", supplCondition, result);

                testPassed = TestPassedByDice(result, supplCondition, diceType, breakTest);
            }
            else if (
                (!testPassed && (hateHitReroll || CanBeRerolled(diceType, unit, enemy)))
                ||
                (testPassed && MustBeRerolled(diceType, unit, enemy))
            ) {
                result = RollAllDice(diceType, unitTestPassed, diceNum);
                Console(supplText, ", reroll --> {0}", result);
                testPassed = TestPassedByDice(result, condition, diceType, breakTest);
            }

            dice = result;

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
