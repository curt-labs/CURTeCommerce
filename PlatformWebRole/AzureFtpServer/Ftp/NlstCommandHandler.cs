using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class NlstCommandHandler : ListCommandHandlerBase
    {
        public NlstCommandHandler(FtpConnectionObject connectionObject)
            : base("NLST", connectionObject)
        {
        }

        protected override string BuildReply(string sMessage, string[] asFiles)
        {
            if (sMessage == "-L" || sMessage == "-l")
            {
                return BuildLongReply(asFiles);
            }
            else
            {
                return BuildShortReply(asFiles);
            }
        }
    }
}