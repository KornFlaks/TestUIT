using System.Collections.Generic;

namespace Components.Singletons.Managed
{
    public class SingletonManagedAreaNames : INameComponent<string>
    {
        public SingletonManagedAreaNames(string[] names)
        {
            Values = names;
            Altered = new Queue<(int, string)>();
        }

        public string[] Values { get; set; }
        public Queue<(int, string)> Altered { get; set; }
    }
}