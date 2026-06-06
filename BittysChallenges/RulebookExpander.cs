using BittysSigils;
using DiskCardGame;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BittysChallenges
{
    public class RulebookExpansion
    {
        public static void Register(Harmony harmony)
        {
            harmony.PatchAll(typeof(RulebookExpansion));
        }

        [HarmonyPatch(typeof(RuleBookInfo))]
        public class RulebookPatch
        {

            [HarmonyPostfix]
            [HarmonyPatch(nameof(RuleBookInfo.AbilityShouldBeAdded))]
            private static void Postfix(ref bool __result, ref int abilityIndex)
            {
                AbilityInfo info = AbilitiesUtil.GetInfo((Ability)abilityIndex);
                if (info.ability == Ability.MoveBeside ||
                    info.ability == Ability.ExplodeOnDeath ||
                    info.ability == Ability.SkeletonStrafe ||
                    info.ability == Ability.Sentry ||
                    info.ability == Sigils.GiveNoTransfer.ability ||
                    info.ability == Plugin.GiveParalysis.ability ||
                    info.ability == Ability.ConduitBuffAttack ||
                    info.ability == Sigils.GiveFleeting.ability ||
                    info.ability == Sigils.GiveCantAttack.ability ||
                    info.ability == Sigils.GiveStrafePull.ability ||
                    info.ability == Sigils.GiveStrafeSticky.ability ||
                    info.ability == Sigils.GiveStrafeSuper.ability ||
                    info.ability == Plugin.GiveWarper.ability)
                {
                    __result = true;
                }
            }
        }
        
    }
}