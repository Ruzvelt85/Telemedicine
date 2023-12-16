using System;
using System.Collections.Generic;
using System.IO;

namespace Telemedicine.Common.Infrastructure.SftpClient
{
    /// <summary>
    /// Implementation of the SFTP client.
    /// </summary>
    public interface ISftpClient : IDisposable
    {
        /// <summary>
        /// Retrieves list of files in remote directory.
        /// </summary>
        /// <param name="path">Path to a remote directory.</param>
        /// <returns>A list of files full names.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="path" /> is <b>null</b> or <b>white space</b>.</exception>
        /// <exception cref="Exceptions.ListDirectorySftpClientException">Client can't list directory.</exception>
        /// <exception cref="System.ObjectDisposedException">The method was called after the client was disposed.</exception>
        IEnumerable<string> ListDirectory(string path);

        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        /// <param name="path">The file to be opened for reading.</param>
        /// <returns>A read-only <see cref="StreamReader" /> on the specified path.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="path" /> is <b>null</b> or <b>white space</b>.</exception>
        /// <exception cref="Exceptions.OpenReadSftpClientException">Client can't open file.</exception>
        /// <exception cref="System.ObjectDisposedException">The method was called after the client was disposed.</exception>
        StreamReader OpenRead(string path);

        /// <summary>
        /// Checks if a connection to the server via SFTP has been established earlier.
        /// If not, it establishes connections.
        /// </summary>
        /// <returns>
        /// <c>false</c> if the connection to the server via SFTP was established earlier; otherwise, <c>true</c>.
        /// </returns>
        /// <exception cref="Exceptions.ConnectSftpClientException">Client can't open file.</exception>
        /// <exception cref="System.ObjectDisposedException">The method was called after the client was disposed.</exception>
        bool TryConnect();
    }
}
