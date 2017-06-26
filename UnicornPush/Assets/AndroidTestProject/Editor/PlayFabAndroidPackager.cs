
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PlayFab.Internal
{
    public static class PlayFabAndroidPackager
    {
        // Build the Android Push Plugin to the UnitySdk Directory
        private static readonly Dictionary<string, string> PackagePaths = new Dictionary<string, string> {
            {"10.0.1", "C:/depot/sdks/UnitySDK/Packages/Push_Unity5_GPS10.0.1.unitypackage"},
            {"8.4.0", "C:/depot/sdks/UnitySDK/Packages/Push_Unity5_GPS8.4.0.unitypackage"},
        };

        private static readonly string[] SdkAssets = {
            "Assets/Plugins/Android"
        };

        [MenuItem("PlayFab/Testing/Build PlayFab Android-Push UnityPackage")]
        public static void BuildAndroidPushPluginPackage()
        {
            var files = Directory.GetFiles(Application.dataPath + "/Plugins/Android");
            foreach (var versionPair in PackagePaths)
            {
                foreach (var file in files)
                {
                    if (file.Contains("play-services-gcm") && file.Contains(versionPair.Key))
                    {
                        var packagePath = PackagePaths[versionPair.Key];
                        Debug.Log("Building package: " + packagePath);
                        AssetDatabase.ExportPackage(SdkAssets, packagePath, ExportPackageOptions.Recurse);
                        Debug.Log("Package built: " + packagePath);
                        return;
                    }
                }
            }
        }

        //////////////////// Android Push Plugin Automation ////////////////////
        private class AndroidPluginTestBlob
        {
            public string[] PluginAndroidFiles;
            public string PluginCsFile;
        }

        private static readonly AndroidPluginTestBlob AndPlug840 = new AndroidPluginTestBlob
        {
            PluginAndroidFiles = new[] {
                "{ANDROID_HOME}/extras/android/m2repository/com/android/support/appcompat-v7/24.2.0/appcompat-v7-24.2.0.aar",
                "{ANDROID_HOME}/extras/google/m2repository/com/google/android/gms/play-services-base/8.4.0/play-services-base-8.4.0.aar",
                "{ANDROID_HOME}/extras/google/m2repository/com/google/android/gms/play-services-basement/8.4.0/play-services-basement-8.4.0.aar",
                "{ANDROID_HOME}/extras/google/m2repository/com/google/android/gms/play-services-gcm/8.4.0/play-services-gcm-8.4.0.aar",
                "{ANDROID_HOME}/extras/android/m2repository/com/android/support/support-v4/23.4.0/support-v4-23.4.0.aar",
                "C:/dev/TestAndPlugAutomation/8.4.0/AndPlug_8.4.0-release.aar",
                "C:/dev/TestAndPlugAutomation/8.4.0/AndroidManifest.xml",
            },
            PluginCsFile = "C:/dev/TestAndPlugAutomation/PlayFabAndroidPushPlugin.cs"
        };
        private static readonly AndroidPluginTestBlob AndPlug1001 = new AndroidPluginTestBlob
        {
            PluginAndroidFiles = new[] {
                "{ANDROID_HOME}/extras/android/m2repository/com/android/support/appcompat-v7/25.1.1/appcompat-v7-25.1.1.aar",
                "{ANDROID_HOME}/extras/google/m2repository/com/google/android/gms/play-services-base/10.0.1/play-services-base-10.0.1.aar",
                "{ANDROID_HOME}/extras/google/m2repository/com/google/android/gms/play-services-basement/10.0.1/play-services-basement-10.0.1.aar",
                "{ANDROID_HOME}/extras/google/m2repository/com/google/android/gms/play-services-gcm/10.0.1/play-services-gcm-10.0.1.aar",
                "{ANDROID_HOME}/extras/google/m2repository/com/google/android/gms/play-services-iid/10.0.1/play-services-iid-10.0.1.aar",
                "{ANDROID_HOME}/extras/android/m2repository/com/android/support/support-v4/24.0.0/support-v4-24.0.0.aar",
                "C:/dev/TestAndPlugAutomation/10.0.1/release.aar",
                "C:/dev/TestAndPlugAutomation/10.0.1/AndroidManifest.xml",
            },
            PluginCsFile = "C:/dev/TestAndPlugAutomation/PlayFabAndroidPushPlugin.cs"
        };

        private static string GetAndroidPluginPath()
        {
            return Path.Combine(Application.dataPath, "Plugins/Android");
        }

        private static string GetAndroidSdkPath()
        {
            var androidSdkPath = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (string.IsNullOrEmpty(androidSdkPath) || !Directory.Exists(androidSdkPath))
                throw new Exception("ANDROID_HOME must be set to a proper AndroidSdk path");
            return androidSdkPath;
        }

        private static void ClearCurrentPlugin()
        {
            var androidPluginFolder = GetAndroidPluginPath();
            Debug.Log("Deleting folder: " + androidPluginFolder);
            if (Directory.Exists(androidPluginFolder))
                Directory.Delete(androidPluginFolder, true);
            Directory.CreateDirectory(androidPluginFolder);
            Directory.CreateDirectory(Path.Combine(androidPluginFolder, "PlayFab"));
        }

        private static void SetupPlayFabAndroidPlugin(AndroidPluginTestBlob plugindetails)
        {
            var androidPluginFolder = GetAndroidPluginPath();
            var androidSdkPath = GetAndroidSdkPath();

            ClearCurrentPlugin();
            foreach (var eachPathTemplate in plugindetails.PluginAndroidFiles)
            {
                var eachSrcPath = eachPathTemplate.Replace("{ANDROID_HOME}", androidSdkPath);
                var eachFileName = Path.GetFileName(eachSrcPath);
                var eachDstPath = Path.Combine(androidPluginFolder, eachFileName);
                Debug.Log("Copying: " + eachSrcPath + " to: " + eachDstPath);
                File.Copy(eachSrcPath, eachDstPath);
            }
            var csSrcPath = plugindetails.PluginCsFile;
            var csFileName = Path.GetFileName(csSrcPath);
            var csDstPath = Path.Combine(Path.Combine(GetAndroidPluginPath(), "PlayFab"), csFileName);
            Debug.Log("Copying: " + csSrcPath + " to: " + csDstPath);
            File.Copy(csSrcPath, csDstPath);
            AssetDatabase.Refresh();
        }

        [MenuItem("PlayFab/Testing/Set AndroidPushPlugin to 8.4.0")]
        public static void SetAndPlug8()
        {
            SetupPlayFabAndroidPlugin(AndPlug840);
        }

        [MenuItem("PlayFab/Testing/Set AndroidPushPlugin to 10.0.1")]
        public static void SetAndPlug10()
        {
            SetupPlayFabAndroidPlugin(AndPlug1001);
        }
    }
}
