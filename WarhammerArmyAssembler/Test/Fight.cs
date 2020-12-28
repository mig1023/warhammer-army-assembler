using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Test
{
    class Fight
    {
        static int round = 0;

        static bool attackWithKillingBlow = false;
        static bool attackIsPoisoned = false;

        public static void BattleRoyaleTest(Unit unit, Unit unitMount)
        {
            foreach (string enemyGroupName in Enemy.GetEnemiesGroups())
            {
                string currentText = Interface.TestUnit.GetFullConsoleText();

                Test.Data.Console(Test.Data.supplText, "{0}\n\n", enemyGroupName.ToUpper());

                if (currentText == String.Empty)
                    Test.Data.Console(Test.Data.text, "\n");

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
            string currentText = Interface.TestUnit.GetFullConsoleText();

            Interface.TestUnit.PreventConsoleOutput(prevent: true);

            for (int i = 0; i < 1000; i++)
                result[FullTest(unit, unitMount, enemy, enemyMount)] += 1;

            Interface.TestUnit.PreventConsoleOutput(prevent: false);

            if (royalNotation)
                Test.Data.Console(Test.Data.text, "vs {0}: win: {1:f1}% defeat: {2:f1}%\n", enemy.TestListName, (double)result[1] / 10, (double)result[2] / 10);                
            else
            {
                Test.Data.Console(Test.Data.text, "{0} win: {1:f1}%\n{2} win: {3:f1}%", unit.Name, (double)result[1] / 10, enemy.TestListName, (double)result[2] / 10);

                if (result[0] > 0)
                    Test.Data.Console(Test.Data.text, "\nNobody win: {0:f1}%", (double)result[0] / 10);
            }

            if (currentText == String.Empty)
                Test.Data.Console(Test.Data.text, "\n");

            WinDefeatScale(result[1], result[2]);
        }

        private static void WinDefeatScale(double win, double defeat)
        {
            Brush scaleColor = Test.Data.goodText;

            foreach(double param in new List<double> { win, defeat })
            {
                int scale = (int)param / 10;

                for (int i = 0; i < scale; i++)
                    Test.Data.Console(scaleColor, "|");

                scaleColor = Test.Data.badText;
            }

            Test.Data.Console(Test.Data.text, "\n\n");
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
            Test.Data.testConsole.Clear();

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

            Test.Data.Console(Test.Data.text, "{0} vs {1}", unit.Name,  enemy.Name);

            Dictionary<int, int> roundWounds = new Dictionary<int, int>();
            InitRoundWounds(participants, ref roundWounds);

            foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                u.Value[0] = CheckTerror(u.Value[0], u.Value[1], u.Value[2], u.Value[3]);

            while (BothOpponentsAreAlive(participants) && (round < 100))
            {
                round += 1;

                Test.Data.Console(Test.Data.supplText, "\n\nround: {0}\n", round);

                foreach (Unit u in new List<Unit> { unit, enemy })
                    UnitRoundShow(u, u == unit);

                participants.Sort((a, b) => a.CompareTo(b));

                if (round == 1)
                    participants.Sort((a, b) => a.CompareTo(b));

                ShowRoundOrder(participants);

                Dictionary<int, int> attacksRound = new Dictionary<int, int>();

                foreach (Unit u in participants)
                    attacksRound[u.ID] = PrintAttack(u, u.Attacks, roundWounds, unit, enemy, unit.Mount);

                InitRoundWounds(participants, ref roundWounds);

                if (!unit.SteamTank)
                    SpecialAttacks.ImpactHit(unit, participants, ref roundWounds, round);

                foreach (Unit u in participants)
                    if (BothOpponentsAreAlive(participants))
                    {
                        Unit actor = UnitFromParticipants(participants, u);
                        Unit opponent = SelectOpponent(participants, u);

                        if ((participants.Count > 2) && (actor.Wounds > 0))
                            Test.Data.Console(Test.Data.supplText, "\n\n{0} chose {1} as his opponent", actor.Name, opponent.Name);

                        int woundsAtStartOfRound = opponent.Wounds;

                        Param.Tests(ref actor, opponent, context: Param.ContextType.Round);

                        if (actor.SteamTank)
                            SpecialAttacks.ImpactHit(actor, participants, ref roundWounds, round);
                        
                        if (actor.HellPitAbomination)
                            SpecialAttacks.HellPitAbomination(ref actor, participants, ref roundWounds, round);

                        if (actor.Giant)
                            SpecialAttacks.GiantAttacks(ref actor, participants, ref roundWounds, round);

                        if (actor.Attacks <= 0)
                            continue;

                        if (actor.PassThisRound)
                        {
                            actor.PassThisRound = false;
                            continue;
                        }

                        roundWounds[opponent.ID] += Round(ref actor, ref opponent, attacksRound[actor.ID], round);

                        if (opponent.Regeneration && (woundsAtStartOfRound > opponent.Wounds) && !opponent.WoundedWithKillingBlow)
                            Regeneration(opponent, (woundsAtStartOfRound - opponent.Wounds));

                        if (opponent.Wounds <= 0)
                            Test.Data.Console(Test.Data.badText, "\n\n{0} SLAIN", opponent.Name);
                    }

                Test.Data.Console(Test.Data.text, "\n");

                Dictionary<int, int> battleResult = new Dictionary<int, int>(roundWounds);

                if (BothOpponentsAreAlive(participants))
                {
                    bool draw = true;

                    foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                        if (!((u.Key == unit.Mount) && (unit.Wounds > 0)) && !((u.Key == enemy.Mount) && (enemy.Wounds > 0)))
                            battleResult[u.Key.ID] += RoundBonus(u.Value, roundWounds);
                        
                    foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                    {
                        if (u.Key.Wounds <= 0)
                            continue;

                        if (((u.Key == unit.Mount) && (unit.Wounds > 0)) || ((u.Key == enemy.Mount) && (enemy.Wounds > 0)))
                            continue;

                        if (RoundLostBy(u.Value, battleResult))
                        {
                            if (BreakTestFail(u.Value, ref battleResult))
                            {
                                u.Key.Wounds = 0;

                                if (u.Key.Mount != null)
                                    u.Key.Mount.Wounds = 0;

                                Test.Data.Console(Test.Data.text, "\n");
                            }

                            draw = false;

                            if ((u.Key == unit) || (u.Key == enemy))
                                break;
                        }
                    }

                    if (draw)
                        Test.Data.Console(Test.Data.goodText, "\nThe round ended in a draw");
                }
            }

            Test.Data.Console(Test.Data.text, "\nEnd: ");

            if (enemy.Wounds + (enemy.Mount != null && enemy.Mount.IsNotSimpleMount() ? enemy.Mount.Wounds : 0) <= 0)
            {
                Test.Data.Console(Test.Data.text, "{0} win", unit.Name);
                return 1;
            }
            else if (unit.Wounds + (unit.Mount != null && unit.Mount.IsNotSimpleMount() ? unit.Mount.Wounds : 0) <= 0)
            {
                Test.Data.Console(Test.Data.text, "{0} win", enemy.Name);
                return 2;
            }
            else
            {
                Test.Data.Console(Test.Data.text, "{0} and {1} failed to kill each other", unit.Name, enemy.Name);
                return 0;
            }
        }

        private static bool RoundLostBy(List<Unit> units, Dictionary<int, int> roundWounds)
        {
            Unit unit = units[0];
            Unit unitMount = units[1];

            Unit enemy = units[2];
            Unit enemyMount = units[3];

            if ((unit == null) || (unit.Wounds <= 0) || unit.IsSimpleMount())
                return false;

            int unitRoundWounds = roundWounds[unit.ID] + (unitMount != null ? roundWounds[unitMount.ID] : 0);
            int enemyRoundWounds = roundWounds[enemy.ID] + (enemyMount != null ? roundWounds[enemyMount.ID] : 0);

            return unitRoundWounds > enemyRoundWounds;
        }

        private static void AddRoundBonus(string unitName, string bonusName, ref int roundBonus, int bonus)
        {
            Test.Data.Console(Test.Data.supplText, String.Format("\n{0} have +{1} battle result bonus by {2}", unitName, bonus, bonusName));
            roundBonus += 1;
        }

        private static int RoundBonus(List<Unit> units, Dictionary<int, int> roundWounds)
        {
            int roundBonus = 0;

            Unit unit = units[2];
            Unit unitMount = units[3];

            Unit enemy = units[0];
            Unit enemyMount = units[1];

            int unitFullSize = (unit.Size * unit.OriginalWounds) + (unitMount != null ? unitMount.Size * unitMount.OriginalWounds : 0);
            int enemyFullSize = (enemy.Size * enemy.OriginalWounds) + (enemyMount != null ? enemyMount.Size * enemyMount.OriginalWounds : 0);

            string unitSide = (((unitMount != null) && (unit.Wounds <= 0)) ? unitMount.Name : unit.Name);

            if (unitFullSize > enemyFullSize)
                AddRoundBonus(unitSide, "outnumber", ref roundBonus, bonus: 1);

            if (unit.IsUnit() && unit.IsOptionRealised("standard bearer"))
                AddRoundBonus(unitSide, "standard bearer", ref roundBonus, bonus: 1);

            if (unit.IsOptionRealised("Battle Standard Bearer"))
                AddRoundBonus(unitSide, "BSB", ref roundBonus, bonus: 1);

            if (unit.GetRank() > 1)
                AddRoundBonus(unitSide, "ranks", ref roundBonus, bonus: Unit.ParamNormalization((unit.GetRank() - 1), onlyZeroCheck: true));

            if (!String.IsNullOrEmpty(unit.AddToCloseCombat))
            {
                int addBonus = RandomParamParse(unit.AddToCloseCombat);
                AddRoundBonus(unitSide, "special rules", ref roundBonus, bonus: addBonus);
            }

            return roundBonus;
        }

        public static Unit SelectOpponent(List<Unit> participants, Unit unit)
        {
            Unit randomOpponent = null;
            bool canBeOpponent = false;

            do
            {
                randomOpponent = participants[Test.Data.rand.Next(participants.Count)];

                if ((randomOpponent.TestType != unit.TestType) && (randomOpponent.Wounds > 0))
                    canBeOpponent = true;

                if ((randomOpponent.Type == Unit.UnitType.Mount) && (randomOpponent.OriginalWounds == 1))
                    canBeOpponent = false;
            }
            while (!canBeOpponent);

            return randomOpponent;
        }

        private static int PrintAttack(Unit unit, int attackNum, Dictionary<int, int> death,
            Unit tUnit, Unit tEnemy, Unit tMount)
        {
            if (unit.Frenzy)
                Test.Data.Console(Test.Data.supplText, "\n{0} --> is {1}frenzy", unit.Name, (unit.BloodFrenzy ? "blood " : String.Empty));

            int deathInRound = death[unit.ID];

            if (unit.IsSimpleMount())
                deathInRound = ((tMount != null) && (unit.ID == tMount.ID) ? death[tUnit.ID] : death[tEnemy.ID]);

            if ((!unit.IsHeroOrHisMount()) && (unit.Wounds > 0) && (deathInRound > 0))
            {
                attackNum -= deathInRound;
                Test.Data.Console(Test.Data.supplText, "\n-{0} attack {1}", deathInRound, unit.Name);
            }

            if (unit.IsUnit() && ((unit.Wounds * unit.OriginalAttacks) < attackNum))
                attackNum = unit.Wounds * unit.OriginalAttacks;

            return attackNum;
        }

        private static void CheckLostFrenzy(ref Unit unit)
        {
            if (unit.Frenzy && !unit.BloodFrenzy && (unit.Wounds > 0))
            {
                unit.Frenzy = false;
                unit.Attacks -= 1;
                Test.Data.Console(Test.Data.supplText, "\n{0} lost his frenzy", unit.Name);
            }
        }

        private static bool BecameBloodFrenzy(ref Unit unit)
        {
            if (unit.BloodFrenzy && !unit.Frenzy)
            {
                unit.Frenzy = true;
                unit.Attacks += 1;
                Test.Data.Console(Test.Data.supplText, " <-- {0} become subject to blood frenzy", unit.Name);

                return true;
            }
            else
                return false;
        }

        private static Unit CheckTerror(Unit unit, Unit friend, Unit enemy, Unit enemyFriend)
        {
            bool friendTerrorOrFear = (friend != null ? (friend.Terror || friend.Fear) : false);
            bool enemyFriendTerror = (enemyFriend != null ? enemyFriend.Terror : false);

            if (unit.IsSimpleMount())
                return unit;

            if ((!enemy.Terror && !enemyFriendTerror) || unit.Terror || friendTerrorOrFear)
                return unit;

            Unit terrorSource = (((enemyFriend != null) && !enemy.Terror) ? enemyFriend : enemy);

            Test.Data.Console(Test.Data.text, "\n{0} try to resist of terror by {1} ", unit.Name, terrorSource.Name);

            if (unit.Unbreakable)
                Test.Data.Console(Test.Data.goodText, " --> autopassed (unbreakable)");
            else if (unit.ImmuneToPsychology || unit.Undead || unit.Stupidity)
                Test.Data.Console(Test.Data.goodText, " --> autopassed (immune to psychology)");
            else if (unit.Frenzy)
                Test.Data.Console(Test.Data.goodText, " --> autopassed (frenzy)");
            else if (Dice.Roll(unit, Dice.Types.LD, terrorSource, unit.Leadership, 2))
                Test.Data.Console(Test.Data.goodText, " --> passed");
            else
            {
                unit.Wounds = 0;
                Test.Data.Console(Test.Data.badText, " --> fail");
            }

            return unit;
        }

        public static int Round(ref Unit unit, ref Unit enemy, int attackNumber, int round,
            bool impactHit = false, string impactLine = "", bool afterSteamTankAttack = false)
        {
            int roundWounds = 0;
            int originalAttackNumber = attackNumber;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0) && !(impactHit && unit.SteamTank) && !afterSteamTankAttack && (attackNumber > 0)) 
                Test.Data.Console(Test.Data.text, "\n");

            for (int i = 0; i < attackNumber; i++)
            {
                int wounded = Attack(ref unit, ref enemy, round, out bool additionalAttack, (i >= originalAttackNumber), impactHit, impactLine);
                roundWounds += wounded;

                if (enemy.Wounds < wounded)
                {
                    wounded = enemy.Wounds;
                    Test.Data.Console(Test.Data.supplText, ", only {0} can be inflicted", wounded);
                }

                enemy.Wounds -= wounded;

                if (additionalAttack)
                {
                    Test.Data.Console(Test.Data.supplText, " <-- {0} have additional attack by predatory fighter rule", unit.Name);
                    attackNumber += 1;
                }

                if ((wounded > 0) && BecameBloodFrenzy(ref unit))
                    attackNumber += 1;
            }

            enemy.Wounds = Unit.ParamNormalization(enemy.Wounds, onlyZeroCheck: true);

            return roundWounds;
        }

        private static void Regeneration(Unit unit, int roundWounds)
        {
            Test.Data.Console(Test.Data.text, "\n");

            for (int i = 0; i < roundWounds; i++)
            {
                Test.Data.Console(Test.Data.text, "\n{0} --> regeneration ", unit.Name);

                if (Dice.Roll(unit, Dice.Types.REGENERATION, unit, 4))
                {
                    Test.Data.Console(Test.Data.goodText, " --> success");
                    unit.Wounds += 1;
                }
                else
                    Test.Data.Console(Test.Data.badText, " --> fail");
            }
        }

        private static void ShowRoundOrder(List<Unit> allParticipants)
        {
            Test.Data.Console(Test.Data.supplText, "\nround fight order:");

            foreach (Unit u in allParticipants)
                Test.Data.Console(Test.Data.supplText, "{0} {1}", (u == allParticipants[0] ? String.Empty : " -->"), u.Name);
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
                return Dice.Roll(unit, Dice.Types.I, enemy, 4, hiddenDice: true);
        }

        private static bool BreakTestFail(List<Unit> units, ref Dictionary<int, int> woundInRound)
        {
            Unit unit = units[0];
            Unit unitFriend = units[1];

            Unit enemy = units[2];
            Unit enemyFriend = units[3];

            Test.Data.Console(Test.Data.text, "\n{0} break test --> ", unit.Name);

            int temoraryLeadership = unit.Leadership;

            if (unit.Stubborn)
                Test.Data.Console(Test.Data.text, "stubborn --> ");
            else
            {
                temoraryLeadership -= woundInRound[unit.ID] + (unitFriend != null ? woundInRound[unitFriend.ID] : 0);
                temoraryLeadership += woundInRound[enemy.ID] + (enemyFriend != null ? woundInRound[enemyFriend.ID] : 0);
            }

            temoraryLeadership = Unit.ParamNormalization(temoraryLeadership);

            bool enemyFearOrTerror = ((enemy.Wounds > 0) && enemy.IsFearOrTerror());
            bool enemyMountFearOrTerror = ((enemyFriend != null) && (enemyFriend.Wounds > 0) ? enemyFriend.IsFearOrTerror() : false);

            bool unitFearOrTerror = ((unit.Wounds > 0) && unit.IsFearOrTerror());
            bool unitMountFearOrTerror = ((unitFriend != null) && (unitFriend.Wounds > 0) ? unitFriend.IsFearOrTerror() : false);

            bool thereAreMoreOfThem = (
                (unit.OriginalWounds * unit.Size) + (unitFriend != null ? (unitFriend.OriginalWounds * unitFriend.Size) : 0) <
                (enemy.OriginalWounds * enemy.Size) + (enemyFriend != null ? (enemyFriend.OriginalWounds * enemyFriend.Size) : 0)
            );

            if (unit.Unbreakable)
                Test.Data.Console(Test.Data.text, "unbreakable");
            else if (
                thereAreMoreOfThem
                &&
                (enemyFearOrTerror || enemyMountFearOrTerror)
                &&
                !(unit.ImmuneToPsychology || unit.Stupidity || unit.Undead || unitFearOrTerror || unitMountFearOrTerror))
            {
                Test.Data.Console(Test.Data.badText, "autobreak by {0} fear", (enemyFearOrTerror ? enemy.Name : enemyFriend.Name));
                return true;
            }
            else
            {
                if (Dice.Roll(unit, Dice.Types.LD, enemy, temoraryLeadership, out int dice, diceNum: 2, breakTest: true))
                    Test.Data.Console(Test.Data.goodText, " --> passed");
                else
                {
                    Test.Data.Console(Test.Data.badText, " --> fail");

                    if (unit.Undead)
                    {
                        int additionalWounds = (dice - temoraryLeadership);

                        Test.Data.Console(Test.Data.badText, " --> {0} additional wounds", additionalWounds);

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

        private static int Attack(ref Unit unit, ref Unit enemy, int round, out bool additionalAttack,
            bool thisIsAdditionalAttack = false, bool impactHit = false, string impactLine = "")
        {
            attackIsPoisoned = false;
            attackWithKillingBlow = false;
            additionalAttack = false;

            int woundsAtStart = enemy.Wounds;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0))
            {
                if (!impactHit)
                    Test.Data.Console(Test.Data.text, "\n{0} --> hit ", unit.Name);
                else
                {
                    Test.Data.Console(Test.Data.text, "\n{0} --> hit", unit.Name);
                    Test.Data.Console(Test.Data.supplText, " ({0} impact hit)", impactLine);
                }

                int diceForHit = 0;

                if (impactHit || Hit(unit, enemy, round, out diceForHit))
                {
                    Param.Tests(ref enemy, unit, context: Param.ContextType.Hit);

                    if (unit.PredatoryFighter && !thisIsAdditionalAttack && (diceForHit == 6))
                        additionalAttack = true;

                    if (enemy.Wounds <= 0)
                        return woundsAtStart;

                    Test.Data.Console(Test.Data.text, " --> wound ");

                    if (
                        (PoisonedAttack(unit, impactHit) || Wound(unit, enemy, round))
                        &&
                        (KillingAttack(unit, enemy) || NotAS(ref unit, enemy))
                        &&
                        NotWard(ref unit, enemy)
                        &&
                        NotDiscountWound(ref enemy)
                    ) {
                        if (attackWithKillingBlow && enemy.IsHeroOrHisMount())
                        {
                            Test.Data.Console(Test.Data.badText, " --> {0} SLAIN", enemy.Name);
                            enemy.WoundedWithKillingBlow = true;
                            return enemy.Wounds;
                        }
                        else
                        {
                            Test.Data.Console(Test.Data.badText, " --> {0} {1}", enemy.Name, "WOUND");

                            Param.Tests(ref enemy, unit, context: Param.ContextType.Wound);

                            if (enemy.Wounds > 0)
                                return WoundsNumbers(unit, enemy);
                            else
                                return woundsAtStart;
                        }
                    }
                }
                Test.Data.Console(Test.Data.goodText, " --> fail");
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
                _ = int.TryParse(randNumber[0], out diceSize);
                _ = int.TryParse(randNumber[1], out addSomething);
            }
            else
            {
                _ = int.TryParse(randParams[1], out diceSize);
                addSomething = 0;
            }
        }

        public static int RandomParamParse(string param)
        {
            int randomParam = 0;

            if (!param.Contains("D"))
                randomParam = int.Parse(param);
            else
            {
                RandomParamValues(param, out int diceNumber, out int diceSize, out int addSomething);

                for (int i = 0; i < diceNumber; i++)
                    randomParam += Test.Data.rand.Next(diceSize) + 1 + addSomething;
            }

            return randomParam;
        }

        private static int WoundsNumbers(Unit unit, Unit enemy)
        {
            if (unit.AutoDeath)
            {
                Test.Data.Console(Test.Data.text, " <-- lose all wounds");
                return enemy.Wounds;
            }

            if (String.IsNullOrEmpty(unit.MultiWounds) || enemy.NoMultiWounds)
                return 1;

            int multiwounds = RandomParamParse(unit.MultiWounds);

            Test.Data.Console(Test.Data.text, " <-- {0} multiple wounds", multiwounds);
                
            return multiwounds;
        }

        private static bool PoisonedAttack(Unit unit, bool impactHit = false)
        {
            if (!impactHit && unit.PoisonAttack && (Dice.lastDice == 6))
            {
                attackIsPoisoned = true;
                Test.Data.Console(Test.Data.text, "(poison)");
                return true;
            }
            else
                return false;
        }

        private static bool KillingAttack(Unit unit, Unit enemy)
        {
            bool killingBlow = (unit.HeroicKillingBlow || (unit.KillingBlow && !enemy.LargeBase)) && (Dice.lastDice == 6);

            bool extendedKillingBlow = (unit.ExtendedKillingBlow > 0) && !enemy.LargeBase && (Dice.lastDice >= unit.ExtendedKillingBlow);

            if ((killingBlow || extendedKillingBlow) && !enemy.NoKillingBlow && !attackIsPoisoned)
            {
                attackWithKillingBlow = true;
                Test.Data.Console(Test.Data.text, " --> {0}killing blow", (unit.HeroicKillingBlow ? "heroic " : String.Empty));
                return true;
            }

            if ((enemy.Armour != null) && !unit.NoArmour)
                Test.Data.Console(Test.Data.text, " --> AS ");

            return false;
        }

        private static bool Hit(Unit unit, Unit enemy, int round, out int dice)
        {
            int chance = 4;
            dice = 0;

            if (unit.AutoHit || enemy.SteamTank)
            {
                Test.Data.Console(Test.Data.text, "(autohit)");
                return true;
            }
            else if (unit.HitOn > 0)
                chance = unit.HitOn;
            else if (unit.WeaponSkill > enemy.WeaponSkill)
                chance = 3;
            else if ((unit.WeaponSkill * 2) < enemy.WeaponSkill)
                chance = 5;

            return Dice.Roll(unit, Dice.Types.WS, enemy, chance, dice: out dice, round: round);
        }

        private static bool Wound(Unit unit, Unit enemy, int round)
        {
            int chance = 4;
            int strength = unit.Strength;

            if ((unit.Lance || unit.Flail || unit.Resolute) && (round == 1))
            {
                strength += (unit.Resolute ? 1 : 2);
                strength = Unit.ParamNormalization(strength);
            }

            if (unit.AutoWound)
            {
                Test.Data.Console(Test.Data.text, "(autowound)");
                return true;
            }
            else if (unit.WoundOn > 0)
                chance = unit.WoundOn;
            else if (strength == (enemy.Toughness + 1))
                chance = 3;
            else if (strength > (enemy.Toughness + 1))
                chance = 2;
            else if ((strength + 1) == enemy.Toughness)
                chance = 5;
            else if ((strength + 2) == enemy.Toughness)
                chance = 6;
            else if ((strength + 2) < enemy.Toughness)
            {
                Test.Data.Console(Test.Data.supplText, "(impossible)");
                return false;
            }

            return Dice.Roll(unit, Dice.Types.S, enemy, chance);
        }

        private static bool NotAS(ref Unit unit, Unit enemy)
        {
            if ((enemy.Armour == null) || unit.NoArmour)
                return true;

            int chance = Unit.ParamNormalization((unit.Strength + unit.ArmourPiercing) - 3, onlyZeroCheck: true);

            chance += enemy.Armour ?? 0;

            bool armourFail = Dice.Roll(unit, Dice.Types.AS, enemy, chance);

            if (!armourFail)
                Param.Tests(ref unit, enemy, context: Param.ContextType.ArmourSave);

            return armourFail;
        }

        private static bool NotDiscountWound(ref Unit enemy)
        {
            if (!enemy.FirstWoundDiscount)
                return true;
            else
            {
                Test.Data.Console(Test.Data.goodText, " --> first wound discount");

                enemy.FirstWoundDiscount = false;

                return false;
            }
        }

        private static bool NotWard(ref Unit unit, Unit enemy)
        {
            int ward = enemy.Ward ?? 0;

            if ((enemy.WardForFirstWound > 0) && (enemy.Wounds == enemy.OriginalWounds))
            {
                ward = enemy.WardForFirstWound;
                enemy.WardForFirstWound = 0;
            }

            if ((enemy.WardForLastWound > 0) && (enemy.Wounds == 1))
                ward = enemy.WardForLastWound;

            if ((ward <= 0) || unit.NoWard)
                return true;

            Test.Data.Console(Test.Data.text, " --> ward ");

            bool wardFail = Dice.Roll(unit, Dice.Types.WARD, enemy, ward);

            if (!wardFail)
                Param.Tests(ref unit, enemy, context: Param.ContextType.WardSave);

            return wardFail;
        }

        private static Unit UnitFromParticipants(List<Unit> participants, Unit unit)
        {
            foreach (Unit u in participants)
                if (u.ID == unit.ID)
                    return u;

            return null;
        }

        private static void UnitRoundShow(Unit unit, bool firstLine)
        {
            string uLine = (unit.Wounds > 0 ? String.Format("{0}: {1}W", unit.Name, unit.Wounds) : String.Empty);
            bool monstrousMount = (unit.Mount != null) && (unit.Mount.Wounds > 0) && unit.Mount.IsNotSimpleMount();
            string uMount = (monstrousMount ? String.Format("{0}: {1}W", unit.Mount.Name, unit.Mount.Wounds) : String.Empty);
            string bothLine = (!String.IsNullOrEmpty(uLine) && !String.IsNullOrEmpty(uMount) ? " + " : String.Empty);
            Test.Data.Console(Test.Data.supplText, "{0}{1}{2}{3}", uLine, bothLine, uMount, (firstLine ? ", " : String.Empty));
        }
    }
}
