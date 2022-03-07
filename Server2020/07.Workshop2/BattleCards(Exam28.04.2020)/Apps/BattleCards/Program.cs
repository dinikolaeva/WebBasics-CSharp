using SUS.MvcFramework;

namespace BattleCards
{
    class Program
    {
        static async Task Main(string[] args)
        {
           await Host.CreateHostAsync(new Startup());
        }
    }
}