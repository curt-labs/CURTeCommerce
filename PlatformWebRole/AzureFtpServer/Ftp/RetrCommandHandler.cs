using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// Implements the RETR command
    /// </summary>
    internal class RetrCommandHandler : FtpCommandHandler
    {
        public RetrCommandHandler(FtpConnectionObject connectionObject)
            : base("RETR", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            string sFilePath = GetPath(sMessage);

            if (!ConnectionObject.FileSystemObject.FileExists(sFilePath))
            {
                return GetMessage(550, "File doesn't exist");
            }

            var replySocket = new FtpReplySocket(ConnectionObject);

            if (!replySocket.Loaded)
            {
                return GetMessage(550, "Unable to establish data connection");
            }

            SocketHelpers.Send(ConnectionObject.Socket, "150 Please Wait. Starting data transfer of " + sMessage + "\r\n");

            const int m_nBufferSize = 65536;

            IFile file = ConnectionObject.FileSystemObject.OpenFile(sFilePath, false);

            if (file == null || file.File == null)
            {
                return GetMessage(550, "Couldn't open file");
            }

            var abBuffer = new byte[m_nBufferSize];

            int nRead = file.Read(abBuffer, m_nBufferSize);

            while (nRead > 0 && replySocket.Send(abBuffer, nRead))
            {
                nRead = file.Read(abBuffer, m_nBufferSize);
            }

            file.Close();
            replySocket.Close();

            return GetMessage(226, "File download succeeded.");
        }
    }
}