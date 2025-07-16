using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public Transform player;
    public float followRange = 50f;
    public float attackRange = 2f;
    public float memoryTime = 2f;
    public float attackCooldown = 1.5f;

    private float memoryTimer = 0f;
    private float attackTimer = 0f;
    private NavMeshAgent agent;

    private Animator animator;
    private PlayerStamina playerStamina;

    private bool isAttacking = false;

    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStamina = PlayerStamina.Instance;
        }
    }

    void Update()
    {
        if (player == null || agent == null) return;

        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
        Vector3 directionToPlayer = (player.position + Vector3.up * 1f) - rayOrigin;
        float distance = directionToPlayer.magnitude;

        if (distance <= followRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, directionToPlayer.normalized, out hit, followRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    memoryTimer = memoryTime;
                }
            }
        }

        if (memoryTimer > 0f)
        {
            memoryTimer -= Time.deltaTime;

            if (!isAttacking && distance <= attackRange)
            {
                agent.ResetPath();
                animator.SetBool("isWalk", false);

                StartCoroutine(AttackRoutine());
            }
            else
            {
                if (agent.enabled)
                {
                    agent.SetDestination(player.position);
                }
                animator.SetBool("isWalk", true);
            }
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalk", false);
        }
    }

    private IEnumerator AttackRoutine()
    {
        animator.SetTrigger("isAttack");
        PlaySound(attackSound);

        isAttacking = true;
        animator.ResetTrigger("isAttack");
        animator.SetTrigger("isAttack");

        yield return new WaitForSeconds(1.5f);

        if (playerStamina != null && memoryTimer > 0f)
        {
            playerStamina.TakeDamage(10);
            Debug.Log("[ZombieAI] 타격 시점 데미지 1회 발생");
        }

        yield return new WaitForSeconds(2.333f);
        isAttacking = false;
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }


    public void TakeDamage(int damage)
    {
        PlaySound(hurtSound);
        GetComponent<EnemyStamina>().TakeDamage(damage);
    }
}
