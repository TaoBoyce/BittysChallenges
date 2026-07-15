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
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using static BittysChallenges.Abilities;
using Object = UnityEngine.Object;

///Changelog: 6.1.1
///Fix to non-challenge mutiny
///
namespace BittysChallenges
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
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

            AssemblyLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //load audio
            addedSfx.Add(SoundManager.LoadAudioClip(PluginGuid, AssemblyLoc + "/Resources/sfx_vineBoom.ogg"));

            //loading things into API
            Challenges.AddChallenges();
			Boons.AddBoons();
            SlotMods.Add_SlotMods();
			Abilities.AddAbilities();
            SlotMods.Add_Reliant_SlotMods();
            Abilities.Add_Reliant_Abilities();
			Cards.AddCards();
            Dialogue.Add_Dialogue();
            Decks.AddDecks();
            
			//Load all patches
			harmony.PatchAll();

			base.Logger.LogInfo("Plugin Bitty's Challenges is loaded!");
		}
		internal const string PluginGuid = "bitty45.inscryption.challenges";

		internal const string PluginName = "Bitty's Challenges";

		internal const string PluginVersion = "6.1.1";

        internal const string CardPrefix = "bitty";

		public static string AssemblyLoc;
        public static List<AudioClip> addedSfx = new List<AudioClip> { };

		internal static ConfigEntry<int> famineRemoval;
		internal static ConfigEntry<int> abundanceQuality;
		internal static ConfigEntry<int> allowedResets;

		internal static ManualLogSource Log;

        public enum MOD_MODE
        {
            KCM,
            P03
        }
        public static MOD_MODE getModMode()
        {
            //P03
            if (Chainloader.PluginInfos.ContainsKey("zorro.inscryption.infiniscryption.p03kayceerun") &&
            AscensionSaveData.Data != null &&
            AscensionSaveData.Data.currentRun != null &&
            AscensionSaveData.Data.currentRun.playerLives > 0 &&
            ModdedSaveManager.SaveData.GetValueAsBoolean("zorro.inscryption.infiniscryption.p03kayceerun", "IsP03Run"))
            {
                return MOD_MODE.P03;
            }

            //Vanilla
            else
            {
                return MOD_MODE.KCM;
            }
        }
        public static bool modModeActive(MOD_MODE modMode)
        {
            return getModMode() == modMode;
        }

        [HarmonyPatch(typeof(AudioController))]
        public class AudioPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("GetAudioClip")]
            public static void AddAudiosClips(AudioController __instance, string soundId)
            {
                __instance.SFX.AddRange(from x in Plugin.addedSfx
                                        where !__instance.SFX.Contains(x)
                                        select x);
            }

            [HarmonyPrefix]
            [HarmonyPatch("GetLoopClip")]
            public static void AddLoopClips(AudioController __instance, string loopId)
            {
                __instance.Loops.AddRange(from x in Plugin.addedSfx
                                          where !__instance.Loops.Contains(x)
                                          select x);
            }

            [HarmonyPrefix]
            [HarmonyPatch("GetLoop")]
            public static void AddLoops(AudioController __instance, string loopName)
            {
                __instance.Loops.AddRange(from x in Plugin.addedSfx
                                          where !__instance.Loops.Contains(x)
                                          select x);
            }
        }
    }
}


