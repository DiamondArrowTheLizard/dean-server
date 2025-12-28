namespace Models.Models.Tables;

public class Department(int id) : TableBase(id)
{
    public Department() : this(0) { }
    public string DepartmentName { get; set; } = string.Empty;
    public int IdFaculty { get; set; }
}