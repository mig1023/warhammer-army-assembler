using System;
using System.Collections.Generic;

namespace WarhammerArmyAssembler.Test
{
    class SpecialAttacks
    {
        public static void GiantAttacks(ref Unit unit, List<Unit> participants,
            ref Dictionary<int, int> roundWounds, int round)
        {
            Unit opponent = Fight.SelectOpponent(participants, unit);
            bool opponentIsMonster = opponent.LargeBase;

            int attackType = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);

            Data.Console(Data.Text, $"\n\n{unit.Name} chose special attack ");

            if (attackType == 1)
            {
                Data.Console(Data.Text, "--> Yell and Bawl (pass round with opponent)");

                opponent.PassThisRound = true;
                roundWounds[opponent.ID] = 2;
            }
            else if (opponentIsMonster && (attackType >= 2) && (attackType <= 4))
            {
                Data.Console(Data.Text, $"--> Thump with Club --> " +
                    $"{opponent.Name} must pass Initiative test");

                int initiative = opponent.Initiative.Value;

                if (Dice.Roll(unit, Dice.Types.I, opponent, initiative, 1, paramTest: true, hiddenDice: true))
                {
                    Data.Console(Data.GoodText, " --> passed");
                }
                else
                {
                    Data.Console(Data.BadText, " --> FAIL");

                    int firstWoundDice = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);
                    int secondWoundDice = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);

