using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Project.Models
{
    public class FingerPoint
    {
        public static string Tag(string rootRelaticePath)
        {
            if(HttpRuntime.Cache[rootRelaticePath] == null)
            {
                string absolute = HostingEnvironment.MapPath('~' + rootRelaticePath);

                DateTime date = File.GetLastWriteTime(absolute);
                int index = rootRelaticePath.LastIndexOf('/');

                string result = rootRelaticePath.Insert(index, "/v-" + date.Ticks);
                return result;
                //HttpRuntime.Cache.Insert(rootRelaticePath, result, new CacheDependency(absolute)); 
            }
            return HttpRuntime.Cache[rootRelaticePath] as string;
        }
    }
}