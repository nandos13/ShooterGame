using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Handles player look behaviour.
 * Rotates the player for horizontal axis, and tilts the cameras and gun
 * for any movement on the vertical axis.
 */

//TODO: Correctly move BOTH cameras & viewmodel components

public class FPC_Aim : MonoBehaviour {

	private Vector2 mouseMovement;		// Tracks the current angle of the camera
	private Vector2 smoothV;			// Used to store the movement difference each frame

	[Range (0.1f, 10.0f)]
	public float sensitivity = 4.0f;	// Look sensitivity

	[Range(0.0f, 10.0f)]
	public float smoothing = 1.0f;		// 

	public Camera WorldViewCam;
	public Camera ViewModelCam;

	// Use this for initialization
	void Start () 
	{
		//TODO: Apply initial rotation so the player spawns facing the same direction as it is in the editor
	}

	// Update is called once per frame
	void Update () 
	{
		Vector2 moveDirection = new Vector2 (Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		moveDirection = Vector2.Scale (moveDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

		if (smoothing >= 1.0f) 
		{
			// Apply smoothing
			smoothV.x = Mathf.Lerp (smoothV.x, moveDirection.x, 1.0f/smoothing);
			smoothV.y = Mathf.Lerp (smoothV.y, moveDirection.y, 1.0f/smoothing);
		} else 
		{
			// Ignore smoothing
			smoothV.x = Mathf.Lerp (smoothV.x, moveDirection.x, 1.0f);
			smoothV.y = Mathf.Lerp (smoothV.y, moveDirection.y, 1.0f);
		}

		mouseMovement += smoothV;		// Apply the movement to the current angle

		//CLAMP camera to prevent vertical rotation past the direct upwards direction 
		//(preventing looking backwards with an upside down view)
		mouseMovement.y = Mathf.Clamp(mouseMovement.y, -88.0f, 88.0f);

		if (WorldViewCam)
			WorldViewCam.transform.localRotation = Quaternion.AngleAxis (-mouseMovement.y, Vector3.right);
		if (ViewModelCam)
			ViewModelCam.transform.localRotation = Quaternion.AngleAxis (-mouseMovement.y, Vector3.right);
		transform.localRotation = Quaternion.AngleAxis (mouseMovement.x, transform.up);


	}
}
