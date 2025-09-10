using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildTool
{
    [MenuItem("BuildTool/Clear AssetBundles")]
    static void ClearAllAssetBundles()
    {
        var allBundles = AssetDatabase.GetAllAssetBundleNames();
        foreach (var bundle in allBundles)
        {
            AssetDatabase.RemoveAssetBundleName(bundle, true);
            Debug.Log("BuildTool:Remove Old Bundle: " + bundle);
        }
    }

    [MenuItem("BuildTool/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
