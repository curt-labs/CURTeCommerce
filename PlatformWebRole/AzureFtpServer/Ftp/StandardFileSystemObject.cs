using System;
using System.IO;

namespace AzureFtpServer.Ftp.FileSystem
{
    internal class StandardFileSystemObject : IFileSystem
    {
        #region Member Variables

        private readonly string m_sStartDirectory = "";

        #endregion

        #region Construction

        public StandardFileSystemObject(string sStartDirectory)
        {
            m_sStartDirectory = sStartDirectory;
        }

        #endregion

        #region Methods

        private string GetPath(string sPath)
        {
            if (sPath.Length == 0)
            {
                return m_sStartDirectory;
            }

            if (sPath[0] == '\\')
            {
                sPath = sPath.Substring(1);
            }

            return Path.Combine(m_sStartDirectory, sPath);
        }

        #endregion

        #region IFileSystem Members

        public IFile OpenFile(string sPath, bool fWrite)
        {
            var file = new StandardFileObject(GetPath(sPath), fWrite);

            if (file.Loaded)
            {
                return file;
            }
            else
            {
                return null;
            }
        }

        public IFile OpenRemoteFile(string sPath, bool fWrite) {
            var file = new StandardFileObject(GetPath(sPath), fWrite);

            if (file.Loaded) {
                return file;
            } else {
                return null;
            }
        }

        public IFileInfo GetFileInfo(string sPath)
        {
            var info = new StandardFileInfoObject(GetPath(sPath));
            if (info.Loaded)
            {
                return info;
            }
            else
            {
                return null;
            }
        }

        public string[] GetFiles(string sPath)
        {
            string sCurrentPath = GetPath(sPath);
            string[] asFiles = Directory.GetFiles(sCurrentPath);
            RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public string[] GetFiles(string sPath, string sWildcard)
        {
            string sCurrentPath = GetPath(sPath);
            string[] asFiles = Directory.GetFiles(sCurrentPath, sWildcard);
            RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public string[] GetDirectories(string sPath)
        {
            string sCurrentPath = GetPath(sPath);
            string[] asFiles = Directory.GetDirectories(sCurrentPath);
            RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public string[] GetDirectories(string sPath, string sWildcard)
        {
            string sCurrentPath = GetPath(sPath);
            string[] asFiles = Directory.GetDirectories(sCurrentPath, sWildcard);
            RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public bool DirectoryExists(string sPath)
        {
            return Directory.Exists(GetPath(sPath));
        }

        public bool FileExists(string sPath)
        {
            return File.Exists(GetPath(sPath));
        }

        public bool Move(string sOldPath, string sNewPath)
        {
            string sFullPathOld = GetPath(sOldPath);
            string sFullPathNew = GetPath(sNewPath);

            try
            {
                var info = new FileInfo(sFullPathOld);

                if (info == null)
                {
                    return false;
                }

                if ((info.Attributes & FileAttributes.Directory) != 0)
                {
                    Directory.Move(sFullPathOld, sFullPathNew);
                }
                else
                {
                    File.Move(sFullPathOld, sFullPathNew);
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool Delete(string sPath)
        {
            try
            {
                string sFullPath = GetPath(sPath);

                var info = new FileInfo(sFullPath);

                if (info == null)
                {
                    return false;
                }

                if ((info.Attributes & FileAttributes.Directory) != 0)
                {
                    Directory.Delete(sFullPath);
                }
                else
                {
                    File.Delete(sFullPath);
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool CreateDirectory(string sPath)
        {
            string sFullPath = GetPath(sPath);

            try
            {
                Directory.CreateDirectory(sFullPath);
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }


        public bool Put(string sPath, IFile oFile)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void RemovePath(string[] asFiles, string sPath)
        {
            int nIndex = 0;

            string sPathLowerCase = sPath.ToLower();

            foreach (string sString in asFiles)
            {
                if (sString.Substring(0, sPath.Length).ToLower() == sPathLowerCase)
                {
                    string sFileName = sString.Substring(sPath.Length);

                    if (sFileName.Length > 0 && sFileName[0] == '\\')
                    {
                        sFileName = sFileName.Substring(1);
                    }

                    asFiles[nIndex] = sFileName;
                }

                nIndex += 1;
            }
        }
    }
}