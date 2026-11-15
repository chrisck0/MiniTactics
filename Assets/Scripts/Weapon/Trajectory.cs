using System;
using UnityEngine;

public class Trajectory : MonoBehaviour, IPoolable
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private int _linePoints = 10;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void DrawTrajectory(UnitBase owner, UnitBase target)
    {
        transform.position = owner.transform.position;

        float distance = Vector3.Distance(target.transform.position, owner.transform.position);
        Vector3 startVelocity = new Vector3(
            (target.transform.position - owner.transform.position).normalized.x,
            1f,
            (target.transform.position - owner.transform.position).normalized.z
            ) * Mathf.Sqrt(- distance * Physics.gravity.y / 2);

        float time = 0;
        _lineRenderer.positionCount = _linePoints;

        float intervalPoints = - 2 * startVelocity.y / Physics.gravity.y / _linePoints;

        for (int i = 0; i < _linePoints; i++)
        { 
            Vector3 point = new Vector3(
                startVelocity.x * time,
                startVelocity.y * time + Physics.gravity.y / 2 * time * time,
                startVelocity.z * time
                );
            _lineRenderer.SetPosition(i, transform.position + point);
            time += intervalPoints;
        }
    }

    public void ReturnToPool()
    {
    }
}
