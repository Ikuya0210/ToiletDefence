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
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _waterCost = 5;
        private int _currentHealth;

        public void Init()
        {
            ID = IDProvider.GenerateID();
            TargetJudge.Register(transform, ID, true);
            _currentHealth = _maxHealth;
            _healthGauge.Init(_maxHealth);

            InitPool(3);

            // 検索半径
            //float searchRadius = 1.0f;

            var input = Shareables.Get<InputProvider>();
            input.OnDecide
                .Subscribe(_ =>
                    {
                        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        var w = GetPooledObject();
                        w.transform.position = this.transform.position;
                        w.Launch(mouseWorldPos);
                        TakeDamage(_waterCost);
                        /*
                        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mouseWorldPos, searchRadius, 1 << Character.EnemyCharacterLayer);
                        foreach (var col in hitColliders)
                        {
                            if (col.TryGetComponent<ITakeDamage>(out var obj))
                            {
                                obj.TakeDamage(50);
                            }
                        }
                        */
                    })
                    .AddTo(this);

            // 1秒ごとに回復
            Observable.Interval(TimeSpan.FromSeconds(0.2f))
                .Subscribe(_ =>
                {
                    TakeDamage(-1); // 1回復
                })
                .AddTo(this);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _healthGauge.UpdateGauge(_currentHealth, _maxHealth);
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _healthGauge.UpdateGauge(0, _maxHealth);
                OnDead?.Invoke();
            }
        }
    }
}
