using InscryptionAPI.Ascension;
using System;
using System.Collections.Generic;
using System.Text;
using static BittysChallenges.Plugin;

namespace BittysChallenges
{
    public class Decks
    {
        public static void AddDecks()
        {
            Log.LogInfo("Start of starting decks");
            Add_Deck_Pirate();
            Log.LogInfo("End of starting decks");
        }
        private static void Add_Deck_Pirate()
        {
            StarterDeckManager.New(PluginGuid, "PirateDeck", Tools.LoadTexture("starterdeck_icon_pirate.png"),
                new string[]
                {
                    "bitty_Minicello",
                    "bitty_SkeletonPirate",
                    "bitty_SkeletonParrot"
                }, 0);
        }
    }
}
