using UnityEngine;
using System.Collections;

public class CarExplosionController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float partDetachDelay = 0.05f * 3f; // для запису — нехай 1 кадр (~0.02s)
    [SerializeField] private Transform explosionCenter;

    private Rigidbody[] _parts;
    private Vector3[] _initialPositions;
    private Quaternion[] _initialRotations;

    private void Awake()
    {
        _parts = GetComponentsInChildren<Rigidbody>();
        int count = _parts.Length;
        _initialPositions = new Vector3[count];
        _initialRotations = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            var t = _parts[i].transform;
            _initialPositions[i] = t.localPosition;
            _initialRotations[i] = t.localRotation;
            _parts[i].isKinematic = true;
        }
    }

    private IEnumerator Start()
    {
        // Дочекайся першого кадру — Recorder вже повинен бути запущений
        yield return null;
        // Тепер викликай вибух
        Explode();
    }

    public void Explode()
    {
        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        // Невелика затримка, щоб переконатися, що Recorder зчитав початкові Transform
        yield return new WaitForSeconds(partDetachDelay);

        //impulseSource.GenerateImpulse();
        Vector3 center = explosionCenter ? explosionCenter.position : transform.position;
        foreach (var rb in _parts)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce, center, explosionRadius, 1f, ForceMode.Impulse);
        }
    }

    public void ResetParts()
    {
        for (int i = 0; i < _parts.Length; i++)
        {
            var rb = _parts[i];
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            var t = rb.transform;
            t.SetParent(transform);
            t.localPosition = _initialPositions[i];
            t.localRotation = _initialRotations[i];
        }
    }
}
