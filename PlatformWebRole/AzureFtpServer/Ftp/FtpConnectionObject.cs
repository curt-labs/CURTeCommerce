using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.FtpCommands;
using AzureFtpServer.Ftp;
using AzureFtpServer.General;

namespace AzureFtpServer.Ftp
{
    /// <summary>
    /// Processes incoming messages and passes the data on to the relevant handler class.
    /// </summary>
    internal class FtpConnectionObject : FtpConnectionData
    {
        #region Member Variables

        private readonly IFileSystemClassFactory m_fileSystemClassFactory;
        private readonly Hashtable m_theCommandHashTable;

        #endregion

        #region Construction

        public FtpConnectionObject(IFileSystemClassFactory fileSystemClassFactory, int nId, TcpClient socket)
            : base(nId, socket)
        {
            m_theCommandHashTable = new Hashtable();
            m_fileSystemClassFactory = fileSystemClassFactory;

            LoadCommands();
        }

        #endregion

        #region Methods

        public bool Login(string sPassword)
        {
            IFileSystem fileSystem = m_fileSystemClassFactory.Create(User, sPassword);

            if (fileSystem == null)
            {
                return false;
            }

            SetFileSystemObject(fileSystem);
            return true;
        }

        private void LoadCommands()
        {
            AddCommand(new UserCommandHandler(this));
            AddCommand(new PasswordCommandHandler(this));
            AddCommand(new QuitCommandHandler(this));
            AddCommand(new CwdCommandHandler(this));
            AddCommand(new PortCommandHandler(this));
            AddCommand(new PasvCommandHandler(this));
            AddCommand(new ListCommandHandler(this));
            AddCommand(new NlstCommandHandler(this));
            AddCommand(new PwdCommandHandler(this));
            AddCommand(new XPwdCommandHandler(this));
            AddCommand(new TypeCommandHandler(this));
            AddCommand(new RetrCommandHandler(this));
            AddCommand(new NoopCommandHandler(this));
            AddCommand(new SizeCommandHandler(this));
            AddCommand(new DeleCommandHandler(this));
            AddCommand(new AlloCommandHandler(this));
            AddCommand(new StoreCommandHandler(this));
            AddCommand(new MakeDirectoryCommandHandler(this));
            AddCommand(new RemoveDirectoryCommandHandler(this));
            AddCommand(new AppendCommandHandler(this));
            AddCommand(new RenameStartCommandHandler(this));
            AddCommand(new RenameCompleteCommandHandler(this));
            AddCommand(new XMkdCommandHandler(this));
            AddCommand(new XRmdCommandHandler(this));
            AddCommand(new CDUPCommandHandler(this));
        }

        private void AddCommand(FtpCommandHandler handler)
        {
            m_theCommandHashTable.Add(handler.Command, handler);
        }

        public void Process(Byte[] abData)
        {
            string sMessage = Encoding.ASCII.GetString(abData);
            sMessage = sMessage.Substring(0, sMessage.IndexOf('\r'));

            FtpServerMessageHandler.SendMessage(Id, sMessage);

            string sCommand;
            string sValue;

            int nSpaceIndex = sMessage.IndexOf(' ');

            if (nSpaceIndex < 0)
            {
                sCommand = sMessage.ToUpper();
                sValue = "";
            }
            else
            {
                sCommand = sMessage.Substring(0, nSpaceIndex).ToUpper();
                sValue = sMessage.Substring(sCommand.Length + 1);
            }

            var handler = m_theCommandHashTable[sCommand] as FtpCommandHandler;

            if (handler == null)
            {
                FtpServerMessageHandler.SendMessage(Id, string.Format("\"{0}\" : Unknown command", sCommand));
                SocketHelpers.Send(Socket, "550 Unknown command\r\n");
            }
            else
            {
                handler.Process(sValue);
            }
        }

        #endregion
    }
}