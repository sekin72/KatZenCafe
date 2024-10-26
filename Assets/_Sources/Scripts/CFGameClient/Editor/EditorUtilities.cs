#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.U2D;

namespace CFGameClient.Editor
{
    public static class EditorUtilities
    {
        [MenuItem("Assets/Force Reserialize", true, 32)]
        static bool CanForceReserializeSelection()
        {
            return Selection.objects.Length > 0 && Selection.objects.All(EditorUtility.IsPersistent);
        }

        [MenuItem("Assets/Force Reserialize", false, 32)]
        static void ForceReserializeSelection()
        {
            AssetDatabase.ForceReserializeAssets(Selection.objects.Select(AssetDatabase.GetAssetPath)
                .Where(p => !string.IsNullOrEmpty(p)).ToArray());
        }

        private static readonly Dictionary<Type, List<Type>> PossibleTypes = new()
        {
            {
                typeof(MonoScript), new List<Type>
                {
                    typeof(GameObject),
                    typeof(SceneAsset),
                    typeof(ScriptableObject)
                }
            },
            {
                typeof(GameObject), new List<Type>
                {
                    typeof(GameObject),
                    typeof(SceneAsset),
                    typeof(ScriptableObject)
                }
            },
            {
                typeof(Texture2D), new List<Type>
                {
                    typeof(GameObject),
                    typeof(SceneAsset),
                    typeof(Material),
                    typeof(AnimationClip),
                    typeof(ScriptableObject),
                    typeof(SpriteAtlas)
                }
            },
            {
                typeof(Material), new List<Type>
                {
                    typeof(GameObject),
                    typeof(SceneAsset),
                    typeof(ScriptableObject),
                    typeof(AnimationClip)
                }
            },
            {
                typeof(Font), new List<Type>
                {
                    typeof(GameObject),
                    typeof(SceneAsset),
                    typeof(ScriptableObject)
                }
            },
            {
                typeof(AnimatorController), new List<Type>
                {
                    typeof(GameObject),
                    typeof(SceneAsset),
                    typeof(ScriptableObject)
                }
            },
            {
                typeof(AnimationClip), new List<Type>
                {
                    typeof(GameObject),
                    typeof(SceneAsset),
                    typeof(ScriptableObject),
                    typeof(AnimatorController)
                }
            },
            {
                typeof(Shader), new List<Type>
                {
                    typeof(Material),
                    typeof(ShaderVariantCollection),
                }
            }
        };

        [MenuItem("Assets/List Dependencies", false, 20)]
        private static void OnListReferences()
        {
            AssetDatabase.SaveAssets();

            string path;
            var iid = Selection.activeInstanceID;
            if (AssetDatabase.IsMainAsset(iid))
            {
                path = AssetDatabase.GetAssetPath(iid);
            }
            else
            {
                Debug.Log("Error Asset not found");
                return;
            }

            Debug.Log(
                string.Join(
                    "\n",
                    AssetDatabase.GetDependencies(path, true).Except(new[] { path }).Distinct().OrderBy(p => p)
                        .ToArray()
                )
            );
        }

        [MenuItem("Assets/Find References in Project", true, 21)]
        private static bool OnSearchForReferencesValidation()
        {
            return AssetDatabase.IsMainAsset(Selection.activeInstanceID);
        }

        [MenuItem("Assets/Find References in Project", false, 21)]
        private static void OnSearchForReferences()
        {
            AssetDatabase.SaveAssets();
            GC.Collect();
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);

            if (!PossibleTypes.ContainsKey(assetType))
            {
                Debug.LogError("Cannot look up for " + assetType);
                return;
            }

            var checkTypes = PossibleTypes[assetType];

            Debug.Log(
                "Detected type " + assetType.Name + " (" + Path.GetFileName(path) + ")" +
                " looking for " + string.Join(", ", checkTypes.Select(s => s.Name).ToArray()),
                Selection.activeObject
            );

            var assets = new List<string>();
            foreach (var checkType in checkTypes)
            {
                var guids = AssetDatabase.FindAssets("t:" + checkType.Name, new[] { "Assets" });
                assets.Capacity += guids.Length;
                foreach (var guid in guids)
                {
                    assets.Add(AssetDatabase.GUIDToAssetPath(guid));
                }
            }

            for (var i = 0; i < assets.Count; i++)
            {
                var assetPath = assets[i];
                EditorUtility.DisplayProgressBar(
                    "Find References", "Searching in " + assetPath, (float)i / assets.Count
                );

                var dependencies = AssetDatabase.GetDependencies(assetPath, false);
                if (dependencies.Contains(path))
                {
                    Debug.Log(assetPath, AssetDatabase.LoadMainAssetAtPath(assetPath));
                }
            }

