using System.Collections;
using System.Linq;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    [SerializeField] private Transform[] wheels;
    
    private bool _isRotating;
    private float _speed;
    private float[] _wheelsRadii;
    private void Start()
    {
        _wheelsRadii = wheels.Select(WheelRadiusCalculation).ToArray();
    }

    private float WheelRadiusCalculation(Transform wheel)
    {
        return wheel.localScale.x / 2f;
    }

    public void StartRotating(float speed)
    {
        _isRotating = true;
        _speed = speed;

        StartCoroutine(Rotating());
    }

    private IEnumerator Rotating()
    {
        while (_isRotating)
        {
            Rotate();
            
            yield return null;
        }
    }

    private void Rotate()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].Rotate(Vector3.right, CalculateRotationDegrees(i));
        }
    }
    
    private float CalculateRotationDegrees(int wheelRadiusIndex)
    {
        float rotationSpeed = _speed / _wheelsRadii[wheelRadiusIndex];
        float rotationDegrees = rotationSpeed * Mathf.Rad2Deg;
        
        return rotationDegrees;
    }
    
    public void StopRotating()
    {
        _isRotating = false;
    }
}