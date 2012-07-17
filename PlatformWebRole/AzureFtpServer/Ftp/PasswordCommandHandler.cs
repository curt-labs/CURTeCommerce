using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class PasswordCommandHandler : FtpCommandHandler
    {
        public PasswordCommandHandler(FtpConnectionObject connectionObject)
            : base("PASS", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            if (ConnectionObject.Login(sMessage))
            {
                return GetMessage(220, "Password ok, FTP server ready");
            }
            else
            {
                return GetMessage(530, "Username or password incorrect");
            }
        }
    }
}