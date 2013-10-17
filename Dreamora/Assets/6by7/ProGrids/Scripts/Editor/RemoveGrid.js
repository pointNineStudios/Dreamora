#pragma strict
class RemoveGrid extends EditorWindow 
{
	@MenuItem("Window/6by7/Utility/Remove Grid")
    static function Initialize()
	{
		if(GameObject.Find("_grid"))
		{
			DestroyImmediate(GameObject.Find("_grid"));
			Debug.Log("Grid Deleted!");
		}
		else
			Debug.Log("Could not find grid object to delete!");
    }
}