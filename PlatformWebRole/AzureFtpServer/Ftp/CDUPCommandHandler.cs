using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    internal class CDUPCommandHandler : FtpCommandHandler
    {
        public CDUPCommandHandler(FtpConnectionObject connectionObject)
            : base("CDUP", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Replace('/', '\\');

            if (!FileNameHelpers.IsValid(sMessage))
            {
                return GetMessage(550, "Not a valid directory string.");
            }

            string sDirectory = GetPath(sMessage);

            if (!ConnectionObject.FileSystemObject.DirectoryExists(sDirectory))
            {
                return GetMessage(550, "Not a valid directory.");
            }

            // Process the current path and remove the last child directory
            string current = ConnectionObject.CurrentDirectory.Replace(@"\", @"/");
            if (current == @"/")
                return GetMessage(550, "Not a valid directory.");

            int lastDirectoryDelim = current.LastIndexOf(@"/");
            if (lastDirectoryDelim > -1)
                current = current.Remove(lastDirectoryDelim, current.Length - lastDirectoryDelim);


            ConnectionObject.CurrentDirectory = current;
            return GetMessage(250,
                              string.Format("CWD Successful ({0})", ConnectionObject.CurrentDirectory.Replace("\\", "/")));
        }
    }
}