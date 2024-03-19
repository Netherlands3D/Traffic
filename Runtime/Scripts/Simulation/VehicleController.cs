using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Traffic.Simulation;


namespace Netherlands3D.Traffic.Simulation
{
    public class VehicleController : MonoBehaviour
{
    public List<Transform> waypoints; // List of waypoints for the vehicle to follow
    private int currentWaypointIndex = 0; // Index of the current waypoint the vehicle is heading towards
    private bool isMoving = true;
    public float maxSpeed = 1.5f;
    private float currentSpeed = 1.5f; 
    public float turnSpeed = 1.5f;
    public float speed = 1.5f;
    public TrafficLight trafficLight;
    public TrafficLight currentTrafficLight;
    


    private void Start()
    {
        if (trafficLight != null)
        {
            trafficLight.OnYellowLight += HandleYellowLight;
            trafficLight.OnLightChanged += ReactToTrafficLight;
        }

        if (waypoints.Count > 0)
        {
            // Set the initial target rotation towards the first waypoint
            Vector3 initialDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
            Quaternion initialRotation = Quaternion.LookRotation(initialDirection);
            transform.rotation = initialRotation;
        }
    }

    private void Update()
    {
        if (isMoving && waypoints.Count > 0)
        {
            MoveVehicle();
            TurnVehicleTowardsWaypoint();
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime);
        }
        else if (currentTrafficLight != null && currentTrafficLight.currentColor == TrafficLight.LightColor.Red)
        {
            StopVehicle();
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime);
        }
        else
        {
            GoVehicle();
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime);
        }

        // Move the vehicle at the current speed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void MoveVehicle()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        // Check if the vehicle has reached the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.5f) // 0.5 is the threshold to consider the waypoint reached
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // Loop back to the first waypoint or stop the vehicle if it should only run once
            }
        }
    }

    private void TurnVehicleTowardsWaypoint()
    {
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    public void SubscribeToTrafficLight(TrafficLight trafficLight)
    {
        // this.trafficLight = trafficLight;
        // trafficLight.OnYellowLight += HandleYellowLight;
        if (currentTrafficLight != null)
        {
            currentTrafficLight.OnLightChanged -= HandleLightChange;
        }

        currentTrafficLight = trafficLight;
        currentTrafficLight.OnLightChanged += HandleLightChange;
    }

    public void ReactToTrafficLight(TrafficLight.LightColor color)
    {
        Debug.Log("Reacting to traffic light color: " + color);
        if (color == TrafficLight.LightColor.Red)
        {
            Debug.Log("Traffic light is red, stopping vehicle");
            StopVehicle();
        }
        else if (color == TrafficLight.LightColor.Green || color == TrafficLight.LightColor.Yellow)
        {
            Debug.Log("Traffic light is green or yellow, allowing vehicle to go");
            GoVehicle();
        }
    }

    public void HandleYellowLight()
    {
        StopVehicle();
    }

    private void HandleLightChange(TrafficLight.LightColor newColor)
    {
        if (newColor == TrafficLight.LightColor.Red)
        {
            StopVehicle();
        }
        else
        {
            GoVehicle();
        }
    }

    public void StopVehicle()
    {
        Debug.Log("StopVehicle method called");
        isMoving = false;
    }

    public void GoVehicle()
    {
        Debug.Log("GoVehicle method called");
        isMoving = true;
    }
}
}
