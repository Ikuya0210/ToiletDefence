using UnityEngine;

namespace PinballBenki.Scene
{
    public enum SceneNames
    {
        None = 0,
        Title,
        Field,
        Buttle,
    }

    public static class SceneNamesExtensions
    {
        public static string ToSceneNameString(this SceneNames sceneName)
        {
            return sceneName switch
            {
                SceneNames.Title => "Title",
                SceneNames.Field => "Field",
                SceneNames.Buttle => "Buttle",
                _ => ""
            };
        }
    }
}
