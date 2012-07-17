using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class RemoveDirectoryCommandHandler : RemoveDirectoryCommandHandlerBase
    {
        public RemoveDirectoryCommandHandler(FtpConnectionObject connectionObject)
            : base("RMD", connectionObject)
        {
        }
    }
}