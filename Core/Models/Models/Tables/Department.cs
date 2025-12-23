namespace Models.Models.Tables;

public class Department(int id) : TableBase(id)
{
    public string DepartmentName { get; set; } = string.Empty;
    public int IdFaculty { get; set; }
}