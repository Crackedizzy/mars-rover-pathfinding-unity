using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathfindingTester : MonoBehaviour
{
    // The A* manager.
    private AStarManager AStarManager = new AStarManager();
    // List of possible waypoints.
    private List<GameObject> Waypoints = new List<GameObject>();
    // List of waypoint map connections. Represents a path.
    private List<Connection> AStarPath = new List<Connection>();
    // The start, goal and end nodes.
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject goal1;
    [SerializeField]
    private GameObject end;
    // Debug line offset.
    Vector3 OffSet = new Vector3(0, 0.3f, 0);
    // A list of all waypoint nodes set to goal in the environment.
    private List<GameObject> WaypointGoals = new List<GameObject>();
    // Movement variables.
    private float currentSpeed = 20;
    private int currentTargetArrayIndex = 0;
    private Vector3 currentTargetPos;
    private bool agentMove = true;
    // Route Timer.
    private float timer = 0;
    // Distance Calculator.
    private float totalDistance = 0;
    private Vector3 lastPosition = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        if (start == null || goal1 == null || end == null)
        {
            Debug.Log("No start, goal1 or end waypoints.");
            return;
        }
        VisGraphWaypointManager tmpWpM = start.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("Start is not a waypoint.");
            return;
        }
        tmpWpM = goal1.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("Goal1 is not a waypoint.");
            return;
        }
        tmpWpM = end.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("End is not a waypoint.");
            return;
        }
        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            VisGraphWaypointManager tmpWaypointMan =
                waypoint.GetComponent<VisGraphWaypointManager>();
            if (tmpWaypointMan)
            {
                Waypoints.Add(waypoint);
            }
        }
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            VisGraphWaypointManager tmpWaypointMan =
                waypoint.GetComponent<VisGraphWaypointManager>();
            if (tmpWaypointMan.WaypointType == VisGraphWaypointManager.waypointPropsList.Goal)
            {
                WaypointGoals.Add(waypoint);
            }
            // Loop through a waypoints connections.
            foreach (VisGraphConnection aVisGraphConnection in tmpWaypointMan.Connections)
            {
                if (aVisGraphConnection.ToNode != null)
                {
                    Connection aConnection = new Connection();
                    aConnection.FromNode = waypoint;
                    aConnection.ToNode = aVisGraphConnection.ToNode;
                    AStarManager.AddConnection(aConnection);
                }
                else
                {
                    Debug.Log("Warning, " + waypoint.name +
                              " has a missing to node for a connection!");
                }
            }
        }
        // Run A Star...
        // AStarPath stores all the connections in the path/route to the goal/end node.
        AStarPath = AStarManager.PathfindAStar(start, goal1);
        if (AStarPath.Count == 0)
        {
            Debug.Log("Warning, A* did not return a path between the start and end node.");
        } 
        List<Connection> pGoal1 = new List<Connection>(AStarPath);
        List<Connection> goalToEnd = AStarManager.PathfindAStar(goal1, end);

        if (goalToEnd.Count == 0)
        {
            Debug.Log("Warning, A* did not return a path between the goal and end node.");
        }
        pGoal1.AddRange(goalToEnd);
        AStarPath = pGoal1;
        lastPosition = transform.position;
    }
    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Draw path.
        foreach (Connection aConnection in AStarPath)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet),
                (aConnection.ToNode.transform.position + OffSet));
        }
    }
    void AvoidObstacle(Vector3 avoidanceTarget)
    {
        // Set target position for avoidance
        Vector3 direction = avoidanceTarget - transform.position;
        direction.y = 0; // Keep movement on a flat plane
        float distance = direction.magnitude;

        if (distance > 0)
        {
            Vector3 normDirection = direction / distance;
            transform.position += normDirection * currentSpeed * Time.deltaTime;

            // Face the avoidance target
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = rotation;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (agentMove)
        {
            //Raycast variables
            float avoidDistance = 3f;
            float lookAhead = 5f;
            Vector3 EntityPosition = transform.position + Vector3.up * 0.5f;
            Debug.DrawRay(EntityPosition, transform.forward * lookAhead, Color.red);


            // Forward Raycast
            RaycastHit hitForwardRight;
            bool obstacleDetected = Physics.Raycast(EntityPosition, transform.forward, out hitForwardRight, lookAhead);

            if (obstacleDetected)
            {
                if (hitForwardRight.collider.CompareTag("Obstacle"))
                {
                    Debug.Log("Obstacle detected: " + hitForwardRight.collider.name);

                    // Calculate avoidance target
                    Vector3 avoidanceTarget = hitForwardRight.point + (hitForwardRight.normal * avoidDistance);

                    // Adjust movement towards avoidance target
                    AvoidObstacle(avoidanceTarget);
                    return;
                }
            }

            // Timer and distance.
            Vector3 tmpDir = lastPosition - transform.position;
            float tmpDistance = tmpDir.magnitude;
            totalDistance += tmpDistance;
            //Debug.Log("distance: " + TotalDistance);
            lastPosition = transform.position;
            timer += Time.deltaTime;
            // Set the current target.
            currentTargetPos = AStarPath[currentTargetArrayIndex].ToNode.transform.position;
            // Clear y to avoid up/down movement. Assumes flat surface.
            currentTargetPos.y = transform.position.y;
            // Get a vector to the target position.
            Vector3 direction = currentTargetPos - transform.position;
            // Calculate the length of the relative position vector
            float distance = direction.magnitude;
            // Face in the right direction.
            direction.y = 0;
            if (direction.magnitude > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = rotation;
            }
            // Calculate the normalised direction to the target from a game object.
            Vector3 normDirection = direction / distance;
            // Move the game object.
            transform.position = transform.position + normDirection * currentSpeed * Time.deltaTime;
            // Check if close to current target.
            if (distance < 1)
            {
                if (AStarPath[currentTargetArrayIndex].ToNode.Equals(goal1) == true)
                {
                    Debug.Log("Time: " + timer);
                    Debug.Log("Distance: " + totalDistance);
                    totalDistance = 0;
                    timer = 0;
                }
                // Close to target, so move to the next target in the list (if there is one).
                currentTargetArrayIndex++;
                if (currentTargetArrayIndex == AStarPath.Count)
                {
                    // The A* agent has reached the goal location.
                    // Decide what it should do next.
                    // For example, it could plan a route back to the start and then stop.
                    // Output timer and distance information.
                    Debug.Log("Time: " + timer);
                    Debug.Log("Distance: " + totalDistance);
                    totalDistance = 0;
                    timer = 0;
                    // Check if the current target is the start node. If yes, then stop.
                    if (AStarPath[(currentTargetArrayIndex - 1)].ToNode.Equals(start) == true)
                    {
                        agentMove = false;
                        Debug.Log("Agent Stopped.");
                        return;
                    }
                    // Not back at start, so plan path back to the start.
                    AStarPath = AStarManager.PathfindAStar(end, start);
                    currentTargetArrayIndex = 0;
                }
            }
        }
        else
        {
            // This code runs if agentMove is false.
        }
    }
}