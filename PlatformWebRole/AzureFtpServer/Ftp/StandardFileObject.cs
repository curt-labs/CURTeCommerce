using System;
using System.IO;
using AzureFtpServer.General;

namespace AzureFtpServer.Ftp.FileSystem
{
    internal class StandardFileObject : LoadedClass, IFile
    {
        private FileStream m_theFile;

        public StandardFileObject(string sPath, bool fWrite)
        {
            try
            {
                m_theFile = new FileStream(sPath,
                                           (fWrite) ? FileMode.OpenOrCreate : FileMode.Open,
                                           (fWrite) ? FileAccess.Write : FileAccess.Read);

                if (fWrite)
                {
                    m_theFile.Seek(0, SeekOrigin.End);
                }

                m_fLoaded = true;
            }
            catch (IOException)
            {
                m_theFile = null;
            }
        }

        #region IFile Members

        public int Read(byte[] abData, int nDataSize)
        {
            if (m_theFile == null)
            {
                return 0;
            }

            try
            {
                return m_theFile.Read(abData, 0, nDataSize);
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public int Write(byte[] abData, int nDataSize)
        {
            if (m_theFile == null)
            {
                return 0;
            }

            try
            {
                m_theFile.Write(abData, 0, nDataSize);
            }
            catch (IOException)
            {
                return 0;
            }

            return nDataSize;
        }

        public void Close()
        {
            if (m_theFile != null)
            {
                try
                {
                    m_theFile.Close();
                }
                catch (IOException)
                {
                }

                m_theFile = null;
            }
        }

        public MemoryStream File
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}