using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using InscryptionAPI.Ascension;
using InscryptionAPI.Card;
using InscryptionAPI.Saves;
using System.Linq;
using BittysChallenges.Encounters;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using InscryptionAPI.Boons;
using InscryptionAPI.Triggers;
using InscryptionMod.Abilities;
using Pixelplacement;
using Object = UnityEngine.Object;
using BittysSigils;
using BepInEx.Bootstrap;
using InscryptionAPI.Helpers.Extensions;

///Changelog: 6.0.0
///Refactor to make future modification easier.
///
///Credit:
namespace BittysChallenges
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("spapi.inscryption.mergesigils", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("bitty45.inscryption.sigils", BepInDependency.DependencyFlags.HardDependency)]
    public partial class Plugin : BaseUnityPlugin
    {
		private void Awake()
		{
			Plugin.Log = base.Logger;
			Harmony harmony = new Harmony(PluginGuid);

			//configs
			famineRemoval = base.Config.Bind<int>("General", "Famine Challenge Severity", 3, "The number of cards removed from your side deck.");
			abundanceQuality = base.Config.Bind<int>("General", "Abundance Challenge Quality", 5, "The number of cards added to your side deck.");
			allowedResets = base.Config.Bind<int>("General", "Extra Lives Allowed Resets", 3, "The max number of times that extra lives will reset the scales during a run.");
			
			using (var s = Tools.CurrentAssembly.GetManifestResourceStream("BittysChallenges.Resources.testbundle"))
			{
				assetBundle = AssetBundle.LoadFromStream(s);
				addedSfx = new List<AudioClip>
				{
					assetBundle.LoadAsset<AudioClip>("vine-boom")
				};
			}

			//loading things into API
			Challenges.AddChallenges();
			Boons.AddBoons();
			Abilities.AddAbilities();
			Cards.AddCards();
            


			//Load all patches
            StarterChallengesPatch.Register(harmony);
			EncounterAddPatches.Register(harmony);
			MiscEncounters.Register(harmony);
			BotchedPatch.Register(harmony);
			SideDeckPatch.Register(harmony);
			SprinterDraw.Register(harmony);
			NoFecundityNerf.Register(harmony);
			RandomPiratesPatch.Register(harmony);
			HarderBosses.Register(harmony);
			InfiniteLives.Register(harmony);
			ReverseScales.Register(harmony);
			Environment.Register(harmony);
			UnfairHandPatch.Register(harmony);
            RedrawHand.Register(harmony);
			Champions.Register(harmony);
            Dialogue.Register(harmony); 
			harmony.PatchAll(typeof(Plugin));
			harmony.PatchAll();
            RulebookExpansion.Register(harmony);

			base.Logger.LogInfo("Plugin Bitty's Challenges is loaded!");
		}
		internal const string PluginGuid = "bitty45.inscryption.challenges";

		internal const string PluginName = "Bitty's Challenges";

		internal const string PluginVersion = "6.0.0";

        internal const string CardPrefix = "bitty";

        public static AssetBundle assetBundle;
		public static List<AudioClip> addedSfx = new List<AudioClip>();

		internal static ConfigEntry<int> famineRemoval;
		internal static ConfigEntry<int> abundanceQuality;
		internal static ConfigEntry<int> allowedResets;

		internal static ManualLogSource Log;

        public static bool IsP03Run
        {
            get
            {
                bool flag = Chainloader.PluginInfos.ContainsKey("zorro.inscryption.infiniscryption.p03kayceerun") && AscensionSaveData.Data != null && AscensionSaveData.Data.currentRun != null && AscensionSaveData.Data.currentRun.playerLives > 0;
                bool result = (flag && ModdedSaveManager.SaveData.GetValueAsBoolean("zorro.inscryption.infiniscryption.p03kayceerun", "IsP03Run"));
                return result;
            }
        }

        [HarmonyPatch(typeof(AudioController), nameof(AudioController.GetAudioClip))]
		[HarmonyPrefix]
		public static void AddAudios(AudioController __instance, string soundId)
		{
			__instance.SFX.AddRange(addedSfx.Where(x => !__instance.SFX.Contains(x)));
		}

		[HarmonyPatch(typeof(AudioController), nameof(AudioController.GetLoopClip))]
		[HarmonyPrefix]
		public static void AddLoops(AudioController __instance, string loopId)
		{
			__instance.Loops.AddRange(addedSfx.Where(x => !__instance.Loops.Contains(x)));
		}

		[HarmonyPatch(typeof(AudioController), nameof(AudioController.GetLoop))]
		[HarmonyPrefix]
		public static void AddLoops2(AudioController __instance, string loopName)
		{
			__instance.Loops.AddRange(addedSfx.Where(x => !__instance.Loops.Contains(x)));
		}
	}
	public partial class Plugin
    {
		
		public class RandomPiratesPatch
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(RandomPiratesPatch));
			}
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
					for (int i = 0; i < Math.Min(numSkeles,3); i = num + 1)
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
		public class StarterChallengesPatch
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(StarterChallengesPatch));
			}

			[HarmonyPatch(typeof(RunIntroSequencer), "TryModifyStarterCards")]
			[HarmonyPostfix]
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
			[HarmonyPatch(typeof(RunIntroSequencer), "RunIntroSequence")]
			[HarmonyPostfix]
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
		public class EncounterAddPatches
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(EncounterAddPatches));
			}
			[HarmonyPostfix]
			[HarmonyPatch(typeof(EncounterBuilder), "Build")]
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
		public class BotchedPatch
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(BotchedPatch));
			}
			[HarmonyPatch(typeof(DuplicateMergeSequencer), "MergeCards")]
			[HarmonyPostfix]
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
				x.ability == Ability.GuardDog||
				x.ability == Ability.Reach ||
				x.ability == Ability.Flying ||
				x.ability == Ability.RandomAbility ||
				x.ability == Plugin.GiveFragile.ability ||
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
		public class SideDeckPatch
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(SideDeckPatch));
			}
			[HarmonyPatch(typeof(CardDrawPiles3D), "InitializePiles")]
			[HarmonyPostfix]
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
		public class SprinterDraw
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(SprinterDraw));
			}
			[HarmonyPatch(typeof(PlayerHand), "AddCardToHand")]
			[HarmonyPostfix]
			public static void SprinterHandPatch(ref PlayableCard card)
			{
				if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_sprinter.challengeType))
				{
					ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_sprinter.challengeType);
					CardModificationInfo mod = new CardModificationInfo();


					if(!Plugin.IsP03Run) mod.fromCardMerge = true;
					mod.abilities.Add(GetRandomStrafe().ability);

					if(!Plugin.IsP03Run || card.AllAbilities().Count() <= 4)
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
			x.ability == Plugin.GiveWarper.ability ||
			x.ability == Sigils.GiveStrafePull.ability ||
			x.ability == Sigils.GiveStrafeSuper.ability);

				AbilityInfo abilityInfo = validAbilities[UnityEngine.Random.Range(0, validAbilities.Count)];
				return abilityInfo;
            }
		}
		public class NoFecundityNerf
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(NoFecundityNerf));
			}
			[HarmonyPatch(typeof(DrawCopy))]
			[HarmonyPatch("CardToDrawTempMods", MethodType.Getter)]
			[HarmonyPostfix]
			private static void Postfix(ref List<CardModificationInfo> __result)
			{
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_oldFecund.challengeType))
				{
					__result = null;
				}
			}
			[HarmonyPostfix]
			[HarmonyPatch(typeof(DrawCopy), nameof(DrawCopy.OnResolveOnBoard))]
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
		public class HarderBosses
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(HarderBosses));
			}
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
		public class InfiniteLives
        {
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(InfiniteLives));
			}

			[HarmonyPostfix]
			[HarmonyPatch(typeof(LifeManager), nameof(LifeManager.ShowDamageSequence))]
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
				Plugin.Log.LogInfo(string.Format("Setting LifeResets to ",num));
				ModdedSaveManager.RunState.SetValue(Plugin.PluginGuid, "BittysChallenges.LifeRepeats", num.ToString());
			}
		}
		public class ReverseScales
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(ReverseScales));
			}

			[HarmonyPostfix]
			[HarmonyPatch(typeof(TurnManager), nameof(TurnManager.SetupPhase))]
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
		public class Environment
        {
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(Environment));
			}

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

            [HarmonyPatch(typeof(BoonsHandler), nameof(BoonsHandler.BoonsEnabled), MethodType.Getter)]
			[HarmonyPostfix]
			public static void OverrideBoonsEnabled(BoonsHandler __instance, ref bool __result)
			{
				if (AscensionSaveData.Data.ChallengeIsActive(Challenges.Challenge_environment.challengeType))
				{
					__result = true;
				}
			}
			[HarmonyPatch(typeof(TurnManager), "SetupPhase")]
			[HarmonyPrefix]
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
					int i = EnvironmentNumber()%boons.Count;
					bool boonActive = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed()+2);
					if (boonActive && boons != null)
					{
						RunState.Run.playerDeck.AddBoon(boons[i]);
						Plugin.Log.LogInfo("Using boon: " + boons[i]);
					}
				}

				return true;
			}
			[HarmonyPatch(typeof(TurnManager), "CleanupPhase")]
			[HarmonyPostfix]
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
			[HarmonyPatch(typeof(BoardManager), "SacrificesCreateRoomForCard")]
			[HarmonyPostfix]
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
		public class RedrawHand
		{
			public static void Register(Harmony harmony)
			{
				harmony.PatchAll(typeof(RedrawHand));
			}

			[HarmonyPatch(typeof(TurnManager), "SetupPhase")]
			private class CloverGiverPatch
			{
				[HarmonyPostfix]
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
        }
        public class UnfairHandPatch
		{
            public static void Register(Harmony harmony)
            {
                harmony.PatchAll(typeof(UnfairHandPatch));
            }

			[HarmonyPatch(typeof(CardDrawPiles3D), "InitializePiles")]
			[HarmonyPostfix]
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

            [HarmonyPatch(typeof(Deck), "GetFairHand")]
            private class GetFairHandPatcher
            {
                [HarmonyPrefix]
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
        }
		
		public class Champions
        {
            public static void Register(Harmony harmony)
            {
                harmony.PatchAll(typeof(Champions));
            }
			[HarmonyPatch(typeof(TurnManager), "CleanupPhase")]
			private static class ChampionCleanupPatch
			{
				[HarmonyPostfix]
				public static void ChampionCleanup()
				{
					summonedChampion = false;
					summonCount = 0;
				}
            }
            [HarmonyPatch(typeof(Part1BossOpponent), "PostResetScalesSequence")]
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
    }
	public partial class Plugin
    {
        public class AddCloverReRollAbility : SpecialCardBehaviour, IOnOtherCardResolveInHand
        {
            public readonly static SpecialTriggeredAbility CloverReRollSpecialAbility = SpecialTriggeredAbilityManager.Add(PluginGuid, "CloverReRollSpecialAbility", typeof(AddCloverReRollAbility)).Id;

            private CardModificationInfo mod = new CardModificationInfo();
            public override bool RespondsToDrawn()
            {
				return true;
            }
            public override IEnumerator OnDrawn()
            {
                mod.singletonId = "bitty_mergeSigil";
                base.PlayableCard.AddTemporaryMod(mod);
                yield break;
            }
            public override bool RespondsToResolveOnBoard()
            {
				return true;
            }
            public override IEnumerator OnResolveOnBoard()
            {
				yield return MoveCardIntoDeck(Singleton<PlayerHand>.Instance.cardsInHand[0], true);
				int cardsToDraw = 0;
				int cardsToShuffle = Singleton<PlayerHand>.Instance.cardsInHand.Count;
				for(int i = 0; i < cardsToShuffle; i++)
				{
					cardsToDraw++;
					yield return MoveCardIntoDeck(Singleton<PlayerHand>.Instance.cardsInHand[0]);
				}

				if(CardDrawPiles3D.Instance.SideDeck.CardsInDeck > 0)
                {
                    yield return new WaitForSeconds(0.4f);
                    if (Singleton<ViewManager>.Instance.CurrentView != View.Default)
                    {
                        yield return new WaitForSeconds(0.2f);
                        Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                        yield return new WaitForSeconds(0.2f);
                    }
                    CardDrawPiles3D.Instance.SidePile.Draw();
                    yield return CardDrawPiles3D.Instance.DrawFromSidePile();
                }
                for (int i = 0; i < cardsToDraw; i++)
                {
                    if (CardDrawPiles3D.Instance.Deck.CardsInDeck > 0)
                    {
                        yield return new WaitForSeconds(0.4f);
                        if (Singleton<ViewManager>.Instance.CurrentView != View.Default)
                        {
                            yield return new WaitForSeconds(0.2f);
                            Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                            yield return new WaitForSeconds(0.2f);
                        }
                        CardDrawPiles3D.Instance.Pile.Draw();
                        yield return CardDrawPiles3D.Instance.DrawCardFromDeck();
                    }
                }
				base.PlayableCard.Anim.PlayDeathAnimation(false);
				Object.Destroy(base.PlayableCard.gameObject);
				yield break;
            }
            public bool RespondsToOtherCardResolveInHand(PlayableCard resolvingCard)
            {
                return resolvingCard.IsPlayerCard() && resolvingCard != base.PlayableCard;
            }
            public IEnumerator OnOtherCardResolveInHand(PlayableCard resolvingCard)
            {
                Singleton<PlayerHand>.Instance.RemoveCardFromHand(base.PlayableCard);
                Object.Destroy(base.PlayableCard.gameObject);
                yield break;
            }

            public IEnumerator MoveCardIntoDeck(PlayableCard card, bool toSideDeck = false)
            {
                bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;

                if (Singleton<PlayerHand>.Instance.CardsInHand.Contains(card) && Singleton<ViewManager>.Instance.CurrentView != View.Hand)
                {
                    yield return new WaitForSeconds(0.2f);
                    Singleton<ViewManager>.Instance.SwitchToView(View.Hand, false, false);
                    yield return new WaitForSeconds(0.2f);
                }
                if (Singleton<PlayerHand>.Instance.CardsInHand.Contains(card))
                {
                    Singleton<PlayerHand>.Instance.RemoveCardFromHand(card);
                }

                if (is3D && toSideDeck)
                {
					//if from hand, tween to be horizontal
                    Singleton<CardDrawPiles3D>.Instance.sidePile.MoveCardToPile(card, true);
                    Object.Destroy(card.gameObject, 1f);
                }
                else if (is3D)
                {
                    Singleton<CardDrawPiles3D>.Instance.pile.MoveCardToPile(card, true);
                    Object.Destroy(card.gameObject, 1f);
                }
                CreateCardInDeck(card.Info, toSideDeck);
            }
            public void CreateCardInDeck(CardInfo info, bool toSideDeck = false)
            {
                bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;
                if (is3D && toSideDeck)
                {
                    Singleton<CardDrawPiles3D>.Instance.sidePile.CreateCards(1);
                    Singleton<CardDrawPiles3D>.Instance.SideDeck.AddCard(info);
                }
                else
                {
                    if (is3D)
                    {
                        Singleton<CardDrawPiles3D>.Instance.pile.CreateCards(1);
                    }
                    Singleton<CardDrawPiles>.Instance.Deck.AddCard(info);
                }
            }
        }
        public class AddTravelingOuroAbility : SpecialCardBehaviour
		{
			public readonly static SpecialTriggeredAbility TravelingOuroSpecialAbility = SpecialTriggeredAbilityManager.Add(PluginGuid, "TravelingOuroSpecialAbility", typeof(AddTravelingOuroAbility)).Id;
            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
                return true;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
                MiscEncounters.IncreaseOuro();
				int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
				List<string> list = new List<string>
					{
						"OuroDies1",
						"OuroDies2",
						"OuroDies3"
					};

				if (Singleton<Opponent>.Instance.OpponentType == Opponent.Type.PirateSkullBoss)
				{
					if (killer != null && killer.OpponentCard == false)
					{
						list = new List<string>
						{
						"RoyalOuroDiesPlayer" 
						};
					}
					else
					{
						list = new List<string>
						{
						"RoyalOuroDies"
						};
					}
				}
                if (IsP03Run)
                {
					list = new List<string>
					{
						"P03OuroDies1",
						"P03OuroDies2",
						"P03OuroDies3"
					};
                }
				string zoom = list[SeededRandom.Range(0, list.Count, currentRandomSeed++)];
				yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(zoom, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				yield break;
            }
		}
		public class AddGoldenSheepAbility : SpecialCardBehaviour
		{
			public readonly static SpecialTriggeredAbility GoldenSheepSpecialAbility = SpecialTriggeredAbilityManager.Add(PluginGuid, "GoldenSheep Special Ability", typeof(AddGoldenSheepAbility)).Id;


			public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
			{
				return killer != null || wasSacrifice;
			}
			public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
			{
				MiscEncounters.GoldenSheepKill();
				yield return this.BreakCage(true);
				int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
				List<string> list = new List<string>
					{
						"SheepDies1",
						"SheepDies2",
						"SheepDies3"
					};
				string zoom = list[SeededRandom.Range(0, list.Count, currentRandomSeed++)];
				yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(zoom, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				
				yield break;
			}
			private IEnumerator BreakCage(bool fromBattle)
			{
				yield return new WaitForSeconds(0.5f);
				if (fromBattle)
				{
					CardInfo pelt = CardLoader.GetCardByName("PeltGolden");
					CardModificationInfo mod = new CardModificationInfo();
					mod.abilities.Add(Plugin.GiveFragile.ability);
					pelt.Mods.Add(mod);
					RunState.Run.playerDeck.AddCard(pelt);
					yield return new WaitForSeconds(0.3f);

					Singleton<ViewManager>.Instance.SwitchToView(View.Hand, false, false);
					yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(pelt, null, 0.25f, null);
					yield return new WaitForSeconds(0.45f);
				}
				yield break;
			}
			public override bool RespondsToUpkeep(bool playerUpkeep)
			{
				return base.PlayableCard.OpponentCard != playerUpkeep;
			}
			public override IEnumerator OnUpkeep(bool playerUpkeep)
			{
				turnCount++;
				Plugin.Log.LogInfo("turn count: "+turnCount);
				if (turnCount > MAX_TURNS)
				{
					int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
					List<string> list = new List<string>
						{
							"SheepEscapes1",
							"SheepEscapes2",
							"SheepEscapes3"
						};
					string zoom = list[SeededRandom.Range(0, list.Count, currentRandomSeed++)];
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(zoom, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

					yield return base.PlayableCard.Die(false, null, false);
				}
				yield break;
			}

			private int turnCount;

			private int MAX_TURNS = 2;

			public static Ability ability;
		}
		public class GiveWarper : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveWarper.ability;
				}
			}

			public override bool RespondsToTurnEnd(bool playerTurnEnd)
			{
				return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
			}

			public override IEnumerator OnTurnEnd(bool playerTurnEnd)
			{
				CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
				CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
				yield return new WaitForSeconds(0.25f);
				yield return base.StartCoroutine(this.DoStrafe(toLeft, toRight));
				yield break;
			}

			protected virtual IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
			{
				bool flag = toLeft == null;
				if (flag)
				{
					toLeft = Singleton<BoardManager>.Instance.playerSlots.Last<CardSlot>();
				}
				bool flag2 = toRight == null;
				if (flag2)
				{
					toRight = Singleton<BoardManager>.Instance.playerSlots.First<CardSlot>();
				}
				CardSlot toLefttwice = Singleton<BoardManager>.Instance.GetAdjacent(toLeft, true);
				CardSlot toRighttwice = Singleton<BoardManager>.Instance.GetAdjacent(toRight, false);
				bool flag3 = toLefttwice == null;
				if (flag3)
				{
					toLefttwice = Singleton<BoardManager>.Instance.playerSlots.Last<CardSlot>();
				}
				bool flag4 = toRighttwice == null;
				if (flag4)
				{
					toRighttwice = Singleton<BoardManager>.Instance.playerSlots.First<CardSlot>();
				}
				bool canmoveleft = toLeft.Card == null || toLefttwice.Card == null;
				bool canmoveright = toRight.Card == null || toRighttwice.Card == null;
				bool flag5 = this.movingLeft && !canmoveleft;
				if (flag5)
				{
					this.movingLeft = false;
				}
				bool flag6 = !this.movingLeft && !canmoveright;
				if (flag6)
				{
					this.movingLeft = true;
				}
				CardSlot destination = this.movingLeft ? toLeft : toRight;
				bool flag7 = destination.Card != null;
				if (flag7)
				{
					destination = (this.movingLeft ? toLefttwice : toRighttwice);
				}
				Plugin.Log.LogInfo(destination.Index);
				yield return base.StartCoroutine(this.MoveToSlot(destination));
				yield break;
			}

			protected IEnumerator MoveToSlot(CardSlot destination)
			{
				base.Card.RenderInfo.SetAbilityFlipped(this.Ability, this.movingLeft);
				base.Card.RenderInfo.flippedPortrait = (this.movingLeft && base.Card.Info.flipPortraitForStrafe);
				base.Card.RenderCard();
				bool flag = destination != null && destination.Card == null;
				if (flag)
				{
					CardSlot oldSlot = base.Card.Slot;
					yield return Singleton<BoardManager>.Instance.AssignCardToSlot(base.Card, destination, 0.1f, null, true);
					yield return this.PostSuccessfulMoveSequence(oldSlot);
					yield return new WaitForSeconds(0.25f);
					oldSlot = null;
					oldSlot = null;
				}
				else
				{
					base.Card.Anim.StrongNegationEffect();
					yield return new WaitForSeconds(0.15f);
				}
				yield break;
			}

			protected virtual IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
			{
				yield break;
			}

			public static Ability ability;

			protected bool movingLeft;
		}
		public class GiveFragile : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveFragile.ability;
				}
			}

            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
                return true;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
				DeckInfo currentDeck = SaveManager.SaveFile.CurrentDeck;
				CardInfo card = currentDeck.Cards.Find((CardInfo x) => x.HasAbility(Plugin.GiveFragile.ability) && x.name == base.Card.Info.name);
				currentDeck.RemoveCard(card);
				yield return base.LearnAbility(0.5f);

				if (!wasSacrifice && killer != null)
				{
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FragileEnemy", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
					{
						base.Card.Info.displayedName, killer.Info.displayedName
                    }, null);
				}
				else if (!wasSacrifice && killer == null)
				{
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FragileDies", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
					{
						base.Card.Info.displayedName
					}, null);
				}
				else
				{
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FragileSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
					   {
						base.Card.Info.displayedName
					   }, null);
				}

				yield break;
            }

            public static Ability ability;
		}
		public class GiveFalseUnkillable : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveFalseUnkillable.ability;
				}
			}

			public static Ability ability;
		}
		public class GiveParalysis : AbilityBehaviour
        {
			public override Ability Ability
			{
				get
				{
					return GiveParalysis.ability;
				}
			}
            public override bool RespondsToUpkeep(bool playerUpkeep)
            {
                bool evenTurn = Singleton<TurnManager>.Instance.TurnNumber % 2 == 0;
                return base.Card != null && base.Card.OpponentCard != playerUpkeep && evenTurn;
			}
            public override IEnumerator OnUpkeep(bool playerUpkeep)
            {
				yield return base.PreSuccessfulTriggerSequence();

				CardModificationInfo cardModificationInfo = new CardModificationInfo();
				if (!base.Card.HasAbility(Sigils.GiveCantAttack.ability))
				{
					cardModificationInfo.abilities.Add(Sigils.GiveCantAttack.ability);
					cardModificationInfo.RemoveOnUpkeep = true;
					base.Card.AddTemporaryMod(cardModificationInfo);
					base.Card.Anim.StrongNegationEffect();
				}
				yield return base.LearnAbility(0f);
				yield break;
			}

            public static Ability ability;
		}
		public class GiveMuddy : MergeKillSelf
		{
			public override Ability Ability
			{
				get
				{
					return GiveMuddy.ability;
				}
			}
			private void Start()
			{
				mod.singletonId = "bitty_mergeSigil";
				base.Card.AddTemporaryMod(mod);
			}
            public override bool CanMergeWith(PlayableCard mergeCard)
            {
				return true;
            }
            public override IEnumerator OnPreMergeDeath(PlayableCard mergeCard)
			{
				yield break;
            }
			public override IEnumerator OnPreCreatureMerge(PlayableCard mergeCard)
			{
				CardModificationInfo mod = new CardModificationInfo();
				mod.abilities.Add(Sigils.GiveCantAttack.ability);
				mod.RemoveOnUpkeep = true;
				mergeCard.AddTemporaryMod(mod);
				mergeCard.AddTemporaryMod(new CardModificationInfo(GiveMuddy.ability) { fromCardMerge = true });

                if (!Plugin.IsP03Run && !CardDisplayer3D.EmissionEnabledForCard(mergeCard.renderInfo, mergeCard))
                {
                    mergeCard.RenderInfo.forceEmissivePortrait = true;
                    mergeCard.StatsLayer.SetEmissionColor(GameColors.Instance.gold);
                }
                mergeCard.RenderCard();
                yield break;
            }
            public override bool RespondsToSacrifice()
            {
				return true;
            }
            public override IEnumerator OnSacrifice()
            {
                yield return base.PreSuccessfulTriggerSequence();
                CardModificationInfo mod = new CardModificationInfo();
                mod.abilities.Add(Sigils.GiveCantAttack.ability);
                mod.RemoveOnUpkeep = true;
                Singleton<BoardManager>.Instance.CurrentSacrificeDemandingCard.AddTemporaryMod(mod);
                yield return base.LearnAbility(0f);
                yield break;
            }

            private CardModificationInfo mod = new CardModificationInfo();

			public static Ability ability;
		}
		public class GiveShelter : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveShelter.ability;
				}
			}

			public static Ability ability;
		}
		public class GiveDynamite : MergeKillSelf
		{
			public override Ability Ability
			{
				get
				{
					return GiveDynamite.ability;
				}
			}
			private void Start()
			{
				mod.singletonId = "bitty_mergeSigil";
				base.Card.AddTemporaryMod(mod);
			}
            public override IEnumerator OnPreMergeDeath(PlayableCard mergeCard)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return this.ExplodeFromSlot(mergeCard.Slot);
				yield return mergeCard.TakeDamage(10, base.Card);
				yield return base.LearnAbility(0.25f);
				yield break;
            }
            public override IEnumerator OnPreCreatureMerge(PlayableCard mergeCard)
			{
				yield break;
			}
			protected IEnumerator ExplodeFromSlot(CardSlot slot)
			{
				List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot);
				if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slot.Index)
				{
					if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
					{
						yield return adjacentSlots[0].Card.TakeDamage(10, null);
					}
					adjacentSlots.RemoveAt(0);
				}
				if (slot.opposingSlot.Card != null && !slot.opposingSlot.Card.Dead)
				{
					yield return slot.opposingSlot.Card.TakeDamage(10, null);
				}
				if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
				{
					yield return adjacentSlots[0].Card.TakeDamage(10, null);
				}
				yield break;
			}
			
			private CardModificationInfo mod = new CardModificationInfo();

			public static Ability ability;
		}
		public class GiveStrafeKiller : Strafe
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafeKiller.ability;
				}
			}
			public override IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
			{
				bool toLeftValid = toLeft != null;
				bool toRightValid = toRight != null;
				if (this.movingLeft && !toLeftValid)
				{
					this.movingLeft = false;
				}
				if (!this.movingLeft && !toRightValid)
				{
					this.movingLeft = true;
				}
				CardSlot destination = this.movingLeft ? toLeft : toRight;
				bool destinationValid = this.movingLeft ? toLeftValid : toRightValid;
				if (destination != null && destination.Card != null)
				{
					yield return destination.Card.Die(false, base.Card);
				}
				yield return new WaitForSeconds(0.2f);
				yield return base.MoveToSlot(destination, destinationValid);
				yield return base.LearnAbility(0f);
				yield break;
			}
			public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield break;
			}

			public static Ability ability;
		}
		public class GiveStrafeAvalanche : Strafe
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafeAvalanche.ability;
				}
			}
			public override IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
			{
				bool toLeftValid = toLeft != null;
				bool toRightValid = toRight != null;
				if (this.movingLeft && !toLeftValid)
				{
					this.movingLeft = false;
				}
				if (!this.movingLeft && !toRightValid)
				{
					this.movingLeft = true;
				}
				CardSlot destination = this.movingLeft ? toLeft : toRight;
				bool destinationValid = this.movingLeft ? toLeftValid : toRightValid;

				List<CardSlot> slotsCopy = null;
				if (!base.Card.OpponentCard)
				{
					slotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
                }
                else
                {
					slotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				}

				if (slotsCopy != null && slotsCopy[slotsCopy.Count-1].Card == base.Card)
				{
					yield return base.Card.Die(false);
					yield return base.LearnAbility(0f);
					yield break;
				}
				int killLoops = 0;
				while (destination != null && destination.Card != null && killLoops < 5)
				{
					killLoops++;
					yield return destination.Card.Die(false, base.Card);
				}
				yield return new WaitForSeconds(0.2f);
				
				yield return base.MoveToSlot(destination, destinationValid);
				yield return base.LearnAbility(0f);
				yield break;
			}
			public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
			{
				if(oldSlot.Card != null)
                {
					yield return oldSlot.Card.Die(false, base.Card);
                }
				yield break;
			}

			public static Ability ability;
		}
		public class GiveObeliskSlot : MergeKillOther
		{
			public override Ability Ability
			{
				get
				{
					return GiveObeliskSlot.ability;
				}
			}
			private void Start()
			{
				mod.singletonId = "bitty_mergeSigil";
				base.Card.AddTemporaryMod(mod);
			}
			public override IEnumerator OnPreMergeDeath(PlayableCard mergeCard)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.LearnAbility(0.25f);

				if (mergeCard.Info.HasTrait(Trait.Goat))
				{
					AudioController.Instance.PlaySound2D("creepy_rattle_lofi", MixerGroup.None, 1f, 0f, null, null, null, null, false);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("GoatSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					DeckInfo currentDeck = SaveManager.SaveFile.CurrentDeck;
					CardInfo card = currentDeck.Cards.Find((CardInfo x) => x == mergeCard.Info);
					if (card != null)
					{
						Plugin.Log.LogInfo("Removing: " + card.name);
						currentDeck.RemoveCard(card);
					}
					Singleton<ViewManager>.Instance.SwitchToView(View.Default);
					yield return new WaitForSeconds(0.25f);
					RunState.Run.playerDeck.AddBoon(BoonData.Type.StartingBones);
					yield return Singleton<BoonsHandler>.Instance.PlayBoonAnimation(BoonData.Type.StartingBones);
					yield return Singleton<ResourcesManager>.Instance.AddBones(8, null);
					yield return new WaitForSeconds(0.25f);
				}
				else if (mergeCard.HasTrait(Trait.Pelt))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("PeltSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

					List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.Card.Info.name != "bitty_Obelisk");
					if (opponentSlotsCopy != null)
					{
						CardInfo card = opponentSlotsCopy[0].Card.Info;
						card.Mods.Add(new CardModificationInfo(Ability.BuffNeighbours));
						AudioController.Instance.PlaySound3D("dueldisk_card_played", MixerGroup.TableObjectsSFX, opponentSlotsCopy[0].Card.transform.position, 2f, 0f, null, null, null, null, false);

						opponentSlotsCopy[0].Card.OnStatsChanged(); 
						opponentSlotsCopy[0].Card.Anim.PlayTransformAnimation();
					}
				}
				else if (mergeCard.name.Contains("Squirrel"))
				{
					yield return new WaitForSeconds(0.7f);
					squirrelSacrifices++;
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("SquirrelSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
					{
						squirrelSacrifices.ToString()
					}, null);
				}
				yield break;
			}
			public override IEnumerator OnPreCreatureMerge(PlayableCard mergeCard)
			{
				
				yield break;
			}

			public int squirrelSacrifices;

			private CardModificationInfo mod = new CardModificationInfo();

			public static Ability ability;
		}
		public class GiveCannoneer : SpecialCardBehaviour
		{
			public readonly static SpecialTriggeredAbility MinicelloSpecialAbility = SpecialTriggeredAbilityManager.Add(PluginGuid, "Minicello Special Ability", typeof(GiveCannoneer)).Id;
			
			static List<GiveCannoneer> allInstancesOfThisClass = new List<GiveCannoneer>();

			public void ClearAllTargetIcons()
			{
				foreach (var instance in allInstancesOfThisClass)
				{
					if (instance != null)
					{
						instance.CleanupTargetIcons();
					}
				}
			}
			private void Start()
			{
				GiveCannoneer.allInstancesOfThisClass.Add(this);
			}
			public override bool RespondsToTurnEnd(bool playerTurnEnd)
			{
				return playerTurnEnd != base.PlayableCard.OpponentCard;
			}
			public override IEnumerator OnTurnEnd(bool playerTurnEnd)
			{
				if (this.cannonTargetSlots.Count > 0)
				{
					yield return this.FireCannonsSequence();
				}
				if (base.PlayableCard != null)
				{
					yield return this.ChooseCannonTargetsSequence();
				}
				yield break;
			}
            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
                return true;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
				this.CleanupTargetIcons();
				yield break;
            }
            public void CleanupTargetIcons()
			{
				this.targetIcons.ForEach(delegate (GameObject x)
				{
					if (x != null)
					{
						this.CleanUpTargetIcon(x);
					}
				});
				this.targetIcons.Clear();
			}
			private void CleanUpTargetIcon(GameObject icon)
			{
				if (icon != null)
				{
					Tween.LocalScale(icon.transform, Vector3.zero, 0.1f, 0f, Tween.EaseIn, Tween.LoopType.None, null, delegate ()
					{
						Object.Destroy(icon);
					}, true);
				}
			}
			private IEnumerator FireCannonsSequence()
			{
				for (int i = 0; i < this.cannonTargetSlots.Count; i ++)
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, true);
					yield return new WaitForSeconds(0.25f);
					CardSlot slot = this.cannonTargetSlots[i];
					if (slot.Card != null && !slot.Card.Dead)
					{
						AudioController.Instance.PlaySound2D("pirateskull_cannon_fire", MixerGroup.TableObjectsSFX, 0.7f, 0f, new AudioParams.Pitch(AudioParams.Pitch.Variation.Small), null, null, null, false);

						yield return new WaitForSeconds(0.3f);
						Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
						yield return new WaitForSeconds(0.2f);
						this.CleanUpTargetIcon(this.targetIcons[i]);
						GameObject cannonBall = Object.Instantiate<GameObject>(ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/CannonBallAnim"));
						cannonBall.transform.position = slot.transform.position;
						Object.Destroy(cannonBall, 1f);
						yield return new WaitForSeconds(0.1666f);
						Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.4f);
						AudioController.Instance.PlaySound3D("metal_object_hit#1", MixerGroup.TableObjectsSFX, cannonBall.transform.position, 1f, 0f, new AudioParams.Pitch(AudioParams.Pitch.Variation.Small), null, null, null, false);
						yield return slot.Card.TakeDamage(10, PlayableCard);
					}
				}
				this.CleanupTargetIcons();
				yield break;
			}
			private IEnumerator ChooseCannonTargetsSequence()
			{
				Plugin.Log.LogInfo("Cannoneer Activation");
				yield return new WaitForSeconds(0.3f);
				int num = GetRandomSeed() + Singleton<TurnManager>.Instance.TurnNumber;
				if (this.targetIconPrefab == null)
				{
					this.targetIconPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/CannonTargetIcon");
				}
				if (base.PlayableCard.OpponentCard)
				{
					List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
					playerSlotsCopy.RemoveAll((CardSlot x) => this.cannonTargetSlots.Contains(x));
					this.cannonTargetSlots.Clear();
					this.cannonTargetSlots.Add(playerSlotsCopy[SeededRandom.Range(0, playerSlotsCopy.Count, num++)]);
				}
                else
				{
					List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy.RemoveAll((CardSlot x) => this.cannonTargetSlots.Contains(x));
					this.cannonTargetSlots.Clear();
					this.cannonTargetSlots.Add(opponentSlotsCopy[SeededRandom.Range(0, opponentSlotsCopy.Count, num++)]);
				}
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.25f);
				foreach (CardSlot slot in this.cannonTargetSlots)
				{
					yield return new WaitForSeconds(0.05f);
					GameObject gameObject = Object.Instantiate<GameObject>(this.targetIconPrefab, slot.transform);
					gameObject.transform.localPosition = new Vector3(0f, 0.25f, 0f);
					gameObject.transform.localRotation = Quaternion.identity;
					this.targetIcons.Add(gameObject);
				}

				AudioController.Instance.PlaySound2D("dial_low", MixerGroup.TableObjectsSFX, 0.7f, 0f, new AudioParams.Pitch(AudioParams.Pitch.Variation.Small), null, null, null, false);
				yield break;
			}
			public bool triggeredThisTurn;

			private List<CardSlot> cannonTargetSlots = new List<CardSlot>();

			private List<GameObject> targetIcons = new List<GameObject>();

			private GameObject targetIconPrefab;
		}

		[HarmonyPatch(typeof(TurnManager), "CleanupPhase")]
		[HarmonyPostfix]
		public static void CannoneerPatch()
		{
			try
            {
				Singleton<GiveCannoneer>.Instance.ClearAllTargetIcons();
			}
            catch
            {
				Plugin.Log.LogInfo("Did not find Cannoneer Instance");
            }
		}
		public class GiveRaft : MergeKillSelf
		{
			public override Ability Ability
			{
				get
				{
					return GiveRaft.ability;
				}
			}
			public override bool IsActualDeath
            {
                get
                {
					return false;
                }
            }
			private void Start()
			{
				mod.singletonId = "bitty_mergeSigil";
				mod.negateAbilities.Add(Ability.Submerge);
				base.Card.AddTemporaryMod(mod);
			}
            public override IEnumerator OnPreCreatureMerge(PlayableCard mergeCard)
			{
				CardModificationInfo mod = new CardModificationInfo();
				mod.negateAbilities.Add(Ability.Submerge);
				mod.negateAbilities.Add(Ability.SubmergeSquid);
				mergeCard.AddTemporaryMod(mod);
				yield break;
            }
            public override IEnumerator OnPreMergeDeath(PlayableCard mergeCard)
            {
				yield break;
			}

			private CardModificationInfo mod = new CardModificationInfo();

			public static Ability ability;
        }
        public class GiveRedChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveRedChamp.ability;
                }
            }

            public static Ability ability;
        }
        public class GiveYellowChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveYellowChamp.ability;
                }
            }

            public static Ability ability;
        }
        public class GiveGreenChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveGreenChamp.ability;
                }
            }
            public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
                return killer == base.Card;
            }
            public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
                yield return base.PreSuccessfulTriggerSequence();
                yield return Singleton<LifeManager>.Instance.ShowDamageSequence(1, 1, base.Card.OpponentCard);
                yield return base.LearnAbility(0f);
                yield break;
            }

            public static Ability ability;
        }
        public class GiveOrangeChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveOrangeChamp.ability;
                }
            }

            public override bool RespondsToDealDamageDirectly(int amount)
            {
                return RunState.Run.currency >= 1 || !this.Card.OpponentCard;
            }
            public override IEnumerator OnDealDamageDirectly(int amount)
            {

                yield return base.PreSuccessfulTriggerSequence();
				if (this.Card.OpponentCard)
				{
					yield return Singleton<CurrencyBowl>.Instance.SpillOnTable();
					yield return new WaitForSeconds(0.4f);

					int moneyToTake = 1;
					if (RunState.Run.currency >= 2) moneyToTake = 2;
					List<Rigidbody> list = Singleton<CurrencyBowl>.Instance.TakeWeights(moneyToTake);
					foreach (Rigidbody rigidbody in list)
					{
						float num = (float)list.IndexOf(rigidbody) * 0.05f;
						Tween.Position(rigidbody.transform, rigidbody.transform.position + Vector3.up * 0.5f, 0.075f, num, Tween.EaseIn, Tween.LoopType.None, null, null, true);
						Tween.Position(rigidbody.transform, new Vector3(0f, 5.5f, 4f), 0.3f, 0.125f + num, Tween.EaseOut, Tween.LoopType.None, null, null, true);
						Object.Destroy(rigidbody.gameObject, 0.5f);
					}

					RunState.Run.currency = RunState.Run.currency - moneyToTake;

					yield return new WaitForSeconds(0.4f);
					yield return base.StartCoroutine(Singleton<CurrencyBowl>.Instance.CleanUpFromTableAndExit());
				}
				else
                {
                    yield return Singleton<CurrencyBowl>.Instance.ShowGain(5, false, true);
                    RunState.Run.currency += 2;
                }
                yield return base.LearnAbility(0f);
                yield break;
            }

            public static Ability ability;
        }
        public class GiveCyanChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveCyanChamp.ability;
                }
            }
            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
				return !wasSacrifice;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
                yield return base.PreSuccessfulTriggerSequence();
                foreach (PlayableCard target in Singleton<BoardManager>.Instance.CardsOnBoard)
				{
					target.Anim.StrongNegationEffect();
					yield return target.TakeDamage(1, null);
                }
                yield return base.LearnAbility(0f);
                yield break;
            }

            public static Ability ability;
        }
        public class GiveWhiteChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveWhiteChamp.ability;
                }
            }
            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
                return !wasSacrifice && base.Card.OnBoard;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
                yield return base.PreSuccessfulTriggerSequence();
                yield return new WaitForSeconds(0.3f);
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("Boulder"), base.Card.Slot, 0.15f, true);
                yield return base.LearnAbility(0.5f);
                yield break;
            }

            public static Ability ability;
        }
        public class GiveMagentaChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveMagentaChamp.ability;
                }
            }
            public override bool RespondsToUpkeep(bool playerUpkeep)
            {
				return playerUpkeep == base.Card.OpponentCard;
            }
            public override IEnumerator OnUpkeep(bool playerUpkeep)
            {
                yield return new WaitForSeconds(0.3f);
                int seed = GetRandomSeed() + Singleton<TurnManager>.Instance.TurnNumber;
				PlayableCard target;
                if (base.Card.OpponentCard)
                {
                    List<PlayableCard> playerCardsCopy = Singleton<BoardManager>.Instance.GetPlayerCards();
					if(playerCardsCopy.Count > 0)
                    {
                        target = playerCardsCopy[SeededRandom.Range(0, playerCardsCopy.Count - 1, seed++)];
                    }
                    else
                    {
                        target = null;
                    }
                }
                else
                {
                    List<PlayableCard> opponentCardsCopy = Singleton<BoardManager>.Instance.GetOpponentCards();
					if(opponentCardsCopy.Count > 0)
                    {
                        target = opponentCardsCopy[SeededRandom.Range(0, opponentCardsCopy.Count - 1, seed++)];
                    }
					else
					{
						target = null;
					}
                }
				if(target == null)
				{
					yield break;
				}
                yield return base.PreSuccessfulTriggerSequence();
				base.Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.3f);
                target.Anim.StrongNegationEffect();
                yield return target.TakeDamage(1, base.Card);
                yield return base.LearnAbility(0.5f);
                yield break;
            }

            public static Ability ability;
        }
        public class GivePurpleChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GivePurpleChamp.ability;
                }
            }
            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
				return otherCard != null && otherCard.Slot != null && otherCard.IsPlayerCard() == base.Card.OpponentCard && otherCard.Slot != base.Card.Slot.opposingSlot;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                yield return new WaitForSeconds(0.15f);
				CardSlot targetSlot = base.Card.OpposingSlot();
                if (targetSlot.Card != null)
                {
                    base.Card.Anim.StrongNegationEffect();
                    yield return new WaitForSeconds(0.3f);
                }
                else
                {
                    yield return base.PreSuccessfulTriggerSequence();
                    Vector3 a = (otherCard.Slot.transform.position + targetSlot.transform.position) / 2f;
                    Tween.Position(otherCard.transform, a + Vector3.up * 0.5f, 0.1f, 0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
                    yield return Singleton<BoardManager>.Instance.AssignCardToSlot(otherCard, targetSlot, 0.1f, null, true);
                    yield return new WaitForSeconds(0.3f);
                    yield return base.LearnAbility(0.1f);
                }
                yield break;
            }

            public static Ability ability;
        }
        public class GiveBlueChamp : AbilityBehaviour, IOnBellRung
        {
            public override Ability Ability
            {
                get
                {
                    return GiveBlueChamp.ability;
                }
            }
            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return RespondsToTrigger(otherCard);
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
                yield return base.PreSuccessfulTriggerSequence();
                yield return AddAirborne(otherCard);
                yield return base.LearnAbility(0f);
                yield break;
            }
            public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
            {
				return RespondsToTrigger(otherCard);
            }
            public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
            {
                yield return base.PreSuccessfulTriggerSequence();
                yield return AddAirborne(otherCard);
                yield return base.LearnAbility(0f);
                yield break;
            }
            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
				return true;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
                yield return base.PreSuccessfulTriggerSequence();
				foreach(PlayableCard target in Singleton<BoardManager>.Instance.CardsOnBoard)
				{
					yield return RemoveAirborne(target);
				}
                yield return base.LearnAbility(0f);
                yield break;
            }
            private bool RespondsToTrigger(PlayableCard otherCard)
            {
                return !base.Card.Dead && !otherCard.Dead && otherCard != base.Card && otherCard.OpponentCard == base.Card.OpponentCard;
            }

            public IEnumerator AddAirborne(PlayableCard target)
            {
                CardModificationInfo cardModificationInfo = target.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == airborneMod.singletonId);
				if (cardModificationInfo == null && !target.Info.HasTrait(Trait.Terrain) && !target.HasAbility(Ability.Flying))
				{
					Plugin.Log.LogInfo("Apply Airborne");
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
					target.AddTemporaryMod(airborneMod);
					target.OnStatsChanged();
					target.Anim.StrongNegationEffect();
					yield return new WaitForSeconds(0.1f);
				}
            }
            public IEnumerator RemoveAirborne(PlayableCard target)
			{
                CardModificationInfo cardModificationInfo = target.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == airborneMod.singletonId);
                if (cardModificationInfo != null)
                {
                    Plugin.Log.LogInfo("Remove Airborne");
                    Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                    target.RemoveTemporaryMod(cardModificationInfo);
					target.OnStatsChanged();
                    target.Anim.StrongNegationEffect();
                    yield return new WaitForSeconds(0.1f);
                }
            }

            public bool RespondsToBellRung(bool playerCombatPhase)
            {
				return true;
            }

            public IEnumerator OnBellRung(bool playerCombatPhase)
            {
                yield return base.PreSuccessfulTriggerSequence();
                List<PlayableCard> cards;
				cards = this.Card.IsPlayerCard() ? Singleton<BoardManager>.Instance.GetPlayerCards() : Singleton<BoardManager>.Instance.GetOpponentCards();
                foreach (PlayableCard curCard in cards)
                {
					if(curCard != this.Card)
                    {
                        yield return AddAirborne(curCard);
                    }
                }
                yield return base.LearnAbility(0f);
                yield break;
            }

            CardModificationInfo airborneMod = new CardModificationInfo()
			{
				singletonId = "bitty_airborne_champ",
				abilities = new List<Ability> { Ability.Flying}
			};

            public static Ability ability;
        }
        public class GiveLightBlueChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveLightBlueChamp.ability;
                }
            }
            public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
            {
				return !wasSacrifice;
            }
            public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
            {
                yield return base.PreSuccessfulTriggerSequence();
				yield return ExplodeFromSlot(base.Card.slot);
                yield return base.LearnAbility(0f);
                yield break;
            }
            public IEnumerator ExplodeFromSlot(CardSlot slot)
            {
                List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot);
                if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slot.Index)
                {
                    if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
                    {
                        yield return this.BombCard(adjacentSlots[0].Card, slot.Card);
                    }
                    adjacentSlots.RemoveAt(0);
                }
                if (slot.opposingSlot.Card != null && !slot.opposingSlot.Card.Dead)
                {
                    yield return this.BombCard(slot.opposingSlot.Card, slot.Card);
                }
                if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
                {
                    yield return this.BombCard(adjacentSlots[0].Card, slot.Card);
                }
                yield break;
            }
            private IEnumerator BombCard(PlayableCard target, PlayableCard attacker)
            {
                yield return new WaitForSeconds(0.5f);
                target.Anim.PlayHitAnimation();
                yield return target.TakeDamage(3, attacker);
                yield break;
            }

            public static Ability ability;
        }
        public class GiveLightGreenChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveLightGreenChamp.ability;
                }
            }

            public override bool RespondsToResolveOnBoard()
            {
				return true;
            }
            public override IEnumerator OnResolveOnBoard()
            {
                yield return base.PreSuccessfulTriggerSequence();
                Card.Status.hiddenAbilities.Add(Ability.DeathShield);
                Card.AddTemporaryMod(new CardModificationInfo(Ability.DeathShield));
				Card.ResetShield();
                yield return base.LearnAbility(0f);
                yield break;
            }
            public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
            {
                if (attacker != null)
                {
                    bool flying = attacker.HasAbility(Ability.Flying);
                    return slot.Card == this.Card && (!flying || base.Card.HasAbility(Ability.Reach));
                }
                return false;
            }
            public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
            {
                yield return base.PreSuccessfulTriggerSequence();
                Card.AddTemporaryMod(new CardModificationInfo()
                {
                    negateAbilities = { this.Ability }
                });
                yield return base.LearnAbility(0f);
                yield break;
            }

            public static Ability ability;
        }
        public class GiveBrightRedChamp : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveBrightRedChamp.ability;
                }
            }
            public override bool RespondsToUpkeep(bool playerUpkeep)
            {
                return playerUpkeep == base.Card.OpponentCard;
            }
            public override IEnumerator OnUpkeep(bool playerUpkeep)
            {
                yield return base.PreSuccessfulTriggerSequence();
				foreach(PlayableCard target in Singleton<BoardManager>.Instance.CardsOnBoard)
				{
					if (target.OpponentCard == base.Card.OpponentCard && !target.HasTrait(Trait.Terrain) &&
						target.Health < target.MaxHealth)
					{
						target.HealDamage(1);
					}
				}
                yield return base.LearnAbility(0.5f);
                yield break;
            }

            public static Ability ability;
        }

        #region ChallengeBoons
        public abstract class ChallengeBoonBase : BoonBehaviour
        {
			protected abstract BoonData.Type boonType { get; }
            public override bool RespondsToPostBoonActivation()
            {
                return true;
            }
			public override IEnumerator OnPostBoonActivation()
			{
				if (!Plugin.IsP03Run && Singleton<BoonsHandler>.Instance.HasBoonOfType(boonType))
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Default);
					yield return Singleton<BoonsHandler>.Instance.PlayBoonAnimation(boonType);
				}
				string P03 = Plugin.IsP03Run ? "P03" : "";
				if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					ChallengeActivationUI.Instance.ShowActivation(Challenges.Challenge_environment.challengeType);
					Singleton<TextDisplayer>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "EnvironmentsIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null));
				}

				yield break;
			}
            public override bool RespondsToPostBattleCleanup()
            {
				return true;
            }
            public override IEnumerator OnPostBattleCleanup()
            {
				SetSceneEffectsShown(false);
				yield break;
            }
            public List<CardSlot> GetSlots(int terrainCards, List<CardSlot> slotsCopy)
			{
				List<CardSlot> slots = new List<CardSlot>();
				int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
				for (int i = 0; i < terrainCards; i++)
				{
					if (terrainCards - i > slotsCopy.Count)
					{
						return slots;
					}
					int randomSlot = SeededRandom.Range(0, slotsCopy.Count, currentRandomSeed + i);
					slots.Add(slotsCopy[randomSlot]);
					slotsCopy.Remove(slotsCopy[randomSlot]);
				}
				return slots;
			}
			public void SetSceneEffectsShown(bool showEffects, 
				Color mainLightColor, 
				Color cardLightColor, 
				Color interactablesColor, 
				Color slotDefaultColor, 
				Color slotInteractableColor, 
				Color slotHighlightColor,
				Color queueSlotDefaultColor,
				Color queueSlotInteractableColor,
				Color queueSlotHighlightColor)
			{
				Singleton<TableVisualEffectsManager>.Instance.SetDustParticlesActive(!showEffects);
				if (showEffects)
				{
					Color darkRed = GameColors.Instance.darkRed;
					darkRed.a = 0.5f;
					Color brownOrange = GameColors.Instance.glowRed;
					brownOrange.a = 0.5f;
					Singleton<TableVisualEffectsManager>.Instance.ChangeTableColors(
						mainLightColor,
						cardLightColor,
						interactablesColor,
						slotDefaultColor,
						slotInteractableColor,
						slotHighlightColor,
						queueSlotDefaultColor,
						queueSlotInteractableColor,
						queueSlotHighlightColor);
					return;
				}
				Singleton<TableVisualEffectsManager>.Instance.ResetTableColors();
			}
			public void SetSceneEffectsShown(bool showEffects)
			{
				Singleton<TableVisualEffectsManager>.Instance.SetDustParticlesActive(!showEffects);
				if (showEffects)
				{
					Color darkRed = GameColors.Instance.gray;
					darkRed.a = 0.5f;
					Color brownOrange = GameColors.Instance.blue;
					brownOrange.a = 0.5f;
					Singleton<TableVisualEffectsManager>.Instance.ChangeTableColors(
						GameColors.Instance.brightNearWhite, GameColors.Instance.brightBlue,
						GameColors.Instance.brightNearWhite, darkRed, GameColors.Instance.gray,
						GameColors.Instance.brightNearWhite, brownOrange, GameColors.Instance.blue,
						GameColors.Instance.brightNearWhite);
					return;
				}
				Singleton<TableVisualEffectsManager>.Instance.ResetTableColors();
			}
        }
		public class ChallengeBoonMud : ChallengeBoonBase
        {
            internal static BoonData.Type boo;

            protected override BoonData.Type boonType
            {
                get
                {
					return boo;
                }
            }
            public override bool RespondsToPreBoonActivation()
            {
                return true;
            }
            public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("MudBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MudBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}

				List<CardInfo> boardCards = new List<CardInfo>();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.PlayerSlotsCopy)
				{
					if (slot.Card != null)
                    {
						boardCards.Add(slot.Card.Info);
					}
				}

				List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
				playerSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(Math.Max((playerSlotsCopy.Count + 1) / 2, 1), playerSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Mud"), slots[i]);

					if (RunState.CurrentRegionTier >= 1)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("Daus"), slots[i].opposingSlot);
					}
				}

				yield break;
            }
		}
		public class ChallengeBoonHail : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
            public override bool RespondsToPreBoonActivation()
            {
                return true;
            }
            public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.gray;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.blue;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, GameColors.Instance.brightBlue, GameColors.Instance.brightBlue,
						GameColors.Instance.brightNearWhite, darkRed, GameColors.Instance.gray,
						GameColors.Instance.brightNearWhite, brownOrange, GameColors.Instance.blue,
						GameColors.Instance.brightNearWhite);
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("HailBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("HailBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}

				if (RunState.CurrentRegionTier >= 1)
				{
					List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

					List<CardSlot> slots = GetSlots(1, opponentSlotsCopy);

					for (int i = 0; i < slots.Count; i++)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Shelter"), slots[i]);
					}
				}
				yield break;
            }
            public override bool RespondsToUpkeep(bool playerUpkeep)
            {
                return true;
            }
            public override IEnumerator OnUpkeep(bool playerUpkeep)
			{
				checkTrue = false;
				Plugin.Log.LogInfo("Hail Activation");
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.AllSlotsCopy)
				{
					CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(slot, true);
					CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(slot, false);
					bool toLeftValid = toLeft != null && toLeft.Card != null;
					bool toRightValid = toRight != null && toRight.Card != null;
					if (!(toLeftValid && toLeft.Card.HasAbility(GiveShelter.ability)) 
						&& !(toRightValid && toRight.Card.HasAbility(GiveShelter.ability)))
					{
						yield return DamageCheck(slot, playerUpkeep);
					}
				}
				if (checkTrue)
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
					yield return Singleton<BoonsHandler>.Instance.PlayBoonAnimation(boonType);
					yield return new WaitForSeconds(0.5f);
				}
				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield break;
			}
			public IEnumerator DamageCheck(CardSlot slot, bool playerUpkeep)
            {
				if (slot.Card != null && slot.Card.OpponentCard != playerUpkeep && !slot.Card.Info.HasTrait(Trait.Terrain))
				{
					if (RunState.CurrentRegionTier >= 1 || slot.Card.Health > 1)
					{
						Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
						yield return new WaitForSeconds(0.5f);
						yield return slot.Card.TakeDamage(1, null);
						checkTrue = true;
					}

					if (slot.Card == null && RunState.CurrentRegionTier >= 2)
                    {
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_IceCube"), slot, 0.5f, false);
					}
					yield return new WaitForSeconds(0.3f);
				}
			}
			bool checkTrue = false;
		}
		public class ChallengeBoonCliffs : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
            public override bool RespondsToPreBoonActivation()
            {
                return true;
            }
			public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("CliffsBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("CliffsBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}
				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;

				PlayableCard card = new PlayableCard();
				card.Info = CardLoader.GetCardByName("bitty_Cliff");
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card.Info, opponentSlotsCopy[0]);
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card.Info, opponentSlotsCopy[0].opposingSlot);

				yield return Singleton<Opponent>.Instance.QueueCard(card.Info, opponentSlotsCopy[0], true, false);
				yield break;
			}
		}
		public class ChallengeBoonMushrooms : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MushroomsBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}

				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				opponentSlotsCopy.Remove(opponentSlotsCopy[0]);
				opponentSlotsCopy.Remove(opponentSlotsCopy[opponentSlotsCopy.Count - 1]);
				opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(1, opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Mushrooms"), slots[i]);
				}

				yield return AudioController.Instance.PlaySound2D("mushroom_large_appear", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
				yield break;
			}
		}
		public class ChallengeBoonDynamite : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("DynamiteBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("DynamiteBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}

				List<CardInfo> boardCards = new List<CardInfo>();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.PlayerSlotsCopy)
				{
					if (slot.Card != null)
					{
						boardCards.Add(slot.Card.Info);
					}
				}

				List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
				playerSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(Math.Max((playerSlotsCopy.Count + 1) / 2, 1), playerSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Dynamite"), slots[i]);
				}

				if (RunState.CurrentRegionTier >= 2)
				{
					List<CardSlot> playerSlotsCopy2 = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
					playerSlotsCopy2.RemoveAll((CardSlot x) => x.Card != null);
					for (int i = 0; i < playerSlotsCopy2.Count; i++)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("GoldNugget"), playerSlotsCopy2[i].opposingSlot);
					}
				}
				yield break;
			}
		}
		public class ChallengeBoonBait : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("BaitBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BaitBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}

				List<CardInfo> boardCards = new List<CardInfo>();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.OpponentSlotsCopy)
				{
					if (slot.Card != null)
					{
						boardCards.Add(slot.Card.Info);
					}
				}

				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(Math.Max((opponentSlotsCopy.Count + 1) / 2, 1), opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("BaitBucket"), slots[i]);
				}

				if (RunState.CurrentRegionTier >= 2)
				{
					List<CardSlot> opponentSlotsCopy2 = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy2.RemoveAll((CardSlot x) => x.Card != null);
					for (int i = 0; i < opponentSlotsCopy2.Count; i++)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Mud"), opponentSlotsCopy2[i].opposingSlot);
					}
				}

				yield break;
			}
            public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
				return card.Info.name == "BaitBucket";
            }
            public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("Shark"), deathSlot);
                yield break;
            }
        }
		public class ChallengeBoonTrap : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("TrapBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("TrapBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}

				List<CardInfo> boardCards = new List<CardInfo>();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.OpponentSlotsCopy)
				{
					if (slot.Card != null)
					{
						boardCards.Add(slot.Card.Info);
					}
				}

				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(Math.Max((opponentSlotsCopy.Count + 1) / 2, 1), opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("Trap"), slots[i]);
				}

				if (RunState.CurrentRegionTier >= 2)
				{
					List<CardSlot> opponentSlotsCopy2 = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy2.RemoveAll((CardSlot x) => x.Card != null);
					for (int i = 0; i < opponentSlotsCopy2.Count; i++)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_IceCube"), opponentSlotsCopy2[i].opposingSlot);
					}
				}

				yield break;
			}
		}
		public class ChallengeBoonTotem : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("TotemBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("TotemBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}

				List<CardInfo> boardCards = new List<CardInfo>();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.OpponentSlotsCopy)
				{
					if (slot.Card != null)
					{
						boardCards.Add(slot.Card.Info);
					}
				}

				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots((opponentSlotsCopy.Count / 5) + 1, opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Totem"), slots[i]);
					if (RunState.CurrentRegionTier >= 2)
					{
						slots[i].Card.AddTemporaryMod(new CardModificationInfo(Ability.DeathShield) { healthAdjustment = 1 });
					}
				}

				yield break;
			}
		}
		public class ChallengeBoonBloodMoon : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.darkRed;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.glowRed;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, GameColors.Instance.glowRed, //main light
						GameColors.Instance.brightRed, //card light
						GameColors.Instance.nearBlack, //interactables
						darkRed, //slot default
						GameColors.Instance.lightGray, //slotInteractables
						GameColors.Instance.nearBlack, //slotHighlight
						brownOrange, //queueSlot default
						GameColors.Instance.lightGray, //queueSlotInteractable
						GameColors.Instance.nearBlack);
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BloodMoonBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}

				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield return new WaitForSeconds(0.1f);
				yield return Singleton<Opponent>.Instance.ClearBoard();
				yield return Singleton<Opponent>.Instance.ClearQueue();
				yield return new WaitForSeconds(0.1f);
				LeshyAnimationController.Instance.SetEyesTexture(ResourceBank.Get<Texture>("Art/Effects/red"));

				yield return CardGlitchSequence(CardLoader.GetCardByName("DireWolfCub"));

				yield break;
			}
			public static IEnumerator CardGlitchSequence(CardInfo grizzlyInfo)
			{
				Singleton<UIManager>.Instance.Effects.GetEffect<ScreenGlitchEffect>().SetIntensity(1f, 1.5f);
				Singleton<CameraEffects>.Instance.Shake(0.1f, 1f);
				AudioController.Instance.PlaySound2D("broken_hum", MixerGroup.None, 0.5f, 0f, null, null, null, null, false);
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.OpponentSlotsCopy)
				{
					if (!Singleton<TurnManager>.Instance.Opponent.QueuedSlots.Contains(slot))
					{
						yield return Singleton<TurnManager>.Instance.Opponent.QueueCard(grizzlyInfo, slot, false, false, false);
					}
					if (slot.Card == null)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(grizzlyInfo, slot, 0f, true);
					}
					if (slot.Card != null)
					{
						Tutorial4BattleSequencer.GiveCardReachAndRedColor(slot.Card);
					}
				}
				foreach (PlayableCard playableCard in Singleton<TurnManager>.Instance.Opponent.Queue)
				{
					Tutorial4BattleSequencer.GiveCardReachAndRedColor(playableCard);
				}
				yield return new WaitForSeconds(0.5f);
				yield break;
			}
		}
		public class ChallengeBoonCarrotPatch : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.darkRed;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.glowRed;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, GameColors.Instance.glowRed, //main light
						GameColors.Instance.brightRed, //card light
						GameColors.Instance.nearBlack, //interactables
						darkRed, //slot default
						GameColors.Instance.lightGray, //slotInteractables
						GameColors.Instance.nearBlack, //slotHighlight
						brownOrange, //queueSlot default
						GameColors.Instance.lightGray, //queueSlotInteractable
						GameColors.Instance.nearBlack);
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BloodMoonBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}

				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield return new WaitForSeconds(0.1f);
				yield return Singleton<Opponent>.Instance.ClearBoard();
				yield return Singleton<Opponent>.Instance.ClearQueue();
				yield return new WaitForSeconds(0.1f);
				LeshyAnimationController.Instance.SetEyesTexture(ResourceBank.Get<Texture>("Art/Effects/red"));

				yield return CardGlitchSequence(CardLoader.GetCardByName("Rabbit"));
				SetSceneEffectsShown(false);
				LeshyAnimationController.Instance.ResetEyesTexture();
				Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
				yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("CarrotBoonIntro", TextDisplayer.MessageAdvanceMode.Auto, TextDisplayer.EventIntersectMode.Wait, null, null);

				yield return new WaitForSeconds(0.5f);
				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("CarrotBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

				yield break;
			}
			public static IEnumerator CardGlitchSequence(CardInfo grizzlyInfo)
			{
				Singleton<UIManager>.Instance.Effects.GetEffect<ScreenGlitchEffect>().SetIntensity(1f, 1.5f);
				Singleton<CameraEffects>.Instance.Shake(0.1f, 1f);
				AudioController.Instance.PlaySound2D("broken_hum", MixerGroup.None, 0.5f, 0f, null, null, null, null, false);
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.OpponentSlotsCopy)
				{
					if (!Singleton<TurnManager>.Instance.Opponent.QueuedSlots.Contains(slot))
					{
						yield return Singleton<TurnManager>.Instance.Opponent.QueueCard(grizzlyInfo, slot, false, false, false);
					}
					if (slot.Card == null)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(grizzlyInfo, slot, 0f, true);
					}
					if (slot.Card != null)
					{
						Tutorial4BattleSequencer.GiveCardReachAndRedColor(slot.Card);
					}
				}
				foreach (PlayableCard playableCard in Singleton<TurnManager>.Instance.Opponent.Queue)
				{
					Tutorial4BattleSequencer.GiveCardReachAndRedColor(playableCard);
				}
				yield return new WaitForSeconds(0.5f);
				yield break;
			}
		}
		public class ChallengeBoonBlizzard : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.gray;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.blue;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, GameColors.Instance.brightBlue, GameColors.Instance.brightBlue,
						GameColors.Instance.brightNearWhite, darkRed, GameColors.Instance.gray,
						GameColors.Instance.brightNearWhite, brownOrange, GameColors.Instance.blue,
						GameColors.Instance.brightNearWhite);
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("BlizzardBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BlizzardBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}
				List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;

				PlayableCard card = new PlayableCard();
				card.Info = CardLoader.GetCardByName("bitty_Avalanche");
				if (playerSlotsCopy[0].Card != null)
				{
					yield return playerSlotsCopy[0].Card.TakeDamage(10, null);
				}
				if (playerSlotsCopy[0].opposingSlot.Card != null)
				{
					yield return playerSlotsCopy[0].opposingSlot.Card.TakeDamage(10, null);
				}
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card.Info, playerSlotsCopy[0]);
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card.Info, playerSlotsCopy[0].opposingSlot);
				yield break;
			}
			public override bool RespondsToUpkeep(bool playerUpkeep)
			{
				return playerUpkeep;
			}
			public override IEnumerator OnUpkeep(bool playerUpkeep)
			{
				Plugin.Log.LogInfo("Avalanche Activation");
				bool avalancheExists = false;
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.AllSlotsCopy)
				{
					if(slot.Card != null && slot.Card.Info.name == "bitty_Avalanche")
                    {
						avalancheExists = true;
                    }
				}
				if (!avalancheExists)
                {
					Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
					yield return Singleton<BoonsHandler>.Instance.PlayBoonAnimation(boonType);
					yield return new WaitForSeconds(0.5f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
					yield return new WaitForSeconds(0.5f);

					List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;

					PlayableCard card = new PlayableCard();
					card.Info = CardLoader.GetCardByName("bitty_Avalanche");
					if (playerSlotsCopy[0].Card != null)
					{
						yield return playerSlotsCopy[0].Card.TakeDamage(10, null);
					}
					if (playerSlotsCopy[0].opposingSlot.Card != null)
					{
						yield return playerSlotsCopy[0].opposingSlot.Card.TakeDamage(10, null);
					}
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card.Info, playerSlotsCopy[0]);
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card.Info, playerSlotsCopy[0].opposingSlot);
				}
				yield return new WaitForSeconds(0.5f);
				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield break;
			}
		}
		public class ChallengeBoonObelisk : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(1, opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Obelisk"), slots[i]);
					if (slots[i].opposingSlot.Card != null)
					{
						yield return slots[i].opposingSlot.Card.Die(false, null, false);
					}
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_ObeliskSpace"), slots[i].opposingSlot);
				}

				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("ObeliskBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("ObeliskBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}
				yield break;
			}
		}
		public class ChallengeBoonMinicello : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{

				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				opponentSlotsCopy.Remove(opponentSlotsCopy[0]);
				opponentSlotsCopy.Remove(opponentSlotsCopy[opponentSlotsCopy.Count - 1]);
				opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(1, opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					CardInfo card = CardLoader.GetCardByName("bitty_Minicello");
					card.mods.Add(new CardModificationInfo(Ability.AllStrike));
					card.mods.Add(new CardModificationInfo(0, 3));
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card, slots[i]);
				}

				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("MinicelloBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						Singleton<ViewManager>.Instance.SwitchToView(View.Board);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MinicelloBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MinicelloBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
						Singleton<ViewManager>.Instance.SwitchToView(View.Default);
					}
				}

				yield break;
			}
		}
		public class ChallengeBoonDarkForest : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.gray;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.lightGray;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, 
					GameColors.Instance.gray, 
					GameColors.Instance.gray,
					GameColors.Instance.brightNearWhite, 
					darkRed, 
					GameColors.Instance.gray,
					GameColors.Instance.lightGray, 
					brownOrange, 
					GameColors.Instance.gray,
					GameColors.Instance.lightGray);

				List<CardInfo> boardCards = new List<CardInfo>();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.OpponentSlotsCopy)
				{
					if (slot.Card != null)
					{
						boardCards.Add(slot.Card.Info);
						PlayableCard otherCard = slot.Card;
						if (otherCard.HasTrait(Trait.Terrain))
						{
							CardInfo cardInfo = otherCard.Info.Clone() as CardInfo;
							CardModificationInfo cardModificationInfo = new CardModificationInfo();
							cardModificationInfo.attackAdjustment = 1;
							cardModificationInfo.nameReplacement = string.Format(Localization.Translate("Living {0}"), cardInfo.DisplayedNameLocalized);
							cardInfo.Mods.Add(cardModificationInfo);
							yield return otherCard.TransformIntoCard(cardInfo);
						}
						yield break;
					}
				}

				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);
				int terrainCards = 0;
				if (boardCards.Count <= 0)
				{
					terrainCards = 3;
				}
				else if (boardCards.Count <= 1)
				{
					terrainCards = 2;
				}
				else if (boardCards.Count == 2)
				{
					terrainCards = 1;
				}

				List<CardSlot> slots = GetSlots(terrainCards, opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					CardInfo card = CardLoader.GetCardByName("Tree");
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card, slots[i]);
				}

				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("DarkForestBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						Singleton<ViewManager>.Instance.SwitchToView(View.Board);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("DarkForestBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}


				yield break;
			}
			public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return otherCard.OpponentCard;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
				if (otherCard.HasTrait(Trait.Terrain))
				{
					CardInfo cardInfo = otherCard.Info.Clone() as CardInfo;
					CardModificationInfo cardModificationInfo = new CardModificationInfo();
					cardModificationInfo.attackAdjustment = 1;
					cardModificationInfo.nameReplacement = string.Format(Localization.Translate("Living {0}"), cardInfo.DisplayedNameLocalized);
					cardInfo.Mods.Add(cardModificationInfo);
					yield return otherCard.TransformIntoCard(cardInfo);
				}
				yield break;
            }
		}
		public class ChallengeBoonFlood : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.gray;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.blue;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, GameColors.Instance.darkBlue, GameColors.Instance.brightBlue,
						GameColors.Instance.brightNearWhite, darkRed, GameColors.Instance.gray,
						GameColors.Instance.brightNearWhite, brownOrange, GameColors.Instance.blue,
						GameColors.Instance.brightNearWhite);
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("FloodBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FloodBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
						yield return new WaitForSeconds(0.7f);
					}
				}
				for(int i = 0; i < 4 - RunState.CurrentRegionTier; i++)
				{
					yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(CardLoader.GetCardByName("bitty_Raft"));
					yield return new WaitForSeconds(0.2f);
				}

				if (RunState.CurrentRegionTier >= 1)
				{
					List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

					List<CardSlot> slots = GetSlots(1, opponentSlotsCopy);

					for (int i = 0; i < slots.Count; i++)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Shelter"), slots[i]);
					}
				}
				yield break;
			}
            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return true;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
				Plugin.Log.LogInfo("Flood Activation");
				CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(otherCard.slot, true);
				CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(otherCard.slot, false);
				bool toLeftValid = toLeft != null && toLeft.Card != null;
				bool toRightValid = toRight != null && toRight.Card != null;
				if (!(toLeftValid && toLeft.Card.HasAbility(GiveShelter.ability))
					&& !(toRightValid && toRight.Card.HasAbility(GiveShelter.ability)))
				{
					yield return WaterCheck(otherCard);
				}
				yield break;
			}
			public IEnumerator WaterCheck(PlayableCard card)
			{
				CardModificationInfo mod = new CardModificationInfo();
				mod.abilities.Add(Ability.Submerge);
				if (!card.Info.HasTrait(Trait.Terrain) && !card.HasAbility(Ability.Flying) && !card.HasAbility(Ability.Submerge))
				{
					Plugin.Log.LogInfo("Apply Waterborne");
					card.AddTemporaryMod(mod);
					card.OnStatsChanged();
					card.Anim.StrongNegationEffect();
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		public class ChallengeBoonBreeze : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
				{
					if (SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("BreezeBoonIntro"))
					{
						yield return new WaitForSeconds(0.7f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BreezeBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}

				if (RunState.CurrentRegionTier >= 1)
				{
					List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

					List<CardSlot> slots = GetSlots(1, opponentSlotsCopy);

					for (int i = 0; i < slots.Count; i++)
					{
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Shelter"), slots[i]);
					}
				}
				yield break;
			}
			public override bool RespondsToUpkeep(bool playerUpkeep)
			{
				return playerUpkeep;
			}
			public override IEnumerator OnUpkeep(bool playerUpkeep)
			{
				Plugin.Log.LogInfo("Breeze Activation");
				yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BreezeActivation", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.CancelSelf, null, null);
				checkTrue = false;
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.AllSlotsCopy)
				{
					CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(slot, true);
					CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(slot, false);
					bool toLeftValid = toLeft != null && toLeft.Card != null;
					bool toRightValid = toRight != null && toRight.Card != null;
					if (!(toLeftValid && toLeft.Card.HasAbility(GiveShelter.ability))
						&& !(toRightValid && toRight.Card.HasAbility(GiveShelter.ability))
						&& slot.Card != null)
					{
						yield return AirborneCheck(slot, playerUpkeep);
					}
				}
				yield return new WaitForSeconds(0.5f);
				if (checkTrue)
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
					yield return Singleton<BoonsHandler>.Instance.PlayBoonAnimation(boonType);
					yield return new WaitForSeconds(0.5f);
				}
				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield break;
			}
			public IEnumerator AirborneCheck(CardSlot slot, bool playerUpkeep)
			{
				CardModificationInfo mod = new CardModificationInfo();
				mod.abilities.Add(Ability.Flying);
				mod.singletonId = "bitty_airborne";
				CardModificationInfo cardModificationInfo = slot.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_airborne");
				if (cardModificationInfo != null)
				{
					Plugin.Log.LogInfo("Remove Airborne");
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
					slot.Card.RemoveTemporaryMod(cardModificationInfo);
					slot.Card.OnStatsChanged();
					slot.Card.Anim.StrongNegationEffect();
					checkTrue = false;
					yield return new WaitForSeconds(0.1f);
				}
				else if (!slot.Card.Info.HasTrait(Trait.Terrain) && !slot.Card.HasAbility(Ability.Flying) && !slot.Card.HasAbility(Ability.Submerge) && !slot.Card.HasAbility(Ability.WhackAMole))
				{
					Plugin.Log.LogInfo("Apply Airborne");
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
					slot.Card.AddTemporaryMod(mod);
					slot.Card.OnStatsChanged();
					slot.Card.Anim.StrongNegationEffect();
					checkTrue = true;
					yield return new WaitForSeconds(0.1f);
				}
			}
			bool checkTrue = false;
		}
		public class ChallengeBoonGraveyard : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.gray;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.lightGray;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, GameColors.Instance.lightGray, GameColors.Instance.lightGray,
						GameColors.Instance.gray, darkRed, GameColors.Instance.gray,
						GameColors.Instance.gray, brownOrange, GameColors.Instance.gray,
						GameColors.Instance.gray);
				string P03 = Plugin.IsP03Run ? "P03" : "";
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro") 
					&& SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed(P03 + "GraveyardBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "GraveyardBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}
				else if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "GraveyardBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "GraveyardBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}

				if (!Plugin.IsP03Run && RunState.CurrentRegionTier >= 1)
				{
					List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

					List<CardSlot> slots = GetSlots(Math.Max((opponentSlotsCopy.Count + 1)/ 2, 1), opponentSlotsCopy);

					for (int i = 0; i < slots.Count; i++)
					{
						string name = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + i) ? "bitty_SkeletonPirate" : "Amoeba";
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), slots[i]);
						PlayableCard card = slots[i].Card;
						yield return card.TriggerHandler.OnTrigger(Trigger.Drawn, Array.Empty<object>());
						yield return Singleton<BoardManager>.Instance.AssignCardToSlot(card, slots[i], 0.1f, null, true);
					}
				}
				else if (Plugin.IsP03Run)
                {
					List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
					opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

					List<CardSlot> slots = GetSlots(Math.Max((opponentSlotsCopy.Count+1)/2,1), opponentSlotsCopy);

					for (int i = 0; i < slots.Count; i++)
					{
						string name = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + i) ? "BrokenBot" : "Amoebot";
						yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), slots[i]);
						PlayableCard card = slots[i].Card;
						yield return card.TriggerHandler.OnTrigger(Trigger.Drawn, Array.Empty<object>()); 
						yield return Singleton<BoardManager>.Instance.AssignCardToSlot(card, slots[i], 0.1f, null, true);
					}
				}
				yield break;
			}
			public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
			{
				return deathSlot.Card != null && !this.currentlyResurrectingCards.Contains(deathSlot.Card.Info) && deathSlot.Card == card;
			}
			public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
			{
				REVIVES++;
				if (REVIVES > 5)
				{
					yield break;
				}
				this.currentlyResurrectingCards.Add(deathSlot.Card.Info);
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(deathSlot.Card.Info, deathSlot, 0.1f, true);
				yield return new WaitForSeconds(0.1f);
				if (deathSlot.Card != null)
				{
					yield return deathSlot.Card.Die(false, null, true);
				}
				this.currentlyResurrectingCards.Clear();
				yield break;
			}
            public override bool RespondsToTurnEnd(bool playerTurnEnd)
            {
				return true;
            }
            public override IEnumerator OnTurnEnd(bool playerTurnEnd)
            {
				REVIVES = 0;
				yield break;
            }
            private int REVIVES;
			private List<CardInfo> currentlyResurrectingCards = new List<CardInfo>();
		}
		public class ChallengeBoonFlashGrowth : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				Color darkRed = GameColors.Instance.darkLimeGreen;
				darkRed.a = 0.5f;
				Color brownOrange = GameColors.Instance.brightLimeGreen;
				brownOrange.a = 0.5f;
				SetSceneEffectsShown(true, GameColors.Instance.brightLimeGreen, GameColors.Instance.brightLimeGreen,
						GameColors.Instance.brightLimeGreen, darkRed, GameColors.Instance.darkLimeGreen,
						GameColors.Instance.brightLimeGreen, brownOrange, GameColors.Instance.limeGreen,
						GameColors.Instance.brightLimeGreen);
				string P03 = Plugin.IsP03Run ? "P03" : "";
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed(P03 + "FlashGrowthBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "FlashGrowthBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}
				else if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "FlashGrowthBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "FlashGrowthBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}
				yield break;
			}
            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
				return true;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
			{
				if (otherCard.HasAbility(Ability.Transformer))
				{
					yield return otherCard.TriggerHandler.OnTrigger(Trigger.Upkeep, new object[]
					{
						true
					});
                }
                else
				{
					yield return otherCard.TriggerHandler.OnTrigger(Trigger.Upkeep, new object[]
					{
					otherCard.IsPlayerCard()? Singleton<TurnManager>.Instance.IsPlayerTurn : !Singleton<TurnManager>.Instance.IsPlayerTurn
                    });
				}
                yield return otherCard.TriggerHandler.OnTrigger(Trigger.TurnEnd, new object[]
                    {
                    otherCard.IsPlayerCard()? Singleton<TurnManager>.Instance.IsPlayerTurn : !Singleton<TurnManager>.Instance.IsPlayerTurn
                    });
                yield break;
            }
		}
		public class ChallengeBoonConveyor : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				for (int i = 0; i < Singleton<BoardManager>.Instance.opponentSlots.Count - 1; i++)
				{
					Singleton<BoardManager>.Instance.opponentSlots[i].SetTexture(Resources.Load<Texture2D>("art/cards/card_slot_left"));
				}
				Singleton<BoardManager>.Instance.opponentSlots[Singleton<BoardManager>.Instance.opponentSlots.Count - 1].SetTexture(Tools.LoadTexture("card_slot_up"));
				for (int j = 1; j < Singleton<BoardManager>.Instance.playerSlots.Count; j++)
				{
					Singleton<BoardManager>.Instance.playerSlots[j].SetTexture(Resources.Load<Texture2D>("art/cards/card_slot_left"));
				}
				Singleton<BoardManager>.Instance.playerSlots[0].SetTexture(Tools.LoadTexture("card_slot_up"));

				string P03 = "P03";
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed(P03 + "ConveyorBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "ConveyorBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}
				else if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "ConveyorBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "ConveyorBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}
				yield break;
			}
			public override bool RespondsToUpkeep(bool playerUpkeep)
            {
				return Singleton<TurnManager>.Instance.TurnNumber != 1 && playerUpkeep;
			}
            public override IEnumerator OnUpkeep(bool playerUpkeep)
			{
				yield return new WaitForSeconds(0.25f);
				yield return Singleton<BoardManager>.Instance.MoveAllCardsClockwise();
				yield return new WaitForSeconds(0.25f);
				yield break;
            }

		}
		public class ChallengeBoonGemSanctuary : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				List<CardSlot> emptySlots = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				emptySlots.RemoveAll((CardSlot x) => x.Card != null);

				List<CardSlot> slots = GetSlots(Math.Max((emptySlots.Count + 1) / 2, 1), emptySlots);
				CardSlot animatorSlot = null;
				for (int i = 0; i < slots.Count; i++)
				{
					string name = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + i + 1) ? "EmptyVessel_OrangeGem" : "EmptyVessel_GreenGem";
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), slots[i]);
					if (i == 0)
					{
						slots[i].Card.AddTemporaryMod(new CardModificationInfo(Ability.BuffGems));
						animatorSlot = slots[i];
					}
				}

				emptySlots = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
				emptySlots.RemoveAll((CardSlot x) => x.Card != null || x == animatorSlot);

				slots = GetSlots(1, emptySlots);

				for (int i = 0; i < slots.Count; i++)
				{
					string name = "EmptyVessel_GreenGem";
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), slots[i]);
					if (i == 0)
					{
						slots[i].Card.AddTemporaryMod(new CardModificationInfo(Ability.BuffGems));
					}
				}

				string P03 = "P03";
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed(P03 + "GemSanctuaryBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "GemSanctuaryBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}
				else if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "GemSanctuaryBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "GemSanctuaryBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
				}
				yield break;
			}
		}
		public class ChallengeBoonElectricStorm : ChallengeBoonBase
		{
			internal static BoonData.Type boo;

			protected override BoonData.Type boonType
			{
				get
				{
					return boo;
				}
			}
			public override bool RespondsToPreBoonActivation()
			{
				return true;
			}
			public override IEnumerator OnPreBoonActivation()
			{
				string P03 = "P03";
				if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed(P03 + "ElectricStormBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "ElectricStormBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, new Action<DialogueEvent.Line>(Dialogue.P03HappyCloseUp));
				}
				else if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "ElectricStormBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "ElectricStormBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, new Action<DialogueEvent.Line>(Dialogue.P03HappyCloseUp));
				}
				yield break;
			}
            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
				return true;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
				otherCard.TakeDamage(1, null);
				otherCard.AddTemporaryMod(new CardModificationInfo(1, 0)
                {
					fromOverclock = true
                });
				yield break;
            }
        }
        #endregion
    }
}
namespace BittysChallenges.Encounters
{
	public partial class MiscEncounters
    {
		public static void Register(Harmony harmony)
		{
			harmony.PatchAll(typeof(MiscEncounters));
		}
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
				return SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed()+6);
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

		[HarmonyPatch(typeof(Opponent), "QueueCard")]
		[HarmonyPostfix]
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
}


