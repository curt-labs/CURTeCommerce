using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class UserCommandHandler : FtpCommandHandler
    {
        public UserCommandHandler(FtpConnectionObject connectionObject)
            : base("USER", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            ConnectionObject.User = sMessage;

            return GetMessage(331, string.Format("User {0} logged in, needs password", sMessage));
        }
    }
}