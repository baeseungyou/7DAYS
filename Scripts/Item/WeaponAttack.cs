using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private Weapon weapon;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        weapon = GetComponent<Weapon>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Weapon weapon = GetComponent<Weapon>();
            if (weapon == null) return; 

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

/*
            if (weapon.attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(weapon.attackSound);
            }
            */
        }
    }
}
