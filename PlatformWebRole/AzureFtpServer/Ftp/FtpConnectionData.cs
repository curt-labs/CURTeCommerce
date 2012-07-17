using System.Net.Sockets;
using AzureFtpServer.Ftp.FileSystem;

namespace AzureFtpServer.Ftp
{
    /// <summary>
    /// Contains all the data relating to a particular FTP connection
    /// </summary>
    internal class FtpConnectionData
    {
        #region Member Variables

        private readonly int m_nId;
        private readonly TcpClient m_theSocket;
        private IFileSystem m_fileSystem;
        private int m_nPortCommandSocketPort = 20;
        private string m_sCurrentDirectory = "\\";
        private string m_sPortCommandSocketAddress = "";

        #endregion

        #region Construction

        public FtpConnectionData(int nId, TcpClient socket)
        {
            m_nId = nId;
            m_theSocket = socket;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Main connection socket
        /// </summary>
        public TcpClient Socket
        {
            get { return m_theSocket; }
        }

        public string User { get; set; }

        /// <summary>
        /// This connection's current directory
        /// </summary>
        public string CurrentDirectory
        {
            get { return m_sCurrentDirectory; }

            set { m_sCurrentDirectory = value; }
        }

        /// <summary>
        /// This connection's Id
        /// </summary>
        public int Id
        {
            get { return m_nId; }
        }

        /// <summary>
        /// Socket address from PORT command.
        /// See FtpReplySocket class.
        /// </summary>
        public string PortCommandSocketAddress
        {
            get { return m_sPortCommandSocketAddress; }

            set { m_sPortCommandSocketAddress = value; }
        }

        /// <summary>
        /// Port from PORT command.
        /// See FtpReplySocket class.
        /// </summary>
        public int PortCommandSocketPort
        {
            get { return m_nPortCommandSocketPort; }

            set { m_nPortCommandSocketPort = value; }
        }

        /// <summary>
        /// Whether the connection is in binary or ASCII transfer mode.
        /// </summary>
        public bool BinaryMode { get; set; }

        /// <summary>
        /// If this is non-null the last command was a PASV and should therefore use this socket.
        /// If this is null the last command was a PORT command and should therefore use that mechanism instead.
        /// </summary>
        public TcpClient PasvSocket { get; set; }

        /// <summary>
        /// Rename takes place with 2 commands - we store the old name here
        /// </summary>
        public string FileToRename { get; set; }

        /// <summary>
        /// This is true if the file to rename is a directory
        /// </summary>
        public bool RenameDirectory { get; set; }

        public IFileSystem FileSystemObject
        {
            get { return m_fileSystem; }
        }

        protected void SetFileSystemObject(IFileSystem fileSystem)
        {
            m_fileSystem = fileSystem;
        }

        #endregion
    }
}