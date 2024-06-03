using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour {
    public static ScoreboardUI Instance;

    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI enemyScoreText;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerScoreText.text = "";
        enemyScoreText.text = "";
    }

    public void UpdateScoreboardUI() {
        playerScoreText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
        enemyScoreText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);
    }

    private string PointsByThrowToString(IReadOnlyList<int> pointsByThrow) {
        string toString = "";
        for (var i = 0; i < pointsByThrow.Count; i++) {
            var point = pointsByThrow[i];
            
            if (i % 2 == 1) toString += "/";
            toString += point;
            if (i % 2 == 1) toString += "\t";
        }
        return toString;
    }
}