@CustomEditor (ProGrids)
class ProGrids_Ed extends Editor 
{
    function OnInspectorGUI() 
	{
		//DrawDefaultInspector();
		target.gridLineColor_XY = EditorGUILayout.ColorField("XY Color", target.gridLineColor_XY);
		target.gridLineColor_XZ = EditorGUILayout.ColorField("XZ Color", target.gridLineColor_XZ);
		target.gridLineColor_YZ = EditorGUILayout.ColorField("YZ Color", target.gridLineColor_YZ);
		if(GUILayout.Button("Save Settings"))
		{
			var gridPrefab = Resources.LoadAssetAtPath("Assets/6by7/ProGrids/_grid.prefab", typeof(Object));
			PrefabUtility.ReplacePrefab(target.gameObject, gridPrefab);
		}
		
		if(GUILayout.Button("Delete Grid"))
		{
			DestroyImmediate(target.gameObject);
		}
    }
}