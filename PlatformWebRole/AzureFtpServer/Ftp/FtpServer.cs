using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.General;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Net;

namespace AzureFtpServer.Ftp
{
    /// <summary>
    /// Listens for incoming connections and accepts them.
    /// Incomming socket connections are then passed to the socket handling class (FtpSocketHandler).
    /// </summary>
    public class FtpServer
    {
        #region Member Variables

        private readonly ArrayList m_apConnections;
        private readonly IFileSystemClassFactory m_fileSystemClassFactory;
        private int m_nId;
        private int m_nPort;
        private TcpListener m_socketListen;
        private Thread m_theThread;
        private bool m_started = false;

     #endregion

        #region Events

        #region Delegates

        public delegate void ConnectionHandler(int nId);

        #endregion

        public event ConnectionHandler ConnectionClosed;
        public event ConnectionHandler NewConnection;

        #endregion

        #region Construction

        public FtpServer(IFileSystemClassFactory fileSystemClassFactory)
        {
            m_apConnections = new ArrayList();
            m_fileSystemClassFactory = fileSystemClassFactory;
        }

        ~FtpServer()
        {
            if (m_socketListen != null)
            {
                m_socketListen.Stop();
            }
        }

        public bool Started
        {
            get { return m_started; }
        }

        #endregion

        #region Methods

        public void Start()
        {
            Start(21);
            m_started = true;
        }

        public void Start(int nPort)
        {
            m_nPort = nPort;
            m_theThread = new Thread(ThreadRun);
            m_theThread.Start();
            m_started= true;
        }

        public void Stop()
        {
            for (int nConnection = 0; nConnection < m_apConnections.Count; nConnection++)
            {
                var handler = m_apConnections[nConnection] as FtpSocketHandler;
                handler.Stop();
            }

            m_socketListen.Stop();
            m_theThread.Join();
            m_started = false;
        }

        private void ThreadRun()
        {
            m_socketListen = SocketHelpers.CreateTcpListener(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["FTP"].IPEndpoint);

            if (m_socketListen != null)
            {
                m_socketListen.Start();

                FtpServerMessageHandler.SendMessage(0, "FTP Server Started");

                bool fContinue = true;

                while (fContinue)
                {
                    TcpClient socket = null;

                    try
                    {
                        socket = m_socketListen.AcceptTcpClient();
                    }
                    catch (SocketException)
                    {
                        fContinue = false;
                    }
                    finally
                    {
                        if (socket == null)
                        {
                            fContinue = false;
                        }
                        else
                        {

                            IPEndPoint ipend = (IPEndPoint)socket.Client.RemoteEndPoint;
                            IPAddress ip = ipend.Address;
                            if (FirewallModel.allowConnection(ip.ToString())) {
                                socket.NoDelay = false;

                                m_nId++;

                                FtpServerMessageHandler.SendMessage(m_nId, "New connection");

                                SendAcceptMessage(socket);
                                InitialiseSocketHandler(socket);
                            } else {
                                socket.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                FtpServerMessageHandler.SendMessage(0, "Error in starting FTP server");
            }
        }

        private void SendAcceptMessage(TcpClient socket)
        {
            SocketHelpers.Send(socket, Encoding.ASCII.GetBytes("220 FTP Server Ready\r\n"));
        }

        private void InitialiseSocketHandler(TcpClient socket)
        {
            var handler = new FtpSocketHandler(m_fileSystemClassFactory, m_nId);
            handler.Start(socket);

            m_apConnections.Add(handler);

            handler.Closed += handler_Closed;

            if (NewConnection != null)
            {
                NewConnection(m_nId);
            }
        }

        #endregion

        #region Event Handlers

        private void handler_Closed(FtpSocketHandler handler)
        {
            m_apConnections.Remove(handler);

            if (ConnectionClosed != null)
            {
                ConnectionClosed(handler.Id);
            }
        }

        #endregion
    }
}