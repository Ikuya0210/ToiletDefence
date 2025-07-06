using System;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class PointManager : IDisposable
    {
        private readonly ReactiveProperty<int> _point;
        public ReadOnlyReactiveProperty<int> Point => _point;

        public PointManager()
        {
            _point = new(0);
        }

        public void AddPoint(int value)
        {
            _point.Value += value;
        }

        public bool TrySpendPoint(int value)
        {
            if (_point.Value >= value)
            {
                _point.Value -= value;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            _point?.Dispose();
        }
    }
}
