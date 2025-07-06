using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterCreater : ObjectPool<CharacterView>
    {
        public void Init()
        {
            InitPool(1);
        }

        public CharacterModel Create(CharacterEntity entity, Transform parent, bool isAlly)
        {
            if (entity == null)
            {
                Debug.LogError("空のCharacterEntityが渡されたので、nullを返します。");
                return null;
            }
            uint id = IDProvider.GenerateID();

            var viewObj = GetPooledObject();
            viewObj.transform.SetParent(parent);
            viewObj.name = $"{entity.Name}_{id}";
            TargetJudge.Register(viewObj.transform, id, isAlly);

            var model = new CharacterModel(entity, id);
            viewObj.Init(entity, id);

            var presenter = new CharacterPresenter();
            presenter.AddTo(viewObj);
            presenter.Bind(model, viewObj);

            return model;
        }
    }
}
