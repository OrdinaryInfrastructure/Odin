namespace Odin.RemoteFiles;

/// <summary>
/// Connection protocol; SFTP, HTTPS, etc
/// </summary>
public enum ConnectionProtocol
{
    /// <summary>
    /// SSH FTP
    /// </summary>
    Sftp = 0,
    /// <summary>
    /// FTP
    /// </summary>
    Ftp = 1,
    /// <summary>
    /// HTTPS
    /// </summary>
    Https = 2
}