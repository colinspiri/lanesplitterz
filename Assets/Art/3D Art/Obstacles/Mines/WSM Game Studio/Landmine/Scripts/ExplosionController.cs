using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.Bombs
{
    public class ExplosionController : MonoBehaviour
    {
        public float power = 10f;
        public float radius = 2f;
        public float upForce = 1f;
        public int damage = 10;

        private void Start()
        {
            Explode();
            Destroy(gameObject, 1.5f);
        }

        // Update is called once per frame
        void Explode()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(power, transform.position, radius, upForce, ForceMode.Impulse);

                    //Handle Explosion Damage here
                    //Sample
                    //Health healthScript = hit.gameObject.GetComponent<Health>();

                    //if (healthScript != null)
                    //    healthScript.TakeDamage(damage);
                }
            }
        }
    }
}
