using BittysSigils;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Ascension;
using InscryptionAPI.Boons;
using InscryptionAPI.Card;
using InscryptionAPI.Saves;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static BittysChallenges.Abilities;
using static BittysChallenges.Boons;
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
        #region Challenge Loaders
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
        #endregion
        #region Challenge Patches
        [HarmonyPatch]
        public class RandomPiratesPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.SetupPhase))]
            public static void ChallengeActivations()
            {
                if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType) && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Default && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Totem)
                {
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_harderFinalBoss.challengeType);
                }
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Opponent), nameof(Opponent.SpawnOpponent))]
            public static void AddToEncounter(ref Opponent __result)
            {
                if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType) && __result.OpponentType != Opponent.Type.Default && __result.OpponentType != Opponent.Type.Totem)
                {
                    List<List<CardInfo>> tp = __result.TurnPlan;
                    int lanes = Singleton<BoardManager>.Instance.PlayerSlotsCopy.Count;
                    if (tp.Count > 0)
                    {
                        CardInfo skeleton = CardLoader.GetCardByName("SkeletonPirate");
                        int idealTurn;

                        if (tp[0].Count < 2)
                        {
                            idealTurn = 0;
                        }
                        else
                        {
                            idealTurn = 1;
                        }
                        Plugin.Log.LogInfo(idealTurn);
                        if (tp[idealTurn].Count < lanes)
                        {
                            Plugin.Log.LogInfo("Adding Skeleton to turn plan...");
                            tp[idealTurn].Add(skeleton);
                        }

                        idealTurn++;
                        if (tp[idealTurn].Count < lanes)
                        {
                            Plugin.Log.LogInfo("Adding Undead Parrot to turn plan...");
                            tp[idealTurn].Add(CardLoader.GetCardByName("SkeletonParrot"));
                        }

                        //If it's the angler boss, give him a mole seaman >:)
                        if (__result.OpponentType == Opponent.Type.AnglerBoss || __result.OpponentType == Opponent.Type.PirateSkullBoss)
                        {
                            if (tp[0].Count < lanes)
                            {
                                idealTurn = 0;
                            }
                            else
                            {
                                idealTurn = 1;
                            }
                            if (tp[idealTurn].Count < lanes)
                            {
                                tp[idealTurn].Add(CardLoader.GetCardByName("MoleSeaman"));
                                Plugin.Log.LogInfo("Adding Mole Seaman to turn plan...");
                            }
                        }
                    }
                }
            }
            [HarmonyPrefix]
            [HarmonyPatch(typeof(GiantShip), nameof(GiantShip.MutinySequence))]
            public static bool MutinyChangePre()
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType))
                {
                    return false;
                }
                return true;
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(GiantShip), nameof(GiantShip.MutinySequence))]
            public static IEnumerator MutinyChangePost(IEnumerator values)
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType))
                {
                    int numSkeles = (Singleton<GiantShip>.Instance.nextHealthThreshold - Singleton<GiantShip>.Instance.PlayableCard.Health) / 5 + 1;
                    int num;
                    for (int i = 0; i < Math.Min(numSkeles, 3); i = num + 1)
                    {

                        List<CardSlot> validSlots = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
                        validSlots.RemoveAll((CardSlot x) => x.Card != null);
                        if (validSlots.Count > 0)
                        {
                            Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, true);
                            yield return new WaitForSeconds(0.5f);
                            Singleton<CardRenderCamera>.Instance.GetLiveRenderCamera(Singleton<GiantShip>.Instance.Card.StatsLayer as RenderLiveStatsLayer).GetComponentInChildren<PirateShipAnimatedPortrait>().NextSkeletonJumpOverboard();
                            yield return new WaitForSeconds(1f);
                            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                            CardSlot slot = validSlots[UnityEngine.Random.Range(0, validSlots.Count)];
                            yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_WoodenBoard"), slot, 0.1f, true);
                            yield return new WaitForSeconds(0.2f);
                            Singleton<GiantShip>.Instance.skelesSpawned++;
                        }
                        validSlots = null;
                        num = i;
                    }
                    if (Singleton<GiantShip>.Instance.mutineesSinceDialogue > 1)
                    {
                        yield return new WaitForSeconds(0.3f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("PirateSkullShipMutinee", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        Singleton<GiantShip>.Instance.mutineesSinceDialogue = 0;
                    }
                    Singleton<GiantShip>.Instance.mutineesSinceDialogue++;
                }
                yield break;
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(PirateSkullBossOpponent), nameof(PirateSkullBossOpponent.StartPhase2))]
            public static IEnumerator RoyalPhase2(IEnumerator values)
            {
                yield return values;
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType))
                {
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_harderFinalBoss.challengeType);
                    List<CardSlot> opponentSlots = Singleton<BoardManager>.Instance.OpponentSlotsCopy;

                    if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_travelingOuro.challengeType))
                    {
                        Plugin.Log.LogInfo("Creating an Ouroboros...");
                        CardInfo ouro = CardLoader.GetCardByName("bitty_TravelingOuroboros");
                        CardModificationInfo mod = new CardModificationInfo();
                        mod.fromCardMerge = true;
                        mod.abilities.Add(Ability.WhackAMole);
                        for (int i = 1; i <= AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false); i++)
                        {
                            Plugin.Log.LogInfo(i);
                            mod.abilities.Add(MiscEncounters.ValidAbilities(i).ability);
                        }
                        mod.attackAdjustment = MiscEncounters.TravelingOuroborosBuffs();
                        mod.healthAdjustment = MiscEncounters.TravelingOuroborosBuffs();

                        if (MiscEncounters.TravelingOuroborosBuffs() < AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false))
                        {
                            mod.attackAdjustment = AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false);
                            mod.healthAdjustment = AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false);
                        }
                        ouro.mods.Add(mod);

                        List<CardSlot> opponentSlotsCopy1 = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                        opponentSlotsCopy1.RemoveAll((CardSlot x) => Singleton<Opponent>.Instance.queuedCards.Find((PlayableCard y) => y.QueuedSlot == x));
                        if (opponentSlotsCopy1.Count >= 1)
                        {
                            yield return Singleton<Opponent>.Instance.QueueCard(ouro, opponentSlotsCopy1[0], true, true, true);

                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("RoyalOuro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                            Plugin.Log.LogInfo("Ouro Sequence Finished");
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.4f);
                        CardInfo mole = CardLoader.GetCardByName("MoleSeaman");
                        CardModificationInfo mod = new CardModificationInfo();
                        mod.attackAdjustment = 1;
                        mod.healthAdjustment = 4;
                        mod.abilities.Add(Ability.BuffNeighbours);
                        mod.nameReplacement = "Mole Firstmate";
                        mole.AddAppearances(GoldEmission.Appearance.GoldEmission);
                        mole.mods.Add(mod);

                        List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                        opponentSlotsCopy.RemoveAll((CardSlot x) => Singleton<Opponent>.Instance.queuedCards.Find((PlayableCard y) => y.QueuedSlot == x));
                        if (opponentSlotsCopy.Count >= 1)
                        {
                            yield return Singleton<Opponent>.Instance.QueueCard(mole, opponentSlotsCopy[0], true, true, true);

                            Plugin.Log.LogInfo("Playing animation");
                            View oldView = Singleton<ViewManager>.Instance.CurrentView;
                            Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);

                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("RoyalFirstMate", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                            yield return new WaitForSeconds(0.5f);
                            Singleton<ViewManager>.Instance.SwitchToView(oldView, false, false);
                        }
                    }
                    Plugin.Log.LogInfo("Phase 2 additions complete");
                }
                yield break;
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Part1Opponent), nameof(Part1Opponent.TryModifyCardWithTotem))]
            public static void RoyalTotem(PlayableCard card)
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType))
                {
                    if (Singleton<Part1Opponent>.Instance.totem != null && Singleton<Part1Opponent>.Instance.OpponentType == Opponent.Type.PirateSkullBoss)
                    {
                        card.StatsLayer.SetEmissionColor(Singleton<Part1Opponent>.Instance.InteractablesGlowColor);
                        if (!card.TemporaryMods.Exists((CardModificationInfo x) => x.fromTotem) && !card.Info.HasTrait(Trait.Giant))
                        {
                            card.AddTemporaryMod(new CardModificationInfo
                            {
                                abilities =
                                    {
                                        Singleton<Part1Opponent>.Instance.totem.TotemItemData.bottom.effectParams.ability
                                    },
                                fromTotem = true
                            });
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RunIntroSequencer))]
        public class StarterChallengesPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(RunIntroSequencer.TryModifyStarterCards))]
            public static void StartersPatch()
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_waterborneStarter.challengeType))
                {
                    foreach (CardInfo cardInfo in RunState.Run.playerDeck.Cards)
                    {
                        if (!cardInfo.HasTrait(Trait.Pelt))
                        {
                            Plugin.Log.LogInfo("Waterborne Check");
                            CardModificationInfo mod = new CardModificationInfo(Ability.Submerge);
                            if (!cardInfo.HasAbility(Ability.Submerge))
                            {
                                RunState.Run.playerDeck.ModifyCard(cardInfo, mod);
                            }
                        }
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_shockedStarter.challengeType))
                {
                    foreach (CardInfo cardInfo in RunState.Run.playerDeck.Cards)
                    {
                        if (!cardInfo.HasTrait(Trait.Pelt))
                        {
                            Plugin.Log.LogInfo("Paralysis Check");
                            CardModificationInfo mod = new CardModificationInfo(GiveParalysis.ability);
                            if (!cardInfo.HasAbility(GiveParalysis.ability))
                            {
                                RunState.Run.playerDeck.ModifyCard(cardInfo, mod);
                            }
                        }
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_weakStarters.challengeType))
                {
                    foreach (CardInfo cardInfo in RunState.Run.playerDeck.Cards)
                    {
                        if (!cardInfo.HasTrait(Trait.Pelt))
                        {
                            Plugin.Log.LogInfo("Weak Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.healthAdjustment = -1;
                            if (cardInfo.Health > 1)
                            {
                                mod.nameReplacement = "Weak " + cardInfo.displayedName;
                                RunState.Run.playerDeck.ModifyCard(cardInfo, mod);
                            }
                        }
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_weakSoulStarter.challengeType))
                {
                    foreach (CardInfo cardInfo in RunState.Run.playerDeck.Cards)
                    {
                        if (!cardInfo.HasTrait(Trait.Pelt))
                        {
                            Plugin.Log.LogInfo("Weak Soul Check");
                            CardModificationInfo mod = new CardModificationInfo(Sigils.GiveNoTransfer.ability);
                            if (!cardInfo.HasAbility(Sigils.GiveNoTransfer.ability))
                            {
                                RunState.Run.playerDeck.ModifyCard(cardInfo, mod);
                            }
                        }
                    }
                }

                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_ascenderBane.challengeType))
                {
                    Plugin.Log.LogInfo("Ascender's Bane Check");
                    RunState.Run.playerDeck.AddCard(CardLoader.GetCardByName(CardPrefix + "_" + "Ascender's Bane"));
                }
            }
            [HarmonyPostfix]
            [HarmonyPatch(nameof(RunIntroSequencer.RunIntroSequence))]
            public static IEnumerator StartersAnnouncer(IEnumerator values)
            {
                yield return values;
                bool dialoguePlayed = false;
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_waterborneStarter.challengeType))
                {
                    yield return new WaitForSeconds(0.5f);
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_waterborneStarter.challengeType);
                    if (!dialoguePlayed && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("WaterborneStart"))
                    {
                        dialoguePlayed = true;
                        yield return new WaitForSeconds(0.5f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("WaterborneStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_shockedStarter.challengeType))
                {
                    yield return new WaitForSeconds(0.5f);
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_shockedStarter.challengeType);
                    if (!dialoguePlayed && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("ShockedStart"))
                    {
                        dialoguePlayed = true;
                        yield return new WaitForSeconds(0.5f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("ShockedStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_weakStarters.challengeType))
                {
                    yield return new WaitForSeconds(0.5f);
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_weakStarters.challengeType);
                    bool weakenedCards = false;
                    foreach (CardInfo cardInfo in RunState.Run.playerDeck.Cards)
                    {
                        if (!cardInfo.HasTrait(Trait.Pelt) && cardInfo.Health != cardInfo.baseHealth)
                        {
                            weakenedCards = true;
                        }
                    }
                    if (!dialoguePlayed && !Plugin.IsP03Run && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("WeakStart") && weakenedCards)
                    {
                        dialoguePlayed = true;
                        yield return new WaitForSeconds(0.5f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("WeakStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                    else if (!dialoguePlayed && Plugin.IsP03Run && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("P03WeakStart") && weakenedCards)
                    {
                        dialoguePlayed = true;
                        yield return new WaitForSeconds(0.5f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03WeakStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_weakSoulStarter.challengeType))
                {
                    yield return new WaitForSeconds(0.5f);
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_weakSoulStarter.challengeType);
                    if (!dialoguePlayed && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("WeakSoulStart"))
                    {
                        dialoguePlayed = true;
                        yield return new WaitForSeconds(0.5f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("WeakSoulStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_ascenderBane.challengeType))
                {
                    yield return new WaitForSeconds(0.5f);
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_ascenderBane.challengeType);
                    if (!dialoguePlayed && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("AscenderBaneStart"))
                    {
                        dialoguePlayed = true;
                        yield return new WaitForSeconds(0.5f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("AscenderBaneStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(EncounterBuilder))]
        public class EncounterAddPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(EncounterBuilder.Build))]
            public static void AddToEncounter(ref EncounterData __result, CardBattleNodeData nodeData)
            {

                List<List<CardInfo>> tp = __result.opponentTurnPlan;


                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_travelingOuro.challengeType))
                {
                    Plugin.Log.LogInfo("Checking to see if we should add Ouroboros...");
                    if (MiscEncounters.RollForOuro(nodeData))
                    {
                        Plugin.Log.LogInfo("Creating an Ouroboros...");
                        CardInfo ouro = CardLoader.GetCardByName("bitty_TravelingOuroboros");
                        CardModificationInfo mod = new CardModificationInfo();
                        if (IsP03Run)
                        {
                            ouro.portraitTex = Tools.LoadSprite("portrait_ourobot.png");
                            mod.nameReplacement = "Ourobot";
                        }
                        mod.abilities.Add(Ability.GuardDog);
                        if (!IsP03Run)
                        {
                            mod.fromCardMerge = true;
                            for (int i = 1; i <= AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false); i++)
                            {
                                Plugin.Log.LogInfo(i);
                                mod.abilities.Add(MiscEncounters.ValidAbilities(i).ability);
                            }
                        }
                        mod.attackAdjustment = MiscEncounters.TravelingOuroborosBuffs();
                        mod.healthAdjustment = MiscEncounters.TravelingOuroborosBuffs();

                        if (!IsP03Run && MiscEncounters.TravelingOuroborosBuffs() < AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false))
                        {
                            mod.attackAdjustment = AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false);
                            mod.healthAdjustment = AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false);
                        }
                        ouro.mods.Add(mod);

                        Plugin.Log.LogInfo("Finding Ouro placement...");

                        int idealTurn;
                        if (IsP03Run && tp[3].Count <= 1)
                        {
                            idealTurn = 3;
                        }
                        else if (IsP03Run)
                        {
                            idealTurn = 4;
                        }
                        else if (tp[0].Count <= 1)
                        {
                            idealTurn = 0;
                        }
                        else
                        {
                            idealTurn = 1;
                        }

                        if (tp[idealTurn].Count < Singleton<BoardManager>.Instance.PlayerSlotsCopy.Count)
                        {
                            Plugin.Log.LogInfo("Adding Ouro to turn plan...");
                            tp[idealTurn].Add(ouro);

                            Plugin.Log.LogInfo(string.Format("Added Ouroboros in turn {0}", idealTurn));
                        }
                    }
                    else
                    {
                        Plugin.Log.LogInfo("Failed Ouro Roll...");
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_goldenSheep.challengeType))
                {
                    Plugin.Log.LogInfo("Golden Ram Challenge on...");
                    Plugin.Log.LogInfo("Checking to see if we should add Golden Ram...");
                    int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                    bool flag = 0 == SeededRandom.Range(0, 3, currentRandomSeed);
                    flag = true;
                    Plugin.Log.LogInfo("Sheep Roll: " + flag);
                    if (flag && MiscEncounters.TimesGoldenSheepKilled() < AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false))
                    {
                        Plugin.Log.LogInfo("Creating a Golden Ram...");
                        CardInfo goldSheep = CardLoader.GetCardByName("bitty_GoldenSheep");

                        CardModificationInfo mod = new CardModificationInfo();
                        mod.healthAdjustment += AscensionStatsData.GetStatValue(AscensionStat.Type.BossesDefeated, false);

                        goldSheep.mods.Add(mod);

                        Plugin.Log.LogInfo("Finding Golden Ram placement...");

                        int idealTurn;
                        if (tp[0].Count <= 1)
                        {
                            idealTurn = 0;
                        }
                        else
                        {
                            idealTurn = 1;
                        }
                        if (tp[idealTurn].Count < Singleton<BoardManager>.Instance.PlayerSlotsCopy.Count)
                        {
                            Plugin.Log.LogInfo("Adding Gold Ram to turn plan...");
                            tp[idealTurn].Add(goldSheep);

                            Plugin.Log.LogInfo("added gold ram to " + idealTurn);
                        }
                    }
                    else
                    {
                        Plugin.Log.LogInfo("Failed Roll...");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(DuplicateMergeSequencer))]
        public class BotchedPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(DuplicateMergeSequencer.MergeCards))]
            public static CardInfo MycoPatch(CardInfo card1)
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_mycoBotch.challengeType))
                {
                    bool mistake = false;
                    CardModificationInfo cardModificationInfo = new CardModificationInfo();
                    int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                    float num = SeededRandom.Value(currentRandomSeed++);
                    if (num < 0.33f)
                    {
                        if (card1.Mods.Exists((CardModificationInfo x) => x.abilities.Count > 0))
                        {
                            List<CardModificationInfo> list = card1.Mods.FindAll((CardModificationInfo x) => x.abilities.Count > 0);

                            list[SeededRandom.Range(0, list.Count, currentRandomSeed++)].abilities[0] = ValidMycoAbilities(1).ability;
                            mistake = true;
                            Singleton<TextDisplayer>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MycoFailSigils", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null));
                            goto IL_1F7;
                        }
                    }
                    if (num < 0.66f && card1.Attack > 0)
                    {
                        int num2 = card1.Attack / 2;
                        cardModificationInfo.attackAdjustment = (-num2);
                        mistake = true;
                        Singleton<TextDisplayer>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MycoFailAttack", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null));
                    }
                    else if (card1.Health > 1)
                    {
                        int num2 = card1.Health / 2;
                        cardModificationInfo.healthAdjustment = (-num2);
                        mistake = true;
                        Singleton<TextDisplayer>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MycoFailHealth", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null));

                    }
                IL_1F7:
                    RunState.Run.playerDeck.ModifyCard(card1, cardModificationInfo);
                    if (mistake)
                    {
                        ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_mycoBotch.challengeType);
                        mistake = false;
                    }
                }
                return card1;
            }
            public static AbilityInfo ValidMycoAbilities(int plus)
            {
                List<AbilityInfo> validAbilities = ScriptableObjectLoader<AbilityInfo>.AllData.FindAll((AbilityInfo x) =>
                x.ability == Ability.StrafeSwap ||
                x.ability == Ability.StrafePush ||
                x.ability == Sigils.GiveStrafePull.ability ||
                x.ability == Sigils.GiveStrafeSticky.ability ||
                x.ability == Ability.GainAttackOnKill ||
                x.ability == Ability.WhackAMole ||
                x.ability == Ability.QuadrupleBones ||
                x.ability == Ability.Submerge ||
                x.ability == Ability.GuardDog ||
                x.ability == Ability.Reach ||
                x.ability == Ability.Flying ||
                x.ability == Ability.RandomAbility ||
                x.ability == GiveFragile.ability ||
                x.ability == Ability.MadeOfStone ||
                x.ability == Ability.BuffEnemy ||
                x.ability == Ability.OpponentBones ||
                x.ability == Ability.BoneDigger ||
                x.ability == Ability.Brittle);

                int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                AbilityInfo abilityInfo = validAbilities[SeededRandom.Range(0, validAbilities.Count, currentRandomSeed + plus)];
                Plugin.Log.LogInfo(abilityInfo.ability);
                return abilityInfo;
            }
        }

        [HarmonyPatch(typeof(CardDrawPiles3D))]
        public class SideDeckPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(CardDrawPiles3D.InitializePiles))]
            public static void AbundanceFaminePatch()
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_famine.challengeType) || AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_abundance.challengeType))
                {
                    int faminesActive = 0;
                    int abundancesActive = 0;
                    for (int i = 0; i < AscensionSaveData.Data.GetNumChallengesOfTypeActive(Challenges.Challenge_famine.challengeType); i++)
                    {
                        faminesActive++;
                    }
                    for (int i = 0; i < AscensionSaveData.Data.GetNumChallengesOfTypeActive(Challenges.Challenge_abundance.challengeType); i++)
                    {
                        abundancesActive++;
                    }

                    Plugin.Log.LogInfo("Famine Severity: " + faminesActive * Plugin.famineRemoval.Value);
                    Plugin.Log.LogInfo("Abundance Quality: " + abundancesActive * Plugin.abundanceQuality.Value);

                    ChallengeActivationUI.TryShowActivation(Challenges.Challenge_abundance.challengeType);
                    CardInfo info = Singleton<CardDrawPiles3D>.Instance.SideDeck.cards.Count > 0 ? Singleton<CardDrawPiles3D>.Instance.SideDeck.cards[0] : CardLoader.GetCardByName("Bee");
                    Plugin.Log.LogInfo(info.displayedName);
                    for (int i = 0; i < (abundancesActive * Plugin.abundanceQuality.Value); i++)
                    {
                        Singleton<CardDrawPiles3D>.Instance.sidePile.CreateCards(1);
                        Singleton<CardDrawPiles3D>.Instance.SideDeck.AddCard(info);
                    }

                    ChallengeActivationUI.TryShowActivation(Challenges.Challenge_famine.challengeType);
                    for (int i = 0; i < Math.Min(10, ((faminesActive * Plugin.famineRemoval.Value))); i++)
                    {
                        CardDrawPiles3D.Instance.SidePile.Draw();
                        CardDrawPiles3D.Instance.SideDeck.Draw();
                    }

                    Plugin.Log.LogInfo("Total cards in side deck: " + CardDrawPiles3D.Instance.SideDeck.CardsInDeck);
                    if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_famine.challengeType) && !DialogueEventsData.EventIsPlayed("P03FamineIntro") && IsP03Run)
                    {
                        Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03FamineIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
                        {
                            info.displayedName
                        }, new Action<DialogueEvent.Line>(Dialogue.P03HappyCloseUp)));
                    }
                    else if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_famine.challengeType) && !DialogueEventsData.EventIsPlayed("FamineIntro"))
                    {
                        Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FamineIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
                        {
                            info.displayedName
                        }, null));
                    }
                    else if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_abundance.challengeType) && !DialogueEventsData.EventIsPlayed("P03AbundanceIntro") && IsP03Run)
                    {
                        Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03AbundanceIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
                        {
                            (abundancesActive * Plugin.abundanceQuality.Value).ToString()
                        }, new Action<DialogueEvent.Line>(Dialogue.P03HappyCloseUp)));
                    }
                    else if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_abundance.challengeType) && !DialogueEventsData.EventIsPlayed("AbundanceIntro"))
                    {
                        Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("AbundanceIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
                        {
                            info.displayedName
                        }, null));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerHand))]
        public class SprinterDraw
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(PlayerHand.AddCardToHand))]
            public static void SprinterHandPatch(ref PlayableCard card)
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_sprinter.challengeType))
                {
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_sprinter.challengeType);
                    CardModificationInfo mod = new CardModificationInfo();


                    if (!Plugin.IsP03Run) mod.fromCardMerge = true;
                    mod.abilities.Add(GetRandomStrafe().ability);

                    if (!Plugin.IsP03Run || card.AllAbilities().Count() <= 4)
                    {
                        card.AddTemporaryMod(mod);
                    }

                    if (!Plugin.IsP03Run && !CardDisplayer3D.EmissionEnabledForCard(card.renderInfo, card))
                    {
                        card.RenderInfo.forceEmissivePortrait = true;
                        card.StatsLayer.SetEmissionColor(GameColors.Instance.lightPurple);
                    }
                    card.RenderCard();
                }
            }
            public static AbilityInfo GetRandomStrafe()
            {
                List<AbilityInfo> validAbilities = ScriptableObjectLoader<AbilityInfo>.AllData.FindAll((AbilityInfo x) =>
            x.ability == Ability.StrafeSwap ||
            x.ability == Ability.StrafePush ||
            x.ability == Ability.Strafe ||
            x.ability == Ability.MoveBeside ||
            x.ability == Sigils.GiveStrafeSticky.ability ||
            x.ability == GiveWarper.ability ||
            x.ability == Sigils.GiveStrafePull.ability ||
            x.ability == Sigils.GiveStrafeSuper.ability);

                AbilityInfo abilityInfo = validAbilities[UnityEngine.Random.Range(0, validAbilities.Count)];
                return abilityInfo;
            }
        }

        [HarmonyPatch(typeof(DrawCopy))]
        public class NoFecundityNerf
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(DrawCopy.CardToDrawTempMods), MethodType.Getter)]
            private static void Postfix(ref List<CardModificationInfo> __result)
            {
                if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_oldFecund.challengeType))
                {
                    __result = null;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(DrawCopy.OnResolveOnBoard))]
            public static IEnumerator UnNerfDialogue(IEnumerator values)
            {
                yield return values;
                if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_oldFecund.challengeType) && (!DialogueEventsData.EventIsPlayed("P03FecundityUnNerfIntro") || !DialogueEventsData.EventIsPlayed("FecundityUnNerfIntro")))
                {
                    Singleton<ChallengeActivationUI>.Instance.ShowTextLines(new string[]
                    {
                    Localization.Translate("DEPLOY SIGIL UNNERF: FECUNDITY"),
                    Localization.Translate("AddSigilToCopy()"),
                    Localization.Translate("// It was asked for.")
                    });
                    yield return new WaitForSeconds(0.5f);
                    if (IsP03Run && !DialogueEventsData.EventIsPlayed("P03FecundityUnNerfIntro"))
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03FecundityUnNerfIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null,
                            new Action<DialogueEvent.Line>(Dialogue.P03HappyCloseUp));
                    }
                    else if (!IsP03Run && !DialogueEventsData.EventIsPlayed("FecundityUnNerfIntro"))
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FecundityUnNerfIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
            }
        }

        [HarmonyPatch]
        public class HarderBosses
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.SetupPhase))]
            public static void ChallengeActivations()
            {
                if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderBosses.challengeType) && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Default && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Totem)
                {
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_harderBosses.challengeType);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TradeCardsForPelts), nameof(TradeCardsForPelts.TradePhase))]
            public static IEnumerator PostTradePlayQueueCards(IEnumerator values)
            {
                yield return values;
                if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderBosses.challengeType))
                {
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_harderBosses.challengeType);
                    yield return Singleton<TurnManager>.Instance.opponent.PlayCardsInQueue();
                }
                yield break;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Part1Opponent), nameof(Part1Opponent.TryModifyCardWithTotem))]
            public static void AddToSignatures(PlayableCard card)
            {
                var OpponentType = Singleton<Part1Opponent>.Instance.OpponentType;
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderBosses.challengeType) && OpponentType != Opponent.Type.Default && OpponentType != Opponent.Type.Totem)
                {
                    if (OpponentType == Opponent.Type.ProspectorBoss)
                    {
                        if (card.Info.name == "Mule")
                        {
                            Plugin.Log.LogInfo("Mule Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.Sentry);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "Bloodhound")
                        {
                            Plugin.Log.LogInfo("Bloodhound Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.Deathtouch);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "Coyote")
                        {
                            Plugin.Log.LogInfo("Coyote Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.Deathtouch);
                            ApplyMods(mod, card);
                        }
                    }
                    else if (OpponentType == Opponent.Type.AnglerBoss)
                    {
                        if (card.Info.name == "Kingfisher")
                        {
                            Plugin.Log.LogInfo("Kingfisher Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.BuffNeighbours);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "BaitBucket")
                        {
                            Plugin.Log.LogInfo("BaitBucket Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.Reach);
                            ApplyMods(mod, card);
                        }
                    }
                    else if (OpponentType == Opponent.Type.TrapperTraderBoss)
                    {
                        if (card.Info.name == "Bullfrog")
                        {
                            Plugin.Log.LogInfo("Bullfrog Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.SteelTrap);
                            ApplyMods(mod, card);

                        }
                        if (card.Info.name == "Rabbit")
                        {
                            Plugin.Log.LogInfo("Rabbit Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.WhackAMole);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "TrapFrog")
                        {
                            Plugin.Log.LogInfo("TrapFrog Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.GuardDog);
                            ApplyMods(mod, card);
                        }
                    }
                    else if (OpponentType == Opponent.Type.LeshyBoss)
                    {
                        if (card.Info.name == "Amalgam")
                        {
                            Plugin.Log.LogInfo("Amalgam Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.Sharp);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "MantisGod")
                        {
                            Plugin.Log.LogInfo("MantisGod Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.SplitStrike);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "Mantis")
                        {
                            Plugin.Log.LogInfo("Mantis Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.DoubleStrike);
                            ApplyMods(mod, card);
                        }
                    }
                    else if (OpponentType == Opponent.Type.PirateSkullBoss)
                    {
                        if (card.Info.name == "MoleSeaman")
                        {
                            Plugin.Log.LogInfo("MoleSeaman Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.ConduitBuffAttack);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "SkeletonPirate")
                        {
                            Plugin.Log.LogInfo("SkeletonPirate Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.Deathtouch);
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "SkeletonParrot")
                        {
                            Plugin.Log.LogInfo("SkeletonParrot Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.IceCube);

                            card.Info.SetIceCube(CardLoader.GetCardByName("Maggots"));
                            ApplyMods(mod, card);
                        }
                        if (card.Info.name == "Maggots")
                        {
                            Plugin.Log.LogInfo("Maggots Check");
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.abilities.Add(Ability.Brittle);
                            ApplyMods(mod, card);
                        }
                    }
                }
            }
            public static void ApplyMods(CardModificationInfo mod, PlayableCard card)
            {
                Plugin.Log.LogInfo("Adding Mods...");
                mod.fromCardMerge = true;
                card.AddTemporaryMod(mod);
                if (!CardDisplayer3D.EmissionEnabledForCard(card.renderInfo, card))
                {
                    card.RenderInfo.forceEmissivePortrait = true;
                    card.StatsLayer.SetEmissionColor(GameColors.Instance.nearWhite);
                }
                card.RenderCard();
            }
        }

        [HarmonyPatch(typeof(LifeManager))]
        public class InfiniteLives
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(LifeManager.ShowDamageSequence))]
            public static IEnumerator GiveLife(IEnumerator values)
            {
                yield return values;
                if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_infiniteLives.challengeType) && Singleton<LifeManager>.Instance.Balance <= -5)
                {
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_infiniteLives.challengeType);
                    LifeRepeatsIncrease();

                    yield return Singleton<LifeManager>.Instance.ShowResetSequence();
                    if (LifeRepeats() < Math.Max(0, Plugin.allowedResets.Value))
                    {
                        if (!DialogueEventsData.EventIsPlayed("InfiniteLivesIntro"))
                        {
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("InfiniteLivesIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        }
                        else if (Singleton<Opponent>.Instance.OpponentType == Opponent.Type.PirateSkullBoss)
                        {
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("InfiniteLivesRoyal", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        }
                        else
                        {
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("InfiniteLivesRepeat", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        }
                    }
                    else if (LifeRepeats() == Math.Max(0, Plugin.allowedResets.Value))
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("InfiniteLivesLoop", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                    else
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("InfiniteLivesLoopBreak", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        Singleton<LifeManager>.Instance.SetNumWeightsImmediate(5, 0);
                        LifeRepeatsReset();
                    }
                }
                yield break;
            }

            public static int LifeRepeats()
            {
                return ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.LifeRepeats");
            }
            public static void LifeRepeatsIncrease(int by = 1)
            {
                int num = ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.LifeRepeats") + by;
                Plugin.Log.LogInfo(string.Format("Increasing LifeResets by {0} to {1}", by, num));
                ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.LifeRepeats", num.ToString());
            }
            public static void LifeRepeatsReset(int num = 0)
            {
                Plugin.Log.LogInfo(string.Format("Setting LifeResets to ", num));
                ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.LifeRepeats", num.ToString());
            }
        }

        [HarmonyPatch(typeof(TurnManager))]
        public class ReverseScales
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(TurnManager.SetupPhase))]
            public static IEnumerator TipScale(IEnumerator values)
            {
                yield return values;
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_reverseScales.challengeType))
                {
                    ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_reverseScales.challengeType);
                    yield return Singleton<LifeManager>.Instance.ShowDamageSequence(1, 1, false, 0.125f, null, 0f, false);
                }
            }
        }

        [HarmonyPatch]
        public class Environments
        {
            public static int EnvironmentNumber()
            {
                Plugin.Log.LogInfo(ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.EnvironmentNumber"));
                return ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.EnvironmentNumber");
            }
            public static void ResetEnviroNumber(int value = 0)
            {
                Plugin.Log.LogInfo(string.Format("Resetting Environment Number to {0}", value));
                ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.EnvironmentNumber", value);
            }
            public static void IncreaseEnviroNumber(int by = 1)
            {
                int num = ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.EnvironmentNumber") + by;
                Plugin.Log.LogInfo(string.Format("Increasing Environment Number by {0} to {1}", by, num));
                ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.EnvironmentNumber", num.ToString());
                Plugin.Log.LogInfo(EnvironmentNumber());
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(BoonsHandler), nameof(BoonsHandler.BoonsEnabled), MethodType.Getter)]
            public static void OverrideBoonsEnabled(BoonsHandler __instance, ref bool __result)
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_environment.challengeType))
                {
                    __result = true;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.SetupPhase))]
            public static bool EnvironmentBoonGiver(ref IEnumerator __result)
            {
                var OpponentType = Singleton<Opponent>.Instance.OpponentType;
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_environment.challengeType) && (OpponentType == Opponent.Type.Default || OpponentType == Opponent.Type.Totem))
                {
                    ClearEnvironmentBoons();
                    IncreaseEnviroNumber();
                    List<BoonData.Type> boons = new List<BoonData.Type>();
                    if (!Plugin.IsP03Run)
                    {

                        Plugin.Log.LogInfo("Region Tier: " + RunState.CurrentRegionTier);
                        if (RunState.CurrentRegionTier >= 0)
                        {
                            boons.Add(ChallengeBoonCliffs.boo);
                            Plugin.Log.LogInfo("Added Cliffs to boons pool: " + ChallengeBoonCliffs.boo);

                            Plugin.Log.LogInfo("Region: " + RunState.CurrentMapRegion.name);
                            switch (RunState.CurrentMapRegion.name)
                            {
                                case "Forest":
                                    boons.Add(ChallengeBoonTotem.boo);
                                    Plugin.Log.LogInfo("Added Totem to boons pool: " + ChallengeBoonTotem.boo);
                                    break;
                                case "Wetlands":
                                    boons.Add(ChallengeBoonMud.boo);
                                    Plugin.Log.LogInfo("Added Mud to boons pool: " + ChallengeBoonMud.boo);
                                    break;
                                case "Alpine":
                                    boons.Add(ChallengeBoonHail.boo);
                                    Plugin.Log.LogInfo("Added Hail to boons pool: " + ChallengeBoonHail.boo);
                                    break;
                                case "Magma_bitty":
                                    break;
                            }
                        }
                        if (RunState.CurrentRegionTier >= 1)
                        {
                            boons.Add(ChallengeBoonBreeze.boo);
                            Plugin.Log.LogInfo("Added Breeze to boons pool: " + ChallengeBoonBreeze.boo);
                            boons.Add(ChallengeBoonFlashGrowth.boo);
                            Plugin.Log.LogInfo("Added Flash Growth to boons pool: " + ChallengeBoonFlashGrowth.boo);
                            boons.Add(ChallengeBoonGraveyard.boo);
                            Plugin.Log.LogInfo("Added Graveyard to boons pool: " + ChallengeBoonGraveyard.boo);

                            switch (RunState.CurrentMapRegion.name)
                            {
                                case "Forest":
                                    boons.Add(ChallengeBoonDynamite.boo);
                                    Plugin.Log.LogInfo("Added Prospector's Camp to boons pool: " + ChallengeBoonDynamite.boo);
                                    break;
                                case "Wetlands":
                                    boons.Add(ChallengeBoonBait.boo);
                                    Plugin.Log.LogInfo("Added Angler's Pool to boons pool: " + ChallengeBoonBait.boo);
                                    break;
                                case "Alpine":
                                    boons.Add(ChallengeBoonTrap.boo);
                                    Plugin.Log.LogInfo("Added Trapper's Hunting Grounds to boons pool: " + ChallengeBoonTrap.boo);
                                    break;
                            }
                        }
                        if (RunState.CurrentRegionTier >= 2)
                        {
                            boons.Add(ChallengeBoonObelisk.boo);
                            Plugin.Log.LogInfo("Added Obelisk to boons pool: " + ChallengeBoonObelisk.boo);

                            boons.Add(ChallengeBoonMushrooms.boo);
                            Plugin.Log.LogInfo("Added Mushrooms to boons pool: " + ChallengeBoonMushrooms.boo);

                            if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode))
                            {
                                boons.Add(ChallengeBoonBloodMoon.boo);
                                Plugin.Log.LogInfo("Added Blood Moon to boons pool: " + ChallengeBoonBloodMoon.boo);
                            }
                            if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode) && SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + 1) && !DialogueEventsData.EventIsPlayed("CarrotBoonIntro"))
                            {
                                boons.Add(ChallengeBoonCarrotPatch.boo);
                                Plugin.Log.LogInfo("Added Blood Moon(?) to boons pool: " + ChallengeBoonCarrotPatch.boo);
                            }
                            if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType) || AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.FinalBoss))
                            {
                                boons.Add(ChallengeBoonMinicello.boo);
                                Plugin.Log.LogInfo("Added Minicello to boons pool: " + ChallengeBoonMinicello.boo);
                            }

                            switch (RunState.CurrentMapRegion.name)
                            {
                                case "Forest":
                                    boons.Add(ChallengeBoonDarkForest.boo);
                                    Plugin.Log.LogInfo("Added Dark Forest to boons pool: " + ChallengeBoonDarkForest.boo);
                                    break;
                                case "Wetlands":
                                    boons.Add(ChallengeBoonFlood.boo);
                                    Plugin.Log.LogInfo("Added Flood to boons pool: " + ChallengeBoonFlood.boo);
                                    break;
                                case "Alpine":
                                    boons.Add(ChallengeBoonBlizzard.boo);
                                    Plugin.Log.LogInfo("Added Blizzard to boons pool: " + ChallengeBoonBlizzard.boo);
                                    break;
                            }
                        }
                    }
                    else //P03 boons
                    {
                        boons.Add(ChallengeBoonGraveyard.boo);
                        Plugin.Log.LogInfo("Added Graveyard to boons pool: " + ChallengeBoonGraveyard.boo);
                        boons.Add(ChallengeBoonFlashGrowth.boo);
                        Plugin.Log.LogInfo("Added Flash Growth to boons pool: " + ChallengeBoonFlashGrowth.boo);
                        boons.Add(ChallengeBoonConveyor.boo);
                        Plugin.Log.LogInfo("Added Conveyor to boons pool: " + ChallengeBoonConveyor.boo);
                        boons.Add(ChallengeBoonGemSanctuary.boo);
                        Plugin.Log.LogInfo("Added Gem Sanctuary to boons pool: " + ChallengeBoonGemSanctuary.boo);
                        boons.Add(ChallengeBoonElectricStorm.boo);
                        Plugin.Log.LogInfo("Added Electrical Storm to boons pool: " + ChallengeBoonElectricStorm.boo);
                    }
                    int i = EnvironmentNumber() % boons.Count;
                    bool boonActive = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + 2);
                    if (boonActive && boons != null)
                    {
                        RunState.Run.playerDeck.AddBoon(boons[i]);
                        Plugin.Log.LogInfo("Using boon: " + boons[i]);
                    }
                }

                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.CleanupPhase))]
            public static void EnvironmentBoonCleanup()
            {
                foreach (CardSlot slot in Singleton<BoardManager>.Instance.AllSlotsCopy)
                {
                    if (SaveManager.SaveFile.IsPart1)
                    {
                        slot.SetTexture(ResourceBank.Get<Texture>("Art/Cards/card_slot"));
                    }
                    if (SaveManager.SaveFile.IsPart3)
                    {
                        slot.SetTexture(ResourceBank.Get<Texture>("Art/Cards/card_slot_tech"));
                    }
                    if (SaveManager.SaveFile.IsGrimora)
                    {
                        slot.SetTexture(ResourceBank.Get<Texture>("Art/Cards/card_slot_undead"));
                    }
                    if (SaveManager.SaveFile.IsMagnificus)
                    {
                        slot.SetTexture(ResourceBank.Get<Texture>("Art/Cards/card_slot_wizard"));
                    }
                }
                Singleton<TableVisualEffectsManager>.Instance.ResetTableColors();
                ClearEnvironmentBoons();
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(BoardManager), nameof(BoardManager.SacrificesCreateRoomForCard))]
            public static void MergeSigilPatch(ref bool __result)
            {
                foreach (CardSlot slot in Singleton<BoardManager>.Instance.PlayerSlotsCopy)
                {
                    if (slot.Card != null)
                    {
                        CardModificationInfo cardModificationInfo = slot.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_mergeSigil");
                        if (cardModificationInfo != null)
                        {
                            __result = true;
                        }
                    }
                }
            }
            public static void ClearEnvironmentBoons()
            {
                if (RunState.Run.playerDeck.Boons.Count > 0)
                {
                    foreach (var boon in RunState.Run.playerDeck.Boons)
                    {
                        if (allEnvironmentBoons.Contains(boon.type))
                        {
                            RunState.Run.playerDeck.RemoveAllBoonsOfType(boon.type);
                        }
                    }
                    Plugin.Log.LogInfo("Resetting Environment Boons...");
                }
            }
            public static List<BoonData.Type> allEnvironmentBoons = new List<BoonData.Type>
            {
				//leshy
				ChallengeBoonCliffs.boo,
                ChallengeBoonFlashGrowth.boo,

                ChallengeBoonTotem.boo,
                ChallengeBoonMud.boo,
                ChallengeBoonHail.boo,

                ChallengeBoonBreeze.boo,
                ChallengeBoonGraveyard.boo,

                ChallengeBoonDynamite.boo,
                ChallengeBoonBait.boo,
                ChallengeBoonTrap.boo,

                ChallengeBoonObelisk.boo,
                ChallengeBoonMushrooms.boo,
                ChallengeBoonBloodMoon.boo,
                ChallengeBoonMinicello.boo,
                ChallengeBoonCarrotPatch.boo,

                ChallengeBoonDarkForest.boo,
                ChallengeBoonFlood.boo,
                ChallengeBoonBlizzard.boo,

				//p03
				ChallengeBoonConveyor.boo,
                ChallengeBoonGemSanctuary.boo,
                ChallengeBoonElectricStorm.boo
            };
        }

        public class VineBoomDeath : ChallengeBehaviour
        {
            public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
                return true;
            }
            public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
                AudioController.Instance.PlaySound2D("vine-boom", MixerGroup.None, 4f, CustomRandom.RandomBetween(0f, 0.2f), new AudioParams.Pitch(AudioParams.Pitch.Variation.Large), null, null, null, false);
                yield break;
            }
            public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
            {
                return true;
            }
            public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
            {
                otherCard.Status.hiddenAbilities.Add(Ability.ExplodeOnDeath);
                otherCard.AddTemporaryMod(new CardModificationInfo(Ability.ExplodeOnDeath));
                yield break;
            }
        }

        public class FleetingSquirrels : ChallengeBehaviour
        {
            public override bool RespondsToUpkeep(bool playerUpkeep)
            {
                return Singleton<CardDrawPiles3D>.Instance.SideDeck.CardsInDeck > 0;
            }
            public override IEnumerator OnUpkeep(bool playerUpkeep)
            {
                sideDeckName = Singleton<CardDrawPiles3D>.Instance.SideDeck.Cards[0].name;
                yield break;
            }
            public override bool RespondsToOtherCardDrawn(PlayableCard card)
            {
                return card.Info.name == sideDeckName || card.Info.name.ToLower().Contains("emptyvessel");
            }
            public override IEnumerator OnOtherCardDrawn(PlayableCard card)
            {
                card.AddTemporaryMod(new CardModificationInfo(Sigils.GiveFleeting.ability));
                yield break;
            }

            public string sideDeckName;
        }

        [HarmonyPatch(typeof(TurnManager))]
        public class RedrawHand
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(TurnManager.SetupPhase))]
            public static IEnumerator CloverGiver(IEnumerator __result)
            {
                yield return __result;
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_redrawHand.challengeType))
                {
                    ChallengeActivationUI.TryShowActivation(Challenges.Challenge_redrawHand.challengeType);
                    yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(CardLoader.GetCardByName(CardPrefix + "_" + "Clover"), null, 0.25f, null);
                    if (!DialogueEventsData.EventIsPlayed("RedrawHandIntro"))
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("RedrawHandIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
                yield break;
            }
        }

        [HarmonyPatch]
        public class UnfairHandPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(CardDrawPiles3D), "InitializePiles")]
            public static void UnfairHandShowActivation()
            {
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_unfairHand.challengeType))
                {
                    ChallengeActivationUI.TryShowActivation(Challenges.Challenge_unfairHand.challengeType);
                    if (!DialogueEventsData.EventIsPlayed("UnfairHandIntro"))
                    {
                        Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("UnfairHandIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null));
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Deck), "GetFairHand")]
            private static bool GetFairHand_nomore(Deck __instance, ref List<CardInfo> __result, int numCards = 4, List<CardInfo> existingHand = null)
            {
                if (!AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_unfairHand.challengeType))
                {
                    return true;
                }
                Plugin.Log.LogInfo("UNFAIR HAND GO!");
                List<CardInfo> list = new List<CardInfo>();
                List<CardInfo> list2 = new List<CardInfo>(__instance.Cards);
                int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                bool flag = existingHand != null;
                if (flag)
                {
                    list2.RemoveAll((CardInfo x) => existingHand.Contains(x));
                    list.AddRange(existingHand);
                }
                int num = list2.Count;
                if (num > numCards)
                {
                    for (int i = 0; i < numCards - 1; i++)
                    {
                        int seed = currentRandomSeed + i;
                        CardInfo item = list2[SeededRandom.Range(0, num, seed)];
                        list.Add(item);
                        list2.Remove(item);
                        num--;
                    }
                }
                else
                {
                    list = list2;
                }
                __result = list;
                return false;
            }
        }

        [HarmonyPatch]
        public class Champions
        {
            [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.CleanupPhase))]
            private static class ChampionCleanupPatch
            {
                [HarmonyPostfix]
                public static void ChampionCleanup()
                {
                    summonedChampion = false;
                    summonCount = 0;
                }
            }

            [HarmonyPatch(typeof(Part1BossOpponent), nameof(Part1BossOpponent.PostResetScalesSequence))]
            private static class MoreChampionsOnPhaseTransition
            {
                [HarmonyPostfix]
                public static void ChampionCleanup()
                {
                    summonedChampion = false;
                }
            }

            [HarmonyPatch(typeof(Part1Opponent), nameof(Part1Opponent.ModifyQueuedCard))]
            private class ChampionPatch
            {
                [HarmonyPrefix]
                public static void ChangeToChamp(ref PlayableCard card)
                {
                    summonCount++;
                    var OpponentType = Singleton<Part1Opponent>.Instance.OpponentType;
                    if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_champion.challengeType) && !card.HasAnyOfTraits(new Trait[] { Trait.Giant, Trait.Uncuttable, Trait.Terrain }) //&& card.InOpponentQueue
                        && OpponentType != Part1Opponent.Type.TrapperTraderBoss)
                    {
                        int random = SeededRandom.Range(75, 200, SaveManager.SaveFile.GetCurrentRandomSeed() + Singleton<TurnManager>.Instance.TurnNumber + summonCount);
                        random += Singleton<Part1Opponent>.Instance.Difficulty;
                        //Plugin.Log.LogInfo("Champion random: " + random);
                        if (random >= 100 && !summonedChampion)
                        {
                            ChallengeActivationUI.TryShowActivation(Challenges.Challenge_champion.challengeType);
                            summonedChampion = true;

                            ///Out of 100
                            ///Mythic: 10
                            ///Legendary: 10
                            ///Epic: 15
                            ///Rare: 20
                            ///Uncommon: 20
                            ///Common: 25
                            ///Common
                            ///case <= 13 Red
                            ///case <= 25 Yellow
                            ///Uncommon
                            ///case <= 35 Green
                            ///case <= 45 Purple
                            ///Rare
                            ///case <= 55 Magenta
                            ///case <= 65 Cyan
                            ///Epic
                            ///case <= 73 Orange
                            ///case <= 80 Light Blue
                            ///Legendary
                            ///case <= 85 Light Green
                            ///case <= 90 White
                            ///Mythic
                            ///case <= 95 Blue
                            ///case <= 100 Bright Red

                            if (random <= 113)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.RedChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(0, 2) { abilities = new List<Ability> { GiveRedChamp.ability }, fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 125)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.YellowChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(1, 0) { abilities = new List<Ability> { GiveYellowChamp.ability }, fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 135)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.GreenChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveGreenChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 145)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.OrangeChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveOrangeChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 155)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.CyanChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveCyanChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 165)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.WhiteChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveWhiteChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 173)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.MagentaChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveMagentaChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 180)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.PurpleChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GivePurpleChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 185)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.BlueChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveBlueChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 190)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.LightBlueChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveLightBlueChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 195)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.LightGreenChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveLightGreenChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            else if (random <= 200)
                            {
                                card.ApplyAppearanceBehaviours(new List<CardAppearanceBehaviour.Appearance> { CAppearances.BrightRedChampAppearance });
                                card.AddTemporaryMod(new CardModificationInfo(GiveBrightRedChamp.ability) { fromCardMerge = true });
                                card.AddTemporaryMod(championIDMod);
                            }
                            card.RenderCard();
                        }
                    }
                }
                public static CardModificationInfo championIDMod = new CardModificationInfo() { singletonId = "bitty_champion" };
            }

            public static bool summonedChampion = false;
            public static int summonCount = 0;

        }

        [HarmonyPatch]
        public class MiscEncounters
        {
            public static int TravelingOuroborosBuffs()
            {
                Plugin.Log.LogInfo(ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.TravelingOuroborosBuffs"));
                return ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.TravelingOuroborosBuffs");
            }
            public static void ResetOuro(int value = 0)
            {
                Plugin.Log.LogInfo(string.Format("Resetting Traveling Ouroboros to {0}", value));
                ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.TravelingOuroborosBuffs", value);
            }
            public static void IncreaseOuro(int by = 1)
            {
                int num = ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.TravelingOuroborosBuffs") + by;
                Plugin.Log.LogInfo(string.Format("Increasing Traveling Ouroboros by {0} to {1}", by, num));
                ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.TravelingOuroborosBuffs", num.ToString());
                Plugin.Log.LogInfo(TravelingOuroborosBuffs());
            }
            public static int TimesGoldenSheepKilled()
            {
                return ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.GoldenSheepKilled");
            }
            public static void GoldenSheepKill(int by = 1)
            {
                int num = ModdedSaveManager.RunState.GetValueAsInt(Plugin.PluginGuid, "BittysChallenges.GoldenSheepKilled") + by;
                Plugin.Log.LogInfo(string.Format("Increasing Golden Sheep Killed by {0} to {1}", by, num));
                ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.GoldenSheepKilled", num.ToString());
                Plugin.Log.LogInfo(TimesGoldenSheepKilled());
            }

            public static bool RollForOuro(CardBattleNodeData nodeData)
            {
                if (Plugin.IsP03Run)
                {
                    return SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + 6);
                }
                return true;
            }
            public static double TurnAverage(IEnumerable<CardInfo> turn)
            {
                int num = 0;
                double num2 = 0.0;
                foreach (CardInfo cardInfo in turn)
                {
                    num2 += (double)cardInfo.PowerLevel;
                    num++;
                }
                return (num == 0) ? -100.0 : (num2 / (double)num);
            }
            public static AbilityInfo ValidAbilities(int plus)
            {
                List<AbilityInfo> validAbilities = ScriptableObjectLoader<AbilityInfo>.AllData.FindAll((AbilityInfo x) => x.metaCategories.Contains(AbilityMetaCategory.BountyHunter) ||
                x.ability == Ability.StrafeSwap ||
                x.ability == Ability.StrafePush ||
                x.ability == Sigils.GiveStrafePull.ability ||
                x.ability == Sigils.GiveStrafeSticky.ability ||
                x.ability == Sigils.GiveSwapStats.ability ||
                x.ability == Ability.GainAttackOnKill ||
                x.ability == Ability.CreateEgg ||
                x.ability == Ability.ExplodeOnDeath ||
                x.ability == Ability.Evolve ||
                x.ability == Ability.IceCube ||
                x.ability == Ability.Brittle);

                validAbilities.RemoveAll((AbilityInfo x) =>
                x.ability == Ability.Sentry ||
                x.ability == Ability.DeathShield ||
                x.ability == Ability.Flying ||
                x.ability == Ability.WhackAMole ||
                x.ability == Ability.SplitStrike ||
                x.ability == Ability.SwapStats ||
                x.ability == Ability.GuardDog);

                int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                AbilityInfo abilityInfo = validAbilities[SeededRandom.Range(0, validAbilities.Count, currentRandomSeed + plus)];
                Plugin.Log.LogInfo(abilityInfo.ability);
                return abilityInfo;
            }
            public static void SetOuroCardSlot(CardInfo card, int slot)
            {
                IntWrapper intWrapper;
                bool flag = !ouroAnimationPlayTable.TryGetValue(card, out intWrapper);
                if (flag)
                {
                    ouroAnimationPlayTable.Add(card, new IntWrapper
                    {
                        Value = slot
                    });
                }
                else
                {
                    intWrapper.Value = slot;
                }
            }
            private static int GetOuroCardSlot(CardInfo card)
            {
                IntWrapper intWrapper;
                bool flag = ouroAnimationPlayTable.TryGetValue(card, out intWrapper);
                int result;
                if (flag)
                {
                    result = intWrapper.Value;
                }
                else
                {
                    result = -1;
                }
                return result;
            }
            public class IntWrapper
            {
                public int Value { get; set; }
            }
            private static ConditionalWeakTable<CardInfo, IntWrapper> ouroAnimationPlayTable = new ConditionalWeakTable<CardInfo, IntWrapper>();

            private static Vector3 GetSlotOffset(Transform transform)
            {
                return new Vector3(transform.position.x, -1, 1.5f);
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(Opponent.QueueCard))]
            public static IEnumerator PlayEncounterIntro(IEnumerator sequenceEvent, CardInfo cardInfo, CardSlot slot)
            {
                sequenceEvent.MoveNext();
                yield return sequenceEvent.Current;
                sequenceEvent.MoveNext();

                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_travelingOuro.challengeType))
                {
                    if (cardInfo.name == "bitty_TravelingOuroboros")
                    {
                        MiscEncounters.SetOuroCardSlot(cardInfo, slot.Index);
                    }
                    int customSlot = GetOuroCardSlot(cardInfo);
                    if (customSlot >= 0)
                    {
                        Plugin.Log.LogInfo("Playing animation");

                        View oldView = Singleton<ViewManager>.Instance.CurrentView;

                        ViewInfo targetPos = ViewManager.GetViewInfo(View.OpponentQueue);

                        Vector3 translationOffset;
                        Vector3 rotationOffset;
                        try
                        {
                            translationOffset = targetPos.camPosition + GetSlotOffset(Singleton<BoardManager3D>.Instance.OpponentQueueSlots[slot.Index].transform) - ViewManager.GetViewInfo(Singleton<ViewManager>.Instance.CurrentView).camPosition;
                            rotationOffset = targetPos.camRotation - ViewManager.GetViewInfo(Singleton<ViewManager>.Instance.CurrentView).camRotation;

                            Singleton<ViewManager>.Instance.OffsetPosition(translationOffset, 0.75f);
                            Singleton<ViewManager>.Instance.OffsetRotation(rotationOffset, 0.75f);
                        }
                        catch
                        {
                            Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
                        }

                        ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_travelingOuro.challengeType);
                        if (!Plugin.IsP03Run && !DialogueEventsData.EventIsPlayed("OuroIntro"))
                        {
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("OuroIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        }
                        else if (Plugin.IsP03Run && !DialogueEventsData.EventIsPlayed("P03OuroIntro"))
                        {
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03OuroIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null,
                                new Action<DialogueEvent.Line>(Dialogue.P03HappyCloseUp));
                        }
                        else if (!Plugin.IsP03Run && Singleton<Opponent>.Instance.OpponentType == Opponent.Type.PirateSkullBoss)
                        {

                        }
                        else if (!Plugin.IsP03Run)
                        {
                            int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                            List<string> list = new List<string>
                        {
                            "OuroZoom1",
                            "OuroZoom2",
                            "OuroZoom3",
                            "OuroZoom4"
                        };
                            string zoom = list[SeededRandom.Range(0, list.Count, currentRandomSeed++)];
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(zoom, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        }

                        yield return new WaitForSeconds(0.5f);
                        Singleton<ViewManager>.Instance.SwitchToView(oldView, false, false);
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_goldenSheep.challengeType))
                {
                    int customSlot = -1;
                    if (cardInfo.name == "bitty_GoldenSheep")
                    {
                        customSlot = slot.Index;
                    }

                    if (customSlot >= 0)
                    {
                        Plugin.Log.LogInfo("Playing animation");

                        View oldView = Singleton<ViewManager>.Instance.CurrentView;

                        ViewInfo targetPos = ViewManager.GetViewInfo(View.OpponentQueue);


                        Vector3 translationOffset;
                        Vector3 rotationOffset;
                        try
                        {
                            translationOffset = targetPos.camPosition + GetSlotOffset(Singleton<BoardManager3D>.Instance.OpponentQueueSlots[slot.Index].transform) - ViewManager.GetViewInfo(Singleton<ViewManager>.Instance.CurrentView).camPosition;
                            rotationOffset = targetPos.camRotation - ViewManager.GetViewInfo(Singleton<ViewManager>.Instance.CurrentView).camRotation;

                            Singleton<ViewManager>.Instance.OffsetPosition(translationOffset, 0.75f);
                            Singleton<ViewManager>.Instance.OffsetRotation(rotationOffset, 0.75f);
                        }
                        catch
                        {
                            Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
                        }

                        ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_goldenSheep.challengeType);
                        if (!DialogueEventsData.EventIsPlayed("GoldenSheepIntro"))
                        {
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("GoldenSheepIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        }
                        else
                        {
                            int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                            List<string> list = new List<string>
                        {
                            "GoldenSheepZoom1",
                            "GoldenSheepZoom2",
                            "GoldenSheepZoom3",
                            "GoldenSheepZoom4"
                        };
                            string zoom = list[SeededRandom.Range(0, list.Count, currentRandomSeed++)];
                            yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(zoom, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        }

                        yield return new WaitForSeconds(0.5f);
                        Singleton<ViewManager>.Instance.SwitchToView(oldView, false, false);
                        targetPos = null;
                        translationOffset = default(Vector3);
                        rotationOffset = default(Vector3);
                    }
                }
                if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_harderFinalBoss.challengeType))
                {
                    int customSlot = -1;
                    if (cardInfo.name == "SkeletonPirate")
                    {
                        customSlot = slot.Index;
                    }
                    if (customSlot >= 0 && !DialogueEventsData.EventIsPlayed("PirateIntro"))
                    {
                        ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_harderFinalBoss.challengeType);
                        Plugin.Log.LogInfo("Playing animation");

                        View oldView = Singleton<ViewManager>.Instance.CurrentView;

                        ViewInfo targetPos = ViewManager.GetViewInfo(View.OpponentQueue);


                        Vector3 translationOffset;
                        Vector3 rotationOffset;
                        try
                        {
                            translationOffset = targetPos.camPosition + GetSlotOffset(Singleton<BoardManager3D>.Instance.OpponentQueueSlots[slot.Index].transform) - ViewManager.GetViewInfo(Singleton<ViewManager>.Instance.CurrentView).camPosition;
                            rotationOffset = targetPos.camRotation - ViewManager.GetViewInfo(Singleton<ViewManager>.Instance.CurrentView).camRotation;

                            Singleton<ViewManager>.Instance.OffsetPosition(translationOffset, 0.75f);
                            Singleton<ViewManager>.Instance.OffsetRotation(rotationOffset, 0.75f);
                        }
                        catch
                        {
                            Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
                        }

                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("PirateIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                        yield return new WaitForSeconds(0.5f);
                        Singleton<ViewManager>.Instance.SwitchToView(oldView, false, false);
                        targetPos = null;
                        translationOffset = default(Vector3);
                        rotationOffset = default(Vector3);
                    }
                }
                yield break;
            }

        }
        #endregion
    }
}
