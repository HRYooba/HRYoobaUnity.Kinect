using UnityEngine;
using UnityEditor;

namespace HRYooba.Kinect.Editor
{
    public class MenuItemKinect
    {
        private static void InstantiateGameObject(MenuCommand menuCommand, string path)
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
        }

        [MenuItem("GameObject/HRYooba/RfilkovKinectManager", false, 21)]
        private static void CreateRfilkovKinectManager(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "RfilkovKinectManager");
        }
    }
}