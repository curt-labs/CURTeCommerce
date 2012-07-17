using System;
using System.IO;
using System.Text;
using AzureFtpServer.General;

namespace AzureFtpServer.Ftp.FileSystem
{
    internal class StandardFileInfoObject : LoadedClass, IFileInfo
    {
        #region Member Variables

        private readonly FileInfo m_theInfo;

        #endregion

        #region Construction

        public StandardFileInfoObject(string sPath)
        {
            try
            {
                m_theInfo = new FileInfo(sPath);
                m_fLoaded = true;
            }
            catch (IOException)
            {
                m_theInfo = null;
            }
        }

        #endregion

        #region IFileInfo Members

        public string Path()
        {
            return m_theInfo.Name;
        }

        public bool FileObjectExists()
        {
            return m_theInfo != null;
        }

        public bool IsDirectory()
        {
            return (m_theInfo.Attributes & FileAttributes.Directory) != 0;
        }

        public DateTime GetModifiedTime()
        {
            return m_theInfo.LastWriteTime;
        }

        public long GetSize()
        {
            return m_theInfo.Length;
        }

        public string GetAttributeString()
        {
            bool fDirectory = (m_theInfo.Attributes & FileAttributes.Directory) != 0;
            bool fReadOnly = (m_theInfo.Attributes & FileAttributes.ReadOnly) != 0;

            var builder = new StringBuilder();

            if (fDirectory)
            {
                builder.Append("d");
            }
            else
            {
                builder.Append("-");
            }

            builder.Append("r");

            if (fReadOnly)
            {
                builder.Append("-");
            }
            else
            {
                builder.Append("w");
            }

            if (fDirectory)
            {
                builder.Append("x");
            }
            else
            {
                builder.Append("-");
            }

            if (fDirectory)
            {
                builder.Append("r-xr-x");
            }
            else
            {
                builder.Append("r--r--");
            }

            return builder.ToString();
        }

        #endregion
    }
}