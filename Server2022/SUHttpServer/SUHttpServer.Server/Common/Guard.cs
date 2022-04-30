namespace SUHttpServer.Server.Common
{
    public class Guard
    {
        public static void AgainstNull(object value, string name = null)
        {
            if (value == null)
            {
                name ??= "Value";

                throw new ArgumentException($"{name} can not be null.");
            }
        }
    }
}
