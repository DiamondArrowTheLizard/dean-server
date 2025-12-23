
using Interfaces.Models.Tables;

namespace Models.Models.Tables;

public abstract class TableBase(int id) : ITable
{
    public int Id { get; set; } = id;
}