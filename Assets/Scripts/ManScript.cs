using System.Collections;
using UnityEngine;

public class ManScript : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 0.4f;
    [SerializeField] private float _moveSpeed = 1.5f;

    private bool _dead;
    private Rigidbody _rb;
    private Collider _col;
    private Animator _anim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _anim = GetComponentInChildren<Animator>();

        if (_anim != null)
            _anim.applyRootMotion = false;
    }

    private void FixedUpdate()
    {
        if (_dead || _rb == null) return;

        Vector3 move = Vector3.forward * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + move);
    }

    private void Update()
    {
        if (_dead || _anim == null) return;

        _anim.SetFloat("Speed", _moveSpeed);
    }

    public void Hit()
    {
        if (_dead) return;
        _dead = true;

        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }

        if (_col != null)
            _col.enabled = false;

        if (_anim != null)
            _anim.SetFloat("Speed", 0f);

        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(_destroyDelay);

        GameManager.Instance?.OnManDestroyed(this);
        Destroy(gameObject);
    }
}