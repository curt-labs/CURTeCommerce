using System;
using System.IO;

namespace AzureFtpServer.Ftp.FileSystem
{
    public interface IFile
    {
        MemoryStream File { get; set; }
        int Read(byte[] abData, int nDataSize);
        int Write(byte[] abData, int nDataSize);
        void Close();
    }

    public interface IFileInfo
    {
        DateTime GetModifiedTime();
        long GetSize();
        string GetAttributeString();
        bool IsDirectory();
        string Path();
        bool FileObjectExists();
    }

    public interface IFileSystem
    {
        IFile OpenFile(string sPath, bool fWrite);
        IFileInfo GetFileInfo(string sPath);

        string[] GetFiles(string sPath);
        string[] GetFiles(string sPath, string sWildcard);
        string[] GetDirectories(string sPath);
        string[] GetDirectories(string sPath, string sWildcard);

        bool DirectoryExists(string sPath);
        bool FileExists(string sPath);
        bool Put(string sPath, IFile oFile); // Added

        bool CreateDirectory(string sPath);
        bool Move(string sOldPath, string sNewPath);
        bool Delete(string sPath);
    }

    public interface IFileSystemClassFactory
    {
        IFileSystem Create(string sUser, string sPassword);
    }
}