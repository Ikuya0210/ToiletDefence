using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.Scene
{
    public interface IShareable
    {
        Type ShareType { get; }
    }

    public abstract class BaseShareableInitializer : MonoBehaviour
    {
        /// <summary>
        /// Shareableの初期化タイミング
        /// </summary>
        protected internal virtual InitTimings InitTiming => InitTimings.Last;

        public abstract IShareable Init();

        /// <summary>
        /// 遷移中のタスク
        /// <summary>
        public virtual Func<CancellationToken, UniTask> TransitionTask => null;

        public enum InitTimings
        {
            Last = 0,
            Data,
            Comunication,
            Basis,
            GUI,
        }
    }
}
