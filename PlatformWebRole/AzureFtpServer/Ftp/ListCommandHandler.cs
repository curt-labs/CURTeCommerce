using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class ListCommandHandler : ListCommandHandlerBase
    {
        public ListCommandHandler(FtpConnectionObject connectionObject)
            : base("LIST", connectionObject)
        {
        }

        protected override string BuildReply(string sMessage, string[] asDirectories, string[] asFiles)
        {
            return BuildLongReply(asDirectories, asFiles);
        }
    }
}