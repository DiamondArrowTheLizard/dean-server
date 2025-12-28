namespace Models.Models.Tables;

public class AcademicDegree(int id) : TableBase(id)
{
    public AcademicDegree() : this(0) { }
    public string DegreeName { get; set; } = string.Empty;
}