using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Utils.Logging
{
    //USAGE: lb, lbprm (live templates)
    // lbs - log.Info($"begin {nameof(METHODNAME)}");
    // lbe - log.Info($"end {nameof(METHODNAME)}");
    public class LogBuilder
    {
        private static readonly Log log = new(nameof(LogBuilder));

        public enum Type
        {
            None,
            Simple,
            Json,
            JsonPretty,
        }

        private JObject _jObject;

        private readonly Type _type;
        private readonly bool _clearAfterToString;
        private readonly StringBuilder _sb;
        private readonly string _text;
        private bool IsJson => _type is Type.Json or Type.JsonPretty;

        public LogBuilder(string text, Type lbType = Type.JsonPretty, bool clearAfterToString = false) : this(lbType, clearAfterToString)
        {
            _text = text;
        }

        public LogBuilder(Type lbType = Type.Simple, bool clearAfterToString = false)
        {
            _type = lbType;
            _clearAfterToString = clearAfterToString;
            if (_type == Type.Simple)
                _sb = new StringBuilder();
            else
                _jObject = new JObject();
        }

        public void Clear()
        {
            if (_type == Type.Simple)
                _sb.Clear();
            else
                _jObject = new JObject();
        }

        public static implicit operator string(LogBuilder t) => t.ToString();

        public LogBuilder Param(string param, object value)
        {
            if (IsJson)
            {
                _jObject[param] = value.ToString();
            }
            else
            {
                _sb.Append($"{param}{value}\n");
            }

            return this;
        }

        public LogBuilder ParamJson(string param, JToken value)
        {
            if (IsJson)
            {
                _jObject[param] = value;
            }
            else
            {
                _sb.Append($"{param}{value}\n");
            }

            return this;
        }

        public LogBuilder ParamWarnArr(string param, object value) => ParamArr($"<color=yellow>{param}</color>", $"<color=yellow>{value}</color>");
        public LogBuilder ParamArr(string param, object value)
        {
            if (IsJson)
            {
                log.Error("cannot use ParamArr in not json");
                return this;
            }

            var c = _jObject[param];
            if (c == null || !c.HasValues || c.Type != JTokenType.Array) 
                _jObject[param] = new JArray();
            _jObject[param].Value<JArray>().Add(value.ToString());
            
            return this;
        }

        public override string ToString()
        {
            string s;

            if (_type == Type.Simple)
                s = $"\n{_sb}";
            else
                s = _jObject.ToString(_type == Type.JsonPretty ? Formatting.Indented : Formatting.None);

            if (_clearAfterToString)
                Clear();

            if (string.IsNullOrEmpty(_text))
                return s;

            return $"{_text} {s}";
        }
    }
}