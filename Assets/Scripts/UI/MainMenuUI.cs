using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject logo;
    void Start()
    {
        logo.transform.DOLocalMoveX(0, 1f).SetEase(Ease.OutBounce);
    }
}
