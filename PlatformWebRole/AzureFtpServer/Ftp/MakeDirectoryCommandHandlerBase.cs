using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class MakeDirectoryCommandHandlerBase : FtpCommandHandler
    {
        protected MakeDirectoryCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            string sFile = GetPath(sMessage);

            if (!ConnectionObject.FileSystemObject.CreateDirectory(sFile))
            {
                return GetMessage(550, string.Format("Couldn't create directory. ({0})", sFile));
            }

            return GetMessage(257, sFile);
        }
    }
}