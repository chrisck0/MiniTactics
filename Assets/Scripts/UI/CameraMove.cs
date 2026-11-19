using Unity.Cinemachine;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private float _cursorMoveSpeed;
    [SerializeField] private float _targetMoveSpeed;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private LayerMask _ground;

    private Vector3 _targetPosition;
    private bool _leftTopExist;
    private bool _leftBottomExist;
    private bool _rightTopExist;
    private bool _rightBottomExist;
    private Vector3 _leftTopPosition;
    private Vector3 _rightBottomPosition;
    private Vector3 _currentPosition;
    private int _verticalDirection;
    private int _horizontalDirection;
    private bool _isTrackingTarget;

    public bool IsTrackingTarget => _isTrackingTarget;

    private void Update()
    {
        if (_canMove) FollowCursor();
        if (_isTrackingTarget) FollowTarget();
    }

    private void FollowCursor()
    {
        CheckCurrentCoord();
        SetDirection();

        bool canMoveVertical = CanMoveVertical();
        bool canMoveHorizontal = CanMoveHorizontal();
        if (!canMoveVertical && !canMoveHorizontal) return;

        _camera.transform.position += new Vector3(
            _horizontalDirection,
            0f,
            _verticalDirection
            ).normalized * _cursorMoveSpeed * Time.deltaTime;
    }

    private void FollowTarget()
    {
        SetCurrentPosition();
        CheckCurrentCoord();

        SetTargetDirection();
        bool canMoveVertical = CanMoveVertical();
        bool canMoveHorizontal = CanMoveHorizontal();

        if ((!canMoveVertical && !canMoveHorizontal))
        {
            _isTrackingTarget = false;
        }

        _camera.transform.position += new Vector3(
            _horizontalDirection,
            0f,
            _verticalDirection
            ).normalized * _targetMoveSpeed * Time.deltaTime;
    }

    public void CanMoveCamera(bool canMove)
    {
        _canMove = canMove;
    }

    public void SetTileTarget(Vector2Int coord)
    {
        _targetPosition = coord.CoordToWorld();
    }

    public void CheckCurrentCoord()
    {
        int threshold = Screen.height / 4;

        Ray leftTopRay = Camera.main.ScreenPointToRay(new Vector2(threshold, Screen.height - threshold));
        Ray leftBottomRay = Camera.main.ScreenPointToRay(new Vector2(threshold, threshold));
        Ray rightTopRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width - threshold, Screen.height - threshold));
        Ray rightBottomRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width - threshold, threshold));
        RaycastHit hit;

        _leftTopPosition = _currentPosition;
        _rightBottomPosition = _currentPosition;
        _leftTopExist = false;
        _leftBottomExist = false;
        _rightTopExist = false;
        _rightBottomExist = false;

        if (Physics.Raycast(leftTopRay, out hit, Mathf.Infinity, _ground))
        {
            _leftTopPosition = hit.point;
            _leftTopExist = true;
        }

        if (Physics.Raycast(leftBottomRay, out hit, Mathf.Infinity, _ground))
        {
            _leftBottomExist = true;
        }

        if (Physics.Raycast(rightTopRay, out hit, Mathf.Infinity, _ground))
        {
            _rightTopExist = true;
        }

        if (Physics.Raycast(rightBottomRay, out hit, Mathf.Infinity, _ground))
        {
            _rightBottomPosition = hit.point;
            _rightBottomExist = true;
        }
    }

    private void SetTargetDirection()
    {
        _horizontalDirection = 0;
        _verticalDirection = 0;

        if (Mathf.Abs(_currentPosition.x - _targetPosition.x) >= 0.1f)
        {
            if (_currentPosition.x < _targetPosition.x) _horizontalDirection = 1;
            if (_currentPosition.x > _targetPosition.x) _horizontalDirection = -1;
        }

        if (Mathf.Abs(_currentPosition.z - _targetPosition.z) >= 0.1f)
        {
            if (_currentPosition.z < _targetPosition.z) _verticalDirection = 1;
            if (_currentPosition.z > _targetPosition.z) _verticalDirection = -1;
        }
    }

    private bool CanMoveVertical()
    {
        if (_verticalDirection == 1 && !_leftTopExist && !_rightTopExist)
        {
            _verticalDirection = 0;
        }
        if (_verticalDirection == -1 && !_leftBottomExist && !_rightBottomExist)
        {
            _verticalDirection = 0;
        }

        return _verticalDirection != 0;
    }

    private bool CanMoveHorizontal()
    {
        if (_horizontalDirection == -1 && !_leftTopExist && !_leftBottomExist)
        {
            _horizontalDirection = 0;
        }
        if (_horizontalDirection == 1 && !_rightTopExist && !_rightBottomExist)
        {
            _horizontalDirection = 0;
        }

        return _horizontalDirection != 0;
    }

    public void SetDirection()
    {
        _horizontalDirection = 0;
        _verticalDirection = 0;

        if (_leftTopPosition.z < _targetPosition.z) _verticalDirection = 1;
        if (_rightBottomPosition.z > _targetPosition.z) _verticalDirection = -1;
        if (_leftTopPosition.x > _targetPosition.x) _horizontalDirection = -1;
        if (_rightBottomPosition.x < _targetPosition.x) _horizontalDirection = 1;
    }

    public void SetCameraTrackTarget(Vector2Int coord)
    {
        _isTrackingTarget = true;
        SetTileTarget(coord);
    }

    public void SetCurrentPosition()
    {
        Ray middleRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(middleRay, out hit, Mathf.Infinity, _ground))
        {
            _currentPosition = new Vector3(
                hit.point.x,
                0f,
                hit.point.z
                );
        }
    }

    public void InitPosition(Vector3 position)
    {
        _camera.transform.position = position;
    }
}