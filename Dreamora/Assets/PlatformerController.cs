using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class PlatformerController : MonoBehaviour {
	private float Speed = 6.0F;
	public float WalkSpeed = 6.0F;
	public float RunSpeed = 12.0F;
	public float AirSpeed = 4.0F;
    public float jumpSpeed = 20.0F;
    public float gravity = 50.0F;
	public float slideSpeed = 24.0F;
	public float slideLimit;
	private Animator anim;
    private Vector3 moveDirection = Vector3.zero;
	private Vector3 resetPosition;
	private RaycastHit hit;
	
	static int jumpState = Animator.StringToHash("Base Layer.Jump");
	
	CharacterController controller;
	
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();
		slideLimit = controller.slopeLimit - 0.1f;
		
		resetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalSpeed = Input.GetAxis("Horizontal")*0.75f;
		float verticalSpeed = Input.GetAxis("Vertical");
		
		if (verticalSpeed<0)
			verticalSpeed*=0.5f;
		else if (Input.GetButton("Walk")){
			verticalSpeed*=0.5f;
		}
		
		if (Input.GetButton("Walk")){
			horizontalSpeed*=0.5f;
		}
		
		if (Input.GetButton("Fire1")){
				transform.position = resetPosition;
		}
		transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);;
		
		if (Input.GetButton("Walk")){
			Speed = WalkSpeed;
		} else {
			Speed = RunSpeed;
		}
		
		if (controller.isGrounded) {
			var sliding = false;
			
			if (Physics.Raycast(transform.position, -Vector3.up, out hit, 3)) {
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			/*else {
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, hit);
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}*/
			if ( sliding ) {
				Vector3 hitNormal = hit.normal;
				moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
				Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
				moveDirection *= slideSpeed;
			} else {
				moveDirection = new Vector3(horizontalSpeed, 0, verticalSpeed);
				
				moveDirection = transform.TransformDirection(moveDirection);
		        moveDirection *= Speed;
			}
			
			anim.SetBool("InAir",false);
			
	        if (Input.GetButton("Jump")){
	            moveDirection.y = jumpSpeed;
			}
		} else {
			anim.SetBool("InAir",true);
				
			moveDirection.x = horizontalSpeed*Speed;
			moveDirection.z = verticalSpeed*Speed;
			
			moveDirection = transform.TransformDirection(moveDirection);
		}
		
		anim.SetFloat("Speed",verticalSpeed);
		anim.SetFloat("Direction",horizontalSpeed);
		
		moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit) {
	    Rigidbody body = hit.collider.attachedRigidbody;
	
	    // no rigidbody
	    if (body == null || body.isKinematic)
	        return;
	
	    // We dont want to push objects below us
	
	    if (hit.moveDirection.y < -0.5)
	    {
	        //platform = hit.transform;
	        return;
	    }

	    // Calculate push direction from move direction, 
	    // we only push objects to the sides never up and down
	    // if you wanted up and down pushing, change 0 to hit.moveDirection.y
	    Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
	
	    // If you know how fast your character is trying to move,
	    // then you can also multiply the push velocity by that.
	    // Apply the push and decrease speed to 25% of the normal walk speed.
	    body.velocity = (pushDir * Speed)/4;
	}
}
