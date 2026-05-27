using System.Collections.Concurrent;

public static class ConnectionManager
{
    private static ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _userConnections = new();
    private static ConcurrentDictionary<string, int> _connectionToUser = new();

    public static bool IsOnline(int userId)
    {
        return _userConnections.TryGetValue(userId, out var connections) && !connections.IsEmpty;
    }

    public static void AddConnection(int userId, string connectionId)
    {
        var connections = _userConnections.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
        connections.TryAdd(connectionId, 0);
        _connectionToUser[connectionId] = userId;
    }

    public static IEnumerable<string> GetConnections(int userId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
            return connections.Keys.ToList();
        return Enumerable.Empty<string>();
    }

    public static int? RemoveConnection(string connectionId)
    {
        if (_connectionToUser.TryRemove(connectionId, out var userId))
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connections.TryRemove(connectionId, out _);
                if (connections.IsEmpty)
                    _userConnections.TryRemove(userId, out _);
            }
            return userId;
        }
        return null;
    }
}
