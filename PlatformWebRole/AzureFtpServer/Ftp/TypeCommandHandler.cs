using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// Implements the 'TYPE' command
    /// </summary>
    internal class TypeCommandHandler : FtpCommandHandler
    {
        public TypeCommandHandler(FtpConnectionObject connectionObject)
            : base("TYPE", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.ToUpper();

            if (sMessage == "A")
            {
                ConnectionObject.BinaryMode = false;
                return GetMessage(200, "ASCII transfer mode active.");
            }
            else if (sMessage == "I")
            {
                ConnectionObject.BinaryMode = true;
                return GetMessage(200, "Binary transfer mode active.");
            }
            else
            {
                return GetMessage(550, string.Format("Error - unknown binary mode \"{0}\"", sMessage));
            }
        }
    }
}