using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameManager.Instance.UnlockTrajectory();

        if (collision.gameObject.TryGetComponent(out ManScript target))
        {
            target.Hit();
        }
    }
}
