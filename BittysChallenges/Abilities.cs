using BittysSigils;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers.Extensions;
using InscryptionAPI.RuleBook;
using InscryptionAPI.Slots;
using InscryptionAPI.Triggers;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static BittysChallenges.Plugin;
using static UnityEngine.GraphicsBuffer;
using Object = UnityEngine.Object;

namespace BittysChallenges
{
    [HarmonyPatch]
    public class Abilities
    {
        public static void AddAbilities()
        {
            Log.LogInfo("Start of sigils");
            Add_Ability_FalseUnkillable();
            Add_Ability_Warper();
            Add_Ability_Fragile();
            Add_Ability_Paralysis();
            Add_Ability_StrafeKiller();
            Add_Ability_StrafeAvalanche();
            Add_Ability_Raft();
            Add_Ability_SlotSpawner();

            Add_Ability_RedChamp();
            Add_Ability_YellowChamp();
            Add_Ability_GreenChamp();
            Add_Ability_OrangeChamp();
            Add_Ability_CyanChamp();
            Add_Ability_WhiteChamp();
            Add_Ability_MagentaChamp();
            Add_Ability_PurpleChamp();
            Add_Ability_BlueChamp();
            Add_Ability_LightBlueChamp();
            Add_Ability_LightGreenChamp();
            Add_Ability_BrightRedChamp();
            Log.LogInfo("End of sigils");
        }
        #region Ability Loaders
        private static void Add_Ability_SlotSpawner()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Slot Spawner",
                "When played, spawns a slot. Used for testing.",
                typeof(GiveSlotSpawner),
                Tools.LoadTexture("ability_test.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 1;

            // Pass the ability to the API.
            GiveSlotSpawner.ability = abilityInfo.ability;
        }
        private static void Add_Ability_Raft()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Seaworthy",
                "When played, converts the slot into a Raft.",
                typeof(GiveRaft),
                Tools.LoadTexture("ability_raft.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            .SetSlotRedirect("Raft", SlotMods.SlotMod_Raft.SlotType, GameColors.Instance.orange)
            ;
            abilityInfo.powerLevel = 1;

            // Pass the ability to the API.
            GiveRaft.ability = abilityInfo.ability;
        }
        private static void Add_Ability_Warper()
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
        private static void Add_Ability_Fragile()
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
        private static void Add_Ability_FalseUnkillable()
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
        private static void Add_Ability_Paralysis()
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
            
