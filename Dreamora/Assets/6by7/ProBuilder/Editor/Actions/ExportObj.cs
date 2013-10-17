using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportObj : MonoBehaviour {

	[MenuItem ("Window/ProBuilder/Actions/Export Selected to OBJ")]
	public static void ExportOBJ()
	{
		pb_Editor_Utility.ExportOBJ(pbUtil.GetComponents<pb_Object>(Selection.transforms)); 
	}
}
