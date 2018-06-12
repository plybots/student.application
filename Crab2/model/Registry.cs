using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CRABSTUDENT.model
{
    class Registry
    {
        private static Registry instance = new Registry();

        public readonly Dictionary<string, object> registry 
            = new Dictionary<string, object>();

        public static Registry getInstance()
        {
            return instance;
        }

        private Registry()
        {

        }

        public bool isRegistered(string className)
        {
            return registry.ContainsKey(className);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public object getReference(string className)
        {
           
            if (isRegistered(className))
            {
                return registry[className];
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void register(object instance)
        {
            if (registry.ContainsKey(TypeDescriptor.GetClassName(instance)))
            {
                registry.Remove(TypeDescriptor.GetClassName(instance));   
            }

            registry.Add(TypeDescriptor.GetClassName(instance), instance);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void register(object instance, string name)
        {
            if (registry.ContainsKey(name))
            {
                registry.Remove(name);
            }

            registry.Add(name, instance);
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool remove(string className)
        {
            if (registry.ContainsKey(className))
            {
                registry.Remove(className);
            }

            if (!registry.ContainsKey(className))
            {
                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
