using BittysSigils;
using DiskCardGame;
using InscryptionAPI.Card;
using System;
using System.Collections.Generic;
using System.Text;
using static BittysChallenges.Abilities;
using static BittysChallenges.Plugin;
using static DiskCardGame.CardAppearanceBehaviour;

namespace BittysChallenges
{
    public class Cards
    {
        public static void AddCards()
        {
            Log.LogInfo("Start of cards");
            Add_Card_TravelingOuroboros();
            Add_Card_GoldenSheep();
            Add_Card_WoodenBoard();
            Add_Card_Cliff();
            Add_Card_Mushrooms();
            Add_Card_IceCube();
            Add_Card_Totem();
            Add_Card_Avalanche();
            Add_Card_Obelisk();
            Add_Card_Minicello();
            Add_Card_DeckSkeletonPirate();
            Add_Card_DeckSkeletonParrot();
            Add_Card_Raft();
            Add_Card_AscenderBane();
            Add_Card_CloverReRoll();
            Add_Card_Test();
            Log.LogInfo("End of cards");
        }
        #region CardLoaders
        private static void Add_Card_Test()
        {
            CardInfo Test = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "ChallengeTest",
                // Card display name.
                "Test",
                // Attack.
                0,
                // Health.
                1,
                // Description
                description: "Testing Card."
            )

