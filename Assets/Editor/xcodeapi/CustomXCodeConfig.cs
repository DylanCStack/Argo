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
			rootDict.SetString("Privacy - Contacts Usage Description","Contacts are needed to facilitate message sharing.");//allows access to contacts
			rootDict.SetString("Privacy - Photo Library Usage Description","Access Required to send Videos and Photos.");//allows access to photolibrary
			rootDict.SetBoolean("Application uses Wi-Fi",true);//an attempt to lessen data usage
			rootDict.SetString("Privacy - Camera Usage Description","Camera access required for target detection and tracking");//allows camera usage

			//set custom URLScheme for app
			rootDict.SetString ("CFBundleURLName", "com.EyeCue.Argo");
			rootDict.CreateArray ("CFBundleURLSchemes").AddString ("argo");

			// Write plist changes
			File.WriteAllText(plistPath, plist.WriteToString());

			//create project with path
			string projPath = PBXProject.GetPBXProjectPath (pathToBuiltProject);
			PBXProject proj = new PBXProject ();

			//fill project object
			proj.ReadFromString (File.ReadAllText (projPath));
			string target = proj.TargetGuidByName ("Unity-iPhone");

			//set frameworks to import
			proj.AddFrameworkToProject (target, "MobileCoreServices.framework", false);//needed to access Video Picker
			proj.AddFrameworkToProject (target, "Contacts.framework", false);//needed to access Contacts
			proj.AddFrameworkToProject (target, "ContactsUI.framework", false);

			// Write framework changes
			File.WriteAllText (projPath, proj.WriteToString());

		}
	}
}
