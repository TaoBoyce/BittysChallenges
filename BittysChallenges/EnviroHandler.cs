using DiskCardGame;
using System;
using System.Collections.Generic;
using System.Text;
using static BittysChallenges.Boons;
using static BittysChallenges.Plugin;

namespace BittysChallenges
{
    public class EnviroHandler
    {
        public static List<EnviroBoon> EnviroBoonList;
        public static readonly string REGION_ANY = "Any";
        public static readonly string REGION_FOREST = "Forest";
        public static readonly string REGION_WETLANDS = "Wetlands";
        public static readonly string REGION_ALPINE = "Alpine";
        public static readonly string REGION_MAGMA = "Magma_bitty";
        public static void Init_EnviroBoonList()
        {
            EnviroBoonList = new List<EnviroBoon>();
            EnviroBoonList.Add(new EnviroBoon("Cliffs", ChallengeBoonCliffs.boo, REGION_ANY, 0, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Breeze", ChallengeBoonBreeze.boo, REGION_ANY, 1, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Growth", ChallengeBoonFlashGrowth.boo, REGION_ANY, 1, MOD_MODE.KCM, MOD_MODE.P03));
            EnviroBoonList.Add(new EnviroBoon("Grave", ChallengeBoonGraveyard.boo, REGION_ANY, 1, MOD_MODE.KCM, MOD_MODE.P03));
            EnviroBoonList.Add(new EnviroBoon("Obelisk", ChallengeBoonObelisk.boo, REGION_ANY, 2, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Mushrooms", ChallengeBoonMushrooms.boo, REGION_ANY, 2, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("BloodMoon", ChallengeBoonBloodMoon.boo, REGION_ANY, 2, new ECond_Challenge_Inclusive(AscensionChallenge.GrizzlyMode), MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("BloodMoon(?)", ChallengeBoonCarrotPatch.boo, REGION_ANY, 2, new ECond_CarrotPatch(), MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Minicello", ChallengeBoonMinicello.boo, REGION_ANY, 2, new ECond_Challenge_Inclusive(Challenges.Challenge_harderFinalBoss.challengeType, AscensionChallenge.FinalBoss), MOD_MODE.KCM));

            EnviroBoonList.Add(new EnviroBoon("Totem", ChallengeBoonTotem.boo, REGION_FOREST, 0, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Dynamite", ChallengeBoonDynamite.boo, REGION_FOREST, 1, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("DForest", ChallengeBoonDarkForest.boo, REGION_FOREST, 2, MOD_MODE.KCM));

            EnviroBoonList.Add(new EnviroBoon("Hail", ChallengeBoonHail.boo, REGION_ALPINE, 0, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Trap", ChallengeBoonTrap.boo, REGION_ALPINE, 1, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Bliz", ChallengeBoonBlizzard.boo, REGION_ALPINE, 2, MOD_MODE.KCM));

            EnviroBoonList.Add(new EnviroBoon("Mud", ChallengeBoonMud.boo, REGION_WETLANDS, 0, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Bait", ChallengeBoonBait.boo, REGION_WETLANDS, 1, MOD_MODE.KCM));
            EnviroBoonList.Add(new EnviroBoon("Flood", ChallengeBoonFlood.boo, REGION_WETLANDS, 2, MOD_MODE.KCM));

            EnviroBoonList.Add(new EnviroBoon("Gem", ChallengeBoonGemSanctuary.boo, REGION_ANY, 0, MOD_MODE.P03));
            EnviroBoonList.Add(new EnviroBoon("Overclock", ChallengeBoonElectricStorm.boo, REGION_ANY, 0, MOD_MODE.P03));
        }
        #region Enviroment Boons
        public class EnviroBoon
        {
            public EnviroBoon(string boonName, BoonData.Type boonType, string regionName, int regionTier, params List<MOD_MODE> modes)
            {
                this.boonName = boonName;
                this.boonType = boonType;
                this.regionName = regionName;
                this.regionTier = regionTier;
                this.modes = modes;
                this.condition = null;
            }
            public EnviroBoon(string boonName, BoonData.Type boonType, string regionName, int regionTier, EnviroCondition condition, params List<MOD_MODE> modes)
            {
                this.boonName = boonName;
                this.boonType = boonType;
                this.regionName = regionName;
                this.regionTier = regionTier;
                this.modes = modes;
                this.condition = condition;
            }
            public string boonName;
            public BoonData.Type boonType;
            public string regionName;
            public int regionTier;
            public EnviroCondition condition;
            public List<MOD_MODE> modes;
        }
        public abstract class EnviroCondition
        {
            public abstract bool IsActive();
        }
        public class ECond_Challenge_Inclusive : EnviroCondition
        {
            public ECond_Challenge_Inclusive(params List<AscensionChallenge> challenges)
            {
                this.challenges = challenges;
            }
            List<AscensionChallenge> challenges;
            public override bool IsActive()
            {
                foreach (var challenge in challenges)
                {
                    if (AscensionSaveData.Data.ChallengeIsActive(challenge))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public class ECond_Challenge_Exclusive : EnviroCondition
        {
            public ECond_Challenge_Exclusive(params List<AscensionChallenge> challenges)
            {
                this.challenges = challenges;
            }
            List<AscensionChallenge> challenges;
            public override bool IsActive()
            {
                foreach (var challenge in challenges)
                {
                    if (!AscensionSaveData.Data.ChallengeIsActive(challenge))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public class ECond_CarrotPatch : EnviroCondition
        {
            public override bool IsActive()
            {
                return !DialogueEventsData.EventIsPlayed("CarrotBoonIntro") &&
                    AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode);
            }
        }
        #endregion
    }
}
