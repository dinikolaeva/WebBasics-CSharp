namespace SUS.MvcFramework
{
    public class ServiceCollection : IServiceCollection
    {
        private Dictionary<Type, Type> dependencyContainer = new Dictionary<Type, Type>();
        public void Add<TSource, TDestination>()
        {
            this.dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }

        //dependency injection
        public object CreateInstance(Type type)
        {
            if (this.dependencyContainer.ContainsKey(type))
            {
                type = this.dependencyContainer[type];
            }

            //get public constructor with least parameters
            var constructor = type.GetConstructors()
                                  .OrderBy(c => c.GetParameters().Count())
                                  .FirstOrDefault();

            //get parameters of that constructor
            var parameters = constructor.GetParameters();

            var parameterValues = new List<object>();

            //create instance foreach parameter
            foreach (var parameter in parameters)
            {
                //recursion
                var parameterValue = CreateInstance(parameter.ParameterType);
                //fullfil the parameterValues list
                parameterValues.Add(parameterValue);
            }

            //invoke the constructor with the list of parameters value(array)
            var obj = constructor.Invoke(parameterValues.ToArray());

            return obj;
        }
    }
}
