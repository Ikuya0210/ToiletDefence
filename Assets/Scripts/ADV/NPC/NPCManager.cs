using System.Threading;
using Cysharp.Threading.Tasks;
using PinballBenki.Input;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class NPCManager : MonoBehaviour
    {
        [SerializeField] private NPCCtrl[] _npcCtrls;
        private DialogueDirector _dialogueDirector;

        public void Init(IADVGUI aDVGUI)
        {
            foreach (var npcCtrl in _npcCtrls)
            {
                npcCtrl.Init();
                npcCtrl.OnTalkAsync = (text, ct) => TalkAsync(npcCtrl.Name, text, ct);
            }

            _dialogueDirector = new(aDVGUI, 10);
        }

        private UniTask TalkAsync(string npcName, string script, CancellationToken ct)
            => _dialogueDirector != null
                ? _dialogueDirector.TalkAsync(npcName, script, ct)
                : UniTask.CompletedTask;
    }
}
