//using Microsoft.AspNetCore.Http;
//using System.Text;

//namespace TeduEcommerce.Public.Web.Extensions
//{
//    public static class SessionExtensions
//    {
//        public static void SetStringSession(this ISession session, string key, string value)
//        {
//            session.Set(key, Encoding.UTF8.GetBytes(value));
//        }

//        public static string GetStringSession(this ISession session, string key)
//        {
//            var data = session.GetString(key);
//            if (data == null)
//            {
//                return null;
//            }
//            return data;
//        }
//    }
//}