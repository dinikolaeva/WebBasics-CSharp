﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SUS.HTTP
{
    public class HttpServer : IHttpServer
    {
        List<Route> routeTable;
        public HttpServer(List<Route> routeTable)
        {
            this.routeTable = routeTable;
        }

        public async Task StartAsync(int port)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();

            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                ProcessClientAsync(tcpClient);
            }
        }

        private async Task ProcessClientAsync(TcpClient tcpClient)
        {
            try
            {
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    List<byte> data = new List<byte>();
                    int position = 0;
                    byte[] buffer = new byte[4096];

                    while (true)
                    {
                        int count = await stream.ReadAsync(buffer, position, buffer.Length);
                        position += count;

                        if (count < buffer.Length)
                        {
                            var partialBuffer = new byte[count];
                            Array.Copy(buffer, partialBuffer, count);
                            data.AddRange(partialBuffer);
                            break;
                        }
                        else
                        {
                            data.AddRange(buffer);
                        }
                    }

                    //byte[] => string(text);
                    var requestAsString = Encoding.UTF8.GetString(data.ToArray());

                    var request = new HttpRequest(requestAsString);

                    Console.WriteLine($"{request.Method} {request.Path} => {request.Headers.Count} headers");

                    HttpResponse response;

                    var route = this.routeTable
                                    .FirstOrDefault(r => string
                                                        .Compare(r.Path, request.Path, true) == 0 &&
                                                                 r.Method == request.Method);

                    if (route != null)
                    {
                        response = route.Action(request);
                    }
                    else
                    {
                        //404 Not Found
                        response = new HttpResponse("text/html", new byte[0], HttpStatusCode.NotFound);
                    }

                    response.Headers.Add(new Header("Server", "SUS Server 1.0"));

                    var sessionCookie = request.Cookies
                                               .FirstOrDefault(c => c.Name == HttpConstants.SessionCookieName);

                    if (sessionCookie != null)
                    {
                        var responseSessionCookie = new ResponseCookie(sessionCookie.Name, sessionCookie.Value);
                        responseSessionCookie.Path = "/";
                        response.Cookies.Add(responseSessionCookie);
                    }

                    var responseHeaderBytes = Encoding.UTF8.GetBytes(response.ToString());

                    await stream.WriteAsync(responseHeaderBytes, 0, responseHeaderBytes.Length);
                    await stream.WriteAsync(response.Body, 0, response.Body.Length);
                }

                tcpClient.Close();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}