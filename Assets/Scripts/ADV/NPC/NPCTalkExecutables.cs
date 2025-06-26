using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

namespace PinballBenki.ADV
{
    /// <summary>
    /// 基盤
    /// </summary>
    internal abstract class BaseNpcExecutable : ScriptExecuter.IExecutable
    {
        public virtual string Command => "none";
        protected readonly IADVDialogue _dialogue;
        private readonly Func<string> _npcNameFunc;
        protected string GetNpcName() => _npcNameFunc != null ? _npcNameFunc() : "NPC";
        protected ScriptExecuter.IOwner _owner;

        protected BaseNpcExecutable(IADVGUI advgui, Func<string> npcNameFunc)
        {
            _dialogue = advgui.Dialogue;
            _npcNameFunc = npcNameFunc ?? throw new ArgumentNullException(nameof(npcNameFunc));
        }

        void ScriptExecuter.IExecutable.SetOwner(ScriptExecuter.IOwner owner)
        {
            _owner = owner;
        }

        public virtual UniTask<int> ExecuteAsync(string[] args, CancellationToken ct)
            => UniTask.FromResult(0);
    }

    /// <summary>
    /// TALKコマンドの実行クラス
    /// </summary>
    internal sealed class TextNPCExecutable : BaseNpcExecutable
    {
        public TextNPCExecutable(IADVGUI advgui, Func<string> npcNameFunc)
            : base(advgui, npcNameFunc) { }

        public override string Command => "talk";

        public override async UniTask<int> ExecuteAsync(string[] args, CancellationToken ct)
        {
            if (args.Length < 1)
            {
                Debug.LogError("Talkコマンドの引数が不足しています。");
                return 0;
            }

            if (!_dialogue.IsVisible)
            {
                await _dialogue.ShowAsync(ct);
                await UniTask.DelayFrame(1, cancellationToken: ct); // Wait for the dialogue to be shown
            }
            string text = string.Join(" ", args);
            _owner.WaitNext();
            await _dialogue.SetTextAsync(GetNpcName(), text, ct);
            return 0;
        }
    }

    /// <summary>
    /// SELECTコマンドの実行クラス
    /// </summary>
    internal sealed class SelectNPCExecutable : BaseNpcExecutable
    {
        public SelectNPCExecutable(IADVGUI advgui, Func<string> npcNameFunc)
            : base(advgui, npcNameFunc) { }

        public override string Command => "select";

        public override async UniTask<int> ExecuteAsync(string[] args, CancellationToken ct)
        {
            if (args.Length < 2)
            {
                Debug.LogError("Talkコマンドの引数が不足しています。");
                return 0;
            }

            if (!_dialogue.IsVisible)
            {
                await _dialogue.ShowAsync(ct);
                await UniTask.DelayFrame(1, cancellationToken: ct); // Wait for the dialogue to be shown
            }
            int num = await _dialogue.SelectAsync(args, ct);
            return num;
        }
    }

    /// <summary>
    ///  BUTTLEREQUESTコマンドの実行クラス
    /// </summary>
    internal sealed class ButtleRequestNPCExecutable : BaseNpcExecutable
    {
        public ButtleRequestNPCExecutable(IADVGUI advgui, Func<string> npcNameFunc)
            : base(advgui, npcNameFunc) { }

        public override string Command => "buttlerequest";

        public override UniTask<int> ExecuteAsync(string[] args, CancellationToken ct)
        {
            return UniTask.FromResult(0);
        }
    }
}
