using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization;
using System.Reflection;

namespace Network
{
    // https://stackoverflow.com/questions/5170333/binaryformatter-deserialize-unable-to-find-assembly-after-ilmerge

    sealed class DeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            String exeAssembly = Assembly.GetExecutingAssembly().FullName;

            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, exeAssembly));

            return typeToDeserialize;
        }
    }
}
