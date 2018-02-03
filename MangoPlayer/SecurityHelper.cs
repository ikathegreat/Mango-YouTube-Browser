using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static MangoPlayer.Constants;

namespace MangoPlayer
{
    public static class SecurityHelper
    {
        static readonly char[] padding = { '=' };
        public static string Crypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text.TrimEnd(padding).Replace('+', '-').Replace('/', '_'));
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ Key);
            }
            return Convert.ToBase64String(data);
        }

        public static string Decrypt(string text)
        {
            byte[] data = Convert.FromBase64String(text);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ Key);
            }
            string result = Encoding.UTF8.GetString(data).Replace('_', '/').Replace('-', '+');
            switch (Encoding.UTF8.GetString(data).Length % 4)
            {
                case 2: result += "=="; break;
                case 3: result += "="; break;
            }
            return result;
        }
    }

}