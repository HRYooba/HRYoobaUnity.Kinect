using UnityEngine;
using UnityEditor;
using HRYooba.Library;
using HRYooba.Kinect.Services;

namespace HRYooba.Kinect.Editor
{
    public class CreateJsonWindow : EditorWindow
    {
        [MenuItem("Tools/HRYooba/Kinect/Create Json")]
        private static void Create()
        {
            var window = GetWindow<CreateJsonWindow>();
            window.titleContent = new GUIContent("Create Json");
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Create AreasSetting Json"))
            {
                CreateAreasSettingJson();
            }

            if (GUILayout.Button("Create KinectsSetting Json"))
            {
                CreateKinectsSettingJson();
            }
        }

        private void CreateAreasSettingJson()
        {
            var json = new AreasSetting
            {
                Settings = new[] { new AreasSetting.Setting() }
            };
            JsonHelper.Save(AreaDataJsonManager.JsonPath, json);
            Debug.Log($"Created {AreaDataJsonManager.JsonPath}");
        }

        private void CreateKinectsSettingJson()
        {
            var json = new KinectsSetting
            {
                Settings = new[] { new KinectsSetting.Setting() }
            };
            JsonHelper.Save(KinectDataJsonManager.JsonPath, json);
            Debug.Log($"Created {KinectDataJsonManager.JsonPath}");
        }
    }
}