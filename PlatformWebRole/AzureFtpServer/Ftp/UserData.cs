using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AzureFtpServer.Ftp
{
    [Serializable]
    internal class UserDataItem
    {
        private string m_sPassword = "";
        private string m_sStartingDirectory = "C:\\";

        public string Password
        {
            get { return m_sPassword; }

            set { m_sPassword = value; }
        }

        public string StartingDirectory
        {
            get { return m_sStartingDirectory; }

            set { m_sStartingDirectory = value; }
        }
    }

    public class UserData
    {
        #region Member Variables

        private static UserData m_theObject;
        private Hashtable m_mapUserToData;

        #endregion

        #region Construction

        protected UserData()
        {
            m_mapUserToData = new Hashtable();
        }

        #endregion

        #region Properties

        public string[] Users
        {
            get
            {
                ICollection collectionUsers = m_mapUserToData.Keys;
                var asUsers = new string[collectionUsers.Count];

                int nIndex = 0;

                foreach (string sUser in collectionUsers)
                {
                    asUsers[nIndex] = sUser;
                    nIndex++;
                }

                return asUsers;
            }
        }

        public int UserCount
        {
            get { return m_mapUserToData.Count; }
        }

        public static UserData Get()
        {
            if (m_theObject == null)
            {
                m_theObject = new UserData();
            }

            return m_theObject;
        }

        #endregion

        #region Methods

        private UserDataItem GetUserItem(string sUser)
        {
            return m_mapUserToData[sUser] as UserDataItem;
        }

        public void AddUser(string sUser)
        {
            m_mapUserToData.Add(sUser, new UserDataItem());
        }

        public void RemoveUser(string sUser)
        {
            m_mapUserToData.Remove(sUser);
        }

        public string GetUserPassword(string sUser)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                return item.Password;
            }
            else
            {
                return "";
            }
        }

        public void SetUserPassword(string sUser, string sPassword)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                item.Password = sPassword;
            }
        }

        public string GetUserStartingDirectory(string sUser)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                return item.StartingDirectory;
            }
            else
            {
                return "C:\\";
            }
        }

        public void SetUserStartingDirectory(string sUser, string sDirectory)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                item.StartingDirectory = sDirectory;
            }
        }

        public bool HasUser(string sUser)
        {
            UserDataItem item = GetUserItem(sUser);
            return item != null;
        }

        public bool Save(string sFileName)
        {
            try
            {
                var formatter = new BinaryFormatter();
                var fileStream = new FileStream(sFileName, FileMode.Create);
                formatter.Serialize(fileStream, m_mapUserToData);
                fileStream.Close();
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool Load(string sFileName)
        {
            if (!File.Exists(sFileName))
            {
                return true;
            }

            try
            {
                var formatter = new BinaryFormatter();
                var fileStream = new FileStream(sFileName, FileMode.Open);
                m_mapUserToData = formatter.Deserialize(fileStream) as Hashtable;
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        private string GetDefaultPath()
        {
            return ("Users.dat");
        }

        public bool Save()
        {
            return Save(GetDefaultPath());
        }

        public bool Load()
        {
            return Load(GetDefaultPath());
        }

        #endregion
    }
}