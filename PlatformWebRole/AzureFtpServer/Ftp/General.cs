using System.Diagnostics;
using System.Text;

namespace AzureFtpServer.Ftp.General
{
    /// <summary>
    /// Helper functions relating to files and file names/paths
    /// </summary>
    public class FileNameHelpers
    {
        public static bool IsValid(string sFileName)
        {
            if (sFileName.IndexOf("\\\\") >= 0)
            {
                return false;
            }

            if (sFileName.IndexOf("...") >= 0)
            {
                return false;
            }

            return true;
        }
    }

    public class TextHelpers
    {
        public static string RightAlignString(string sString, int nWidth, char cDelimiter)
        {
            var stringBuilder = new StringBuilder();

            for (int nCharacter = 0; nCharacter < nWidth - sString.Length; nCharacter++)
            {
                stringBuilder.Append(cDelimiter);
            }

            stringBuilder.Append(sString);
            return stringBuilder.ToString();
        }

        public static string Month(int nMonth)
        {
            switch (nMonth)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
                default:
                    Debug.Assert(false);
                    return "";
            }
        }
    }
}