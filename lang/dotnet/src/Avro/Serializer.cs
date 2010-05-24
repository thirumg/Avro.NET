//#define TESTSERIALIZATION

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




        static MethodInfo getMethodInfo(MethodType methodType, Schema schema, Type dataType)
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
                        proxy = generateMethodInfo(methodType, schema, dataType);
                        proxyLookup.Add(hashcode, proxy);
                    }
                }
            }

            return proxy;
        }

        private static MethodInfo generateMethodInfo(MethodType methodType, Schema schema, Type dataType)
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
            else if (schema is RecordSchema)
            {
                if (MethodType.Encoder == methodType)
                    return generateRecordEncoder((RecordSchema)schema, dataType);
                else
                    return generateRecordDecoder((RecordSchema)schema, dataType);
            }
            throw new NotSupportedException("Schema of type " + schema.Type + " is not supported yet.");
        }



        private static readonly Type FieldAttributeType = typeof(FieldAttribute);

        class PropertyHelper
        {
            public PropertyInfo Property { get; set; }
            public FieldAttribute FieldAttribute { get; set; }
            public string Name { get; set; }
        }

        struct RecordState
        {
            public string FieldName;
            public PropertyInfo Property;
            public FieldAttribute FieldAttribute;

            public RecordState(string fieldname, PropertyInfo property, FieldAttribute field)
            {
                this.FieldName = fieldname;
                this.Property = property;
                this.FieldAttribute = field;
            }
        }

        private static MethodInfo generateRecordDecoder(RecordSchema schema, Type dataType)
        {
            const string PREFIX = "generateRecordDecoder(RecordSchema, Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "type = {0}", dataType);

            ConstructorInfo defaultConstructor = dataType.GetConstructor(Type.EmptyTypes);
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "defaultConstructor = {0}", defaultConstructor);
            if (null == defaultConstructor)
                throw new NotSupportedException("Types without a default constructor are not supported.");

            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("DecodeRecord{0}", hashCode);
            Type[] args = getDecoderMethodArgs(dataType);

            ILGenerator il = null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, dataType, args, out il);
            LocalBuilder recordLocal = il.DeclareLocal(dataType);

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, defaultConstructor);
            il.Emit(OpCodes.Stloc_0);

            Dictionary<string, RecordState> fieldLookup = getFieldAttributes(dataType);

            foreach (Field field in schema.Fields)
            {
                RecordState state;

                if (!fieldLookup.TryGetValue(field.Name, out state))
                {
                    if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "field \"{0}\" is not represented in type \"{1}\"", field.Name, dataType);

                    continue;
                }


                il.Emit(OpCodes.Ldloc_0);
                
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);

                MethodInfo propertyMethod = getMethodInfo(MethodType.Decoder, field.Schema, state.Property.PropertyType);
                MethodInfo setMethod= state.Property.GetSetMethod();
                il.Emit(OpCodes.Call, propertyMethod);
                il.Emit(OpCodes.Callvirt, setMethod);
                il.Emit(OpCodes.Nop);

            }

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);






            EmitHelperInstance.Save();
            
            return method;
        }


        private static MethodInfo generateRecordEncoder(RecordSchema schema, Type dataType)
        {
            const string PREFIX = "generateRecordEncoder(RecordSchema, Type) - ";

            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("EncodeRecord{0}", hashCode);
            Type[] args = getEncoderMethodArgs(dataType);

            Dictionary<string, RecordState> recordLookup = getFieldAttributes(dataType);

            ILGenerator il = null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, null, args, out il);
            il.Emit(OpCodes.Nop);



            foreach (Field field in schema.Fields)
            {
                RecordState state;

                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Looking up field \"{0}\"", field.Name);
                if (!recordLookup.TryGetValue(field.Name, out state))
                {
                    if (log.IsWarnEnabled) log.WarnFormat(PREFIX + "Field \"{0}\" was not found.", field.Name);
                }

                MethodInfo propertyMethod = getMethodInfo(MethodType.Encoder, field.Schema, state.Property.PropertyType);
                MethodInfo getPropertyMethod = state.Property.GetGetMethod();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                //il.Emit(OpCodes.Ldarg_3);
                il.Emit(OpCodes.Callvirt, getPropertyMethod);
                il.Emit(OpCodes.Call, propertyMethod);
                il.Emit(OpCodes.Nop);

                //il.Emit(OpCodes.Call
            }
            il.Emit(OpCodes.Ret);

            EmitHelperInstance.Save();

            return method;
        }

        private static Dictionary<string, RecordState> getFieldAttributes(Type dataType)
        {
            //TODO:Comeback and add support for fields. 

            const string PREFIX = "getFieldAttributes(Type) - ";

            Dictionary<string, RecordState> recordLookup = new Dictionary<string, RecordState>(StringComparer.OrdinalIgnoreCase);
            PropertyInfo[] properties = dataType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "checking property \"{0}\"", property.Name);

                FieldAttribute[] attributes = property.GetCustomAttributes(FieldAttributeType, true) as FieldAttribute[];
                if (attributes.Length == 0)
                {
                    if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "No attributes defined for \"{0}\"", property.Name);
                    continue;
                }

                FieldAttribute attribute = attributes[0];

                if (string.IsNullOrEmpty(attribute.Name))
                {
                    if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Attribute Name is null, so using property name \"{0}\"", property.Name);
                    attribute.Name = property.Name;
                }

                RecordState state = new RecordState(attribute.Name, property, attribute);
                try
                {
                    recordLookup.Add(state.FieldName, state);
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled) log.Error("Exception thrown while adding key \"" + state.FieldName + "\"", ex);
                    throw;
                }
            }
            return recordLookup;
        }

        private static MethodInfo generateArrayDecoder(ArraySchema schema, Type dataType)
        {
            const string PREFIX = "generateArrayEncoder(ArraySchema, Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Generating encode method for {0} schema = {1}", dataType, schema);

            validateArrayType(dataType);
            Type elementType = dataType.GetElementType();

            MethodInfo valueEncodeMethod = getMethodInfo(MethodType.Decoder, schema.ItemSchema, elementType);
            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("DecodeArray{0}", hashCode);

            Type[] args = getDecoderMethodArgs(dataType);
            ILGenerator il = null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, dataType, args, out il);
            LocalBuilder localLength = il.DeclareLocal(typeof(long));
            LocalBuilder localValues = il.DeclareLocal(dataType);
            LocalBuilder localI = il.DeclareLocal(typeof(long));
            LocalBuilder localTempHolder = il.DeclareLocal(dataType);
            LocalBuilder localFlag = il.DeclareLocal(typeof(bool));
            Label label0 = il.DefineLabel();
            Label label1 = il.DefineLabel();
            Label label2 = il.DefineLabel();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DecoderHelper.ReadLong);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Conv_Ovf_I);
            il.Emit(OpCodes.Newarr, elementType);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Conv_I8);
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Br_S, label0);
            il.MarkLabel(label1);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Conv_Ovf_I);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, valueEncodeMethod);
            il.Emit(OpCodes.Stelem_Ref);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Conv_I8);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_2);
            il.MarkLabel(label0);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Clt);
            il.Emit(OpCodes.Stloc_S, localFlag);
            il.Emit(OpCodes.Ldloc_S, localFlag);
            il.Emit(OpCodes.Brtrue_S, label1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Br_S, label2);
            il.MarkLabel(label2);
            il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Ret);

            EmitHelperInstance.Save();

            return method;
        }



        static void validateArrayType(Type type)
        {
            if (!type.IsArray)
                throw new NotSupportedException(type + " is not supported for array serialization / deserialization");


        }

        private static readonly MethodInfo Array_GetLongLength = typeof(Array).GetMethod("get_LongLength");

        private static MethodInfo generateArrayEncoder(ArraySchema schema, Type dataType)
        {
            const string PREFIX = "generateArrayEncoder(ArraySchema, Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Generating encode method for {0} schema = {1}", dataType, schema);

            validateArrayType(dataType);
            Type elementType = dataType.GetElementType();

            MethodInfo valueEncodeMethod = getMethodInfo(MethodType.Encoder, schema.ItemSchema, elementType);


            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("EncodeArray{0}", hashCode);


            Type[] args = getEncoderMethodArgs(dataType);
            ILGenerator il = null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, null, args, out il);
            LocalBuilder localI = il.DeclareLocal(typeof(long));
            LocalBuilder localFlag = il.DeclareLocal(typeof(bool));
            Label label0 = il.DefineLabel();
            Label label1 = il.DefineLabel();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, EncoderHelper.WriteArrayStart);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, Array_GetLongLength);
            il.Emit(OpCodes.Callvirt, EncoderHelper.WriteLong);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Conv_I8);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Br_S, label1);
            il.MarkLabel(label0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Conv_Ovf_I);
            il.Emit(OpCodes.Ldelem_Ref);
            il.Emit(OpCodes.Call, valueEncodeMethod);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Conv_I8);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_0);
            il.MarkLabel(label1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, Array_GetLongLength);
            il.Emit(OpCodes.Clt);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Brtrue_S, label0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, EncoderHelper.WriteArrayEnd);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);




            EmitHelperInstance.Save();

            return method;
        }

        static class EncoderHelper
        {
            private static readonly Logger log;
            public static readonly MethodInfo WriteMapStart;
            public static readonly MethodInfo SetItemCount;
            public static readonly MethodInfo StartItem;
            public static readonly MethodInfo WriteString;
            public static readonly MethodInfo WriteMapEnd;
            public static readonly MethodInfo WriteArrayStart;
            public static readonly MethodInfo WriteArrayEnd;
            public static readonly MethodInfo WriteLong;

            static EncoderHelper()
            {
                const string PREFIX = "..ctor() - ";
                log = new Logger();

                WriteMapStart = EncoderType.GetMethod("WriteMapStart");
                SetItemCount = EncoderType.GetMethod("SetItemCount");
                StartItem = EncoderType.GetMethod("StartItem");
                WriteString = EncoderType.GetMethod("WriteString");
                WriteMapEnd = EncoderType.GetMethod("WriteMapEnd");
                WriteArrayStart = EncoderType.GetMethod("WriteArrayStart");
                WriteArrayEnd = EncoderType.GetMethod("WriteArrayEnd");
                WriteLong = EncoderType.GetMethod("WriteLong");

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(PREFIX + "WriteMapStart = {0}", WriteMapStart);
                    log.DebugFormat(PREFIX + "SetItemCount = {0}", SetItemCount);
                    log.DebugFormat(PREFIX + "StartItem = {0}", StartItem);
                    log.DebugFormat(PREFIX + "WriteString = {0}", WriteString);
                    log.DebugFormat(PREFIX + "WriteMapEnd = {0}", WriteMapEnd);
                    log.DebugFormat(PREFIX + "WriteArrayStart = {0}", WriteArrayStart);
                    log.DebugFormat(PREFIX + "WriteArrayEnd = {0}", WriteArrayEnd);
                    log.DebugFormat(PREFIX + "WriteLong = {0}", WriteLong);
                }

            }




        }
        static class DecoderHelper
        {
            private static readonly Logger log;
            public static readonly Type DecoderType;
            public static readonly MethodInfo ReadMapStart;
            public static readonly MethodInfo ReadString;
            public static readonly MethodInfo ReadLong;

            static DecoderHelper()
            {
                const string PREFIX = "..ctor() - ";
                log = new Logger();
                DecoderType = typeof(Decoder);
                ReadMapStart = DecoderType.GetMethod("ReadMapStart");
                ReadString = DecoderType.GetMethod("ReadString");
                ReadLong = DecoderType.GetMethod("ReadLong");
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(PREFIX + "DecoderType = {0}", DecoderType);
                    log.DebugFormat(PREFIX + "ReadMapStart = {0}", ReadMapStart);
                    log.DebugFormat(PREFIX + "ReadString = {0}", ReadString);
                    log.DebugFormat(PREFIX + "ReadLong = {0}", ReadLong);
                }
            }


        }

        static class TypeHelper
        {
            static readonly Logger log = new Logger();

            public static Type CreateGenericType(Type genericType, params Type[] args)
            {
                const string PREFIX = "CreateGenericType(Type, Type[]) - ";
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(PREFIX + "genericType = {0}", genericType);
                    int index = 0;
                    foreach (Type arg in args)
                        log.DebugFormat(PREFIX + "arg{0} = {1}", index++, arg);
                }

                Type returnType = genericType.MakeGenericType(args);
                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "returnType = {0}", returnType);
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
            public Type IEnumerableType { get; private set; }
            public Type KeyValuePairType { get; private set; }
            public Type IEnumeratorType { get; private set; }
            public MethodInfo IEnumerableGetEnumerator { get; private set; }
            public MethodInfo IEnumeratorGetCurrent { get; private set; }
            public MethodInfo IEnumeratorMoveNext { get; private set; }
            public Type ICollectionType{ get; private set; }
            public MethodInfo ICollectionGetCount { get; private set; }
            public MethodInfo KeyValuePairGetKey { get; private set; }
            public MethodInfo KeyValuePairGetValue { get; private set; }
            public Type KeyType { get; private set; }
            public Type ValueType { get; private set; }


            public DictionaryHelper(Type keyType, Type valueType)
            {
                const string PREFIX = "ctor(Type, Type) - ";

                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "arg0 = {0}, arg1 = {1}", keyType, valueType);
                KeyType = keyType;
                ValueType = valueType;
                IDictionaryType = TypeHelper.CreateGenericType(typeof(IDictionary<,>), keyType, valueType);
                DictionaryType = TypeHelper.CreateGenericType(typeof(Dictionary<,>), keyType, valueType);
                DictionaryConstructor = DictionaryType.GetConstructor(Type.EmptyTypes);
                DictionaryAdd = IDictionaryType.GetMethod("Add");
                KeyValuePairType = TypeHelper.CreateGenericType(typeof(KeyValuePair<,>), keyType, valueType);
                IEnumerableType = TypeHelper.CreateGenericType(typeof(IEnumerable<>), KeyValuePairType);
                IEnumerableGetEnumerator = IEnumerableType.GetMethod("GetEnumerator");
                IEnumeratorType = TypeHelper.CreateGenericType(typeof(IEnumerator<>), KeyValuePairType);
                IEnumeratorGetCurrent = IEnumeratorType.GetMethod("get_Current");

                IEnumeratorMoveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
                ICollectionType = TypeHelper.CreateGenericType(typeof(ICollection<>), KeyValuePairType);
                ICollectionGetCount = ICollectionType.GetMethod("get_Count");

                KeyValuePairGetKey = KeyValuePairType.GetMethod("get_Key");
                KeyValuePairGetValue = KeyValuePairType.GetMethod("get_Value");
                

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(PREFIX + "KeyType = {0}", KeyType);
                    log.DebugFormat(PREFIX + "ValueType = {0}", ValueType);
                    log.DebugFormat(PREFIX + "IDictionaryType = {0}", IDictionaryType);
                    log.DebugFormat(PREFIX + "DictionaryType = {0}", DictionaryType);
                    log.DebugFormat(PREFIX + "DictionaryConstructor = {0}", DictionaryConstructor);
                    log.DebugFormat(PREFIX + "DictionaryAdd = {0}", DictionaryAdd);
                    log.DebugFormat(PREFIX + "KeyValuePairType = {0}", KeyValuePairType);
                    log.DebugFormat(PREFIX + "IEnumerableType = {0}", IEnumerableType);
                    log.DebugFormat(PREFIX + "IEnumerableGetEnumerator = {0}", IEnumerableGetEnumerator);
                    log.DebugFormat(PREFIX + "IEnumeratorMoveNext = {0}", IEnumeratorMoveNext);
                    log.DebugFormat(PREFIX + "ICollectionType = {0}", ICollectionType);
                    log.DebugFormat(PREFIX + "ICollectionGetCount = {0}", ICollectionGetCount);
                    log.DebugFormat(PREFIX + "KeyValuePairGetKey = {0}", KeyValuePairGetKey);
                    log.DebugFormat(PREFIX + "KeyValuePairGetValue = {0}", KeyValuePairGetValue);
                    
                }
            }

        }

        class EmitHelper
        {
            private static readonly Logger log = new Logger();

#if(DEBUG)
            private AssemblyName _AssemblyName;
            private AssemblyBuilder _AssemblyBuilder;
            private ModuleBuilder _ModuleBuilder;
            private TypeBuilder _TypeBuilder;
#endif

            public EmitHelper()
            {
#if(DEBUG)
                _AssemblyName = new AssemblyName("TestAssembly");
                _AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_AssemblyName, AssemblyBuilderAccess.RunAndSave);
                _ModuleBuilder = _AssemblyBuilder.DefineDynamicModule(_AssemblyName.Name + ".dll");
                _TypeBuilder = _ModuleBuilder.DefineType("GenerateEncoderMap.Test");
#endif
            }

            public MethodInfo CreateMethod(string name, Type returnType, Type[] args, out ILGenerator il)
            {
                const string PREFIX = "CreateMethod(string, Type, Type[], out ILGenerator) - ";
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(PREFIX + "name = \"{0}\" returnType = \"{0}\"", name, returnType);
                    int index=0;
                    foreach (Type arg in args)
                        log.DebugFormat("arg{0} == {1}", index++, arg);
                }


                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Creating method \"{0}\"", name);
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");

                MethodInfo method = null;

