using UnityEditor;
using UnityEditor.SceneManagement;

namespace CFGameClient.Editor
{
    [InitializeOnLoad]
    public class EditorInitialization
    {
        static EditorInitialization()
        {
            var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
            EditorSceneManager.playModeStartScene = sceneAsset;
        }
    }
}
