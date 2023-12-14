using System.Collections.Generic;
using Newtonsoft.Json;

namespace CycladeLocalization
{
    [JsonObject(MemberSerialization.OptOut)]
    public class LocJson
    {
        public Dictionary<string, Dictionary<string, string>> Areas;
    }
}