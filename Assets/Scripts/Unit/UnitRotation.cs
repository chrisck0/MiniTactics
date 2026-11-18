using UnityEngine;

public class UnitRotation : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    [SerializeField] private float _rotationSpeed;
    private Vector3 _direction;
    private Vector3[] _directions = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
    private bool _isRotating;

    public bool IsRotating => _isRotating;

    private void Update() => Rotate();

    public void SetDirection(Vector2Int targetCoord, DirectionType weaponDirection)
    {
        _direction = transform.forward;

        if (weaponDirection == DirectionType.None)
        {
            _direction = (targetCoord.CoordToWorld() - transform.position).normalized;
            _isRotating = true;
        }
        else
        {
            _direction = _directions[(int)weaponDirection - 1];
            _isRotating = true;
        }

        Debug.Log($"Rotation {_direction}");
    }

    public void Rotate()
    {
        if (!_isRotating) return;

        if (Vector3.Dot(transform.forward, _direction) > 0.99f)
        {
            _isRotating = false;
            return;
        }

        transform.forward = Vector3.Slerp(transform.forward, _direction, Time.deltaTime * _rotationSpeed);
    }
}
