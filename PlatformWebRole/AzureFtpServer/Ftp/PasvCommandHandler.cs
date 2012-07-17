using System.Net.Sockets;
using AzureFtpServer.Ftp;
using AzureFtpServer.General;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureFtpServer.FtpCommands
{
    internal class PasvCommandHandler : FtpCommandHandler
    {
        private const int m_nPort = 20;

        public PasvCommandHandler(FtpConnectionObject connectionObject)
            : base("PASV", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            if (ConnectionObject.PasvSocket == null)
            {
                TcpListener listener = SocketHelpers.CreateTcpListener(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["FTP"].IPEndpoint);

                if (listener == null)
                {
                    return GetMessage(550, string.Format("Couldn't start listener on port {0}", m_nPort));
                }

                SendPasvReply();

                listener.Start();

                ConnectionObject.PasvSocket = listener.AcceptTcpClient();

                listener.Stop();
                return "";
            }
            else
            {
                SendPasvReply();
                return "";
            }
        }

        private void SendPasvReply()
        {
            string sIpAddress = SocketHelpers.GetLocalAddress().ToString();
            sIpAddress = sIpAddress.Replace('.', ',');
            sIpAddress += ',';
            sIpAddress += "0";
            sIpAddress += ',';
            sIpAddress += m_nPort.ToString();
            SocketHelpers.Send(ConnectionObject.Socket, string.Format("227 ={0}\r\n", sIpAddress));
        }
    }
}