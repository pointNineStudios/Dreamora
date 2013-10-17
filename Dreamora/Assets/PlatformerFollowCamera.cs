using UnityEngine;
using System.Collections;

public class PlatformerFollowCamera : MonoBehaviour {
	public GameObject target;
	public float rotateSpeedX = 5;
	public float rotateSpeedY = 1.9f;
	public float CameraOffset = 8f;
	
	float x;
	float y;
	
	int yMinLimit = -40; // Down
	int yMaxLimit = 70; // Up
	
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		//x = transform.eulerAngles.x;
		y = transform.eulerAngles.y;
	}
	
	void LateUpdate() {
        x += Input.GetAxis("Mouse X") * rotateSpeedX;
        //target.transform.Rotate(0, x, 0);
		
		y -= Input.GetAxis("Mouse Y") * rotateSpeedY;
		
		y = ClampAngle(y, yMinLimit, yMaxLimit);
		
		Quaternion rotation = Quaternion.Euler(y, x, 0);
        //Vector3 localTargetOffset = target.transform.TransformDirection(Vector3.zero);
        //Vector3 targetPos = target.transform.position + localTargetOffset;
 
        //var targetDistance = AdjustLineOfSight(targetPos, direction);
        //currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, distanceVelocity, closerSnapLag * .3);
 
        transform.rotation = rotation;
        //transform.position = targetPos + direction * currentDistance;
		transform.position = target.transform.position - (rotation * new Vector3(0,0,CameraOffset));
		
		// Look at position
		Vector3 lookAtPosition = new Vector3(target.transform.position.x,target.transform.position.y+2,target.transform.position.z);
		
		transform.LookAt(lookAtPosition);
 
        //target.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
		
		/*
		float vertical = Input.GetAxis("Mouse Y") * rotateSpeedY;
        float desiredAngleY = target.transform.eulerAngles.y;
		float desiredAngleX = target.transform.eulerAngles.x;
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        transform.position = target.transform.position - (rotation * offset);
        transform.LookAt(target.transform);
		
		target.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);*/
		
		// Setup the layer mask for everything, except IgnoreCamera layer
		var layerMask = 1 << 9;
  		layerMask = ~layerMask;
		
		// Make sure the camera doesn't go behind objects and you lose sight of the player
		RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        
        if (Physics.Raycast(ray, out hit, Vector3.Distance(transform.position,target.transform.position),layerMask)) {
            //if (hit.rigidbody != null)
                //hit.rigidbody.AddForceAtPosition(ray.direction * pokeForce, hit.point);
			transform.position = hit.point+transform.forward.normalized;
			transform.LookAt(lookAtPosition);
        }
    }
	
	static float ClampAngle (float angle, float min, float max) {
	    if (angle < -360)
	        angle += 360;
	    if (angle > 360)
	        angle -= 360;
	    return Mathf.Clamp (angle, min, max);
	}
}
