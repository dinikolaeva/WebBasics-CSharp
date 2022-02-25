using System.Text;

namespace SUS.HTTP
{
    public class HttpRequest
    {
        public HttpRequest(string requestString)
        {
            this.Headers = new List<Header>();
            this.Cookies = new List<Cookie>();

            var lines = requestString.Split(new string[] { HttpConstants.NewLine }, StringSplitOptions.None);

            var headerLine = lines[0];
            var haderLineParts = headerLine.Split(' ');

            this.Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), haderLineParts[0], true);
            this.Path = haderLineParts[1];

            int lineIndex = 1;
            bool isInHeaders = true;
            StringBuilder bodyBuilder = new StringBuilder();

            while (lineIndex < lines.Length)
            {
                var line = lines[lineIndex];
                lineIndex++;

                if (string.IsNullOrEmpty(line))
                {
                    isInHeaders = false;
                    continue;
                }

                if (isInHeaders)
                {
                    //read headers
                    this.Headers.Add(new Header(line));
                }
                else
                {
                    //read body
                    bodyBuilder.AppendLine(line);
                }
            }

            if (this.Headers.Any(x => x.Name == HttpConstants.RequestCookieHeader))
            {
                var cookiesAsString = this.Headers.FirstOrDefault(x => x.Name == HttpConstants.RequestCookieHeader).Value;

                var cookiesParts = cookiesAsString.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var cookieAsString in cookiesParts)
                {
                    this.Cookies.Add(new Cookie(cookieAsString));
                }
            }

            this.Body = bodyBuilder.ToString();
        }

        public string Path { get; set; }

        public HttpMethod Method { get; set; }
        public ICollection<Header> Headers { get; set; }

        public ICollection<Cookie> Cookies { get; set; }

        public string Body { get; set; }
    }
}
