using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //const for new line in HTTP, only!!!
            const string NewLine = "\r\n";
            //Loopback(localhost, port 8080):
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 8080);
            //start loopback:
            tcpListener.Start();

            //starting program that never stops: (daemon, service)
            while (true)
            {
                //create client tcp listener:
                var client = tcpListener.AcceptTcpClient();
                //now we have everithing to comunicate with client
                //we get stream and we can read or write from/with client:
                //use using() to automatic close the stream after we end with write:
                using (var stream = client.GetStream())
                {

                    //reading from stream:
                    //1. create buffer:
                    byte[] buffer = new byte[1000000];
                    //2. read from stream:
                    var lengthOfReading = stream.Read(buffer, 0, buffer.Length);
                    // get string from byte stream:
                    var requestString = Encoding.UTF8.GetString(buffer, 0, lengthOfReading);
                    Console.WriteLine(requestString);

                    Console.WriteLine(new string('=', 70));

                    string html = $"<h1>Hello from DemoServer 2022 {DateTime.Now}</h1>" +
                        //create form:
                        $"<form method=post><input name=username /><input name=password />" +
                        $"<input type=submit></form>";

                    //3.return response to browser:
                    var response = "HTTP/1.1 200 OK" + NewLine +
                        "Server: DemoServer 2022" + NewLine +
                        "Content-Type: text/html; charset=utf-8" + NewLine +
                        "Content-Length: " + html.Length + NewLine +
                        NewLine +
                        html + NewLine;

                    //4.create response buffer for writing the stream:
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes);
                }
            }

        }

        public async Task ReadData()
        {
            //Console.InputEncoding = Encoding.UTF8;
            string url = "https://softuni.bg/courses/csharp-web-basics";
            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            Console.WriteLine(html);
        }
    }
}
