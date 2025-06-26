using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PinballBenki
{
    /// <summary>
    /// 内部のToLowerでコマンドを比較するため、コマンドは大文字小文字を区別しない
    /// </summary>
    public class ScriptExecuter : ScriptExecuter.IOwner
    {
        public Action<int, string[]> OnCustomAction;
        private readonly Dictionary<string, Func<string[], CancellationToken, UniTask<int>>> _commandMap;
        private readonly Dictionary<string, bool> _flags = new();
        private bool _isNextCommand;
        private bool _isWaitNext;
        private bool _forceEnd;

        public ScriptExecuter(IExecutable[] executables)
        {
            if (executables == null)
            {
                throw new ArgumentNullException(nameof(executables), "Executablesをnullにすることはできません。");
            }

            _commandMap = new();
            foreach (var executable in executables)
            {
                if (executable == null)
                {
                    continue;
                }
                executable.SetOwner(this);
                _commandMap[executable.Command.ToLower()] = executable.ExecuteAsync;
            }
        }

        public void ToNext()
        {
            _isNextCommand = true;
        }

        void IOwner.WaitNext()
        {
            _isWaitNext = true;
        }

        void IOwner.ForceEnd()
        {
            _forceEnd = true;
        }

        void IOwner.InvokeCustomAction(int id, string[] args)
        {
            OnCustomAction?.Invoke(id, args);
        }

        public async UniTask Exec(string script, CancellationToken ct)
        {
            _flags.Clear();

            var lines = script
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var execStack = new Stack<bool>();
            bool canExec = true;
            int lineSkipCount = 0;
            _forceEnd = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lineSkipCount > 0)
                {
                    lineSkipCount--;
                    continue;
                }

                var trimmedLine = lines[i].Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                var parts = trimmedLine.Split(new[] { ' ' }, 2);
                var command = parts[0].ToLower();

                if (command == "if")
                {
                    var flagName = parts.Length > 1 ? parts[1].Trim() : "";
                    bool flagValue = false;
                    _flags.TryGetValue(flagName, out flagValue);
                    execStack.Push(canExec);
                    canExec = canExec && flagValue;
                    continue;
                }
                if (command == "else")
                {
                    if (execStack.Count > 0)
                    {
                        bool parent = execStack.Peek();
                        canExec = parent && !canExec;
                    }
                    continue;
                }
                if (command == "endif")
                {
                    if (execStack.Count > 0)
                    {
                        canExec = execStack.Pop();
                    }
                    continue;
                }
                if (!canExec)
                {
                    continue;
                }

                // フラグ操作コマンド
                if (command == "setflag" && parts.Length > 1)
                {
                    _flags[parts[1].Trim()] = true;
                    continue;
                }
                if (command == "unsetflag" && parts.Length > 1)
                {
                    _flags[parts[1].Trim()] = false;
                    continue;
                }

                if (_commandMap.TryGetValue(command, out var executeFunc))
                {
                    string[] args = parts.Length > 1 ? parts[1].Split(' ') : Array.Empty<string>();
                    _isWaitNext = false;
                    int t = await executeFunc(args, ct);
                    if (_forceEnd)
                    {
                        return; // 強制終了
                    }
                    lineSkipCount += t;
                    if (_isWaitNext)
                    {
                        _isNextCommand = false;
                        await UniTask.WaitUntil(() => _isNextCommand, cancellationToken: ct);
                    }
                }
            }
        }

        public interface IExecutable
        {
            string Command { get; }
            /// <summary>
            /// 基本的に0を返す。飛ばす行数を返す
            /// 0より小さい場合は次の行の実行を待機しない
            /// </summary>
            // メモ：分岐の条件の後にsetflafを書いておいて、直後にif
            UniTask<int> ExecuteAsync(string[] args, CancellationToken ct);

            void SetOwner(IOwner owner);
        }

        public interface IOwner
        {
            /// <summary>
            /// このコマンドの後、ToNextが呼ばれるまで、次のコマンドを実行されないようにする
            /// </summary>
            void WaitNext();

            void ForceEnd();

            void InvokeCustomAction(int id, string[] args);
        }
    }
}
