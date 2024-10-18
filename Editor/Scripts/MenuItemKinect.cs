using UnityEngine;
using UnityEditor;

namespace HRYooba.Kinect.Editor
{
    public class MenuItemKinect
    {
        private static GameObject InstantiateGameObject(MenuCommand menuCommand, string path)
        {
            // Create a custom game object
            var obj = Resources.Load<GameObject>(path);
            var go = GameObject.Instantiate(obj) as GameObject;
            go.name = obj.name;
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

            return go;
        }

        [MenuItem("GameObject/HRYooba/RfilkovKinectManager", false, 21)]
        private static void CreateRfilkovKinectManager(MenuCommand menuCommand)
        {
            var obj = InstantiateGameObject(menuCommand, "RfilkovKinectManager");

            var rendererComponents = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in rendererComponents)
            {
                var materials = renderer.sharedMaterials;
                Material[] newMaterials = new Material[materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null)
                    {
                        newMaterials[i] = new Material(materials[i]); // Create a new instance of each material
                    }
                }
                renderer.sharedMaterials = newMaterials;
            }
        }
    }
}