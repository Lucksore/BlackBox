using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace BlackBox
{
    class tripleDES
    {
        private TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        private static Encoding E = Encoding.UTF8;

        public tripleDES(string Key)
        {
            des.Key = GetValidKey(Key);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
        }
        public byte[] EncryptData(byte[] Data) => des.CreateEncryptor().TransformFinalBlock(Data, 0, Data.Length);
        public byte[] DecryptData(byte[] Data) => des.CreateDecryptor().TransformFinalBlock(Data, 0, Data.Length);
        public byte[] EncryptData(string Data) => EncryptData(E.GetBytes(Data));
        public byte[] EncryptData(string[] Data)
        {
            string s = string.Empty;
            for (int i = 0; i < Data.Length; i++) s += Data[i] + '\n';
            return EncryptData(s);
        }
        public void EncryptFile(byte[] Data, string FilePath) => File.WriteAllBytes(FilePath, EncryptData(Data));
        public void EncryptFile(string Data, string FilePath) => File.WriteAllBytes(FilePath, EncryptData(Data));
        public void EncryptFile(string[] Data, string FilePath) => File.WriteAllBytes(FilePath, EncryptData(Data));
        public string DecryptFile(string FilePath)
        {
            byte[] bytes = File.ReadAllBytes(FilePath);
            if (bytes.Length == 0) return string.Empty;
            else return E.GetString(DecryptData(bytes));
        }
        private byte[] GetValidKey(string Key)
        {
            if (Key.Length > 16) Key = Key.Substring(0, 16);
            if (Key.Length < 16)
            {
                int d = 16 - Key.Length;
                for (int i = 0; i < d; i++) Key += '-';
            }
            return E.GetBytes(Key);
        }
        public void SetKey(string Key) => des.Key = GetValidKey(Key);
    }
}
