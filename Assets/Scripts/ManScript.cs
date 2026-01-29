using UnityEngine;

public class ManScript : MonoBehaviour
{
    [SerializeField] float _destroyDelay = 0.4f;

    Rigidbody _rb;
    Collider _col;
    Animator _anim;
    bool _isHit;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _anim = GetComponentInChildren<Animator>();
    }

    public void Hit()
    {
        if (_isHit) return; // ? MUST BE FIRST
        _isHit = true;

        GameManager.Instance.AddScore(50);
        GameManager.Instance.OnManKilled();

        _rb.isKinematic = true;
        _col.enabled = false;
        _anim.SetFloat("Speed", 0f);

        Destroy(gameObject, _destroyDelay);
    }
}