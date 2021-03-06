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