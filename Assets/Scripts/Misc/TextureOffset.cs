using UnityEngine;
using System.Collections;

public class TextureOffset : MonoBehaviour {

	[Range (-10, 10)]
	public float xSpeed;
	[Range (-10, 10)]
	public float ySpeed;

	private Renderer r;
	private Vector2 offset = Vector2.zero;

	void Start () 
	{
		r = GetComponent<Renderer>();
	}

	void Update () 
	{
		offset.x += xSpeed * Time.deltaTime;
		if (offset.x > 0)
			offset.x -= ((int)offset.x - 0);
		
		offset.y += ySpeed * Time.deltaTime;
		if (offset.y > 0)
			offset.y -= ((int)offset.y - 0);
		r.material.SetTextureOffset("_MainTex", offset);
	}
}
