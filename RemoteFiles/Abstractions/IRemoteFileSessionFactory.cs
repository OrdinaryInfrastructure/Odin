using Odin.System;

namespace Odin.RemoteFiles;

/// <summary>
/// Factory for creating connection providers for remote files
/// </summary>
public interface IRemoteFileSessionFactory
{
    /// <summary>
    /// Create a remote files provider for the connection name configured in application settings
    /// </summary>
    /// <param name="connectionName"></param>
    /// <returns></returns>
    ResultValue<IRemoteFileSession> CreateRemoteFileSession(string connectionName);
}