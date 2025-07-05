using System;
using R3;
using UnityEngine;

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
            model.Move
                .Subscribe(targetId =>
                {
                    // Notify the view to move to the target ID
                    view.SetTarget(targetId);
                })
                .AddTo(_disposables);

            view.OnMoveCompleted
                .Subscribe(isCompleted =>
                {
                    if (isCompleted)
                    {
                        model.SetState(Character.State.Rest);
                    }
                    else
                    {
                        model.SetState(Character.State.Idle);
                    }
                })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables = null;
        }
    }
}
