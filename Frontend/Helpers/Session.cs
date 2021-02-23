using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Frontend.Helpers
{
    public static class Session
    {
        public static void AddStringListToSession(ISession session, string key, IEnumerable<string> valuesToStore)
        {
            var listAsString = string.Join(",", valuesToStore);
            session.SetString(key, listAsString);
        }

        public static List<string> GetStringListFromSession(ISession session, string key)
        {
            var sessionString = session.GetString(key);
            return sessionString.Split(",").ToList();
        }
    }
}