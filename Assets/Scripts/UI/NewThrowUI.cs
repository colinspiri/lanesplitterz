using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ScriptableObjectArchitecture;

public class NewThrowUI : MonoBehaviour
{
    [SerializeField] private GameObject roundUI;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private float roundUITime;
    [SerializeField] private GameEvent roundUIEnd;
    private int currentRound = 0;

    // Start is called before the first frame update
    void Start()
    {
        RoundManager.OnNewRound += () => StartCoroutine(NextRoundUI());
        StartCoroutine(NextRoundUI());
    }

    private IEnumerator NextRoundUI()
    {
        currentRound++;

        if (currentRound <= 5)
        {
            roundText.text = "Round " + currentRound;
            roundUI.SetActive(true);
            yield return new WaitForSeconds(roundUITime);
            roundUI.SetActive(false);

            if (currentRound == 1) roundUIEnd.Raise(); // should start dialogue
        }
    }
}
