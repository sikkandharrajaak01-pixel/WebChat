using System.Collections.Concurrent;
public static class ConnectionManager
{
    private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _connections = new();
    public static void AddConnection(int userId, string connectionId)
    {
        var bag = _connections.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
        bag.TryAdd(connectionId, 0);
    }
    public static int? RemoveConnection(string connectionId)
    {
        foreach (var kvp in _connections)
        {
            if (kvp.Value.TryRemove(connectionId, out _))
            {
                if (kvp.Value.IsEmpty)
                    _connections.TryRemove(kvp.Key, out _);
                return kvp.Key;
            }
        }
        return null;
    }
    public static IEnumerable<string> GetConnections(int userId)
    {
        if (_connections.TryGetValue(userId, out var bag))
            return bag.Keys.ToList();
        return Enumerable.Empty<string>();
    }
    public static bool IsOnline(int userId)
    {
        return _connections.TryGetValue(userId, out var bag) && !bag.IsEmpty;
    }
}