using UnityEngine;
using UnityEditor;
using System.Collections;

public class AutoNodraw : Editor {

	const float COLLISION_DISTANCE = .1f;
	public static Material nodrawMat;

	[MenuItem("Window/ProBuilder/Actions/Auto NoDraw (Beta!)")]
	public static void NoDrawAllTheThings()
	{
		nodrawMat = (Material)Resources.Load("Materials/NoDraw");
		if(nodrawMat == null)
		{
			Debug.LogError("Nodraw Material does not exist at location \"Resources/Materials/NoDraw.mat\".  Aborting before someone gets hurt.");
			return;
		}

		Undo.RegisterSceneUndo("AutoNodraw");

		// Find all ProBuilder objects in the scene.
		pb_Object[] pbs = (pb_Object[])Object.FindObjectsOfType(typeof(pb_Object));

		// Cycle through every quad
		foreach(pb_Object pb in pbs)
		{
			if(pb.entityType != ProBuilder.EntityType.Brush && pb.entityType != ProBuilder.EntityType.Occluder)
				continue;

			// If an object is a duplicate, incorrect faces may be nodrawn
			pb.DuplicateCheck();
			foreach(pb_Face q in pb.quads)
			{
				if(HiddenFace(pb, q, COLLISION_DISTANCE))
					// If a hidden face is found, set material to NoDraw
					pb.SetQuadMaterial(q, nodrawMat);
			}
		}
	}

	public static bool HiddenFace(pb_Object pb, pb_Face q, float dist)
	{
		// Grab the face normal
		Vector3 dir = pbUtil.PlaneNormal(pb.VerticesInWorldSpace(q));
		
		// And also the center of the face
		Vector3 orig = pb.QuadCenter(q);

		// Case a ray from the center of the face out in the normal direction.
		// If an object is hit, return true (that this face is hidden), otherwise
		// return false.  This is pretty simplistic and doesn't account for a lot
		// of "gotchas", but it ought to serve as a fairly decent jumping off point
		// for NoDrawing a dense level.
		RaycastHit hit;
		if(Physics.Raycast(orig, dir, out hit, dist)) {
			// We've hit something.  Now check to see if it is a ProBuilder object,
			// and if so, make sure it's a visblocking brush.
			pb_Entity ent = hit.transform.GetComponent<pb_Entity>();
			if(ent != null)
			{
				if(ent.entityType == ProBuilder.EntityType.Brush || ent.entityType == ProBuilder.EntityType.Occluder)
					return true;		// it's a brush, blocks vision, return true
				else
					return false;		// not a vis blocking brush
			}
		}

		// It ain't a ProBuilder object of the entity type Brush or Occluder (world brush)
		return false;
	}
}
