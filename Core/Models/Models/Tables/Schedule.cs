namespace Models.Models.Tables;

public class Schedule(int id) : TableBase(id)
{
    public int StudyWeeks { get; set; }
    public string DayOfWeekEnum { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string ClassTypeEnum { get; set; } = string.Empty;
    public int IdStudyGroup { get; set; }
    public int IdDiscipline { get; set; }
}