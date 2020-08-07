using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

// Чтобы запустить этот скрипт надо нажать верхнюю вкладку Component > Find Game Objects with Missing Scripts и он создаст те объекты у которых пропавшие скрипты
public class FindGameObjectsWithMissingScripts : Editor
{
    [MenuItem("Component/Find Game Objects with Missing Scripts")]
    public static void FindGameObjects()
    {
        // Get all the paths to prefabs in our project
        string[] PrefabPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();


        GameObject Parent = null;

        // Load each preefab
        foreach (string path in PrefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            // Get all components of this preefab
            Component[] components = prefab.GetComponents<Component>();

            // Check if any component is null, if so we have a missing component
            foreach (Component component in components)
            {
                if (component == null)
                {
                    if (Parent == null)
                    {
                        Parent = new GameObject("Missing Component Objects");
                    }

                    GameObject Instance = Instantiate(prefab, Parent.transform);

                    break;
                }
            }
        }
    }
}
