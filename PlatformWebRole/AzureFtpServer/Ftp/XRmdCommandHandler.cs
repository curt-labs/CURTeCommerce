using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class XRmdCommandHandler : RemoveDirectoryCommandHandlerBase
    {
        public XRmdCommandHandler(FtpConnectionObject connectionObject)
            : base("XRMD", connectionObject)
        {
        }
    }
}