namespace Models.Models.Tables;

public class Position(int id) : TableBase(id)
{
    public Position() : this(0) { }
    public string PositionName { get; set; } = string.Empty;
}