            EditorUtility.ClearProgressBar();
            GC.Collect();
        }

        [MenuItem("Assets/Find Unused Assets/Textures", true, 22)]
        private static bool FindUnusedAssetsValidation()
        {
            var iid = Selection.activeInstanceID;
            if (!AssetDatabase.IsMainAsset(iid))
            {
                return false;
            }

            var path = AssetDatabase.GetAssetPath(iid);

            return AssetDatabase.IsValidFolder(path);
        }

        [MenuItem("Assets/Find Unused Assets/Textures", false, 22)]
        private static void FindUnusedAssets()
        {
            ListUnusedAssetsUnderFolder(Selection.activeInstanceID, typeof(Texture2D));
        }

        [MenuItem("Assets/Print ObjectType", false, 23)]
        private static void PrintObjectType()
        {
            AssetDatabase.SaveAssets();

            string path;
            var iid = Selection.activeInstanceID;
            if (AssetDatabase.IsMainAsset(iid))
            {
                path = AssetDatabase.GetAssetPath(iid);
            }
            else
            {
                Debug.Log("Error Asset not found");
                return;
            }

            var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            Debug.Log("Type of asset at " + path + " is " + mainAssetType + ", " + mainAssetType.Assembly);
        }

        private static void ListUnusedAssetsUnderFolder(int folderGuid, Type assetType)
        {
            AssetDatabase.SaveAssets();
            GC.Collect();
            var path = AssetDatabase.GetAssetPath(folderGuid);
            var assetTypeName = assetType.Name;
            var pathGuids = AssetDatabase.FindAssets("t:" + assetTypeName, new[] { path });
            var pathAssets = new List<string>(pathGuids.Length);
            foreach (var guid in pathGuids)
            {
                var p = AssetDatabase.GUIDToAssetPath(guid);
                pathAssets.Add(p);
            }

            var checkTypes = PossibleTypes[assetType];

            var assets = new List<string>();
            foreach (var checkType in checkTypes)
            {
                var guids = AssetDatabase.FindAssets("t:" + checkType.Name, new[] { "Assets" });
                assets.Capacity += guids.Length;
                foreach (var guid in guids)
                {
                    assets.Add(AssetDatabase.GUIDToAssetPath(guid));
                }
            }

            var assetsArray = assets.ToArray();
            var dependencies = AssetDatabase.GetDependencies(assetsArray, false);
            for (var i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                EditorUtility.DisplayProgressBar("Find References", "Searching ", (float)i / dependencies.Length);
                if (!dependency.StartsWith(path))
                {
                    continue;
                }

                pathAssets.Remove(dependency);
            }

            EditorUtility.ClearProgressBar();

            long totalSize = 0;
            pathAssets
                .Select(p => new { path = p, size = new FileInfo(p).Length })
                .OrderByDescending(meta => meta.size)
                .ForEach(
                    meta =>
                    {
                        totalSize += meta.size;
                        Debug.LogFormat(
                            AssetDatabase.LoadMainAssetAtPath(meta.path),
                            "{0} | size: {1}", meta.path, GetBytesReadable(meta.size)
                        );
                    }
                );

            Debug.LogFormat(
                AssetDatabase.LoadMainAssetAtPath(path),
                "Total unused {0} size is {1} at {2}", assetTypeName, GetBytesReadable(totalSize), path
            );
            GC.Collect();
        }

        [MenuItem("Tools/Utils/List All Asset Types", false, 502)]
        private static void ListAllAssetTypes()
        {
            var scriptableType = typeof(ScriptableObject);

            var assets = AssetDatabase.GetAllAssetPaths();
            var types = assets
                .Where(path => path.StartsWith("Assets/"))
                .Select(AssetDatabase.GetMainAssetTypeAtPath)
                .Distinct()
                .Where(t => !scriptableType.IsAssignableFrom(t))
                .ToList();

            types.Add(scriptableType);

            var typeNames = types
                .Where(t => t != null)
                .Select(t => t.ToString())
                .OrderBy(s => s)
                .ToArray();

            Debug.Log("Available Types : \n" + string.Join("\n", typeNames));
        }

        public static string GetBytesReadable(long i)
        {
            // Get absolute value
            var absolute_i = i < 0 ? -i : i;
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = i >> 50;
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = i >> 40;
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = i >> 30;
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = i >> 20;
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = i >> 10;
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }

            // Divide by 1024 to get fractional value
            readable = readable / 1024;
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
    }
}
#endif