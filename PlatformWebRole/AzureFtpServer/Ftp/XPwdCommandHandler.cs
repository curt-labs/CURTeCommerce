using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// Present working directory command handler
    /// </summary>
    internal class XPwdCommandHandler : PwdCommandHandlerBase
    {
        public XPwdCommandHandler(FtpConnectionObject connectionObject)
            : base("XPWD", connectionObject)
        {
        }
    }
}