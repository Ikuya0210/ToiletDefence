using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using PinballBenki.Input;
using R3;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class DialogueDirector
    {
        private ScriptExecuter _scriptExecuter;
        private PartialInput _input;
        private IADVGUI _aDVGUI;
        private string _currentNpcName = "";

        public DialogueDirector(IADVGUI aDVGUI, uint inputPriority)
        {
            _aDVGUI = aDVGUI;
            _input = new(inputPriority);
            _scriptExecuter = new(new ScriptExecuter.IExecutable[]{
                new TextNPCExecutable(aDVGUI, () => _currentNpcName)
                , new SelectNPCExecutable(aDVGUI, () => _currentNpcName)
                ,new ButtleRequestNPCExecutable(aDVGUI, () => _currentNpcName)
            });
        }

        public async UniTask TalkAsync(string talkerName, string script, CancellationToken ct)
        {
            var dialogue = _aDVGUI.Dialogue;
            _currentNpcName = talkerName;
            _input.Activate();
            IDisposable decide = _input.OnDecide.Subscribe(_ =>
                {
                    dialogue.Skip();
                    _scriptExecuter.ToNext();
                });

            await _scriptExecuter.Exec(script, ct);

            _input.Deactivate();
            decide.Dispose();

            if (dialogue.IsVisible)
            {
                await dialogue.HideAsync(ct);
            }
        }
    }
}
