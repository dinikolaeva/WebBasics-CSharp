using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TaskCreateDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Task> tasks = new List<Task>();

            for (int i = 1; i <= 100; i++)
            {
                var task = Task.Run(async () =>
                {
                    HttpClient httpClient = new HttpClient();

                    var url = $"https://vicove.com/vic-{i}";

                    var httpResponse = await httpClient.GetAsync(url);
                    var vic = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(vic.Length);
                });

                tasks.Add(task);
            }

            //wait all task to finish
            Task.WaitAll(tasks.ToArray());
        }
    }
}
