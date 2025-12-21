
namespace Models.Models.Tables;

public class Street(int id) : TableBase(id)
{
    public string StreetName { get; set; } = string.Empty;
}