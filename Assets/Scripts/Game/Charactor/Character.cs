using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

namespace GGGameOver.Toilet.Game
{
    public static class Character
    {
        public static CharacterModel Create(CharacterEntity entity, Transform parent, bool isAlly)
        {
            if (entity == null)
            {
                Debug.LogError("空のCharacterEntityが渡されたので、nullを返します。");
                return null;
            }
            uint id = IDProvider.GenerateID();

            var obj = new GameObject(entity.Name);
            obj.transform.SetParent(parent);
            TargetJudge.Register(obj.transform, id, isAlly);

            var model = new CharacterModel(entity, id);
            var view = obj.AddComponent<CharacterView>();
            view.Init(entity, id);

            var presenter = new CharacterPresenter();
            presenter.AddTo(obj);
            presenter.Bind(model, view);

            return model;
        }

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
