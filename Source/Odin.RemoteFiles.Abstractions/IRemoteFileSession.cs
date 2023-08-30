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
    /// Lists files in a directory
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerable<IRemoteFileInfo> ListDirectory(string path);

    /// <summary>
    /// Checks for file or directory existence. Path can contain * and ? wildcards for pattern matching.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool Exists(string path);
}