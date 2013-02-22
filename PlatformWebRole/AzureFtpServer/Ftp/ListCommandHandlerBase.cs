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
                stringBuilder.Append("drwxr-xr-x 1 owner group");
                stringBuilder.Append(" 0 ");

                DateTime fileDate = DateTime.Now;
                string sDay = fileDate.Day.ToString();

                stringBuilder.Append(TextHelpers.Month(fileDate.Month));
                stringBuilder.Append(" ");

                if (sDay.Length == 1) {
                    stringBuilder.Append(" ");
                }

                stringBuilder.Append(sDay);
                stringBuilder.Append(" ");
                stringBuilder.Append(string.Format("{0:hh}", fileDate));
                stringBuilder.Append(":");
                stringBuilder.Append(string.Format("{0:mm}", fileDate));
                stringBuilder.Append(" ");

                stringBuilder.Append(asDirectories[nIndex]);
                stringBuilder.Append(" ");
                stringBuilder.Append(Environment.NewLine);
            }
            
            for (int nIndex = 0; nIndex < asFiles.Length; nIndex++)
            {
                string sFile = asFiles[nIndex];
                sFile = Path.Combine(sDirectory, sFile);

                IFileInfo info = ConnectionObject.FileSystemObject.GetFileInfo(sFile);

                if (info != null)
                {
                    string sAttributes = info.GetAttributeString();
                    stringBuilder.Append(sAttributes);
                    stringBuilder.Append(" 1 owner group");

                    DateTime fileDate = DateTime.Now;

                    string sFileSize = info.GetSize().ToString().PadLeft(12);
                    stringBuilder.Append(sFileSize);
                    //stringBuilder.Append(TextHelpers.RightAlignString(sFileSize, 13, ' '));
                    stringBuilder.Append(" ");
                    fileDate = info.GetModifiedTime();

                    string sDay = fileDate.Day.ToString();

                    stringBuilder.Append(TextHelpers.Month(fileDate.Month));
                    stringBuilder.Append(" ");

                    if (sDay.Length == 1)
                    {
                        stringBuilder.Append(" ");
                    }

                    stringBuilder.Append(sDay);
                    stringBuilder.Append(" ");
                    stringBuilder.Append(string.Format("{0:hh}", fileDate));
                    stringBuilder.Append(":");
                    stringBuilder.Append(string.Format("{0:mm}", fileDate));
                    stringBuilder.Append(" ");

                    stringBuilder.Append(asFiles[nIndex]);
                    stringBuilder.Append(" ");
                    stringBuilder.Append(Environment.NewLine);
                }
            }

            string o = stringBuilder.ToString();
            return o;
        }
    }
}