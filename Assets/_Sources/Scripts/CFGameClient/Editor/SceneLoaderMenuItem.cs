#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace CFGameClient.Editor
{
    public class SceneLoaderMenuItem
    {
        [MenuItem("CerberusFramework/Scenes/PreloaderScene", false, 1)]
        public static void LoadPreloaderScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_CFSources/Scenes/PreloaderScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/LoadingScene", false, 2)]
        public static void LoadLoadingScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_CFSources/Scenes/LoadingScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/MainMenuScene", false, 3)]
        public static void LoadMainScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Sources/Scenes/MainMenuScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/LevelScene", false, 4)]
        public static void LoadLevelScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Sources/Scenes/LevelScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/CFDemoScene", false, 5)]
        public static void LoadCFDemoScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Sources/Scenes/CFDemoScene.unity", OpenSceneMode.Single);
            }
        }
    }
}
#endif