using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinCar : MonoBehaviour
{
    [SerializeField] private Vector3 dir;
    
    private void Update()
    {
        if (PlayerMovement.Instance.gameObject.activeInHierarchy)
        {
            transform.position += dir * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Collider>().tag == "Player")
        {
            var rigidBodies = GetComponentsInChildren<Rigidbody>();
            foreach (var rg in rigidBodies)
            {
                rg.isKinematic = false;
                rg.transform.parent = null;
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.other.tag == "Player")
        {
            var rigidBodies = GetComponentsInChildren<Rigidbody>();
            foreach (var rg in rigidBodies)
            {
                rg.isKinematic = false;
                rg.transform.parent = transform.parent;
                rg.AddForce(col.impulse, ForceMode.Impulse);
            }
            
            Destroy(gameObject);
        }
    }
}
