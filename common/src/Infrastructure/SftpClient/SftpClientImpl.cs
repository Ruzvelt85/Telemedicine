using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Telemedicine.Common.Infrastructure.SftpClient.Exceptions;
using Microsoft.Extensions.Options;
using Renci.SshNet.Sftp;
using Serilog;
using ExternalSftpClient = Renci.SshNet.SftpClient;

namespace Telemedicine.Common.Infrastructure.SftpClient
{
    /// <summary>
    /// Implementation of the SFTP client.
    /// </summary>
    internal class SftpClientImpl : ISftpClient
    {
        private bool _disposed;
        private readonly ILogger _logger = Log.ForContext<SftpClientImpl>();
        private readonly ExternalSftpClient _externalSftpClient;
        private bool IsConnected => _externalSftpClient.IsConnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SftpClientImpl" /> class.
        /// </summary>
        /// <param name="sftpClientFactory">Builder for external SFTP client <see cref="IExternalSftpClientFactory"/></param>
        /// <param name="sftpClientOptionsMonitor">Options monitor for SFTP client settings <see cref="SftpClientSettings"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SftpClientImpl(IExternalSftpClientFactory sftpClientFactory, IOptionsMonitor<SftpClientSettings> sftpClientOptionsMonitor)
        {
            if (sftpClientFactory is null)
            {
                throw new ArgumentNullException(nameof(sftpClientFactory));
            }
            if (sftpClientOptionsMonitor is null)
            {
                throw new ArgumentNullException(nameof(sftpClientOptionsMonitor));
            }

            _logger.Debug("Creating SFTP client.");
            _externalSftpClient = sftpClientFactory.Create(sftpClientOptionsMonitor.CurrentValue);
        }

        /// <inheritdoc cref="ISftpClient.ListDirectory"/>
        public IEnumerable<string> ListDirectory(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path), "Value cannot be null.");
            }

            _logger.Debug("Retrieving list of files in remote directory. Path: '{Path}'", path);
            Connect();

            IEnumerable<SftpFile> sftpFiles;
            try
            {
                sftpFiles = _externalSftpClient.ListDirectory(path);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Retrieving list of files in remote directory. Path: '{Path}'", path);
                Debug.Assert(false, $"Retrieving list of files in remote directory. Path: '{path}'");
                throw new ListDirectorySftpClientException(ex);
            }

            foreach (var file in sftpFiles)
            {
                yield return file.FullName;
            }
        }

        /// <inheritdoc cref="ISftpClient.OpenRead"/>
        public StreamReader OpenRead(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            _logger.Debug("Opening an existing file for reading. Path: '{Path}'", path);
            Connect();

            try
            {
                return new StreamReader(_externalSftpClient.OpenRead(path), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to open existing file for reading. Path: '{Path}'", path);
                Debug.Assert(false, $"Failed to open existing file for reading. Path: '{path}'");
                throw new OpenReadSftpClientException(ex);
            }
        }

        /// <inheritdoc cref="ISftpClient.TryConnect"/>
        public bool TryConnect()
        {
            _logger.Debug("Attempting to connect to the server via SFTP.");

            if (_disposed)
            {
                _logger.Warning("SFTP client has already been disposed.");
                return false;
            }

            try
            {
                Connect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            DisposeExternalSftpClient();

            void DisposeExternalSftpClient()
            {
                try
                {
                    _externalSftpClient.Dispose();
                    _logger.Debug("Successfully disposed external SFTP client.");
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "An exception occurred while disposing external SFTP client. Message: {ErrorMessage}", ex.Message);
                }
            }
        }

        protected void Connect()
        {
            _logger.Debug("Connecting to the server via SFTP.");
            CheckDisposed();

            if (IsConnected)
            {
                _logger.Debug("Connection to the server via SFTP was established earlier.");
                return;
            }

            try
            {
                _externalSftpClient.Connect();
                _logger.Debug("Successfully connected to the server via SFTP.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred while connecting to SFTP server. Message: {ErrorMessage}", ex.Message);
                Debug.Assert(false, "An exception occurred while connecting to SFTP server.");
                throw new ConnectSftpClientException(ex);
            }
        }

        protected void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            var exception = new ObjectDisposedException(GetType().FullName);
            Log.Error(exception, "SFTP client has already been disposed.");
            throw exception;
        }
    }
}
