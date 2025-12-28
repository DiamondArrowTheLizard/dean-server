namespace Models.Models.Tables;

public class AcademicTitle(int id) : TableBase(id)
{
    public AcademicTitle() : this(0) { }
    public string TitleName { get; set; } = string.Empty;
}