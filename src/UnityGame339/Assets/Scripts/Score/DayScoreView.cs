using System;
using Game.Runtime;
using TMPro;
using UnityEngine;
using Game339.Shared.Services.Implementation;


public class DayScoreView : MonoBehaviour
{
   private static readonly int Start = Animator.StringToHash("start");
   
   private static ScoreService scoreService => ServiceResolver.Resolve<ScoreService>();
   private TextMeshProUGUI _scoreText;

   [SerializeField] private GameObject _stampObject;
   private Animator _stampAnimator;
   private TextMeshProUGUI _stampText;
   
   private void Awake()
   {
      _scoreText = GetComponentInChildren<TextMeshProUGUI>();
      _stampText = _stampObject.GetComponentInChildren<TextMeshProUGUI>();
      _stampAnimator = _stampObject.GetComponent<Animator>();
      _stampObject.SetActive(false);
    
      scoreService.MinigameScore.ChangeEvent += UpdateScore;
      scoreService.MinigameGrade.ChangeEvent += UpdateGrade;
   }

   private void OnDestroy()
   {
      scoreService.MinigameScore.ChangeEvent -= UpdateScore;
      scoreService.MinigameGrade.ChangeEvent -= UpdateGrade;
   }

   private void UpdateScore(int score)
   {
      _scoreText.text = "Score: " + score;
   }

   private void UpdateGrade(ScoreService.Grade grade)
   {
      if (grade == ScoreService.Grade.Hidden)
      {
         _stampObject.SetActive(false);
         return;
      }
      
      _stampText.text = scoreService.GradeAsString;
      _stampObject.SetActive(true); 
      _stampAnimator.SetTrigger(Start);
   }
   
}
