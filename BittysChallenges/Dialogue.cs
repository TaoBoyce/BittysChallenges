using DiskCardGame;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BittysChallenges
{
    [HarmonyPatch]
    public class Dialogue
    {
        public static DialogueEvent.LineSet SetAbilityInfoDialogue(string dialogue)
        {
            return new DialogueEvent.LineSet(new List<DialogueEvent.Line>
            {
                new DialogueEvent.Line
                {
                    text = dialogue
                }
            });
        }
        public static void P03HappyCloseUp(DialogueEvent.Line line)
        {
            if (line.p03Face == P03AnimationController.Face.Happy)
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.P03Face, true, false);
            }
            else
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Default, true, false);
            }
        }

        [HarmonyPatch(typeof(DialogueDataUtil))]
        public class DialoguePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(DialogueDataUtil.ReadDialogueData))]
            public static void ModDialogue()
            {
                ///-------------Act 1 Lines----------
                DialogueHelper.AddOrModifySimpleDialogEvent("RedrawHandIntro", new string[]
                {
                "Is that a clover?",
                "I see."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("UnfairHandIntro", new string[]
                {
                "Oh?",
                "Did you not like having a stacked deck?",
                "That's fine with me.",
                "It will be more fair this way anyways."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroIntro", new string[]
                {
                "oh?",
                "my very own ouroboros.",
                "i will be sure to put it to good use."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom1", new string[]
                {
                "the ouroboros has followed you.",
                "it's fangs are beared, and it's ready for another fight."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom3", new string[]
                {
                "the unyielding ouroboros has returned,",
                "growing stronger every death."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom4", new string[]
                {
                "as inevitable as death,",
                "the ouroboros has returned."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroZoom2", new string[]
                {
                "a serpent slithers out from the undergrowth.",
                "perhaps you have seen it before?"
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroDies1", new string[]
                {
                "the ouroboros has died.",
                "and yet, it will return."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroDies2", new string[]
                {
                "the serpent's wrath has been delayed.",
                "for now."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("OuroDies3", new string[]
                {
                "the ouroboros only grows stronger from death.",
                "it will be back."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("MycoFailSigils", new string[]
                {
                "a-ah, the sigils..."
                }, null, null, null, "DoctorIntro");
                DialogueHelper.AddOrModifySimpleDialogEvent("MycoFailAttack", new string[]
                {
                "o-oh, the power..."
                }, null, null, null, "DoctorIntro");
                DialogueHelper.AddOrModifySimpleDialogEvent("MycoFailHealth", new string[]
                {
                "a-ah, the health..."
                }, null, null, null, "DoctorIntro");
                DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepIntro", new string[]
                 {
                "ah...",
                "Chrysomallos, the golden ram.",
                "what a glorious pelt...."
                 }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom1", new string[]
                {
                "The golden ram.",
                "catch it before it escapes."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom3", new string[]
                {
                "a rare sight.",
                "it will not stay for long."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom4", new string[]
                {
                "what glittering wool,",
                "attached to such a rare creature."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("GoldenSheepZoom2", new string[]
                {
                "oh?",
                "a rare chance to get a rare pelt.",
                "best make the most of it."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("SheepDies1", new string[]
                {
                "You slay the glimmering creature, stealing its pelt."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("SheepDies2", new string[]
                {
                "The end of such a glorious creature.",
                "And what a glorious pelt you have obtained."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("SheepDies3", new string[]
                {
                "Chrysomallos...",
                "How tragic your tale is...",
                "To be killed for your pelt..."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("SheepEscapes1", new string[]
                {
                "The Golden Ram lives another day."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("SheepEscapes2", new string[]
                {
                "Time's up.",
                "The Golden Ram has found an escape route."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("SheepEscapes3", new string[]
                {
                "The glimmer of the Golden Ram's fur blinds you,",
                "giving it the opportunity to escape."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("FragileEnemy", new string[]
                {
                "Ah...",
                "Your [v:0] has taken a devastating blow from the [v:1].",
                "You will not be able to take it with your caravan."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("FragileDies", new string[]
                {
                "Ah...",
                "You won't be seeing your [v:0] again,",
                "It has taken too much damage."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("FragileSacrifice", new string[]
                {
                "Ah...",
                "Did you permanently kill your [v:0] on purpose?",
                "A shame..."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("RoyalFirstMate", new string[]
                {
                "Ha! There be my first mate!"
                }, null, null, Emotion.Laughter, "PirateSkullPreCharge");
                DialogueHelper.AddOrModifySimpleDialogEvent("RoyalOuro", new string[]
                {
                "Argh! There be snakes on me ship as well!"
                }, null, null, Emotion.Anger, "PirateSkullPreCharge");
                DialogueHelper.AddOrModifySimpleDialogEvent("RoyalOuroDiesPlayer", new string[]
                {
                "Ha!",
                "I should hire ye to get rid of the rest of them!"
                }, null, null, Emotion.Neutral, "PirateSkullPreCharge");
                DialogueHelper.AddOrModifySimpleDialogEvent("RoyalOuroDies", new string[]
                {
                "Ha! It's dead!"
                }, null, null, Emotion.Laughter, "PirateSkullPreCharge");
                DialogueHelper.AddOrModifySimpleDialogEvent("PirateIntro", new string[]
                {
                "what?",
                "Pirates?",
                "...",
                "I'll allow it."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesIntro", new string[]
                {
                "what?",
                "cheating?",
                "how dissapointing...",
                "...",
                "the game will be soured from a pitifully easy ascent."
                }, null, null, Emotion.Anger, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesRepeat", new string[]
                {
                "cheater..."
                }, new string[][]
                {
                new string[]
                {
                    "Have you no shame?"
                },
                new string[]
                {
                    "To cheat so blatantly..."
                },
                new string[]
                {
                    "you should be dead."
                },
                new string[]
                {
                    "and yet..."
                },
                new string[]
                {
                    "how dissapointing..."
                },
                new string[]
                {
                    "..."
                }
                }, null, Emotion.Anger, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesLoop", new string[]
                 {
                "I won't tolerate this for much longer..."
                 }, null, null, Emotion.Anger, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesLoopBreak", new string[]
                 {
                "Enough of this."
                 }, null, null, Emotion.Anger, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("InfiniteLivesRoyal", new string[]
                {
                "Yer not dead?"
                }, null, null, Emotion.Curious, "PirateSkullPreCharge");
                DialogueHelper.AddOrModifySimpleDialogEvent("FecundityUnNerfIntro", new string[]
                {
                "ah...",
                "back to normal then?",
                "I'll admit, I had gotten used to the changes..."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("WeakStart", new string[]
                {
                "Weak Cards?",
                "This is hard to explain...",
                "Perhaps they are sick? Yes.",
                "A crippling disease afflicted your meager troupe of creatures."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("WaterborneStart", new string[]
                {
                "Waterborne Cards?",
                "This is hard to explain...",
                "Perhaps a mutation? Yes.",
                "Your caravan of creatures had a startling mutation,",
                "They could only survive in the water."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("ShockedStart", new string[]
                {
                "Paralyzed Cards?",
                "Hmm...",
                "Your group of creatures were fatigued from the long journey,",
                "but there would be a long way to go yet."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("WeakSoulStart", new string[]
                {
                "Weak Souled Cards?",
                "Hmm...",
                "You will not be able to extract their souls into new creatures,",
                "For better or worse..."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("AscenderBaneStart", new string[]
                {
                "The lives of past challengers weighed down on you..."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("FamineIntro", new string[]
                {
                "Hm?",
                "How to explain this...",
                "You were running low on supplies that day...",
                "You'll have fewer [c:G][v:0]s[c:] to work with."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("AbundanceIntro", new string[]
                {
                "Hm?",
                "An abundance of [c:G][v:0]s[c:]...",
                "Must be mating season..."
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("EnvironmentsIntro", new string[]
                {
                "Hm?",
                "Environmental effects?",
                "how interesting...",
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("MudBoonIntro", new string[]
                {
                "In order to proceed,",
                "you had to slog through the wetter parts of the swamp.",
                "The thick [c:G]mud[c:] stuck to your boots, and hindered your movements..."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("HailBoonIntro", new string[]
                {
                "The frigid air was not the only obstacle in your way,",
                "the harsh ice and unforgiving snow also stood in your path.",
                "before long you found yourself in...",
                "a [c:B]hailstorm.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("CliffsBoonIntro", new string[]
                {
                "You found yourself cornered against a sheer rock wall,",
                "the [c:G]cliffside.[c:]",
                "Against the cold stone, there would be less space to fight."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("MushroomsBoonIntro", new string[]
                {
                "The mycologists had left one of their experiments behind,",
                "unbeknownst to them, it began to fester and grow.",
                "even the creatures fighting you would not be safe from the [c:G]fungal mass.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("DynamiteBoonIntro", new string[]
                {
                "You stumbled across one of the Prospector's Camps.",
                "The camp was filled with prospecting tools.",
                "pickaxes, headlamps...",
                "and [c:bR]dynamite.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("BaitBoonIntro", new string[]
                {
                "You came across one of the Angler's Ponds.",
                "the area stank of rotting fish,",
                "emanating from the nearby abandoned buckets of [c:dB]bait.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("TrapBoonIntro", new string[]
                {
                "You came across one of the Trapper's Hunting Grounds.",
                "the stench of blood hung in the air...",
                "you noticed the numerous [c:G]traps[c:] lying in wait."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("TotemBoonIntro", new string[]
                {
                "You came upon a strange [c:bR]totem.[c:]",
                "a mysterious energy swirled around it,",
                "the creatures nearby seem more agressive than usual..."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("BloodMoonBoonIntro", new string[]
                {
                "Ah...",
                "a [c:bR]Blood Moon.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("CarrotBoonIntro", new string[]
                {
                "Er...",
                "You...",
                "...",
                "...What?"
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("CarrotBoonIntro2", new string[]
                {
                "I am at a loss for words."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("BlizzardBoonIntro", new string[]
                {
                "The wind was howling around you.",
                "Stuck in the middle of a blizzard,",
                "You heard rumbling from the mountains above.",
                "Here it comes...",
                "an [c:B]avalanche.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("ObeliskBoonIntro", new string[]
                {
                "You stumble across a strange black stone.",
                "A stone tablet sits in front of it, clearly made for sacrifices.",
                "Perhaps a certain creature may cause a reaction?"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("WorthySacrifice", new string[]
                {
                "The obelisk trembles in delight.",
                "This is truly a worthy sacrifice."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("PeltSacrifice", new string[]
                {
                "The obelisk rumbles with anger.",
                "A pelt is a truly pitiful sacrifice."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("TerrainSacrifice", new string[]
                {
                "The obelisk rumbles with anger.",
                "Such a thing... does not bleed."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("NormalSacrifice", new string[]
                {
                "The obelisk does not react.",
                "Perhaps, something greater."
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("SquirrelSacrifice", new string[]
                {
                "...",
                "...That is not a proper sacrifice."
                }, new string[][]
                {
                new string[]
                {
                    "Stop this."
                },
                new string[]
                {
                    "You..."
                },
                new string[]
                {
                    "This is bloodshed without meaning."
                },
                new string[]
                {
                    "[v:0] squirrels..."
                }
                }, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("MinicelloBoonIntro", new string[]
                {
                "Hmm?",
                "What is this?"
                }, null, null, Emotion.Surprise, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("MinicelloBoonIntro2", new string[]
                {
                "Heh heh heh...",
                "Ye walked into th' pirate's cove!",
                "Me crew will get rid of ye quick!"
                }, null, null, Emotion.Laughter, "PirateSkullPreCharge");
                DialogueHelper.AddOrModifySimpleDialogEvent("DarkForestBoonIntro", new string[]
                {
                "You carve through the thick underbrush and foilage.",
                "The trees blotting out the sun...",
                "As you travel, you hear the woods start to creak around you.",
                "You should know better than to walk in the darkness..."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("FloodBoonIntro", new string[]
                {
                "As your caravan of creatures travels, you hear a rushing sound.",
                "You climb to a higher place as water flows around you.",
                "You are caught in a [c:B]flood.[c:]",
                "Only the terrain and flying creatures will be able to avoid the waters."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("BreezeBoonIntro", new string[]
                {
                "As your caravan of creatures moves across a clearing, the winds blow stronger.",
                "A strong breeze greets your face, and you grasp onto the surroundings for dear life.",
                "Your creatures are blown [c:G]airborne.[c:]",
                "Only burrowing and submerged creatures will be able to avoid the gusts."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("BreezeActivation", new string[]
                {
                "The Breeze blows..."
                }, null, null, null, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("GraveyardBoonIntro", new string[]
                {
                "You come across a pile of corpses.",
                "A strange energy swirls around them as you approach.",
                "Then...",
                "[c:R]The dead walk.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("GraveyardBoonIntro2", new string[]
                {
                "[c:R]The dead walk.[c:]"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("FlashGrowthBoonIntro", new string[]
                {
                "You came across an overgrown glade.",
                "The trees seemed taller, and stronger than usual.",
                "Your creatures grew faster as well.",
                "<color=#25C102>Flash Growth.</color>"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");
                DialogueHelper.AddOrModifySimpleDialogEvent("FlashGrowthBoonIntro2", new string[]
                {
                "<color=#25C102>Flash Growth.</color>"
                }, null, null, Emotion.Neutral, "NewRunDealtDeckDefault");



                ///----------------P03 Lines---------------
                ///P03 faces:
                ///		Default
                ///		Bored
                ///		Angry
                ///		Happy
                ///		Thinking
                DialogueHelper.AddDialogue("P03FamineIntro",
                new List<string> //dialogue
                {
                "Hm?",
                "You're decreasing the number of vessels you have?",
                "Surely a player as bad as you needs the extra chump blockers?",
                "You'll see."
                }, new List<string> //faces
                {
                "Default",
                "",
                "Happy",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "y",
                ""
                });
                DialogueHelper.AddDialogue("P03AbundanceIntro",
                new List<string> //dialogue
                {
                "Hm?",
                "You're increasing the number of vessels you have?",
                "Makes sense.",
                "After all, a player as bad as you needs the extra chump blockers.",
                "[v:0] vessels..."
                }, new List<string> //faces
                {
                "Default",
                "",
                "",
                "Happy",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "",
                "y",
                ""
                });
                DialogueHelper.AddDialogue("P03FecundityUnNerfIntro",
                new List<string> //dialogue
                {
                "Oh?",
                "You couldn't even stick to the changes?",
                "Pathetic."
                }, new List<string> //faces
                {
                "Default",
                "Happy",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03OuroIntro",
                new List<string> //dialogue
                {
                "Oh?",
                "My own Ourobot?",
                "You must be masochistic if you thought this was a good idea.",
                "Your funeral."
                }, new List<string> //faces
                {
                "Thinking",
                "Default",
                "Happy",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03OuroDies1",
                new List<string> //dialogue
                {
                "There it goes."
                }, new List<string> //faces
                {
                "Default"
                }, new List<string> //dialogue wavies
                {
                ""
                });
                DialogueHelper.AddDialogue("P03OuroDies2",
                new List<string> //dialogue
                {
                "You just made it stronger.",
                "Of course, you knew that already."
                }, new List<string> //faces
                {
                "Default",
                "Happy"
                }, new List<string> //dialogue wavies
                {
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03OuroDies3",
                new List<string> //dialogue
                {
                "I'm not worried.",
                "It'll come back to crush you later."
                }, new List<string> //faces
                {
                "Default",
                "Happy"
                }, new List<string> //dialogue wavies
                {
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03WeakStart",
                new List<string> //dialogue
                {
                "Hm?",
                "Weaker starting cards?",
                "As if one health will make a difference."
                }, new List<string> //faces
                {
                "Default",
                "",
                ""
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03EnvironmentsIntro",
                new List<string> //dialogue
                {
                "Hm?",
                "Were my environments not good enough for you?",
                "Fine."
                }, new List<string> //faces
                {
                "Default",
                "",
                ""
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03GraveyardBoonIntro",
                new List<string> //dialogue
                {
                "Hm...",
                "You have found a...",
                "Robot scrapyard.",
                "There's broken down robots everywhere.",
                "Every robot in this area dies twice."
                }, new List<string> //faces
                {
                "Thinking",
                "Default",
                "",
                "",
                ""
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03GraveyardBoonIntro2",
                new List<string> //dialogue
                {
                "It's the scrapyard again.",
                "You know the drill."
                }, new List<string> //faces
                {
                "Default",
                ""
                }, new List<string> //dialogue wavies
                {
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03FlashGrowthBoonIntro",
                new List<string> //dialogue
                {
                "Eugh.",
                "This is one of [c:O]HIS.[c:]",
                "Your... transformer bots will be more effective here.",
                "They'll transform when played.",
                "You'll see."
                }, new List<string> //faces
                {
                "Angry",
                "",
                "Default",
                "",
                ""
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03FlashGrowthBoonIntro2",
                new List<string> //dialogue
                {
                "Your transformer bots will be more effective here.",
                "They'll tranform when played."
                }, new List<string> //faces
                {
                "Default",
                ""
                }, new List<string> //dialogue wavies
                {
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03ConveyorBoonIntro",
                new List<string> //dialogue
                {
                "Ah...",
                "A favorite of mine.",
                "At the start of each of your turns all cards will be moved clockwise.",
                "I'm sure you've seen it before."
                }, new List<string> //faces
                {
                "Happy",
                "",
                "",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03ConveyorBoonIntro2",
                new List<string> //dialogue
                {
                "At the start of each of your turns all cards will be moved clockwise.",
                "You've seen it before."
                }, new List<string> //faces
                {
                "Default",
                ""
                }, new List<string> //dialogue wavies
                {
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03GemSanctuaryBoonIntro",
                new List<string> //dialogue
                {
                "Ah...",
                "Your <color=#25C102>G</color>[c:O]E[c:][c:B]M[c:]s will be more useful.",
                "More useful than [c:R]he[c:] ever was able to make them...",
                "...As long as you keep that one alive."
                }, new List<string> //faces
                {
                "Thinking",
                "Default",
                "Angry",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03GemSanctuaryBoonIntro2",
                new List<string> //dialogue
                {
                "Your <color=#25C102>G</color>[c:O]E[c:][c:B]M[c:]s will be more useful.",
                "...As long as you keep that one alive."
                }, new List<string> //faces
                {
                "Default",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03ElectricStormBoonIntro",
                new List<string> //dialogue
                {
                "Heh.",
                "This one will be quite...",
                "Shocking.",
                "You find yourself in an electrical storm.",
                "When a card is played, it will take 1 damage.",
                "If it survives, it'll be stronger for a bit."
                }, new List<string> //faces
                {
                "Happy",
                "",
                "Happy",
                "",
                "",
                "Default"
                }, new List<string> //dialogue wavies
                {
                "",
                "",
                "",
                "",
                "",
                ""
                });
                DialogueHelper.AddDialogue("P03ElectricStormBoonIntro2",
                new List<string> //dialogue
                {
                "I think you'll find this one quite...",
                "Shocking."
                }, new List<string> //faces
                {
                "Default",
                "Happy"
                }, new List<string> //dialogue wavies
                {
                "",
                ""
                });
            }
        }
        
    }
}