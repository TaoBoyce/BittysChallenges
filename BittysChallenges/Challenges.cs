using DiskCardGame;
using InscryptionAPI.Ascension;
using System;
using System.Collections.Generic;
using System.Text;
using static BittysChallenges.Plugin;

namespace BittysChallenges
{
    public class Challenges
    {
        public static AscensionChallengeInfo Challenge_waterborneStarter;
        public static AscensionChallengeInfo Challenge_shockedStarter;
        public static AscensionChallengeInfo Challenge_travelingOuro;
        public static AscensionChallengeInfo Challenge_mycoBotch;
        public static AscensionChallengeInfo Challenge_famine;
        public static AscensionChallengeInfo Challenge_abundance;
        public static AscensionChallengeInfo Challenge_weakStarters;
        public static AscensionChallengeInfo Challenge_sprinter;
        public static AscensionChallengeInfo Challenge_oldFecund;
        public static AscensionChallengeInfo Challenge_goldenSheep;
        public static AscensionChallengeInfo Challenge_harderFinalBoss;
        public static AscensionChallengeInfo Challenge_weakSoulStarter;
        public static AscensionChallengeInfo Challenge_harderBosses;
        public static AscensionChallengeInfo Challenge_infiniteLives;
        public static AscensionChallengeInfo Challenge_reverseScales;
        public static AscensionChallengeInfo Challenge_environment;
        public static AscensionChallengeInfo Challenge_vineBoom;
        public static AscensionChallengeInfo Challenge_fleetingSquirrels;
        public static AscensionChallengeInfo Challenge_unfairHand;
        public static AscensionChallengeInfo Challenge_ascenderBane;
        public static AscensionChallengeInfo Challenge_redrawHand;
        public static AscensionChallengeInfo Challenge_champion;

