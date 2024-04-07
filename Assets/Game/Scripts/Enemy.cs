using Horror.Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        patrol,
        follow
    }


    [SerializeField] private Player player;

    [Header("EnemyConfig")]
    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int maxLife;
    [SerializeField] private Transform[] patrolPoints;
    private bool die;
    private int currentLife;
    private Animator animator;
    private NavMeshAgent agent;
    private Coroutine coroutineAttack;
    private Coroutine coroutineTakeDamage;
    private bool inAttack;
    private bool follow;
    public State state;

    private int currentPatrolPoints;
    private bool inPatrol;

    private Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        currentLife = maxLife;
        agent.speed = speed;
        player = FindAnyObjectByType<Player>();
        state = State.patrol;
    }

    void FixedUpdate()
    {
        if (die) return;
        SetAnimations();
    }

    private void LookAt()
    {
        transform.root.LookAt(target.position);
    }

    private void SetAnimations()
    {    
        switch (state)
        {
            case State.patrol:
                Patrol();
                break;
            case State.follow:
                FollowPlayer();
                break;
            default:
                break;
        }
    }

    private void Patrol()
    {
        if (!inPatrol)
        {
            target = patrolPoints[currentPatrolPoints];
            agent.SetDestination(target.position);
            inPatrol = true;
        }
        

        if (agent.remainingDistance <= agent.stoppingDistance)
        {            
            animator.SetBool("walk", false);
            currentPatrolPoints++;

            if (currentPatrolPoints >= patrolPoints.Length)
            {
                currentPatrolPoints = 0;
            }

            inPatrol = false;

        }
        else
        {
            animator.SetBool("walk", true);
        }
    }
    private void FollowPlayer()
    {
        if (!inAttack)
        {
            target = player.transform;

            agent.SetDestination(target.position);
        }

        if (agent.remainingDistance == 0) return;
        if (agent.remainingDistance <= 5)
        {
            agent.speed = Mathf.Clamp(speed / 3f, 1, 10);
        }
        else
        {
            agent.speed = speed;
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            animator.SetBool("walk", false);
            if (!inAttack)
            {
                coroutineAttack = StartCoroutine(AttackCourotine());
            }
            agent.isStopped = false;

        }
        else
        {
            animator.SetBool("walk", true);
        }
    }

    private IEnumerator AttackCourotine()
    {
        inAttack = true;
        if (player.die) yield break;
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.5f);
        player.TakeDamage(damage);
        yield return new WaitForSeconds((1 / attackSpeed));
        inAttack = false;

    }

    public void TakeDamage(int Damage)
    {
        state = State.follow;

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
