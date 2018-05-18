//数据加密类
    class DataEncryption
    {
        private const string sStrKey = "qJzGEh6hESZDVJeCnFPGuxzaiB7NLQM6";//必须为32位
        private const string sIV = "ILoveYou1+3=";//必须为12位 FromBase64String最后一位必须为=

        #region 3DES加密
        /// <summary>
        /// 3DES加密
        /// </summary>
        /// <param name="aStrString">要加密的字符串</param>
        /// <param name="aStrKey">秘钥</param>
        /// <param name="mode">运算模式</param>
        /// <param name="iv">加密矢量：只有在CBC解密模式下才适用</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt3Des(string aStrString, string aStrKey = sStrKey, CipherMode mode = CipherMode.CBC, string iv = sIV)
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider()
                {
                    //  Key = Encoding.Unicode.GetBytes(aStrKey),这样写会报指定键的大小对此算法无效
                    Key = Convert.FromBase64String(aStrKey),
                    Mode = mode
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = Convert.FromBase64String(iv);
                }
                var desEncrypt = des.CreateEncryptor();
                byte[] buffer = Encoding.Unicode.GetBytes(aStrString);
                return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }

        }

        #endregion


        #region 3DES解密
        /// <summary>
        /// 3DES解密
        /// </summary>
        /// <param name="aStrString">需要解密的字符串</param>
        /// <param name="aStrKey">秘钥</param>
        /// <param name="mode">运算模式</param>
        /// <param name="iv">加密矢量：只有在CBC模式下才适用</param>
        /// <returns>解密字符串</returns>
        public static string Decrypt3Des(string aStrString, string aStrKey = sStrKey, CipherMode mode = CipherMode.CBC, string iv = sIV)
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider()
                {
                    //Key = Encoding.Unicode.GetBytes(aStrKey),
                    Key = Convert.FromBase64String(aStrKey),
                    Mode = mode,
                    Padding = PaddingMode.PKCS7
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = Convert.FromBase64String(iv);
                }
                var desDecrypt = des.CreateDecryptor();
                byte[] buffer = Convert.FromBase64String(aStrString);// Encoding.Unicode.GetBytes(aStrString);
                var result = Encoding.Unicode.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }

        }

        #endregion

    }