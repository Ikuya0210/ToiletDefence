using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class NPCCtrl : MonoBehaviour, ITalkable
    {
        public Func<string, CancellationToken, UniTask> OnTalkAsync { get; set; }
        public string Name => _name;

        [SerializeField] private string _name;
        [SerializeField] private Character _character;
        [SerializeField] private TextAsset _dialogueScript;

        public void Init()
        {
            _character.Init();
        }

        public async UniTask TalkAsync(Character character, CancellationToken ct)
        {
            if (OnTalkAsync == null || _dialogueScript == null)
            {
                return;
            }
            await OnTalkAsync(_dialogueScript.text, ct);
        }
    }
}
