using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class ChangeIOSBuildNumber {

	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject) {

		if (buildTarget == BuildTarget.iOS) {

			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			// Change value of CFBundleVersion in Xcode plist
			rootDict.SetString("Privacy - Contacts Usage Description","Contacts are needed to facilitate message sharing.");
			rootDict.SetString("Privacy - Photo Library Usage Description","Access Required to send Videos and Photos.");
			rootDict.SetString("Application uses Wi-Fi","YES");
			rootDict.SetString("Privacy - Camera Usage Description","Camera access required for target detection and tracking");
			rootDict.SetString("Privacy - Camera Usage Description","Camera access required for target detection and tracking");
			rotDict.SetDict ();

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());
		}
	}
}
