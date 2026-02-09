using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform _cannonBase;
    [SerializeField] private Transform _smallCannon;
    [SerializeField] private Transform _firePoint;

    [Header("Shot")]
    [SerializeField] private ShotScript _shotPrefab;
    [SerializeField] private float _force = 20f;
    [SerializeField] private float _fireRate = 0.2f;

    [Header("Rotation")]
    [SerializeField] private float _yawSpeed = 60f;
    [SerializeField] private float _pitchSpeed = 40f;
    [SerializeField] private float _minPitch = -80f;
    [SerializeField] private float _maxPitch = 10f;
    [SerializeField] private float _shiftMultiplier = 2f;

    [Header("Trajectory")]
    [SerializeField] private SimulatedScene _simulatedScene;

    private float _yaw;
    private float _pitch = -30f;
    private float _nextFireTime;

    private Vector3 _lastPos;
    private Vector3 _lastDir;
    private float _lastForce;

    private void Update()
    {
        Rotate();
        SimulateIfNeeded();

        if (Input.GetKeyDown(KeyCode.Space))
            Fire();
    }

    private void Rotate()
    {
        float mult = Input.GetKey(KeyCode.LeftShift) ? _shiftMultiplier : 1f;

        _yaw += Input.GetAxis("Horizontal") * _yawSpeed * mult * Time.deltaTime;
        _pitch -= Input.GetAxis("Vertical") * _pitchSpeed * mult * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        _cannonBase.localRotation = Quaternion.Euler(0, _yaw, 0);
        _smallCannon.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }

    private void SimulateIfNeeded()
    {
        if (_simulatedScene == null) return;

        Vector3 pos = _firePoint.position;
        Vector3 dir = _firePoint.forward;

        if (pos == _lastPos && dir == _lastDir && _force == _lastForce)
            return;

        _simulatedScene.SimulateTrajectory(_shotPrefab,pos,dir * _force);

        _lastPos = pos;
        _lastDir = dir;
        _lastForce = _force;
    }

    private void Fire()
    {
        if (Time.time < _nextFireTime) return;
        if (GameManager.Instance.GameOver) return;

        _nextFireTime = Time.time + _fireRate;

        ShotScript shot = Instantiate(_shotPrefab,_firePoint.position,_firePoint.rotation);

        shot.ApplyImpulse(_firePoint.forward * _force);
        GameManager.Instance.OnShotUsed();
    }
}