            //free card
            .AddAbilities(GiveSlotSpawner.ability)

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_test.png"), Tools.LoadTexture("portrait_test.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, Test);
        }
        private static void Add_Card_Raft()
        {
            CardInfo Raft = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Raft",
                // Card display name.
                "Raft",
                // Attack.
                0,
                // Health.
                1,
                // Description
                description: "A dry patch in the flood."
            )

            //free card
            .AddAbilities(GiveRaft.ability)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_raft.png"), Tools.LoadTexture("portrait_raft.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, Raft);
        }
        private static void Add_Card_AscenderBane()
        {
            CardInfo newCard = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Ascender's Bane",
                // Card display name.
                "Ascender's Bane",
                // Attack.
                0,
                // Health.
                1,
                // Description
                description: "A reminder of past challengers."
            )

            .SetBloodCost(4)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_ascendersbane"), Tools.LoadTexture("portrait_ascendersbane"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, newCard);
        }
        private static void Add_Card_CloverReRoll()
        {
            CardInfo newCard = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Clover",
                // Card display name.
                "",
                // Attack.
                0,
                // Health.
                0,
                // Description
                description: "A reminder of past challengers."
            )

            //card appearance
            .AddAppearances(CAppearances.CloverAppearance)
            .AddSpecialAbilities(AddCloverReRollAbility.CloverReRollSpecialAbility)

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_blank"), Tools.LoadTexture("portrait_blank"))
            .SetExtendedProperty("bitty_spaceNotRequired", true);
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, newCard);
        }
        private static void Add_Card_TravelingOuroboros()
        {
            CardInfo TravelingOuroboros = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "TravelingOuroboros",
                // Card display name.
                "Ouroboros",
                // Attack.
                1,
                // Health.
                1,
                // Description
                description: "My very own Ouroboros."
            )

            //cost
            .SetCost(bloodCost: 2)

            .AddAbilities(GiveFalseUnkillable.ability)

            .AddSpecialAbilities(AddTravelingOuroAbility.TravelingOuroSpecialAbility)
            //card appearance
            .AddAppearances(Appearance.RareCardBackground)

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_ouroboros.png"), Tools.LoadTexture("portrait_ouroboros_emission.png"))

            .AddTribes(Tribe.Reptile)
            .SetIceCube(CardLoader.GetCardByName("Adder"))
            ;
            TravelingOuroboros.defaultEvolutionName = "Jörmungandr";
            // Pass the card to the API.
            CardManager.Add(CardPrefix, TravelingOuroboros);
        }
        private static void Add_Card_GoldenSheep()
        {
            CardInfo GoldenSheep = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "GoldenSheep",
                // Card display name.
                "Chrysomallos",
                // Attack.
                0,
                // Health.
                4,
                // Description
                description: "A mystical, glittering being."
            )

            //cost
            .SetCost(bloodCost: 1)

            .AddAbilities(Ability.StrafeSwap)
            //special ability
            .AddSpecialAbilities(AddGoldenSheepAbility.GoldenSheepSpecialAbility)
            //card appearance
            .AddAppearances(Appearance.RareCardBackground)
            .AddAppearances(GoldEmission.Appearance.GoldEmission)

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_goldram.png"), Tools.LoadTexture("portrait_goldram_emission.png"))

            .AddTribes(Tribe.Hooved)
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, GoldenSheep);
        }
        private static void Add_Card_WoodenBoard()
        {
            CardInfo WoodenBoard = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "WoodenBoard",
                // Card display name.
                "Wooden Board",
                // Attack.
                0,
                // Health.
                1,
                // Description
                description: "A regular wooden board."
            )

            //free card

            .AddAbilities(Ability.Submerge)
            .AddAbilities(Sigils.GiveDeathBell.ability)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_woodenplank.png"), Tools.LoadTexture("portrait_woodenplank.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, WoodenBoard);
        }     
        private static void Add_Card_Cliff()
        {
            CardInfo cliff = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Cliff",
                // Card display name.
                "Cliff",
                // Attack.
                0,
                // Health.
                10,
                // Description
                description: "A solid wall of rock."
            )

            //free card

            .AddAbilities(Ability.MadeOfStone)
            .AddAbilities(Ability.Reach)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_cliff.png"), Tools.LoadTexture("portrait_cliff.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, cliff);
        }
        private static void Add_Card_Mushrooms()
        {
            CardInfo mushrooms = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Mushrooms",
                // Card display name.
                "Mushrooms",
                // Attack.
                0,
                // Health.
                2,
                // Description
                description: "A collection of strange mushrooms. They attract anything nearby."
            )

            //free card

            .AddAbilities(Sigils.GiveMushrooms.ability)
            //card appearance
            .AddTraits(Trait.Terrain)
            .AddAppearances(Appearance.TerrainBackground)

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_mushroom.png"), Tools.LoadTexture("portrait_fungus.png"))
            ;
            mushrooms.defaultEvolutionName = "Mega Mushrooms";
            // Pass the card to the API.
            CardManager.Add(CardPrefix, mushrooms);
        }      
        private static void Add_Card_IceCube()
        {
            CardInfo IceCube = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "IceCube",
                // Card display name.
                "Ice Cube",
                // Attack.
                0,
                // Health.
                1,
                // Description
                description: "A block of ice."
            )

            //free card

            .AddAbilities(Sigils.GiveDeathBell.ability)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_icecube.png"), Tools.LoadTexture("portrait_icecube.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, IceCube);
        }
        private static void Add_Card_Totem()
        {
            CardInfo Totem = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Totem",
                // Card display name.
                "Cursed Totem",
                // Attack.
                0,
                // Health.
                2,
                // Description
                description: "A totem surrounded in mysterious energy."
            )

            //free card
            .AddAbilities(Ability.BuffNeighbours)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_totem.png"), Tools.LoadTexture("portrait_totem_emission.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, Totem);
        }
        private static void Add_Card_Avalanche()
        {
            CardInfo Avalanche = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Avalanche",
                // Card display name.
                "Avalanche",
                // Attack.
                0,
                // Health.
                9,
                // Description
                description: "A monsterous mound of snow."
            )

            //free card
            .AddAbilities(GiveStrafeAvalanche.ability)
            .AddAbilities(Ability.MadeOfStone)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_avalanche.png"), Tools.LoadTexture("portrait_avalanche.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, Avalanche);
        }
        private static void Add_Card_Obelisk()
        {
            CardInfo Obelisk = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Obelisk",
                // Card display name.
                "Obelisk",
                // Attack.
                0,
                // Health.
                10,
                // Description
                description: "A tall mysterious stone."
            )

            //free card

            .AddAbilities(Ability.MadeOfStone)
            //card appearance
            .SetTerrain()

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_obelisk.png"), Tools.LoadTexture("portrait_obelisk.png"))
            ;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, Obelisk);
        }
        private static void Add_Card_Minicello()
        {
            CardInfo minicello = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "Minicello",
                // Card display name.
                "Minicello",
                // Attack.
                1,
                // Health.
                1,
                // Description
                description: "A miniture version of a famous pirate's ship."
            )

            //cost
            .SetCost(bloodCost: 1)

            .AddSpecialAbilities(GiveCannoneer.MinicelloSpecialAbility)
            .AddAbilities(Ability.Submerge)
            .AddAbilities(Ability.SkeletonStrafe)
            //card appearance
            .AddAppearances(Appearance.RareCardBackground)

            .SetIceCube(CardLoader.GetCardByName("SkeletonPirate"))

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_minicello.png"), Tools.LoadTexture("portrait_minicello_emission.png"))
            .SetPixelPortrait(Tools.LoadTexture("pixelportrait_ghostshiprepaired.png"))
            ;
            minicello.defaultEvolutionName = "Mediumcello";
            minicello.temple = CardTemple.Undead;
            // Pass the card to the API.
            CardManager.Add(CardPrefix, minicello);
        }
        private static void Add_Card_DeckSkeletonPirate()
        {
            CardInfo skeletonpirate = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "SkeletonPirate",
                // Card display name.
                "Skeleton Crew",
                // Attack.
                2,
                // Health.
                1,
                // Description
                description: "A loyal member of Royal's crew."
            )
            //cost
            .SetCost(bonesCost: 2)

            .AddAbilities(Ability.Brittle)
            //card appearance

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_skeletonpirate.png"), Tools.LoadTexture("portrait_skeletonpirate_emission.png"))
            .SetPixelPortrait(Tools.LoadTexture("pixelportrait_skeletoncrew.png"))
            .AddAppearances(BittysSigils.Plugin.UndeadAppearance)
            ;
            skeletonpirate.defaultEvolutionName = "Sans";
            // Pass the card to the API.
            CardManager.Add(CardPrefix, skeletonpirate);
        }
        private static void Add_Card_DeckSkeletonParrot()
        {
            CardInfo skeletonparrot = CardManager.New(

                // Card ID Prefix
                modPrefix: CardPrefix,
                // Card internal name.
                "SkeletonParrot",
                // Card display name.
                "Undead Parrot",
                // Attack.
                2,
                // Health.
                3,
                // Description
                description: "A loyal member of Royal's crew."
            )
            .SetTribes(Tribe.Bird)
            //cost
            .SetCost(bloodCost: 1)

            .AddAbilities(Ability.Brittle)
            .AddAbilities(Ability.Flying)
            //card appearance

            .SetPortraitAndEmission(Tools.LoadTexture("portrait_skeletonparrot.png"), Tools.LoadTexture("portrait_skeletonparrot_emission.png"))
            .SetPixelPortrait(Tools.LoadTexture("pixelportrait_undeadparrot.png"))
            .AddAppearances(BittysSigils.Plugin.UndeadAppearance)
            ;
            skeletonparrot.defaultEvolutionName = "Polly";
            // Pass the card to the API.
            CardManager.Add(CardPrefix, skeletonparrot);
        }
        #endregion

    }
}