            // Pass the ability to the API.
            GiveParalysis.ability = abilityInfo.ability;
        }
        private static void Add_Ability_StrafeKiller()
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
        private static void Add_Ability_StrafeAvalanche()
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
        //CHAMPION ABILITIES ------------------------------------------------
        #region Champ Abilities
        private static void Add_Ability_RedChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Red Champion",
                "This champion starts with 2 additional Health.",
                typeof(GiveRedChamp),
                Tools.LoadTexture("ability_redChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveRedChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_YellowChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Yellow Champion",
                "This champion starts with 1 additional Power.",
                typeof(GiveYellowChamp),
                Tools.LoadTexture("ability_yellowChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveYellowChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_GreenChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Green Champion",
                "This champion will deal 1 damage directly to you, after it destroys another creature.",
                typeof(GiveGreenChamp),
                Tools.LoadTexture("ability_greenChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveGreenChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_OrangeChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Orange Champion",
                "This champion will steal 2 golden teeth from you if it damages you directly.",
                typeof(GiveOrangeChamp),
                Tools.LoadTexture("ability_orangeChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveOrangeChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_CyanChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Cyan Champion",
                "When this champion perishes, it will deal 1 damage to every other creature on the board.",
                typeof(GiveCyanChamp),
                Tools.LoadTexture("ability_cyanChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveCyanChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_WhiteChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "White Champion",
                "When this champion perishes, it will create a Boulder in its space. [define:Boulder]",
                typeof(GiveWhiteChamp),
                Tools.LoadTexture("ability_whiteChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveWhiteChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_MagentaChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Magenta Champion",
                "At the start of each turn, this champion will attack for 1 damage in a random space on your side.",
                typeof(GiveMagentaChamp),
                Tools.LoadTexture("ability_magentaChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveMagentaChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_PurpleChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Purple Champion",
                "Each time you play a card, if the space opposing this champion is empty, that card will move to that space.",
                typeof(GivePurpleChamp),
                Tools.LoadTexture("ability_purpleChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GivePurpleChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_BlueChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Blue Champion",
                "This champion will cause all other non-Terrain cards on the same side to gain flight, as long as the champion is alive.",
                typeof(GiveBlueChamp),
                Tools.LoadTexture("ability_blueChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveBlueChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_LightBlueChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Light Blue Champion",
                "When this champion perishes, it will deal 3 damage to creatures to the left and right of it as well as the opposing creature.",
                typeof(GiveLightBlueChamp),
                Tools.LoadTexture("ability_lightBlueChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveLightBlueChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_LightGreenChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Light Green Champion",
                "This champion will prevent all damage from the first strike dealt to it. Damage from effects like sigils will not be prevented.",
                typeof(GiveLightGreenChamp),
                Tools.LoadTexture("ability_lightGreenChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveLightGreenChamp.ability = abilityInfo.ability;
        }
        private static void Add_Ability_BrightRedChamp()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Bright Red Champion",
                "At the start of each turn, this champion will heal all other non-terrain creatures on the opponent's side by 1.",
                typeof(GiveBrightRedChamp),
                Tools.LoadTexture("ability_brightRedChampion.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 5;

            // Pass the ability to the API.
            GiveBrightRedChamp.ability = abilityInfo.ability;
        }
        #endregion
        #endregion
        #region Abilities
        public class GiveSlotSpawner : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveSlotSpawner.ability;
                }
            }
            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
            public override IEnumerator OnResolveOnBoard()
            {
                List<CardSlot> playerSlots = Singleton<BoardManager3D>.Instance.PlayerSlotsCopy;
                for(int i = 0; i < playerSlots.Count; i++)
                {
                    if (i == 0) { yield return playerSlots[0].SetSlotModification(SlotMods.SlotMod_Flood.SlotType); }
                    if (i == 0) { yield return playerSlots[1].SetSlotModification(SlotMods.SlotMod_Growth.SlotType); }
                    if (i == 0) { yield return playerSlots[2].SetSlotModification(SlotMods.SlotMod_Obelisk.SlotType); }
                }
                base.Card.Anim.PlayDeathAnimation(false);
                Object.Destroy(base.Card.gameObject);
                yield break;
            }

            public static Ability ability;
        }
        public class GiveRaft : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return GiveRaft.ability;
                }
            }
            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
            public override IEnumerator OnResolveOnBoard()
            {
                yield return Card.Slot.SetSlotModification(SlotMods.SlotMod_Raft.SlotType);
                base.Card.Anim.PlayDeathAnimation(false);
                Object.Destroy(base.Card.gameObject);
                yield break;
            }

            public static Ability ability;
        }
        public class AddCloverReRollAbility : SpecialCardBehaviour, IOnOtherCardResolveInHand
        {
            public readonly static SpecialTriggeredAbility CloverReRollSpecialAbility = SpecialTriggeredAbilityManager.Add(PluginGuid, "CloverReRollSpecialAbility", typeof(AddCloverReRollAbility)).Id;

            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
            public override IEnumerator OnResolveOnBoard()
            {
                yield return MoveCardIntoDeck(Singleton<PlayerHand>.Instance.cardsInHand[0], true);
                int cardsToDraw = 0;
                int cardsToShuffle = Singleton<PlayerHand>.Instance.cardsInHand.Count;
                for (int i = 0; i < cardsToShuffle; i++)
                {
                    cardsToDraw++;
                    yield return MoveCardIntoDeck(Singleton<PlayerHand>.Instance.cardsInHand[0]);
                }

                if (CardDrawPiles3D.Instance.SideDeck.CardsInDeck > 0)
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
                Challenges.MiscEncounters.IncreaseOuro();

                string zoom = "OuroDies";

                if (Singleton<Opponent>.Instance.OpponentType == Opponent.Type.PirateSkullBoss)
                {
                    if (killer != null && killer.OpponentCard == false)
                    {
                        zoom = "RoyalOuroDiesPlayer";
                    }
                    else
                    {
                        zoom = "RoyalOuroDies";
                    }
                }
                if (IsP03Run)
                {
                    zoom = "P03OuroDies";
                }
                yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent(zoom, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                yield break;
            }
        }
        public class AddGoldenSheepAbility : SpecialCardBehaviour
        {
            public readonly static SpecialTriggeredAbility GoldenSheepSpecialAbility = SpecialTriggeredAbilityManager.Add(PluginGuid, "GoldenSheep Special Ability", typeof(AddGoldenSheepAbility)).Id;


            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
                return true;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
                Challenges.MiscEncounters.GoldenSheepKill();
                yield return this.GiveWool(true);
                yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("SheepDies", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                yield break;
            }
            private IEnumerator GiveWool(bool fromBattle)
            {
                yield return new WaitForSeconds(0.5f);
                if (fromBattle)
                {
                    CardInfo pelt = CardLoader.GetCardByName("PeltGolden");
                    CardModificationInfo mod = new CardModificationInfo();
                    mod.abilities.Add(GiveFragile.ability);
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
                if (turnCount > MAX_TURNS)
                {
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("SheepEscapes", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                    base.PlayableCard.Anim.PlayDeathAnimation(false);
                    Object.Destroy(base.PlayableCard.gameObject);
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
                CardInfo card = currentDeck.Cards.Find((CardInfo x) => x.HasAbility(GiveFragile.ability) && x.name == base.Card.Info.name);
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

                if (slotsCopy != null && slotsCopy[slotsCopy.Count - 1].Card == base.Card)
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
                if (oldSlot.Card != null)
                {
                    yield return oldSlot.Card.Die(false, base.Card);
                }
                yield break;
            }

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
                for (int i = 0; i < this.cannonTargetSlots.Count; i++)
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.CleanupPhase))]
        public static void CannoneerPatch()
        {
            try
            {
                GiveCannoneer cannon = new GiveCannoneer();
                cannon.ClearAllTargetIcons();
            }
            catch
            {
                Plugin.Log.LogInfo("Did not find Cannoneer Instance");
            }
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
                    if (playerCardsCopy.Count > 0)
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
                    if (opponentCardsCopy.Count > 0)
                    {
                        target = opponentCardsCopy[SeededRandom.Range(0, opponentCardsCopy.Count - 1, seed++)];
                    }
                    else
                    {
                        target = null;
                    }
                }
                if (target == null)
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
                foreach (PlayableCard target in Singleton<BoardManager>.Instance.CardsOnBoard)
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
                    if (curCard != this.Card)
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
                abilities = new List<Ability> { Ability.Flying }
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
                foreach (PlayableCard target in Singleton<BoardManager>.Instance.CardsOnBoard)
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
        #endregion
    }
}
