using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

namespace PinballBenki.ADV
{
    public abstract class BaseNpcExecutable : ScriptExecuter.IExecutable
    {
        public virtual string Command => "none";
        protected readonly IADVDialogue _dialogue;
        private readonly Func<string> _npcNameFunc;
        protected string GetNpcName() => _npcNameFunc != null ? _npcNameFunc() : "NPC";

        protected BaseNpcExecutable(IADVGUI advgui, Func<string> npcNameFunc)
        {
            _dialogue = advgui.Dialogue;
            _npcNameFunc = npcNameFunc ?? throw new ArgumentNullException(nameof(npcNameFunc));
        }

        public virtual UniTask ExecuteAsync(string[] args, CancellationToken ct)
            => UniTask.CompletedTask;
    }

    public sealed class TextNPCExecutable : BaseNpcExecutable
    {
        public TextNPCExecutable(IADVGUI advgui, Func<string> npcNameFunc)
            : base(advgui, npcNameFunc) { }

        public override string Command => "talk";

        public override async UniTask ExecuteAsync(string[] args, CancellationToken ct)
        {
            if (args.Length < 1)
            {
                Debug.LogError("Talkコマンドの引数が不足しています。");
                return;
            }

            if (!_dialogue.IsVisible)
            {
                await _dialogue.ShowAsync(ct);
                await UniTask.DelayFrame(1, cancellationToken: ct); // Wait for the dialogue to be shown
            }
            string text = string.Join(" ", args);
            await _dialogue.SetTextAsync(GetNpcName(), text, ct);
        }
    }
}
