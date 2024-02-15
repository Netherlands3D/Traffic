using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] Transform[] Points;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f; // Separate rotation speed

    private int pointsIndex;

    void Start()
    {
        transform.position = Points[pointsIndex].transform.position;
    }

    void Update()
    {
        if (pointsIndex <= Points.Length - 1)
        {
            MoveTowardsPoint();
            RotateTowardsPoint();
        }
    }

    void MoveTowardsPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, Points[pointsIndex].transform.position, moveSpeed * Time.deltaTime);
        if (transform.position == Points[pointsIndex].transform.position)
        {
            pointsIndex += 1;
        }
    }

    void RotateTowardsPoint()
    {
        if (pointsIndex < Points.Length)
        {
            Vector3 direction = Points[pointsIndex].transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
