namespace Models.Models.Tables;

public class FacultyOrder(int id) : TableBase(id)
{
    public DateTime DateOfSign { get; set; }
    public string NewStudentStatusEnum { get; set; } = string.Empty;
    public int IdFaculty { get; set; }
}