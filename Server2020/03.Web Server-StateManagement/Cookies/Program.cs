using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cookies
{
    class Program
    {
        static Dictionary<string, int> SessionStorage = new Dictionary<string, int>();
        const string NewLine = "\r\n";

        static async Task Main(string[] args)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 80);
            tcpListener.Start();

            while (true)
            {
                var client = tcpListener.AcceptTcpClient();
                await ProcessClientAsync(client);
            }
        }

        public static async Task ProcessClientAsync(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                byte[] buffer = new byte[1000000];
                var lengthOfReading = await stream.ReadAsync(buffer, 0, buffer.Length);
                var requestString = Encoding.UTF8.GetString(buffer, 0, lengthOfReading);
                Console.WriteLine(requestString);

                Console.WriteLine(new string('=', 70));

                //check that session id exist:
                var sid = Guid.NewGuid().ToString();
                var match = Regex.Match(requestString, @"sid=[^\n]*\r\n");

                if (match.Success)
                {
                    sid = match.Value.Substring(4);
                }                

                //check if session exist:
                bool sessionSet = false;

                if (requestString.Contains("sid="))
                {
                    sessionSet = true;
                }
                //check if session storage didn't have this session:
                if (!SessionStorage.ContainsKey(sid))
                {
                    SessionStorage.Add(sid, 0);
                }

                SessionStorage[sid]++;

                Console.WriteLine(sid);

                //print count of session storage:
                string html = $"<h1>Hello from DemoServer 2022 {DateTime.Now} for the {SessionStorage[sid]} time</h1>" +
                    $"<form method=post><input name=username /><input name=password />" +
                    $"<input type=submit></form>";

                var response = "HTTP/1.1 200 OK" + NewLine +
                    "Server: DemoServer 2022" + NewLine +
                    "Content-Type: text/html; charset=utf-8" + NewLine +
                    //make verification thoes we have session set:
                    (!sessionSet ?
                    //set cookie header:
                    //given session:
                    $"Set-Cookie: sid={sid}" : string.Empty +
                    //we can have more than one cookie, but everyone must be in new line!!!
                    //set date time to format cookie with Expire:
                    "Set-Cookie: sid2=dsfdg54gs6454fs56g4h5; Expires=" + DateTime.UtcNow.AddSeconds(10).ToString("R") +
                    //set date time to format cookie with MaxAge (only seconds!!!):
                    "Set-Cookie: lang=en; Max-Age=") + (3 * 60) +
                    "Content-Length: " + html.Length + NewLine +
                    NewLine +
                    html + NewLine;

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes);
            }
        }
    }
}
