using UnityEngine;
using System.Collections;

public class AiChase : MonoBehaviour {

    private GameObject target;
    // Use this for initialization
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<NavMeshAgent>().destination = target.transform.position;
    }
}

