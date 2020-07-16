//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PathManager : MonoBehaviour
//{
//    public int walkSpeed;

//    private Stack<Vector3> currentPath;
//    private Vector3 currentWaypointPosition;
//    private float moveTimeTotal;
//    private float moveTimeCurrent;

//    public void NavigateTo(Vector3 destination)
//    {
//        currentPath = new Stack<Vector3>();
//        var currentNode = FindClosestWaypoint(transform.position);
//        var endNode = FindClosestWaypoint(destination);
//        if (currentNode == null || endNode == null || currentNode == endNode)
//            return;

//        var Op
//    }

//    public void Stop();

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    private Waypoint FindClosestWaypoint(Vector3 target)
//    {
//        GameObject closest = null;
//        float closestDist = Mathf.Infinity;
//        foreach (var waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
//        {
//            var dist = (waypoint.transform.position - target).magnitude;
//            if(dist < closestDist)
//            {
//                closest = waypoint;
//                closestDist = dist;
//            }
//        }
//        if(closest != null)
//        {
//            return closest.GetComponent<Waypoint>();
//        }
//        return null;
//    }
//}
