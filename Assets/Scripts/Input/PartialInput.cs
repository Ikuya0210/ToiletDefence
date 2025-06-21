using System;
using System.Collections.Generic;
using System.Linq;
using PinballBenki.Scene;
using R3;
using UnityEngine;

namespace PinballBenki.Input
{
    /// <summary>
    /// 個別に入力を制御するためのクラス。
    /// </summary>
    public class PartialInput : IDisposable
    {
        public Observable<Unit> OnDecide => _inputProvider.OnDecide.Where(_ => IsValid());
        public Observable<Unit> OnBack => _inputProvider.OnBack.Where(_ => IsValid());
        public Observable<Unit> OnMenu => _inputProvider.OnMenu.Where(_ => IsValid());
        public Observable<Unit> OnFlip_L => _inputProvider.OnFlip_L.Where(_ => IsValid());
        public Observable<Unit> OnFlip_R => _inputProvider.OnFlip_R.Where(_ => IsValid());
        public Observable<Vector2> OnNavigate => _inputProvider.OnNavigate.Where(_ => IsValid());
        public Observable<Vector2> OnNavigate4 => _inputProvider.OnNavigate4.Where(_ => IsValid());
        public Observable<Vector2> OnNavigate8 => _inputProvider.OnNavigate8.Where(_ => IsValid());

        private InputProvider _inputProvider;
        private readonly uint _priority;
        private bool _active;

        private static readonly List<PartialInput> _nonZeroActivePartialInputs = new();

        public bool IsValid()
        {
            if (!_active || _nonZeroActivePartialInputs.Count == 0)
            {
                return _active; // No active partial inputs, valid state
            }
            return _nonZeroActivePartialInputs[0]._priority == this._priority; // Only valid if this is the highest priority active input
        }


        public PartialInput(uint priority = 0) : this(Shareables.Get<InputProvider>(), priority) { }

        public PartialInput(InputProvider inputProvider, uint priority = 0)
        {
            _inputProvider = inputProvider;
            _priority = priority;
            _active = true;
        }

        public void Activate()
        {
            if (_active)
            {
                return;
            }
            _active = true;
            if (_priority > 0)
            {
                _nonZeroActivePartialInputs.Add(this);
                _nonZeroActivePartialInputs.OrderByDescending(x => x._priority);
            }
        }

        public void Deactivate()
        {
            if (!_active)
            {
                return;
            }
            _active = false;
            if (_priority > 0)
            {
                _nonZeroActivePartialInputs.Remove(this);
                _nonZeroActivePartialInputs.OrderByDescending(x => x._priority);
            }
        }

        public void Dispose()
        {
            Deactivate();
            _inputProvider = null;
        }
    }
}
