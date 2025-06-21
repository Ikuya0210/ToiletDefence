using UnityEngine;

namespace PinballBenki.Scene
{
    public enum SceneNames
    {
        None = 0,
        Title,
        ADV,
        Game,
    }

    public static class SceneNamesExtensions
    {
        public static string ToSceneNameString(this SceneNames sceneName)
        {
            return sceneName switch
            {
                SceneNames.Title => "Title",
                SceneNames.ADV => "ADVBase",
                SceneNames.Game => "Game",
                _ => ""
            };
        }
    }
}
