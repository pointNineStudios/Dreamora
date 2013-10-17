using UnityEngine;
using UnityEditor;
using System.Collections;

public class ProBuilderMenuItems : EditorWindow {

	[MenuItem("Window/ProBuilder/ProBuilder Cube _%k")]
	public static void Cube()
	{
		GameObject go = ProBuilder.CreatePrimitive(ProBuilder.Shape.Cube);
		pb_Editor_Utility.SetEntityType(ProBuilder.EntityType.Brush, go);
		pb_Editor_Utility.ScreenCenter( go );
	}

	[MenuItem("Window/ProBuilder/Open Shape Menu %#k")]
	public static void ShapeMenu()
	{
		EditorWindow.GetWindow(typeof(pb_Geometry_Interface), true, "Shape Menu", true);
	}

	[MenuItem("Window/ProBuilder/ProBuilder Window")]
	public static void PbWin()
	{
		if(EditorPrefs.HasKey("defaultOpenInDockableWindow") && !EditorPrefs.GetBool("defaultOpenInDockableWindow"))
			EditorWindow.GetWindow(typeof(pb_Editor), true, "ProBuilder", true);			// open as floating window
		else
			EditorWindow.GetWindow(typeof(pb_Editor), false, "ProBuilder", true);			// open as dockable window
	}
}