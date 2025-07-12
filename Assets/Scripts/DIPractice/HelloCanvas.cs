using System;
using UnityEngine;
using UnityEngine.UI;

namespace DIPractice
{
    public class HelloCanvas : MonoBehaviour, IDisposable
    {
        public Button helloButton;

        public void Dispose()
        {
            if (helloButton != null)
            {
                helloButton.onClick.RemoveAllListeners();
            }
        }
    }
}
