using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.Bombs
{
    public class Landmine : MonoBehaviour
    {
        public GameObject explosionPrefab;

        private void OnTriggerEnter(Collider other)
        {
            Explode();
        }

        private void Explode()
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    } 
}
