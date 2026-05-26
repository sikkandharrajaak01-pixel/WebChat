using System.Collections.Concurrent;

public static class ConnectionManager
{
    public static ConcurrentDictionary<int, string> UserConnections = new ConcurrentDictionary<int, string>();
}
