using System.IO;
using AzureFtpServer.Ftp.FileSystem;

namespace AzureFtpServer.Azure
{
    public sealed class AzureFile : IFile
    {
        #region Implementation of IFile

        public MemoryStream File { get; set; }

        public int Read(byte[] abData, int nDataSize)
        {
            if (File == null)
            {
                return 0;
            }

            try
            {
                return File.Read(abData, 0, nDataSize);
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public int Write(byte[] abData, int nDataSize)
        {
            if (File == null)
            {
                File = new MemoryStream();
            }

            try
            {
                File.Write(abData, 0, nDataSize);
                return nDataSize;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public void Close()
        {
            if (File != null)
            {
                try
                {
                    File.Close();
                }
                catch (IOException)
                {
                }

                File = null;
            }
        }

        public void Load(byte[] data)
        {
            File = new MemoryStream(data, 0, data.Length);
        }

        #endregion
    }
}