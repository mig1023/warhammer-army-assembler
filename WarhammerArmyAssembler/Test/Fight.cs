using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Test
{
    class Fight
    {
        static int round = 0;

        static bool attackWithKillingBlow = false;
        static bool attackIsPoisoned = false;

        public static async void TestByName(Data.TestTypes testType)
        {
            Data.testConsole.Clear();

            Progress<string> current = new Progress<string>();
            current.ProgressChanged += (sender, args) => Interface.Changes.main.currentTest.Content = args.ToString();

            Interface.Test.VisibilityTest(before: true);

            if (testType == Data.TestTypes.battleRoyale)
            {
                await Task.Run(() => Fight.BattleRoyaleTest(Data.unit, Data.unitMount, current));
            }
            else if (testType == Data.TestTypes.statisticTest)
            {
                await Task.Run(() => Fight.StatisticTest(Data.unit, Data.unitMount, Data.enemy, Data.enemyMount));
            }
            else
            {
                await Task.Run(() => Fight.FullTest(Data.unit, Data.unitMount, Data.enemy, Data.enemyMount));
            }

            foreach (Interface.Text line in Data.testConsole)
                Interface.Test.FromConsoleToOutput(line.Content, line.Color);

            Interface.Test.VisibilityTest();
        }

        private static void EnemiesGroupBox(string name, int current)
        {
            int len = name.Length + 6;
            string firestLineFix = (current == 0 ? "\n" : String.Empty);

            Data.Console(Data.supplText, $"\n{new String('/', len)}\n{firestLineFix}");
            Data.Console(Data.supplText, $"// {name.ToUpper()} //\n");
            Data.Console(Data.supplText, $"{new String('/', len)}\n\n");
        }

        public static void BattleRoyaleTest(Unit unit, Unit unitMount, IProgress<string> progress)
        {
            int current = 0; 

            foreach (string enemyGroupName in Enemy.Groups())
            {
                string currentText = Interface.Test.GetFullConsoleText();

                EnemiesGroupBox(enemyGroupName, current);

                foreach (Enemy enemy in Enemy.ByGroup(enemyGroupName))
                {
                    current += 1;

                    Unit currentEnemy = enemy
                        .Clone()
                        .GetOptionRules(hasMods: out _, directModification: true)
                        .GetUnitMultiplier();

                    Unit currentMount = enemy
                        .Mount?
                        .Clone()
                        .GetOptionRules(hasMods: out _, directModification: true)
                        .GetUnitMultiplier();

                    int enemyCount = Enemy.Count();

                    if (current < enemyCount)
                        progress.Report($"{current}/{enemyCount} {enemy.Name}");
                    else
                        progress.Report("Preparing a report... please wait");

                    StatisticTest(unit, unitMount, currentEnemy, currentMount, royalNotation: true);
                }
            }
        }

        public static void StatisticTest(Unit unit, Unit unitMount, Unit enemy, Unit enemyMount,
            bool royalNotation = false)
        {
            int[] result = new int[3];

            Interface.Test.PreventConsoleOutput(prevent: true);

            for (int i = 0; i < 1000; i++)
                result[FullTest(unit, unitMount, enemy, enemyMount)] += 1;

            Interface.Test.PreventConsoleOutput(prevent: false);

            if (royalNotation)
            {
                Data.Console(Data.text, $"vs {enemy.Name}: " +
                    $"win: {(double)result[1] / 10:f1}% " +
                    $"defeat: {((double)result[2] / 10):f1}%\n");
            }
            else
            {
                Data.Console(Data.text, $"{unit.Name} " +
                    $"win: {(double)result[1] / 10:f1}%" +
                    $"\n{enemy.Name} win: {((double)result[2] / 10):f1}%");

                if (result[0] > 0)
                    Data.Console(Data.text, $"\nNobody win: {(double)result[0] / 10:f1}%");
            }

            WinDefeatScale(result[1], result[2]);
        }

        private static void ScaleLine(string marker, int len, string el)
        {
            Data.Console(Data.supplText, marker);

            for (int i = 0; i < len; i++)
                Data.Console(Data.supplText, el);
        }

        private static void ScaleDraw()
        {
            Data.Console(Data.text, "\n");

            ScaleLine("0%", 46, " ");
            ScaleLine("50%", 45, " ");
            Data.Console(Data.supplText, "100%");

            Data.Console(Data.text, "\n");

            ScaleLine("+", 48, "-");
            ScaleLine("+", 49, "-");
            Data.Console(Data.supplText, "+");

            Data.Console(Data.text, "\n");
        }

        private static void WinDefeatLine(int columns, Brush scaleColor, bool floor = false)
        {
            double scaleColumns = (double)columns / 10;
            int scale = (floor ? (int)Math.Floor(scaleColumns) : (int)Math.Ceiling(scaleColumns));

            for (int i = 0; i < scale; i++)
                Data.Console(scaleColor, "|");
        }

        private static void WinDefeatScale(int win, int defeat)
        {
            ScaleDraw();

            WinDefeatLine(win, Data.goodText);
            WinDefeatLine(defeat, Data.badText, floor: true);

            Data.Console(Data.text, "\n\n");
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
            {
                int value = u.IsNotSimpleMount() ? u.Wounds.Value : 0;

                if (opponentsWounds.ContainsKey(u.TestType))
                    opponentsWounds[u.TestType] += value;
                else
                    opponentsWounds.Add(u.TestType, value);
            }

            bool isUnitsHasWounds = opponentsWounds[Unit.TestTypeTypes.Unit] > 0;
            bool isOpponentsHasWounds = opponentsWounds[Unit.TestTypeTypes.Enemy] > 0;

            return isUnitsHasWounds && isOpponentsHasWounds;
        }

        public static int FullTest(Unit originalUnit, Unit originalUnitMount,
            Unit originalEnemy, Unit originalEnemyMount)
        {
            round = 0;

            Unit unit = originalUnit.Clone().SetTestType(Unit.TestTypeTypes.Unit);
            Unit enemy = originalEnemy.Clone().SetTestType(Unit.TestTypeTypes.Enemy);

            List<Unit> participants = new List<Unit>() { unit, enemy };
            unit.Mount = originalUnitMount?.Clone().SetTestType(Unit.TestTypeTypes.Unit);
            enemy.Mount = originalEnemyMount?.Clone().SetTestType(Unit.TestTypeTypes.Enemy);

            Dictionary<Unit, List<Unit>> BreakTestOrder = new Dictionary<Unit, List<Unit>>
            {
                [enemy] = new List<Unit> { enemy, enemy.Mount, unit, unit.Mount },
                [unit] = new List<Unit> { unit, unit.Mount, enemy, enemy.Mount },
            };

            if (unit.Name == enemy.Name)
            {
                enemy.Name += " (enemy)";
            }

            if (originalUnitMount != null)
            {
                participants.Add(unit.Mount);
                BreakTestOrder[unit.Mount] = new List<Unit> { unit.Mount, unit, enemy, enemy.Mount };
            }

            if (originalEnemyMount != null)
            {
                participants.Add(enemy.Mount);
                BreakTestOrder[enemy.Mount] = new List<Unit> { enemy.Mount, enemy, unit, unit.Mount };

                if ((unit.Mount != null) && (unit.Mount.Name == enemy.Mount.Name))
                    enemy.Mount.Name += " (enemy)";
            }

            string testHead = $"{unit.Name} vs {enemy.Name}";
            Data.Console(Data.text, testHead);
            Data.Console(Data.supplText, $"\n{new string('-', testHead.Length)}");

            Dictionary<int, int> roundWounds = new Dictionary<int, int>();
            InitRoundWounds(participants, ref roundWounds);

            foreach (List<Unit> unitInOrd in BreakTestOrder.Values)
                unitInOrd[0] = CheckTerror(unitInOrd[0], unitInOrd[1], unitInOrd[2], unitInOrd[3]);

            while (BothOpponentsAreAlive(participants) && (round < 100))
            {
                round += 1;

                Data.Console(Data.supplText, $"\n\nround: {round}\n");

                foreach (Unit u in new List<Unit> { unit, enemy })
                    UnitRoundShow(u, u == unit);

                participants.Sort((a, b) => a.CompareTo(b));

                ShowRoundOrder(participants);

                Dictionary<int, int> attacksRound = new Dictionary<int, int>();

                foreach (Unit u in participants)
                    attacksRound[u.ID] = PrintAttack(u, roundWounds, unit, enemy, unit.Mount);

                InitRoundWounds(participants, ref roundWounds);

                if (!unit.SteamTank)
                    SpecialAttacks.ImpactHit(unit, participants, ref roundWounds, round);

                foreach (Unit u in participants)
                {
                    if (!BothOpponentsAreAlive(participants))
                        continue;

                    Unit actor = UnitFromParticipants(participants, u);
                    Unit opponent = SelectOpponent(participants, u);

                    if ((participants.Count > 2) && (actor.Wounds.Value > 0))
                        Data.Console(Data.supplText, $"\n\n{actor.Name} chose {opponent.Name} as his opponent");

                    int woundsAtStartOfRound = opponent.Wounds.Value;

                    Param.Tests(ref actor, opponent, context: Param.ContextType.Round);

                    if (actor.SteamTank)
                        SpecialAttacks.ImpactHit(actor, participants, ref roundWounds, round);
                        
                    if (actor.HellPitAbomination)
                        SpecialAttacks.HellPitAbomination(ref actor, participants, ref roundWounds, round);

                    if (actor.Giant)
                        SpecialAttacks.GiantAttacks(ref actor, participants, ref roundWounds, round);

                    if (actor.Attacks.Value <= 0)
                        continue;

                    if (actor.PassThisRound)
                    {
                        actor.PassThisRound = false;
                        continue;
                    }

                    roundWounds[opponent.ID] += Round(ref actor, ref opponent, attacksRound[actor.ID], round);

                    bool regeneration = opponent.Regeneration || (opponent.ExtendedRegeneration > 0);

                    if (regeneration && (woundsAtStartOfRound > opponent.Wounds.Value) && !opponent.WoundedWithKillingBlow)
                        Regeneration(opponent, (woundsAtStartOfRound - opponent.Wounds.Value));

                    if (opponent.Wounds.Value <= 0)
                        Data.Console(Data.badText, $"\n\n{opponent.Name} SLAIN");
                }

                Data.Console(Data.text, "\n");

                Dictionary<int, int> battleResult = new Dictionary<int, int>(roundWounds);

                if (BothOpponentsAreAlive(participants))
                {
                    bool draw = true;

                    foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                    {
                        bool unitMount = (u.Key == unit.Mount) && (unit.Wounds.Value > 0);
                        bool enemyMount = (u.Key == enemy.Mount) && (enemy.Wounds.Value > 0);

                        if (!unitMount && !enemyMount)
                            battleResult[u.Key.ID] += RoundBonus(u.Value);
                    }
                        
                    foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                    {
                        if (u.Key.Wounds.Value <= 0)
                            continue;

                        bool unitMount = (u.Key == unit.Mount) && (unit.Wounds.Value > 0);
                        bool enemyMount = (u.Key == enemy.Mount) && (enemy.Wounds.Value > 0);

                        if (unitMount || enemyMount)
                            continue;

                        if (RoundLostBy(u.Value, battleResult))
                        {
                            if (BreakTestFail(u.Value, ref battleResult))
                            {
                                u.Key.Wounds.Value = 0;

                                if (u.Key.Mount != null)
                                    u.Key.Mount.Wounds.Value = 0;

                                Data.Console(Data.text, "\n");
                            }

                            draw = false;

                            if ((u.Key == unit) || (u.Key == enemy))
                                break;
                        }
                    }

                    if (draw)
                        Data.Console(Data.goodText, "\nThe round ended in a draw");
                }
            }

            Data.Console(Data.text, "\nEnd: ");

            if (IsThisUnitDeath(enemy))
            {
                Data.Console(Data.text, $"{unit.Name} win");
                return 1;
            }
            else if (IsThisUnitDeath(unit))
            {
                Data.Console(Data.text, $"{enemy.Name} win");
                return 2;
            }
            else
            {
                Data.Console(Data.text, $"{unit.Name} and {enemy.Name} failed to kill each other");
                return 0;
            }
        }

        private static bool IsThisUnitDeath(Unit unit)
        {
            int nountWounds = unit.Mount != null && unit.Mount.IsNotSimpleMount() ?
                unit.Mount.Wounds.Value : 0;

            return unit.Wounds.Value + nountWounds <= 0;
        }

        private static bool RoundLostBy(List<Unit> units, Dictionary<int, int> roundWounds)
        {
            Unit unit = units[0];
            Unit unitMount = units[1];

            Unit enemy = units[2];
            Unit enemyMount = units[3];

            if ((unit == null) || (unit.Wounds.Value <= 0) || unit.IsSimpleMount())
                return false;

            int unitRoundWounds = roundWounds[unit.ID] + (unitMount != null ? roundWounds[unitMount.ID] : 0);
            int enemyRoundWounds = roundWounds[enemy.ID] + (enemyMount != null ? roundWounds[enemyMount.ID] : 0);

            return unitRoundWounds > enemyRoundWounds;
        }

        private static void AddRoundBonus(string unitName, string bonusName, ref int roundBonus, int bonus)
        {
            Data.Console(Data.supplText, $"\n{unitName} have +{bonus} battle result bonus by {bonusName}");
            roundBonus += 1;
        }

        private static int RoundBonus(List<Unit> units)
        {
            int roundBonus = 0;

            Unit unit = units[2];
            Unit unitMount = units[3];

            Unit enemy = units[0];
            Unit enemyMount = units[1];

            int unitFullSize = (unit.Size * unit.Wounds.Original) + 
                (unitMount != null ? unitMount.Size * unitMount.Wounds.Original : 0);

            int enemyFullSize = (enemy.Size * enemy.Wounds.Original) + 
                (enemyMount != null ? enemyMount.Size * enemyMount.Wounds.Original : 0);

            string unitSide = ((unitMount != null) && (unit.Wounds.Value <= 0)) ?
                unitMount.Name : unit.Name;

            if (unitFullSize > enemyFullSize)
                AddRoundBonus(unitSide, "outnumber", ref roundBonus, bonus: 1);

            if (unit.IsUnit() && unit.IsOptionRealised("standard bearer"))
                AddRoundBonus(unitSide, "standard bearer", ref roundBonus, bonus: 1);

            if (unit.IsOptionRealised("Battle Standard Bearer"))
                AddRoundBonus(unitSide, "BSB", ref roundBonus, bonus: 1);

            if (unit.GetRank() > 1)
            {
                int bonus = Unit.ParamNormalization((unit.GetRank() - 1), onlyZeroCheck: true);
                AddRoundBonus(unitSide, "ranks", ref roundBonus, bonus);
            }

            if (!String.IsNullOrEmpty(unit.AddToCloseCombat))
            {
                int bonus = RandomParamParse(unit.AddToCloseCombat);
                AddRoundBonus(unitSide, "special rules", ref roundBonus, bonus);
            }

            return roundBonus;
        }

        public static Unit SelectOpponent(List<Unit> participants, Unit unit)
        {
            Unit randomOpponent = null;
            bool canBeOpponent = false;

            do
            {
                randomOpponent = participants[Data.rand.Next(participants.Count)];

                if ((randomOpponent.TestType != unit.TestType) && (randomOpponent.Wounds.Value > 0))
                    canBeOpponent = true;

                if ((randomOpponent.Type == Unit.UnitType.Mount) && (randomOpponent.Wounds.Original == 1))
                    canBeOpponent = false;
            }
            while (!canBeOpponent);

            return randomOpponent;
        }

        private static int PrintAttack(Unit unit, Dictionary<int, int> death,
            Unit tUnit, Unit tEnemy, Unit tMount)
        {
            if (unit.Frenzy)
            {
                string blood = unit.BloodFrenzy ? "blood " : String.Empty;
                Data.Console(Data.supplText, $"\n{unit.Name} --> is {blood}frenzy");
            }

            int deathInRound = death[unit.ID];

            if (unit.IsSimpleMount())
            {
                deathInRound = (tMount != null) && (unit.ID == tMount.ID) ?
                    death[tUnit.ID] : death[tEnemy.ID];
            }

            int attackNum = unit.Attacks.Value, attacks = 0;

            int unitFront = (unit.IsHeroOrHisMount() ? 1 : unit.GetFront());

            if ((!unit.IsHeroOrHisMount()) && (unit.Wounds.Value > 0) && (deathInRound > 0))
            {
                unitFront -= deathInRound;
                string multiple = deathInRound > 1 ? "s" : String.Empty;
                Data.Console(Data.supplText, $"\n\n-{deathInRound} unit{multiple} in {unit.Name} front");
            }

            if (unit.IsUnit())
            {
                if (attackNum > 100)
                {
                    for (int i = 0; i < unitFront; i++)
                        attacks += GetRandomAttack(attackNum, unit.Name, i);
                }
                else
                {
                    attacks = attackNum * unitFront;
                }
            }
            else
            {
                attacks = attackNum > 100 ?
                    GetRandomAttack(attackNum, unit.Name, 0) : attackNum;
            }

            return attacks;
        }

        private static int GetRandomAttack(int attackNum, string unitName, int current)
        {
            string param = attackNum.ToString();

            int dices = int.Parse(param[0].ToString());
            int diceSize = int.Parse(param[1].ToString());
            int attacks = int.Parse(param[2].ToString());

            for (int i = 0; i < dices; i++)
                attacks += Data.rand.Next(diceSize) + 1;

            if (current == 0)
                Data.Console(Data.supplText, "\n");

            Data.Console(Data.supplText, $"\n{unitName} attacks for " +
                $"{Unit.GetRandomAttacksLine(attackNum)}: {attacks}");

            return attacks;
        }

        private static void CheckLostFrenzy(ref Unit unit)
        {
            if (!unit.Frenzy || unit.BloodFrenzy || unit.Wounds.Value <= 0)
                return;

            unit.Frenzy = false;
            unit.Attacks.Value -= 1;
            Data.Console(Data.supplText, $"\n{unit.Name} lost his frenzy");
        }

        private static bool BecameBloodFrenzy(ref Unit unit)
        {
            if (!unit.BloodFrenzy || unit.Frenzy)
                return false;

            unit.Frenzy = true;
            unit.Attacks.Value += 1;
            Data.Console(Data.supplText, $" <-- {unit.Name} become subject to blood frenzy");

            return true;
        }

        private static Unit CheckTerror(Unit unit, Unit friend, Unit enemy, Unit enemyFriend)
        {
            bool friendTerrorOrFear = (friend != null ? (friend.Terror || friend.Fear) : false);
            bool enemyFriendTerror = enemyFriend?.Terror ?? false;

            if (unit.IsSimpleMount())
                return unit;

            if ((!enemy.Terror && !enemyFriendTerror) || unit.Terror || friendTerrorOrFear)
                return unit;

            Unit terrorSource = (((enemyFriend != null) && !enemy.Terror) ? enemyFriend : enemy);

            Data.Console(Data.text, $"\n\n{unit.Name} " +
                $"try to resist of terror by {terrorSource.Name} ");

            if (unit.Unbreakable)
            {
                Data.Console(Data.goodText, " --> autopassed (unbreakable)");
            }
            else if (unit.ImmuneToPsychology || unit.Undead || unit.Stupidity)
            {
                Data.Console(Data.goodText, " --> autopassed (immune to psychology)");
            }
            else if (unit.Frenzy)
            {
                Data.Console(Data.goodText, " --> autopassed (frenzy)");
            }
            else if (Dice.Roll(unit, Dice.Types.LD, terrorSource, unit.Leadership.Value, 2))
            {
                Data.Console(Data.goodText, " --> passed");
            }
            else
            {
                unit.Wounds.Value = 0;
                Data.Console(Data.badText, " --> fail");
            }

            return unit;
        }

        public static int Round(ref Unit unit, ref Unit enemy, int attackNumber, int round,
            bool impactHit = false, string impactLine = "", bool afterSteamTankAttack = false)
        {
            int roundWounds = 0;
            int originalAttackNumber = attackNumber;
            bool nobodyDeath = (unit.Wounds.Value > 0) && (enemy.Wounds.Value > 0);
            bool steamImpactHit = impactHit && unit.SteamTank;

            if (nobodyDeath && !steamImpactHit && !afterSteamTankAttack && (attackNumber > 0)) 
                Data.Console(Data.text, "\n");

            for (int i = 0; i < attackNumber; i++)
            {
                bool thisIsAdditionalAttack = i >= originalAttackNumber;

                int wounded = Attack(ref unit, ref enemy, round,
                    out bool additionalAttack, thisIsAdditionalAttack, impactHit, impactLine);

                roundWounds += wounded;

                if ((enemy.Wounds.Value < wounded) && !enemy.WoundedWithKillingBlow)
                {
                    wounded = enemy.Wounds.Value;
                    Data.Console(Data.supplText, $", only {wounded} can be inflicted");
                }

                enemy.Wounds.Value -= wounded;

                if (additionalAttack)
                {
                    Data.Console(Data.supplText, $" <-- {unit.Name} have additional " +
                        $"attack by predatory fighter rule");

                    attackNumber += 1;
                }

                if ((wounded > 0) && BecameBloodFrenzy(ref unit))
                {
                    attackNumber += 1;
                }
            }

            enemy.Wounds.Value = Unit.ParamNormalization(enemy.Wounds.Value, onlyZeroCheck: true);

            return roundWounds;
        }

        private static void Regeneration(Unit unit, int roundWounds)
        {
            Data.Console(Data.text, "\n");

            int regeneration = (unit.ExtendedRegeneration > 0 ? unit.ExtendedRegeneration : 4);

            for (int i = 0; i < roundWounds; i++)
            {
                string regenarationRate = regeneration == 4 ?
                    $"({regeneration}+) " : String.Empty;

                Data.Console(Data.text, $"\n{unit.Name} --> regeneration ({regeneration}+) ");

                if (Dice.Roll(unit, Dice.Types.REGENERATION, unit, regeneration))
                {
                    Data.Console(Data.goodText, " --> success");
                    unit.Wounds.Value += 1;
                }
                else
                {
                    Data.Console(Data.badText, " --> fail");
                }
            }
        }

        private static void ShowRoundOrder(List<Unit> allParticipants)
        {
            Data.Console(Data.supplText, "\nround fight order:");

            foreach (Unit u in allParticipants)
            {
                string participant = u == allParticipants[0] ? String.Empty : " -->";
                Data.Console(Data.supplText, $"{participant} {u.Name}");
            }
        }

        public static bool CheckInitiative(Unit unit, Unit enemy)
        {
            bool unitIsUnit = (unit.TestType == Unit.TestTypeTypes.Unit);
            bool enemyIsEnemy = (enemy.TestType == Unit.TestTypeTypes.Enemy);

            if ((round == 1) && unitIsUnit && enemyIsEnemy && (!enemy.HitFirst))
            {
                return true;
            }
            else if ((round == 1) && !unitIsUnit && !enemyIsEnemy && (!unit.HitFirst))
            {
                return true;
            }
            else if (unit.HitFirst && !enemy.HitFirst)
            {
                return true;
            }
            else if (!unit.HitFirst && enemy.HitFirst)
            {
                return false;
            }
            else if (unit.HitLast && !enemy.HitLast)
            {
                return false;
            }
            else if (!unit.HitLast && enemy.HitLast)
            {
                return true;
            }
            else if (unit.Initiative.Value > enemy.Initiative.Value)
            {
                return true;
            }
            else if (unit.Initiative.Value < enemy.Initiative.Value)
            {
                return false;
            }
            else
            {
                return Dice.Roll(unit, Dice.Types.I, enemy, 4, hiddenDice: true);
            }
        }

        private static bool BreakTestFail(List<Unit> units, ref Dictionary<int, int> woundInRound)
        {
            Unit unit = units[0];
            Unit unitFriend = units[1];

            Unit enemy = units[2];
            Unit enemyFriend = units[3];

            Data.Console(Data.text, $"\n{unit.Name} break test --> ");

            int temoraryLeadership = unit.Leadership.Value;

            if (unit.Stubborn)
            {
                Data.Console(Data.text, "stubborn --> ");
            }
            else
            {
                temoraryLeadership -= woundInRound[unit.ID] + (unitFriend != null ? woundInRound[unitFriend.ID] : 0);
                temoraryLeadership += woundInRound[enemy.ID] + (enemyFriend != null ? woundInRound[enemyFriend.ID] : 0);
            }

            temoraryLeadership = Unit.ParamNormalization(temoraryLeadership);

            bool enemyFearOrTerror = (enemy.Wounds.Value > 0) && enemy.IsFearOrTerror();
            bool enemyMountFearOrTerror = (enemyFriend != null) && (enemyFriend.Wounds.Value > 0) ?
                enemyFriend.IsFearOrTerror() : false;

            bool unitFearOrTerror = (unit.Wounds.Value > 0) && unit.IsFearOrTerror();
            bool unitMountFearOrTerror = (unitFriend != null) && (unitFriend.Wounds.Value > 0) ?
                unitFriend.IsFearOrTerror() : false;

            int unitAreMoreOfThem = (unit.Wounds.Original * unit.Size) +
                (unitFriend != null ? (unitFriend.Wounds.Original * unitFriend.Size) : 0);
            int enemyAreMoreOfThem = (enemy.Wounds.Original * enemy.Size) +
                (enemyFriend != null ? (enemyFriend.Wounds.Original * enemyFriend.Size) : 0);

            bool thereAreMoreOfThem = unitAreMoreOfThem < enemyAreMoreOfThem;

            bool itNotFear = (unit.ImmuneToPsychology || unit.Stupidity ||
                unit.Undead || unitFearOrTerror || unitMountFearOrTerror);

            if (unit.Unbreakable)
            {
                Data.Console(Data.text, "unbreakable");
            }
            else if (thereAreMoreOfThem && (enemyFearOrTerror || enemyMountFearOrTerror) && !itNotFear)
            {
                string enemyName = enemyFearOrTerror ? enemy.Name : enemyFriend.Name;
                Data.Console(Data.badText, $"autobreak by {enemyName} fear");
                return true;
            }
            else
            {
                if (Dice.Roll(unit, Dice.Types.LD, enemy, temoraryLeadership, out int dice, diceNum: 2, breakTest: true))
                {
                    Data.Console(Data.goodText, " --> passed");
                }
                else
                {
                    Data.Console(Data.badText, " --> fail");

                    if (unit.Undead)
                    {
                        int additionalWounds = (dice - temoraryLeadership);

                        Data.Console(Data.badText, $" --> {additionalWounds} additional wounds");

                        if (unit.Wounds.Value < additionalWounds)
                            additionalWounds = unit.Wounds.Value;

                        woundInRound[unit.ID] += additionalWounds;
                        unit.Wounds.Value -= additionalWounds;

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            CheckLostFrenzy(ref unit);

            return false;
        }

        private static int Attack(ref Unit unit, ref Unit enemy, int round, out bool additionalAttack,
            bool thisIsAdditionalAttack = false, bool impactHit = false, string impactLine = "")
        {
            attackIsPoisoned = false;
            attackWithKillingBlow = false;
            additionalAttack = false;

            int woundsAtStart = enemy.Wounds.Value;

            if ((unit.Wounds.Value > 0) && (enemy.Wounds.Value > 0))
            {
                string spacer = !impactHit ? " " : String.Empty;
                Data.Console(Data.text, $"\n{unit.Name} --> hit{spacer}");

                if (impactHit)
                    Data.Console(Data.supplText, $" ({impactLine} impact hit)");

                int diceForHit = 0;

                if (impactHit || Hit(unit, enemy, round, out diceForHit))
                {
                    Param.Tests(ref enemy, unit, context: Param.ContextType.Hit);

                    if (unit.PredatoryFighter && !thisIsAdditionalAttack && (diceForHit == 6))
                        additionalAttack = true;

                    if (enemy.Wounds.Value <= 0)
                        return woundsAtStart;

                    Data.Console(Data.text, " --> wound ");

                    if (
                        (PoisonedAttack(unit, enemy, impactHit) || Wound(unit, enemy, round))
                        &&
                        (KillingAttack(unit, enemy) || NotAS(ref unit, enemy))
                        &&
                        NotWard(ref unit, enemy)
                        &&
                        NotDiscountWound(unit, ref enemy)
                    ) {
                        if (attackWithKillingBlow && enemy.IsHeroOrHisMount())
                        {
                            Data.Console(Data.badText, $" --> {enemy.Name} SLAIN");
                            enemy.WoundedWithKillingBlow = true;
                            return enemy.Wounds.Value;
                        }
                        else
                        {
                            Data.Console(Data.badText, $" --> {enemy.Name} WOUND");

                            Param.Tests(ref enemy, unit, context: Param.ContextType.Wound);

                            if (enemy.Wounds.Value > 0)
                                return WoundsNumbers(unit, ref enemy);
                            else
                                return woundsAtStart;
                        }
                    }
                }
                Data.Console(Data.goodText, " --> fail");
            }
            return 0;
        }

        public static void RandomParamValues(string param, out int diceNumber, out int diceSize, out int addSomething)
        {
            string[] randParams = param.Split('D');

            bool diceNumberParse = int.TryParse(randParams[0], out diceNumber);

            if (!diceNumberParse)
                diceNumber = 1;

            if (randParams[1].Contains('+'))
            {
                string[] randNumber = randParams[1].Split('+');
                int.TryParse(randNumber[0], out diceSize);
                int.TryParse(randNumber[1], out addSomething);
            }
            else
            {
                int.TryParse(randParams[1], out diceSize);
                addSomething = 0;
            }
        }

        public static int RandomParamParse(string param)
        {
            int randomParam = 0;

            if (!param.Contains("D"))
            {
                randomParam = int.Parse(param);
            }
            else
            {
                RandomParamValues(param, out int diceNumber, out int diceSize, out int addSomething);

                for (int i = 0; i < diceNumber; i++)
                    randomParam += Data.rand.Next(diceSize) + 1 + addSomething;
            }

            return randomParam;
        }

        private static int WoundsNumbers(Unit unit, ref Unit enemy)
        {
            if (unit.AutoDeath)
            {
                Data.Console(Data.text, " <-- lose all wounds");
                return enemy.Wounds.Value;
            }

            if (String.IsNullOrEmpty(unit.MultiWounds) || enemy.NoMultiWounds)
                return 1;

            int multiwounds = RandomParamParse(unit.MultiWounds);

            Data.Console(Data.text, $" <-- {multiwounds} multiple wounds");

            if (enemy.FirstWoundDiscount)
            {
                Data.Console(Data.text, " <-- first wound discount");

                enemy.FirstWoundDiscount = false;

                multiwounds -= 1;
            }

            return multiwounds;
        }

        private static bool PoisonedAttack(Unit unit, Unit enemy, bool impactHit = false)
        {
            if (impactHit || !unit.PoisonAttack || enemy.ImmuneToPoison || (Dice.lastDice != 6))
                return false;

            attackIsPoisoned = true;
            Data.Console(Data.text, "(poison)");
            return true;  
        }

        private static bool KillingAttack(Unit unit, Unit enemy)
        {
            bool normalKillingBlowActual = unit.KillingBlow && !enemy.LargeBase;
            bool killingBlowActual = unit.HeroicKillingBlow || normalKillingBlowActual;
            bool killingBlow = killingBlowActual && (Dice.lastDice == 6);

            bool extendedKillingBlow = (unit.ExtendedKillingBlow > 0) &&
                !enemy.LargeBase && (Dice.lastDice >= unit.ExtendedKillingBlow);

            if ((killingBlow || extendedKillingBlow) && !enemy.NoKillingBlow && !attackIsPoisoned)
            {
                attackWithKillingBlow = true;
                string heroic = unit.HeroicKillingBlow ? "heroic " : String.Empty;
                Data.Console(Data.text, " --> {heroic}killing blow");
                return true;
            }

            if ((enemy.Armour != null) && !enemy.Armour.Null && !unit.NoArmour)
            {
                Data.Console(Data.text, " --> AS ");
            }
                
            return false;
        }

        private static bool Hit(Unit unit, Unit enemy, int round, out int dice)
        {
            int chance = 4;
            dice = 0;

            if (unit.AutoHit || enemy.SteamTank)
            {
                Data.Console(Data.text, "(autohit)");
                return true;
            }
            else if (unit.HitOn > 0)
            {
                chance = unit.HitOn;
            }
            else if (enemy.OpponentHitOn > 0)
            {
                chance = enemy.OpponentHitOn;
            }
            else if (unit.WeaponSkill.Value > enemy.WeaponSkill.Value)
            {
                chance = 3;
            }
            else if ((unit.WeaponSkill.Value * 2) < enemy.WeaponSkill.Value)
            {
                chance = 5;
            }

            return Dice.Roll(unit, Dice.Types.WS, enemy, chance, dice: out dice, round: round);
        }

        private static void StrengthBonus(ref int strength, int bonus)
        {
            strength += bonus;
            strength = Unit.ParamNormalization(strength);
        }

        private static bool Wound(Unit unit, Unit enemy, int round)
        {
            int chance = 4;
            int strength = unit.Strength.Value;

            if ((unit.Lance || unit.Flail || unit.Resolute) && (round == 1))
                StrengthBonus(ref strength, (unit.Resolute ? 1 : 2));

            if ((unit.ChargeStrengthBonus > 0) && (round == 1))
                StrengthBonus(ref strength, unit.ChargeStrengthBonus);

            if (unit.AutoWound)
            {
                Data.Console(Data.text, "(autowound)");
                return true;
            }
            else if (unit.WoundOn > 0)
            {
                chance = unit.WoundOn;
            }
            else if (strength == (enemy.Toughness.Value + 1))
            {
                chance = 3;
            }
            else if (strength > (enemy.Toughness.Value + 1))
            {
                chance = 2;
            }
            else if ((strength + 1) == enemy.Toughness.Value)
            {
                chance = 5;
            }
            else if ((strength + 2) == enemy.Toughness.Value)
            {
                chance = 6;
            }
            else if ((strength + 2) < enemy.Toughness.Value)
            {
                Data.Console(Data.supplText, "(impossible)");
                return false;
            }

            return Dice.Roll(unit, Dice.Types.S, enemy, chance);
        }

        private static bool NotAS(ref Unit unit, Unit enemy)
        {
            if ((enemy.Armour == null) || (enemy.Armour.Null) || unit.NoArmour)
                return true;

            int chanceValue = (unit.Strength.Value + unit.ArmourPiercing) - 3;
            int chance = Unit.ParamNormalization(chanceValue, onlyZeroCheck: true);

            chance += enemy.Armour.Value;

            bool armourFail = Dice.Roll(unit, Dice.Types.AS, enemy, chance);

            if (!armourFail)
                Param.Tests(ref unit, enemy, context: Param.ContextType.ArmourSave);

            return armourFail;
        }

        private static bool NotDiscountWound(Unit unit, ref Unit enemy)
        {
            if (!enemy.FirstWoundDiscount || !String.IsNullOrEmpty(unit.MultiWounds))
                return true;

            Data.Console(Data.text, " --> first wound discount");

            enemy.FirstWoundDiscount = false;

            return false;
        }

        private static bool NotWard(ref Unit unit, Unit enemy)
        {
            int ward = ((enemy.Ward == null) ? 0 : enemy.Ward.Value);

            if ((enemy.WardForFirstWound > 0) && (enemy.Wounds.Value == enemy.Wounds.Original))
            {
                ward = enemy.WardForFirstWound;
                enemy.WardForFirstWound = 0;
            }

            if ((enemy.WardForLastWound > 0) && (enemy.Wounds.Value == 1))
                ward = enemy.WardForLastWound;

            if ((ward <= 0) || unit.NoWard)
                return true;

            Data.Console(Data.text, " --> ward ");

            bool wardFail = Dice.Roll(unit, Dice.Types.WARD, enemy, ward);

            if (!wardFail)
                Param.Tests(ref unit, enemy, context: Param.ContextType.WardSave);

            return wardFail;
        }

        private static Unit UnitFromParticipants(List<Unit> participants, Unit unit) =>
            participants.Where(x => x.ID == unit.ID).FirstOrDefault();

        private static void UnitRoundShow(Unit unit, bool firstLine)
        {
            string uLine = unit.Wounds.Value > 0 ?
                $"{unit.Name}: {unit.Wounds.Value}W" : String.Empty;

            bool monstrousMount = (unit.Mount != null) &&
                (unit.Mount.Wounds.Value > 0) && unit.Mount.IsNotSimpleMount();

            string uMount = monstrousMount ?
                $"{unit.Mount.Name}: {unit.Mount.Wounds.Value}W" : String.Empty;

            string bothLine = !String.IsNullOrEmpty(uLine) &&
                !String.IsNullOrEmpty(uMount) ? " + " : String.Empty;

            Data.Console(Data.supplText, $"{uLine}{bothLine}{uMount}" +
                $"{(firstLine ? ", " : String.Empty)}");
        }
    }
}
