using AzureFtpServer.Ftp;
using AzureFtpServer.FtpCommands;

namespace AzureFtpServer.Ftp
{
    internal class AlloCommandHandler : FtpCommandHandler
    {
        public AlloCommandHandler(FtpConnectionObject connectionObject)
            : base("ALLO", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(202, "Allo processed successfully (depreciated).");
        }
    }
}