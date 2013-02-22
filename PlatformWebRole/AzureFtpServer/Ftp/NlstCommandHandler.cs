using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class NlstCommandHandler : ListCommandHandlerBase
    {
        public NlstCommandHandler(FtpConnectionObject connectionObject)
            : base("NLST", connectionObject)
        {
        }

        protected override string BuildReply(string sMessage, string[] asDirectories, string[] asFiles)
        {
            if (sMessage == "-L" || sMessage == "-l")
            {
                return BuildLongReply(asDirectories, asFiles);
            }
            else
            {
                return BuildShortReply(asDirectories, asFiles);
            }
        }
    }
}