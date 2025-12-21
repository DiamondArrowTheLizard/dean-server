namespace Models.Models.Tables;

public class Position(int id) : TableBase(id)
{
    public string PositionName { get; set; } = string.Empty;
}