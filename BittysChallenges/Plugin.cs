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
using static BittysChallenges.Abilities;

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
			harmony.PatchAll();

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

		[HarmonyPatch(typeof(AudioController))]
        public class AudioPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(AudioController.GetAudioClip))]
            public static void AddAudios(AudioController __instance, string soundId)
            {
                __instance.SFX.AddRange(addedSfx.Where(x => !__instance.SFX.Contains(x)));
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(AudioController.GetLoopClip))]
            public static void AddLoops(AudioController __instance, string loopId)
            {
                __instance.Loops.AddRange(addedSfx.Where(x => !__instance.Loops.Contains(x)));
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(AudioController.GetLoop))]
            public static void AddLoops2(AudioController __instance, string loopName)
            {
                __instance.Loops.AddRange(addedSfx.Where(x => !__instance.Loops.Contains(x)));
            }
        }
	}
}


