namespace Models.Models.Tables;

public class ClassroomSchedule(int id) : TableBase(id)
{
    public int IdClassroom { get; set; }
    public int IdSchedule { get; set; }
}