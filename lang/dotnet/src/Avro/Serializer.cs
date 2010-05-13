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
        private static readonly Type SerializerType = typeof(Serializer);

        static Dictionary<long, MethodInfo> encoderMethodLookup = new Dictionary<long, MethodInfo>();
        static Dictionary<long, MethodInfo> decodeMethodLookup = new Dictionary<long, MethodInfo>();

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




        static MethodInfo getDynamicMethod(MethodType methodType, Schema schema, Type dataType)
        {
            Dictionary<long, MethodInfo> proxyLookup = null;

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

            long hashcode = getHashCode(schema, dataType);

            MethodInfo proxy = null;

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

        private static MethodInfo generateDynamicMethod(MethodType methodType, Schema schema, Type dataType)
        {
            if (schema is PrimitiveSchema)
            {
                if(MethodType.Encoder==methodType)
                    return generatePrimitiveEncoder(dataType);
                else
                    return generatePrimitiveDecoder(dataType);
            }
            else if (schema is MapSchema)
            {
                if (MethodType.Encoder == methodType)
                    return generateMapEncoder((MapSchema)schema, dataType);
                else
                    return generateMapDecoder((MapSchema)schema, dataType);

            }
            else if (schema is ArraySchema)
            {
                if (MethodType.Encoder == methodType)
                    return generateArrayEncoder((ArraySchema)schema, dataType);
                else
                    return generateArrayDecoder((ArraySchema)schema, dataType);
            }
            throw new NotSupportedException("Schema of type " + schema.Type + " is not supported yet.");
        }

        private static DynamicMethod generateArrayDecoder(ArraySchema arraySchema, Type dataType)
        {

            throw new NotImplementedException();
        }

        private static DynamicMethod generateArrayEncoder(ArraySchema arraySchema, Type dataType)
        {
            if (!dataType.IsArray)
                throw new NotSupportedException();
            throw new NotImplementedException();
        }


        public class DecoderHelper
        {
            private static readonly Logger log;
            public static readonly Type DecoderType;
            public static readonly MethodInfo ReadMapStart;
            public static readonly MethodInfo ReadString;

            static DecoderHelper()
            {
                const string PREFIX = "..ctor() - ";
                log = new Logger();
                DecoderType = typeof(Decoder);
                ReadMapStart = DecoderType.GetMethod("ReadMapStart");
                ReadString = DecoderType.GetMethod("ReadString");
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(PREFIX + "DecoderType = {0}", DecoderType);
                    log.DebugFormat(PREFIX + "ReadMapStart = {0}", ReadMapStart);
                    log.DebugFormat(PREFIX + "ReadString = {0}", ReadString);
                }
            }


        }

        static class TypeHelper
        {
            static readonly Logger log = new Logger();

            public static Type CreateType(Type genericType, params Type[] args)
            {
                const string PREFIX = "CreateType(Type, Type[]) - ";
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(PREFIX + "genericType = {0}", genericType);
                    int index = 0;
                    foreach (Type arg in args)
                        log.DebugFormat("arg{0} = {1}", index++, arg);
                }

                Type returnType = genericType.MakeGenericType(args);
                if (log.IsDebugEnabled) log.DebugFormat("returnType = {0}", returnType);
                return returnType;
            }


        }

        class DictionaryHelper
        {
            private static readonly Logger log = new Logger();

            public Type IDictionaryType { get; private set; }
            public Type DictionaryType { get; private set; }
            public ConstructorInfo DictionaryConstructor { get; private set; }
            public MethodInfo DictionaryAdd { get; private set; }


            public DictionaryHelper(Type arg0, Type arg1)
            {
                const string PREFIX = "ctor(Type, Type) - ";

                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "arg0 = {0}, arg1 = {1}", arg0, arg1);

                IDictionaryType = TypeHelper.CreateType(typeof(IDictionary<,>), arg0, arg1);
                DictionaryType = TypeHelper.CreateType(typeof(Dictionary<,>), arg0, arg1);
                DictionaryConstructor = DictionaryType.GetConstructor(Type.EmptyTypes);
                DictionaryAdd = IDictionaryType.GetMethod("Add");


                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("IDictionaryType = {0}", IDictionaryType);
                    log.DebugFormat("DictionaryType = {0}", DictionaryType);
                    log.DebugFormat("DictionaryConstructor = {0}", DictionaryConstructor);
                    log.DebugFormat("DictionaryAdd = {0}", DictionaryAdd);
                }
            }

        }

        private static DynamicMethod generateMapDecoder(MapSchema schema, Type dataType)
        {
            const string PREFIX = "generateMapDecoder(MapSchema, Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "dataType = {0}, schema = {1}", dataType, schema);

            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("DecodeMap{0}", hashCode);

            DictionaryHelper dictHelper = new DictionaryHelper(typeof(string), typeof(string));

            Type[] args = getDecoderMethodArgs(dictHelper.IDictionaryType);

            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "creating method {0}", methodName);
            DynamicMethod method = new DynamicMethod(methodName, null, args, SerializerType);
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "method = {0}", method);

            




            ILGenerator il = method.GetILGenerator();
            LocalBuilder localLength = il.DeclareLocal(typeof(long));
            LocalBuilder localMap = il.DeclareLocal(dictHelper.IDictionaryType);
            LocalBuilder localIndex = il.DeclareLocal(typeof(long));
            LocalBuilder localTest = il.DeclareLocal(dictHelper.IDictionaryType);
            LocalBuilder localFlag = il.DeclareLocal(typeof(bool));
            Label L_0030 = il.DefineLabel();
            Label L_0014 = il.DefineLabel();
            Label L_003e = il.DefineLabel();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DecoderHelper.ReadMapStart);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, dictHelper.DictionaryConstructor);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Conv_I8);
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Br_S, L_0030);
            il.MarkLabel(L_0014);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DecoderHelper.ReadString);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DecoderHelper.ReadString);
            il.Emit(OpCodes.Callvirt, dictHelper.DictionaryAdd);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Conv_I8);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_2);
            il.MarkLabel(L_0030);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Clt);
            il.Emit(OpCodes.Stloc_S, localFlag);
            il.Emit(OpCodes.Ldloc_S, localFlag);
            il.Emit(OpCodes.Brtrue_S, L_0014);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Br_S, L_003e);
            il.MarkLabel(L_003e);
            il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Ret);

            return method;
        }

        private static long getHashCode(Type dataType)
        {
            const string PREFIX = "getHashCode(Type) - ";
            long dataTypeHashCode = dataType.GetHashCode();
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "dataTypeHashCode = {0}", dataTypeHashCode);
            return Math.Abs(dataTypeHashCode);
        }

        private static long getHashCode(Schema schema, Type dataType)
        {
            const string PREFIX = "getHashCode(Schema, Type) - ";
            long schemaHashCode = schema.GetHashCode();
            long dataTypeHashCode = dataType.GetHashCode();
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "schemaHashCode = {0}, dataTypeHashCode = {1}", schemaHashCode, dataTypeHashCode);
            return Math.Abs(schemaHashCode) + Math.Abs(dataTypeHashCode);
        }

        private static readonly Type EncoderType = typeof(Encoder);
        




        private static DynamicMethod generateMapEncoder(MapSchema schema, Type dataType)
        {
            const string PREFIX = "generateMapEncoder(MapSchema, Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Generating encode method for {0} schema = {1}", dataType, schema);

            if (!dataType.IsGenericType)
            {
                throw new NotSupportedException("Only types based on System.Collections.Generic.IDictionary are supported.");
            }

            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("EncodeMap{0}", hashCode);
            Type[] genericArgs = dataType.GetGenericArguments();
            if(genericArgs.Length!=2)
                throw new NotSupportedException("Only types based on System.Collections.Generic.IDictionary are supported.");

            if (typeof(string) != genericArgs[0])
                throw new NotSupportedException("Only string keys are supported.");
            Type genericTypeDef = dataType.GetGenericTypeDefinition();
            Type[] args = getEncoderMethodArgs(dataType);

            
            

            
            Type keyValuePairType = TypeHelper.CreateType(typeof(KeyValuePair<,>), typeof(string), typeof(string));
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "keyValuePairType = {0}", keyValuePairType);
            Type ienumerableType = TypeHelper.CreateType(typeof(IEnumerable<>), keyValuePairType);
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "ienumerableType = {0}", ienumerableType);
            Type enumeratorType = TypeHelper.CreateType(typeof(IEnumerator<>), keyValuePairType);
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "enumeratorType = {0}", enumeratorType);
            MethodInfo methodGetEnumerator = ienumerableType.GetMethod("GetEnumerator");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "methodGetEnumerator = {0}", methodGetEnumerator);


            MethodInfo methodGetCurrent = enumeratorType.GetMethod("get_Current");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "get_Current = {0}", methodGetCurrent);

            MethodInfo methodMoveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "methodMoveNext = {0}", methodMoveNext);

            Type icollectionType = TypeHelper.CreateType(typeof(ICollection<>), keyValuePairType);
            MethodInfo methodGetCount = icollectionType.GetMethod("get_Count");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "get_Count = {0}", methodGetCount);

            

            MethodInfo methodWriteMapStart = EncoderType.GetMethod("WriteMapStart");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "WriteMapStart = {0}", methodWriteMapStart);
            MethodInfo methodSetItemCount = EncoderType.GetMethod("SetItemCount");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "SetItemCount = {0}", methodSetItemCount);
            MethodInfo methodStartItem = EncoderType.GetMethod("StartItem");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "StartItem = {0}", methodStartItem);
            MethodInfo methodget_Key = keyValuePairType.GetMethod("get_Key");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "methodget_Key = {0}", methodget_Key);
            MethodInfo methodget_Value = keyValuePairType.GetMethod("get_Value");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "methodget_Value = {0}", methodget_Value);
            MethodInfo methodWriteString = EncoderType.GetMethod("WriteString");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "methodWriteString = {0}", methodWriteString);
            MethodInfo methodWriteMapEnd = EncoderType.GetMethod("WriteMapEnd");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "methodWriteMapEnd = {0}", methodWriteMapEnd);

            MethodInfo methodDispose = typeof(IDisposable).GetMethod("Dispose");
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "methodDispose = {0}", methodDispose);

            //AssemblyName assName = new AssemblyName("TestAssembly");

            //AssemblyBuilder assbuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Save);
            //ModuleBuilder modbuilder = assbuilder.DefineDynamicModule(assName.Name + ".dll");
            //TypeBuilder typeBuilder = modbuilder.DefineType("GenerateEncoderMap.Test");

            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "creating method {0}", methodName);
            DynamicMethod method = new DynamicMethod(methodName, null, args, SerializerType);
            
            //MethodBuilder method = typeBuilder.DefineMethod(methodName, MethodAttributes.Public|MethodAttributes.Static,null, args);
            
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "method = {0}", method);
            
            ILGenerator il = method.GetILGenerator();
            
            LocalBuilder localEntry = il.DeclareLocal(keyValuePairType);
            //localEntry.SetLocalSymInfo("entry");
            LocalBuilder localEnumerator = il.DeclareLocal(enumeratorType);
            LocalBuilder localFlag = il.DeclareLocal(typeof(bool));

            Label labelGetCurrent = il.DefineLabel();
            Label labelEndFinally = il.DefineLabel();
            Label labelMoveNext = il.DefineLabel();

            
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, methodWriteMapStart);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, methodGetCount);
            il.Emit(OpCodes.Callvirt, methodSetItemCount);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, methodGetEnumerator);
            il.Emit(OpCodes.Stloc_1);
            Label labelExceptionBlock = il.BeginExceptionBlock();
            il.Emit(OpCodes.Br_S, labelMoveNext);
            il.MarkLabel(labelGetCurrent);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, methodGetCurrent);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, methodStartItem);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloca_S, localEntry);
            il.Emit(OpCodes.Call, methodget_Key);
            il.Emit(OpCodes.Callvirt, methodWriteString);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloca_S, localEntry);
            il.Emit(OpCodes.Call, methodget_Value);
            il.Emit(OpCodes.Callvirt, methodWriteString);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.MarkLabel(labelMoveNext);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, methodMoveNext);
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Brtrue_S, labelGetCurrent);
            //il.Emit(OpCodes.Leave_S, labelExceptionBlock);
        
            il.BeginFinallyBlock();
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Brtrue_S, labelEndFinally);//labelMoveNext);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, methodDispose);
            
            il.Emit(OpCodes.Nop);
            //il.Emit(OpCodes.Endfinally);
            il.MarkLabel(labelEndFinally);
            il.EndExceptionBlock();
            //il.MarkLabel(lblEndFinally);
            il.Emit(OpCodes.Nop);
            

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, methodWriteMapEnd);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);

            //Type type = typeBuilder.TypeHelper.createType();
            //assbuilder.Save(assName.Name + ".dll");

            return method;
            //return null;// method;
        }

        static Type[] getEncoderMethodArgs(Type type)
        {
            return new Type[] { typeof(Stream), typeof(Encoder), type };
        }
        static Type[] getDecoderMethodArgs(Type type)
        {
            return new Type[] { typeof(Stream), typeof(Decoder)};
        }

        private static MethodInfo generatePrimitiveDecoder(Type dataType)
        {
            const string PREFIX = "generatePrimitiveDecoder(Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Generating encode method for {0}", dataType);
            string methodName = string.Format("DecodePrimitive{0}", dataType.Name);

            Type[] args = getDecoderMethodArgs(dataType);

            MethodInfo decoderMethodToCall = null;

            if (!decoderMethods.TryGetValue(dataType, out decoderMethodToCall))
            {
                throw new NotSupportedException("Type of " + dataType + " is not supported.");
            }

            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "using decoder method of {0}", decoderMethodToCall);

            DynamicMethod method = new DynamicMethod(methodName, dataType, args, SerializerType);
            
            ILGenerator il = method.GetILGenerator();
            LocalBuilder builder = il.DeclareLocal(dataType);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, decoderMethodToCall);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Br_S, builder);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return method;
        }



        private static DynamicMethod generatePrimitiveEncoder(Type dataType)
        {
            const string PREFIX = "generatePrimitiveEncoder(Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Generating encode method for {0}", dataType);
            string methodName = string.Format("EncodePrimitive{0}", dataType.Name);

            Type[] args = getEncoderMethodArgs(dataType);

            MethodInfo encoderMethodToCall = null;

            if (!encoderMethods.TryGetValue(dataType, out encoderMethodToCall))
            {
                throw new NotSupportedException("Type of " + dataType + " is not supported.");
            }
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "using encoder method of {0}", encoderMethodToCall);
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
            Type dataType = data.GetType();



            MethodInfo proxy = getDynamicMethod(MethodType.Encoder, schema, dataType);
            proxy.Invoke(null, new object[]{iostr, encoder, data});
        }

        //static readonly Type[] SerializeArgs = new Type[] { typeof(Stream), typeof(Encoder), typeof(object) };
       

        public static void TestEncodeString(Stream iostr, Encoder encoder, string data)
        {
            encoder.WriteString(iostr, data);
        }
        public static string TestDecodeString(Stream iostr, Decoder decoder)
        {
            return decoder.ReadString(iostr);
        }

        static void encodeMap<T>(Stream iostr, Encoder encoder, IDictionary<string, T> data)
        {



        }



        public static void TestEncodeMap(Stream iostr, Encoder encoder, IDictionary<string, string> data)
        {
            encoder.WriteMapStart(iostr);
            encoder.SetItemCount(iostr, data.Count);
            foreach (KeyValuePair<string, string> entry in data)
            {
                encoder.StartItem(iostr);
                encoder.WriteString(iostr, entry.Key);
                encoder.WriteString(iostr, entry.Value);
            }
            encoder.WriteMapEnd(iostr);
        }
        public static IDictionary<string, string> TestDecodeMap(Stream iostr, Decoder decoder)
        {
            long length = decoder.ReadMapStart(iostr);
            IDictionary<string, string> map = new Dictionary<string, string>();
            for (long i = 0; i < length; i++)
            {
                map.Add(
                    decoder.ReadString(iostr),
                    decoder.ReadString(iostr)
                    );
            }
            
            return map;
        }


        public static T Deserialize<T>(PrefixStyle prefixStyle, Schema schema, MemoryStream iostr, Decoder decoder)
        {
            return (T)Deserialize(prefixStyle, schema, iostr, decoder, typeof(T));
        }

        public static object Deserialize(PrefixStyle prefixStyle, Schema schema, MemoryStream iostr, Decoder decoder, Type type)
        {
            if (null == schema) throw new ArgumentNullException("schema", "schema cannot be null.");
            if (null == iostr) throw new ArgumentNullException("iostr", "iostr cannot be null.");

            return TestDecodeMap(iostr, decoder);

            MethodInfo proxy = getDynamicMethod(MethodType.Decoder, schema, type);
            return proxy.Invoke(null, new object[] { iostr, decoder });
        }
    }

    

    abstract class SerializerProxy
    {
        public abstract void Serialize(Stream iostr, Encoder encoder, object data);
        public abstract object Deserialize(Stream iostr, Decoder decoder);
    }
}
