using System.Collections;
using System.Collections.Generic;
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
    private float _canFire = -1f;

    [SerializeField]
    private int _maxShots = 20;
    private int _currentShots;

    [SerializeField] 
    private LineRenderer _lineRenderer;
    [SerializeField] 
    private int _linePoints = 30;
    [SerializeField] 
    private float _timeBetweenPoints = 0.1f;

    [SerializeField] 
    private float _yawSpeed;
    [SerializeField] 
    private float _pitchSpeed;

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

    private float _currentYaw;
    private float _currentPitch;

    void Start()
    {
        _currentShots = _maxShots;
        UIManager.Instance.UpdateShots(_currentShots);

        _currentYaw = _cannonBase.localEulerAngles.y;
        if (_currentYaw > 180f)
            _currentYaw -= 360f;

        _currentPitch = _smallCannon.localEulerAngles.x;
        if (_currentPitch > 180f)
            _currentPitch -= 360f;
    }

    void Update()
    {
        float speedMultiplier = 1f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedMultiplier = _shiftMultiplier;
        }

        HandleYaw(speedMultiplier);
        HandlePitch(speedMultiplier);
        DrawTrajectory();

        Debug.Log($"H={Input.GetAxis("Horizontal")} V={Input.GetAxis("Vertical")}");

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _currentShots > 0)
        {
            Shoot();
        }
    }


    private void HandleYaw(float multiplier)
    {
        float h = Input.GetAxis("Horizontal");

        _currentYaw += h * _yawSpeed * multiplier * Time.deltaTime;
        _currentYaw = Mathf.Clamp(_currentYaw, _minYaw, _maxYaw);

        _cannonBase.localRotation = Quaternion.Euler(0f, _currentYaw, 0f);
    }


    private void HandlePitch(float multiplier)
    {
        float v = Input.GetAxis("Vertical");

        _currentPitch -= v * _pitchSpeed * multiplier * Time.deltaTime;
        _currentPitch = Mathf.Clamp(_currentPitch, _minPitch, _maxPitch);

        _smallCannon.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);
    }

    private void Shoot()
    {
        _currentShots--;
        UIManager.Instance.UpdateShots(_currentShots);
        _canFire = Time.time + _fireRate;

        GameObject shot = Instantiate(_shotPrefab,_firePoint.position,_firePoint.rotation);

        Rigidbody rb = shot.GetComponent<Rigidbody>();
        rb.AddForce(_firePoint.forward * _shootForce, ForceMode.Impulse);

        if (_currentShots <= 0)
        {
            GameManager.Instance.OnShotsEmpty();
        }
    }


    private void DrawTrajectory()
    {
        Vector3 startPosition = _firePoint.position;
        Vector3 startVelocity = _firePoint.forward * _shootForce;

        _lineRenderer.positionCount = _linePoints;

        for (int i = 0; i < _linePoints; i++)
        {
            float time = i * _timeBetweenPoints;

            Vector3 point = startPosition + startVelocity * time + 0.5f * Physics.gravity * time * time;

            _lineRenderer.SetPosition(i, point);
        }
    }
}