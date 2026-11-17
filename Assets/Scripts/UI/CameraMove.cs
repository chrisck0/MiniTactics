using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private LayerMask _ground;
    private Vector3 _tileCursorPosition;
    private Vector3 _leftTopPosition;
    private Vector3 _rightBottomPosition;
    private Ray _leftTopRay;
    private Ray _rightBottomRay;
    private int _verticalDirection;
    private int _horizontalDirection;


    private void Update()
    {
        CheckCurrentCoord();

        bool canMoveVertical = CanMoveVerticalScreen();
        bool canMoveHorizontal = CanMoveHorizontalScreen();
        if (!_canMove || !canMoveVertical && !canMoveHorizontal) return;

        _camera.transform.position += new Vector3(
            _horizontalDirection,
            0f,
            _verticalDirection
            ).normalized * _moveSpeed * Time.deltaTime;
    }

    public void CanMoveCamera(bool canMove)
    {
        _canMove = canMove;
    }

    public void SetTarget(Vector2Int coord)
    {
        _tileCursorPosition = coord.CoordToWorld();
    }

    public void CheckCurrentCoord()
    {
        int threshold = Screen.height / 4;

        Ray leftTopRay = Camera.main.ScreenPointToRay(new Vector2(threshold, Screen.height - threshold));
        Ray rightBottomRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width - threshold * 2, threshold));
        RaycastHit hit;

        _leftTopPosition = _tileCursorPosition;
        _rightBottomPosition = _tileCursorPosition;
            
        if (Physics.Raycast(leftTopRay, out hit, Mathf.Infinity, _ground))
        {
            _leftTopPosition = hit.point;
        }
        if (Physics.Raycast(rightBottomRay, out hit, Mathf.Infinity, _ground))
        {
            _rightBottomPosition = hit.point;
        }
    }

    public bool CanMoveVerticalScreen()
    {
        _verticalDirection = 0;

        if (_leftTopPosition.z < _tileCursorPosition.z) _verticalDirection = 1;
        if (_rightBottomPosition.z > _tileCursorPosition.z) _verticalDirection = -1;

        return _verticalDirection != 0;
    }

    public bool CanMoveHorizontalScreen()
    {
        _horizontalDirection = 0;

        if (_leftTopPosition.x > _tileCursorPosition.x) _horizontalDirection = -1;
        if (_rightBottomPosition.x < _tileCursorPosition.x - 1) _horizontalDirection = 1;

        return _horizontalDirection != 0;
    }
}
