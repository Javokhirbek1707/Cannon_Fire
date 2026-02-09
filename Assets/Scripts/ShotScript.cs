using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 6f;
    private Rigidbody _rb;

    private const string SimulatedSceneName = "SimulatedPhysicsScene";

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, _lifeTime);
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        if (_rb == null || _rb.isKinematic)
            return;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.AddForce(impulse, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //  Ignore ALL collisions in simulated scene
        if (gameObject.scene.name == SimulatedSceneName)
            return;

        if (collision.gameObject.TryGetComponent(out ManScript man))
        {
            man.Hit();
        }

        Destroy(gameObject);
    }
}