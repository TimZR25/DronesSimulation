using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraMoving : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _startMoveSpeed;
    [SerializeField] private SpriteRenderer _groundSR;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _minZoom;
    [SerializeField] private float _maxZoom;

    private Vector2 _minBounds;
    private Vector2 _maxBounds;

    private Camera _camera;

    private float _moveSpeed;

    private void Awake()
    {
        _moveSpeed = _startMoveSpeed;

        _minBounds = _groundSR.bounds.min;
        _maxBounds = _groundSR.bounds.max;

        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 newPosition = transform.position + new Vector3(horizontal, vertical, 0) * _moveSpeed * Time.unscaledDeltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, _minBounds.x, _maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _minBounds.y, _maxBounds.y);

        transform.position = newPosition;
    }

    private void Zoom()
    {
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");

        if (zoomInput != 0)
        {
            float orthographicSize = Mathf.Clamp(
                _camera.orthographicSize - zoomInput * _zoomSpeed,
                _minZoom,
                _maxZoom
            );

            _camera.orthographicSize = orthographicSize;

            float minSpeed = _startMoveSpeed / 1.5f;
            float maxSpeed = _startMoveSpeed * 2;

            float clamp = Mathf.Clamp(orthographicSize, minSpeed, maxSpeed);

            _moveSpeed = minSpeed + maxSpeed - clamp;
        }
    }
}
