using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Creates a health bar above a unit's head.
 */

public class HealthBarHandler : MonoBehaviour {

	public Vector3 offset = new Vector3(0,1,0);				// Position offset
	[Range (0.1f, 3.0f)]
	public float scale = 1;									// Scale of the health bar element
	[Range (4.0f, 100.0f)]
	public float transitionSpeed = 10;						// Speed of bar movement when damage is taken

	public bool fadeOut = true;
	[Range (0.0f, 3.0f)]
	public float fadeTime = 3.0f;

	public Canvas healthBarPrefab;							// The prefab canvas to display over head

	private Canvas hbInstance;								// Stores the new canvas

	void Start () 
	{
		// Create a new health bar
		if (healthBarPrefab)
		{
			hbInstance = Instantiate (healthBarPrefab) as Canvas;
			hbInstance.transform.SetParent(hbInstance.transform, true);
			// Set the appropriate variables
			HealthBarOverhead hbo = hbInstance.GetComponent<HealthBarOverhead>();
			if (hbo)
			{
				hbo.owner = this.gameObject;
				hbo.offset = offset;
				hbo.SetScale(new Vector3(scale, scale, scale));
				hbo.transitionSpeed = transitionSpeed;
				hbo.fadeOut = fadeOut;
				hbo.fadeTime = fadeTime;

				Health affectedHealth = GetComponent<Health>();
				if (!affectedHealth)
					affectedHealth = GetComponentInParent<Health>();

				if (affectedHealth)
					hbo.healthComponent = affectedHealth;
				else
					Debug.Log("Attempted to create health bar on object: " + gameObject.name + ", but no health component was found. Health bar will not update.");
			}
		}
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (offset + transform.position, 0.2f);
	}
}
