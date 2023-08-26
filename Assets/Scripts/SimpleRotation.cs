using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.back * speed);
    }

}