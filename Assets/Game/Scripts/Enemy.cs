using Horror.Player;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("EnemyConfig")]
    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int maxLife;
    private bool die;
    private int currentLife;
    private Animator animator;
    private NavMeshAgent agent;
    private Coroutine coroutineAttack;
    private Coroutine coroutineTakeDamage;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        currentLife = maxLife;
        agent.speed = speed;
    }

    void FixedUpdate()
    {
        if (die) return;

        agent.SetDestination(player.transform.position);
        SetAnimations();
    }

    private void SetAnimations()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("walk", false);
            coroutineAttack = StartCoroutine(AttackCourotine());
        }
        else
        {
            StopCoroutine(coroutineAttack);
            animator.SetBool("walk", true);
        }
    }

    private IEnumerator AttackCourotine()
    {
        while (true)
        {
            player.TakeDamage(damage);
            yield return new WaitForSeconds(0.2f);
            animator.SetTrigger("attack");
            yield return new WaitForSeconds((1 / attackSpeed));
        }
    }

    public void TakeDamage(int Damage)
    {
        currentLife -= Damage;
        if (coroutineTakeDamage == null)
        {
            coroutineTakeDamage = StartCoroutine(TakeDamage());
        }
    }

    public IEnumerator TakeDamage()
    {
        if (currentLife <= 0)
        {
            currentLife = 0;
            die = true;
            animator.SetTrigger("die");
            agent.isStopped = true;
            GetComponent<Collider>().enabled = false;

            yield break;

        }

        animator.SetTrigger("takeDamage");
        yield return new WaitForSeconds(0.2f);
        coroutineTakeDamage = null;
    }



}
