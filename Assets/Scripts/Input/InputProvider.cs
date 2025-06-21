using System;
using PinballBenki.Scene;
using R3;
using UnityEngine;

namespace PinballBenki.Input
{
    public sealed class InputProvider : IDisposable, IShareable
    {
        public Type ShareType => typeof(InputProvider);

        private Subject<Unit> _onDecide;
        private Subject<Unit> _onBack;
        private Subject<Unit> _onMenu;
        private Subject<Unit> _onFlip_L;
        private Subject<Unit> _onFlip_R;
        private Subject<Vector2> _onNavigate;

        public Observable<Unit> OnDecide => _onDecide;
        public Observable<Unit> OnBack => _onBack;
        public Observable<Unit> OnMenu => _onMenu;
        public Observable<Unit> OnFlip_L => _onFlip_L;
        public Observable<Unit> OnFlip_R => _onFlip_R;
        public Observable<Vector2> OnNavigate => _onNavigate;
        public Observable<Vector2> OnNavigate4 => _onNavigate.Select(v2 =>
        {
            return new Vector2(
                Mathf.RoundToInt(v2.x),
                Mathf.RoundToInt(v2.y));
        });
        public Observable<Vector2> OnNavigate8 => _onNavigate.Select(v2 =>
        {
            return new Vector2(
                Mathf.RoundToInt(v2.x * 0.5f) * 2,
                Mathf.RoundToInt(v2.y * 0.5f) * 2);
        });

        private readonly InputActions _inputActions;
        private readonly CompositeDisposable _disposables;

        internal InputProvider()
        {
            _inputActions = new();
            _inputActions.Enable();

            _disposables = new();

            _onDecide = new();
            _onBack = new();
            _onMenu = new();
            _onFlip_L = new();
            _onFlip_R = new();
            _onNavigate = new();

            _disposables.Add(_onDecide);
            _disposables.Add(_onBack);
            _disposables.Add(_onMenu);
            _disposables.Add(_onFlip_L);
            _disposables.Add(_onFlip_R);
            _disposables.Add(_onNavigate);

            _inputActions.Player.Decide.performed += ctx => _onDecide.OnNext(Unit.Default);
            _inputActions.Player.Back.performed += ctx => _onBack.OnNext(Unit.Default);
            _inputActions.Player.Menu.performed += ctx => _onMenu.OnNext(Unit.Default);
            _inputActions.Player.Flip_L.performed += ctx => _onFlip_L.OnNext(Unit.Default);
            _inputActions.Player.Flip_R.performed += ctx => _onFlip_R.OnNext(Unit.Default);
            _inputActions.Player.Navigate.performed += ctx => _onNavigate.OnNext(ctx.ReadValue<Vector2>());
            _inputActions.Player.Navigate.canceled += ctx => _onNavigate.OnNext(Vector2.zero);
        }

        public void Dispose()
        {
            _inputActions.Disable();
            _inputActions.Dispose();
        }
    }
}
