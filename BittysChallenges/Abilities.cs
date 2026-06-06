using DiskCardGame;
using InscryptionAPI.Card;
using System;
using System.Collections.Generic;
using System.Text;
using static BittysChallenges.Plugin;

namespace BittysChallenges
{
    public class Abilities
    {
        public static void AddAbilities()
        {
            Log.LogInfo("Start of sigils");
            Add_Ability_FalseUnkillable();
            Add_Ability_Warper();
            Add_Ability_Fragile();
            Add_Ability_Paralysis();
            Add_Ability_Muddy();
            Add_Ability_Shelter();
            Add_Ability_Dynamite();
            Add_Ability_StrafeKiller();
            Add_Ability_StrafeAvalanche();
            Add_Ability_ObeliskSlot();
            Add_Ability_Raft();
            Log.LogInfo("End of sigils");

            Log.LogInfo("Start of champs");
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
            Log.LogInfo("End of champs");
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
            abilityInfo.abilityLearnedDialogue = Dialogue.Dialogue.SetAbilityInfoDialogue("Stunned and confused.");

            // Pass the ability to the API.
            GiveParalysis.ability = abilityInfo.ability;
        }
        private static void Add_Ability_Muddy()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Muddy",
                "Other cards may be placed on top of [creature], but will be unable to attack for one turn and will gain Muddy. If [creature] is sacrificed, the sacrificing creature will be unable to attack for one turn.",
                typeof(GiveMuddy),
                Tools.LoadTexture("ability_mud.png")
            ).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = -1;


            // Pass the ability to the API.
            GiveMuddy.ability = abilityInfo.ability;
        }
        private static void Add_Ability_Shelter()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Shelter",
                "Adjacent cards are sheltered from environmental effects.",
                typeof(GiveShelter),
                Tools.LoadTexture("ability_shelter.png")
            ).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 3;


            // Pass the ability to the API.
            GiveShelter.ability = abilityInfo.ability;
        }
        private static void Add_Ability_Dynamite()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Explosive",
                "Other cards may be placed on top of [creature]; the other card, adjacent cards, and opposing cards will all be dealt 10 damage.",
                typeof(GiveDynamite),
                Tools.LoadTexture("ability_dynamite.png")
            ).AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = -1;


            // Pass the ability to the API.
            GiveDynamite.ability = abilityInfo.ability;
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
        private static void Add_Ability_ObeliskSlot()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Sacrificial Slab",
                "Other cards may be placed on top of [creature]; the other card will die.",
                typeof(GiveObeliskSlot),
                Tools.LoadTexture("ability_sacrificeslab.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 0;

            // Pass the ability to the API.
            GiveObeliskSlot.ability = abilityInfo.ability;
        }
        private static void Add_Ability_Raft()
        {
            AbilityInfo abilityInfo = AbilityManager.New(
                PluginGuid,
                "Seaworthy",
                "Other cards may be placed on top of [creature], any Waterborne sigils on the card will be negated.",
                typeof(GiveRaft),
                Tools.LoadTexture("ability_raft.png")
            )
            .AddMetaCategories(AbilityMetaCategory.Part1Rulebook)
            ;
            abilityInfo.powerLevel = 1;

            // Pass the ability to the API.
            GiveRaft.ability = abilityInfo.ability;
        }
        //CHAMPION ABILITIES ------------------------------------------------
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
    }
}
