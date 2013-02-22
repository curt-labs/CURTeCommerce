using System;
using System.IO;
using System.Text;
using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Ftp.General;
using AzureFtpServer.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// Base class for list commands
    /// </summary>
    internal abstract class ListCommandHandlerBase : FtpCommandHandler
    {
        public ListCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            SocketHelpers.Send(ConnectionObject.Socket, "150 Opening data connection for LIST\r\n");

            string[] asFiles = null;
            string[] asDirectories = null;

            sMessage = sMessage.Trim();

            string sPath = GetPath("");

            if (sMessage.Length == 0 || sMessage[0] == '-')
            {
                asFiles = ConnectionObject.FileSystemObject.GetFiles(sPath);
                asDirectories = ConnectionObject.FileSystemObject.GetDirectories(sPath);
            }
            else
            {
                asFiles = ConnectionObject.FileSystemObject.GetFiles(sPath, sMessage);
                asDirectories = ConnectionObject.FileSystemObject.GetDirectories(sPath, sMessage);
            }

            string sFileList = BuildReply(sMessage, asDirectories, asFiles);

            var socketReply = new FtpReplySocket(ConnectionObject);

            if (!socketReply.Loaded)
            {
                return GetMessage(550, "LIST unable to establish return connection.");
            }

            socketReply.Send(sFileList);
            socketReply.Close();

            return GetMessage(226, "LIST successful.");
        }

        protected abstract string BuildReply(string sMessage, string[] asDirectories, string[] asFiles);

        protected string BuildShortReply(string[] asDirectories, string[] asFiles)
        {
            /*string sFileList = string.Join("\r\n", asDirectories);*/
            string sFileList = string.Join("\r\n", asFiles);
            sFileList += "\r\n";
            return sFileList;
        }

        protected string BuildLongReply(string[] asDirectories, string[] asFiles)
        {
            string sDirectory = GetPath("");

            var stringBuilder = new StringBuilder();

            for (int nIndex = 0; nIndex < asDirectories.Length; nIndex++) {
                string ftprow = "drwxr-xr-x 1 owner group " + "0".PadLeft(12) + " ";
                ftprow += String.Format("{0:MMM dd HH:mm}", DateTime.Now) + " " + asDirectories[nIndex] + Environment.NewLine;
                stringBuilder.Append(ftprow);
            }
            
            for (int nIndex = 0; nIndex < asFiles.Length; nIndex++)
            {
                string sFile = asFiles[nIndex];
                sFile = Path.Combine(sDirectory, sFile);

                IFileInfo info = ConnectionObject.FileSystemObject.GetFileInfo(sFile);

                if (info != null)
                {
                    string perms = info.GetAttributeString().Replace("d", "-");
                    string ftprow = perms + " 1 owner group " + info.GetSize().ToString().PadLeft(12) + " ";
                    ftprow += String.Format("{0:MMM dd HH:mm}", info.GetModifiedTime()) + " " + asFiles[nIndex] + Environment.NewLine;
                    stringBuilder.Append(ftprow);
                }
            }

            string o = stringBuilder.ToString();
            return o;
        }
    }
}