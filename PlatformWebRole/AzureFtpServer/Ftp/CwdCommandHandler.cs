using System.IO;
using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    internal class CwdCommandHandler : FtpCommandHandler
    {
        public CwdCommandHandler(FtpConnectionObject connectionObject)
            : base("CWD", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Replace('/', '\\');

            if (!FileNameHelpers.IsValid(sMessage))
            {
                return GetMessage(550, "Not a valid directory string.");
            }

            string sDirectory = GetPath(sMessage);

            if (!ConnectionObject.FileSystemObject.DirectoryExists(sDirectory))
            {
                return GetMessage(550, "Not a valid directory.");
            }

            string newPath = Path.Combine(ConnectionObject.CurrentDirectory, sMessage);
            ConnectionObject.CurrentDirectory = newPath;
            return GetMessage(250,
                              string.Format("CWD Successful ({0})", ConnectionObject.CurrentDirectory.Replace("\\", "/")));
        }
    }
}