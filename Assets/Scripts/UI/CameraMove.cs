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
    private Vector3 _mapLeftTopPosition;
    private Vector3 _mapLeftBottomPosition;
    private Vector3 _mapRightTopPosition;
    private Vector3 _mapRightBottomPosition;
    private Vector3 _leftTopPosition;
    private Vector3 _leftBottomPosition;
    private Vector3 _rightTopPosition;
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

        _leftTopPosition = _mapLeftTopPosition;
        _leftBottomPosition = _mapLeftBottomPosition;
        _rightTopPosition = _mapRightTopPosition;
        _rightBottomPosition = _mapRightBottomPosition;

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
            _leftBottomPosition = hit.point;
            _leftBottomExist = true;
        }

        if (Physics.Raycast(rightTopRay, out hit, Mathf.Infinity, _ground))
        {
            _rightTopPosition = hit.point;
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
        float leftX = Mathf.Max(_leftTopPosition.x, _leftBottomPosition.x);
        float rightX = Mathf.Min(_rightTopPosition.x, _rightBottomPosition.x);
        float topZ = Mathf.Min(_leftTopPosition.z, _rightTopPosition.z);
        float bottomZ = Mathf.Max(_leftBottomPosition.z, _rightBottomPosition.z);

        if (topZ < _targetPosition.z) _verticalDirection = 1;
        if (bottomZ > _targetPosition.z) _verticalDirection = -1;
        if (leftX > _targetPosition.x) _horizontalDirection = -1;
        if (rightX < _targetPosition.x) _horizontalDirection = 1;
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

    public void InitPosition(Vector3 position,
        Vector3 leftTopPosition, Vector3 leftBottomPosition,
        Vector3 rightTopPosition, Vector3 rightBottomPosition)
    {
        _camera.transform.position = position;
        _mapLeftTopPosition = leftTopPosition;
        _mapLeftBottomPosition = leftBottomPosition;
        _mapRightTopPosition = rightTopPosition;
        _mapRightBottomPosition = rightBottomPosition;
    }
}