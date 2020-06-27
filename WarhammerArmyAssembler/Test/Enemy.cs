using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Enemy : Unit
    {
        public static int MaxIDindex = -10;

        private static Dictionary<string, List<Enemy>> GetEnemiesDictionary()
        {
            return  new Dictionary<string, List<Enemy>>
            {
                ["Lords"] = new List<Enemy>(EnemiesLords),
                ["Heroes"] = new List<Enemy>(EnemiesHeroes),
                ["Core Units"] = new List<Enemy>(EnemiesCoreUnits),
                ["Special Units"] = new List<Enemy>(EnemiesSpecialUnits),
                ["Rare Units"] = new List<Enemy>(EnemiesRareUnits),
                ["Monsters"] = new List<Enemy>(EnemiesMonsters),
            };
        }

        public static Enemy GetByName(string enemyName)
        {
            foreach (List<Enemy> enemyList in new List<List<Enemy>>
            {
                EnemiesMonsters, EnemiesHeroes, EnemiesLords,
                EnemiesCoreUnits, EnemiesSpecialUnits, EnemiesRareUnits,
            })
                foreach (Enemy enemy in enemyList)
                    if (enemy.TestListName == enemyName)
                        return enemy.SetID();

            return null;
        }

        private Enemy SetID()
        {
            MaxIDindex -= 1;

            this.ID = MaxIDindex;

            return this;
        }

        public static List<string> GetEnemiesGroups()
        {
            return GetEnemiesDictionary().Keys.ToList<string>();
        }

        public static List<Enemy> GetEnemiesByGroup(string groupName)
        {
            return GetEnemiesDictionary()[groupName];
        }

        private static Enemy NewEnemy(string Name, UnitType Type, int Size,
            int Movement, int WeaponSkill, int BallisticSkill, int Strength, int Toughness,
            int Wounds, int Initiative, int Attacks, int Leadership, int Armour = 0, int Ward = 0, bool Fear = false,
            bool Regeneration = false, bool Stupidity = false, bool LargeBase = false, bool Terror = false, string TestListName = "",
            string Reroll = "", bool ColdBlooded = false, bool Undead = false, bool KillingBlow = false,
            bool PoisonAttack = false, bool ImmuneToPsychology = false, bool Hate = false,
            bool NoKollingBlow = false, bool HeroicKillingBlow = false, bool Stubborn = false,
            bool HellPitAbomination = false, bool Unbreakable = false, bool HitFirst = false, bool HitLast = false, bool Frenzy = false,
            bool Flail = false, string MultiWounds = "", Enemy Mount = null,
            bool Lance = false, int ArmourPiercing = 0, bool AutoWound = false, bool StrengthInNumbers = false,
            bool SteamTank = false, bool NoArmour = false
            )
        {
            Enemy enemy = new Enemy();

            enemy.Name = Name;
            enemy.TestListName = TestListName;
            enemy.Type = Type;
            enemy.Size = Size;
            enemy.Movement = new MainParam(Movement, enemy);
            enemy.WeaponSkill = new MainParam(WeaponSkill, enemy);
            enemy.BallisticSkill = new MainParam(BallisticSkill, enemy);
            enemy.Strength = new MainParam(Strength, enemy);
            enemy.Toughness = new MainParam(Toughness, enemy);
            enemy.Wounds = new MainParam(Wounds, enemy);
            enemy.Initiative = new MainParam(Initiative, enemy);
            enemy.Attacks = new MainParam(Attacks, enemy);
            enemy.Leadership = new MainParam(Leadership, enemy);
            enemy.Armour = new MainParam(Armour, enemy);
            enemy.Ward = new MainParam(Ward, enemy);
            enemy.Fear = Fear;
            enemy.Terror = Terror;
            enemy.Regeneration = Regeneration;
            enemy.Stupidity = Stupidity;
            enemy.LargeBase = LargeBase;
            enemy.Reroll = Reroll;
            enemy.Undead = Undead;
            enemy.KillingBlow = KillingBlow;
            enemy.PoisonAttack = PoisonAttack;
            enemy.ImmuneToPsychology = ImmuneToPsychology;
            enemy.Hate = Hate;
            enemy.NoKollingBlow = NoKollingBlow;
            enemy.HeroicKillingBlow = HeroicKillingBlow;
            enemy.Stubborn = Stubborn;
            enemy.HellPitAbomination = HellPitAbomination;
            enemy.Unbreakable = Unbreakable;
            enemy.HitFirst = HitFirst;
            enemy.HitLast = HitLast;
            enemy.Frenzy = Frenzy;
            enemy.Flail = Flail;
            enemy.MultiWounds = MultiWounds;
            enemy.Mount = Mount;
            enemy.Lance = Lance;
            enemy.AutoWound = AutoWound;
            enemy.ArmourPiercing = ArmourPiercing;
            enemy.StrengthInNumbers = StrengthInNumbers;
            enemy.SteamTank = SteamTank;
            enemy.NoArmour = NoArmour;

            return enemy;
        }

        private static List<Enemy> EnemiesMonsters = new List<Enemy>
        {
            NewEnemy(
                Name: "Troll",
                TestListName: "Troll (Orcs&Goblins)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 3,
                BallisticSkill: 1,
                Strength: 5,
                Toughness: 4,
                Wounds: 3,
                Initiative: 1,
                Attacks: 3,
                Leadership: 4,
                Fear: true,
                Regeneration: true,
                Stupidity: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Gyrobomber",
                TestListName: "Gyrobomber (Dwarfs)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 1,
                WeaponSkill: 4,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 5,
                Wounds: 3,
                Initiative: 2,
                Attacks: 2,
                Leadership: 9,
                Armour: 4,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Ancient Kroxigor",
                TestListName: "Ancient Kroxigor (Lizardmen)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 3,
                BallisticSkill: 1,
                Strength: 7,
                Toughness: 4,
                Wounds: 3,
                Initiative: 1,
                Attacks: 4,
                Leadership: 7,
                Fear: true,
                ColdBlooded: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Tomb Scorpion",
                TestListName: "Tomb Scorpion (Tomb Kings)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 7,
                WeaponSkill: 4,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 5,
                Wounds: 3,
                Initiative: 3,
                Attacks: 4,
                Leadership: 8,
                Undead: true,
                KillingBlow: true,
                PoisonAttack: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Hippogryph",
                TestListName: "Hippogryph (Bretonnia)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 8,
                WeaponSkill: 4,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 5,
                Wounds: 4,
                Initiative: 4,
                Attacks: 4,
                Leadership: 8,
                Terror: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Griffon",
                TestListName: "Griffon (The Empire)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 5,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 5,
                Wounds: 4,
                Initiative: 5,
                Attacks: 4,
                Leadership: 7,
                Terror: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Manticore",
                TestListName: "Manticore (Chaos)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 5,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 5,
                Wounds: 4,
                Initiative: 5,
                Attacks: 4,
                Leadership: 5,
                Terror: true,
                KillingBlow: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Varghulf",
                TestListName: "Varghulf (Vampire)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 8,
                WeaponSkill: 5,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 5,
                Wounds: 4,
                Initiative: 2,
                Attacks: 5,
                Leadership: 4,
                Terror: true,
                Undead: true,
                Regeneration: true,
                Hate: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "War Hydra",
                TestListName: "War Hydra (Dark Elves)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 4,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 5,
                Wounds: 6,
                Initiative: 2,
                Attacks: 5,
                Leadership: 6,
                Armour: 4,
                Terror: true,
                LargeBase: true,

                Mount: NewEnemy(
                    Name: "Apparentice",
                    Type: UnitType.Mount,
                    Size: 2,
                    Movement: 5,
                    WeaponSkill: 4,
                    BallisticSkill: 4,
                    Strength: 3,
                    Toughness: 3,
                    Wounds: 2,
                    Initiative: 3,
                    Attacks: 2,
                    Leadership: 8,
                    NoKollingBlow: true
                )
            ),

            NewEnemy(
                Name: "Dragon Ogre",
                TestListName: "Dragon Ogre Shaggoth (Beastmen)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 7,
                WeaponSkill: 6,
                BallisticSkill: 3,
                Strength: 5,
                Toughness: 5,
                Wounds: 6,
                Initiative: 4,
                Attacks: 5,
                Leadership: 9,
                Armour: 4,
                Terror: true,
                ImmuneToPsychology: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Stegadon",
                TestListName: "Stegadon (Lizardmen)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 3,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 6,
                Wounds: 5,
                Initiative: 2,
                Attacks: 4,
                Leadership: 5,
                Armour: 4,
                LargeBase: true,
                ColdBlooded: true,
                Terror: true,
                Stubborn: true,
                ImmuneToPsychology: true,

                Mount: NewEnemy(
                    Name: "Skink Crew",
                    Type: UnitType.Mount,
                    Size: 5,
                    Movement: 6,
                    WeaponSkill: 2,
                    BallisticSkill: 3,
                    Strength: 3,
                    Toughness: 2,
                    Wounds: 4,
                    Initiative: 4,
                    Attacks: 4,
                    Leadership: 5,
                    Armour: 4,
                    ColdBlooded: true,
                    PoisonAttack: true,
                    NoKollingBlow: true
                )
            ),

            NewEnemy(
                Name: "Treeman",
                TestListName: "Treeman (Wood Elves)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 5,
                WeaponSkill: 5,
                BallisticSkill: 0,
                Strength: 6,
                Toughness: 6,
                Wounds: 6,
                Initiative: 2,
                Attacks: 5,
                Leadership: 8,
                Armour: 3,
                Terror: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Hell Pit Abomination",
                TestListName: "Hell Pit Abomination (Skaven)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 0,
                WeaponSkill: 3,
                BallisticSkill: 1,
                Strength: 6,
                Toughness: 5,
                Wounds: 6,
                Initiative: 4,
                Attacks: 0,
                Leadership: 8,
                Regeneration: true,
                Terror: true,
                Stubborn: true,
                LargeBase: true,
                HellPitAbomination: true
            ),

            NewEnemy(
                Name: "Necrosphinx",
                TestListName: "Necrosphinx (Tomb Kings)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 4,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 8,
                Wounds: 5,
                Initiative: 1,
                Attacks: 5,
                Leadership: 8,
                Armour: 3,
                Terror: true,
                LargeBase: true,
                HeroicKillingBlow: true,
                Undead: true
            ),

            NewEnemy(
                Name: "Star Dragon",
                TestListName: "Star Dragon (High Elves)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 7,
                BallisticSkill: 0,
                Strength: 7,
                Toughness: 6,
                Wounds: 7,
                Initiative: 2,
                Attacks: 6,
                Leadership: 9,
                Armour: 3,
                Terror: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Steam Tank",
                TestListName: "Steam Tank (Empire)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 0,
                WeaponSkill: 0,
                BallisticSkill: 4,
                Strength: 6,
                Toughness: 6,
                Wounds: 10,
                Initiative: 0,
                Attacks: 0,
                Leadership: 10,
                Armour: 1,
                Unbreakable: true,
                Terror: true,
                LargeBase: true,
                SteamTank: true
            ),
        };

        private static List<Enemy> EnemiesCoreUnits = new List<Enemy>
        {
            NewEnemy(
                Name: "Clanrat Slaves",
                TestListName: "20 Clanrat Slaves (Skaven)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 5,
                WeaponSkill: 2,
                BallisticSkill: 2,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 4,
                Attacks: 1,
                Leadership: 2,
                StrengthInNumbers: true
            ),

            NewEnemy(
                Name: "Men-at-arms",
                TestListName: "20 Men-at-arms (Bretonnia)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 4,
                WeaponSkill: 2,
                BallisticSkill: 2,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 3,
                Attacks: 1,
                Leadership: 5,
                Armour: 5
            ),

            NewEnemy(
                Name: "Empire swordmens",
                TestListName: "20 Empire swordmens (Empire)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 4,
                WeaponSkill: 3,
                BallisticSkill: 3,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 3,
                Attacks: 1,
                Leadership: 7,
                Armour: 6
            ),

            NewEnemy(
                Name: "Orc boys",
                TestListName: "20 Orc Boys (Orcs&Goblins)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 4,
                WeaponSkill: 3,
                BallisticSkill: 3,
                Strength: 3,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 1,
                Leadership: 7,
                Armour: 5
            ),

            NewEnemy(
                Name: "Skeleton Warriors",
                TestListName: "20 Skeleton Warriors (Tomb Kings)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 4,
                WeaponSkill: 2,
                BallisticSkill: 2,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 2,
                Attacks: 1,
                Leadership: 5,
                Armour: 5,
                Undead: true
            ),

            NewEnemy(
                Name: "Lothern Sea Guard",
                TestListName: "20 Lothern Sea Guard (High Elves)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 5,
                WeaponSkill: 4,
                BallisticSkill: 4,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 5,
                Attacks: 1,
                Leadership: 8,
                Armour: 6,
                HitFirst: true
            ),

            NewEnemy(
                Name: "Crypt Ghouls",
                TestListName: "20 Crypt Ghouls (Vampire Counts)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 4,
                WeaponSkill: 3,
                BallisticSkill: 0,
                Strength: 3,
                Toughness: 4,
                Wounds: 1,
                Initiative: 3,
                Attacks: 2,
                Leadership: 5,
                Undead: true,
                PoisonAttack: true
            ),

            NewEnemy(
                Name: "Back Ark Corsairs",
                TestListName: "20 Back Ark Corsairs (Dark Elves)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 5,
                WeaponSkill: 4,
                BallisticSkill: 4,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 5,
                Attacks: 2,
                Leadership: 8,
                Armour: 5,
                Hate: true
            ),

            NewEnemy(
                Name: "Dryads",
                TestListName: "20 Dryads (Wood Elves)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 5,
                WeaponSkill: 4,
                BallisticSkill: 0,
                Strength: 4,
                Toughness: 4,
                Wounds: 1,
                Initiative: 6,
                Attacks: 2,
                Leadership: 8,
                Fear: true
            ),

            NewEnemy(
                Name: "Bestigor",
                TestListName: "20 Bestigor (Beastmen)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 5,
                WeaponSkill: 4,
                BallisticSkill: 3,
                Strength: 6,
                Toughness: 4,
                Wounds: 1,
                Initiative: 3,
                Attacks: 1,
                Leadership: 7,
                Armour: 5,
                HitLast: true
            ),

            NewEnemy(
                Name: "Knights of the Realms",
                TestListName: "8 Knights of the Realms (Bretonnia)",
                Type: UnitType.Core,
                Size: 8,
                Movement: 4,
                WeaponSkill: 4,
                BallisticSkill: 3,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 3,
                Attacks: 1,
                Leadership: 8,
                Armour: 2,
                Lance: true,

                Mount: NewEnemy(
                    Name: "Warhorse",
                    Type: UnitType.Mount,
                    Size: 8,
                    Movement: 8,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 3,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 1,
                    Leadership: 5
                )
            ),

            NewEnemy(
                Name: "Longbeards",
                TestListName: "20 Longbeards (Dwarfs)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 3,
                WeaponSkill: 5,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 1,
                Leadership: 9,
                Armour: 4,
                ImmuneToPsychology: true
            ),

            NewEnemy(
                Name: "Temple Guard",
                TestListName: "20 Temple Guard (Lizardmen)",
                Type: UnitType.Core,
                Size: 20,
                Movement: 4,
                WeaponSkill: 4,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 2,
                Leadership: 8,
                Armour: 4,
                ColdBlooded: true
            ),

            NewEnemy(
                Name: "Chosen Knights of Chaos",
                TestListName: "8 Chosen Knights (Chaos)",
                Type: UnitType.Core,
                Size: 8,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 3,
                Strength: 5,
                Toughness: 4,
                Wounds: 1,
                Initiative: 5,
                Attacks: 2,
                Leadership: 8,
                Armour: 1,

                Mount: NewEnemy(
                    Name: "Chaos Steed",
                    Type: UnitType.Mount,
                    Size: 8,
                    Movement: 8,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 1,
                    Leadership: 5
                )
            ),
        };

        private static List<Enemy> EnemiesSpecialUnits = new List<Enemy>
        {
            NewEnemy(
                Name: "Tree Kin",
                TestListName: "5 Tree Kin (Wood Elves)",
                Type: UnitType.Special,
                Size: 5,
                Movement: 5,
                WeaponSkill: 4,
                BallisticSkill: 4,
                Strength: 4,
                Toughness: 5,
                Wounds: 3,
                Initiative: 3,
                Attacks: 3,
                Leadership: 8,
                Armour: 4,
                Fear: true
            ),

            NewEnemy(
                Name: "Pegasus Knights",
                TestListName: "8 Pegasus Knights (Bretonnia)",
                Type: UnitType.Special,
                Size: 8,
                Movement: 4,
                WeaponSkill: 4,
                BallisticSkill: 3,
                Strength: 3,
                Toughness: 4,
                Wounds: 2,
                Initiative: 4,
                Attacks: 1,
                Leadership: 8,
                Armour: 3,
                Lance: true,

                Mount: NewEnemy(
                    Name: "Boar",
                    Type: UnitType.Mount,
                    Size: 8,
                    Movement: 8,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 4,
                    Attacks: 2,
                    Leadership: 7
                )
            ),

            NewEnemy(
                Name: "Chaos Ogres",
                TestListName: "5 Chaos Ogres (Beastmen)",
                Type: UnitType.Special,
                Size: 5,
                Movement: 6,
                WeaponSkill: 3,
                BallisticSkill: 2,
                Strength: 4,
                Toughness: 4,
                Wounds: 3,
                Initiative: 2,
                Attacks: 3,
                Leadership: 7,
                Armour: 6,
                Fear: true
            ),

             NewEnemy(
                Name: "Plague Monks",
                TestListName: "20 Plague Monks (Skaven)",
                Type: UnitType.Special,
                Size: 20,
                Movement: 5,
                WeaponSkill: 3,
                BallisticSkill: 3,
                Strength: 3,
                Toughness: 4,
                Wounds: 1,
                Initiative: 3,
                Attacks: 3,
                Leadership: 5,
                Frenzy: true,
                StrengthInNumbers: true
            ),

            NewEnemy(
                Name: "Tomb Guard",
                TestListName: "16 Tomb Guard (Tomb Kings)",
                Type: UnitType.Special,
                Size: 16,
                Movement: 4,
                WeaponSkill: 3,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 4,
                Wounds: 1,
                Initiative: 3,
                Attacks: 1,
                Leadership: 8,
                Armour: 5,
                KillingBlow: true,
                Undead: true
            ),

            NewEnemy(
                Name: "Grave Guard",
                TestListName: "16 Grave Guard (Vampire Counts)",
                Type: UnitType.Special,
                Size: 16,
                Movement: 4,
                WeaponSkill: 3,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 4,
                Wounds: 1,
                Initiative: 3,
                Attacks: 2,
                Leadership: 8,
                Armour: 4,
                KillingBlow: true,
                Undead: true
            ),

            NewEnemy(
                Name: "Greatswords",
                TestListName: "20 Greatswords (Empire)",
                Type: UnitType.Special,
                Size: 20,
                Movement: 4,
                WeaponSkill: 4,
                BallisticSkill: 3,
                Strength: 5,
                Toughness: 3,
                Wounds: 1,
                Initiative: 3,
                Attacks: 2,
                Leadership: 8,
                Armour: 5,
                HitLast: true,
                Stubborn: true
            ),

            NewEnemy(
                Name: "Black Orcs",
                TestListName: "16 Black Orcs (Orcs&Goblins)",
                Type: UnitType.Special,
                Size: 16,
                Movement: 4,
                WeaponSkill: 4,
                BallisticSkill: 3,
                Strength: 5,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 1,
                Leadership: 8,
                Armour: 5,
                HitLast: true
            ),

            NewEnemy(
                Name: "Orc Boar Boys",
                TestListName: "8 Orc Boar Boys (Orcs&Goblins)",
                Type: UnitType.Special,
                Size: 8,
                Movement: 4,
                WeaponSkill: 3,
                BallisticSkill: 3,
                Strength: 3,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 1,
                Leadership: 7,
                Armour: 3,

                Mount: NewEnemy(
                    Name: "Boar",
                    Type: UnitType.Mount,
                    Size: 8,
                    Movement: 7,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 3,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 1,
                    Leadership: 3
                )
            ),

            NewEnemy(
                Name: "Cold One Knights",
                TestListName: "8 Cold One Knights (Dark Elves)",
                Type: UnitType.Special,
                Size: 8,
                Movement: 5,
                WeaponSkill: 5,
                BallisticSkill: 4,
                Strength: 4,
                Toughness: 3,
                Wounds: 1,
                Initiative: 6,
                Attacks: 1,
                Leadership: 9,
                Armour: 2,
                Hate: true,
                Fear: true,
                Lance: true,

                Mount: NewEnemy(
                    Name: "Cold One",
                    Type: UnitType.Mount,
                    Size: 8,
                    Movement: 7,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 2,
                    Attacks: 1,
                    Leadership: 3
                )
            ),


            NewEnemy(
                Name: "Cold One Cavalry",
                TestListName: "8 Cold One Cavalry (Lizardmen)",
                Type: UnitType.Special,
                Size: 8,
                Movement: 4,
                WeaponSkill: 4,
                BallisticSkill: 0,
                Strength: 4,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 2,
                Leadership: 8,
                Armour: 2,
                ColdBlooded: true,
                Fear: true,

                Mount: NewEnemy(
                    Name: "Cold One",
                    Type: UnitType.Mount,
                    Size: 8,
                    Movement: 7,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 2,
                    Attacks: 1,
                    Leadership: 3
                )
            ),

            NewEnemy(
                Name: "Bloodletters",
                TestListName: "20 Bloodletters (Chaos)",
                Type: UnitType.Special,
                Size: 20,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 3,
                Wounds: 1,
                Initiative: 4,
                Attacks: 2,
                Leadership: 8,
                Armour: 6,
                Frenzy: true
            ),

            NewEnemy(
                Name: "Sword Masters",
                TestListName: "16 Sword Masters of Hoeth (High Elves)",
                Type: UnitType.Special,
                Size: 16,
                Movement: 5,
                WeaponSkill: 6,
                BallisticSkill: 4,
                Strength: 5,
                Toughness: 3,
                Wounds: 1,
                Initiative: 5,
                Attacks: 2,
                Leadership: 8,
                Armour: 5,
                HitFirst: true
            ),

            NewEnemy(
                Name: "Hammerers",
                TestListName: "16 Hammerers (Dwarfs)",
                Type: UnitType.Special,
                Size: 16,
                Movement: 3,
                WeaponSkill: 5,
                BallisticSkill: 3,
                Strength: 6,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 1,
                Leadership: 9,
                Armour: 5,
                Stubborn: true
            ),
        };

        private static List<Enemy> EnemiesRareUnits = new List<Enemy>
        {
            NewEnemy(
                Name: "Snotling Pump Wagon",
                TestListName: "Snotling Pump Wagon (Orcs&Goblins)",
                Type: UnitType.Rare,
                Size: 1,
                Movement: 6,
                WeaponSkill: 2,
                BallisticSkill: 0,
                Strength: 2,
                Toughness: 4,
                Wounds: 3,
                Initiative: 3,
                Attacks: 5,
                Leadership: 4,
                Unbreakable: true,
                Armour: 6
            ),

            NewEnemy(
                Name: "Flagellants",
                TestListName: "24 Flagellant Warband (The Empire)",
                Type: UnitType.Rare,
                Size: 24,
                Movement: 4,
                WeaponSkill: 2,
                BallisticSkill: 2,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 3,
                Attacks: 1,
                Leadership: 10,
                Unbreakable: true,
                Frenzy: true,
                Flail: true
            ),

            NewEnemy(
                Name: "Waywathers",
                TestListName: "16 Waywathers (Wood Elves)",
                Type: UnitType.Rare,
                Size: 16,
                Movement: 5,
                WeaponSkill: 4,
                BallisticSkill: 5,
                Strength: 3,
                Toughness: 3,
                Wounds: 1,
                Initiative: 5,
                Attacks: 2,
                Leadership: 8,
                HitFirst: true
            ),

            NewEnemy(
                Name: "White Lions",
                TestListName: "16 White Lions (High Elves)",
                Type: UnitType.Rare,
                Size: 16,
                Movement: 5,
                WeaponSkill: 5,
                BallisticSkill: 4,
                Strength: 6,
                Toughness: 3,
                Wounds: 1,
                Initiative: 5,
                Attacks: 1,
                Leadership: 8,
                Armour: 6,
                HitFirst: true
            ),

            NewEnemy(
                Name: "Black Guard",
                TestListName: "16 Black Guard (Dark Elves)",
                Type: UnitType.Rare,
                Size: 16,
                Movement: 5,
                WeaponSkill: 5,
                BallisticSkill: 4,
                Strength: 4,
                Toughness: 3,
                Wounds: 1,
                Initiative: 6,
                Attacks: 1,
                Leadership: 9,
                Armour: 5,
                Hate: true,
                Stubborn: true
            ),

            NewEnemy(
                Name: "Troll Slayers",
                TestListName: "16 Troll Slayers (Dwarfs)",
                Type: UnitType.Rare,
                Size: 16,
                Movement: 3,
                WeaponSkill: 4,
                BallisticSkill: 3,
                Strength: 5,
                Toughness: 4,
                Wounds: 1,
                Initiative: 2,
                Attacks: 1,
                Leadership: 10,
                Unbreakable: true
            ),

            NewEnemy(
                Name: "Grail Knights",
                TestListName: "12 Grail Knights (Bretonnia)",
                Type: UnitType.Rare,
                Size: 12,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 3,
                Wounds: 1,
                Initiative: 5,
                Attacks: 2,
                Leadership: 8,
                Armour: 2,
                Ward: 5,
                Lance: true,

                Mount: NewEnemy(
                    Type: UnitType.Mount,
                    Name: "Warhorse",
                    Size: 12,
                    Movement: 8,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 3,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 1,
                    Leadership: 5
                )
            ),

            NewEnemy(
                Name: "Blood Knights",
                TestListName: "6 Blood Knights (Vampire Counts)",
                Type: UnitType.Rare,
                Size: 6,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 3,
                Strength: 5,
                Toughness: 4,
                Wounds: 1,
                Initiative: 4,
                Attacks: 2,
                Leadership: 7,
                Armour: 2,
                Ward: 5,
                Frenzy: true,
                Undead: true,
                Lance: true,

                Mount: NewEnemy(
                    Type: UnitType.Mount,
                    Name: "Nightmare",
                    Size: 6,
                    Movement: 8,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 2,
                    Attacks: 1,
                    Leadership: 3
                )
            ),

            NewEnemy(
                Name: "Skullcrushers",
                TestListName: "6 Skullcrushers of Khorne (Chaos)",
                Type: UnitType.Rare,
                Size: 6,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 4,
                Wounds: 1,
                Initiative: 5,
                Attacks: 2,
                Leadership: 8,
                Armour: 1,
                Fear: true,

                Mount: NewEnemy(
                    Type: UnitType.Mount,
                    Name: "Juggernaut",
                    Size: 6,
                    Movement: 7,
                    WeaponSkill: 5,
                    BallisticSkill: 0,
                    Strength: 5,
                    Toughness: 4,
                    Wounds: 3,
                    Initiative: 2,
                    Attacks: 3,
                    Leadership: 7
                )
            ),
        };

        private static List<Enemy> EnemiesHeroes = new List<Enemy>
        {
            NewEnemy(
                Name: "Tretch Craventail",
                TestListName: "Tretch Craventail (Skaven)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 5,
                WeaponSkill: 5,
                BallisticSkill: 4,
                Strength: 4,
                Toughness: 4,
                Wounds: 2,
                Initiative: 6,
                Attacks: 4,
                Leadership: 6,
                Armour: 5,
                Ward: 4
            ),

            NewEnemy(
                Name: "The Herald Nekaph",
                TestListName: "The Herald Nekaph (Tomb Kings)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 4,
                Wounds: 2,
                Initiative: 3,
                Attacks: 3,
                Leadership: 8,
                Ward: 5,
                KillingBlow: true,
                Undead: true,
                Flail: true,
                MultiWounds: "2"
            ),

            NewEnemy(
                Name: "Gitilla",
                TestListName: "Gitilla da Hunter (Orcs&Goblins)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 4,
                WeaponSkill: 4,
                BallisticSkill: 4,
                Strength: 4,
                Toughness: 4,
                Wounds: 2,
                Initiative: 4,
                Attacks: 3,
                Leadership: 7,
                Armour: 3,

                Mount: NewEnemy(
                    Type: UnitType.Mount,
                    Name: "Ulda the Great Wolf",
                    Size: 1,
                    Movement: 9,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 3,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 2,
                    Leadership: 3
                )
            ),

            NewEnemy(
                Name: "Moonclaw",
                TestListName: "Moonclaw, Son of Murrslieb (Beastmen)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 5,
                WeaponSkill: 3,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 4,
                Wounds: 2,
                Initiative: 3,
                Attacks: 3,
                Leadership: 7,
                Ward: 5,

                Mount: NewEnemy(
                    Type: UnitType.Mount,
                    Name: "Umbralok",
                    Size: 1,
                    Movement: 7,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 2,
                    Attacks: 3,
                    Leadership: 6
                )
            ),

            NewEnemy(
                Name: "Ludwig Schwarzhelm",
                TestListName: "Ludwig Schwarzhelm (The Empire)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 4,
                WeaponSkill: 6,
                BallisticSkill: 5,
                Strength: 4,
                Toughness: 4,
                Wounds: 2,
                Initiative: 5,
                Attacks: 3,
                Leadership: 8,
                Armour: 2,
                KillingBlow: true,
                Reroll: "ToWound",

                Mount: NewEnemy(
                    Type: UnitType.Mount,
                    Name: "Warhorse",
                    Size: 1,
                    Movement: 8,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 3,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 1,
                    Leadership: 5
                )
            ),

            NewEnemy(
                Name: "Gor-Rok",
                TestListName: "Gor-Rok, The Great White Lizard (Lizardmen)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 0,
                Strength: 5,
                Toughness: 5,
                Wounds: 2,
                Initiative: 3,
                Attacks: 4,
                Leadership: 8,
                ColdBlooded: true,
                Armour: 3,
                Stubborn: true,
                NoKollingBlow: true,
                Reroll: "ToHit;OpponentToWound"
            ),

            NewEnemy(
                Name: "Josef Bugman",
                TestListName: "Josef Bugman Master Brewer (Dwarfs)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 3,
                WeaponSkill: 6,
                BallisticSkill: 5,
                Strength: 5,
                Toughness: 5,
                Wounds: 2,
                Initiative: 4,
                Attacks: 4,
                Leadership: 10,
                Armour: 3,
                Ward: 4,
                ImmuneToPsychology: true
            ),

            NewEnemy(
                Name: "Drycha",
                TestListName: "Drycha (Wood Elves)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 5,
                WeaponSkill: 7,
                BallisticSkill: 4,
                Strength: 5,
                Toughness: 4,
                Wounds: 3,
                Initiative: 8,
                Attacks: 5,
                Leadership: 8,
                Terror: true,
                Reroll: "ToHit"
            ),

            NewEnemy(
                Name: "Caradryan",
                TestListName: "Caradryan, Capitain ot The Phoenix Guard (High Elves)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 5,
                WeaponSkill: 6,
                BallisticSkill: 6,
                Strength: 4,
                Toughness: 3,
                Wounds: 2,
                Initiative: 7,
                Attacks: 3,
                Leadership: 9,
                Armour: 5,
                Ward: 4,
                Fear: true,
                HitFirst: true,
                MultiWounds: "D3"
            ),

            NewEnemy(
                Name: "Konrad",
                TestListName: "Konrad Von Carstein (Vampire Counts)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 6,
                WeaponSkill: 7,
                BallisticSkill: 4,
                Strength: 5,
                Toughness: 4,
                Wounds: 2,
                Initiative: 6,
                Attacks: 4,
                Leadership: 6,
                Armour: 5,
                Fear: true,
                HitFirst: true,
                Reroll: "ToHit",
                MultiWounds: "2",
                Undead: true
            ),

            NewEnemy(
                Name: "Throgg",
                TestListName: "Throgg, King of Trolls (Chaos)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 6,
                WeaponSkill: 5,
                BallisticSkill: 2,
                Strength: 6,
                Toughness: 5,
                Wounds: 4,
                Initiative: 2,
                Attacks: 5,
                Leadership: 8,
                Fear: true,
                Regeneration: true,
                LargeBase: true
            ),

            NewEnemy(
                Name: "Malus Darkblade (Tz'arkan)",
                TestListName: "Malus Darkblade in Tz'arkan state (Dark Elves)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 6,
                WeaponSkill: 7,
                BallisticSkill: 5,
                Strength: 5,
                Toughness: 5,
                Wounds: 2,
                Initiative: 9,
                Attacks: 3,
                Leadership: 10,
                Armour: 3,
                Reroll: "ToWound",
                NoArmour: true,

                Mount: NewEnemy(
                    Name: "Spite",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 7,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 2,
                    Attacks: 2,
                    Leadership: 4,
                    Armour: 5,
                    Fear: true
                )
            ),

            NewEnemy(
                Name: "Deathmaster Snikch",
                TestListName: "Deathmaster Snikch (Skaven)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 6,
                WeaponSkill: 8,
                BallisticSkill: 6,
                Strength: 4,
                Toughness: 4,
                Wounds: 2,
                Initiative: 10,
                Attacks: 6,
                Leadership: 8,
                Ward: 4,
                HitFirst: true,
                ArmourPiercing: 2,
                MultiWounds: "D3"
            ),

            NewEnemy(
                Name: "Chakax",
                TestListName: "Chakax, The Eternity Warden (Lizardmen)",
                Type: UnitType.Hero,
                Size: 1,
                Movement: 4,
                WeaponSkill: 5,
                BallisticSkill: 0,
                Strength: 7,
                Toughness: 5,
                Wounds: 2,
                Initiative: 3,
                Attacks: 4,
                Leadership: 8,
                ColdBlooded: true,
                Armour: 4,
                Ward: 5,
                Unbreakable: true,
                HitFirst: true,
                Reroll: "ToHit"
            ),
        };

        private static List<Enemy> EnemiesLords = new List<Enemy>
        {
            NewEnemy(
                Name: "Green Knight",
                TestListName: "Green Knight (Bretonnia)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 6,
                WeaponSkill: 7,
                BallisticSkill: 3,
                Strength: 6,
                Toughness: 4,
                Wounds: 3,
                Initiative: 6,
                Attacks: 4,
                Leadership: 9,
                Ward: 5,
                ImmuneToPsychology: true,
                Terror: true,
                Undead: true,

                Mount: NewEnemy(
                    Name: "Shadow Steed",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 8,
                    WeaponSkill: 4,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 4,
                    Attacks: 1,
                    Leadership: 5,
                    Armour: 5
                )
            ),

            NewEnemy(
                Name: "Khuzrak",
                TestListName: "Khuzrak, The One-eye (Beastmen)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 5,
                WeaponSkill: 7,
                BallisticSkill: 1,
                Strength: 5,
                Toughness: 5,
                Wounds: 3,
                Initiative: 5,
                Attacks: 4,
                Leadership: 9,
                Armour: 2
            ),


            NewEnemy(
                Name: "Khalida",
                TestListName: "High Queen Khalida, Beloved of Asaph (Tomb Kings)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 6,
                WeaponSkill: 6,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 5,
                Wounds: 3,
                Initiative: 9,
                Attacks: 5,
                Leadership: 10,
                HitFirst: true,
                Undead: true,
                PoisonAttack: true
            ),

            NewEnemy(
                Name: "Louen Leoncoeur",
                TestListName: "Louen Leoncoeur, The Lionhearted (Bretonnia)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 4,
                WeaponSkill: 7,
                BallisticSkill: 5,
                Strength: 5,
                Toughness: 4,
                Wounds: 3,
                Initiative: 7,
                Attacks: 5,
                Leadership: 9,
                Armour: 3,
                Ward: 5,
                Lance: true,
                ImmuneToPsychology: true,
                Regeneration: true,
                Reroll: "ToHit;ToWound",

                Mount: NewEnemy(
                    Name: "Beaquis",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 8,
                    WeaponSkill: 5,
                    BallisticSkill: 0,
                    Strength: 5,
                    Toughness: 5,
                    Wounds: 4,
                    Initiative: 6,
                    Attacks: 4,
                    Leadership: 9,
                    Armour: 5,
                    Terror: true
                )
            ),

            NewEnemy(
                Name: "Kurt Helborg",
                TestListName: "Kurt Helborg (The Empire)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 6,
                WeaponSkill: 7,
                BallisticSkill: 3,
                Strength: 4,
                Toughness: 4,
                Wounds: 3,
                Initiative: 6,
                Attacks: 4,
                Leadership: 9,
                Armour: 2,
                Stubborn: true,
                ImmuneToPsychology: true,
                AutoWound: true,
                NoArmour: true,

                Mount: NewEnemy(
                    Name: "Warhorse",
                    Type: UnitType.Mount,
                    Size: 8,
                    Movement: 8,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 3,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 1,
                    Leadership: 5
                )
            ),

            NewEnemy(
                Name: "Zacharias",
                TestListName: "Zacharias The Everliving (Vampire Counts)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 6,
                WeaponSkill: 6,
                BallisticSkill: 6,
                Strength: 5,
                Toughness: 5,
                Wounds: 4,
                Initiative: 8,
                Attacks: 5,
                Leadership: 10,
                Ward: 4,
                Undead: true,

                Mount: NewEnemy(
                    Name: "Zombie Dragon",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 6,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 6,
                    Toughness: 6,
                    Wounds: 6,
                    Initiative: 1,
                    Attacks: 4,
                    Leadership: 4,
                    Armour: 5,
                    Terror: true,
                    Undead: true
                )
            ),

            NewEnemy(
                Name: "Karl Franz",
                TestListName: "The Emperor Karl Franz (The Empire)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 4,
                WeaponSkill: 6,
                BallisticSkill: 5,
                Strength: 4,
                Toughness: 4,
                Wounds: 3,
                Initiative: 6,
                Attacks: 4,
                Leadership: 10,
                Armour: 4,
                Ward: 4,
                AutoWound: true,
                NoArmour: true,
                MultiWounds: "D3",

                Mount: NewEnemy(
                    Name: "Deathclaw",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 6,
                    WeaponSkill: 6,
                    BallisticSkill: 0,
                    Strength: 5,
                    Toughness: 5,
                    Wounds: 4,
                    Initiative: 5,
                    Attacks: 4,
                    Leadership: 8,
                    Terror: true
                )
            ),

            NewEnemy(
                Name: "Tyrion",
                TestListName: "Tyrion, The Defender of Ulthuan (High Elves)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 5,
                WeaponSkill: 9,
                BallisticSkill: 7,
                Strength: 7,
                Toughness: 3,
                Wounds: 4,
                Initiative: 10,
                Attacks: 4,
                Leadership: 10,
                HitFirst: true,
                Armour: 1,
                Ward: 4,
                Regeneration: true,

                Mount: NewEnemy(
                    Name: "Malhandir",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 10,
                    WeaponSkill: 4,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 3,
                    Wounds: 1,
                    Initiative: 5,
                    Attacks: 2,
                    Leadership: 7
                )
            ),

            NewEnemy(
                Name: "Torgrim Grudgebearer",
                TestListName: "Torgrim Grudgebearer (Dwarfs)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 3,
                WeaponSkill: 7,
                BallisticSkill: 6,
                Strength: 4,
                Toughness: 5,
                Wounds: 7,
                Initiative: 4,
                Attacks: 4,
                Leadership: 10,
                Armour: 2,
                Ward: 4,
                HitFirst: true,
                ImmuneToPsychology: true,
                Stubborn: true,

                Mount: NewEnemy(
                    Name: "Thronebearers",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 3,
                    WeaponSkill: 5,
                    BallisticSkill: 3,
                    Strength: 4,
                    Toughness: 0,
                    Wounds: 1,
                    Initiative: 3,
                    Attacks: 4,
                    Leadership: 0
                )
            ),

            NewEnemy(
                Name: "Orion",
                TestListName: "Orion, The King in the Wood (Wood Elves)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 9,
                WeaponSkill: 8,
                BallisticSkill: 8,
                Strength: 6,
                Toughness: 5,
                Wounds: 5,
                Initiative: 9,
                Attacks: 5,
                Leadership: 10,
                HitFirst: true,
                Frenzy: true,
                Terror: true,
                Unbreakable: true,
                Ward: 5,

                Mount: NewEnemy(
                    Name: "Hound of Orion",
                    Type: UnitType.Hero,
                    Size: 2,
                    Movement: 9,
                    WeaponSkill: 4,
                    BallisticSkill: 0,
                    Strength: 4,
                    Toughness: 4,
                    Wounds: 1,
                    Initiative: 4,
                    Attacks: 1,
                    Leadership: 6,
                    Frenzy: true,
                    Unbreakable: true
                )
            ),

            NewEnemy(
                Name: "Vermin Lord",
                TestListName: "Vermin Lord (Skaven)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 8,
                WeaponSkill: 8,
                BallisticSkill: 4,
                Strength: 6,
                Toughness: 5,
                Wounds: 5,
                Initiative: 10,
                Attacks: 5,
                Leadership: 8,
                Ward: 5,
                ImmuneToPsychology: true,
                Terror: true,
                MultiWounds: "D3"
            ),


            NewEnemy(
                Name: "Malekith",
                TestListName: "Malekith, Witch King of Naggaroth (Dark Elves)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 8,
                WeaponSkill: 5,
                BallisticSkill: 4,
                Strength: 6,
                Toughness: 3,
                Wounds: 3,
                Initiative: 8,
                Attacks: 4,
                Leadership: 10,
                NoArmour: true,
                Armour: 4,
                Ward: 2,

                Mount: NewEnemy(
                    Name: "Seraphon",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 6,
                    WeaponSkill: 6,
                    BallisticSkill: 0,
                    Strength: 6,
                    Toughness: 6,
                    Wounds: 6,
                    Initiative: 4,
                    Attacks: 5,
                    Leadership: 8,
                    Armour: 3,
                    Terror: true
                )
            ),

            NewEnemy(
                Name: "Kroq-Gar",
                TestListName: "Kroq-Gar Ancient (Lizardmen)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 4,
                WeaponSkill: 6,
                BallisticSkill: 3,
                Strength: 6,
                Toughness: 5,
                Wounds: 3,
                Initiative: 4,
                Attacks: 5,
                Leadership: 8,
                ColdBlooded: true,
                Armour: 3,
                Ward: 5,
                MultiWounds: "2",

                Mount: NewEnemy(
                    Name: "Grymloq",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 7,
                    WeaponSkill: 3,
                    BallisticSkill: 0,
                    Strength: 7,
                    Toughness: 5,
                    Wounds: 5,
                    Initiative: 2,
                    Attacks: 5,
                    Leadership: 5,
                    Armour: 4,
                    Terror: true,
                    ColdBlooded: true,
                    MultiWounds: "D3",
                    LargeBase: true
                )
            ),

            NewEnemy(
                Name: "Archaon",
                TestListName: "Archaon, Lord of the End Times (Chaos)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 4,
                WeaponSkill: 9,
                BallisticSkill: 5,
                Strength: 5,
                Toughness: 5,
                Wounds: 4,
                Initiative: 7,
                Attacks: 10,
                Leadership: 10,
                Armour: 1,
                Ward: 3,
                ImmuneToPsychology: true,
                NoArmour: true,
                Terror: true,

                Mount: NewEnemy(
                    Name: "Dorghar",
                    Type: UnitType.Mount,
                    Size: 1,
                    Movement: 8,
                    WeaponSkill: 4,
                    BallisticSkill: 0,
                    Strength: 5,
                    Toughness: 5,
                    Wounds: 3,
                    Initiative: 3,
                    Attacks: 3,
                    Leadership: 9,
                    Armour: 4,
                    LargeBase: true
                )
            ),

            NewEnemy(
                Name: "Grimgor Ironhide",
                TestListName: "Grimgor Ironhide (Orcs&Goblins)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 4,
                WeaponSkill: 8,
                BallisticSkill: 1,
                Strength: 7,
                Toughness: 5,
                Wounds: 3,
                Initiative: 5,
                Attacks: 7,
                Leadership: 9,
                Armour: 1,
                Ward: 5,
                Hate: true,
                HitFirst: true,
                ImmuneToPsychology: true
            ),

            NewEnemy(
                Name: "Bloodthister",
                TestListName: "Greater Daemon Bloodthister (Chaos)",
                Type: UnitType.Lord,
                Size: 1,
                Movement: 6,
                WeaponSkill: 10,
                BallisticSkill: 0,
                Strength: 7,
                Toughness: 6,
                Wounds: 7,
                Initiative: 10,
                Attacks: 7,
                Leadership: 9,
                Armour: 4,
                Ward: 5,
                Terror: true,
                KillingBlow: true,
                LargeBase: true
            ),
        };
    }
}
