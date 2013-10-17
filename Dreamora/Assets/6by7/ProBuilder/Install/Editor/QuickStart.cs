#if UNITY_EDITOR
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

public class QuickStart : EditorWindow
{
	[MenuItem("Window/ProBuilder/Quick Start")]
	public static void InitQuickStart()
	{
		EditorWindow.GetWindow(typeof(QuickStart), true, "ProBuilder Quick Start", true);
	}

	Rect BottomButton { get { return new Rect(2, Screen.height-26, Screen.width-4, 24); } }

	public void OnEnable()
	{
		LoadImages();
	}

	Texture2D warn, vers;
	public void LoadImages()
	{
		warn = (Texture2D)Resources.LoadAssetAtPath("Assets/6by7/ProBuilder/Install/Image/warning.png", typeof(Texture2D));
		vers = (Texture2D)Resources.LoadAssetAtPath("Assets/6by7/ProBuilder/Install/Image/whichVersion.png", typeof(Texture2D));

		MaxImportSettings(warn);
		MaxImportSettings(vers);
	}

	public void MaxImportSettings(Texture2D img)
	{
		if(AssetDatabase.GetAssetPath( (Texture2D)img) != null)
		{
			TextureImporter tempImporter = TextureImporter.GetAtPath( AssetDatabase.GetAssetPath( (Texture2D)img) ) as TextureImporter;
			tempImporter.isReadable = true;
			tempImporter.textureType = TextureImporterType.GUI;
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath((Texture2D)img), ImportAssetOptions.ForceUpdate);
		}
	}    

	System.Type pb;
	int curVersion = 0;
	int step = -2;
	bool agreeToRead = false;
	int UP_OR_INSTALL_BUTTON_HEIGHT = 100;
	public void OnGUI()
	{
		// GUI.Label(new Rect(Screen.width-60, 2, 58, 24), Screen.width + ", " + Screen.height);

		switch(step)
		{
			case -2:
				Heading(new Vector2(410, 262), "ProBuilder Quick Start", "");

				if(GUILayout.Button(new GUIContent("Fresh Install", "Select this is this is the first time you have installed ProBuilder in this project."), GUILayout.MinHeight(UP_OR_INSTALL_BUTTON_HEIGHT)))
				{
					LoadLatestPack();
					this.Close();
				}

				if(GUILayout.Button(new GUIContent("Upgrade Existing", "Select this if you have an already existing ProBuilder project that you would like to upgrade."), GUILayout.MinHeight(UP_OR_INSTALL_BUTTON_HEIGHT)))
					step = -1;
				break;

			case -1:
				Heading(new Vector2(410, 350), "It's Dangerous to go alone", "");
				
				if(warn == null)
					LoadImages();

				GUI.Label( new Rect(10, 40, 400, 170),
					warn);
				// val = EditorGUILayout.IntSlider(10, 200, val);
				GUILayout.Space(175);

				EditorGUILayout.HelpBox("If you are unable to successfully update your prject, please email contact@sixbyseven.com for help.  Alternatively, find us on Skype: username: gabriel_williams", MessageType.Info);

				GUILayout.Label("I will read all instructions, and create a backup of my project.");
				agreeToRead = EditorGUILayout.Toggle("I promise", agreeToRead);


				if(agreeToRead)
				{
					GUI.backgroundColor = Color.green;
						if(GUI.Button(BottomButton, "Continue"))
							step = 0;
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUI.backgroundColor = Color.red;
						GUI.Button(BottomButton, "Continue");
					GUI.backgroundColor = Color.white;
				}

				break;

			case 0:
				Heading(new Vector2(520, 700), "Version Selection", "To begin, please select which version of ProBuilder your project was built with.");

				EditorGUILayout.HelpBox("Back up your project!  If the upgrade process fails, all project data will be lost!", MessageType.Error);

				// User input
				string[] pbVersions = new string[6]{"No Selection",  "ProBuilder 1.6", "ProBuilder 1.7", "ProBuilder2 Beta 1-3", "ProBuilder2 Beta 4-5 (v344, v398)", "ProBuilder2 RC (v447+)"};
				GUILayout.Label("Please select the ProBuilder version that your objects were created with.");
				curVersion = EditorGUILayout.Popup(curVersion, pbVersions);
				
				if(vers == null)	
					LoadImages();

				GUILayout.Label(vers);

				if(curVersion > 0)
				{
					GUI.backgroundColor = Color.green;
					if(GUI.Button(BottomButton, "Continue"))
					{
						switch(curVersion)
						{
							case 0:
							case 1:
							case 2:
								LoadLatestPack();
								step = 1;
								break;
							case 3:
							case 4:
								LoadUpgradePack();
								step = 2;
								break;
							case 5:
								LoadLatestPack();
								this.Close();
								break;
						}
					}
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUI.backgroundColor = Color.red;
						GUI.Button(BottomButton, "Continue");
					GUI.backgroundColor = Color.white;
				}
				break;
			case 1:
					LoadUpgradePack();
					step = 2;
				break;

			case 2:

				OpenInstallWindow();
				break;
		}
	}

	public void Heading(Vector2 minScreenSize, string title, string description)
	{
				// Set min window size
		this.minSize = minScreenSize;
		this.maxSize = minScreenSize;

		// Title
		GUILayout.Label("Step " + ((step > 0) ? step : 0) + " : " + title, EditorStyles.boldLabel);
		
		GUILayout.Space(4);

		// Description
		GUILayout.Label(description, EditorStyles.wordWrappedLabel);
	
		GUILayout.Space(4);
	}

	public void OpenInstallWindow()
	{
		ClearLog();

		this.minSize = new Vector2(520, 150);
		this.maxSize = new Vector2(520, 150);

		EditorGUILayout.HelpBox("There will be some errors that come up in the console.  This is expected.  Don't worry!", MessageType.Info);

		GUI.backgroundColor = Color.green;

		if(GUILayout.Button("Continue", GUILayout.MinHeight(100)))
		{

			// AssetDatabase.ImportAsset("Assets/6by7/ProBuilder/Install/Editor/ProBuilderInstall.cs", ImportAssetOptions.ForceSynchronousImport);
			// System.Type pbVB = Assembly.Load("Assembly-CSharp").GetTypes().First(t => t.Name == "ProBuilderInstall");
			System.Type pb = System.Type.GetType("ProBuilderInstall");
			
			if(pb == null)
			{
				Debug.Log("Oooone sec, not quite finished loading.  Try clicking Continue again in a few seconds.");
			}

			MethodInfo initInstallWindow = pb.GetMethod("Init");
			initInstallWindow.Invoke(null, new object[1]{curVersion});
			// EditorApplication.ExecuteMenuItem("Window/ProBuilder/Install ProBuilder");
			step = 4;
			this.Close();
		}
		GUI.backgroundColor = Color.white;

	}

	public void LoadLatestPack()
	{
				// upload new pack
		string pb_path = GetProBuilderPack();
		if(pb_path == "") return;
		
		AssetDatabase.ImportPackage(pb_path, false);
	}

	public void LoadUpgradePack()
	{
		string pb_path = GetProBuilderUpgradePack();
		if(pb_path == "") return;

		AssetDatabase.ImportPackage(pb_path, false);
	}

	public static string GetProBuilderPack()
	{
		string[] allFiles = Directory.GetFiles("Assets/6by7/ProBuilder/Install/", "*.*", SearchOption.AllDirectories);
		string[] allPackages = System.Array.FindAll(allFiles, name => name.EndsWith(".unitypackage"));
		string pack = "";
		// if(allPackages.Length > 3)	
		// 	Debug.LogWarning("Whoa, your project is full of unitypackages.");

		// sort out non ProBuilder2v- packages
		int highestVersion = 0;
		for(int i = 0; i < allPackages.Length; i++) {
			if(allPackages[i].Contains("ProBuilder2-v")) {

				// probably not necessary here, but regular expressions are super cool.  
				string pattern = @"[v0-9]{4,6}";
				MatchCollection matches = Regex.Matches(allPackages[i], pattern, RegexOptions.IgnorePatternWhitespace);

				int revision = -1;
				foreach(Match m in matches)
					revision = int.Parse(m.ToString().Replace("v", ""));
				
				if(revision < 1)
					continue;

				if(revision > highestVersion) {
					highestVersion = revision;
					pack = allPackages[i];
				}
			}
		}
		// Debug.Log("Importing ProBuilder revision number " + highestVersion);
		return pack;
	}

	public static string GetProBuilderUpgradePack()
	{
		string[] allFiles = Directory.GetFiles("Assets/", "*.*", SearchOption.AllDirectories);
		string[] allPackages = System.Array.FindAll(allFiles, name => name.EndsWith(".unitypackage"));

		for(int i = 0; i < allPackages.Length; i++) {
			if(allPackages[i].Contains("upgrade") && allPackages[i].Contains("ProBuilder2"))
				return allPackages[i];
		}

		return "";
	}

	// thanks, george - http://answers.unity3d.com/questions/10580/editor-script-how-to-clear-the-console-output-wind.html
	public static void ClearLog()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(SceneView));

		System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
		MethodInfo method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}
}

#endif