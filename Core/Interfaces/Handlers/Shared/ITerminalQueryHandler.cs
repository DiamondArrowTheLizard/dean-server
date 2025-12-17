
namespace Interfaces.Handlers.Shared;

public interface ITerminalQueryHandler
{
    public bool HandleTerminalQuery(string queryString, out string outputString);
}