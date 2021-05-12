using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Test
{
    class SpecialAttacks
    {
        public static void GiantAttacks(ref Unit unit, List<Unit> participants,
            ref Dictionary<int, int> roundWounds, int round)
        {
            Unit giantOpponent = Fight.SelectOpponent(participants, unit);
            bool opponentIsMonster = giantOpponent.LargeBase;

            int attackType = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);

            Test.Data.Console(Test.Data.text, "\n\n{0} chose special attack ", unit.Name);

            if (attackType == 1)
            {
                Test.Data.Console(Test.Data.text, "--> Yell and Bawl (pass round with opponent)");

                giantOpponent.PassThisRound = true;
                roundWounds[giantOpponent.ID] = 2;
            }
            else if (opponentIsMonster && (attackType >= 2) && (attackType <= 4))
            {
                Test.Data.Console(Test.Data.text, "--> Thump with Club --> {0} must pass Initiative test", giantOpponent.Name);

                if (Test.Dice.Roll(unit, Test.Dice.Types.I, giantOpponent, giantOpponent.Initiative, 1, paramTest: true, hiddenDice: true))
                    Test.Data.Console(Test.Data.goodText, " --> passed");
                else
                {
                    Test.Data.Console(Test.Data.badText, " --> FAIL");

                    int firstWoundDice = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);
                    int secondWoundDice = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);

                    if (firstWoundDice == secondWoundDice)
                    {
                        Test.Data.Console(Test.Data.text, " --> Giant's club embeds itself in the ground");
                        unit.PassThisRound = true;
                    }
                    else
                    {
                        int wounds = firstWoundDice + secondWoundDice;
                        Test.Data.Console(Test.Data.badText, " --> Giant's inflict {0} wounds", wounds);

                        if (wounds > giantOpponent.OriginalWounds)
                        {
                            wounds = giantOpponent.OriginalWounds;
                            Test.Data.Console(Test.Data.supplText, ", only {0} can be inflicted", wounds);
                        }

                        roundWounds[giantOpponent.ID] += wounds;
                        giantOpponent.Wounds -= wounds;
                    }
                }
            }
            else if (opponentIsMonster && (attackType >= 5))
            {
                Test.Data.Console(Test.Data.text, "--> 'Eadbutt");
                Test.Data.Console(Test.Data.badText, " --> {0} WOUND", giantOpponent.Name);

                roundWounds[giantOpponent.ID] += 1;
                giantOpponent.Wounds -= 1;
                giantOpponent.PassThisRound = true;
            }
            else if (!opponentIsMonster && (attackType == 2))
            {
                Test.Data.Console(Test.Data.text, "--> Jump Up and Down", unit.Name);

                if (Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true) == 1)
                {
                    Test.Data.Console(Test.Data.text, " --> fall");
                    Test.Data.Console(Test.Data.badText, " --> {0} WOUND", unit.Name);

                    unit.PassThisRound = true;
                    roundWounds[unit.ID] += 1;
                    unit.Wounds -= 1;
                }
                else
                {
                    int attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 2, hiddenDice: true);
                    roundWounds[giantOpponent.ID] += Fight.Round(ref unit, ref giantOpponent, attacks, round);
                }
            }
            else if (!opponentIsMonster && (attackType == 3))
            {
                Test.Data.Console(Test.Data.text, "--> Pick Up and... ");

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
                Test.Data.Console(Test.Data.text, pickUpType[pickType]);
                Test.Data.Console(Test.Data.badText, " --> {0} SLAIN", giantOpponent.Name);

                roundWounds[giantOpponent.ID] += giantOpponent.OriginalWounds;
                giantOpponent.Wounds -= giantOpponent.OriginalWounds;
            }
            else
            {
                Test.Data.Console(Test.Data.text, "--> {0} Swing with Club", unit.Name);

                int attacks = Dice.RollAll(Dice.Types.OTHER, unit, diceNum: 1, hiddenDice: true);
                roundWounds[giantOpponent.ID] += Fight.Round(ref unit, ref giantOpponent, attacks, round);
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

            roundWounds[hellOpponent.ID] += Fight.Round(ref unit, ref hellOpponent, attacks, round);
        }

        public static void ImpactHit(Unit unit, List<Unit> participants, ref Dictionary<int, int> roundWounds, int round)
        {
            bool impactHit = (round == 1) &&
                (!String.IsNullOrEmpty(unit.ImpactHit) || (unit.Mount != null && !String.IsNullOrEmpty(unit.Mount.ImpactHit)));

            if (impactHit || unit.SteamTank)
            {
                Unit impactUnit = (unit.Mount != null && !String.IsNullOrEmpty(unit.Mount.ImpactHit) ? unit.Mount : unit);
                Unit impactOpponent = Fight.SelectOpponent(participants, impactUnit);

                string impactOutLine = String.Empty;

                int attacks = ImpactHitNumer(unit, unit.Mount, out impactOutLine, out bool steamFail);

                if (steamFail)
                    unit.Wounds -= 1;

                roundWounds[impactOpponent.ID] += Fight.Round(
                    ref impactUnit, ref impactOpponent, attacks, round, impactHit: true, impactLine: impactOutLine
                );
            }
        }

        private static int ImpactHitNumer(Unit unit, Unit unitMount, out string impactOutLine, out bool steamFail)
        {
            string impactHit = String.Empty;
            steamFail = false;

            if (unit.SteamTank)
            {
                int steamPoint = Test.Data.rand.Next(unit.Wounds / 2) + (Test.Data.rand.Next(6) + 1);

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

                foreach (Unit u in new List<Unit> { unit, unitMount })
                {
                    Fight.RandomParamValues(u.ImpactHit, out int diceNumber, out int diceSize, out int addSomething);

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
            return Fight.RandomParamParse(impactHit);
        }
    }
}
