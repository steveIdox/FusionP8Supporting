using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace Format
{
    public class Json
    {
        public static string Format(string value) 
        {  
            return JsonConvert.SerializeObject(value);
        }
        public static string Format(string[] values)
        {
            return JsonConvert.SerializeObject(values);
        }
    }
}
