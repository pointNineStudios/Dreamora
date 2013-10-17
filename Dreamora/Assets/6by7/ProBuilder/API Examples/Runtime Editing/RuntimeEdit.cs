using UnityEngine;
using System.Collections;

public class RuntimeEdit : MonoBehaviour {

	pb_Face quad;
	pb_Object pb;

	void Awake()
	{
		pb = (pb_Object)ProBuilder.CreatePrimitive(ProBuilder.Shape.Cube).GetComponent<pb_Object>();
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(5, Screen.height - 25, 80, 20), "Reset")) {
			if(pb != null)
				Destroy(pb.gameObject);

			pb = (pb_Object)ProBuilder.CreatePrimitive(ProBuilder.Shape.Cube).GetComponent<pb_Object>();
		}
	}

	Vector2 mousePosition_initial = Vector2.zero;
	bool dragging = false;
	public float rotateSpeed = 100f;
	public void LateUpdate()
	{
		if(pb == null)
			return;

		if(Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt)))
		{
			mousePosition_initial = Input.mousePosition;
			dragging = true;
		}

		if(dragging)
		{
			Vector2 delta = (Vector3)mousePosition_initial - (Vector3)Input.mousePosition;
			Vector3 dir = new Vector3(delta.y, delta.x, 0f);

			pb.gameObject.transform.RotateAround(Vector3.zero, dir, rotateSpeed * Time.deltaTime);
		}

		if(Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0))
		{
			dragging = false;
		}
	}

	public void Update()
	{
		if(Input.GetMouseButtonUp(0) && !Input.GetKey(KeyCode.LeftAlt)) {
			if(FaceCheck(Input.mousePosition))
			if(pb != null && quad != null)
			{
				Vector3 nrml = pbUtil.PlaneNormal(pb.VerticesInWorldSpace(quad));
				if(Input.GetKey(KeyCode.LeftShift))
					pb.TranslateVertices(quad.DistinctIndices(), nrml.normalized * -.5f);
				else
					pb.TranslateVertices(quad.DistinctIndices(), nrml.normalized * .5f);
			}
		}
	}

	public bool FaceCheck(Vector3 pos)
	{
		Ray ray = Camera.main.ScreenPointToRay (pos);
		RaycastHit hit;

		if( Physics.Raycast(ray.origin, ray.direction, out hit))
		{
			pb_Object hitpb = hit.transform.gameObject.GetComponent<pb_Object>();

			if(hitpb == null)
				return false;

			Mesh m = hitpb.msh;

			int[] tri = new int[3] {
				m.triangles[hit.triangleIndex * 3 + 0], 
				m.triangles[hit.triangleIndex * 3 + 1], 
				m.triangles[hit.triangleIndex * 3 + 2] 
			};

			pb = hitpb;
			quad = hitpb.QuadWithTriangle(tri);
			return true;
		}
		return false;
	}
}
