namespace Models.Models.Tables;

public class Performance(int id) : TableBase(id)
{
    public string MarkTypeEnum { get; set; } = string.Empty;
    public int Mark { get; set; }
    public int IdTeacher { get; set; }
    public int IdStudent { get; set; }
}