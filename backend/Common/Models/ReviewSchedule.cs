namespace Common.Models;

public class ReviewSchedule
{
    public int IntervalDays { get; set; } = 1;
    public double EaseFactor { get; set; } = 2.5;
    public int ReviewCount { get; set; } = 0;
}