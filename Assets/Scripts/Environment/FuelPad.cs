using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPad : MonoBehaviour
{
    private PlayerMovement _playerMove;
    [Range(0.01f, 1f)]
    [SerializeField] private float fuelAdd = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        _playerMove = PlayerMovement.Instance;
    }

    // Update is called once per frame
    void OnTriggerEnter()
    {
        _playerMove.RestoreFuel( fuelAdd );
    }
}
