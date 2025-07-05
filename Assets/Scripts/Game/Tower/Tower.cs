using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private HealthGauge _healthGauge;
        [SerializeField] private int maxHealth = 100;
        private int _currentHealth;

        public void Init()
        {
            _currentHealth = maxHealth;
            _healthGauge.Init(maxHealth);
        }
    }
}
