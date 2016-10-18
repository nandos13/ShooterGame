using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
        private bool alive;

        // var for patrol
        public GameObject[] waypoints;
        private int waypointInd = 0;
        public float patrolSpeed = 0.5f;

        // var for chasing
        public float chaseSpeed = 1f;
        public GameObject target;

         
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updatePosition = true;
            agent.updateRotation = false;

            state = AIEnemyControl.State.PATROL;
            alive = true;
        }

        void Update()
        {
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
                Debug.Log("1");
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
            //else
            //{
            //    character.Move(Vector3.zero, false, false);
            //}
        }

        void Chase()
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.transform.position);
            character.Move(agent.desiredVelocity, false, false);

        }

        void OnTriggerEnter (Collider coll)
        {
            if (coll.tag == "Player")
            {
                state = AIEnemyControl.State.CHASE;
                target = coll.gameObject;
            }
        }


    }

}
