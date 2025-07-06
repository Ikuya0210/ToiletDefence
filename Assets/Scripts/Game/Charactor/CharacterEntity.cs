using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    [CreateAssetMenu(fileName = "CharacterEntity", menuName = "Game/CharacterEntity")]
    public class CharacterEntity : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Vector2 _spriteScale = Vector2.one;
        [SerializeField] private int _health = 100;
        [SerializeField] private int _attackPower = 10;
        [SerializeField] private float _moveSpeed = 0.1f;
        [SerializeField] private float _restTime = 1;

        public string Name => _name;
        public Sprite Sprite => _sprite;
        public Vector2 SpriteScale => _spriteScale;
        public int Health => _health;
        public int AttackPower => _attackPower;
        public float MoveSpeed => _moveSpeed;
        public float RestTime => _restTime;
    }
}
