
namespace Game339.Shared.Services.Implementation
{
    public class ScoreService: IScoreService
    {
        public ObservableValue<int> DayScore { get; } = new ObservableValue<int>();
        public ObservableValue<int> TotalScore { get; } = new ObservableValue<int>();
        public ObservableValue<Grade> MinigameGrade { get; } = new ObservableValue<Grade>();

        public string GradeAsString => MinigameGrade.Value switch
        {
            Grade.Perfect => "A+",
            Grade.Good => "B",
            Grade.Bad => "F",
            _ => ""
        };

        public void SetMinigameGrade(float goodThreshold, float perfectThreshold)
        {
            MinigameGrade.Value = Grade.Bad;
            if (DayScore.Value >= goodThreshold) MinigameGrade.Value = Grade.Good;
            else if (DayScore.Value >= perfectThreshold) MinigameGrade.Value = Grade.Perfect;
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