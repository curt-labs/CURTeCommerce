using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.General;

namespace AzureFtpServer.Ftp
{
    /// <summary>
    /// Contains the socket read functionality. Works on its own thread since all socket operation is blocking.
    /// </summary>
    internal class FtpSocketHandler
    {
        #region Member Variables

        private const int m_nBufferSize = 65536;
        private readonly IFileSystemClassFactory m_fileSystemClassFactory;
        private readonly int m_nId;
        private FtpConnectionObject m_theCommands;
        private TcpClient m_theSocket;
        private Thread m_theThread;

        #endregion

        #region Events

        #region Delegates

        public delegate void CloseHandler(FtpSocketHandler handler);

        #endregion

        public event CloseHandler Closed;

        #endregion

        #region Construction

        public FtpSocketHandler(IFileSystemClassFactory fileSystemClassFactory, int nId)
        {
            m_nId = nId;
            m_fileSystemClassFactory = fileSystemClassFactory;
        }

        #endregion

        #region Methods

        public void Start(TcpClient socket)
        {
            m_theSocket = socket;

            m_theCommands = new FtpConnectionObject(m_fileSystemClassFactory, m_nId, socket);
            m_theThread = new Thread(ThreadRun);
            m_theThread.Start();
        }

        public void Stop()
        {
            SocketHelpers.Close(m_theSocket);
            m_theThread.Join();
        }

        private void ThreadRun()
        {
            var abData = new Byte[m_nBufferSize];

            try
            {
                int nReceived = m_theSocket.GetStream().Read(abData, 0, m_nBufferSize);

                while (nReceived > 0)
                {
                    m_theCommands.Process(abData);

                    nReceived = m_theSocket.GetStream().Read(abData, 0, m_nBufferSize);
                }
            }
            catch (SocketException)
            {
            }
            catch (IOException)
            {
            }

            FtpServerMessageHandler.SendMessage(m_nId, "Connection closed");

            if (Closed != null)
            {
                Closed(this);
            }

            m_theSocket.Close();
        }

        #endregion

        #region Properties

        public int Id
        {
            get { return m_nId; }
        }

        #endregion
    }
}