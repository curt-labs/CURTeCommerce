using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AzureFtpServer.General
{
    public sealed class SocketHelpers
    {
        private SocketHelpers()
        {
        }

        public static bool Send(TcpClient socket, byte[] abMessage)
        {
            return Send(socket, abMessage, 0, abMessage.Length);
        }

        public static bool Send(TcpClient socket, byte[] abMessage, int nStart)
        {
            return Send(socket, abMessage, nStart, abMessage.Length - nStart);
        }

        public static bool Send(TcpClient socket, byte[] abMessage, int nStart, int nLength)
        {
            bool fReturn = true;

            try
            {
                var writer = new BinaryWriter(socket.GetStream());
                writer.Write(abMessage, nStart, nLength);
                writer.Flush();
            }
            catch (IOException)
            {
                fReturn = false;
            }
            catch (SocketException)
            {
                fReturn = false;
            }

            return fReturn;
        }

        public static bool Send(TcpClient socket, string sMessage)
        {
            byte[] abMessage = Encoding.ASCII.GetBytes(sMessage);
            return Send(socket, abMessage);
        }

        public static void Close(TcpClient socket)
        {
            if (socket == null)
            {
                return;
            }

            try
            {
                if (socket.GetStream() != null)
                {
                    try
                    {
                        socket.GetStream().Flush();
                    }
                    catch (SocketException)
                    {
                    }

                    try
                    {
                        socket.GetStream().Close();
                    }
                    catch (SocketException)
                    {
                    }
                }
            }
            catch (SocketException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                socket.Close();
            }
            catch (SocketException)
            {
            }
        }

        public static TcpClient CreateTcpClient(string sAddress, int nPort)
        {
            TcpClient client = null;

            try
            {
                client = new TcpClient(sAddress, nPort);
            }
            catch (SocketException)
            {
                client = null;
            }

            return client;
        }

        public static TcpListener CreateTcpListener(IPEndPoint endPoint)
        {
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(endPoint);
            }
            catch (SocketException)
            {
                listener = null;
            }

            return listener;
        }

        public static IPAddress GetLocalAddress()
        {
            IPHostEntry hostEntry = Dns.Resolve(Dns.GetHostName());

            if (hostEntry == null || hostEntry.AddressList.Length == 0)
            {
                return null;
            }

            return hostEntry.AddressList[0];
        }
    }
}