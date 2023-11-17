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
        public static void SpawnParticlesOnCard(this PlayableCard target, Texture2D tex, Color color, bool reduceY = false)
        {
            PaperCardAnimationController anim = target.Anim as PaperCardAnimationController;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(anim.deathParticles);
            ParticleSystem particle = gameObject.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainMod = particle.main;
            particle.startColor = color;
            particle.GetComponent<ParticleSystemRenderer>().material = new Material(particle.GetComponent<ParticleSystemRenderer>().material) { mainTexture = tex, color = color, };
            mainMod.startColor = new ParticleSystem.MinMaxGradient(color);
            gameObject.SetActive(true);
            gameObject.transform.SetParent(anim.transform);
            gameObject.transform.position = anim.deathParticles.transform.position;
            gameObject.transform.localScale = anim.deathParticles.transform.localScale;
            gameObject.transform.rotation = anim.deathParticles.transform.rotation;
            if (reduceY)
            {
                particle.transform.position = new Vector3(particle.transform.position.x, particle.transform.position.y - 0.1f, particle.transform.position.z);
            }
            UnityEngine.Object.Destroy(gameObject, 6f);
        }
    }
}
