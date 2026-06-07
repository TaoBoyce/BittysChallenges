using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using BittysSigils;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Ascension;
using InscryptionAPI.Boons;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers.Extensions;
using InscryptionAPI.Saves;
using InscryptionAPI.Sound;
using InscryptionAPI.Triggers;
using InscryptionMod.Abilities;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static BittysChallenges.Abilities;
using Object = UnityEngine.Object;

///Changelog: 6.1.0
///Updates to existing challenges
///SFX uses API
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

            SoundManager.LoadAudioClip(PluginGuid, "Resources/sfx_vineBoom.ogg");

            //loading things into API
            Challenges.AddChallenges();
			Boons.AddBoons();
			Abilities.AddAbilities();
			Cards.AddCards();
            
			//Load all patches
			harmony.PatchAll();

			base.Logger.LogInfo("Plugin Bitty's Challenges is loaded!");
		}
		internal const string PluginGuid = "bitty45.inscryption.challenges";

		internal const string PluginName = "Bitty's Challenges";

		internal const string PluginVersion = "6.1.0";

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
	}
}


