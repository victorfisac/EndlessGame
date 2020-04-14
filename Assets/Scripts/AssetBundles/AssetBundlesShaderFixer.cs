using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.AssetBundles
{
    public class AssetBundlesShaderFixer : MonoBehaviour
    {
        #if UNITY_EDITOR
        private void Awake()
        {
            Graphic[] _graphics = GetComponentsInChildren<Graphic>();

            for (int i = 0, count = _graphics.Length; i < count; i++)
            {
                Graphic _graphic = _graphics[i];
                
                if (_graphic.material == null)
                    continue;
                
                if (_graphic.material.shader == null)
                    continue;

                _graphic.material.shader = Shader.Find(_graphic.material.shader.name);
            }

            ParticleSystemRenderer[] _particles = GetComponentsInChildren<ParticleSystemRenderer>();

            for (int i = 0, count = _particles.Length; i < count; i++)
            {
                ParticleSystemRenderer _particle = _particles[i];
                
                if (_particle.material == null)
                    continue;
                
                if (_particle.material.shader == null)
                    continue;

                _particle.material.shader = Shader.Find(_particle.material.shader.name);
            }
        }
        #endif
    }
}