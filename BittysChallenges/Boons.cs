using DiskCardGame;
using InscryptionAPI.Boons;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
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
            Add_Boon_Conveyor();
            Add_Boon_GemSanctuary();
            Add_Boon_ElectricalStorm();
            Log.LogInfo("End of boons");
        }

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
        private static void Add_Boon_Conveyor()
        {
            Texture boonRulebookIcon = Tools.LoadTexture("boonicon_breeze");
            Texture boonCardArt = Tools.LoadTexture("boon_blank");
            BoonData.Type boon = BoonManager.New(PluginGuid, "Environment: Factory", typeof(ChallengeBoonConveyor), "On Upkeep, all cards are rotated clockwise.", boonRulebookIcon, boonCardArt, false, false, false);
            ChallengeBoonConveyor.boo = boon;
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
    }
}
