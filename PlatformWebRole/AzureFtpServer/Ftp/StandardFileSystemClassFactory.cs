namespace AzureFtpServer.Ftp.FileSystem
{
    public class StandardFileSystemClassFactory : IFileSystemClassFactory
    {
        #region IFileSystemClassFactory Members

        public IFileSystem Create(string sUser, string sPassword)
        {
            if (UserData.Get().HasUser(sUser) && UserData.Get().GetUserPassword(sUser) == sPassword)
            {
                return new StandardFileSystemObject(UserData.Get().GetUserStartingDirectory(sUser));
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}