
namespace Models.Models.Tables;

public class Street(int id) : TableBase(id)
{
    public Street() : this(0) { }
    public string StreetName { get; set; } = string.Empty;
}