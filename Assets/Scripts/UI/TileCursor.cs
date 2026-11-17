using UnityEngine;

public class TileCursor : MonoBehaviour
{
    [SerializeField] private CameraMove _cameraMove;

    /// <summary>
    /// 커서 위치 설정
    /// </summary>
    public void SetPosition(Vector2Int coord)
    {
        transform.position = coord.CoordToWorld();
        _cameraMove.SetTileCursorTarget(coord);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
