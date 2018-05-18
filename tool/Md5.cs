using System.Security.Cryptography;
using System.Text;

namespace Utility
{
    public class Md5Util
    {

        public static byte[] GetMD5FromUTF8(string source)
        {
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(source));
        }


        public static string GetMD5StringFromUTF8(string source)
        {
            var sb = new StringBuilder();
            var bytes = GetMD5FromUTF8(source);
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

    }
}