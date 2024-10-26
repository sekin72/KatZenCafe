#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CFGameClient.Editor
{
    public static class SaveDeleterUtility
    {
        [MenuItem("CerberusFramework/Delete Local Save", false, 1)]
        public static void SaveDeleter()
        {
            var filePath = Application.persistentDataPath;
            FileUtil.DeleteFileOrDirectory(filePath);
        }
    }
}
#endif