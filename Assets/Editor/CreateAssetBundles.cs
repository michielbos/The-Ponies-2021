using System;
using System.IO;
using UnityEditor;

public class CreateAssetBundles {
	private const string ASSET_BUNDLE_PATH = "Build/AssetBundles/";
	private const string BUILD_BUNDLES_MENU = "Assets/Build AssetBundles/";
	private const string WINDOWS_32 = "Windows32";
	private const string WINDOWS_64 = "Windows64";
	private const string LINUX_32 = "Linux32";
	private const string LINUX_64 = "Linux64";
	private const string MAC = "Mac";

	[MenuItem(BUILD_BUNDLES_MENU + "All")]
	private static void BuildAllAssetBundles () {
		BuildWindows32Bundles();
		BuildWindows64Bundles();
		BuildLinux32Bundles();
		BuildLinux64Bundles();
		BuildMacBundles();
	}

	[MenuItem(BUILD_BUNDLES_MENU + WINDOWS_32)]
	private static void BuildWindows32Bundles () {
		BuildBundles(WINDOWS_32, BuildTarget.StandaloneWindows);
	}
	
	[MenuItem(BUILD_BUNDLES_MENU + WINDOWS_64)]
	private static void BuildWindows64Bundles () {
		BuildBundles(WINDOWS_64, BuildTarget.StandaloneWindows64);
	}
	
	[MenuItem(BUILD_BUNDLES_MENU + LINUX_32)]
	private static void BuildLinux32Bundles () {
		BuildBundles(LINUX_32, BuildTarget.StandaloneLinux);
	}
	
	[MenuItem(BUILD_BUNDLES_MENU + LINUX_64)]
	private static void BuildLinux64Bundles () {
		BuildBundles(LINUX_64, BuildTarget.StandaloneLinux64);
	}
	
	[MenuItem(BUILD_BUNDLES_MENU + MAC)]
	private static void BuildMacBundles () {
		BuildBundles(MAC, BuildTarget.StandaloneOSX);
	}

	private static void BuildBundles (String folder, BuildTarget buildTarget) {
		if (!Directory.Exists(ASSET_BUNDLE_PATH + folder)) {
			Directory.CreateDirectory(ASSET_BUNDLE_PATH + folder);
		}
		BuildPipeline.BuildAssetBundles(ASSET_BUNDLE_PATH + folder, BuildAssetBundleOptions.None, buildTarget);
	}
}