using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace PWAPushNotification.Infrastructure
{
    public static class PersistentStorage
    {
        private static Dictionary<string, PushSubscription> StaticDic = new Dictionary<string, PushSubscription>();

        public static PushSubscription GetSubscription(string client)
        {
            PushSubscription res;
            StaticDic.TryGetValue(client, out res);
            return res;
        }

        public static void SaveSubscription(string client, PushSubscription subscription)
        {
            StaticDic.Add(client, subscription);
        }

        public static List<string> GetClientNames()
        {
            return StaticDic.Keys.ToList();
        }
    }


}
