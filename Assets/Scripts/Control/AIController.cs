using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionDuration = 2f; // How long after losing the player to wait before moving back to patrol behavior
        [SerializeField] float dwellDuration = 1f; // How long will the enemy wait at each waypoint before moving on
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float patrolSpeedFraction = 0.2f;

        Fighter fighter;
        GameObject player;

        private Vector3 guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeDwelling = 0;
        private int currentWaypointIndex = 0;

        private void Start() {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");

            guardPosition = transform.position;
        }

        private void Update()
        {
            if(GetComponent<Health>().IsDead()) return;

            if(InAttackRangeOfPlayer()&& fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionDuration)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void PatrolBehavior()
        {
            // Find the next waypoint in the patrol loop to go to if there is a patrol behavior

            Vector3 nextPosition = guardPosition;

            if (patrolPath != null)
            {
                if(AtWayPoint())
                {
                    if (DwellingAtWaypoint()) return;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
                
            }

            GetComponent<Mover>().StartMoveAction(nextPosition, patrolSpeedFraction); // go back to starting position
        }

        private bool DwellingAtWaypoint()
        {
            // Pause at each way point for a period before moving on to the next
            timeDwelling += Time.deltaTime;

            if(timeDwelling < dwellDuration)
            {
                return true;
            }
            
            timeDwelling = 0;
            return false;
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWayPoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            if(Vector3.Distance(transform.position, player.transform.position) < chaseDistance) { return true; }

            return false;
        }

        private void GiveChase(){

            Fighter fighter = GetComponent<Fighter>();
            GameObject player = GameObject.FindWithTag("Player");
            if(fighter.CanAttack(player))
            {
                fighter.Attack(player);
            }
            Debug.Log(transform.gameObject.name + "Chasing");
        }

        // private void StopChasing()
        // {
        //     GetComponent<Fighter>().Cancel(); // untarget the player
        //     GetComponent<Mover>().StartMoveAction(guardPosition); // go back to starting position
        // }

        // Called by unity
        private void OnDrawGizmosSelected() {
            // Used to visualize the aggro distance of the enemy

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
        
        
    }
}

