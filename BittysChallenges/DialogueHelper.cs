using DiskCardGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BittysChallenges
{
    public static class DialogueHelper
    {
        public static DialogueEvent.LineSet CreateLineSet(string[] lineString, Emotion emotion = Emotion.Neutral, TextDisplayer.LetterAnimation animation = TextDisplayer.LetterAnimation.None, P03AnimationController.Face p03Face = P03AnimationController.Face.Default, int speakerIndex = 0)
        {
            return new DialogueEvent.LineSet
            {
                lines = (from s in lineString
                         select new DialogueEvent.Line
                         {
                             text = s,
                             emotion = emotion,
                             letterAnimation = animation,
                             p03Face = p03Face,
                             speakerIndex = speakerIndex
                         }).ToList()
            };
        }
        public static void AddDialogue(string id, List<string> lines, List<string> faces, List<string> dialogueWavies)
        {
            DialogueEvent.Speaker speaker = DialogueEvent.Speaker.P03;
            bool flag = faces.Exists((s) => s.ToLowerInvariant().Contains("leshy"));
            if (flag)
            {
                speaker = DialogueEvent.Speaker.Leshy;
            }
            else
            {
                bool flag2 = faces.Exists((s) => s.ToLowerInvariant().Contains("telegrapher"));
                if (flag2)
                {
                    speaker = DialogueEvent.Speaker.P03Telegrapher;
                }
                else
                {
                    bool flag3 = faces.Exists((s) => s.ToLowerInvariant().Contains("archivist"));
                    if (flag3)
                    {
                        speaker = DialogueEvent.Speaker.P03Archivist;
                    }
                    else
                    {
                        bool flag4 = faces.Exists((s) => s.ToLowerInvariant().Contains("photographer"));
                        if (flag4)
                        {
                            speaker = DialogueEvent.Speaker.P03Photographer;
                        }
                        else
                        {
                            bool flag5 = faces.Exists((s) => s.ToLowerInvariant().Contains("canvas"));
                            if (flag5)
                            {
                                speaker = DialogueEvent.Speaker.P03Canvas;
                            }
                            else
                            {
                                bool flag6 = faces.Exists((s) => s.ToLowerInvariant().Contains("goo"));
                                if (flag6)
                                {
                                    speaker = DialogueEvent.Speaker.Goo;
                                }
                                else
                                {
                                    bool flag7 = faces.Exists((s) => s.ToLowerInvariant().Contains("side"));
                                    if (flag7)
                                    {
                                        speaker = DialogueEvent.Speaker.P03MycologistSide;
                                    }
                                    else
                                    {
                                        bool flag8 = faces.Exists((s) => s.ToLowerInvariant().Contains("mycolo"));
                                        if (flag8)
                                        {
                                            speaker = DialogueEvent.Speaker.P03MycologistMain;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            bool leshy = speaker == DialogueEvent.Speaker.Leshy || speaker == DialogueEvent.Speaker.Goo;
            Emotion leshyEmotion = faces.Exists((s) => s.ToLowerInvariant().Contains("goocurious")) ? Emotion.Curious : Emotion.Neutral;
            bool flag9 = string.IsNullOrEmpty(id);
            if (!flag9)
            {
                List<DialogueEvent> events = DialogueDataUtil.Data.events;
                DialogueEvent dialogueEvent = new DialogueEvent();
                dialogueEvent.id = id;
                dialogueEvent.speakers = new List<DialogueEvent.Speaker>
                {
                    DialogueEvent.Speaker.Single,
                    speaker
                };
                dialogueEvent.mainLines = new DialogueEvent.LineSet(faces.Zip(lines, (face, line) => new DialogueEvent.Line
                {
                    text = line,
                    specialInstruction = "",
                    p03Face = leshy ? P03AnimationController.Face.NoChange : face.ParseFace(),
                    speakerIndex = 1,
                    emotion = leshy ? leshyEmotion : face.ParseFace().FaceEmotion()
                }).Zip(dialogueWavies, delegate (DialogueEvent.Line line, string wavy)
                {
                    bool flag10 = !string.IsNullOrEmpty(wavy) && wavy.ToLowerInvariant() == "y";
                    if (flag10)
                    {
                        line.letterAnimation = TextDisplayer.LetterAnimation.WavyJitter;
                    }
                    return line;
                }).ToList());
                events.Add(dialogueEvent);
            }
        }
        private static P03AnimationController.Face ParseFace(this string face)
        {
            bool flag = string.IsNullOrEmpty(face);
            P03AnimationController.Face result;
            if (flag)
            {
                result = P03AnimationController.Face.NoChange;
            }
            else
            {
                result = (P03AnimationController.Face)Enum.Parse(typeof(P03AnimationController.Face), face);
            }
            return result;
        }
        private static Emotion FaceEmotion(this P03AnimationController.Face face)
        {
            bool flag = face == P03AnimationController.Face.Angry;
            Emotion result;
            if (flag)
            {
                result = Emotion.Anger;
            }
            else
            {
                bool flag2 = face == P03AnimationController.Face.Thinking;
                if (flag2)
                {
                    result = Emotion.Curious;
                }
                else
                {
                    bool flag3 = face == P03AnimationController.Face.MycologistAngry;
                    if (flag3)
                    {
                        result = Emotion.Anger;
                    }
                    else
                    {
                        bool flag4 = face == P03AnimationController.Face.MycologistLaughing;
                        if (flag4)
                        {
                            result = Emotion.Laughter;
                        }
                        else
                        {
                            result = Emotion.Neutral;
                        }
                    }
                }
            }
            return result;
        }
        public static void AddOrModifySimpleDialogEvent(string eventId, string line, TextDisplayer.LetterAnimation? animation = null, Emotion? emotion = null)
        {
            string[] lines = new string[]
            {
                line
            };
            AddOrModifySimpleDialogEvent(eventId, lines, null, animation, emotion, "NewRunDealtDeckDefault");
        }
        private static void SyncLineCollection(List<DialogueEvent.Line> curLines, string[] newLines, TextDisplayer.LetterAnimation? animation, Emotion? emotion)
        {
            while (curLines.Count > newLines.Length)
            {
                curLines.RemoveAt(curLines.Count - 1);
            }
            for (int i = 0; i < curLines.Count; i++)
            {
                curLines[i].text = newLines[i];
            }
            for (int j = curLines.Count; j < newLines.Length; j++)
            {
                DialogueEvent.Line line = CloneLine(curLines[0]);
                line.text = newLines[j];
                bool flag = animation != null;
                if (flag)
                {
                    line.letterAnimation = animation.Value;
                }
                bool flag2 = emotion != null;
                if (flag2)
                {
                    line.emotion = emotion.Value;
                }
                curLines.Add(line);
            }
        }
        public static void AddOrModifySimpleDialogEvent(string eventId, string[] lines, string[][] repeatLines = null, TextDisplayer.LetterAnimation? animation = null, Emotion? emotion = null, string template = "NewRunDealtDeckDefault")
        {
            bool flag = false;
            DialogueEvent dialogueEvent = DialogueDataUtil.Data.GetEvent(eventId);
            bool flag2 = dialogueEvent == null;
            if (flag2)
            {
                flag = true;
                dialogueEvent = CloneDialogueEvent(DialogueDataUtil.Data.GetEvent(template), eventId, false);
                while (dialogueEvent.mainLines.lines.Count > lines.Length)
                {
                    dialogueEvent.mainLines.lines.RemoveAt(lines.Length);
                }
            }
            SyncLineCollection(dialogueEvent.mainLines.lines, lines, animation, emotion);
            bool flag3 = repeatLines == null;
            if (flag3)
            {
                dialogueEvent.repeatLines.Clear();
            }
            else
            {
                while (dialogueEvent.repeatLines.Count > repeatLines.Length)
                {
                    dialogueEvent.repeatLines.RemoveAt(dialogueEvent.repeatLines.Count - 1);
                }
                for (int i = 0; i < dialogueEvent.repeatLines.Count; i++)
                {
                    SyncLineCollection(dialogueEvent.repeatLines[i].lines, repeatLines[i], animation, emotion);
                }
            }
            bool flag4 = flag;
            if (flag4)
            {
                DialogueDataUtil.Data.events.Add(dialogueEvent);
            }
        }
        public static DialogueEvent.Line CloneLine(DialogueEvent.Line line)
        {
            return new DialogueEvent.Line
            {
                p03Face = line.p03Face,
                emotion = line.emotion,
                letterAnimation = line.letterAnimation,
                speakerIndex = line.speakerIndex,
                text = line.text,
                specialInstruction = line.specialInstruction,
                storyCondition = line.storyCondition,
                storyConditionMustBeMet = line.storyConditionMustBeMet
            };
        }
        public static DialogueEvent CloneDialogueEvent(DialogueEvent dialogueEvent, string newId, bool includeRepeat = false)
        {
            DialogueEvent dialogueEvent2 = new DialogueEvent
            {
                id = newId,
                groupId = dialogueEvent.groupId,
                mainLines = new DialogueEvent.LineSet(),
                speakers = new List<DialogueEvent.Speaker>(),
                repeatLines = new List<DialogueEvent.LineSet>()
            };
            foreach (DialogueEvent.Line line in dialogueEvent.mainLines.lines)
            {
                dialogueEvent2.mainLines.lines.Add(CloneLine(line));
            }
            if (includeRepeat)
            {
                foreach (DialogueEvent.LineSet lineSet in dialogueEvent.repeatLines)
                {
                    DialogueEvent.LineSet lineSet2 = new DialogueEvent.LineSet();
                    foreach (DialogueEvent.Line line2 in lineSet.lines)
                    {
                        lineSet2.lines.Add(CloneLine(line2));
                    }
                    dialogueEvent2.repeatLines.Add(lineSet2);
                }
            }
            foreach (DialogueEvent.Speaker item in dialogueEvent.speakers)
            {
                dialogueEvent2.speakers.Add(item);
            }
            return dialogueEvent2;
        }
    }
}