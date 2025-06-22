using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PinballBenki
{
    /// <summary>
    /// 内部のToLowerでコマンドを比較するため、コマンドは大文字小文字を区別しない
    /// </summary>
    public class ScriptExecuter
    {
        private readonly Dictionary<string, Func<string[], CancellationToken, UniTask>> _commandMap;
        private bool _isNextCommand;

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
                _commandMap[executable.Command.ToLower()] = executable.ExecuteAsync;
            }
        }

        public void NextCommand()
        {
            _isNextCommand = true;
        }

        public async UniTask Exec(string script, CancellationToken ct)
        {
            // 改行コードを揃えた後、空白行で分割
            var lines = script
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // 各行を処理
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    continue; // 空行はスキップ
                }

                // コマンドと引数を分割
                var parts = trimmedLine.Split(new[] { ' ' }, 2);
                var command = parts[0].ToLower();

                // コマンドがマップに存在するか確認
                if (_commandMap.TryGetValue(command, out var executeFunc))
                {
                    // コマンドを実行
                    string[] args = parts.Length > 1 ? parts[1].Split(' ') : Array.Empty<string>();
                    await executeFunc(args, ct);
                    _isNextCommand = false;
                    await UniTask.WaitUntil(() => _isNextCommand, cancellationToken: ct);
                }
            }
        }

        public interface IExecutable
        {
            string Command { get; }
            UniTask ExecuteAsync(string[] args, CancellationToken ct);
        }
    }
}
