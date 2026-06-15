using DiskCardGame;
using HarmonyLib;
using I2.TextAnimation;
using InscryptionAPI.Dialogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace BittysChallenges
{
    [HarmonyPatch]
    public class Dialogue
    {
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
        public static Dictionary<Emotion, P03AnimationController.Face> P03_EmoteFaces = new Dictionary<Emotion, P03AnimationController.Face>()
        {
            { Emotion.None, P03AnimationController.Face.NoChange },
            { Emotion.Anger, P03AnimationController.Face.Angry },
            { Emotion.Laughter, P03AnimationController.Face.Happy },
            { Emotion.Curious, P03AnimationController.Face.Thinking },
            { Emotion.Neutral, P03AnimationController.Face.Default },
            { Emotion.Quiet, P03AnimationController.Face.Bored },
            { Emotion.Surprise, P03AnimationController.Face.Default }

        };
        private static CustomLine GenerateEmotionLine(string text, bool wavy = false, Emotion emotion = Emotion.Neutral, DialogueEvent.Speaker speaker = DialogueEvent.Speaker.Leshy, bool P03 = false, P03AnimationController.Face faceOverride = P03AnimationController.Face.NoChange)
        {

            TextDisplayer.LetterAnimation letterAnim = TextDisplayer.LetterAnimation.Jitter;
            if (wavy)
            {
                letterAnim = TextDisplayer.LetterAnimation.WavyJitter;
            }
            P03AnimationController.Face face = P03AnimationController.Face.NoChange;
            if (P03)
            {
                if(faceOverride != P03AnimationController.Face.NoChange)
                {
                    face = faceOverride;
                }
                else
                {
                    face = P03_EmoteFaces[emotion];
                }
            }
            CustomLine line = new CustomLine()
            {
                p03Face = face,
                emotion = emotion,
                letterAnimation = letterAnim,
                text = text,
                speaker = speaker
            };
            return line;
        }
        private static CustomLine GenerateP03EmotionLine(string text, bool wavy = false, Emotion emotion = Emotion.Anger, DialogueEvent.Speaker speaker = DialogueEvent.Speaker.Leshy, P03AnimationController.Face faceOverride = P03AnimationController.Face.NoChange)
        {
            return GenerateEmotionLine(text, wavy, emotion, speaker, true, faceOverride);
        }

        public static void Add_Dialogue()
        {
            Plugin.Log.LogInfo("Start of dialogue");

            #region Base KCM
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "RedrawHandIntro",
            new List<CustomLine>
            {
                    "Is that a clover?",
                    "I see."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "UnfairHandIntro",
            new List<CustomLine>
            {
                "Oh?",
                "Did you not like having a stacked deck?",
                "That's fine with me.",
                "It will be more fair this way anyways."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "OuroIntro",
            new List<CustomLine>
            {
                "Oh?",
                "My very own Ouroboros.",
                "I will be sure to put it to good use."
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                "The ouroboros has followed you.",
                "It's fangs are beared, and it's ready for another fight."
                },
                new List<CustomLine>{
                "the unyielding ouroboros has returned,",
                "growing stronger every death."
                },
                new List<CustomLine>{
                "as inevitable as death,",
                "the ouroboros has returned."
                },
                new List<CustomLine>{
                "a serpent slithers out from the undergrowth.",
                "perhaps you have seen it before?"
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "OuroDies",
            new List<CustomLine>
            {
                "the ouroboros has died.",
                "and yet, it will return."
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                "the ouroboros has died.",
                "and yet, it will return."
                },
                new List<CustomLine>{
                "the serpent's wrath has been delayed.",
                "for now."
                },
                new List<CustomLine>{
                "the ouroboros only grows stronger from death.",
                "it will be back."
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "MycoFailSigils",
            new List<CustomLine>
            {
                "a-ah, the sigils..."
            }, defaultSpeaker: DialogueEvent.Speaker.Mushroom);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "MycoFailAttack",
            new List<CustomLine>
            {
                "o-oh, the power..."
            }, defaultSpeaker: DialogueEvent.Speaker.Mushroom);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "MycoFailHealth",
            new List<CustomLine>
            {
                "a-ah, the health..."
            }, defaultSpeaker: DialogueEvent.Speaker.Mushroom);

            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "GoldenSheepIntro",
            new List<CustomLine>
            {
                "ah...",
                "Chrysomallos, the golden ram.",
                "what a glorious pelt...."
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                "The golden ram.",
                "catch it before it escapes."
                },
                new List<CustomLine>{
                "a rare sight.",
                "it will not stay for long."
                },
                new List<CustomLine>{
                "what glittering wool,",
                "attached to such a rare creature."
                },
                new List<CustomLine>{
                "oh?",
                "a rare chance to get a rare pelt.",
                "best make the most of it."
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "SheepDies",
            new List<CustomLine>
            {
                "You slay the glimmering creature, stealing its pelt."
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                "You slay the glimmering creature, stealing its pelt."
                },
                new List<CustomLine>{
                "The end of such a glorious creature.",
                "And what a glorious pelt you have obtained."
                },
                new List<CustomLine>{
                "Chrysomallos...",
                "How tragic your tale is...",
                "To be killed for your pelt..."
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "SheepEscapes",
            new List<CustomLine>
            {
                "The Golden Ram lives another day."
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                "The Golden Ram lives another day."
                },
                new List<CustomLine>{
                "Time's up.",
                "The Golden Ram has found an escape route."
                },
                new List<CustomLine>{
                "The glimmer of the Golden Ram's fur blinds you,",
                "giving it the opportunity to escape."
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "FragileEnemy",
            new List<CustomLine>
            {
                "Ah...",
                "Your [v:0] has taken a devastating blow from the [v:1].",
                "You will not be able to take it with your caravan."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "FragileDies",
            new List<CustomLine>
            {
                "Ah...",
                "You won't be seeing your [v:0] again,",
                "It has taken too much damage."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "FragileSacrifice",
            new List<CustomLine>
            {
                "Ah...",
                "Did you permanently kill your [v:0] on purpose?",
                "A shame..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "RoyalFirstMate",
            new List<CustomLine>
            {
                "Ha! There be my first mate!"
            }, defaultSpeaker: DialogueEvent.Speaker.PirateSkull);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "RoyalOuro",
            new List<CustomLine>
            {
                "Argh! There be snakes on me ship as well!"
            }, defaultSpeaker: DialogueEvent.Speaker.PirateSkull);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "RoyalOuroDiesPlayer",
            new List<CustomLine>
            {
                "Ha!",
                "I should hire ye to get rid of the rest of them!"
            }, defaultSpeaker: DialogueEvent.Speaker.PirateSkull);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "RoyalOuroDies",
            new List<CustomLine>
            {
                "Ha! It's dead!"
            }, defaultSpeaker: DialogueEvent.Speaker.PirateSkull);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "PirateIntro",
            new List<CustomLine>
            {
                "what?",
                "Pirates?",
                "...",
                "I'll allow it."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "InfiniteLivesIntro",
            new List<CustomLine>
            {
                GenerateEmotionLine("what?", true, Emotion.Anger),
                GenerateEmotionLine("cheating?", true, Emotion.Anger),
                GenerateEmotionLine("how dissapointing...", true, Emotion.Anger),
                GenerateEmotionLine("...", true, Emotion.Anger),
                GenerateEmotionLine("the game will be soured from a pitifully easy ascent.", false, Emotion.Anger)
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                    GenerateEmotionLine("cheater...", true, Emotion.Anger)
                },
                new List<CustomLine>{
                    GenerateEmotionLine("Have you no shame?", true, Emotion.Anger)
                },
                new List<CustomLine>{
                    GenerateEmotionLine("To cheat so blatantly...", true, Emotion.Anger)
                },
                new List<CustomLine>{
                    GenerateEmotionLine("you should be dead.", true, Emotion.Anger)
                },
                new List<CustomLine>{
                    GenerateEmotionLine("and yet...", true, Emotion.Anger)
                },
                new List<CustomLine>{
                    GenerateEmotionLine("how dissapointing...", true, Emotion.Anger)
                },
                new List<CustomLine>{
                    GenerateEmotionLine("...", true, Emotion.Anger)
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "InfiniteLivesLoop",
            new List<CustomLine>
            {
                GenerateEmotionLine("I won't tolerate this for much longer...", true, Emotion.Anger)
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "InfiniteLivesLoopBreak",
            new List<CustomLine>
            {
                GenerateEmotionLine("Enough of this.", false, Emotion.Anger)
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "InfiniteLivesRoyal",
            new List<CustomLine>
            {
                new CustomLine
                {
                    emotion = Emotion.Curious,
                    speaker = DialogueEvent.Speaker.PirateSkull,
                    text = "Yer not dead?"
                }
            });

            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "FecundityUnNerfIntro",
            new List<CustomLine>
            {
                "ah...",
                "back to normal then?",
                "I'll admit, I had gotten used to the changes..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "WeakStart",
            new List<CustomLine>
            {
                GenerateEmotionLine("Weak Cards?", emotion: Emotion.Curious),
                "This is hard to explain...",
                "Perhaps they are sick?",
                "Yes...",
                "A crippling disease afflicted your meager troupe of creatures."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "WaterborneStart",
            new List<CustomLine>
            {
                GenerateEmotionLine("Waterborne Cards?", emotion: Emotion.Curious),
                "This is hard to explain...",
                "Perhaps a mutation?",
                "Yes...",
                "Your caravan of creatures had a startling mutation,",
                "They could only survive in the water."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "ShockedStart",
            new List<CustomLine>
            {
                GenerateEmotionLine("Paralyzed Cards?", emotion : Emotion.Curious),
                "Hmm...",
                "Your group of creatures were fatigued from the long journey,",
                "but there would be a long way to go yet."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "WeakSoulStart",
            new List<CustomLine>
            {
                GenerateEmotionLine("Weak Souled Cards?", emotion : Emotion.Curious),
                "Hmm...",
                "You will not be able to extract their souls into new creatures,",
                "For better or worse..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "AscenderBaneStart",
            new List<CustomLine>
            {
                "The lives of past challengers weighed down on you..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "FamineIntro",
            new List<CustomLine>
            {
                GenerateEmotionLine("Hm?", emotion : Emotion.Curious),
                "How to explain this...",
                "You were running low on supplies that day...",
                "You'll have fewer [c:G][v:0]s[c:] to work with."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "AbundanceIntro",
            new List<CustomLine>
            {
                GenerateEmotionLine("Hm?", emotion : Emotion.Curious),
                "An abundance of [c:G][v:0]s[c:]...",
                "Must be mating season..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "EnvironmentsIntro",
            new List<CustomLine>
            {
                GenerateEmotionLine("Hm?", emotion : Emotion.Curious),
                "Environmental effects?",
                "how interesting..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "MudBoonIntro",
            new List<CustomLine>
            {
                "In order to proceed,",
                "you had to slog through the wetter parts of the swamp.",
                "The thick [c:G]mud[c:] stuck to your boots, and hindered your movements..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "HailBoonIntro",
            new List<CustomLine>
            {
                "The frigid air was not the only obstacle in your way,",
                "the harsh ice and unforgiving snow also stood in your path.",
                "before long you found yourself in...",
                "a [c:B]hailstorm.[c:]"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "CliffsBoonIntro",
            new List<CustomLine>
            {
                "You found yourself cornered against a sheer rock wall,",
                "the [c:G]cliffside.[c:]",
                "Against the cold stone, there would be less space to fight."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "MushroomsBoonIntro",
            new List<CustomLine>
            {
                "The mycologists had left one of their experiments behind,",
                "unbeknownst to them, it began to fester and grow.",
                "even the creatures fighting you would not be safe from the [c:G]fungal mass.[c:]"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "DynamiteBoonIntro",
            new List<CustomLine>
            {
                "You stumbled across one of the Prospector's Camps.",
                "The camp was filled with prospecting tools.",
                "pickaxes, headlamps...",
                "and [c:bR]dynamite.[c:]"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "BaitBoonIntro",
            new List<CustomLine>
            {
                "You came across one of the Angler's Ponds.",
                "the area stank of rotting fish,",
                "emanating from the nearby abandoned buckets of [c:dB]bait.[c:]"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "TrapBoonIntro",
            new List<CustomLine>
            {
                "You came across one of the Trapper's Hunting Grounds.",
                "the stench of blood hung in the air...",
                "you noticed the numerous [c:G]traps[c:] lying in wait."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "TotemBoonIntro",
            new List<CustomLine>
            {
                "You came upon a strange [c:bR]totem.[c:]",
                "a mysterious energy swirled around it,",
                "the creatures nearby seem more agressive than usual..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "BloodMoonBoonIntro",
            new List<CustomLine>
            {
                "Ah...",
                "a [c:bR]Blood Moon.[c:]"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "CarrotBoonIntro",
            new List<CustomLine>
            {
                "Er...",
                "You...",
                "...",
                "...What?"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "CarrotBoonIntro2",
            new List<CustomLine>
            {
                "I am at a loss for words."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "BlizzardBoonIntro",
            new List<CustomLine>
            {
                "The wind was howling around you.",
                "Stuck in the middle of a blizzard,",
                "You heard rumbling from the mountains above.",
                "Here it comes...",
                "an [c:B]avalanche.[c:]"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "ObeliskBoonIntro",
            new List<CustomLine>
            {
                "You stumble across a strange black stone.",
                "A stone tablet sits in front of it, clearly made for sacrifices.",
                "Perhaps a certain creature may cause a reaction?"
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "WorthySacrifice",
            new List<CustomLine>
            {
                "The obelisk trembles in delight.",
                "This is truly a worthy sacrifice."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "PeltSacrifice",
            new List<CustomLine>
            {
                "The obelisk rumbles with anger.",
                "A pelt is a truly pitiful sacrifice."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "TerrainSacrifice",
            new List<CustomLine>
            {
                "The obelisk rumbles with anger.",
                "Such a thing... does not bleed."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "NormalSacrifice",
            new List<CustomLine>
            {
                "The obelisk does not react.",
                "Perhaps, something greater."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "SquirrelSacrifice",
            new List<CustomLine>
            {
                "The obelisk does not react.",
                "Perhaps, something greater."
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                    "...",
                    "...That is not a proper sacrifice."
                }, 
                new List<CustomLine> {
                    "Stop this."
                },
                new List<CustomLine> {
                    "You..."
                },
                new List<CustomLine> {
                    "This is bloodshed without meaning."
                },
                new List<CustomLine> {
                    "[v:0] squirrels..."
                }
            }, DialogueEvent.MaxRepeatsBehaviour.PlayLastLine);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "MinicelloBoonIntro",
            new List<CustomLine>
            {
                GenerateEmotionLine("Hmm?",emotion: Emotion.Curious),
                GenerateEmotionLine("What is this?", emotion: Emotion.Curious),
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                    GenerateEmotionLine("Heh heh heh...", true, Emotion.Laughter, DialogueEvent.Speaker.PirateSkull),
                    GenerateEmotionLine("Ye walked into th' pirate's cove!", true, Emotion.Laughter, DialogueEvent.Speaker.PirateSkull),
                    GenerateEmotionLine("Me crew will get rid of ye quick!", false, Emotion.Laughter, DialogueEvent.Speaker.PirateSkull),
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "DarkForestBoonIntro",
            new List<CustomLine>
            {
                "You carve through the thick underbrush and foilage.",
                "The trees blotting out the sun...",
                "As you travel, you hear the woods start to creak around you.",
                "You should know better than to walk in the darkness..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "FloodBoonIntro",
            new List<CustomLine>
            {
                "As your caravan of creatures travels, you hear a rushing sound.",
                "You climb to a higher place as water flows around you.",
                "You are caught in a [c:B]flood.[c:]",
                "Only the terrain and flying creatures will be able to avoid the waters."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "BreezeBoonIntro",
            new List<CustomLine>
            {
                "As your caravan of creatures moves across a clearing, the winds blow stronger.",
                "A strong breeze greets your face, and you grasp onto the surroundings for dear life.",
                "Your creatures are blown [c:G]airborne.[c:]",
                "Only burrowing and submerged creatures will be able to avoid the gusts."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "BreezeActivation",
            new List<CustomLine>
            {
                "The Breeze blows..."
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "GraveyardBoonIntro",
            new List<CustomLine>
            {
                "You come across a pile of corpses.",
                "A strange energy swirls around them as you approach.",
                "Then...",
                "[c:R]The dead walk.[c:]"
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                    "[c:R]The dead walk.[c:]"
                }
            }, DialogueEvent.MaxRepeatsBehaviour.PlayLastLine);
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "FlashGrowthBoonIntro",
            new List<CustomLine>
            {
                "You came across an overgrown glade.",
                "The trees seemed taller, and stronger than usual.",
                "Your creatures grew faster as well.",
                "The glade glowed with unnatural light.",
                "<color=#25C102>Flash Growth.</color>"
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine> {
                    "The glade glowed with unnatural light.",
                    "<color=#25C102>Flash Growth.</color>"
                }
            }, DialogueEvent.MaxRepeatsBehaviour.PlayLastLine);
            #endregion
            #region P03 KCM
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03FamineIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Hm?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("You're decreasing the number of vessels you have?", emotion: Emotion.Neutral),
                GenerateP03EmotionLine("Surely a player as bad as you would need the extra chump blockers?", true, Emotion.Laughter),
                GenerateP03EmotionLine("I guess we'll see.", emotion: Emotion.Neutral)
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03AbundanceIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Hm?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("You're increasing the number of vessels you have?"),
                GenerateP03EmotionLine("Makes sense."),
                GenerateP03EmotionLine("After all, a player as bad as you needs the extra chump blockers.", true, emotion: Emotion.Laughter)
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03FecundityUnNerfIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Oh?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("You couldn't even stick to the changes?"),
                GenerateP03EmotionLine("Pathetic."),
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03OuroIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Oh?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("My own Ourobot?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("You must be masochistic if you thought this was a good idea.", true, emotion:Emotion.Laughter),
                GenerateP03EmotionLine("Your funeral.")
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03OuroDies",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("There it goes.")
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("There it goes.")
                },
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("You just made it stronger."),
                    GenerateP03EmotionLine("Of course, you knew that already.", true, Emotion.Laughter)
                },
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("I'm not worried."),
                    GenerateP03EmotionLine("It'll come back to crush you later.", true, Emotion.Laughter)
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03WeakStart",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Hm?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("Weaker starting cards?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("As if one health will make a difference.")
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03EnvironmentsIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Hm?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("Were my environments not good enough for you?", emotion: Emotion.Curious),
                GenerateP03EmotionLine("Fine.")
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03GraveyardBoonIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Hm...", emotion: Emotion.Quiet),
                GenerateP03EmotionLine("You have found a..."),
                GenerateP03EmotionLine("Robot scrapyard."),
                GenerateP03EmotionLine("There's broken down robots everywhere."),
                GenerateP03EmotionLine("Every robot in this area dies twice."),
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("It's the scrapyard again."),
                    GenerateP03EmotionLine("You know the drill.")
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03FlashGrowthBoonIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Eugh.", emotion: Emotion.Anger),
                GenerateP03EmotionLine("This is one of [c:O]HIS.[c:]", emotion: Emotion.Anger),
                GenerateP03EmotionLine("Your... transformer bots will be more effective here."),
                GenerateP03EmotionLine("They'll transform when played."),
                GenerateP03EmotionLine("You'll see.")
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("Your transformer bots will be more effective here."),
                    GenerateP03EmotionLine("They'll transform when played.")
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03GemSanctuaryBoonIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Ah...", emotion: Emotion.Quiet),
                GenerateP03EmotionLine("Your <color=#25C102>G</color>[c:O]E[c:][c:B]M[c:]s will be more useful."),
                GenerateP03EmotionLine("More useful than [c:R]he[c:] ever was able to make them...", emotion: Emotion.Anger),
                GenerateP03EmotionLine("...As long as you keep that one alive.")
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("Your <color=#25C102>G</color>[c:O]E[c:][c:B]M[c:]s will be more useful."),
                    GenerateP03EmotionLine("...As long as you keep that one alive.")
                }
            });
            DialogueManager.GenerateEvent(Plugin.PluginGuid,
            "P03ElectricStormBoonIntro",
            new List<CustomLine>
            {
                GenerateP03EmotionLine("Heh.", emotion: Emotion.Laughter),
                GenerateP03EmotionLine("This one will be quite..."),
                GenerateP03EmotionLine("Shocking.", emotion: Emotion.Laughter),
                GenerateP03EmotionLine("You find yourself in an electrical storm."),
                GenerateP03EmotionLine("The cards you play will be shocked."),
                GenerateP03EmotionLine("If they survive, they'll be stronger for a bit.")
            },
            new List<List<CustomLine>>
            {
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("Shocking, heh.", emotion: Emotion.Laughter),
                    GenerateP03EmotionLine("Still funny.", emotion: Emotion.Laughter),
                },
                new List<CustomLine>
                {
                    GenerateP03EmotionLine("The cards you play will be shocked."),
                    GenerateP03EmotionLine("If they survive, they'll be stronger for a bit.")
                }
            }, DialogueEvent.MaxRepeatsBehaviour.PlayLastLine);
            #endregion

            Plugin.Log.LogInfo("End of dialogue");
        }
    }
}