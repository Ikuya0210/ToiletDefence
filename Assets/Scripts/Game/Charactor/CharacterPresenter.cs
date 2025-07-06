using System;
using R3;

namespace GGGameOver.Toilet.Game
{
    public class CharacterPresenter : IDisposable
    {
        private CompositeDisposable _disposables;

        public CharacterPresenter()
        {
            _disposables = new();
        }

        public void Bind(CharacterModel model, CharacterView view)
        {
            // move
            model.Move
                .Subscribe(targetId => view.SetTarget(targetId))
                .AddTo(_disposables);

            view.OnMoveCompleted
                .Subscribe(isArrived => model.OnMoveCompleted(isArrived))
                .AddTo(_disposables);

            // attack
            model.Attack
                .Subscribe(attackPower => view.Attack(attackPower))
                .AddTo(_disposables);
            view.OnAttackCompleted
                .Subscribe(isAttackCompleted => model.OnAttackCompleted(isAttackCompleted))
                .AddTo(_disposables);

            // dead
            model.Dead
                .Subscribe(_ => view.Dead())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables = null;
        }
    }
}
