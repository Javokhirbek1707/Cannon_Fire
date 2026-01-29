using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private Transform _cannonBase;
    [SerializeField] 
    private Transform _smallCannon;
    [SerializeField]
    private Transform _firePoint;

    [SerializeField]
    private GameObject _shotPrefab;
    [SerializeField]
    private float _shootForce = 20f;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _nextFireTime;

    [SerializeField]
    private int _maxShots = 20;
    private int _currentShots;

    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _linePoints = 30;
    [SerializeField]
    private float _timeStep = 0.1f;

    [SerializeField]
    private float _yawSpeed = 10f;
    [SerializeField]
    private float _pitchSpeed = 5f;
    [SerializeField]
    private float _shiftMultiplier = 3f;

    [SerializeField]
    private float _minYaw = -50f;
    [SerializeField]
    private float _maxYaw = 50f;
    [SerializeField] 
    private float _minPitch = -30f;
    [SerializeField]
    private float _maxPitch = -5f;

    private float _yaw;
    private float _pitch;

    [SerializeField]
    private LayerMask _trajectoryMask;

    private bool _trajectoryLocked = false;

    private void Start()
    {
        _currentShots = _maxShots;
        UIManager.Instance.UpdateShots(_currentShots);

        _yaw = _cannonBase.localEulerAngles.y;
        if (_yaw > 180f) _yaw -= 360f;

        _pitch = _smallCannon.localEulerAngles.x;
        if (_pitch > 180f) _pitch -= 360f;
    }

    private void Update()
    {
        float multiplier = Input.GetKey(KeyCode.LeftShift) ? _shiftMultiplier : 1f;

        HandleYaw(multiplier);
        HandlePitch(multiplier);

        if (!_trajectoryLocked)
        {
            DrawTrajectory();
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= _nextFireTime && _currentShots > 0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= _nextFireTime && _currentShots > 0)
        {
            Shoot();
        }
    }

    private void HandleYaw(float multiplier)
    {
        float h = Input.GetAxis("Horizontal");
        _yaw += h * _yawSpeed * multiplier * Time.deltaTime;
        _yaw = Mathf.Clamp(_yaw, _minYaw, _maxYaw);
        _cannonBase.localRotation = Quaternion.Euler(0f, _yaw, 0f);
    }

    private void HandlePitch(float multiplier)
    {
        float v = Input.GetAxis("Vertical");
        _pitch -= v * _pitchSpeed * multiplier * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        _smallCannon.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void Shoot()
    {
        _trajectoryLocked = true;

        _currentShots--;
        UIManager.Instance.UpdateShots(_currentShots);
        _nextFireTime = Time.time + _fireRate;

        GameObject shot = Instantiate(_shotPrefab, _firePoint.position, _firePoint.rotation);
        Rigidbody rb = shot.GetComponent<Rigidbody>();
        rb.AddForce(_firePoint.forward * _shootForce, ForceMode.Impulse);

        if (_currentShots <= 0)
            GameManager.Instance.OnShotsEmpty();

        _lineRenderer.enabled = false;
        Invoke(nameof(EnableLine), 0.05f);

    }

    private void EnableLine()
    {
        _lineRenderer.enabled = true;
    }

    public void UnlockTrajectory()
    {
        _trajectoryLocked = false;
    }


    private void DrawTrajectory()
    {
        Vector3 position = _firePoint.position;
        Vector3 velocity = _firePoint.forward * _shootForce;

        _lineRenderer.positionCount = _linePoints;

        bool grounded = false;

        for (int i = 0; i < _linePoints; i++)
        {
            _lineRenderer.SetPosition(i, position);

            if (!grounded)
            {
                Vector3 nextVelocity = velocity + Physics.gravity * _timeStep;
                Vector3 nextPosition = position + nextVelocity * _timeStep;

                if (Physics.Raycast(position,nextPosition - position, out RaycastHit hit,Vector3.Distance(position, nextPosition),_trajectoryMask))
                {
                    position = hit.point;
                    grounded = true;
                    velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
                }
                else
                {
                    position = nextPosition;
                    velocity = nextVelocity;
                }
            }
            else
            {
                position += velocity.normalized * (_shootForce * 0.1f);
            }
        }
    }
}