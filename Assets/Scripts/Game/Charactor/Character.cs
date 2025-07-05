using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

namespace GGGameOver.Toilet.Game
{
    public static class Character
    {
        private static uint _idCounter = 0;

        public static CharacterModel Create(CharacterEntity entity, Transform parent)
        {
            if (entity == null)
            {
                Debug.LogError("空のCharacterEntityが渡されたので、nullを返します。");
                return null;
            }
            _idCounter++;

            var obj = new GameObject(entity.Name);
            obj.transform.SetParent(parent);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = entity.Sprite;
            renderer.transform.localScale = entity.SpriteScale;

            var model = new CharacterModel(entity, _idCounter);
            var view = obj.AddComponent<CharacterView>();
            view.Init(entity, _idCounter);

            var presenter = new CharacterPresenter();
            presenter.AddTo(obj);
            presenter.Bind(model, view);

            return model;
        }
    }
}
