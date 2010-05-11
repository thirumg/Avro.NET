using System;
using System.Collections.Generic;
using System.IO;
using Avro.IO;
using System.Reflection.Emit;
using System.Reflection;

namespace Avro
{
    public enum PrefixStyle:byte
    {
        None=0,
        Schema=2,

    }



    public static class Serializer
    {
        enum MethodType:byte
        {
            Encoder=1,
            Decoder=2
        }

        private static readonly Logger log = new Logger();
        static Dictionary<long, DynamicMethod> encoderMethodLookup = new Dictionary<long, DynamicMethod>();
        static Dictionary<long, DynamicMethod> decodeMethodLookup = new Dictionary<long, DynamicMethod>();

        static Dictionary<Type, MethodInfo> encoderMethods;
        static Dictionary<Type, MethodInfo> decoderMethods;

        static Dictionary<Type, MethodInfo> dumpMethods(Type type, MethodType methodType, string Prefix)
        {
            const string PREFIX = "dumpMethods(Type, String) - ";

            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "type = {0}", type);

            Dictionary<Type, MethodInfo> methodLookup = new Dictionary<Type, MethodInfo>();
            MethodInfo[] methods = type.GetMethods();
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "found {0} methods for {1}", methods.Length, type);

            foreach (MethodInfo methodInfo in methods)
            {
                if (!methodInfo.Name.StartsWith(Prefix))
                {
                    if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "skipping method \"{0}\" of {1} because it does not start with prefix \"{2}\"", methodInfo.Name, type, Prefix);
                    continue;
                }

                Type handledType = null;

                switch (methodType)
                {
                    case MethodType.Encoder:
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if (parameters.Length == 0 && parameters[0].ParameterType == typeof(Stream))
                        {
                            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "skipping method \"{0}\" of {1} because it does not take an input parameter other than stream", methodInfo.Name, type);
                            continue;
                        }
                        foreach (ParameterInfo parm in parameters)
                        {
                            if (parm.ParameterType == typeof(Stream))
                                continue;
                            handledType = parm.ParameterType;
                            break;
                        }
                        break;
                    case MethodType.Decoder:
                        handledType = methodInfo.ReturnType;
                        break;
                    default:
                        throw new Exception("Unknown method type " + methodType);
                }

                if (handledType == null)
                {

                    continue;
                }

                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "adding method {0} to lookup for {1} to handle {2}", methodInfo.Name, type, handledType);

                if (methodLookup.ContainsKey(handledType))
                {
                    if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "skipping method \"{0}\" of {1} because type {2} is already defined in lookup", methodInfo.Name, type, handledType);
                    continue;
                }

                methodLookup.Add(handledType, methodInfo);
            }

            return methodLookup;
        }

        static Serializer()
        {
            encoderMethods = dumpMethods(typeof(Avro.IO.Encoder), MethodType.Encoder, "Write");
            decoderMethods = dumpMethods(typeof(Avro.IO.Decoder), MethodType.Decoder, "Read");
        }




        static DynamicMethod getDynamicMethod(MethodType methodType, Schema schema, object data)
        {
            Dictionary<long, DynamicMethod> proxyLookup = null;

            switch (methodType)
            {
                case MethodType.Decoder:
                    proxyLookup = decodeMethodLookup ;
                    break;
                case MethodType.Encoder:
                    proxyLookup = encoderMethodLookup;
                    break;
                default:
                    throw new Exception("Unknown method type " + methodType);
            }

            Type dataType = data.GetType();
            int schemaHashCode = schema.GetHashCode();
            int dataTypeHashCode = dataType.GetHashCode();
            long hashcode = schemaHashCode + dataTypeHashCode;

            DynamicMethod proxy = null;

            if (!proxyLookup.TryGetValue(hashcode, out proxy))
            {
                

                lock (proxyLookup)
                {
                    //Double check to make sure that an earlier lock didn't generate our proxy. 
                    if (!proxyLookup.TryGetValue(hashcode, out proxy))
                    {
                        proxy = generateDynamicMethod(methodType, schema, dataType);
                        proxyLookup.Add(hashcode, proxy);
                    }
                }
            }

            return proxy;
        }

        private static DynamicMethod generateDynamicMethod(MethodType methodType, Schema schema, Type dataType)
        {
            if (schema is PrimitiveSchema)
            {
                if(MethodType.Encoder==methodType)
                    return generatePrimitiveEncoder(dataType);
                else
                    return generatePrimitiveDecoder(dataType);
            }


            return null;
        }

        private static DynamicMethod generatePrimitiveDecoder(Type dataType)
        {
            throw new NotImplementedException();
        }

        static Type[] getMethodArgs(Type type)
        {
            return new Type[] { typeof(Stream), typeof(Encoder), type };
        }

        private static DynamicMethod generatePrimitiveEncoder(Type dataType)
        {
            const string PREFIX = "generatePrimitive(Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Generating encode method for {0}", dataType);
            string methodName = string.Format("Primitive{0}", dataType.Name);

            Type[] args = getMethodArgs(dataType);

            MethodInfo encoderMethodToCall = null;

            if (!encoderMethods.TryGetValue(dataType, out encoderMethodToCall))
            {
                throw new NotSupportedException("Type of " + dataType + " is not supported.");
            }

            DynamicMethod method = new DynamicMethod(methodName, null, args, SerializerType);
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            
            il.Emit(OpCodes.Callvirt, encoderMethodToCall);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);
            return method;
        }

        public static void Serialize(PrefixStyle style, Schema schema, Stream iostr, Encoder encoder, object data)
        {
            if (null == schema) throw new ArgumentNullException("schema", "schema cannot be null.");
            if (null == iostr) throw new ArgumentNullException("iostr", "iostr cannot be null.");

            DynamicMethod proxy = getDynamicMethod(MethodType.Encoder, schema, data);
            proxy.Invoke(null, new object[]{iostr, encoder, data});
        }

        //static readonly Type[] SerializeArgs = new Type[] { typeof(Stream), typeof(Encoder), typeof(object) };
        static readonly Type SerializerType = typeof(Serializer);

        public static void Test(Stream iostr, Encoder encoder, string data)
        {
            encoder.WriteString(iostr, data);
        }
    }

    

    abstract class SerializerProxy
    {
        public abstract void Serialize(Stream iostr, Encoder encoder, object data);
        public abstract object Deserialize(Stream iostr, Decoder decoder);
    }
}
