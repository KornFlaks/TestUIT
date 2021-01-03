using System.Collections.Generic;
using Unity.Entities;

namespace Components.Singletons.Managed
{
    public class SingletonManagedCountryNames : IComponentData, INameComponent<string>
    {
        public SingletonManagedCountryNames()
        {
            Values = new string[1];
            Altered = new Queue<(int, string)>();
        }

        public SingletonManagedCountryNames(string[] names) : this()
        {
            Values = names;
        }

        // Managed Component. No job processing for this.
        public string[] Values { get; set; }
        public Queue<(int, string)> Altered { get; set; }
    }
}