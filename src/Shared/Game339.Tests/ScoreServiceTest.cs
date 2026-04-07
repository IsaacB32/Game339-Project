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
    public void SetGrade_ReturnsCorrectValue(int value, float goodThreshold, float perfectThreshold, int expected)
    {
        ScoreService service = new ScoreService();
        service.DayScore.Value = value;

        service.SetMinigameGrade(goodThreshold, perfectThreshold);

        Assert.That((int)service.MinigameGrade.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void SetGrade_ReturnsErrorValue()
    {
        ScoreService service = new ScoreService();
        service.DayScore.Value = 16;

        Assert.Throws<Exception>(() => service.SetMinigameGrade(10, 5));
    }
}