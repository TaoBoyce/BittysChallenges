using DiskCardGame;
using InscryptionAPI.Boons;
using InscryptionAPI.Card;
using InscryptionAPI.Slots;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static BittysChallenges.Abilities;
using static BittysChallenges.Plugin;

namespace BittysChallenges
{
    public class Boons
    {
        public static void AddBoons()
        {
            Log.LogInfo("Start of boons");
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
            Add_Boon_GemSanctuary();
            Add_Boon_ElectricalStorm();
            EnviroHandler.Init_EnviroBoonList();
            Log.LogInfo("End of boons");
        }
        #region Boon Loaders
        private static void Add_Boon_Mud()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_mud");
            Texture boonCardArt = Tools.LoadTexture("boon_swamp");
            BoonData.Type mudBoon = BoonManager.New(PluginGuid + ".mud", "Environment: Mud Swamp", typeof(ChallengeBoonMud), "You will start the battle with Mud on some of your spaces.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonMud.boo = mudBoon;
        }
        private static void Add_Boon_Hail()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_hail");
            Texture boonCardArt = Tools.LoadTexture("boon_snowtrees");
            BoonData.Type hailBoon = BoonManager.New(PluginGuid + ".hail", "Environment: Hail Storm", typeof(ChallengeBoonHail), "At the start of each turn, all of the turn owner's non-terrain cards take 1 damage.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonHail.boo = hailBoon;
        }
        private static void Add_Boon_Cliff()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_cliff");
            Texture boonCardArt = Tools.LoadTexture("boon_cliffs");
            BoonData.Type cliffBoon = BoonManager.New(PluginGuid + ".cliff", "Environment: Cliffside", typeof(ChallengeBoonCliffs), "At the start of the battle, the leftmost lane will be blocked with Cliffs.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonCliffs.boo = cliffBoon;
        }
        private static void Add_Boon_Mushrooms()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_fungus");
            Texture boonCardArt = Tools.LoadTexture("boon_mushrooms");
            BoonData.Type mushroomsBoon = BoonManager.New(PluginGuid + ".mushrooms", "Environment: Fungal Field", typeof(ChallengeBoonMushrooms), "The opponent will start the battle with Mushrooms.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonMushrooms.boo = mushroomsBoon;
        }
        private static void Add_Boon_Dynamite()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_dynamite");
            Texture boonCardArt = Tools.LoadTexture("boon_startingtrees");
            BoonData.Type dynamiteBoon = BoonManager.New(PluginGuid + ".dynamite", "Environment: Prospector's Camp", typeof(ChallengeBoonDynamite), "You will start the battle with Dynamite on some of your spaces.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonDynamite.boo = dynamiteBoon;
        }
        private static void Add_Boon_Bait()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_bait");
            Texture boonCardArt = Tools.LoadTexture("boon_swamp");
            BoonData.Type baitBoon = BoonManager.New(PluginGuid + ".bait", "Environment: Angler's Pond", typeof(ChallengeBoonBait), "The opponent will start the battle with Bait Buckets.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonBait.boo = baitBoon;
        }
        private static void Add_Boon_Trap()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_trap");
            Texture boonCardArt = Tools.LoadTexture("boon_snowtrees");
            BoonData.Type trapBoon = BoonManager.New(PluginGuid + ".trap", "Environment: Trapper's Hunting Grounds", typeof(ChallengeBoonTrap), "The opponent will start the battle with Steel Traps.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonTrap.boo = trapBoon;
        }
        private static void Add_Boon_Totem()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_totem");
            Texture boonCardArt = Tools.LoadTexture("boon_startingtrees");
            BoonData.Type totemBoon = BoonManager.New(PluginGuid + ".totem", "Environment: Cursed Totem", typeof(ChallengeBoonTotem), "The opponent will start the battle with Cursed Totems.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonTotem.boo = totemBoon;
        }
        private static void Add_Boon_BloodMoon()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_Blood_Moon");
            Texture boonCardArt = Tools.LoadTexture("boon_Blood_Moon");
            BoonData.Type bloodMoonBoon = BoonManager.New(PluginGuid + ".bloodmoon", "Environment: Blood Moon", typeof(ChallengeBoonBloodMoon), "The opponent will start the battle with Dire Wolf Pups.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonBloodMoon.boo = bloodMoonBoon;
        }
        private static void Add_Boon_CarrotPatch()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_carrot");
            Texture boonCardArt = Tools.LoadTexture("boon_Blood_Moon");
            BoonData.Type carrotPatchBoon = BoonManager.New(PluginGuid + ".carrot", "Environment: Carrot Patch", typeof(ChallengeBoonCarrotPatch), "The opponent will start the battle with Rabbits.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonCarrotPatch.boo = carrotPatchBoon;
        }
        private static void Add_Boon_Blizzard()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_blizzard");
            Texture boonCardArt = Tools.LoadTexture("boon_snowtrees");
            BoonData.Type blizzardBoon = BoonManager.New(PluginGuid + ".blizzard", "Environment: Blizzard", typeof(ChallengeBoonBlizzard), "At the start of each turn, if there is not an Avalanche present on the board, one will be created on the left-most side of the board.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonBlizzard.boo = blizzardBoon;
        }
        private static void Add_Boon_Obelisk()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_obelisk");
            Texture boonCardArt = Tools.LoadTexture("boon_voidaura");
            BoonData.Type obeliskBoon = BoonManager.New(PluginGuid + ".obelisk", "Environment: Obelisk", typeof(ChallengeBoonObelisk), "The opponent will start the battle with an Obelisk.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonObelisk.boo = obeliskBoon;
        }
        private static void Add_Boon_Minicello()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_pirate");
            Texture boonCardArt = Tools.LoadTexture("boon_pirate");
            BoonData.Type minicelloBoon = BoonManager.New(PluginGuid + ".minicello", "Environment: Pirate's Hollow", typeof(ChallengeBoonMinicello), "The opponent will start the battle with a Minicello.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonMinicello.boo = minicelloBoon;
        }
        private static void Add_Boon_DarkForest()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_tree");
            Texture boonCardArt = Tools.LoadTexture("boon_startingtrees");
            BoonData.Type darkForestBoon = BoonManager.New(PluginGuid + ".darkForest", "Environment: Dark Forest", typeof(ChallengeBoonDarkForest), "The opponent will start the battle with Trees. All of the opponent's terrain have +1 power.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonDarkForest.boo = darkForestBoon;
        }
        private static void Add_Boon_Flood()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_flood");
            Texture boonCardArt = Tools.LoadTexture("boon_flood");
            BoonData.Type floodBoon = BoonManager.New(PluginGuid + ".flood", "Environment: Flood", typeof(ChallengeBoonFlood), "Whenever a creature is played, it gains Waterborne. Terrain, and creatures with Airborne are ignored.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonFlood.boo = floodBoon;
        }
        private static void Add_Boon_Breeze()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
            Texture boonCardArt = Tools.LoadTexture("boon_breeze");
            BoonData.Type breezeBoon = BoonManager.New(PluginGuid + ".breeze", "Environment: Breeze", typeof(ChallengeBoonBreeze), "At the start of every turn, all creatures gain or lose Airborne. Terrain, and creatures with Waterborne or Burrower are ignored.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonBreeze.boo = breezeBoon;
        }
        private static void Add_Boon_Graveyard()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_graveyard");
            Texture boonCardArt = Tools.LoadTexture("boon_graveyard");
            BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Graveyard", typeof(ChallengeBoonGraveyard), "When a creature dies, it dies again.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonGraveyard.boo = boon;
        }
        private static void Add_Boon_FlashGrowth()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_flashgrowth");
            Texture boonCardArt = Tools.LoadTexture("boon_flashgrowth");
            BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Flash Growth", typeof(ChallengeBoonFlashGrowth), "When a card is played, any sigils that activate at the start of the turn are activated.", boonRulebookIcon, boonCardArt, false, false, true);
            ChallengeBoonFlashGrowth.boo = boon;
        }
        private static void Add_Boon_GemSanctuary()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
            Texture boonCardArt = Tools.LoadTexture("boon_blank");
            BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Gem Sanctuary", typeof(ChallengeBoonGemSanctuary), "All gems have +1 power.", boonRulebookIcon, boonCardArt, false, false, false);
            ChallengeBoonGemSanctuary.boo = boon;
        }
        private static void Add_Boon_ElectricalStorm()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
            Texture boonCardArt = Tools.LoadTexture("boon_blank");
            BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Electrical Storm", typeof(ChallengeBoonElectricStorm), "When a card is played, it takes 1 damage and gains 1 power.", boonRulebookIcon, boonCardArt, false, false, false);
            ChallengeBoonElectricStorm.boo = boon;
        }
        #endregion
        #region Boons
        public abstract class ChallengeBoonBase : BoonBehaviour
        {
            protected abstract BoonData.Type boonType { get; }
            public override bool RespondsToPostBoonActivation()
            {
                return true;
            }
            public override IEnumerator OnPostBoonActivation()
            {
                if (modModeActive(MOD_MODE.KCM) && Singleton<BoonsHandler>.Instance.HasBoonOfType(boonType))
                {
                    Singleton<ViewManager>.Instance.SwitchToView(View.Default);
                    yield return Singleton<BoonsHandler>.Instance.PlayBoonAnimation(boonType);
                }
                string P03 = modModeActive(MOD_MODE.P03) ? "P03" : "";
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
            private static int randomMod = 0;
            public List<CardSlot> SelectRandomSlots(int amount, List<CardSlot> slotsCopy)
            {
                List<CardSlot> slots = new List<CardSlot>();
                randomMod += amount;
                int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed() + randomMod;
                for (int i = 0; i < amount; i++)
                {
                    if (i >= slotsCopy.Count)
                    {
                        return slots;
                    }
                    int randomSlot = SeededRandom.Range(0, slotsCopy.Count, currentRandomSeed*i);
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
                    if (!DialogueEventsData.EventIsPlayed("MudBoonIntro"))
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

                List<CardSlot> slots = SelectRandomSlots(Math.Max((playerSlotsCopy.Count + 1) / 2, 1), playerSlotsCopy);

                for (int i = 0; i < slots.Count; i++)
                {

                    yield return slots[i].FadeToSlotMod(SlotMods.SlotMod_Muddy.SlotType);

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
                    if (!DialogueEventsData.EventIsPlayed("HailBoonIntro"))
                    {
                        yield return new WaitForSeconds(0.7f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("HailBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }

                List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

                List<CardSlot> slotsO = SelectRandomSlots(3 - RunState.CurrentRegionTier, opponentSlotsCopy);

                for (int i = 0; i < slotsO.Count; i++)
                {
                    yield return slotsO[i].FadeToSlotMod(SlotMods.SlotMod_Hail.SlotType);
                }


                List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
                playerSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

                int playerHail = 2;
                if(RunState.CurrentRegionTier >= 2)
                {
                    playerHail = 3;
                }
                List<CardSlot> slotsP = SelectRandomSlots(playerHail, playerSlotsCopy);

                for (int i = 0; i < slotsP.Count; i++)
                {
                    yield return slotsP[i].FadeToSlotMod(SlotMods.SlotMod_Hail.SlotType);
                }
                yield break;
            }
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
                    if (!DialogueEventsData.EventIsPlayed("CliffsBoonIntro"))
                    {
                        yield return new WaitForSeconds(0.7f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("CliffsBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
                List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;

                CardInfo cardInfo = CardLoader.GetCardByName("bitty_Cliff");
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(cardInfo, opponentSlotsCopy[0]);
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(cardInfo, opponentSlotsCopy[0].opposingSlot);

                yield return Singleton<Opponent>.Instance.QueueCard(cardInfo, opponentSlotsCopy[0], true, false);
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

                List<CardSlot> slots = SelectRandomSlots(1, opponentSlotsCopy);

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
                    if (!DialogueEventsData.EventIsPlayed("DynamiteBoonIntro"))
                    {
                        yield return new WaitForSeconds(0.7f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("DynamiteBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }

                List<CardSlot> allSlotsCopy = Singleton<BoardManager>.Instance.AllSlotsCopy;
                allSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

                List<CardSlot> slots = SelectRandomSlots(4, allSlotsCopy);

                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].FadeToSlotMod(SlotMods.SlotMod_Dynamite.SlotType);
                }

                if (RunState.CurrentRegionTier >= 2)
                {
                    List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
                    playerSlotsCopy.RemoveAll((CardSlot x) => x.GetSlotModification() == SlotModificationManager.ModificationType.NoModification);
                    for (int i = 0; i < playerSlotsCopy.Count; i++)
                    {
                        yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("GoldNugget"), playerSlotsCopy[i].opposingSlot);
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
                    if (!DialogueEventsData.EventIsPlayed("BaitBoonIntro"))
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

                List<CardSlot> slots = SelectRandomSlots(Math.Max((opponentSlotsCopy.Count + 1) / 2, 1), opponentSlotsCopy);

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
                        yield return opponentSlotsCopy2[i].opposingSlot.FadeToSlotMod(SlotMods.SlotMod_Muddy.SlotType);
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
                    if (!DialogueEventsData.EventIsPlayed("TrapBoonIntro"))
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

                List<CardSlot> slots = SelectRandomSlots(Math.Max((opponentSlotsCopy.Count + 1) / 2, 1), opponentSlotsCopy);

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
                    if (!DialogueEventsData.EventIsPlayed("TotemBoonIntro"))
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

                List<CardSlot> slots = SelectRandomSlots((opponentSlotsCopy.Count / 5) + 1, opponentSlotsCopy);

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
                    if (!DialogueEventsData.EventIsPlayed("BlizzardBoonIntro"))
                    {
                        yield return new WaitForSeconds(0.7f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BlizzardBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }
                List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;

                CardInfo cardInfo = CardLoader.GetCardByName("bitty_Avalanche");
                if (playerSlotsCopy[0].Card != null)
                {
                    yield return playerSlotsCopy[0].Card.TakeDamage(10, null);
                }
                if (playerSlotsCopy[0].opposingSlot.Card != null)
                {
                    yield return playerSlotsCopy[0].opposingSlot.Card.TakeDamage(10, null);
                }
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(cardInfo, playerSlotsCopy[0]);
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(cardInfo, playerSlotsCopy[0].opposingSlot);
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
                    if (slot.Card != null && slot.Card.Info.name == "bitty_Avalanche")
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

                    CardInfo cardInfo = CardLoader.GetCardByName("bitty_Avalanche");
                    if (playerSlotsCopy[0].Card != null)
                    {
                        yield return playerSlotsCopy[0].Card.TakeDamage(10, null);
                    }
                    if (playerSlotsCopy[0].opposingSlot.Card != null)
                    {
                        yield return playerSlotsCopy[0].opposingSlot.Card.TakeDamage(10, null);
                    }
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(cardInfo, playerSlotsCopy[0]);
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(cardInfo, playerSlotsCopy[0].opposingSlot);
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

                List<CardSlot> slots = SelectRandomSlots(1, opponentSlotsCopy);

                for (int i = 0; i < slots.Count; i++)
                {
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_Obelisk"), slots[i]);
                    if (slots[i].opposingSlot.Card != null)
                    {
                        yield return slots[i].opposingSlot.Card.Die(false, null, false);
                    }
                    yield return slots[i].opposingSlot.FadeToSlotMod(SlotMods.SlotMod_Obelisk.SlotType);
                }

                if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
                {
                    if (!DialogueEventsData.EventIsPlayed("ObeliskBoonIntro"))
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

                List<CardSlot> slots = SelectRandomSlots(1, opponentSlotsCopy);

                for (int i = 0; i < slots.Count; i++)
                {
                    CardInfo card = CardLoader.GetCardByName("bitty_Minicello");
                    card.mods.Add(new CardModificationInfo(Ability.AllStrike));
                    card.mods.Add(new CardModificationInfo(0, 3));
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card, slots[i]);
                }

                if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
                {
                    yield return new WaitForSeconds(0.7f);
                    Singleton<ViewManager>.Instance.SwitchToView(View.Board);
                    if (!DialogueEventsData.EventIsPlayed("MinicelloBoonIntro"))
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MinicelloBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("MinicelloBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    Singleton<ViewManager>.Instance.SwitchToView(View.Default);
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

                List<CardSlot> slots = SelectRandomSlots(terrainCards, opponentSlotsCopy);

                for (int i = 0; i < slots.Count; i++)
                {
                    CardInfo card = CardLoader.GetCardByName("Tree");
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card, slots[i]);
                }

                if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed("EnvironmentsIntro"))
                {
                    if (!DialogueEventsData.EventIsPlayed("DarkForestBoonIntro"))
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
                    if (!DialogueEventsData.EventIsPlayed("FloodBoonIntro"))
                    {
                        yield return new WaitForSeconds(0.7f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("FloodBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                        yield return new WaitForSeconds(0.7f);
                    }
                }
                for (int i = 0; i < 3 - RunState.CurrentRegionTier; i++)
                {
                    yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(CardLoader.GetCardByName("bitty_Raft"));
                    yield return new WaitForSeconds(0.2f);
                }

                List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

                List<CardSlot> opponentSlots = SelectRandomSlots(2, opponentSlotsCopy);

                for (int i = 0; i < opponentSlots.Count; i++)
                {
                    yield return opponentSlots[i].FadeToSlotMod(SlotMods.SlotMod_Flood.SlotType);
                }

                List<CardSlot> playerSlots = Singleton<BoardManager>.Instance.AllSlotsCopy;
                playerSlots.RemoveAll((CardSlot x) => x.Card != null);

                for (int i = 0; i < playerSlots.Count; i++)
                {
                    yield return playerSlots[i].FadeToSlotMod(SlotMods.SlotMod_Flood.SlotType);
                }
                yield break;
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
                    if (!DialogueEventsData.EventIsPlayed("BreezeBoonIntro"))
                    {
                        yield return new WaitForSeconds(0.7f);
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BreezeBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                }

                List<CardSlot> allSlotsCopy = Singleton<BoardManager>.Instance.AllSlotsCopy;
                allSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

                List<CardSlot> slots = SelectRandomSlots(6, allSlotsCopy);

                for (int i = 0; i < slots.Count; i++)
                {
                    yield return slots[i].FadeToSlotMod(SlotMods.SlotMod_Breeze.SlotType);
                }
                yield break;
            }
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
                string P03 = modModeActive(MOD_MODE.P03) ? "P03" : "";
                if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro"))
                {
                    yield return new WaitForSeconds(0.7f);
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "GraveyardBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                }

                if (modModeActive(MOD_MODE.KCM) && RunState.CurrentRegionTier >= 1)
                {
                    List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                    opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

                    List<CardSlot> slots = SelectRandomSlots(Math.Max((opponentSlotsCopy.Count + 1) / 2, 1), opponentSlotsCopy);

                    for (int i = 0; i < slots.Count; i++)
                    {
                        string name = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + i) ? "bitty_SkeletonPirate" : "Amoeba";
                        yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), slots[i]);
                    }
                }
                else if (modModeActive(MOD_MODE.P03))
                {
                    List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                    opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card != null);

                    List<CardSlot> slots = SelectRandomSlots(Math.Max((opponentSlotsCopy.Count + 1) / 2, 1), opponentSlotsCopy);

                    for (int i = 0; i < slots.Count; i++)
                    {
                        string name = SeededRandom.Bool(SaveManager.SaveFile.GetCurrentRandomSeed() + i) ? "BrokenBot" : "Amoebot";
                        yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), slots[i]);
                    }
                }
                List<CardSlot> allSlotsCopy = Singleton<BoardManager>.Instance.AllSlotsCopy;
                for(int i = 0; i < allSlotsCopy.Count; i++)
                {
                    yield return allSlotsCopy[i].FadeToSlotMod(SlotMods.SlotMod_Grave.SlotType);
                }
                yield break;
            }
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
                string P03 = modModeActive(MOD_MODE.P03) ? "P03" : "";
                if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro"))
                {
                    yield return new WaitForSeconds(0.7f);
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "FlashGrowthBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                }

                List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                List<CardSlot> slots = SelectRandomSlots(RunState.CurrentRegionTier+1, opponentSlotsCopy);
                for(int i = 0; i < slots.Count; i++)
                {
                    yield return slots[i].FadeToSlotMod(SlotMods.SlotMod_Growth.SlotType);
                }

                List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
                List<CardSlot> slots2 = SelectRandomSlots(2, playerSlotsCopy);
                for (int i = 0; i < slots2.Count; i++)
                {
                    yield return slots2[i].FadeToSlotMod(SlotMods.SlotMod_Growth.SlotType);
                }

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

                List<CardSlot> slots = SelectRandomSlots(Math.Max((emptySlots.Count + 1) / 2, 1), emptySlots);
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

                slots = SelectRandomSlots(1, emptySlots);

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
                if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro"))
                {
                    yield return new WaitForSeconds(0.7f);
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "GemSanctuaryBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
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
                if (SaveFile.IsAscension && DialogueEventsData.EventIsPlayed(P03 + "EnvironmentsIntro"))
                {
                    yield return new WaitForSeconds(0.7f);
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(P03 + "ElectricStormBoonIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, new Action<DialogueEvent.Line>(Dialogue.P03HappyCloseUp));
                }

                List<CardSlot> allSlotsCopy = Singleton<BoardManager>.Instance.AllSlotsCopy;
                List<CardSlot> slots = SelectRandomSlots(6, allSlotsCopy);
                for (int i = 0; i < slots.Count; i++)
                {
                    yield return slots[i].FadeToSlotMod(SlotMods.SlotMod_Overclock.SlotType);
                }
                yield break;
            }
        }
        #endregion
    }
}
