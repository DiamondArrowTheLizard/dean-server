namespace Models.Models.Tables;

public class Teacher(int id) : TableBase(id)
{
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public int IdPosition { get; set; }
    public int IdDepartment { get; set; }
}