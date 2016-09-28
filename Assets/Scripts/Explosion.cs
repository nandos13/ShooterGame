using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{
    public int emitCount = 50;
    public float explosionRadius = 4;
    public float explosionForce = 10;

	void Start () 
    {
        GetComponent<ParticleSystem>().Emit(emitCount);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
            if (col.gameObject.GetComponent<Rigidbody>())
                col.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 2.0f, ForceMode.Impulse);
	}
	
	void Update () 
    {
        if (GetComponent<ParticleSystem>().particleCount == 0)
            GameObject.Destroy(gameObject);
	}
}
