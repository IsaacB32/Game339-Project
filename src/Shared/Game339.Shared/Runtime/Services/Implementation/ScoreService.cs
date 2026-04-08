using System;

namespace Game339.Shared.Services.Implementation
{
    public class ScoreService: IScoreService
    {
        public ObservableValue<int> DayScore { get; } = new ObservableValue<int>();
        public ObservableValue<int> TotalScore { get; } = new ObservableValue<int>();
        public ObservableValue<Grade> MinigameGrade { get; } = new ObservableValue<Grade>();
        public ObservableValue<int> MinigameScore { get; } = new ObservableValue<int>();

        public string GradeAsString => MinigameGrade.Value switch
        {
            Grade.Perfect => "A+",
            Grade.Good => "B",
            Grade.Bad => "F",
            _ => ""
        };

        public void SetMinigameGrade(float goodThreshold, float perfectThreshold)
        {
            if (goodThreshold >= perfectThreshold)
            {
                MinigameGrade.Value = Grade.Hidden;
                throw new Exception($"goodThreshold is greater than perfectThreshold {goodThreshold} > {perfectThreshold}");
            }
                
            MinigameGrade.Value = Grade.Bad;
            if (MinigameScore.Value >= perfectThreshold) MinigameGrade.Value = Grade.Perfect;
            else if (MinigameScore.Value >= goodThreshold) MinigameGrade.Value = Grade.Good;
        }

        public enum Grade
        {
            Perfect,
            Good,
            Bad,
            Hidden
        }
    }
}