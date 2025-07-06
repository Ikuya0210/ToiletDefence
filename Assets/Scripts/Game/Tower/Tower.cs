using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class Tower : MonoBehaviour, ITakeDamage
    {
        public uint ID { get; private set; }
        [SerializeField] private HealthGauge _healthGauge;
        [SerializeField] private int maxHealth = 100;
        private int _currentHealth;

        public void Init()
        {
            ID = IDProvider.GenerateID();
            TargetJudge.Register(transform, ID, true);
            _currentHealth = maxHealth;
            _healthGauge.Init(maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            Debug.Log($"{gameObject.name} ダメージを受けた: {damage}");
            _currentHealth -= damage;
            _healthGauge.UpdateGauge(_currentHealth, maxHealth);
        }
    }
}
