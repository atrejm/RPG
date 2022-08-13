using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float health = 100f;

        private float maxHealth;
        private bool isDead = false;

        private void Update() {
            if(health > 0){return;}

            isDead = true;
            Die();
        }

        private void Start() {
            maxHealth = health;
        }

        public bool IsDead()
        {
            return isDead;
        }
        
        public void TakeDamage(float damage)
        {
            if (isDead) return;
            health = Mathf.Max(health - damage, 0);
            Debug.Log("Health: " + health);                
        }

        private void Die()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}
