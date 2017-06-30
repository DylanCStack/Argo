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

			// Set plist keys/values
			rootDict.SetString("Privacy - Contacts Usage Description","Contacts are needed to facilitate message sharing.");
			rootDict.SetString("Privacy - Photo Library Usage Description","Access Required to send Videos and Photos.");
			rootDict.SetBoolean("Application uses Wi-Fi",true);
			rootDict.SetString("Privacy - Camera Usage Description","Camera access required for target detection and tracking");
			rootDict.SetString("Privacy - Camera Usage Description","Camera access required for target detection and tracking");

			rootDict.SetString ("CFBundleURLName", "com.EyeCue.Argo");
			rootDict.CreateArray ("CFBundleURLSchemes").AddString ("Argo");

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());

			//create project with path
			string projPath = PBXProject.GetPBXProjectPath (pathToBuiltProject);
			PBXProject proj = new PBXProject ();

			//fill project object
			proj.ReadFromString (File.ReadAllText (projPath));
			string target = proj.TargetGuidByName ("Unity-iPhone");

			//set frameworks to import
			proj.AddFrameworkToProject (target, "MobileCoreServices.framework", false);
			proj.AddFrameworkToProject (target, "Contacts.framework", false);
			proj.AddFrameworkToProject (target, "ContactsUI.framework", false);

			//commit changes
			File.WriteAllText (projPath, proj.WriteToString());

		}
	}
}
