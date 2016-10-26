using UnityEngine;
using System.Collections;

[System.Serializable]
public class RotationPieces {

	public Transform transform;
	public float clampUp = 0.0f;
	public float clampDown = 0.0f;
	public float clampLeft = 0.0f;
	public float clampRight = 0.0f;
	[Range (0.0f, 100.0f)]
	public float speed = 1.0f;

	public void rotateToFace (Transform target, float speedOverride = -1.0f)
	{
		/* This function rotates a transform (this.transform) to face another transform (target),
		 * applying clamps to the rotation angles, and smoothing the rotation
		 * by speed.
		 */

		if (transform)
		{
			// Store old rotation for later
			Quaternion oldRotation = transform.rotation;

			// Find vector from the turret to the player
			Vector3 angleToPoint = target.position - transform.position;

			// Calculate lateral angle needed to rotate towards target (side to side angle)
			Vector3 lateralDir = angleToPoint;
			lateralDir.y = 0;
			lateralDir.Normalize ();

			// Ensure the transform will turn the correct way (rather than turning the long way around)
			float lateralAngle = Vector3.Angle (Vector3.forward, lateralDir) * Mathf.Sign (Vector3.Cross (Vector3.forward, lateralDir).y);

			// Clamp lateral angle to specified range
			lateralAngle = Mathf.Clamp (lateralAngle, -clampLeft, clampRight);
			Quaternion lateralRotation = Quaternion.AngleAxis (lateralAngle, Vector3.up);
			transform.rotation = lateralRotation;

			// Calculate medial angle needed to rotate towards target (up and down angle)
			Vector3 medialDir = angleToPoint;
			medialDir.Normalize ();
			float medialAngle = Vector3.Angle (transform.forward, medialDir);

			// Clamp medial angle to specified range
			medialAngle = Mathf.Clamp (medialAngle, -clampDown, clampUp);
			Quaternion otherRotation = Quaternion.AngleAxis (medialAngle, Vector3.Cross (transform.forward, medialDir));

			// If speed override is specified, use that. Else, use speed variable
			float currentRotateSpeed = speedOverride;
			if (currentRotateSpeed < 0)
				currentRotateSpeed = speed;

			transform.rotation = Quaternion.Slerp (oldRotation, otherRotation * lateralRotation, Time.deltaTime * currentRotateSpeed);
		}
	}
}
