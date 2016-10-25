using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/*
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // -- WIP
        AI script, Enemies patrol around map to set waypoints, Also has a Alert investigation, 
        where AI stops and waits a set time and goes back to patroling, unless player goes within 
        aggro range where Enemy will chase player. also has a lose aggro if player runs way far enough.

    ~   Add 'Size' of waypoints, drag waypoint objects into Element slot/s
    ~   Drag Player prefab into 'Target' slot

    // -- things to still add

    ~   Aggro on damaging AI
    ~   (BAF)Bring-A-Friend mechanics, attack an AI, and all surrounding AI will aggro too.
    ~   (LOS)Line-Of_sight mechanics, if behind a wall and out of sight but within range of AI,
            Ai will not Aggro.

    // -- Note: Did a stress test with 120 AI characters and had no lag problems.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
*/


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class AIEnemyControl : MonoBehaviour
    {
        public NavMeshAgent agent;
        public ThirdPersonCharacter character;

        public enum State
        {
            PATROL,
            CHASE
           
        }

        public State state;
        //private bool alive;

        // var for patrol
        public GameObject[] waypoints;
        private int waypointInd = 0;
        [Range(0, 20)]
        public float patrolSpeed = 0.5f;

        // var for chasing
        [Range(0, 20)]
        public float chaseSpeed = 1.0f;
        [Range(0, 200)]
        public float aggroRange = 15.5f;
        [Range(0, 200)]
        public float deAggro = 40.0f;
        public GameObject target;

        

        // var for sight
        public float heightMultiplier;
        [Range(0, 200)]
        public float sightDist = 20.0f;
        


        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updatePosition = true;
            agent.updateRotation = false;

            state = AIEnemyControl.State.PATROL;
            heightMultiplier = 4.22f;
            StartCoroutine("FSM");
            //alive = true;
        }

        void Update()
        {
            if(Vector3.Distance(target.transform.position, transform.position) >= deAggro)
                state = AIEnemyControl.State.PATROL;
            else if (Vector3.Distance(target.transform.position, transform.position) <= aggroRange)
                state = AIEnemyControl.State.CHASE;
            
            FSM();
        }
        
        void FSM()
        {
            switch(state)
            {
                case State.PATROL:
                    Patrol();
                    break;
                case State.CHASE:
                    Chase();
                    break;
                
            }
        }

        void Patrol()
        {
            agent.speed = patrolSpeed;
            if(Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) >= 5)
            {
                agent.SetDestination(waypoints[waypointInd].transform.position);
                character.Move(agent.desiredVelocity, false, false);        // 3rdpersoncontroller, false = crouch, jump ect
            }
            else
            {
                waypointInd += 1;
                if (waypointInd >= waypoints.Length)
                {
                    waypointInd = 0;
                }
            }
        }

        void Chase()
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.transform.position);
            character.Move(agent.desiredVelocity, false, false);

        }

        

        void FixedUpdate()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.green);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDist, Color.green);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDist, Color.green);

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = AIEnemyControl.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = AIEnemyControl.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    state = AIEnemyControl.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }
        }

    }
}
