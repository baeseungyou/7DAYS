using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    private Weapon weapon;

    void Start()
    {
        weapon = GetComponent<Weapon>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStamina enemy = other.GetComponent<EnemyStamina>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)weapon.damage);
                Debug.Log($"[WeaponHit] {other.name}에게 {weapon.damage} 데미지!");
            }
        }
    }
}
