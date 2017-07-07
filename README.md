# Argo

### Project started by Dylan Stackhouse and Clayton Collins, Epicodus Internship, 6/17

## Description
An augmented Reality greeting card app in which users send a greeting card/post card which contains a video message embedded in a QR code.

## Running instructions
1. Build for iOS
2. Verify your phone number from within settings>verify
3. Scan a QR code.([QR Code Generator](http://www.qr-code-generator.com/))
4. Wait for response.
5. On success aim camera at EyeCueTarget.PNG to view video

Verification is not required to scan and view public messages.

## Build instructions
Set iOS build directory to argoIphone. This directory will be written over with every build and will not be committed.

## Custom IOS code
* IOS build settings: `Assets/Editor/xcodeapi/CustomXCodeConfig.cs`
* URLScheme launch manager code injected into `Assets/Plugins/iOS/VuforiaNativeRendererController.mm` to utilize existing UnityAppController subclass
* Video Picker: `Assets/Plugins/iOS/VideoPicker.mm`
* Contacts Scraper: `Assets/Plugins/iOS/ContactPicker.mm`

## Amazon AWS configuration instructions
* Create an S3 storage bucket - allow access by your account
* Retrieve your security credentials from your account's security credentials page
* Put these credentials in the namespace "APIKeys", class named AWSKeys, with public static strings named visible and secret;

## Recommended reading
All fairly simple, but very important for interfacing with the API
* [Unity WWW docs](https://docs.unity3d.com/ScriptReference/WWW.html)
* [Unity WWWForm docs](https://docs.unity3d.com/ScriptReference/WWWForm.html)
* [Unity Coroutine docs](https://docs.unity3d.com/Manual/Coroutines.html)


## API repo
[git repo](git repo)
