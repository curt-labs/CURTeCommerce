using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.General;

namespace AzureFtpServer.FtpCommands
{
    internal class AppendCommandHandler : FtpCommandHandler
    {
        private const int m_nBufferSize = 65536;

        public AppendCommandHandler(FtpConnectionObject connectionObject)
            : base("APPE", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            string sFile = GetPath(sMessage);

            IFile file = ConnectionObject.FileSystemObject.OpenFile(sFile, true);

            if (file == null)
            {
                return GetMessage(425, "Couldn't open file");
            }

            var socketReply = new FtpReplySocket(ConnectionObject);

            if (!socketReply.Loaded)
            {
                return GetMessage(425, "Error in establishing data connection.");
            }

            var abData = new byte[m_nBufferSize];

            SocketHelpers.Send(ConnectionObject.Socket, GetMessage(150, "Opening connection for data transfer."));

            int nReceived = socketReply.Receive(abData);

            while (nReceived > 0)
            {
                nReceived = socketReply.Receive(abData);
                file.Write(abData, nReceived);
            }

            file.Close();
            socketReply.Close();

            return GetMessage(226, string.Format("Appended file successfully. ({0})", sFile));
        }
    }
}