using System.Text;
using System.Web;

namespace F1Interface
{
    internal class QueryStringBuilder
    {
        private readonly StringBuilder stringBuilder;
        private bool firstParameter = true;

        public QueryStringBuilder(string baseUri)
        {
            stringBuilder = new StringBuilder(baseUri);
        }

        public QueryStringBuilder AppendUri(string uri)
        {
            bool endsWithSlash = (stringBuilder[stringBuilder.Length - 1] == '/');
            if (uri[0] != '/' && !endsWithSlash)
            {
                stringBuilder.Append('/');
            }
            else if (uri[0] == '/' && endsWithSlash)
            {
                stringBuilder.Length = stringBuilder.Length - 1;
            }

            stringBuilder.Append(uri);

            return this;
        }

        public QueryStringBuilder AddParameter(string name, object value)
        {
            if (firstParameter)
            {
                firstParameter = false;
                stringBuilder.Append('?');
            }
            else
            {
                stringBuilder.Append('&');
            }

            stringBuilder.Append(HttpUtility.UrlEncode(name));
            stringBuilder.Append("=");
            stringBuilder.Append(HttpUtility.UrlEncode(value.ToString()));

            return this;
        }

        public override string ToString()
            => stringBuilder.ToString();
    }
}