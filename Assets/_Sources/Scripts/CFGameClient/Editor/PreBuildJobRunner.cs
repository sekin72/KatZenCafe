#if UNITY_EDITOR
using System;
using CerberusFramework.Utilities;
using UnityEditor;

namespace CFGameClient.Editor
{
    public static class PreBuildJobRunner
    {
        [MenuItem("CerberusFramework/Run Prebuild Jobs")]
        private static void _RunPrebuildJobs()
        {
            RunPrebuildJobs();
        }

        public static void RunPrebuildJobs()
        {
            foreach (var t in TypeCache.GetTypesDerivedFrom<IPreBuildJob>())
            {
                if (!ValidateType<IPreBuildJob>(t))
                    return;
                var o = Activator.CreateInstance(t) as IPreBuildJob;
                o.OnPreBuild();
            }
        }

        private static bool ValidateType<T>(Type t)
        {
            return typeof(T).IsAssignableFrom(t);
        }
    }
}
#endif