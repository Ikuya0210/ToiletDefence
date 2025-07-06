using System;
using UnityEngine;
using R3;

namespace GGGameOver.Toilet.Game
{
    public class Tower : ObjectPool<Water>, ITakeDamage
    {
        public Action OnDead;
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

            InitPool(3);

            // 検索半径
            float searchRadius = 1.0f;

            var input = Shareables.Get<InputProvider>();
            input.OnDecide
                .Subscribe(_ =>
                    {
                        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        var w = GetPooledObject();
                        w.transform.position = this.transform.position;
                        w.Launch(mouseWorldPos);
                        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mouseWorldPos, searchRadius, 1 << Character.EnemyCharacterLayer);

                        foreach (var col in hitColliders)
                        {
                            if (col.TryGetComponent<ITakeDamage>(out var obj))
                            {
                                obj.TakeDamage(50);
                            }
                        }
                    })
                    .AddTo(this);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            _currentHealth -= damage;
            _healthGauge.UpdateGauge(_currentHealth, maxHealth);
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _healthGauge.UpdateGauge(0, maxHealth);
                OnDead?.Invoke();
            }
        }
    }
}
