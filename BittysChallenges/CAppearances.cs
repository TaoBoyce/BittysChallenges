using DiskCardGame;
using InscryptionAPI.Card;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BittysChallenges
{
    public class CAppearances
    {
        public class CloverAppearanceBehaviour : CardAppearanceBehaviour
        {
            Texture2D CardBG = Tools.LoadTexture("card_clover");
            public override void ApplyAppearance()
            {
                CardBG.filterMode = FilterMode.Point;
                base.Card.RenderInfo.baseTextureOverride = CardBG;
                base.Card.Info.AddDecal(CardBG);
            }
            public override void ResetAppearance()
            {
                Card.Info.Decals.Clear();
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance CloverAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "CloverAppearance", typeof(CloverAppearanceBehaviour)).Id;

        public class RedChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.darkRed);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance RedChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "RedChampAppearance", typeof(RedChampAppearanceBehaviour)).Id;
        public class YellowChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.yellow);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance YellowChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "YellowChampAppearance", typeof(YellowChampAppearanceBehaviour)).Id;
        public class OrangeChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.orange);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance OrangeChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "OrangeChampAppearance", typeof(OrangeChampAppearanceBehaviour)).Id;
        public class CyanChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(Color.cyan);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance CyanChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "CyanChampAppearance", typeof(CyanChampAppearanceBehaviour)).Id;
        public class WhiteChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.brightNearWhite);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance WhiteChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "WhiteChampAppearance", typeof(WhiteChampAppearanceBehaviour)).Id;
        public class MagentaChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(Color.magenta);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance MagentaChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "MagentaChampAppearance", typeof(MagentaChampAppearanceBehaviour)).Id;
        public class PurpleChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.darkPurple);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance PurpleChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "PurpleChampAppearance", typeof(PurpleChampAppearanceBehaviour)).Id;
        public class LightBlueChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.brightBlue);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance LightBlueChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "LightBlueChampAppearance", typeof(LightBlueChampAppearanceBehaviour)).Id;
        public class BrightRedChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.brightRed);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance BrightRedChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "BrightRedChampAppearance", typeof(BrightRedChampAppearanceBehaviour)).Id;
        public class LightGreenChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.brightLimeGreen);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance LightGreenChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "LightGreenChampAppearance", typeof(LightGreenChampAppearanceBehaviour)).Id;
        public class BlueChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.darkBlue);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance BlueChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "BlueChampAppearance", typeof(BlueChampAppearanceBehaviour)).Id;
        public class GreenChampAppearanceBehaviour : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                base.Card.renderInfo.forceEmissivePortrait = true;
                base.Card.StatsLayer.SetEmissionColor(GameColors.instance.darkLimeGreen);
            }
        }
        public readonly static CardAppearanceBehaviour.Appearance GreenChampAppearance = CardAppearanceBehaviourManager.Add(Plugin.PluginGuid, "GreenChampAppearance", typeof(GreenChampAppearanceBehaviour)).Id;
    }
}