        public static void AddChallenges()
        {
            Log.LogInfo("Start of challenges");
            AddMycoChallenge();
            AddWeakStartersChallenge();
            AddWaterborneStarterChallenge();
            AddShockedStarterChallenge();
            AddWeakSoulStarterChallenge();
            AddSprinterChallenge();
            AddFamineChallenge();
            AddAbundanceChallenge();
            AddHarderBossesChallenge();
            AddEnvironmentChallenge();
            AddTravelingOuroChallenge();
            AddHarderFinalBossChallenge();
            AddOldFecundChallenge();
            AddReverseScalesChallenge();
            AddGoldenSheepChallenge();
            AddInfiniteLivesChallenge();
            AddVineBoomChallenge();
            AddFleetingSquirrelsChallenge();
            AddUnfairHandChallenge();
            AddAscenderBaneChallenge();
            AddRedrawHandChallenge();
            AddChampionsChallenge();
            Log.LogInfo("End of challenges");
        }
        private static void AddWaterborneStarterChallenge()
        {
            Challenge_waterborneStarter = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Aquatic Starters",
                    "Cards in starting deck have the Waterborne sigil.",
                    10,
                    Tools.LoadTexture("ascensionicon_waterbornestarterdeck"),
                    Tools.LoadTexture("ascensionicon_activated_waterbornestarterdeck"),
                    2
                    );
        }
        private static void AddShockedStarterChallenge()
        {
            Challenge_shockedStarter = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Shocked Starters",
                    "Cards in starting deck attack every other turn.",
                    20,
                    Tools.LoadTexture("ascensionicon_paralysisstarterdeck"),
                    Tools.LoadTexture("ascensionicon_activated_paralysisstarterdeck"),
                    3
                    );
        }
        private static void AddTravelingOuroChallenge()
        {
            Challenge_travelingOuro = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Traveling Ouroboros",
                    "A traveling Ouroboros appears throughout the run.",
                    40,
                    Tools.LoadTexture("ascensionicon_travelingouro"),
                    Tools.LoadTexture("ascensionicon_activated_travelingouro"),
                    12
                    ).SetFlags("p03");
        }
        private static void AddMycoChallenge()
        {
            Challenge_mycoBotch = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Botched Experiments",
                    "The Mycologists have a chance to make mistakes while fusing cards.",
                    5,
                    Tools.LoadTexture("ascensionicon_myco"),
                    Tools.LoadTexture("ascensionicon_activated_myco"),
                    1
                    );
        }
        private static void AddFamineChallenge()
        {
            Challenge_famine = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Famine",
                    String.Format("The side deck has {0} less cards.", Math.Max(0, Math.Min(10, Plugin.famineRemoval.Value))),
                    15,
                    Tools.LoadTexture("ascensionicon_famine"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    5,
                    2
                    ).SetFlags("p03");
        }
        private static void AddAbundanceChallenge()
        {
            Challenge_abundance = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Abundance",
                    String.Format("The side deck has {0} more cards.", Math.Max(0, Plugin.abundanceQuality.Value)),
                    -15,
                    Tools.LoadTexture("ascensionicon_abundance"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    5,
                    2
                    ).SetFlags("p03")
                    .SetIncompatibleChallengeGetterStatic(Challenge_famine.challengeType);
            Challenge_famine.GetFullChallenge().SetIncompatibleChallengeGetterStatic(Challenge_abundance.challengeType);
        }
        private static void AddWeakStartersChallenge()
        {
            Challenge_weakStarters = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Weak Starters",
                    "Cards in the starting deck have 1 less health.",
                    5,
                    Tools.LoadTexture("ascensionicon_weakstarters"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    2
                    );
        }
        private static void AddSprinterChallenge()
        {
            Challenge_sprinter = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Sprintmaggedon",
                    "All cards get a random Sprinter sigil when drawn.",
                    20,
                    Tools.LoadTexture("ascensionicon_sprintmageddon"),
                    ChallengeManager.HAPPY_ACTIVATED_SPRITE,
                    6
                    ).SetFlags("p03");
        }
        private static void AddOldFecundChallenge()
        {
            Challenge_oldFecund = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Old Fecundity",
                    "Reverts the Fecundity Nerf.",
                    -20,
                    Tools.LoadTexture("ascensionicon_oldfecund"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    6
                    ).SetFlags("p03");
        }
        private static void AddGoldenSheepChallenge()
        {
            Challenge_goldenSheep = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "The Golden Fleece",
                    "A Golden Ram appears randomly throughout the run.",
                    -20,
                    Tools.LoadTexture("ascensionicon_goldensheep"),
                    Tools.LoadTexture("ascensionicon_activated_goldensheep"),
                    7
                    );
        }
        private static void AddHarderFinalBossChallenge()
        {
            Challenge_harderFinalBoss = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "True Pirate",
                    "Pirates invade Bosses. The Final Boss Challenge is harder.",
                    35,
                    Tools.LoadTexture("ascensionicon_harderfinalboss"),
                    Tools.LoadTexture("ascensionicon_activated_harderfinalboss"),
                    13
                    );
        }
        private static void AddWeakSoulStarterChallenge()
        {
            Challenge_weakSoulStarter = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Unspirited Starters",
                    "Cards in the starting deck may not have their sigils transferred.",
                    25,
                    Tools.LoadTexture("ascensionicon_weaksoul"),
                    Tools.LoadTexture("ascensionicon_activated_weaksoul"),
                    3
                    );
        }
        private static void AddHarderBossesChallenge()
        {
            Challenge_harderBosses = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Harder Bosses",
                    "Bosses' main cards are more powerful, and bosses are more agressive.",
                    25,
                    Tools.LoadTexture("ascensionicon_hardersignatures"),
                    Tools.LoadTexture("ascensionicon_activated_hardersignatures"),
                    8
                    );
        }
        private static void AddInfiniteLivesChallenge()
        {
            Challenge_infiniteLives = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Extra Lives",
                    String.Format("The scales will reset once they hit 0, up to {0} times for each candle.", Plugin.allowedResets.Value),
                    -Plugin.allowedResets.Value * 75,
                    Tools.LoadTexture("ascensionicon_infinitelives"),
                    Tools.LoadTexture("ascensionicon_activated_infinitelives"),
                    12
                    );
        }
        private static void AddReverseScalesChallenge()
        {
            Challenge_reverseScales = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Reverse Scales",
                    "Start all battles with 1 damage on the opponent's side of the scale.",
                    -30,
                    Tools.LoadTexture("ascensionicon_reversescales"),
                    ChallengeManager.HAPPY_ACTIVATED_SPRITE,
                    8
                    ).SetFlags("p03")
                    .SetIncompatibleChallengeGetterStatic(AscensionChallenge.StartingDamage);
        }
        private static void AddEnvironmentChallenge()
        {
            Challenge_environment = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Environmental Effects",
                    "At the start of each battle, a random environmental effect may activate.",
                    25,
                    Tools.LoadTexture("ascensionicon_environment"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    9
                    ).SetFlags("p03");
        }
        private static void AddVineBoomChallenge()
        {
            Challenge_vineBoom = ChallengeManager.AddSpecific<VineBoomDeath>(
                    PluginGuid,
                    "Explosive Noise",
                    "When a card dies, a loud boom will play. All cards explode on death.",
                    0,
                    Tools.LoadTexture("ascensionicon_nuclear"),
                    Tools.LoadTexture("activated_nuclear")
                    ).SetFlags("p03");
        }
        private static void AddFleetingSquirrelsChallenge()
        {
            Challenge_fleetingSquirrels = ChallengeManager.AddSpecific<FleetingSquirrels>(
                    PluginGuid,
                    "Runaway Side Deck",
                    "Your Side Deck cards have the Fleeting sigil.",
                    10,
                    Tools.LoadTexture("ascensionicon_fleetingsquirrels"),
                    Tools.LoadTexture("activated_fleetingsquirrels"),
                    1
                    ).SetFlags("p03");
        }
        private static void AddUnfairHandChallenge()
        {
            Challenge_unfairHand = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Unfair Hand",
                    "Your starting hand is randomly drawn.",
                    50,
                    Tools.LoadTexture("ascensionicon_unfairhand"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    4
                    ).SetFlags("p03");
        }
        private static void AddAscenderBaneChallenge()
        {
            Challenge_ascenderBane = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Ascender's Bane",
                    "You start with a useless card in the deck.",
                    20,
                    Tools.LoadTexture("ascensionicon_ascenderbane"),
                    Tools.LoadTexture("ascensionicon_activated_ascenderbane"),
                    4
                    );
        }
        private static void AddRedrawHandChallenge()
        {
            Challenge_redrawHand = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Mulligan",
                    "You may redraw your hand at the start of each battle.",
                    -20,
                    Tools.LoadTexture("ascensionicon_redrawhand"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    0
                    ).SetFlags("p03");
        }

        private static void AddChampionsChallenge()
        {
            Challenge_champion = ChallengeManager.AddSpecific(
                    PluginGuid,
                    "Champions",
                    "Opponent cards have a chance to become a champion.",
                    20,
                    Tools.LoadTexture("ascensionicon_champion"),
                    ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                    6
                    );
        }
    }
}
