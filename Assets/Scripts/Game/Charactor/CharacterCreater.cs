using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterCreater : ObjectPool<CharacterView>
    {
        [SerializeField] private Transform _initialPosAnchor;
        public void Init()
        {
            InitPool(1);
        }

        public CharacterModel Create(CharacterEntity entity, Transform parent, bool isPlayers)
        {
            if (entity == null)
            {
                Debug.LogError("空のCharacterEntityが渡されたので、nullを返します。");
                return null;
            }
            uint id = IDProvider.GenerateID();

            var viewObj = GetPooledObject();
            viewObj.gameObject.layer = isPlayers ? Character.PlayerCharacterLayer : Character.EnemyCharacterLayer;
            viewObj.transform.SetParent(parent);
            viewObj.transform.position = _initialPosAnchor != null ? _initialPosAnchor.position : Vector3.zero;
            // 揺らぎを持たせる
            viewObj.transform.position += new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.8f, 0.8f),
                0
            );
            viewObj.transform.localRotation = Quaternion.identity;
            viewObj.name = $"{entity.Name}_{id}";
            TargetJudge.Register(viewObj.transform, id, isPlayers);

            var model = new CharacterModel(entity, id);
            viewObj.Init(entity, id, isPlayers);

            var presenter = new CharacterPresenter();
            presenter.RegisterTo(viewObj.releaseCancellationToken);
            presenter.Bind(model, viewObj);

            return model;
        }
    }
}
