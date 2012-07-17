using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class XMkdCommandHandler : MakeDirectoryCommandHandlerBase
    {
        public XMkdCommandHandler(FtpConnectionObject connectionObject)
            : base("XMKD", connectionObject)
        {
        }
    }
}