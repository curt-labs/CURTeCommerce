using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// Present working directory command handler
    /// </summary>
    internal class PwdCommandHandler : PwdCommandHandlerBase
    {
        public PwdCommandHandler(FtpConnectionObject connectionObject)
            : base("PWD", connectionObject)
        {
        }
    }
}