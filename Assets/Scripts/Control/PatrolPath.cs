using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Control
{

    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] float waypointGizmoRadius;

        private void Start() {
            
        }
        private void OnDrawGizmos() {
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 waypoint = GetWaypoint(i);

                if (Selection.Contains(gameObject))
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                
                Gizmos.DrawSphere(waypoint, waypointGizmoRadius);

                int j = GetNextIndex(i);
                Vector3 nextWaypoint = GetWaypoint(j);
                Gizmos.DrawLine(waypoint, nextWaypoint);
                 
            }
        }

        
        public int GetNextIndex (int i)
        {
            return (i+1) % transform.childCount;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
