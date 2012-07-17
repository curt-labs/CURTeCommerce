namespace AzureFtpServer.General
{
    public interface ILoaded
    {
        bool Loaded { get; }
    }

    public class LoadedClass : ILoaded
    {
        protected bool m_fLoaded;

        #region ILoaded Members

        public bool Loaded
        {
            get { return m_fLoaded; }
        }

        #endregion
    }
}