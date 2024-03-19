using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Traffic.Simulation;

namespace Netherlands3D.Traffic.Simulation
{
    public class TrafficLight : MonoBehaviour
    {
    public enum LightColor { Red, Yellow, Green }
    public LightColor currentColor = LightColor.Red;
    public event Action<LightColor> OnLightChanged;
    public float countdown;
    public event Action OnYellowLight;
    public float redLightDuration = 5f; // Declare the redLightDuration variable
    public float yellowLightDuration = 2f;
    public float greenLightDuration = 5f;
    private float currentDuration;
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());

        redLight = transform.Find("RedLight")?.gameObject;
        yellowLight = transform.Find("YellowLight")?.gameObject;
        greenLight = transform.Find("GreenLight")?.gameObject;

        if (redLight == null || yellowLight == null || greenLight == null)
        {
            Debug.LogError("One or more traffic light components are missing. Please check your TrafficLight GameObject in the Unity Editor.");
            return; // Exit early to avoid further null reference errors
        }

        SetLight(currentColor);
        currentDuration = redLightDuration;
    }

    private IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            currentColor = LightColor.Green;
            countdown = greenLightDuration;
            yield return new WaitForSeconds(greenLightDuration);

            currentColor = LightColor.Yellow;
            countdown = yellowLightDuration;
            yield return new WaitForSeconds(yellowLightDuration);

            currentColor = LightColor.Red;
            countdown = redLightDuration;
            yield return new WaitForSeconds(redLightDuration);
        }
    }

    private void SetLight(LightColor color)
    {
        redLight.SetActive(color == LightColor.Red);
        yellowLight.SetActive(color == LightColor.Yellow);
        greenLight.SetActive(color == LightColor.Green);
        OnLightChanged?.Invoke(color);
    }

    private void ChangeLight(LightColor newColor)
    {
        currentColor = newColor;
        SetLight(currentColor);
        Debug.Log("Light changed to " + currentColor);

        switch (currentColor)
        {
            case LightColor.Red:
                currentDuration = greenLightDuration;
                break;
            case LightColor.Yellow:
                currentDuration = yellowLightDuration;
                OnYellowLight?.Invoke(); // Trigger the OnYellowLight event
                break;
            case LightColor.Green:
                currentDuration = redLightDuration;
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            VehicleController vehicle = other.GetComponent<VehicleController>();
            if (vehicle != null)
            {   
                // Subscribe the vehicle to the traffic light changes
                vehicle.SubscribeToTrafficLight(this); // Pass this TrafficLight instance

                // Immediately apply the current light state to the vehicle
                if (currentColor == LightColor.Red)
                {
                    vehicle.StopVehicle(); // Stop the vehicle if the light is red
                }
                else
                {
                    vehicle.GoVehicle(); // Allow the vehicle to go if the light is green or yellow
                }
            }
        }
    }


    // OnTriggerExit is not required if vehicles should only stop when entering the trigger during a red light

    private void Update()
    {
        currentDuration -= Time.deltaTime;
        countdown -= Time.deltaTime;

        if (currentDuration <= 0)
        {
            switch (currentColor)
            {
                case LightColor.Green:
                    ChangeLight(LightColor.Yellow);
                    break;
                case LightColor.Yellow:
                    ChangeLight(LightColor.Red);
                    break;
                case LightColor.Red:
                    ChangeLight(LightColor.Green);
                    break;
            }
        }
    }
}

}
