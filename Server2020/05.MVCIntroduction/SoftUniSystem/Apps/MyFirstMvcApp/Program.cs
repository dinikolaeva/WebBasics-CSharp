using MyFirstMvcApp.Controllers;
using SUS.HTTP;
using SUS.MvcFramework;

namespace MyFirstMvcApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
           await Host.CreateHostAsync(new Startup());
        }
    }
}