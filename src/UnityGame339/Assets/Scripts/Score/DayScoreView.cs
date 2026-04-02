using System;
using Game.Runtime;
using TMPro;
using UnityEngine;

public class DayScoreView : MonoBehaviour
{
   private static ScoreService scoreService => ServiceResolver.Resolve<ScoreService>();
   private TextMeshProUGUI _scoreText;

   [SerializeField] private GameObject _stampObject;
   private TextMeshProUGUI _stampText;
   
   private void Awake()
   {
      _scoreText = GetComponentInChildren<TextMeshProUGUI>();
      _stampText = _stampObject.GetComponentInChildren<TextMeshProUGUI>();
      _stampObject.SetActive(false);
      
      scoreService.DayScore.ChangeEvent += UpdateScore;
      scoreService.MinigameGrade.ChangeEvent += UpdateGrade;
   }

   private void OnDestroy()
   {
      scoreService.DayScore.ChangeEvent -= UpdateScore;
      scoreService.MinigameGrade.ChangeEvent -= UpdateGrade;
   }

   private void UpdateScore(int score)
   {
      _scoreText.text = score.ToString();
   }

   private void UpdateGrade(ScoreService.Grade grade)
   {
      if (grade == ScoreService.Grade.Hidden)
      {
         _stampObject.SetActive(false);
         return;
      }
      
      _stampText.text = scoreService.GradeAsString;
      _stampObject.SetActive(true); //TODO animate in
   }
   
}
