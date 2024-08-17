using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePause : MonoBehaviour
{
    [SerializeField] AudioSource selectSound;
    // Start is called before the first frame update
    void Start()
    {
        selectSound.ignoreListenerPause = true;
    }

}
