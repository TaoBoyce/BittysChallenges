using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using InscryptionAPI.Ascension;
using InscryptionAPI.Helpers;
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
using System.Reflection;
using System.IO;
using BepInEx.Bootstrap;
/// card intros don't work if curses mod is active
/// not THAT big of a problem since it doesn't affect functionality, just flavor text
///	
/// Grimora mod compatibility?
/// 
///	Leshy can't use merge sigils
///		It was planned for a future merge sigils update
///	
/// Environments:
/// 
/// Test: 
///		Electrical Storm
///			P03
///		Gem Sanctuary
///			P03 dialogue
///			P03
///		Flood rafts
///			Leshy
///		Sprinter Draw (make it not work if there's 4 sigils on the card already)
///			P03
/// 
/// NEED ART FOR:
/// 
/// Credit:
///		—📸𝗦୭ for fleeting squirrels challenge icon base

///Changelog:
///		Abundance Anti-Challenge
///		Vine Boom Death Toggle
///		Fleeting Squirrels
///		P03 Compatibility for a bunch of challenges
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

			Add_Boon_Mud();
			Add_Boon_Hail();
			Add_Boon_Cliff();
			Add_Boon_Mushrooms();
			Add_Boon_Dynamite();
			Add_Boon_Bait();
			Add_Boon_Trap();
			Add_Boon_Totem();
			Add_Boon_BloodMoon();
			Add_Boon_CarrotPatch();
			Add_Boon_Blizzard();
			Add_Boon_Obelisk();
			Add_Boon_Minicello();
			Add_Boon_DarkForest();
			Add_Boon_Flood();
			Add_Boon_Breeze();
			Add_Boon_Graveyard();
			Add_Boon_FlashGrowth();
			Add_Boon_Conveyor();
			Add_Boon_GemSanctuary();
			Add_Boon_ElectricalStorm();

			Add_Ability_FalseUnkillable();
			Add_Ability_Warper();
			Add_Ability_Fragile();
			Add_Ability_Paralysis();
			Add_Ability_Muddy();
			Add_Ability_Shelter();
			Add_Ability_Dynamite();
			Add_Ability_StrafeKiller();
			Add_Ability_StrafeAvalanche();
			Add_Ability_ObeliskSlot();
			Add_Ability_Raft();

			Add_Card_TravelingOuroboros();
			Add_Card_GoldenSheep();
			Add_Card_WoodenBoard();
			Add_Card_Mud();
			Add_Card_Shelter();
			Add_Card_Cliff();
			Add_Card_Mushrooms();
			Add_Card_Dynamite();
			Add_Card_IceCube();
			Add_Card_Totem();
			Add_Card_Avalanche();
			Add_Card_Obelisk();
			Add_Card_ObeliskSpace();
			Add_Card_Minicello();
			Add_Card_DeckSkeletonPirate();
			Add_Card_DeckSkeletonParrot();
			Add_Card_Raft();

			Add_Deck_Pirate();

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
			Dialogue.Dialogue.Register(harmony); 
			harmony.PatchAll(typeof(Plugin));
			RulebookExpander.RulebookExpansion.Register(harmony);

			base.Logger.LogInfo("Plugin Bitty's Challenges is loaded!");
		}
		internal const string PluginGuid = "bitty45.inscryption.challenges";

		internal const string PluginName = "Bitty's Challenges";

		internal const string PluginVersion = "4.0.1";

		private static AscensionChallengeInfo waterborneStarterChallenge;
		private static AscensionChallengeInfo shockedStarterChallenge;
		public static AscensionChallengeInfo travelingOuroChallenge;
		public static AscensionChallengeInfo mycoChallenge;
		public static AscensionChallengeInfo famineChallenge;
		public static AscensionChallengeInfo abundanceChallenge;
		public static AscensionChallengeInfo weakStartersChallenge;
		public static AscensionChallengeInfo sprinterChallenge;
		public static AscensionChallengeInfo oldFecundChallenge;
		public static AscensionChallengeInfo goldenSheepChallenge;
		public static AscensionChallengeInfo harderFinalBossChallenge;
		public static AscensionChallengeInfo weakSoulStarterChallenge;
		public static AscensionChallengeInfo harderBossesChallenge;
		public static AscensionChallengeInfo infiniteLivesChallenge;
		public static AscensionChallengeInfo reverseScalesChallenge;
		public static AscensionChallengeInfo environmentChallenge;
		public static AscensionChallengeInfo vineBoomChallenge;
		public static AscensionChallengeInfo fleetingSquirrelsChallenge;

		public static AssetBundle assetBundle;
		public static List<AudioClip> addedSfx = new List<AudioClip>();

		internal static ConfigEntry<int> famineRemoval;
		internal static ConfigEntry<int> abundanceQuality;
		internal static ConfigEntry<int> allowedResets;

		internal const string CardPrefix = "bitty";

		internal static ManualLogSource Log;

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
		private void AddWaterborneStarterChallenge()
		{
			waterborneStarterChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Aquatic Starters",
					"Cards in starting deck have the Waterborne sigil.",
					10,
					Tools.LoadTexture("ascensionicon_waterbornestarterdeck"),
					Tools.LoadTexture("ascensionicon_activated_waterbornestarterdeck"),
					2
					);
		}
		private void AddShockedStarterChallenge()
		{
			shockedStarterChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Shocked Starters",
					"Cards in starting deck have the Paralysis sigil.",
					20,
					Tools.LoadTexture("ascensionicon_paralysisstarterdeck"),
					Tools.LoadTexture("ascensionicon_activated_paralysisstarterdeck"),
					3
					);
		}
		private void AddTravelingOuroChallenge()
		{
			travelingOuroChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Traveling Ouroboros",
					"A traveling Ouroboros appears throughout the run.",
					40,
					Tools.LoadTexture("ascensionicon_travelingouro"),
					Tools.LoadTexture("ascensionicon_activated_travelingouro"),
					12
					).SetFlags("p03");
		}
		private void AddMycoChallenge()
		{
			mycoChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Botched Experiments",
					"The Mycologists have a chance to make mistakes while fusing cards.",
					5,
					Tools.LoadTexture("ascensionicon_myco"),
					Tools.LoadTexture("ascensionicon_activated_myco"),
					1
					);
		}
		private void AddFamineChallenge()
		{
			famineChallenge = ChallengeManager.AddSpecific(
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
		private void AddAbundanceChallenge()
		{
			abundanceChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Abundance",
					String.Format("The side deck has {0} more cards.", Math.Max(0, Plugin.abundanceQuality.Value)),
					-15,
					Tools.LoadTexture("ascensionicon_abundance"),
					ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
					5,
					2
					).SetFlags("p03")
					.SetIncompatibleChallengeGetterStatic(famineChallenge.challengeType);
			famineChallenge.GetFullChallenge().SetIncompatibleChallengeGetterStatic(abundanceChallenge.challengeType);
		}
		private void AddWeakStartersChallenge()
		{
			weakStartersChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Weak Starters",
					"Cards in the starting deck have 1 less health.",
					5,
					Tools.LoadTexture("ascensionicon_weakstarters"),
					ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
					2
					);
		}
		private void AddSprinterChallenge()
		{
			sprinterChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Sprintmaggedon",
					"All cards get a random Sprinter sigil when drawn.",
					20,
					Tools.LoadTexture("ascensionicon_sprintmageddon"),
					ChallengeManager.HAPPY_ACTIVATED_SPRITE,
					6
					).SetFlags("p03");
		}
		private void AddOldFecundChallenge()
		{
			oldFecundChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Old Fecundity",
					"Reverts the Fecundity Nerf.",
					-20,
					Tools.LoadTexture("ascensionicon_oldfecund"),
					ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
					6
					).SetFlags("p03");
		}
		private void AddGoldenSheepChallenge()
		{
			goldenSheepChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"The Golden Fleece",
					"A Golden Ram appears randomly throughout the run.",
					-20,
					Tools.LoadTexture("ascensionicon_goldensheep"),
					Tools.LoadTexture("ascensionicon_activated_goldensheep"),
					7
					);
		}
		private void AddHarderFinalBossChallenge()
		{
			harderFinalBossChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"True Pirate",
					"Pirates invade Bosses. The Final Boss Challenge is harder.",
					35,
					Tools.LoadTexture("ascensionicon_harderfinalboss"),
					Tools.LoadTexture("ascensionicon_activated_harderfinalboss"),
					13
					);
		}
		private void AddWeakSoulStarterChallenge()
		{
			weakSoulStarterChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Unspirited Starters",
					"Cards in the starting deck have the Weak Soul sigil.",
					25,
					Tools.LoadTexture("ascensionicon_weaksoul"),
					Tools.LoadTexture("ascensionicon_activated_weaksoul"),
					3
					);
		}
		private void AddHarderBossesChallenge()
		{
			harderBossesChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Harder Bosses",
					"Bosses' main cards are more powerful, and bosses are more agressive.",
					25,
					Tools.LoadTexture("ascensionicon_hardersignatures"),
					Tools.LoadTexture("ascensionicon_activated_hardersignatures"),
					8
					);
		}
		private void AddInfiniteLivesChallenge()
		{
			infiniteLivesChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Extra Lives",
					String.Format("The scales will reset once they hit 0, up to {0} times for each candle.", Plugin.allowedResets.Value),
					-Plugin.allowedResets.Value * 75,
					Tools.LoadTexture("ascensionicon_infinitelives"),
					Tools.LoadTexture("ascensionicon_activated_infinitelives"),
					12
					);
		}
		private void AddReverseScalesChallenge()
		{
			reverseScalesChallenge = ChallengeManager.AddSpecific(
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
		private void AddEnvironmentChallenge()
		{
			environmentChallenge = ChallengeManager.AddSpecific(
					PluginGuid,
					"Environmental Effects",
					"At the start of each battle, a random environmental effect may activate.",
					25,
					Tools.LoadTexture("ascensionicon_environment"),
					ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
					9
					).SetFlags("p03");
		}
		private void AddVineBoomChallenge()
		{
			vineBoomChallenge = ChallengeManager.AddSpecific<VineBoomDeath>(
					PluginGuid,
					"Explosive Noise",
					"When a card dies, a loud boom will play. All cards explode on death.",
					0,
					Tools.LoadTexture("ascensionicon_nuclear"),
					Tools.LoadTexture("activated_nuclear")
					).SetFlags("p03");
		}
		private void AddFleetingSquirrelsChallenge()
		{
			fleetingSquirrelsChallenge = ChallengeManager.AddSpecific<FleetingSquirrels>(
					PluginGuid,
					" Side Deck",
					"Your Side Deck cards have the Fleeting sigil.",
					10,
					Tools.LoadTexture("ascensionicon_fleetingsquirrels"),
					Tools.LoadTexture("activated_fleetingsquirrels"),
					1
					).SetFlags("p03");
		}
	}
	public partial class Plugin
    {
		public static bool IsP03Run
		{
			get
			{
				bool flag = Chainloader.PluginInfos.ContainsKey("zorro.inscryption.infiniscryption.p03kayceerun") && AscensionSaveData.Data != null && AscensionSaveData.Data.currentRun != null && AscensionSaveData.Data.currentRun.playerLives > 0;
				bool result = (flag && ModdedSaveManager.SaveData.GetValueAsBoolean("zorro.inscryption.infiniscryption.p03kayceerun", "IsP03Run"));
				return result;
			}
		}
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
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType) && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Default && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Totem)
				{
					ChallengeActivationUI.Instance.ShowActivation(harderFinalBossChallenge.challengeType);
				}
			}
			[HarmonyPostfix]
			[HarmonyPatch(typeof(Opponent), nameof(Opponent.SpawnOpponent))]
			public static void AddToEncounter(ref Opponent __result)
			{
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType) && __result.OpponentType != Opponent.Type.Default && __result.OpponentType != Opponent.Type.Totem)
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
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType))
				{
					return false;
				}
				return true;
			}
			[HarmonyPostfix]
			[HarmonyPatch(typeof(GiantShip), nameof(GiantShip.MutinySequence))]
			public static IEnumerator MutinyChangePost(IEnumerator values)
			{
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType))
				{
					int numSkeles = (Singleton<GiantShip>.Instance.nextHealthThreshold - Singleton<GiantShip>.Instance.PlayableCard.Health) / 5 + 1;
					int num;
					for (int i = 0; i < numSkeles; i = num + 1)
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
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType))
				{
					ChallengeActivationUI.Instance.ShowActivation(harderFinalBossChallenge.challengeType);
					List<CardSlot> opponentSlots = Singleton<BoardManager>.Instance.OpponentSlotsCopy;

					if (AscensionSaveData.Data.ChallengeIsActive(Plugin.travelingOuroChallenge.challengeType))
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
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType))
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
				if (AscensionSaveData.Data.ChallengeIsActive(waterborneStarterChallenge.challengeType))
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
				if (AscensionSaveData.Data.ChallengeIsActive(shockedStarterChallenge.challengeType))
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
				if (AscensionSaveData.Data.ChallengeIsActive(weakStartersChallenge.challengeType))
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
				if (AscensionSaveData.Data.ChallengeIsActive(weakSoulStarterChallenge.challengeType))
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
			}
			[HarmonyPatch(typeof(RunIntroSequencer), "RunIntroSequence")]
			[HarmonyPostfix]
			public static IEnumerator StartersAnnouncer(IEnumerator values)
			{
				yield return values;
				bool dialoguePlayed = false;
                if (AscensionSaveData.Data.ChallengeIsActive(waterborneStarterChallenge.challengeType))
				{
					yield return new WaitForSeconds(0.5f);
					ChallengeActivationUI.Instance.ShowActivation(waterborneStarterChallenge.challengeType);
					if (!dialoguePlayed && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("WaterborneStart"))
					{
						dialoguePlayed = true;
						yield return new WaitForSeconds(0.5f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("WaterborneStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}
				if (AscensionSaveData.Data.ChallengeIsActive(shockedStarterChallenge.challengeType))
				{
					yield return new WaitForSeconds(0.5f);
					ChallengeActivationUI.Instance.ShowActivation(shockedStarterChallenge.challengeType);
					if (!dialoguePlayed && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("ShockedStart"))
					{
						dialoguePlayed = true;
						yield return new WaitForSeconds(0.5f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("ShockedStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
				}
				if (AscensionSaveData.Data.ChallengeIsActive(weakStartersChallenge.challengeType))
				{
					yield return new WaitForSeconds(0.5f);
					ChallengeActivationUI.Instance.ShowActivation(weakStartersChallenge.challengeType);
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
				if (AscensionSaveData.Data.ChallengeIsActive(weakSoulStarterChallenge.challengeType))
				{
					yield return new WaitForSeconds(0.5f);
					ChallengeActivationUI.Instance.ShowActivation(weakSoulStarterChallenge.challengeType);
					if (!dialoguePlayed && SaveFile.IsAscension && !DialogueEventsData.EventIsPlayed("WeakSoulStart"))
					{
						dialoguePlayed = true;
						yield return new WaitForSeconds(0.5f);
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("WeakSoulStart", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
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

				
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.travelingOuroChallenge.challengeType))
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
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.goldenSheepChallenge.challengeType))
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
				if (AscensionSaveData.Data.ChallengeIsActive(mycoChallenge.challengeType))
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
						ChallengeActivationUI.Instance.ShowActivation(mycoChallenge.challengeType);
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
				x.ability == Sigils.GiveStrafeBoard.ability ||
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
				if (AscensionSaveData.Data.ChallengeIsActive(famineChallenge.challengeType) || AscensionSaveData.Data.ChallengeIsActive(abundanceChallenge.challengeType))
				{
					int faminesActive = 0;
					int abundancesActive = 0;
					for (int i = 0; i < AscensionSaveData.Data.GetNumChallengesOfTypeActive(famineChallenge.challengeType); i++)
					{
						faminesActive++;
					}
					for (int i = 0; i < AscensionSaveData.Data.GetNumChallengesOfTypeActive(abundanceChallenge.challengeType); i++)
					{
						abundancesActive++;
					}

					Plugin.Log.LogInfo("Famine Severity: " + faminesActive * Plugin.famineRemoval.Value);
					Plugin.Log.LogInfo("Abundance Quality: " + abundancesActive * Plugin.abundanceQuality.Value);

					ChallengeActivationUI.TryShowActivation(abundanceChallenge.challengeType);
					CardInfo info = Singleton<CardDrawPiles3D>.Instance.SideDeck.cards.Count > 0 ? Singleton<CardDrawPiles3D>.Instance.SideDeck.cards[0] : CardLoader.GetCardByName("Bee");
					Plugin.Log.LogInfo(info.displayedName);
					for (int i = 0; i < (abundancesActive * Plugin.abundanceQuality.Value); i++)
					{
						Singleton<CardDrawPiles3D>.Instance.sidePile.CreateCards(1);
						Singleton<CardDrawPiles3D>.Instance.SideDeck.AddCard(info);
					}

					ChallengeActivationUI.TryShowActivation(famineChallenge.challengeType);
					for (int i = 0; i < Math.Min(10, ((faminesActive * Plugin.famineRemoval.Value))); i++)
					{
						CardDrawPiles3D.Instance.SidePile.Draw();
						CardDrawPiles3D.Instance.SideDeck.Draw();
					}

					Plugin.Log.LogInfo("Total cards in side deck: " + CardDrawPiles3D.Instance.SideDeck.CardsInDeck);
					if (AscensionSaveData.Data.ChallengeIsActive(famineChallenge.challengeType) && !DialogueEventsData.EventIsPlayed("P03FamineIntro") && IsP03Run)
                    {
						Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03FamineIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
						{
							info.displayedName
						}, new Action<DialogueEvent.Line>(Dialogue.Dialogue.P03HappyCloseUp)));
					}
					else if (AscensionSaveData.Data.ChallengeIsActive(famineChallenge.challengeType) && !DialogueEventsData.EventIsPlayed("FamineIntro"))
					{
						Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FamineIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
						{
							info.displayedName
						}, null));
					}
					else if (AscensionSaveData.Data.ChallengeIsActive(abundanceChallenge.challengeType) && !DialogueEventsData.EventIsPlayed("P03AbundanceIntro") && IsP03Run)
					{
						Singleton<CardDrawPiles3D>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03AbundanceIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
						{
							(abundancesActive * Plugin.abundanceQuality.Value).ToString()
						}, new Action<DialogueEvent.Line>(Dialogue.Dialogue.P03HappyCloseUp)));
					}
					else if (AscensionSaveData.Data.ChallengeIsActive(abundanceChallenge.challengeType) && !DialogueEventsData.EventIsPlayed("AbundanceIntro"))
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
				if (AscensionSaveData.Data.ChallengeIsActive(sprinterChallenge.challengeType))
				{
					ChallengeActivationUI.Instance.ShowActivation(sprinterChallenge.challengeType);
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
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(oldFecundChallenge.challengeType))
				{
					__result = null;
				}
			}
			[HarmonyPostfix]
			[HarmonyPatch(typeof(DrawCopy), nameof(DrawCopy.OnResolveOnBoard))]
			public static IEnumerator UnNerfDialogue(IEnumerator values)
			{
				yield return values;
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(oldFecundChallenge.challengeType) && (!DialogueEventsData.EventIsPlayed("P03FecundityUnNerfIntro") || !DialogueEventsData.EventIsPlayed("FecundityUnNerfIntro")))
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
							new Action<DialogueEvent.Line>(Dialogue.Dialogue.P03HappyCloseUp));
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
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Plugin.harderBossesChallenge.challengeType) && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Default && Singleton<Opponent>.Instance.OpponentType != Opponent.Type.Totem)
				{
					ChallengeActivationUI.Instance.ShowActivation(harderBossesChallenge.challengeType);
				}
			}
			[HarmonyPostfix]
			[HarmonyPatch(typeof(TradeCardsForPelts), nameof(TradeCardsForPelts.TradePhase))]
			public static IEnumerator PostTradePlayQueueCards(IEnumerator values)
			{
				yield return values;
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Plugin.harderBossesChallenge.challengeType))
				{
					ChallengeActivationUI.Instance.ShowActivation(harderBossesChallenge.challengeType);
					yield return Singleton<TurnManager>.Instance.opponent.PlayCardsInQueue();
				}
				yield break;
			}
			[HarmonyPostfix]
			[HarmonyPatch(typeof(Part1Opponent), nameof(Part1Opponent.TryModifyCardWithTotem))]
			public static void AddToSignatures(PlayableCard card)
			{
				var OpponentType = Singleton<Part1Opponent>.Instance.OpponentType;
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.harderBossesChallenge.challengeType) && OpponentType != Opponent.Type.Default && OpponentType != Opponent.Type.Totem)
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
				if (SaveFile.IsAscension && AscensionSaveData.Data.ChallengeIsActive(Plugin.infiniteLivesChallenge.challengeType) && Singleton<LifeManager>.Instance.Balance <= -5)
				{
					ChallengeActivationUI.Instance.ShowActivation(infiniteLivesChallenge.challengeType);
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
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.reverseScalesChallenge.challengeType))
				{
					ChallengeActivationUI.Instance.ShowActivation(reverseScalesChallenge.challengeType);
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

			[HarmonyPatch(typeof(BoonsHandler), nameof(BoonsHandler.BoonsEnabled), MethodType.Getter)]
			[HarmonyPostfix]
			public static void OverrideBoonsEnabled(BoonsHandler __instance, ref bool __result)
			{
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.environmentChallenge.challengeType))
				{
					__result = true;
				}
			}
			[HarmonyPatch(typeof(TurnManager), "SetupPhase")]
			[HarmonyPrefix]
			public static bool EnvironmentBoonGiver(ref IEnumerator __result)
			{
				var OpponentType = Singleton<Opponent>.Instance.OpponentType;
				if (AscensionSaveData.Data.ChallengeIsActive(Plugin.environmentChallenge.challengeType) && (OpponentType == Opponent.Type.Default || OpponentType == Opponent.Type.Totem))
				{
                    ClearEnvironmentBoons();
					List<BoonData.Type> boons = new List<BoonData.Type>();
					if (!Plugin.IsP03Run)
					{
						Plugin.Log.LogInfo("Region Tier: " + RunState.CurrentRegionTier);
						if (RunState.CurrentRegionTier >= 0)
						{
							boons.Add(ChallengeBoonCliffs.boo);
							Plugin.Log.LogInfo("Added Cliffs to boons pool: " + ChallengeBoonCliffs.boo);
							boons.Add(ChallengeBoonGraveyard.boo);
							Plugin.Log.LogInfo("Added Graveyard to boons pool: " + ChallengeBoonGraveyard.boo);

							if (RunState.Run.regionTier >= 0 && RunState.Run.regionTier < RunState.Run.regionOrder.Length)
							{
								Plugin.Log.LogInfo("Region: " + RunState.Run.regionOrder[RunState.Run.regionTier]);
								if (RunState.Run.regionOrder[RunState.Run.regionTier] == 0)//woodlands
								{
									boons.Add(ChallengeBoonTotem.boo);
									Plugin.Log.LogInfo("Added Totem to boons pool: " + ChallengeBoonTotem.boo);
								}
								else if (RunState.Run.regionOrder[RunState.Run.regionTier] == 1)//swamp
								{
									boons.Add(ChallengeBoonMud.boo);
									Plugin.Log.LogInfo("Added Mud to boons pool: " + ChallengeBoonMud.boo);
								}
								else if (RunState.Run.regionOrder[RunState.Run.regionTier] == 2)//snowline
								{
									boons.Add(ChallengeBoonHail.boo);
									Plugin.Log.LogInfo("Added Hail to boons pool: " + ChallengeBoonHail.boo);
								}
							}
						}
						if (RunState.CurrentRegionTier >= 1)
						{
							boons.Add(ChallengeBoonBreeze.boo);
							Plugin.Log.LogInfo("Added Breeze to boons pool: " + ChallengeBoonBreeze.boo);
							boons.Add(ChallengeBoonFlashGrowth.boo);
							Plugin.Log.LogInfo("Added Flash Growth to boons pool: " + ChallengeBoonFlashGrowth.boo);

							if (RunState.Run.regionTier >= 0 && RunState.Run.regionTier < RunState.Run.regionOrder.Length)
							{
								if (RunState.Run.regionOrder[RunState.Run.regionTier] == 0)//woodlands
								{
									boons.Add(ChallengeBoonDynamite.boo);
									Plugin.Log.LogInfo("Added Prospector's Camp to boons pool: " + ChallengeBoonDynamite.boo);
								}
								else if (RunState.Run.regionOrder[RunState.Run.regionTier] == 1)//swamp
								{
									boons.Add(ChallengeBoonBait.boo);
									Plugin.Log.LogInfo("Added Angler's Pool to boons pool: " + ChallengeBoonBait.boo);
								}
								else if (RunState.Run.regionOrder[RunState.Run.regionTier] == 2)//snowline
								{
									boons.Add(ChallengeBoonTrap.boo);
									Plugin.Log.LogInfo("Added Trapper's Hunting Grounds to boons pool: " + ChallengeBoonTrap.boo);
								}
							}
						}
						if (RunState.CurrentRegionTier >= 2)
						{
							if (SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed()))
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

								if (AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType) || AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.FinalBoss))
								{
									boons.Add(ChallengeBoonMinicello.boo);
									Plugin.Log.LogInfo("Added Minicello to boons pool: " + ChallengeBoonMinicello.boo);
								}
							}
							if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode) && SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + 1) && !DialogueEventsData.EventIsPlayed("CarrotBoonIntro"))
							{
								boons.Add(ChallengeBoonCarrotPatch.boo);
								Plugin.Log.LogInfo("Added Blood Moon(?) to boons pool: " + ChallengeBoonCarrotPatch.boo);
							}

							if (RunState.Run.regionTier >= 0 && RunState.Run.regionTier < RunState.Run.regionOrder.Length)
							{
								if (RunState.Run.regionOrder[RunState.Run.regionTier] == 0)//woodlands
								{
									boons.Add(ChallengeBoonDarkForest.boo);
									Plugin.Log.LogInfo("Added Dark Forest to boons pool: " + ChallengeBoonDarkForest.boo);
								}
								else if (RunState.Run.regionOrder[RunState.Run.regionTier] == 1)//swamp
								{
									boons.Add(ChallengeBoonFlood.boo);
									Plugin.Log.LogInfo("Added Flood to boons pool: " + ChallengeBoonFlood.boo);
								}
								else if (RunState.Run.regionOrder[RunState.Run.regionTier] == 2)//snowline
								{
									boons.Add(ChallengeBoonBlizzard.boo);
									Plugin.Log.LogInfo("Added Blizzard to boons pool: " + ChallengeBoonBlizzard.boo);
								}
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
					int i = SeededRandom.Range(0, boons.Count, SaveManager.SaveFile.GetCurrentRandomSeed());
					bool boonActive = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed()+1);
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
	}
	public partial class Plugin
    {
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

				if (mergeCard.Info.name == "Goat")
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
					ChallengeActivationUI.Instance.ShowActivation(environmentChallenge.challengeType);
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

				List<CardSlot> slots = GetSlots(Math.Max((opponentSlotsCopy.Count + 1) / 2, 1), opponentSlotsCopy);

				for (int i = 0; i < slots.Count; i++)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Totem"), slots[i]);
					if (RunState.CurrentRegionTier >= 2)
					{
						slots[i].Card.AddTemporaryMod(new CardModificationInfo(Ability.DeathShield));
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
					Singleton<TurnManager>.Instance.IsPlayerTurn
					});
				}
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

				for (int i = 0; i < slots.Count; i++)
				{
					string name = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + i) ? "EmptyVessel_OrangeGem" : "EmptyVessel_GreenGem";
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), slots[i]);
					if (i == 0)
					{
						slots[i].Card.AddTemporaryMod(new CardModificationInfo(Ability.BuffGems));
					}
				}

				emptySlots = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
				emptySlots.RemoveAll((CardSlot x) => x.Card != null);

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
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "ElectricStormBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, new Action<DialogueEvent.Line>(Dialogue.Dialogue.P03HappyCloseUp));
				}
				else if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro")
					&& SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "ElectricStormBoonIntro"))
				{
					yield return new WaitForSeconds(0.7f);
					yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "ElectricStormBoonIntro2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, new Action<DialogueEvent.Line>(Dialogue.Dialogue.P03HappyCloseUp));
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
	}
	public partial class Plugin
    {
		private void Add_Deck_Pirate()
        {
			StarterDeckManager.New(PluginGuid, "PirateDeck", Tools.LoadTexture("starterdeck_icon_pirate.png"),
				new string[]
				{
					"bitty_Minicello",
					"bitty_SkeletonPirate",
					"bitty_SkeletonParrot"
				}, 0);
        }

		private void Add_Card_TravelingOuroboros()
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

			.AddAbilities(Plugin.GiveFalseUnkillable.ability)

			.AddSpecialAbilities(AddTravelingOuroAbility.TravelingOuroSpecialAbility)
			//card appearance
			.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_ouroboros.png"), Tools.LoadTexture("portrait_ouroboros_emission.png"))

			.AddTribes(Tribe.Reptile)
			.SetIceCube(CardLoader.GetCardByName("Adder"))
			;
			TravelingOuroboros.defaultEvolutionName = "Oreoboros";
			// Pass the card to the API.
			CardManager.Add(CardPrefix, TravelingOuroboros);
		}
		private void Add_Card_GoldenSheep()
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
			.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
			.AddAppearances(GoldEmission.Appearance.GoldEmission)

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_goldram.png"), Tools.LoadTexture("portrait_goldram_emission.png"))

			.AddTribes(Tribe.Hooved)
			;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, GoldenSheep);
		}
		private void Add_Card_WoodenBoard()
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
		private void Add_Card_Mud()
		{
			CardInfo Mud = CardManager.New(

				// Card ID Prefix
				modPrefix: CardPrefix,
				// Card internal name.
				"Mud",
				// Card display name.
				"Mud",
				// Attack.
				0,
				// Health.
				1,
				// Description
				description: "A pile of mud."
			)

			//free card

			.AddAbilities(Plugin.GiveMuddy.ability)
			//card appearance
			.SetTerrain()

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_Swamp_Mud.png"), Tools.LoadTexture("portrait_Swamp_Mud.png"))
			;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, Mud);
		}
		private void Add_Card_Shelter()
		{
			CardInfo shelter = CardManager.New(

				// Card ID Prefix
				modPrefix: CardPrefix,
				// Card internal name.
				"Shelter",
				// Card display name.
				"Shelter",
				// Attack.
				0,
				// Health.
				2,
				// Description
				description: "Shelter from the storm."
			)

			//free card

			.AddAbilities(Plugin.GiveShelter.ability)
			//card appearance
			.SetTerrain()

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_shelter.png"), Tools.LoadTexture("portrait_shelter.png"))
			;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, shelter);
		}
		private void Add_Card_Cliff()
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
		private void Add_Card_Mushrooms()
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
			.AddAppearances(CardAppearanceBehaviour.Appearance.TerrainBackground)

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_fungus.png"), Tools.LoadTexture("portrait_fungus.png"))
			;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, mushrooms);
		}
		private void Add_Card_Dynamite()
		{
			CardInfo Dynamite = CardManager.New(

				// Card ID Prefix
				modPrefix: CardPrefix,
				// Card internal name.
				"Dynamite",
				// Card display name.
				"Dynamite",
				// Attack.
				0,
				// Health.
				1,
				// Description
				description: "A box of dynamite."
			)

			//free card

			.AddAbilities(Plugin.GiveDynamite.ability)
			.AddAbilities(Ability.ExplodeOnDeath)
			//card appearance
			.SetTerrain()

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_dynamite.png"), Tools.LoadTexture("portrait_dynamite.png"))
			;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, Dynamite);
		}
		private void Add_Card_IceCube()
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
		private void Add_Card_Totem()
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
		private void Add_Card_Avalanche()
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
			.AddAbilities(Plugin.GiveStrafeAvalanche.ability)
			.AddAbilities(Ability.MadeOfStone)
			//card appearance
			.SetTerrain()

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_avalanche.png"), Tools.LoadTexture("portrait_avalanche.png"))
			;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, Avalanche);
		}
		private void Add_Card_Obelisk()
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
		private void Add_Card_ObeliskSpace()
		{
			CardInfo Obelisk = CardManager.New(

				// Card ID Prefix
				modPrefix: CardPrefix,
				// Card internal name.
				"ObeliskSpace",
				// Card display name.
				"Sacrificial Altar",
				// Attack.
				0,
				// Health.
				5,
				// Description
				description: "A flat mysterious stone."
			)

			//free card

			.AddAbilities(Ability.MadeOfStone)
			.AddAbilities(Plugin.GiveObeliskSlot.ability)
			//card appearance
			.SetTerrain()

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_sacrificeslab.png"), Tools.LoadTexture("portrait_sacrificeslab.png"))
			;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, Obelisk);
		}
		private void Add_Card_Minicello()
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
			.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)

			.SetIceCube(CardLoader.GetCardByName("SkeletonPirate"))

			.SetPortraitAndEmission(Tools.LoadTexture("portrait_minicello.png"), Tools.LoadTexture("portrait_minicello_emission.png"))
			.SetPixelPortrait(Tools.LoadTexture("pixelportrait_ghostshiprepaired.png"))
			;
			minicello.defaultEvolutionName = "Mediumcello";
			minicello.temple = CardTemple.Undead;
			// Pass the card to the API.
			CardManager.Add(CardPrefix, minicello);
		}
		private void Add_Card_DeckSkeletonPirate()
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
		private void Add_Card_DeckSkeletonParrot()
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
		private void Add_Card_Raft()
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

		private void Add_Ability_Warper()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Warper",
				"At the end of the owner's turn, [creature] will move to the right, jumping over any creatures in its path. If it encounters the edge of the board, it will loop over to the other side.",
				typeof(GiveWarper),
				Tools.LoadTexture("ability_warper.png")
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			GiveWarper.ability = abilityInfo.ability;
		}
		private void Add_Ability_Fragile()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Fragile",
				"If [creature] perishes, it is permanently removed from your deck.",
				typeof(GiveFragile),
				Tools.LoadTexture("ability_fragile.png")
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = -3;

			// Pass the ability to the API.
			GiveFragile.ability = abilityInfo.ability;
		}
		private void Add_Ability_FalseUnkillable()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Unkillable",
				"When [creature] perishes, a copy of it is created in the opponent's hand.",
				typeof(GiveFalseUnkillable),
				Tools.LoadTexture("ability_drawcopyondeath")
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			;
			abilityInfo.powerLevel = 1;

			// Pass the ability to the API.
			GiveFalseUnkillable.ability = abilityInfo.ability;
		}
		private void Add_Ability_Paralysis()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Paralysis",
				"[creature] may not attack every other turn.",
				typeof(GiveParalysis),
				Tools.LoadTexture("ability_paralysis.png")
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = -1;
			abilityInfo.abilityLearnedDialogue = Dialogue.Dialogue.SetAbilityInfoDialogue("Stunned and confused.");

			// Pass the ability to the API.
			GiveParalysis.ability = abilityInfo.ability;
		}
		private void Add_Ability_Muddy()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Muddy",
				"Other cards may be placed on top of [creature], but will be unable to attack for one turn.",
				typeof(GiveMuddy),
				Tools.LoadTexture("ability_mud.png")
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = -1;


			// Pass the ability to the API.
			GiveMuddy.ability = abilityInfo.ability;
		}
		private void Add_Ability_Shelter()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Shelter",
				"Adjacent cards are sheltered from environmental effects.",
				typeof(GiveShelter),
				Tools.LoadTexture("ability_shelter.png")
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = 3;


			// Pass the ability to the API.
			GiveShelter.ability = abilityInfo.ability;
		}
		private void Add_Ability_Dynamite()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Explosive",
				"Other cards may be placed on top of [creature]; the other card, adjacent cards, and opposing cards will all be dealt 10 damage.",
				typeof(GiveDynamite),
				Tools.LoadTexture("ability_dynamite.png")
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = -1;


			// Pass the ability to the API.
			GiveDynamite.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafeKiller()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Trampler",
				"At the end of the owner's turn, [creature] will move in the direction inscribed in the sigil. Creatures in the way will be killed.",
				typeof(GiveStrafeKiller),
				Tools.LoadTexture("ability_strafeskull.png")
			)
			.AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			GiveStrafeKiller.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafeAvalanche()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Avalancher",
				"At the end of the owner's turn, [creature] will move in the direction inscribed in the sigil. Creatures in the way will be killed. If [creature] is at the right most side of the board, it dies.",
				typeof(GiveStrafeAvalanche),
				Tools.LoadTexture("ability_strafeskull.png")
			)
			.AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			GiveStrafeAvalanche.ability = abilityInfo.ability;
		}
		private void Add_Ability_ObeliskSlot()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Sacrificial Slab",
				"Other cards may be placed on top of [creature]; the other card will die.",
				typeof(GiveObeliskSlot),
				Tools.LoadTexture("ability_sacrificeslab.png")
			)
			.AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			GiveObeliskSlot.ability = abilityInfo.ability;
		}
		private void Add_Ability_Raft()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Seaworthy",
				"Other cards may be placed on top of [creature], any Waterborne sigils on the card will be negated.",
				typeof(GiveRaft),
				Tools.LoadTexture("ability_raft.png")
			)
			.AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
			;
			abilityInfo.powerLevel = 1;

			// Pass the ability to the API.
			GiveRaft.ability = abilityInfo.ability;
		}

		private void Add_Boon_Mud()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_mud");
			Texture boonCardArt = Tools.LoadTexture("boon_swamp");
			BoonData.Type mudBoon = BoonManager.New(PluginGuid + ".mud", "Environment: Mud Swamp", typeof(ChallengeBoonMud), "You will start the battle with Mud on some of your spaces.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonMud.boo = mudBoon;
		}
		private void Add_Boon_Hail()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_hail");
			Texture boonCardArt = Tools.LoadTexture("boon_snowtrees");
			BoonData.Type hailBoon = BoonManager.New(PluginGuid + ".hail", "Environment: Hail Storm", typeof(ChallengeBoonHail), "At the start of each turn, all of the turn owner's non-terrain cards take 1 damage.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonHail.boo = hailBoon;
		}
		private void Add_Boon_Cliff()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_cliff");
			Texture boonCardArt = Tools.LoadTexture("boon_cliffs");
			BoonData.Type cliffBoon = BoonManager.New(PluginGuid + ".cliff", "Environment: Cliffside", typeof(ChallengeBoonCliffs), "At the start of the battle, the leftmost lane will be blocked with Cliffs.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonCliffs.boo = cliffBoon;
		}
		private void Add_Boon_Mushrooms()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_fungus");
			Texture boonCardArt = Tools.LoadTexture("boon_mushrooms");
			BoonData.Type mushroomsBoon = BoonManager.New(PluginGuid + ".mushrooms", "Environment: Fungal Field", typeof(ChallengeBoonMushrooms), "The opponent will start the battle with Mushrooms.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonMushrooms.boo = mushroomsBoon;
		}
		private void Add_Boon_Dynamite()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_dynamite");
			Texture boonCardArt = Tools.LoadTexture("boon_startingtrees");
			BoonData.Type dynamiteBoon = BoonManager.New(PluginGuid + ".dynamite", "Environment: Prospector's Camp", typeof(ChallengeBoonDynamite), "You will start the battle with Dynamite on some of your spaces.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonDynamite.boo = dynamiteBoon;
		}
		private void Add_Boon_Bait()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_bait");
			Texture boonCardArt = Tools.LoadTexture("boon_swamp");
			BoonData.Type baitBoon = BoonManager.New(PluginGuid + ".bait", "Environment: Angler's Pond", typeof(ChallengeBoonBait), "The opponent will start the battle with Bait Buckets.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonBait.boo = baitBoon;
		}
		private void Add_Boon_Trap()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_trap");
			Texture boonCardArt = Tools.LoadTexture("boon_snowtrees");
			BoonData.Type trapBoon = BoonManager.New(PluginGuid + ".trap", "Environment: Trapper's Hunting Grounds", typeof(ChallengeBoonTrap), "The opponent will start the battle with Steel Traps.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonTrap.boo = trapBoon;
		}
		private void Add_Boon_Totem()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_totem");
			Texture boonCardArt = Tools.LoadTexture("boon_startingtrees");
			BoonData.Type totemBoon = BoonManager.New(PluginGuid + ".totem", "Environment: Cursed Totem", typeof(ChallengeBoonTotem), "The opponent will start the battle with Cursed Totems.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonTotem.boo = totemBoon;
		}
		private void Add_Boon_BloodMoon()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_Blood_Moon");
			Texture boonCardArt = Tools.LoadTexture("boon_Blood_Moon");
			BoonData.Type bloodMoonBoon = BoonManager.New(PluginGuid + ".bloodmoon", "Environment: Blood Moon", typeof(ChallengeBoonBloodMoon), "The opponent will start the battle with Dire Wolf Pups.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonBloodMoon.boo = bloodMoonBoon;
		}
		private void Add_Boon_CarrotPatch()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_carrot");
			Texture boonCardArt = Tools.LoadTexture("boon_Blood_Moon");
			BoonData.Type carrotPatchBoon = BoonManager.New(PluginGuid + ".carrot", "Environment: Carrot Patch", typeof(ChallengeBoonCarrotPatch), "The opponent will start the battle with Rabbits.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonCarrotPatch.boo = carrotPatchBoon;
		}
		private void Add_Boon_Blizzard()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_blizzard");
			Texture boonCardArt = Tools.LoadTexture("boon_snowtrees");
			BoonData.Type blizzardBoon = BoonManager.New(PluginGuid + ".blizzard", "Environment: Blizzard", typeof(ChallengeBoonBlizzard), "At the start of each turn, if there is not an Avalanche present on the board, one will be created on the left-most side of the board.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonBlizzard.boo = blizzardBoon;
		}
		private void Add_Boon_Obelisk()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_obelisk");
			Texture boonCardArt = Tools.LoadTexture("boon_voidaura");
			BoonData.Type obeliskBoon = BoonManager.New(PluginGuid + ".obelisk", "Environment: Obelisk", typeof(ChallengeBoonObelisk), "The opponent will start the battle with an Obelisk.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonObelisk.boo = obeliskBoon;
		}
		private void Add_Boon_Minicello()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_pirate");
			Texture boonCardArt = Tools.LoadTexture("boon_pirate");
			BoonData.Type minicelloBoon = BoonManager.New(PluginGuid + ".minicello", "Environment: Pirate's Hollow", typeof(ChallengeBoonMinicello), "The opponent will start the battle with a Minicello.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonMinicello.boo = minicelloBoon;
		}
		private void Add_Boon_DarkForest()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_tree");
			Texture boonCardArt = Tools.LoadTexture("boon_startingtrees");
			BoonData.Type darkForestBoon = BoonManager.New(PluginGuid + ".darkForest", "Environment: Dark Forest", typeof(ChallengeBoonDarkForest), "The opponent will start the battle with Trees. All of the opponent's terrain have +1 power.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonDarkForest.boo = darkForestBoon;
		}
		private void Add_Boon_Flood()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_flood");
			Texture boonCardArt = Tools.LoadTexture("boon_flood");
			BoonData.Type floodBoon = BoonManager.New(PluginGuid + ".flood", "Environment: Flood", typeof(ChallengeBoonFlood), "Whenever a creature is played, it gains Waterborne. Terrain, and creatures with Airborne are ignored.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonFlood.boo = floodBoon;
		}
		private void Add_Boon_Breeze()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
			Texture boonCardArt = Tools.LoadTexture("boon_breeze");
			BoonData.Type breezeBoon = BoonManager.New(PluginGuid + ".breeze", "Environment: Breeze", typeof(ChallengeBoonBreeze), "At the start of every turn, all creatures gain or lose Airborne. Terrain, and creatures with Waterborne or Burrower are ignored.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonBreeze.boo = breezeBoon;
		}
		private void Add_Boon_Graveyard()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_graveyard");
			Texture boonCardArt = Tools.LoadTexture("boon_graveyard");
			BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Graveyard", typeof(ChallengeBoonGraveyard), "When a creature dies, it dies again.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonGraveyard.boo = boon;
		}
		private void Add_Boon_FlashGrowth()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_flashgrowth");
			Texture boonCardArt = Tools.LoadTexture("boon_flashgrowth");
			BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Flash Growth", typeof(ChallengeBoonFlashGrowth), "When a card is played, any sigils that activate on upkeep are activated.", boonRulebookIcon, boonCardArt, false, false, true);
			ChallengeBoonFlashGrowth.boo = boon;
		}
		private void Add_Boon_Conveyor()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
			Texture boonCardArt = Tools.LoadTexture("boon_blank");
			BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Factory", typeof(ChallengeBoonConveyor), "On Upkeep, all cards are rotated clockwise.", boonRulebookIcon, boonCardArt, false, false, false);
			ChallengeBoonConveyor.boo = boon;
		}
		private void Add_Boon_GemSanctuary()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
			Texture boonCardArt = Tools.LoadTexture("boon_blank");
			BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Gem Sanctuary", typeof(ChallengeBoonGemSanctuary), "All gems have +1 power.", boonRulebookIcon, boonCardArt, false, false, false);
			ChallengeBoonGemSanctuary.boo = boon;
		}
		private void Add_Boon_ElectricalStorm()
		{
			Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
			Texture boonCardArt = Tools.LoadTexture("boon_blank");
			BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Electrical Storm", typeof(ChallengeBoonElectricStorm), "When a card is played, it takes 1 damage and gains 1 power.", boonRulebookIcon, boonCardArt, false, false, false);
			ChallengeBoonElectricStorm.boo = boon;
		}
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

			if (AscensionSaveData.Data.ChallengeIsActive(Plugin.travelingOuroChallenge.challengeType))
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

					ChallengeActivationUI.Instance.ShowActivation(Plugin.travelingOuroChallenge.challengeType);
					if (!Plugin.IsP03Run && !DialogueEventsData.EventIsPlayed("OuroIntro"))
					{
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("OuroIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
					}
					else if (Plugin.IsP03Run && !DialogueEventsData.EventIsPlayed("P03OuroIntro"))
					{
						yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("P03OuroIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null,
							new Action<DialogueEvent.Line>(Dialogue.Dialogue.P03HappyCloseUp));
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
			if (AscensionSaveData.Data.ChallengeIsActive(Plugin.goldenSheepChallenge.challengeType))
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

					ChallengeActivationUI.Instance.ShowActivation(Plugin.goldenSheepChallenge.challengeType);
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
			if (AscensionSaveData.Data.ChallengeIsActive(Plugin.harderFinalBossChallenge.challengeType))
			{
				int customSlot = -1;
				if (cardInfo.name == "SkeletonPirate")
				{
					customSlot = slot.Index;
				}
				if (customSlot >= 0 && !DialogueEventsData.EventIsPlayed("PirateIntro"))
				{
					ChallengeActivationUI.Instance.ShowActivation(Plugin.harderFinalBossChallenge.challengeType);
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
namespace RulebookExpander
{
	public class RulebookExpansion
	{
		public static void Register(Harmony harmony)
		{
			harmony.PatchAll(typeof(RulebookExpansion));
		}
		[HarmonyPatch(typeof(RuleBookInfo), "AbilityShouldBeAdded")]
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, ref int abilityIndex)
		{
			AbilityInfo info = AbilitiesUtil.GetInfo((Ability)abilityIndex);
			if (info.ability == Ability.MoveBeside || 
				info.ability == Ability.ExplodeOnDeath ||
				info.ability == Ability.SkeletonStrafe ||
				info.ability == Ability.Sentry ||
				info.ability == Sigils.GiveNoTransfer.ability ||
				info.ability == BittysChallenges.Plugin.GiveParalysis.ability ||
				info.ability == Ability.ConduitBuffAttack ||
				info.ability == Sigils.GiveFleeting.ability ||
				info.ability == Sigils.GiveCantAttack.ability ||
				info.ability == Sigils.GiveStrafePull.ability ||
				info.ability == Sigils.GiveStrafeSticky.ability ||
				info.ability == Sigils.GiveStrafeSuper.ability || 
				info.ability == BittysChallenges.Plugin.GiveWarper.ability)
            {
				__result = true;
			}
		}
	}
}
namespace Division.Helpers
{
	public static class DialogueHelper
	{
		public static DialogueEvent.LineSet CreateLineSet(string[] lineString, Emotion emotion = Emotion.Neutral, TextDisplayer.LetterAnimation animation = TextDisplayer.LetterAnimation.None, P03AnimationController.Face p03Face = P03AnimationController.Face.Default, int speakerIndex = 0)
		{
			return new DialogueEvent.LineSet
			{
				lines = (from s in lineString
						 select new DialogueEvent.Line
						 {
							 text = s,
							 emotion = emotion,
							 letterAnimation = animation,
							 p03Face = p03Face,
							 speakerIndex = speakerIndex
						 }).ToList<DialogueEvent.Line>()
			};
		}
		public static void AddDialogue(string id, List<string> lines, List<string> faces, List<string> dialogueWavies)
		{
			DialogueEvent.Speaker speaker = DialogueEvent.Speaker.P03;
			bool flag = faces.Exists((string s) => s.ToLowerInvariant().Contains("leshy"));
			if (flag)
			{
				speaker = DialogueEvent.Speaker.Leshy;
			}
			else
			{
				bool flag2 = faces.Exists((string s) => s.ToLowerInvariant().Contains("telegrapher"));
				if (flag2)
				{
					speaker = DialogueEvent.Speaker.P03Telegrapher;
				}
				else
				{
					bool flag3 = faces.Exists((string s) => s.ToLowerInvariant().Contains("archivist"));
					if (flag3)
					{
						speaker = DialogueEvent.Speaker.P03Archivist;
					}
					else
					{
						bool flag4 = faces.Exists((string s) => s.ToLowerInvariant().Contains("photographer"));
						if (flag4)
						{
							speaker = DialogueEvent.Speaker.P03Photographer;
						}
						else
						{
							bool flag5 = faces.Exists((string s) => s.ToLowerInvariant().Contains("canvas"));
							if (flag5)
							{
								speaker = DialogueEvent.Speaker.P03Canvas;
							}
							else
							{
								bool flag6 = faces.Exists((string s) => s.ToLowerInvariant().Contains("goo"));
								if (flag6)
								{
									speaker = DialogueEvent.Speaker.Goo;
								}
								else
								{
									bool flag7 = faces.Exists((string s) => s.ToLowerInvariant().Contains("side"));
									if (flag7)
									{
										speaker = DialogueEvent.Speaker.P03MycologistSide;
									}
									else
									{
										bool flag8 = faces.Exists((string s) => s.ToLowerInvariant().Contains("mycolo"));
										if (flag8)
										{
											speaker = DialogueEvent.Speaker.P03MycologistMain;
										}
									}
								}
							}
						}
					}
				}
			}
			bool leshy = speaker == DialogueEvent.Speaker.Leshy || speaker == DialogueEvent.Speaker.Goo;
			Emotion leshyEmotion = faces.Exists((string s) => s.ToLowerInvariant().Contains("goocurious")) ? Emotion.Curious : Emotion.Neutral;
			bool flag9 = string.IsNullOrEmpty(id);
			if (!flag9)
			{
				List<DialogueEvent> events = DialogueDataUtil.Data.events;
				DialogueEvent dialogueEvent = new DialogueEvent();
				dialogueEvent.id = id;
				dialogueEvent.speakers = new List<DialogueEvent.Speaker>
				{
					DialogueEvent.Speaker.Single,
					speaker
				};
				dialogueEvent.mainLines = new DialogueEvent.LineSet(faces.Zip(lines, (string face, string line) => new DialogueEvent.Line
				{
					text = line,
					specialInstruction = "",
					p03Face = (leshy ? P03AnimationController.Face.NoChange : face.ParseFace()),
					speakerIndex = 1,
					emotion = (leshy ? leshyEmotion : face.ParseFace().FaceEmotion())
				}).Zip(dialogueWavies, delegate (DialogueEvent.Line line, string wavy)
				{
					bool flag10 = !string.IsNullOrEmpty(wavy) && wavy.ToLowerInvariant() == "y";
					if (flag10)
					{
						line.letterAnimation = TextDisplayer.LetterAnimation.WavyJitter;
					}
					return line;
				}).ToList<DialogueEvent.Line>());
				events.Add(dialogueEvent);
			}
		}
		private static P03AnimationController.Face ParseFace(this string face)
		{
			bool flag = string.IsNullOrEmpty(face);
			P03AnimationController.Face result;
			if (flag)
			{
				result = P03AnimationController.Face.NoChange;
			}
			else
			{
				result = (P03AnimationController.Face)Enum.Parse(typeof(P03AnimationController.Face), face);
			}
			return result;
		}
		private static Emotion FaceEmotion(this P03AnimationController.Face face)
		{
			bool flag = face == P03AnimationController.Face.Angry;
			Emotion result;
			if (flag)
			{
				result = Emotion.Anger;
			}
			else
			{
				bool flag2 = face == P03AnimationController.Face.Thinking;
				if (flag2)
				{
					result = Emotion.Curious;
				}
				else
				{
					bool flag3 = face == P03AnimationController.Face.MycologistAngry;
					if (flag3)
					{
						result = Emotion.Anger;
					}
					else
					{
						bool flag4 = face == P03AnimationController.Face.MycologistLaughing;
						if (flag4)
						{
							result = Emotion.Laughter;
						}
						else
						{
							result = Emotion.Neutral;
						}
					}
				}
			}
			return result;
		}
		public static void AddOrModifySimpleDialogEvent(string eventId, string line, TextDisplayer.LetterAnimation? animation = null, Emotion? emotion = null)
		{
			string[] lines = new string[]
			{
				line
			};
			DialogueHelper.AddOrModifySimpleDialogEvent(eventId, lines, null, animation, emotion, "NewRunDealtDeckDefault");
		}
		private static void SyncLineCollection(List<DialogueEvent.Line> curLines, string[] newLines, TextDisplayer.LetterAnimation? animation, Emotion? emotion)
		{
			while (curLines.Count > newLines.Length)
			{
				curLines.RemoveAt(curLines.Count - 1);
			}
			for (int i = 0; i < curLines.Count; i++)
			{
				curLines[i].text = newLines[i];
			}
			for (int j = curLines.Count; j < newLines.Length; j++)
			{
				DialogueEvent.Line line = DialogueHelper.CloneLine(curLines[0]);
				line.text = newLines[j];
				bool flag = animation != null;
				if (flag)
				{
					line.letterAnimation = animation.Value;
				}
				bool flag2 = emotion != null;
				if (flag2)
				{
					line.emotion = emotion.Value;
				}
				curLines.Add(line);
			}
		}
		public static void AddOrModifySimpleDialogEvent(string eventId, string[] lines, string[][] repeatLines = null, TextDisplayer.LetterAnimation? animation = null, Emotion? emotion = null, string template = "NewRunDealtDeckDefault")
		{
			bool flag = false;
			DialogueEvent dialogueEvent = DialogueDataUtil.Data.GetEvent(eventId);
			bool flag2 = dialogueEvent == null;
			if (flag2)
			{
				flag = true;
				dialogueEvent = DialogueHelper.CloneDialogueEvent(DialogueDataUtil.Data.GetEvent(template), eventId, false);
				while (dialogueEvent.mainLines.lines.Count > lines.Length)
				{
					dialogueEvent.mainLines.lines.RemoveAt(lines.Length);
				}
			}
			DialogueHelper.SyncLineCollection(dialogueEvent.mainLines.lines, lines, animation, emotion);
			bool flag3 = repeatLines == null;
			if (flag3)
			{
				dialogueEvent.repeatLines.Clear();
			}
			else
			{
				while (dialogueEvent.repeatLines.Count > repeatLines.Length)
				{
					dialogueEvent.repeatLines.RemoveAt(dialogueEvent.repeatLines.Count - 1);
				}
				for (int i = 0; i < dialogueEvent.repeatLines.Count; i++)
				{
					DialogueHelper.SyncLineCollection(dialogueEvent.repeatLines[i].lines, repeatLines[i], animation, emotion);
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				DialogueDataUtil.Data.events.Add(dialogueEvent);
			}
		}
		public static DialogueEvent.Line CloneLine(DialogueEvent.Line line)
		{
			return new DialogueEvent.Line
			{
				p03Face = line.p03Face,
				emotion = line.emotion,
				letterAnimation = line.letterAnimation,
				speakerIndex = line.speakerIndex,
				text = line.text,
				specialInstruction = line.specialInstruction,
				storyCondition = line.storyCondition,
				storyConditionMustBeMet = line.storyConditionMustBeMet
			};
		}
		public static DialogueEvent CloneDialogueEvent(DialogueEvent dialogueEvent, string newId, bool includeRepeat = false)
		{
			DialogueEvent dialogueEvent2 = new DialogueEvent
			{
				id = newId,
				groupId = dialogueEvent.groupId,
				mainLines = new DialogueEvent.LineSet(),
				speakers = new List<DialogueEvent.Speaker>(),
				repeatLines = new List<DialogueEvent.LineSet>()
			};
			foreach (DialogueEvent.Line line in dialogueEvent.mainLines.lines)
			{
				dialogueEvent2.mainLines.lines.Add(DialogueHelper.CloneLine(line));
			}
			if (includeRepeat)
			{
				foreach (DialogueEvent.LineSet lineSet in dialogueEvent.repeatLines)
				{
					DialogueEvent.LineSet lineSet2 = new DialogueEvent.LineSet();
					foreach (DialogueEvent.Line line2 in lineSet.lines)
					{
						lineSet2.lines.Add(DialogueHelper.CloneLine(line2));
					}
					dialogueEvent2.repeatLines.Add(lineSet2);
				}
			}
			foreach (DialogueEvent.Speaker item in dialogueEvent.speakers)
			{
				dialogueEvent2.speakers.Add(item);
			}
			return dialogueEvent2;
		}
	}
}
namespace Dialogue
{
	public class Dialogue
    {
		public static void Register(Harmony harmony)
		{
			harmony.PatchAll(typeof(Dialogue));
		}
		public static DialogueEvent.LineSet SetAbilityInfoDialogue(string dialogue)
		{
			return new DialogueEvent.LineSet(new List<DialogueEvent.Line>
			{
				new DialogueEvent.Line
				{
					text = dialogue
				}
			});
		}
		public static void P03HappyCloseUp(DialogueEvent.Line line)
		{
			if (line.p03Face == P03AnimationController.Face.Happy)
			{
				Singleton<ViewManager>.Instance.SwitchToView(View.P03Face, true, false);
			}
			else
			{
				Singleton<ViewManager>.Instance.SwitchToView(View.Default, true, false);
			}
		}
		[HarmonyPatch(typeof(DialogueDataUtil), nameof(DialogueDataUtil.ReadDialogueData))]
		[HarmonyPostfix]
		public static void ModDialogue()
		{
			///-------------Act 1 Lines----------
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroIntro", new string[]
			{
				"oh?",
				"my very own ouroboros.",
				"i will be sure to put it to good use."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom1", new string[]
			{
				"the ouroboros has followed you.",
				"it's fangs are beared, and it's ready for another fight."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom3", new string[]
			{
				"the unyielding ouroboros has returned,",
				"growing stronger every death."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom4", new string[]
			{
				"as inevitable as death,",
				"the ouroboros has returned."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom2", new string[]
			{
				"a serpent slithers out from the undergrowth.",
				"perhaps you have seen it before?"
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroDies1", new string[]
			{
				"the ouroboros has died.",
				"and yet, it will return."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroDies2", new string[]
			{
				"the serpent's wrath has been delayed.",
				"for now."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("OuroDies3", new string[]
			{
				"the ouroboros only grows stronger from death.",
				"it will be back."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("MycoFailSigils", new string[]
			{
				"a-ah, the sigils..."
			}, null, null, null, "DoctorIntro");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("MycoFailAttack", new string[]
			{
				"o-oh, the power..."
			}, null, null, null, "DoctorIntro");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("MycoFailHealth", new string[]
			{
				"a-ah, the health..."
			}, null, null, null, "DoctorIntro");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepIntro", new string[]
			 {
				"ah...",
				"Chrysomallos, the golden ram.",
				"what a glorious pelt...."
			 }, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom1", new string[]
			{
				"The golden ram.",
				"catch it before it escapes."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom3", new string[]
			{
				"a rare sight.",
				"it will not stay for long."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom4", new string[]
			{
				"what glittering wool,",
				"attached to such a rare creature."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom2", new string[]
			{
				"oh?",
				"a rare chance to get a rare pelt.",
				"best make the most of it."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("SheepDies1", new string[]
			{
				"You slay the glimmering creature, stealing its pelt."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("SheepDies2", new string[]
			{
				"The end of such a glorious creature.",
				"And what a glorious pelt you have obtained."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("SheepDies3", new string[]
			{
				"Chrysomallos...",
				"How tragic your tale is...",
				"To be killed for your pelt..."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("SheepEscapes1", new string[]
			{
				"The Golden Ram lives another day."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("SheepEscapes2", new string[]
			{
				"Time's up.",
				"The Golden Ram has found an escape route."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("SheepEscapes3", new string[]
			{
				"The glimmer of the Golden Ram's fur blinds you,",
				"giving it the opportunity to escape."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FragileEnemy", new string[]
			{
				"Ah...",
				"Your [v:0] has taken a devastating blow from the [v:1].",
				"You will not be able to take it with your caravan."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FragileDies", new string[]
			{
				"Ah...",
				"You won't be seeing your [v:0] again,",
				"It has taken too much damage."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FragileSacrifice", new string[]
			{
				"Ah...",
				"Did you permanently kill your [v:0] on purpose?",
				"A shame..."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("RoyalFirstMate", new string[]
			{
				"Ha! There be my first mate!"
			}, null, null, Emotion.Laughter, "PirateSkullPreCharge");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("RoyalOuro", new string[]
			{
				"Argh! There be snakes on me ship as well!"
			}, null, null, Emotion.Anger, "PirateSkullPreCharge");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("RoyalOuroDiesPlayer", new string[]
			{
				"Ha!",
				"I should hire ye to get rid of the rest of them!"
			}, null, null, Emotion.Neutral, "PirateSkullPreCharge");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("RoyalOuroDies", new string[]
			{
				"Ha! It's dead!"
			}, null, null, Emotion.Laughter, "PirateSkullPreCharge");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("PirateIntro", new string[]
			{
				"what?",
				"Pirates?",
				"...",
				"I'll allow it."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesIntro", new string[]
			{
				"what?",
				"cheating?",
				"how dissapointing...",
				"...",
				"the game will be soured from a pitifully easy ascent."
			}, null, null, Emotion.Anger, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesRepeat", new string[]
			{
				"cheater..."
			}, new string[][]
			{
				new string[]
				{
					"Have you no shame?"
				},
				new string[]
				{
					"To cheat so blatantly..."
				},
				new string[]
				{
					"you should be dead."
				},
				new string[]
				{
					"and yet..."
				},
				new string[]
				{
					"how dissapointing..."
				},
				new string[]
				{
					"..."
				}
			}, null, Emotion.Anger, "NewRunDealtDeckDefault"); 
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesLoop", new string[]
			 {
				"I won't tolerate this for much longer..."
			 }, null, null, Emotion.Anger, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesLoopBreak", new string[]
			 {
				"Enough of this."
			 }, null, null, Emotion.Anger, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesRoyal", new string[]
			{
				"Yer not dead?"
			}, null, null, Emotion.Curious, "PirateSkullPreCharge");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FecundityUnNerfIntro", new string[]
			{
				"ah...",
				"back to normal then?",
				"I'll admit, I had gotten used to the changes..."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("WeakStart", new string[]
			{
				"Weak Cards?",
				"This is hard to explain...",
				"Perhaps they are sick? Yes.",
				"A crippling disease afflicted your meager troupe of creatures."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("WaterborneStart", new string[]
			{
				"Waterborne Cards?",
				"This is hard to explain...",
				"Perhaps a mutation? Yes.",
				"Your caravan of creatures had a startling mutation,",
				"They could only survive in the water."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("ShockedStart", new string[]
			{
				"Paralyzed Cards?",
				"Hmm...",
				"Your group of creatures were fatigued from the long journey,",
				"but there would be a long way to go yet."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("WeakSoulStart", new string[]
			{
				"Weak Souled Cards?",
				"Hmm...",
				"You will not be able to extract their souls into new creatures,",
				"For better or worse..."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FamineIntro", new string[]
			{
				"Hm?",
				"How to explain this...",
				"You were running low on supplies that day...",
				"You'll have fewer [c:G][v:0]s[c:] to work with."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("AbundanceIntro", new string[]
			{
				"Hm?",
				"An abundance of [c:G][v:0]s[c:]...",
				"Must be mating season..."
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("EnvironmentsIntro", new string[]
			{
				"Hm?",
				"Environmental effects?",
				"how interesting...",
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("MudBoonIntro", new string[]
			{
				"In order to proceed,",
				"you had to slog through the wetter parts of the swamp.",
				"The thick [c:G]mud[c:] stuck to your boots, and hindered your movements..."
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("HailBoonIntro", new string[]
			{
				"The frigid air was not the only obstacle in your way,",
				"the harsh ice and unforgiving snow also stood in your path.",
				"before long you found yourself in...",
				"a [c:B]hailstorm.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("CliffsBoonIntro", new string[]
			{
				"You found yourself cornered against a sheer rock wall,",
				"the [c:G]cliffside.[c:]",
				"Against the cold stone, there would be less space to fight."
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("MushroomsBoonIntro", new string[]
			{
				"The mycologists had left one of their experiments behind,",
				"unbeknownst to them, it began to fester and grow.",
				"even the creatures fighting you would not be safe from the [c:G]fungal mass.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("DynamiteBoonIntro", new string[]
			{
				"You stumbled across one of the Prospector's Camps.",
				"The camp was filled with prospecting tools.",
				"pickaxes, headlamps...",
				"and [c:bR]dynamite.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("BaitBoonIntro", new string[]
			{
				"You came across one of the Angler's Ponds.",
				"the area stank of rotting fish,",
				"emanating from the nearby abandoned buckets of [c:dB]bait.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("TrapBoonIntro", new string[]
			{
				"You came across one of the Trapper's Hunting Grounds.",
				"the stench of blood hung in the air...",
				"you noticed the numerous [c:G]traps[c:] lying in wait."
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("TotemBoonIntro", new string[]
			{
				"You came upon a strange [c:bR]totem.[c:]",
				"a mysterious energy swirled around it,",
				"the creatures nearby seem more agressive than usual..."
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("BloodMoonBoonIntro", new string[]
			{
				"Ah...",
				"a [c:bR]Blood Moon.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("CarrotBoonIntro", new string[]
			{
				"Er...",
				"You...",
				"...",
				"...What?"
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("CarrotBoonIntro2", new string[]
			{
				"I am at a loss for words."
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("BlizzardBoonIntro", new string[]
			{
				"The wind was howling around you.",
				"Stuck in the middle of a blizzard,",
				"You heard rumbling from the mountains above.",
				"Here it comes...",
				"an [c:B]avalanche.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("ObeliskBoonIntro", new string[]
			{
				"You stumble across a strange black stone.",
				"A stone tablet sits in front of it, clearly made for sacrifices.",
				"Perhaps a certain creature may cause a reaction?"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GoatSacrifice", new string[]
			{
				"The obelisk trembles in delight.",
				"A goat is truly a worthy sacrifice.",
				"You won't be seeing it again."
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("PeltSacrifice", new string[]
			{
				"The obelisk rumbles with anger.",
				"A pelt is a truly pitiful sacrifice."
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("SquirrelSacrifice", new string[]
			{
				"...",
				"...That is not a proper sacrifice."
			}, new string[][]
			{
				new string[]
				{
					"Stop this."
				},
				new string[]
				{
					"You..."
				},
				new string[]
				{
					"This is bloodshed without meaning."
				},
				new string[]
				{
					"[v:0]..."
				}
			}, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("MinicelloBoonIntro", new string[]
			{
				"Hmm?",
				"What is this?"
			}, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("MinicelloBoonIntro2", new string[]
			{
				"Heh heh heh...",
				"Ye walked into th' pirate's cove!",
				"Me crew will get rid of ye quick!"
			}, null, null, Emotion.Laughter, "PirateSkullPreCharge");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("DarkForestBoonIntro", new string[]
			{
				"You carve through the thick underbrush and foilage.",
				"The trees blotting out the sun...",
				"As you travel, you hear the woods start to creak around you.",
				"You should know better than to walk in the darkness..."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FloodBoonIntro", new string[]
			{
				"As your caravan of creatures travels, you hear a rushing sound.",
				"You climb to a higher place as water flows around you.",
				"You are caught in a [c:B]flood.[c:]",
				"Only the terrain and flying creatures will be able to avoid the waters."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("BreezeBoonIntro", new string[]
			{
				"As your caravan of creatures moves across a clearing, the winds blow stronger.",
				"A strong breeze greets your face, and you grasp onto the surroundings for dear life.",
				"Your creatures are blown [c:G]airborne.[c:]",
				"Only burrowing and submerged creatures will be able to avoid the gusts."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("BreezeActivation", new string[]
			{
				"The Breeze blows..."
			}, null, null, null, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GraveyardBoonIntro", new string[]
			{
				"You come across a pile of corpses.",
				"A strange energy swirls around them as you approach.",
				"Then...",
				"[c:R]The dead walk.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("GraveyardBoonIntro2", new string[]
			{
				"[c:R]The dead walk.[c:]"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FlashGrowthBoonIntro", new string[]
			{
				"You came across an overgrown glade.",
				"The trees seemed taller, and stronger than usual.",
				"Your creatures grew faster as well.",
				"<color=#25C102>Flash Growth.</color>"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
			Division.Helpers.DialogueHelper.AddOrModifySimpleDialogEvent("FlashGrowthBoonIntro2", new string[]
			{
				"<color=#25C102>Flash Growth.</color>"
			}, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");



			///----------------P03 Lines---------------
			///P03 faces:
			///		Default
			///		Bored
			///		Angry
			///		Happy
			///		Thinking
			Division.Helpers.DialogueHelper.AddDialogue("P03FamineIntro",
			new List<string> //dialogue
            {
				"Hm?",
				"You're decreasing the number of vessels you have?",
				"Surely a player as bad as you needs the extra chump blockers?",
				"You'll see."
			}, new List<string> //faces
            {
				"Default",
				"",
				"Happy",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"y",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03AbundanceIntro",
			new List<string> //dialogue
            {
				"Hm?",
				"You're increasing the number of vessels you have?",
				"Makes sense.",
				"After all, a player as bad as you needs the extra chump blockers.",
				"[v:0] vessels..."
			}, new List<string> //faces
            {
				"Default",
				"",
				"",
				"Happy",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"",
				"y",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03FecundityUnNerfIntro",
			new List<string> //dialogue
            {
				"Oh?",
				"You couldn't even stick to the changes?",
				"Pathetic."
			}, new List<string> //faces
            {
				"Default",
				"Happy",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03OuroIntro",
			new List<string> //dialogue
            {
				"Oh?",
				"My own Ourobot?",
				"You must be masochistic if you thought this was a good idea.",
				"Your funeral."
			}, new List<string> //faces
            {
				"Thinking",
				"Default",
				"Happy",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03OuroDies1",
			new List<string> //dialogue
            {
				"There it goes."
			}, new List<string> //faces
            {
				"Default"
			}, new List<string> //dialogue wavies
			{
				""
			}); 
			Division.Helpers.DialogueHelper.AddDialogue("P03OuroDies2",
			new List<string> //dialogue
            {
				"You just made it stronger.",
				"Of course, you knew that already."
			}, new List<string> //faces
            {
				"Default",
				"Happy"
			}, new List<string> //dialogue wavies
			{
				"",
				""
			}); 
			Division.Helpers.DialogueHelper.AddDialogue("P03OuroDies3",
			new List<string> //dialogue
            {
				"I'm not worried.",
				"It'll come back to crush you later."
			}, new List<string> //faces
            {
				"Default",
				"Happy"
			}, new List<string> //dialogue wavies
			{
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03WeakStart",
			new List<string> //dialogue
            {
				"Hm?",
				"Weaker starting cards?",
				"As if one health will make a difference."
			}, new List<string> //faces
            {
				"Default",
				"",
				""
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03EnvironmentsIntro",
			new List<string> //dialogue
            {
				"Hm?",
				"Were my environments not good enough for you?",
				"Fine."
			}, new List<string> //faces
            {
				"Default",
				"",
				""
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03GraveyardBoonIntro",
			new List<string> //dialogue
            {
				"Hm...",
				"You have found a...",
				"Robot scrapyard.",
				"There's broken down robots everywhere.",
				"Every robot in this area dies twice."
			}, new List<string> //faces
            {
				"Thinking",
				"Default",
				"",
				"",
				""
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03GraveyardBoonIntro2",
			new List<string> //dialogue
            {
				"It's the scrapyard again.",
				"You know the drill."
			}, new List<string> //faces
            {
				"Default",
				""
			}, new List<string> //dialogue wavies
			{
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03FlashGrowthBoonIntro",
			new List<string> //dialogue
            {
				"Eugh.",
				"This is one of [c:O]HIS.[c:]",
				"Your... transformer bots will be more effective here.",
				"They'll transform when played.",
				"You'll see."
			}, new List<string> //faces
            {
				"Angry",
				"",
				"Default",
				"",
				""
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03FlashGrowthBoonIntro2",
			new List<string> //dialogue
            {
				"Your transformer bots will be more effective here.",
				"They'll tranform when played."
			}, new List<string> //faces
            {
				"Default",
				""
			}, new List<string> //dialogue wavies
			{
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03ConveyorBoonIntro",
			new List<string> //dialogue
            {
				"Ah...",
				"A favorite of mine.",
				"At the start of each of your turns all cards will be moved clockwise.",
				"I'm sure you've seen it before."
			}, new List<string> //faces
            {
				"Happy",
				"",
				"",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03ConveyorBoonIntro2",
			new List<string> //dialogue
            {
				"At the start of each of your turns all cards will be moved clockwise.",
				"You've seen it before."
			}, new List<string> //faces
            {
				"Default",
				""
			}, new List<string> //dialogue wavies
			{
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03GemSanctuaryBoonIntro",
			new List<string> //dialogue
            {
				"Ah...",
				"Your <color=#25C102>G</color>[c:O]E[c:][c:B]M[c:]s will be more useful.",
				"More useful than [c:R]he[c:] ever was able to make them...",
				"...As long as you keep that one alive."
			}, new List<string> //faces
            {
				"Thinking",
				"Default",
				"Angry",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03GemSanctuaryBoonIntro2",
			new List<string> //dialogue
            {
				"Your <color=#25C102>G</color>[c:O]E[c:][c:B]M[c:]s will be more useful.",
				"...As long as you keep that one alive."
			}, new List<string> //faces
            {
				"Default",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03ElectricStormBoonIntro",
			new List<string> //dialogue
            {
				"Heh.",
				"This one will be quite...",
				"Shocking.",
				"You find yourself in an electrical storm.",
				"When a card is played, it will take 1 damage.",
				"If it survives, it'll be stronger for a bit."
			}, new List<string> //faces
            {
				"Happy",
				"",
				"Happy",
				"",
				"",
				"Default"
			}, new List<string> //dialogue wavies
			{
				"",
				"",
				"",
				"",
				"",
				""
			});
			Division.Helpers.DialogueHelper.AddDialogue("P03ElectricStormBoonIntro2",
			new List<string> //dialogue
            {
				"I think you'll find this one quite...",
				"Shocking."
			}, new List<string> //faces
            {
				"Default",
				"Happy"
			}, new List<string> //dialogue wavies
			{
				"",
				""
			});
		}
	}
}
