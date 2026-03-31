using System;
using Game.Runtime;
using TMPro;
using UnityEngine;

public class DayScoreView : MonoBehaviour
{
   private static ScoreService scoreService => ServiceResolver.Resolve<ScoreService>();
   private TextMeshProUGUI _scoreText;
   
   private void Awake()
   {
      _scoreText = GetComponentInChildren<TextMeshProUGUI>();
      scoreService.DayScore.ChangeEvent += UpdateScore;
   }

   private void OnDestroy()
   {
      scoreService.DayScore.ChangeEvent -= UpdateScore;
   }

   private void UpdateScore(int score)
   {
      _scoreText.text = score.ToString();
   }
}
