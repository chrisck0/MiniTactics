using UnityEngine;

public class UnitRotation : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    [SerializeField] private float _rotationSpeed;
    private Vector3 _direction;
    private bool _isRotating;
    private WeaponType _weaponType;
    private DirectionType _weaponDirection;

    public bool IsRotating => _isRotating;

    private void Update() => Rotate();

    public void SetDirection(WeaponType weaponType, Vector2Int targetCoord, DirectionType weaponDirection)
    {
        _direction = transform.forward;
        _weaponType = weaponType;
        _weaponDirection = weaponDirection;

        switch (_weaponType)
        {
            case WeaponType.SelectTarget:
                _direction = (targetCoord.CoordToWorld() - transform.position).normalized;
                _isRotating = true;
                break;
            case WeaponType.SelectDirection:
                _direction = weaponDirection.DirectionToVector3();
                _isRotating = true;
                break;
            case WeaponType.AllRange:
                _isRotating = false;
                break;
        }
    }

    private void Rotate()
    {
        if (!_isRotating) return;

        if (Vector3.Dot(transform.forward, _direction) > 0.99f)
        {
            _isRotating = false;
            if (_weaponType == WeaponType.SelectDirection)
            {
                transform.forward = _weaponDirection.DirectionToVector3();
            }
            return;
        }

        transform.forward = Vector3.Slerp(transform.forward, _direction, Time.deltaTime * _rotationSpeed);
    }
}
