using Game339.Shared;
using UnityEngine;

public class ScoreService: IScoreService
{
    public ObservableValue<int> DayScore { get; } = new ObservableValue<int>();
    public ObservableValue<int> TotalScore { get; } = new ObservableValue<int>();

    public string CalculateGrade(float perfectThreshold, float goodThreshold)
    {
        string grade;
        if (DayScore.Value >= perfectThreshold) grade = "PERFECT";
        else if (DayScore.Value >= goodThreshold) grade = "GOOD";
        else grade = "BAD";
        
        return grade;
    }
}

public interface IScoreService
{
    public string CalculateGrade(float perfectThreshold, float goodThreshold);
}
