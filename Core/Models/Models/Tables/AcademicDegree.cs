namespace Models.Models.Tables;

public class AcademicDegree(int id) : TableBase(id)
{
    public string DegreeName { get; set; } = string.Empty;
}