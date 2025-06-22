using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class NPCManager : MonoBehaviour
    {
        [SerializeField] private NPCCtrl[] _npcCtrls;

        public void Init()
        {
            foreach (var npcCtrl in _npcCtrls)
            {
                npcCtrl.Init();
                npcCtrl.OnTalkAsync = TalkAsync;
            }
        }

        private UniTask TalkAsync(string script, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}
