using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;

        private NavMeshAgent navMeshAgent;
        private Animator animator;
        Ray lastRay;

        private GameObject self;


        // Start is called before the first frame update
        void Start()
        {
            self = gameObject;
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !GetComponent<Health>().IsDead(); // true when dead. avoids afterlife collisions
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed*Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
        private void UpdateAnimator()
        {
            Vector3 v = navMeshAgent.velocity; // global
            Vector3 vLocal = transform.InverseTransformDirection(v);
            float speed = vLocal.z;


            animator.SetFloat("forwardSpeed", speed);
        }
    }
}
