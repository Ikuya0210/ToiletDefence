using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public static class IDProvider
    {
        private static uint _currentId = 0;

        public static uint GenerateID()
        {
            return ++_currentId;
        }

        public static void Reset()
        {
            _currentId = 0;
        }
    }
}
