using System;
using UnityEngine;

namespace PinballBenki.Input
{
    public class InputProvider : IDisposable
    {
        public Action OnAttack;
        private readonly InputActions _inputActions;

        public InputProvider()
        {
            _inputActions = new();
            _inputActions.Enable();
            _inputActions.Player.Attack.performed += _ => OnAttack?.Invoke();
        }

        public void Dispose()
        {
            _inputActions.Disable();
            _inputActions.Dispose();
        }
    }
}
