using System.Collections.Generic;
using System.IO;

namespace Odin.RemoteFiles;

/// <summary>
/// Abstracts remote file operations typically of FTP, Sftp, etc.
/// </summary>
public interface IRemoteFileSession
{
    /// <summary>
    /// Disconnects if connected.
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Connects to the SFTP server. Throws an Exception if failure occurs...
    /// </summary>
    void Connect();

    /// <summary>
    /// Uploads a file to the target file server\location, establishing a connection first if not connected.
    /// </summary>
    /// <param name="textFileContents"></param>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    void UploadFile(string textFileContents, string fullPath);

    /// <summary>
    /// Downloads a file into a Stream
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="output"></param>
    void DownloadFile(string fileName, in Stream output);

    /// <summary>
    /// Downloads a text file and returns the contents as a string
    /// </summary>
    /// <param name="fileName"></param>
    string DownloadTextFile(string fileName);

    /// <summary>
    /// Changes directory
    /// </summary>
    /// <param name="path"></param>
    void ChangeDirectory(string path);

    /// <summary>
    /// Creates a directory
    /// </summary>
    /// <param name="path"></param>
    void CreateDirectory(string path);


    /// <summary>
    /// Deletes a file
    /// </summary>
    /// <param name="filePath"></param>
    void Delete(string filePath);

    /// <summary>
    /// Returns the files (IRemoteFileInfo) that match the specified search pattern in the specified directory
    /// </summary>
    /// <param name="path">The path to the directory to search under</param>
    /// <param name="searchPattern">Optional search pattern for the file name under the specified path. Supports wildcards (*) and (?).</param>
    /// <returns></returns>
    IEnumerable<IRemoteFileInfo> GetFiles(string path, string searchPattern = null);

    /// <summary>
    /// Checks for file or directory existence. 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool Exists(string path, int? timeoutInSeconds = null);
}