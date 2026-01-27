using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManScript : MonoBehaviour
{
    [SerializeField] 
    private float _destroyDelay = 0.4f;

    [SerializeField] 
    private float _changeInterval = 3f;

    private Rigidbody _rb;
    private Collider _col;
    private Animator _anim;
    private bool _isHit;

    private static float _timer = 0f;
    private static int _animationState = 0;


    private static float _globalSpeed = 1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _anim = GetComponentInChildren<Animator>();

        if (_anim == null)
            Debug.LogError("Animator is NULL");

        _anim.applyRootMotion = false;
        _anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _changeInterval)
        {
            _timer = 0f;
            _animationState = (_animationState + 1) % 3;

            if (_animationState == 0) _globalSpeed = 0f;
            if (_animationState == 1) _globalSpeed = 1f; 
            if (_animationState == 2) _globalSpeed = 4f; 
        }

        if (!_isHit)
            _anim.SetFloat("Speed", _globalSpeed);
    }

    public void Hit()
    {
        GameManager.Instance.AddScore(50);
        GameManager.Instance.OnManKilled();

        if (_isHit) return;
        _isHit = true;

        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }

        if (_col != null)
            _col.enabled = false;

        _anim.SetFloat("Speed", 0f);
        Destroy(gameObject, _destroyDelay);
    }
}
