using System.Threading;
using Cysharp.Threading.Tasks;
using PinballBenki.Input;
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
            _scriptExecuter = new(new[]{
                new TextNPCExecutable(aDVGUI, () => _currentNpcName)
            });
        }

        public async UniTask TalkAsync(string talkerName, string script, CancellationToken ct)
        {
            var dialogue = _aDVGUI.Dialogue;
            _currentNpcName = talkerName;
            _input.Activate();

            await _scriptExecuter.Exec(script, ct);

            _input.Deactivate();

            if (dialogue.IsVisible)
            {
                await dialogue.HideAsync(ct);
            }
        }
    }
}
