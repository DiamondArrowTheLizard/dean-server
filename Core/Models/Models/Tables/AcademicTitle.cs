namespace Models.Models.Tables;

public class AcademicTitle(int id) : TableBase(id)
{
    public string TitleName { get; set; } = string.Empty;
}