                    if (firstWoundDice == secondWoundDice)
                    {
                        Data.Console(Data.Text, " --> Giant's club embeds itself in the ground");
                        unit.PassThisRound = true;
                    }
                    else
                    {
                        int wounds = firstWoundDice + secondWoundDice;
                        Data.Console(Data.BadText, $" --> Giant's inflict {wounds} wounds");

                        if (wounds > opponent.Wounds.Original)
                        {
                            wounds = opponent.Wounds.Original;
                            Data.Console(Data.SupplText, $", only {wounds} can be inflicted");
                        }

                        roundWounds[opponent.ID] += wounds;
                        opponent.Wounds.Value -= wounds;
                    }
                }
            }
            else if (opponentIsMonster && (attackType >= 5))
            {
                Data.Console(Data.Text, "--> 'Eadbutt");
                Data.Console(Data.BadText, $" --> {opponent.Name} WOUND");

                roundWounds[opponent.ID] += 1;
                opponent.Wounds.Value -= 1;
                opponent.PassThisRound = true;
            }
            else if (!opponentIsMonster && (attackType == 2))
            {
                Data.Console(Data.Text, "--> Jump Up and Down");

                if (Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true) == 1)
                {
                    Data.Console(Data.Text, " --> fall");
                    Data.Console(Data.BadText, $" --> {unit.Name} WOUND");

                    unit.PassThisRound = true;
                    roundWounds[unit.ID] += 1;
                    unit.Wounds.Value -= 1;
                }
                else
                {
                    int attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 2, hiddenDice: true);
                    roundWounds[opponent.ID] += Fight.Round(ref unit, ref opponent, attacks, round);
                }
            }
            else if (!opponentIsMonster && (attackType == 3))
            {
                Data.Console(Data.Text, "--> Pick Up and... ");

                Dictionary<int, string> pickUpType = new Dictionary<int, string>
                {
                    [1] = "Stuff into Bag",
                    [2] = "Throw back into Combat",
                    [3] = "Hurl",
                    [4] = "Squash",
                    [5] = "Eat",
                    [6] = "Pick Another",
                };

                int pickType = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);
                Data.Console(Data.Text, pickUpType[pickType]);
                Data.Console(Data.BadText, $" --> {opponent.Name} SLAIN");

                roundWounds[opponent.ID] += opponent.Wounds.Original;
                opponent.Wounds.Value -= opponent.Wounds.Original;
            }
            else
            {
                Data.Console(Data.Text, $"--> {unit.Name} Swing with Club");

                int attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);
                roundWounds[opponent.ID] += Fight.Round(ref unit, ref opponent, attacks, round);
            }
        }

        public static void HellPitAbomination(ref Unit unit, List<Unit> participants,
            ref Dictionary<int, int> roundWounds, int round)
        {
            Unit hellOpponent = Fight.SelectOpponent(participants, unit);

            int attackType = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);

            int attacks = 0;
            unit.MultiWounds = String.Empty;
            unit.AutoHit = false;

            if (attackType < 3)
            {
                attacks = 1;
                unit.MultiWounds = "D3";
                Data.Console(Data.SupplText, $"\n\n{unit.Name} feed: 1 attack with D3 multiwound");
            }
            else if ((attackType > 2) && (attackType < 5))
            {
                attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 3, hiddenDice: true);
                Data.Console(Data.SupplText, $"\n\n{unit.Name} flailing fists: 3D6 attacks");
            }
            else
            {
                attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 2, hiddenDice: true);
                unit.AutoHit = true;
                Data.Console(Data.SupplText, $"\n\n{unit.Name} avalanche of flesh: 2D6 attack with autohit");
            }

            roundWounds[hellOpponent.ID] += Fight.Round(ref unit, ref hellOpponent, attacks, round);
        }

        public static void ImpactHit(Unit unit, List<Unit> participants,
            ref Dictionary<int, int> roundWounds, int round)
        {
            bool impactHit = !String.IsNullOrEmpty(unit.ImpactHit) ||
                (unit.ImpactHitByFront > 0);

            bool impactHitByMount = (unit.Mount != null) &&
                !String.IsNullOrEmpty(unit.Mount.ImpactHit);

            if ((round == 1) && (impactHit || impactHitByMount || unit.SteamTank))
            {
                Unit impactUnit = (unit.Mount != null &&
                    !String.IsNullOrEmpty(unit.Mount.ImpactHit) ? unit.Mount : unit);

                Unit impactOpponent = Fight.SelectOpponent(participants, impactUnit);

                int attacks = ImpactHitNumber(unit, unit.Mount,
                    out string impactOutLine, out bool steamFail);

                if (steamFail)
                    unit.Wounds.Value -= 1;

                roundWounds[impactOpponent.ID] += Fight.Round(ref impactUnit,
                    ref impactOpponent, attacks, round, impactHit: true,
                    impactLine: impactOutLine);
            }
        }

        private static int ImpactHitNumber(Unit unit, Unit unitMount, out string impactOutLine, out bool steamFail)
        {
            steamFail = false;

            if (unit.ImpactHitByFront > 0)
            {
                impactOutLine = unit.GetFront().ToString();
                return unit.GetFront();
            }

            string impactHit = String.Empty;

            if (unit.SteamTank)
            {
                int steamPoint = Data.Rand.Next(unit.Wounds.Value / 2) + (Data.Rand.Next(6) + 1);

                Data.Console(Data.Text, $"\n\n{unit.Name} generate {steamPoint} steam point ");

                if (steamPoint > unit.Wounds.Value)
                {
                    Data.Console(Data.BadText, $"--> boiler fail, {unit.Name} WOUND");

                    impactOutLine = String.Empty;
                    steamFail = true;

                    return 0;
                }

                int steamImpactHit = steamPoint * (Data.Rand.Next(3) + 1);

                Data.Console(Data.SupplText, $"--> {steamImpactHit} impact hits");
                impactOutLine = steamImpactHit.ToString();

                return steamImpactHit;
            }
            else if ((unitMount == null) || String.IsNullOrEmpty(unitMount.ImpactHit))
            {
                impactHit = unit.ImpactHit;
            }
            else if (String.IsNullOrEmpty(unit.ImpactHit))
            {
                impactHit = unitMount.ImpactHit;
            }
            else
            {
                int currentImpact = 0, currentAdd = 0;

                foreach (Unit u in new List<Unit> { unit, unitMount })
                {
                    Fight.RandomParamValues(u.ImpactHit,
                        out int diceNumber, out int diceSize, out int addSomething);

                    int diceMax = diceNumber * diceSize;
                    bool diceMaxExactly = (diceMax == currentImpact) && (addSomething > currentAdd);

                    if ((diceMax > currentImpact) || diceMaxExactly)
                    {
                        currentImpact = diceMax;
                        currentAdd = addSomething;
                        impactHit = u.ImpactHit;
                    }
                }
            }

            impactOutLine = impactHit;
            return Fight.RandomParamParse(impactHit);
        }
    }
}
