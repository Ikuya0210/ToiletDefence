using UnityEngine;
using UnityEngine.UI;

namespace GGGameOver.Toilet.Game
{
    public class HealthGauge : MonoBehaviour
    {
        private static readonly int ThresholdID = Shader.PropertyToID("_Threshold");
        [SerializeField] private SpriteRenderer _gaugeSprite;
        private Material _instancedMaterial;

        public void Init(float maxHealth)
        {
            _instancedMaterial = new Material(_gaugeSprite.material);
            _gaugeSprite.material = _instancedMaterial;

            UpdateGauge(maxHealth, maxHealth);
        }

        public void UpdateGauge(float currentHealth, float maxHealth)
        {
            if (_instancedMaterial == null)
            {
                Debug.LogError("マテリアルが初期化されていません。Initメソッドを呼び出してください。");
                return;
            }

            if (maxHealth <= 0)
            {
                Debug.LogWarning("最大体力が0以下です。ゲージを最大に設定します。");
                _instancedMaterial.SetFloat(ThresholdID, 1f);
                return;
            }

            float threshold = currentHealth / maxHealth;
            _instancedMaterial.SetFloat(ThresholdID, threshold);
        }


        private void OnDestroy()
        {
            if (_instancedMaterial != null)
            {
                Destroy(_instancedMaterial);
                _instancedMaterial = null;
            }
        }
    }
}
