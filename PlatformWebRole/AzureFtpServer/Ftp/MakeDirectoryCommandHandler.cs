using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class MakeDirectoryCommandHandler : MakeDirectoryCommandHandlerBase
    {
        public MakeDirectoryCommandHandler(FtpConnectionObject connectionObject)
            : base("MKD", connectionObject)
        {
        }
    }
}