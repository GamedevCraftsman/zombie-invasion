using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GatesController : MonoBehaviour
{
    [System.Serializable]
    public class GateData
    {
        public Transform hinge;
        public Transform gate;
        
        [HideInInspector] public Quaternion initialRotation;
    }

    [SerializeField] private GateData[] gates;
    [SerializeField] private float openAngle = 45f;
    [SerializeField] private float openTime = 2;

    private readonly List<Coroutine> _openCoroutines = new ();
    private void Awake()
    {
        foreach (var g in gates)
            g.initialRotation = g.hinge.rotation;
    }

    public void OpenGates()
    {
        foreach (var gate in gates)
        {
            _openCoroutines.Add(StartCoroutine(OpenGate(gate)));
        }
    }

    private IEnumerator OpenGate(GateData data)
    {
        // Vector from hinge to gate
        Vector3 hingeToGate = (data.gate.position - data.hinge.position).normalized;

        // Determining hinge side
        float side = Vector3.Dot(Vector3.Cross(Vector3.up, hingeToGate), data.hinge.forward);

        float direction = side > 0 ? 1f : -1f;

        // Angels calculation
        Quaternion startRot = data.hinge.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0,  -direction * openAngle);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / openTime;
            data.hinge.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }

    public void ResetGates()
    {
        _openCoroutines?.ForEach(StopCoroutine);
        _openCoroutines?.Clear();

        gates.ToList().ForEach(g => g.hinge.rotation = g.initialRotation);
    }
}
