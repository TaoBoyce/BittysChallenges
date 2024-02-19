using DiskCardGame;
using InscryptionAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BittysChallenges
{
    public static class Tools
    {
        private static Assembly _assembly;
        public static Assembly CurrentAssembly => _assembly ??= Assembly.GetExecutingAssembly();

        public static Texture2D LoadTexture(string name)
        {
            return TextureHelper.GetImageAsTexture(name + (name.EndsWith(".png") ? "" : ".png"), CurrentAssembly);
        }
        public static Sprite LoadSprite(string name)
        {
            return TextureHelper.ConvertTexture(TextureHelper.GetImageAsTexture(name + (name.EndsWith(".png") ? "" : ".png"), CurrentAssembly));
        }
    }
}
