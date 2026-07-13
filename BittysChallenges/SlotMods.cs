using BittysSigils;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Dialogue;
using InscryptionAPI.Helpers;
using InscryptionAPI.RuleBook;
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
    public static class SlotModExtensions
    {
        //A smoother transition to a slot mod than just instantly changing
        public static IEnumerator FadeToSlotMod(this CardSlot cardSlot, SlotModificationManager.ModificationType slotType)
        {
            cardSlot.SetShown(false, false);
            yield return new WaitForSeconds(0.45f);
            yield return cardSlot.SetSlotModification(slotType);
            cardSlot.SetShown(true, false);
            yield break;
        }
    }
    public class SlotMods
    {
        public static void Add_SlotMods()
        {
            Log.LogInfo("Start of slot mods");
            Add_Slot_Raft();
            Add_Slot_Hail();
            Add_Slot_Flood();
            Add_Slot_Breeze();
            Add_Slot_Grave();
            Add_Slot_Growth();
            Add_Slot_Overclock();
            Add_Slot_Dynamite();
            Add_Slot_Obelisk();
            Log.LogInfo("End of slot mods");
        }
        public static void Add_Reliant_SlotMods()
        {
            Log.LogInfo("Start of reliant slot mods");
            Add_Slot_Muddy();
            Log.LogInfo("End of reliant slot mods");
        }
        #region SlotMod Loaders
        public static void Add_Slot_Raft()
        {
            SlotModificationManager.ModificationType SlotRaft = SlotModificationManager.New(
            PluginGuid,
            "SlotRaft",
            typeof(SlotMod_Raft),
            Tools.LoadTexture("card_slot_raft.png")
            )
            .SetRulebook(
            "Raft Slot",
            "Prevents creatures on this slot from submerging into the water.",
            Tools.LoadTexture("rulebookitemicon_raft.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            );
            //Log.LogInfo("pre redirect");
            //SlotRaft.SetAbilityRedirect("submerging", Ability.Submerge, GameColors.instance.orange)
        ;
            //Log.LogInfo("post redirect");
            SlotMod_Raft.SlotType = SlotRaft;
        }
        public static void Add_Slot_Muddy()
        {
            SlotModificationManager.ModificationType SlotMuddy = SlotModificationManager.New(
            PluginGuid,
            "SlotMuddy",
            typeof(SlotMod_Muddy),
            Tools.LoadTexture("card_slot_mud.png")
            )
            .SetRulebook(
            "Muddy Slot",
            "When a creature is played on this slot, it cannot attack for one turn.",
            Tools.LoadTexture("rulebookitemicon_mud.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
            //.SetAbilityRedirect("cannot attack", Sigils.GiveCantAttack.ability, GameColors.instance.orange)
        ;
            SlotMod_Muddy.SlotType = SlotMuddy;
        }
        public static void Add_Slot_Hail()
        {
            SlotModificationManager.ModificationType SlotHail = SlotModificationManager.New(
            PluginGuid,
            "SlotHail",
            typeof(SlotMod_Hail),
            Tools.LoadTexture("card_slot_hail.png")
            )
            .SetRulebook(
            "Hail Slot",
            "At the start of the owner\'s turn, a creature in this slot takes 1 damage.",
            Tools.LoadTexture("rulebookitemicon_hail.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
        ;
            SlotMod_Hail.SlotType = SlotHail;
        }
        public static void Add_Slot_Flood()
        {
            SlotModificationManager.ModificationType SlotFlood = SlotModificationManager.New(
            PluginGuid,
            "SlotFlood",
            typeof(SlotMod_Flood),
            Tools.LoadTexture("card_slot_flood.png")
            )
            .SetRulebook(
            "Flooded Slot",
            "When a creature is played in this slot, it gets submerged in the water unless it is flying.",
            Tools.LoadTexture("rulebookitemicon_flood.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
            //.SetAbilityRedirect("flying", Ability.Flying, GameColors.Instance.orange)
        ;
            SlotMod_Flood.SlotType = SlotFlood;
        }
        public static void Add_Slot_Breeze()
        {
            SlotModificationManager.ModificationType SlotBreeze = SlotModificationManager.New(
            PluginGuid,
            "SlotBreeze",
            typeof(SlotMod_Breeze),
            Tools.LoadTexture("card_slot_breeze.png")
            )
            .SetRulebook(
            "Windy Slot",
            "At the start of the owner\'s turn, a creature in this slot will alternate between flying and not flying. This effect is ignored if the creature naturally flies, submerges, or burrows.",
            Tools.LoadTexture("rulebookitemicon_breeze.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
            //.SetAbilityRedirect("flying",Ability.Flying,GameColors.Instance.orange)
            //.SetAbilityRedirect("flies", Ability.Flying,GameColors.Instance.orange)
            //.SetAbilityRedirect("submerges", Ability.Submerge,GameColors.Instance.brightSeafoam)
            //.SetAbilityRedirect("burrows", Ability.WhackAMole,GameColors.Instance.gold)
        ;
            SlotMod_Breeze.SlotType = SlotBreeze;
        }
        public static void Add_Slot_Grave()
        {
            SlotModificationManager.ModificationType SlotGrave = SlotModificationManager.New(
            PluginGuid,
            "SlotGrave",
            typeof(SlotMod_Grave),
            Tools.LoadTexture("card_slot_grave.png")
            )
            .SetRulebook(
            "Grim Slot",
            "When a creature dies in this slot, it dies again.",
            Tools.LoadTexture("rulebookitemicon_grave.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
        ;
            SlotMod_Grave.SlotType = SlotGrave;
        }
        public static void Add_Slot_Growth()
        {
            SlotModificationManager.ModificationType SlotGrowth = SlotModificationManager.New(
            PluginGuid,
            "SlotGrowth",
            typeof(SlotMod_Growth),
            Tools.LoadTexture("card_slot_overgrowth.png")
            )
            .SetRulebook(
            "Growth Slot",
            "When a creature is played in this slot, instantly evolve it if it has an evolve related sigil. If it does not have Fledgling, it instead gains Fledgling.",
            Tools.LoadTexture("rulebookitemicon_overgrowth.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook,
            SlotModificationManager.ModificationMetaCategory.Part3Rulebook
            )
            //.SetAbilityRedirect("Fledgeling", Ability.Evolve, GameColors.Instance.orange)
        ;
            SlotMod_Growth.SlotType = SlotGrowth;
        }
        public static void Add_Slot_Overclock()
        {
            SlotModificationManager.ModificationType SlotOverclock = SlotModificationManager.New(
            PluginGuid,
            "SlotOverclock",
            typeof(SlotMod_Overclock),
            Tools.LoadTexture("card_slot_overclock.png")
            )
            .SetRulebook(
            "Overclocking Slot",
            "When a creature is played in this slot, it takes 1 damage and gains 1 attack.",
            Tools.LoadTexture("rulebookitemicon_overclock.png"),
            SlotModificationManager.ModificationMetaCategory.Part3Rulebook,
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
        ;
            SlotMod_Overclock.SlotType = SlotOverclock;
        }
        public static void Add_Slot_Dynamite()
        {
            SlotModificationManager.ModificationType SlotDynamite = SlotModificationManager.New(
            PluginGuid,
            "SlotDynamite",
            typeof(SlotMod_Dynamite),
            Tools.LoadTexture("card_slot_dynamite.png")
            )
            .SetRulebook(
            "Dynamite Slot",
            "When a creature is played in this slot, it takes 10 damage and explodes. The dynamite is removed.",
            Tools.LoadTexture("rulebookitemicon_dynamite.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
        ;
            SlotMod_Dynamite.SlotType = SlotDynamite;
        }
        public static void Add_Slot_Obelisk()
        {
            SlotModificationManager.ModificationType SlotObelisk = SlotModificationManager.New(
            PluginGuid,
            "SlotObelisk",
            typeof(SlotMod_Obelisk),
            Tools.LoadTexture("card_slot_obelisk.png")
            )
            .SetRulebook(
            "Obelisk Tablet",
            "When a creature is played in this slot, it is sacrificed.",
            Tools.LoadTexture("rulebookitemicon_obelisk.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
            //.SetStatIconRedirect("sacrificed", SpecialStatIcon.SacrificesThisTurn, GameColors.instance.orange)
        ;
            SlotMod_Obelisk.SlotType = SlotObelisk;
        }
        #endregion
        #region SlotMods
        public class SlotMod_Obelisk : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return Slot.Card != null && Slot.Card == otherCard;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
                if (otherCard.Info.HasTrait(Trait.Goat) || otherCard.HasAbility(Ability.TripleBlood))
                {
                    AudioController.Instance.PlaySound2D("creepy_rattle_lofi", MixerGroup.None, 1f, 0f, null, null, null, null, false);

                    yield return DialogueManager.PlayDialogueEventSafe("WorthySacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait);
                    yield return new WaitForSeconds(0.25f);
                    Singleton<ViewManager>.Instance.SwitchToView(View.Default);
                    yield return new WaitForSeconds(0.25f);
                    yield return Singleton<ResourcesManager>.Instance.AddBones(8, null);
                    yield return new WaitForSeconds(0.25f);
                    properSacrifice = true;
                }
                else if (otherCard.HasTrait(Trait.Pelt) || otherCard.HasTrait(Trait.Terrain))
                {
                    yield return new WaitForSeconds(0.7f);
                    if (otherCard.HasTrait(Trait.Pelt))
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("PeltSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                    else
                    {
                        yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("TerrainSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                    }
                    yield return new WaitForSeconds(0.25f);
                    List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                    opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.Card.Info.name != "bitty_Obelisk");
                    if (opponentSlotsCopy.Count > 0)
                    {
                        AudioController.Instance.PlaySound3D("dueldisk_card_played", MixerGroup.TableObjectsSFX, opponentSlotsCopy[0].Card.transform.position, 2f, 0f, null, null, null, null, false);
                        for (int i = 0; i < opponentSlotsCopy.Count; i++)
                        {
                            opponentSlotsCopy[i].Card.AddTemporaryMod(mod);
                            opponentSlotsCopy[i].Card.Anim.PlayTransformAnimation();
                        }
                    }
                }
                else if (otherCard.name.Contains("Squirrel"))
                {
                    yield return new WaitForSeconds(0.7f);
                    squirrelSacrifices++;
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("SquirrelSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[]
                    {
                        squirrelSacrifices.ToString()
                    }, null);
                    yield return new WaitForSeconds(0.25f);
                }
                else
                {
                    yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("NormalSacrifice", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                    yield return new WaitForSeconds(0.25f);
                }
                Singleton<BoardManager>.Instance.SacrificesMadeThisTurn += 1;
                yield return otherCard.Die(false, null, true);
                if (properSacrifice)
                {
                    yield return Slot.ClearSlotModification();
                }
                yield break;
            }

            CardModificationInfo mod = new CardModificationInfo(1, 0)
            {
                singletonId = "bitty_obeliskAnger",
                abilities = new List<Ability>() { Ability.BuffNeighbours },
                nonCopyable = true,
            };
            private bool properSacrifice = false;
            public int squirrelSacrifices;
        }
        public class SlotMod_Dynamite : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            private GameObject bombPrefab;
            private void Awake()
            {
                this.bombPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb");
            }
            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return Slot.Card != null && Slot.Card == otherCard;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
                yield return this.ExplodeFromSlot(Slot);
                yield return otherCard.TakeDamage(10, null);
                yield return Slot.ClearSlotModification();
                yield break;
            }
            protected IEnumerator ExplodeFromSlot(CardSlot slot)
            {
                List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot);
                if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slot.Index)
                {
                    if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
                    {
                        yield return this.BombCard(adjacentSlots[0].Card);
                    }
                    adjacentSlots.RemoveAt(0);
                }
                if (slot.opposingSlot.Card != null && !slot.opposingSlot.Card.Dead)
                {
                    yield return this.BombCard(slot.opposingSlot.Card);
                }
                if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
                {
                    yield return this.BombCard(adjacentSlots[0].Card);
                }
                yield break;
            }
            private IEnumerator BombCard(PlayableCard target)
            {
                GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(this.bombPrefab);
                bomb.transform.position = Slot.transform.position + Vector3.up * 0.1f;
                Tween.Position(bomb.transform, target.transform.position + Vector3.up * 0.1f, 0.5f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
                yield return new WaitForSeconds(0.5f);
                target.Anim.PlayHitAnimation();
                UnityEngine.Object.Destroy(bomb);
                yield return target.TakeDamage(10, null);
                yield break;
            }
        }
        public class SlotMod_Overclock : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return Slot.Card != null && Slot.Card == otherCard;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
                AudioController.Instance.PlaySound2D("teslacoil_overload", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
                yield return otherCard.TakeDamage(1, null);
                if (otherCard.Dead != true)
                {
                    yield return new WaitForSeconds(0.3f);
                    otherCard.Anim.PlayTransformAnimation();
                    otherCard.AddTemporaryMod(mod);
                }
                yield break;
            }

            CardModificationInfo mod = new CardModificationInfo(1, 0)
            {
                singletonId = "bitty_overclock",
                fromOverclock = true
            };
        }
        public class SlotMod_Growth : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return Slot.Card != null && Slot.Card == otherCard;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
                if (otherCard.HasAbility(Ability.Transformer))
                {
                    yield return otherCard.TriggerHandler.OnTrigger(Trigger.Upkeep, new object[]
                    {
                        true //playerTurn
                    });
                }
                else if (otherCard.HasAbility(Ability.Evolve))
                {
                    yield return otherCard.TriggerHandler.OnTrigger(Trigger.Upkeep, new object[]
                    {
                        otherCard.IsPlayerCard() //playerTurn
                    });
                }
                else
                {
                    otherCard.AddTemporaryMod(mod);
                    otherCard.OnStatsChanged();
                    otherCard.Anim.StrongNegationEffect();
                }
                yield break;
            }

            CardModificationInfo mod = new CardModificationInfo()
            {
                singletonId = "bitty_evolve",
                nonCopyable = true,
                abilities = new List<Ability>() { Ability.Evolve }
            };
        }
        public class SlotMod_Grave : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
                CardModificationInfo cardModificationInfo = card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_grave");
                return card != null && deathSlot == Slot && cardModificationInfo == null;
            }
            public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
            {
                REVIVES++;
                if (REVIVES > 5)
                {
                    yield break;
                }
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(card.Info, deathSlot, 0.1f, true);
                yield return new WaitForSeconds(0.1f);
                if (deathSlot.Card != null)
                {
                    deathSlot.Card.AddTemporaryMod(mod);
                    yield return deathSlot.Card.Die(false, null, true);
                }
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

            CardModificationInfo mod = new CardModificationInfo()
            {
                singletonId = "bitty_grave",
                nonCopyable = true,
            };
        }
        public class SlotMod_Breeze : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToUpkeep(bool playerUpkeep)
            {
                return Slot.Card != null && !Slot.Card.Info.HasTrait(Trait.Terrain) &&
                    Slot.Card.IsPlayerCard() == playerUpkeep;
            }
            public override IEnumerator OnUpkeep(bool playerUpkeep)
            {
                //yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("BreezeActivation", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.CancelSelf, null, null);

                CardModificationInfo cardModificationInfo = Slot.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_airborne");
                if (cardModificationInfo != null)
                {
                    Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                    Slot.Card.RemoveTemporaryMod(cardModificationInfo);
                    Slot.Card.OnStatsChanged();
                    Slot.Card.Anim.StrongNegationEffect();
                    yield return new WaitForSeconds(0.1f);
                }
                else if (!Slot.Card.HasAbility(Ability.Flying))
                {
                    Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                    Slot.Card.AddTemporaryMod(mod);
                    Slot.Card.OnStatsChanged();
                    Slot.Card.Anim.StrongNegationEffect();
                    yield return new WaitForSeconds(0.1f);
                }
                yield break;
            }

            CardModificationInfo mod = new CardModificationInfo()
            {
                abilities = new List<Ability>() { Ability.Flying },
                singletonId = "bitty_airborne",
                nonCopyable = true,
            };
        }
        public class SlotMod_Flood : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
            {
                return otherCard == Slot.Card && !Slot.Card.Info.HasTrait(Trait.Terrain) &&
                    !Slot.Card.HasAbility(Ability.Flying) && !Slot.Card.HasAbility(Ability.Submerge);
            }
            public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
            {
                CardModificationInfo cardModificationInfo = otherCard.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_raft");
                if (cardModificationInfo != null)
                {
                    otherCard.RemoveTemporaryMod(cardModificationInfo);
                }
                otherCard.AddTemporaryMod(mod);
                otherCard.OnStatsChanged();
                otherCard.Anim.StrongNegationEffect();
                yield return new WaitForSeconds(0.1f);
                yield break;
            }

            private CardModificationInfo mod = new CardModificationInfo()
            {
                abilities = new List<Ability>() { Ability.Submerge },
                singletonId = "bitty_flood",
                nonCopyable = true,
            };
        }
        public class SlotMod_Hail : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToUpkeep(bool playerUpkeep)
            {
                return Slot.Card != null && !Slot.Card.Info.HasTrait(Trait.Terrain) &&
                    Slot.Card.IsPlayerCard() == playerUpkeep;
            }
            public override IEnumerator OnUpkeep(bool playerUpkeep)
            {
                if (RunState.CurrentRegionTier >= 1 || Slot.Card.Health >= 1)
                {
                    Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                    yield return new WaitForSeconds(0.5f);
                    yield return Slot.Card.TakeDamage(1, null);
                }

                if (Slot.Card == null && RunState.CurrentRegionTier >= 2)
                {
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("bitty_IceCube"), Slot, 0.5f, false);
                }
                yield return new WaitForSeconds(0.3f);
                yield break;
            }
        }
        public class SlotMod_Muddy : SlotModificationBehaviour
        {
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
            {
                return Slot.Card == otherCard;
            }
            public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
            {
                otherCard.AddTemporaryMod(mod);
                otherCard.Anim.StrongNegationEffect();
                yield break;
            }

            private CardModificationInfo mod = new CardModificationInfo()
            {
                abilities = new List<Ability>() { Sigils.GiveCantAttack.ability },
                RemoveOnUpkeep = true,
            };
        }
        public class SlotMod_Raft : SlotModificationBehaviour
        {
            
            public static SlotModificationManager.ModificationType SlotType;

            public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
            {
                return Slot.Card != null && Slot.Card == otherCard;
            }
            public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
            {
                CardModificationInfo cardModificationInfo = otherCard.TemporaryMods.Find((CardModificationInfo x) 
                    => x.singletonId == "bitty_flood");
                if (cardModificationInfo != null)
                {
                    otherCard.RemoveTemporaryMod(cardModificationInfo);
                }
                otherCard.AddTemporaryMod(mod);
                if (otherCard.Anim.FaceDown)
                {
                    otherCard.Anim.SetFaceDown(false);
                }
                otherCard.OnStatsChanged();
                otherCard.Anim.StrongNegationEffect();
                yield break;
            }

            private CardModificationInfo mod = new CardModificationInfo()
            {
                negateAbilities = new List<Ability>() { Ability.Submerge, Ability.SubmergeSquid },
                singletonId = "bitty_raft",
                nonCopyable = true,
            };
        }
        #endregion
    }
}