#if(TESTSERIALIZATION)
                MethodBuilder builder = _TypeBuilder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static, returnType, args);
                il = builder.GetILGenerator();
                method = builder;
#else
                DynamicMethod dmethod = new DynamicMethod(name, returnType, args, SerializerType);
                il = dmethod.GetILGenerator();
                method = dmethod;
#endif
                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "returning method {0}", method);
                return method;
            }






            public void Save()
            {
#if(TESTSERIALIZATION)
                const string PREFIX = "Save() - ";
                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "saving type.");
                Type type = _TypeBuilder.CreateType();
                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Created type {0}", type);

                _AssemblyBuilder.Save(_AssemblyName.Name + ".dll");
#endif
            }
        }

        private static readonly EmitHelper EmitHelperInstance = new EmitHelper();


        private static MethodInfo generateMapDecoder(MapSchema schema, Type dataType)
        {
            const string PREFIX = "generateMapDecoder(MapSchema, Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "type = {0}, schema = {1}", dataType, schema);

            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("DecodeMap{0}", hashCode);

            Type mapValueType = getMapValueType(dataType);
            DictionaryHelper dictHelper = new DictionaryHelper(typeof(string), mapValueType);

            Type[] args = getDecoderMethodArgs(dictHelper.IDictionaryType);

            MethodInfo valueDecoderMethod = getMethodInfo(MethodType.Decoder, schema.ValueSchema, mapValueType);

            ILGenerator il=null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, dictHelper.IDictionaryType, args, out il);

            LocalBuilder localLength = il.DeclareLocal(typeof(long));
            LocalBuilder localMap = il.DeclareLocal(dictHelper.IDictionaryType);
            LocalBuilder localIndex = il.DeclareLocal(typeof(long));
            //LocalBuilder localTest = il.DeclareLocal(dictHelper.IDictionaryType);
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
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, valueDecoderMethod);
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
            //il.Emit(OpCodes.Ldloc_1);
            //il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Br_S, L_003e);
            il.MarkLabel(L_003e);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);

            EmitHelperInstance.Save();

            return method;
        }

        private static long getHashCode(Type dataType)
        {
            //const string PREFIX = "getHashCode(Type) - ";
            long dataTypeHashCode = dataType.GetHashCode();
            //if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "dataTypeHashCode = {0}", dataTypeHashCode);
            return Math.Abs(dataTypeHashCode);
        }

        private static long getHashCode(Schema schema, Type dataType)
        {
            //const string PREFIX = "getHashCode(Schema, Type) - ";
            long schemaHashCode = schema.GetHashCode();
            long dataTypeHashCode = dataType.GetHashCode();
            //if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "schemaHashCode = {0}, dataTypeHashCode = {1}", schemaHashCode, dataTypeHashCode);
            return Math.Abs(schemaHashCode) + Math.Abs(dataTypeHashCode);
        }

        private static readonly Type EncoderType = typeof(Encoder);



        private static readonly MethodInfo methodDispose = typeof(IDisposable).GetMethod("Dispose");

        private static MethodInfo generateMapEncoder(MapSchema schema, Type dataType)
        {
            const string PREFIX = "generateMapEncoder(MapSchema, Type) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "Generating encode method for {0} schema = {1}", dataType, schema);

            if (!dataType.IsGenericType)
            {
                throw new NotSupportedException("Only types based on System.Collections.Generic.IDictionary are supported.");
            }

            Type mapValueType = getMapValueType(dataType);
            DictionaryHelper dictHelper = new DictionaryHelper(typeof(string), mapValueType);
            
            MethodInfo valueEncodeMethod = getMethodInfo(MethodType.Encoder, schema.ValueSchema, dictHelper.ValueType);



            long hashCode = getHashCode(schema, dataType);
            string methodName = string.Format("EncodeMap{0}", hashCode);

            Type genericTypeDef = dataType.GetGenericTypeDefinition();
            Type[] args = getEncoderMethodArgs(dataType);

            ILGenerator il = null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, null, args, out il);
            LocalBuilder localEntry = il.DeclareLocal(dictHelper.KeyValuePairType);
            LocalBuilder localEnumerator = il.DeclareLocal(dictHelper.IEnumeratorType);
            LocalBuilder localFlag = il.DeclareLocal(typeof(bool));

            Label labelGetCurrent = il.DefineLabel();
            Label labelEndFinally = il.DefineLabel();
            Label labelMoveNext = il.DefineLabel();

            
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, EncoderHelper.WriteMapStart);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, dictHelper.ICollectionGetCount);
            il.Emit(OpCodes.Callvirt, EncoderHelper.SetItemCount);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, dictHelper.IEnumerableGetEnumerator);
            il.Emit(OpCodes.Stloc_1);
            Label labelExceptionBlock = il.BeginExceptionBlock();
            il.Emit(OpCodes.Br_S, labelMoveNext);
            il.MarkLabel(labelGetCurrent);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, dictHelper.IEnumeratorGetCurrent);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, EncoderHelper.StartItem);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloca_S, localEntry);
            il.Emit(OpCodes.Call, dictHelper.KeyValuePairGetKey);
            il.Emit(OpCodes.Callvirt, EncoderHelper.WriteString);
            il.Emit(OpCodes.Nop);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloca_S, localEntry);
            il.Emit(OpCodes.Call, dictHelper.KeyValuePairGetValue);
            il.Emit(OpCodes.Call, valueEncodeMethod);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.MarkLabel(labelMoveNext);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, dictHelper.IEnumeratorMoveNext);
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
            il.Emit(OpCodes.Callvirt, EncoderHelper.WriteMapEnd);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);

            


            //Type type = typeBuilder.TypeHelper.createType();
            //assbuilder.Save(assName.Name + ".dll");

            return method;
            //return null;// method;
        }

        private static Type getMapValueType(Type dataType)
        {
            Type[] genericArgs = dataType.GetGenericArguments();
            if (genericArgs.Length != 2)
                throw new NotSupportedException("Only types based on System.Collections.Generic.IDictionary are supported.");
            if (typeof(string) != genericArgs[0])
                throw new NotSupportedException("Only string keys are supported.");

            Type mapValueType = genericArgs[1];
            return mapValueType;
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
            ILGenerator il = null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, dataType, args, out il);
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



        private static MethodInfo generatePrimitiveEncoder(Type dataType)
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
            ILGenerator il = null;
            MethodInfo method = EmitHelperInstance.CreateMethod(methodName, null, args, out il);
            
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

            MethodInfo proxy = getMethodInfo(MethodType.Encoder, schema, dataType);
            proxy.Invoke(null, new object[]{iostr, encoder, data});
        }

        public static void TestEncodeString(Stream iostr, Encoder encoder, string data)
        {
            encoder.WriteString(iostr, data);
        }
        public static string TestDecodeString(Stream iostr, Decoder decoder)
        {
            return decoder.ReadString(iostr);
        }

        public static T Deserialize<T>(PrefixStyle prefixStyle, Schema schema, Stream iostr, Decoder decoder)
        {
            return (T)Deserialize(prefixStyle, schema, iostr, decoder, typeof(T));
        }

        public static object Deserialize(PrefixStyle prefixStyle, Schema schema, Stream iostr, Decoder decoder, Type type)
        {
            if (null == schema) throw new ArgumentNullException("schema", "schema cannot be null.");
            if (null == iostr) throw new ArgumentNullException("iostr", "iostr cannot be null.");

            //return TestDecodeMap(iostr, decoder);

            MethodInfo proxy = getMethodInfo(MethodType.Decoder, schema, type);
            
            object value = proxy.Invoke(null, new object[] { iostr, decoder });

            return value;
        }
    }
}
