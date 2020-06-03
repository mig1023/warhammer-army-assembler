﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Test
{
    class Fight
    {
        private enum ParamTestType { Pass, Wound, Death };
        
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
                    ImpactHit(unit, participants, ref roundWounds);

                foreach (Unit u in participants)
                    if (BothOpponentsAreAlive(participants))
                    {
                        Unit actor = UnitFromParticipants(participants, u);
                        Unit opponent = SelectOpponent(participants, u);

                        if ((participants.Count > 2) && (actor.Wounds > 0))
                            Test.Data.Console(Test.Data.supplText, "\n\n{0} chose {1} as his opponent", actor.Name, opponent.Name);

                        int woundsAtStartOfRound = opponent.Wounds;

                        TestsAtTheStartOfRound(ref actor, opponent, round);

                        if (actor.SteamTank)
                            ImpactHit(actor, participants, ref roundWounds);
                        else if (u.HellPitAbomination)
                            HellPitAbomination(actor, participants, ref roundWounds);

                        if (actor.Attacks <= 0)
                            continue;

                        if (actor.PassThisRound)
                        {
                            actor.PassThisRound = false;
                            continue;
                        }

                        roundWounds[opponent.ID] += Round(actor, ref opponent, attacksRound[actor.ID], round);

                        if (opponent.Regeneration && (woundsAtStartOfRound > opponent.Wounds) && !opponent.WoundedWithKillingBlow)
                            Regeneration(opponent, (woundsAtStartOfRound - opponent.Wounds));

                        if (opponent.Wounds <= 0)
                            Test.Data.Console(Test.Data.badText, "\n\n{0} SLAIN", opponent.Name);
                    }

                Test.Data.Console(Test.Data.text, "\n");

                if (BothOpponentsAreAlive(participants))
                {
                    bool draw = true;

                    foreach (KeyValuePair<Unit, List<Unit>> u in BreakTestOrder)
                    {
                        if (u.Key.Wounds <= 0)
                            continue;

                        if (((u.Key == unit.Mount) && (unit.Wounds > 0)) || ((u.Key == enemy.Mount) && (enemy.Wounds > 0)))
                            continue;

                        roundWounds[u.Key.ID] += RoundBonus(u.Value);

                        if (RoundLostBy(u.Value, roundWounds))
                        {
                            if (BreakTestFail(u.Value, ref roundWounds))
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

        private static void HellPitAbomination(Unit unit, List<Unit> participants,
            ref Dictionary<int, int> roundWounds)
        {
            Unit impactOpponent = SelectOpponent(participants, unit);

            int attackType = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);
            int attacks = 0;

            if (attackType < 3)
            {
                attacks = 1;
                unit.MultiWounds = "D3";
                Test.Data.Console(Test.Data.supplText, "\n\n{0} feed: 1 attack with D3 multiwound", unit.Name);
            }
            else if ((attackType > 2) && (attackType < 5))
            {
                attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 3, hiddenDice: true);
                Test.Data.Console(Test.Data.supplText, "\n\n{0} flailing fists: 3D6 attacks", unit.Name);
            }
            else
            {
                attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 2, hiddenDice: true);
                unit.AutoHit = true;
                Test.Data.Console(Test.Data.supplText, "\n\n{0} avalanche of flesh: 2D6 attack with autohit", unit.Name);
            }

            roundWounds[impactOpponent.ID] += Round(unit, ref impactOpponent, attacks, round);
        }

        private static void ImpactHit(Unit unit, List<Unit> participants, ref Dictionary<int, int> roundWounds)
        {
            bool impactHit = (round == 1) &&
                (!String.IsNullOrEmpty(unit.ImpactHit) || (unit.Mount != null && !String.IsNullOrEmpty(unit.Mount.ImpactHit)));

            if (impactHit || unit.SteamTank)
            {
                Unit impactUnit = (unit.Mount != null && !String.IsNullOrEmpty(unit.Mount.ImpactHit) ? unit.Mount : unit);
                Unit impactOpponent = SelectOpponent(participants, impactUnit);

                string impactOutLine = String.Empty;

                int attacks = ImpactHitNumer(unit, unit.Mount, out impactOutLine, out bool steamFail);

                if (steamFail)
                    unit.Wounds -= 1;

                roundWounds[impactOpponent.ID] += Round(
                    impactUnit, ref impactOpponent, attacks, round, impactHit: true, impactLine: impactOutLine
                );
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

        private static int RoundBonus(List<Unit> units)
        {
            Unit unit = units[2];
            Unit unitMount = units[3];

            Unit enemy = units[0];
            Unit enemyMount = units[1];

            int unitFullSize = (unit.Size * unit.OriginalWounds) + (unitMount != null ? unitMount.Size * unitMount.OriginalWounds : 0);
            int enemyFullSize = (enemy.Size * enemy.OriginalWounds) + (enemyMount != null ? enemyMount.Size * enemyMount.OriginalWounds : 0);

            string unitSide = (((unitMount != null) && (unit.Wounds <= 0)) ? unitMount.Name : unit.Name);

            if (unitFullSize > enemyFullSize)
            {
                Test.Data.Console(Test.Data.supplText, "\n{0} have +1 battle result bonus by outnumber", unitSide);
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
                Test.Data.Console(Test.Data.supplText, "\n{0} --> is frenzy", unit.Name);

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
            if (unit.Frenzy && (unit.Wounds > 0))
            {
                unit.Frenzy = false;
                unit.Attacks -= 1;
                Test.Data.Console(Test.Data.supplText, "\n{0} lost his frenzy", unit.Name);
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

            Test.Data.Console(Test.Data.text, "\n{0} try to resist of terror by {1} ", unit.Name, terrorSource);

            if (unit.Unbreakable)
                Test.Data.Console(Test.Data.goodText, " --> autopassed (unbreakable)");
            else if (unit.ImmuneToPsychology || unit.Undead)
                Test.Data.Console(Test.Data.goodText, " --> autopassed (imunne to psychology)");
            else if (unit.Frenzy)
                Test.Data.Console(Test.Data.goodText, " --> autopassed (frenzy)");
            else if (Dice.Roll(unit, Dice.Types.LD, enemy, unit.Leadership, 2))
                Test.Data.Console(Test.Data.goodText, " --> passed");
            else
            {
                unit.Wounds = 0;
                Test.Data.Console(Test.Data.badText, " --> fail");
            }

            return unit;
        }

        private static int Round(Unit unit, ref Unit enemy, int attackNumber, int round,
            bool impactHit = false, string impactLine = "", bool afterSteamTankAttack = false)
        {
            int roundWounds = 0;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0) && !(impactHit && unit.SteamTank) && !afterSteamTankAttack) 
                Test.Data.Console(Test.Data.text, "\n");

            for (int i = 0; i < attackNumber; i++)
            {
                int wounded = Attack(unit, ref enemy, round, impactHit, impactLine);
                roundWounds += wounded;
                enemy.Wounds -= wounded;
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
            {
                if (Dice.Roll(unit, Dice.Types.I, enemy, 4, hiddenDice: true))
                    return true;
                else
                    return false;
            }
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
                (unit.UnitStrength * unit.Size) + (unitFriend != null ? (unitFriend.UnitStrength * unitFriend.Size) : 0) <
                (enemy.UnitStrength * enemy.Size) + (enemyFriend != null ? (enemyFriend.UnitStrength * enemyFriend.Size) : 0)
            );

            if (unit.Unbreakable)
                Test.Data.Console(Test.Data.text, "unbreakable");
            else if (
                thereAreMoreOfThem
                &&
                (enemyFearOrTerror || enemyMountFearOrTerror)
                &&
                !(unit.ImmuneToPsychology || unit.Undead || unitFearOrTerror || unitMountFearOrTerror))
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

        private static int Attack(Unit unit, ref Unit enemy, int round, bool impactHit = false, string impactLine = "")
        {
            attackIsPoisoned = false;
            attackWithKillingBlow = false;

            int woundsAtStart = enemy.Wounds;

            if ((unit.Wounds > 0) && (enemy.Wounds > 0))
            {
                if (!impactHit)
                    Test.Data.Console(Test.Data.text, "\n{0} --> hit ", unit.Name);
                else
                {
                    Test.Data.Console(Test.Data.text, "\n{0} --> hit ( ", unit.Name);
                    Test.Data.Console(Test.Data.supplText, "{0} impact hit", impactLine);
                    Test.Data.Console(Test.Data.text, " )");
                }

                if (impactHit || Hit(unit, enemy, round))
                {
                    TestsInRound(ref enemy, unit);

                    if (enemy.Wounds <= 0)
                        return woundsAtStart;

                    Test.Data.Console(Test.Data.text, " --> wound ");

                    if (
                        (PoisonedAttack(unit, impactHit) || Wound(unit, enemy, round))
                        &&
                        (KillingAttack(unit, enemy) || NotAS(unit, enemy))
                        &&
                        (NotWard(unit, enemy))
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
                            return WoundsNumbers(unit, enemy);
                        }
                    }
                }
                Test.Data.Console(Test.Data.goodText, " --> fail");
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
                    randomParam += Test.Data.rand.Next(diceSize) + 1 + addSomething;
            }

            return randomParam;
        }

        private static int ImpactHitNumer(Unit unit, Unit unitMount, out string impactOutLine, out bool steamFail)
        {
            string impactHit = String.Empty;
            steamFail = false;

            if (unit.SteamTank)
            {
                int steamPoint = Test.Data.rand.Next(unit.Wounds/2) + (Test.Data.rand.Next(6) + 1);

                Test.Data.Console(Test.Data.text, "\n\n{0} generate {1} steam point ", unit.Name, steamPoint);

                if (steamPoint > unit.Wounds)
                {
                    Test.Data.Console(Test.Data.badText, "--> boiler fail, {0} WOUND", unit.Name);

                    impactOutLine = String.Empty;
                    steamFail = true;

                    return 0;
                }

                int steamImpactHit = steamPoint * (Test.Data.rand.Next(3) + 1);

                Test.Data.Console(Test.Data.supplText, "--> {2} impact hits", unit.Name, steamPoint, steamImpactHit);
                impactOutLine = steamImpactHit.ToString();

                return steamImpactHit;
            }
            else if ((unitMount == null) || String.IsNullOrEmpty(unitMount.ImpactHit))
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

            Test.Data.Console(Test.Data.text, " <-- {0} multiple wounds", multiwounds);

            if (enemy.Wounds < multiwounds)
            {
                multiwounds = enemy.Wounds;
                Test.Data.Console(Test.Data.supplText, ", only {0} can be inflicted", multiwounds);
            }
                

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
            bool killingBlow = unit.HeroicKillingBlow || (unit.KillingBlow && (enemy.UnitStrength <= 1));

            if (killingBlow && !attackIsPoisoned && (Dice.lastDice == 6))
            {
                attackWithKillingBlow = true;
                Test.Data.Console(Test.Data.text, " --> {0}killing blow", (unit.HeroicKillingBlow ? "heroic " : String.Empty));
                return true;
            }

            if ((enemy.Armour != null) && !unit.NoArmour)
                Test.Data.Console(Test.Data.text, " --> AS ");

            return false;
        }

        private static bool Hit(Unit unit, Unit enemy, int round)
        {
            int chance = 4;

            if (unit.AutoHit || enemy.SteamTank)
            {
                Test.Data.Console(Test.Data.text, "(autohit)");
                return true;
            }
            else if (unit.WeaponSkill > enemy.WeaponSkill)
                chance = 3;
            else if ((unit.WeaponSkill * 2) < enemy.WeaponSkill)
                chance = 5;

            return Dice.Roll (unit, Dice.Types.WS, enemy, chance, round: round);
        }

        private static bool Wound(Unit unit, Unit enemy, int round)
        {
            int chance = 4;
            int strength = unit.Strength;

            if ((unit.Lance || unit.Flail) && (round == 1))
            {
                strength += 2;
                strength = Unit.ParamNormalization(strength);
            }

            if (unit.AutoWound)
            {
                Test.Data.Console(Test.Data.text, "(autowound)");
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
                Test.Data.Console(Test.Data.text, "(impossible)");
                return false;
            }

            return Dice.Roll(unit, Dice.Types.S, enemy, chance);
        }

        private static bool NotAS(Unit unit, Unit enemy)
        {
            if ((enemy.Armour == null) || unit.NoArmour)
                return true;

            int chance = Unit.ParamNormalization((unit.Strength + unit.ArmourPiercing) - 3, onlyZeroCheck: true);

            chance += enemy.Armour ?? 0;

            return Dice.Roll(unit, Dice.Types.AS, enemy, chance);
        }

        private static bool NotWard(Unit unit, Unit enemy)
        {
            if (enemy.Ward == null)
                return true;

            Test.Data.Console(Test.Data.text, " --> ward ");

            return Dice.Roll(unit, Dice.Types.WARD, enemy, enemy.Ward);
        }

        private static Unit UnitFromParticipants(List<Unit> participants, Unit unit)
        {
            foreach (Unit u in participants)
                if (u.ID == unit.ID)
                    return u;

            return null;
        }

        private static void TestsInRound(ref Unit unit, Unit opponent)
        {
            if (!String.IsNullOrEmpty(opponent.DeathByTestAfterHit))
                ParamTest(ref unit, opponent.DeathByTestAfterHit, opponent, ParamTestType.Death, inRound: true);
        }

        private static void TestsAtTheStartOfRound(ref Unit unit, Unit opponent, int round)
        {
            if (!String.IsNullOrEmpty(opponent.PassRoundByTest))
                ParamTest(ref unit, opponent.PassRoundByTest, opponent, ParamTestType.Pass);
            else if (!String.IsNullOrEmpty(opponent.PassRoundByTestOnce) && (round == 1))
                ParamTest(ref unit, opponent.PassRoundByTestOnce, opponent, ParamTestType.Pass);
            
            if (!String.IsNullOrEmpty(opponent.WoundByTest))
                ParamTest(ref unit, opponent.WoundByTest, opponent, ParamTestType.Wound);
            else if (!String.IsNullOrEmpty(opponent.WoundByTestOnce) && (round == 1))
                ParamTest(ref unit, opponent.WoundByTestOnce, opponent, ParamTestType.Wound);
            
            if (!String.IsNullOrEmpty(opponent.DeathByTest))
                ParamTest(ref unit, opponent.DeathByTest, opponent, ParamTestType.Death);
            else if (!String.IsNullOrEmpty(opponent.DeathByTestOnce) && (round == 1))
                ParamTest(ref unit, opponent.DeathByTestOnce, opponent, ParamTestType.Death);
        }

        private static void ParamTest(ref Unit unit, string param, Unit opponent, ParamTestType test, bool inRound = false)
        {
            Test.Data.Console(Test.Data.text, (inRound ? " --> " : "\n\n") + "{0} must pass {1} test ", unit.Name, param);

            int paramValue = (int)typeof(Unit).GetProperty(param).GetValue(unit);
            int diceNum = ((param == "Leadership") ? 2 : 1);

            if (Test.Dice.Roll(unit, param, opponent, paramValue, diceNum, paramTest: true))
                Test.Data.Console(Test.Data.goodText, " --> passed");
            else
                switch(test)
                {
                    case ParamTestType.Pass:
                        Test.Data.Console(Test.Data.badText, " --> pass this round");
                        unit.PassThisRound = true;
                        break;
                    case ParamTestType.Wound:
                        Test.Data.Console(Test.Data.badText, " --> WOUND");
                        unit.Wounds -= 1;
                        break;
                    case ParamTestType.Death:
                        Test.Data.Console(Test.Data.badText, " --> SLAIN");
                        unit.Wounds = 0;
                        break;
                }
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
