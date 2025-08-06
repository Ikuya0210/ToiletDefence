using System;
using System.Collections.Generic;
using System.Threading;
using R3.Triggers;
using UnityEngine;
using R3;

namespace GGGameOver
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : PooledObject<T>
    {
        [SerializeField] private T _pooledObject;
        private Stack<T> _stack;

        protected void InitPool(int initSize)
        {
            _stack = new();
            for (int i = 0; i < initSize; i++)
            {
                var obj = GameObject.Instantiate(_pooledObject);
                obj.OnReleaseToPool = (o) => ReturnToPool(o);
                obj.gameObject.SetActive(false);
                _stack.Push(obj);
            }
        }

        protected T GetPooledObject()
        {
            if (_stack.Count == 0)
            {
                T obj = Instantiate(_pooledObject);
                obj.OnReleaseToPool = (o) => ReturnToPool(o);
                return obj;
            }
            T nextInstance = _stack.Pop();
            nextInstance.gameObject.SetActive(true);
            if (nextInstance._releaseCancellationTokenSource == null)
            {
                nextInstance._releaseCancellationTokenSource = new();
            }
            return nextInstance;
        }

        private void ReturnToPool(T pooledObject)
        {
            _stack.Push(pooledObject);
            pooledObject.gameObject.SetActive(false);
        }
    }

    public abstract class PooledObject<T> : MonoBehaviour where T : PooledObject<T>
    {
        internal Action<T> OnReleaseToPool;
        internal CancellationTokenSource _releaseCancellationTokenSource = new();
        public CancellationToken releaseCancellationToken => _releaseCancellationTokenSource.Token;

        protected void ReleaseToPool()
        {
            Cancel();
            OnReleaseToPool?.Invoke(this as T);
        }

        internal void Cancel()
        {
            if (_releaseCancellationTokenSource != null)
            {
                _releaseCancellationTokenSource.Cancel();
                _releaseCancellationTokenSource.Dispose();
                _releaseCancellationTokenSource = null;
            }
        }

        void OnDestroy()
        {
            Cancel();
        }
    }
}
