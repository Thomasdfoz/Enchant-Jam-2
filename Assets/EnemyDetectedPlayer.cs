using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectedPlayer : MonoBehaviour
{
    Enemy m_Enemy;

    private void Start()
    {
        m_Enemy = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Enemy.state = Enemy.State.follow;
        }
    }
}
