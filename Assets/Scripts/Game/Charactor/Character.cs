using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

namespace GGGameOver.Toilet.Game
{
    public static class Character
    {
        public const float MinRestTime = 0.3f;
        public enum State
        {
            None = 0,
            Idle, // ターゲットがいない状態
            Moving, // ターゲットに向かって移動中
            Attacking, // ターゲットに攻撃中
            Rest, // 攻撃後などで休憩中
            Dead // 死亡状態
        }
    }
}
