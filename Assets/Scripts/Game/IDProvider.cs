using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class IDProvider
    {
        private uint _currentId = 0;

        public uint GenerateID()
        {
            return ++_currentId;
        }

        public void Reset()
        {
            _currentId = 0;
        }
    }
}
