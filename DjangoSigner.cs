using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;

namespace NDjangoSignCookie
{
    /// <summary>
    /// Summary description for DjangoSignedCookie
    /// </summary>
    public static class DjangoSigner
    {
        public static string SECRET_KEY;
        public static char Sep = ':';

        static DjangoSigner()
        {
            // https://github.com/django/django/blob/stable/1.6.x/django/core/signing.py#L79
            SECRET_KEY = "django.http.cookies" + ConfigurationManager.AppSettings["SECRET_KEY"];
        }

        public static string sign(string message, string salt)
        {
            var encoding = new System.Text.UTF8Encoding();
            byte[] shaKey;

            // https://github.com/django/django/blob/stable/1.6.x/django/core/signing.py#L157
            salt = salt + "signer";

            using (var sha1 = SHA1.Create())
            {
                // https://github.com/django/django/blob/stable/1.6.x/django/utils/crypto.py#L44
                shaKey = sha1.ComputeHash(encoding.GetBytes(salt + SECRET_KEY));
            }

            byte[] messageBytes = encoding.GetBytes(message);
            using (var sha = new HMACSHA1(shaKey))
            {
                byte[] hashmessage = sha.ComputeHash(messageBytes);
                var signature = Convert.ToBase64String(hashmessage);

                //Make it urlsafe
                return signature.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }
        }

        public static string unsign(string valueWithSign, string salt = "")
        {
            var sepPos = valueWithSign.LastIndexOf(DjangoSigner.Sep);
            if (sepPos == -1)
            {
                return string.Empty;
            }

            string value = valueWithSign.Substring(0, sepPos);
            string cookieSign = valueWithSign.Substring(sepPos + 1);

            if (cookieSign == DjangoSigner.sign(value, salt))
            {
                return value;
            }
            return string.Empty;
        }
    }


    public static class TimestampSigner
    {
        private static String bst = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static String ToBase62(int val)
        {
            String result = "";
            do
            {
                int a = val % 62;
                result = bst[a] + result;
                val = (val - a) / 62;
            } while (val > 0);
            return result.PadLeft(4, '0');
        }

        private static string GetTimeStamp()
        {
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return ToBase62(unixTimestamp);
        }

        public static void SetSignedCookie(this HttpResponse response, HttpCookie ck, string salt = "")
        {
            // https://github.com/django/django/blob/stable/1.6.x/django/core/signing.py#L182
            string value = ck.Value + DjangoSigner.Sep + GetTimeStamp();

            // https://github.com/django/django/blob/stable/1.6.x/django/http/response.py#L257 
            salt = ck.Name + salt;

            value = value + DjangoSigner.Sep + DjangoSigner.sign(value, salt);

            // https://github.com/django/django/blob/stable/1.6.x/django/http/cookie.py#L45
            ck.Value = '"' + value + '"';
            response.SetCookie(ck);
        }

        public static string GetSignedCookie(this HttpRequest request, string name, string salt = "")
        {
            var ck = request.Cookies[name];
            if (ck == null)
            {
                return string.Empty;
            }

            // https://github.com/django/django/blob/stable/1.6.x/django/http/request.py#L96
            salt = name + salt;
            string value = DjangoSigner.unsign(ck.Value.Trim('"'), salt);

            var sepPos = value.LastIndexOf(DjangoSigner.Sep);
            if (sepPos == -1)
            {
                return string.Empty;
            }

            //todo: must check timestamp max-age
            return value.Substring(0, sepPos);
        }
    }
}
