using Game339.Shared.Services.Implementation;
namespace Game339.Tests;

public class ScoreServiceTest
{
    /*
     * 0 = Perfect
     * 1 = Good
     * 2 = Bad
     * 3 = Hidden
     */
    
    [TestCase(1, 5, 10, 2)]
    [TestCase(4, 5, 10, 2)]
    [TestCase(8, 5, 10, 1)]
    [TestCase(11, 5, 10, 0)]
    // [TestCase(16, 10, 5, 3)]
    public void SetGrade_ReturnsCorrectValue(int value, float goodThreshold, float perfectThreshold, int expected)
    {
        ScoreService service = new ScoreService();
        service.DayScore.Value = value;

        service.SetMinigameGrade(goodThreshold, perfectThreshold);

        Assert.That((int)service.MinigameGrade.Value, Is.EqualTo(expected));
    }
    
    //how can I make the scripts be the same across the shared projects?
    //how can I throw an error but still have the test pass, warning?
    //how do I double-check that my tests actually ran?
}