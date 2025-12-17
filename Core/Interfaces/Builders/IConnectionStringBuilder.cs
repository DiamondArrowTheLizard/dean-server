
using Interfaces.Models;

namespace Interfaces.Builders;

public interface IConnectionStringBuilder 
{

    public void Build();
    public string GetConnectionString();
    public bool ValidateConnection();
}