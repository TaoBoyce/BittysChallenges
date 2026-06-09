using DiskCardGame;
using InscryptionAPI.Helpers;
using InscryptionAPI.Slots;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static BittysChallenges.Abilities;

namespace BittysChallenges
{
    public class SlotMods
    {
        public static void Add_SlotMods()
        {
            Add_Slot_Raft();
        }
        #region SlotMod Loaders
        public static void Add_Slot_Raft()
        {
            SlotModificationManager.ModificationType SlotRaft = SlotModificationManager.New(
            Plugin.PluginGuid,
            "SlotRaft",
            typeof(SlotMod_Raft),
            Tools.LoadTexture("card_slot_test.png")
            )
            .SetRulebook(
            "Raft",
            "Prevents creatures from submerging into the water.",
            Tools.LoadTexture("ability_raft.png"),
            SlotModificationManager.ModificationMetaCategory.Part1Rulebook
            )
        ;
            SlotMod_Raft.SlotType = SlotRaft;
        }
        #endregion
        #region SlotMods
        public abstract class SlotMod : SlotModificationBehaviour
        {
            public abstract SlotModificationManager.ModificationType ModificationType { get; }
        }
        public class SlotMod_Raft : SlotMod
        {
            public override SlotModificationManager.ModificationType ModificationType
            {
                get
                {
                    return SlotType;
                }
            }
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
                negateAbilities = new List<Ability>() { Ability.Submerge, Ability.SubmergeSquid}
            };
        }
        #endregion
    }
}
