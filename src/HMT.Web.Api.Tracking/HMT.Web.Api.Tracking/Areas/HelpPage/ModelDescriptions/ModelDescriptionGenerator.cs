using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace HMT.Web.Api.Tracking.Areas.HelpPage.ModelDescriptions {
    /// <summary>
    /// Generates model descriptions for given types.
    /// </summary>
    public class ModelDescriptionGenerator {
        /// <summary>
        /// 
        /// </summary>
        private readonly IDictionary<Type, Func<object, string>> AnnotationTextGenerator = new Dictionary<Type, Func<object, string>>
        {
            { typeof(RequiredAttribute), a => "Required" },
            { typeof(RangeAttribute), a =>
                {
                    RangeAttribute range = (RangeAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Range: inclusive between {0} and {1}", range.Minimum, range.Maximum);
                }
            },
            { typeof(MaxLengthAttribute), a =>
                {
                    MaxLengthAttribute maxLength = (MaxLengthAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Max length: {0}", maxLength.Length);
                }
            },
            { typeof(MinLengthAttribute), a =>
                {
                    MinLengthAttribute minLength = (MinLengthAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Min length: {0}", minLength.Length);
                }
            },
            { typeof(StringLengthAttribute), a =>
                {
                    StringLengthAttribute strLength = (StringLengthAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "String length: inclusive between {0} and {1}", strLength.MinimumLength, strLength.MaximumLength);
                }
            },
            { typeof(DataTypeAttribute), a =>
                {
                    DataTypeAttribute dataType = (DataTypeAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Data type: {0}", dataType.CustomDataType ?? dataType.DataType.ToString());
                }
            },
            { typeof(RegularExpressionAttribute), a =>
                {
                    RegularExpressionAttribute regularExpression = (RegularExpressionAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Matching regular expression pattern: {0}", regularExpression.Pattern);
                }
            },
        };


        /// <summary>
        /// 
        /// </summary>
        private readonly IDictionary<Type, string> DefaultTypeDocumentation = new Dictionary<Type, string>
        {
            { typeof(short), "integer" },
            { typeof(int), "integer" },
            { typeof(long), "integer" },
            { typeof(ushort), "unsigned integer" },
            { typeof(uint), "unsigned integer" },
            { typeof(ulong), "unsigned integer" },
            { typeof(byte), "byte" },
            { typeof(char), "character" },
            { typeof(sbyte), "signed byte" },
            { typeof(Uri), "URI" },
            { typeof(float), "decimal number" },
            { typeof(double), "decimal number" },
            { typeof(decimal), "decimal number" },
            { typeof(string), "string" },
            { typeof(Guid), "globally unique identifier" },
            { typeof(TimeSpan), "time interval" },
            { typeof(DateTime), "date" },
            { typeof(DateTimeOffset), "date" },
            { typeof(bool), "boolean" },
        };

        /// <summary>
        /// 
        /// </summary>
        private readonly Lazy<IModelDocumentationProvider> _documentationProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public ModelDescriptionGenerator(HttpConfiguration config) {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            _documentationProvider = new Lazy<IModelDocumentationProvider>(() => config.Services.GetDocumentationProvider() as IModelDocumentationProvider);
            GeneratedModels = new Dictionary<string, ModelDescription>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, ModelDescription> GeneratedModels { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private IModelDocumentationProvider DocumentationProvider => _documentationProvider.Value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public ModelDescription GetOrCreateModelDescription(Type modelType) {
            if (modelType == null) {
                throw new ArgumentNullException("modelType");
            }

            Type underlyingType = Nullable.GetUnderlyingType(modelType);
            if (underlyingType != null) {
                modelType = underlyingType;
            }

            string modelName = ModelNameHelper.GetModelName(modelType);
            if (GeneratedModels.TryGetValue(modelName, out ModelDescription modelDescription)) {
                if (modelType != modelDescription.ModelType) {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "A model description could not be created. Duplicate model name '{0}' was found for types '{1}' and '{2}'. " +
                            "Use the [ModelName] attribute to change the model name for at least one of the types so that it has a unique name.",
                            modelName,
                            modelDescription.ModelType.FullName,
                            modelType.FullName));
                }

                return modelDescription;
            }

            if (DefaultTypeDocumentation.ContainsKey(modelType)) {
                return GenerateSimpleTypeModelDescription(modelType);
            }

            if (modelType.IsEnum) {
                return GenerateEnumTypeModelDescription(modelType);
            }

            if (modelType.IsGenericType) {
                Type[] genericArguments = modelType.GetGenericArguments();

                if (genericArguments.Length == 1) {
                    Type enumerableType = typeof(IEnumerable<>).MakeGenericType(genericArguments);
                    if (enumerableType.IsAssignableFrom(modelType)) {
                        return GenerateCollectionModelDescription(modelType, genericArguments[0]);
                    }
                }
                if (genericArguments.Length == 2) {
                    Type dictionaryType = typeof(IDictionary<,>).MakeGenericType(genericArguments);
                    if (dictionaryType.IsAssignableFrom(modelType)) {
                        return GenerateDictionaryModelDescription(modelType, genericArguments[0], genericArguments[1]);
                    }

                    Type keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(genericArguments);
                    if (keyValuePairType.IsAssignableFrom(modelType)) {
                        return GenerateKeyValuePairModelDescription(modelType, genericArguments[0], genericArguments[1]);
                    }
                }
            }

            if (modelType.IsArray) {
                Type elementType = modelType.GetElementType();
                return GenerateCollectionModelDescription(modelType, elementType);
            }

            if (modelType == typeof(NameValueCollection)) {
                return GenerateDictionaryModelDescription(modelType, typeof(string), typeof(string));
            }

            if (typeof(IDictionary).IsAssignableFrom(modelType)) {
                return GenerateDictionaryModelDescription(modelType, typeof(object), typeof(object));
            }

            if (typeof(IEnumerable).IsAssignableFrom(modelType)) {
                return GenerateCollectionModelDescription(modelType, typeof(object));
            }

            return GenerateComplexTypeModelDescription(modelType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="hasDataContractAttribute"></param>
        /// <returns></returns>
        private static string GetMemberName(MemberInfo member, bool hasDataContractAttribute) {
            JsonPropertyAttribute jsonProperty = member.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty != null && !string.IsNullOrEmpty(jsonProperty.PropertyName)) {
                return jsonProperty.PropertyName;
            }

            if (hasDataContractAttribute) {
                DataMemberAttribute dataMember = member.GetCustomAttribute<DataMemberAttribute>();
                if (dataMember != null && !string.IsNullOrEmpty(dataMember.Name)) {
                    return dataMember.Name;
                }
            }

            return member.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="hasDataContractAttribute"></param>
        /// <returns></returns>
        private static bool ShouldDisplayMember(MemberInfo member, bool hasDataContractAttribute) {
            JsonIgnoreAttribute jsonIgnore = member.GetCustomAttribute<JsonIgnoreAttribute>();
            XmlIgnoreAttribute xmlIgnore = member.GetCustomAttribute<XmlIgnoreAttribute>();
            IgnoreDataMemberAttribute ignoreDataMember = member.GetCustomAttribute<IgnoreDataMemberAttribute>();
            NonSerializedAttribute nonSerialized = member.GetCustomAttribute<NonSerializedAttribute>();
            ApiExplorerSettingsAttribute apiExplorerSetting = member.GetCustomAttribute<ApiExplorerSettingsAttribute>();

            bool hasMemberAttribute = member.DeclaringType.IsEnum ?
                member.GetCustomAttribute<EnumMemberAttribute>() != null :
                member.GetCustomAttribute<DataMemberAttribute>() != null;

            // Display member only if all the followings are true:
            // no JsonIgnoreAttribute
            // no XmlIgnoreAttribute
            // no IgnoreDataMemberAttribute
            // no NonSerializedAttribute
            // no ApiExplorerSettingsAttribute with IgnoreApi set to true
            // no DataContractAttribute without DataMemberAttribute or EnumMemberAttribute
            return jsonIgnore == null &&
                xmlIgnore == null &&
                ignoreDataMember == null &&
                nonSerialized == null &&
                (apiExplorerSetting == null || !apiExplorerSetting.IgnoreApi) &&
                (!hasDataContractAttribute || hasMemberAttribute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateDefaultDocumentation(Type type) {
            if (DefaultTypeDocumentation.TryGetValue(type, out string documentation)) {
                return documentation;
            }
            if (DocumentationProvider != null) {
                documentation = DocumentationProvider.GetDocumentation(type);
            }

            return documentation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyModel"></param>
        private void GenerateAnnotations(MemberInfo property, ParameterDescription propertyModel) {
            List<ParameterAnnotation> annotations = new List<ParameterAnnotation>();

            IEnumerable<Attribute> attributes = property.GetCustomAttributes();
            foreach (Attribute attribute in attributes) {
                if (AnnotationTextGenerator.TryGetValue(attribute.GetType(), out Func<object, string> textGenerator)) {
                    annotations.Add(
                        new ParameterAnnotation {
                            AnnotationAttribute = attribute,
                            Documentation = textGenerator(attribute)
                        });
                }
            }

            // Rearrange the annotations
            annotations.Sort((x, y) => {
                // Special-case RequiredAttribute so that it shows up on top
                if (x.AnnotationAttribute is RequiredAttribute) {
                    return -1;
                }
                if (y.AnnotationAttribute is RequiredAttribute) {
                    return 1;
                }

                // Sort the rest based on alphabetic order of the documentation
                return string.Compare(x.Documentation, y.Documentation, StringComparison.OrdinalIgnoreCase);
            });

            foreach (ParameterAnnotation annotation in annotations) {
                propertyModel.Annotations.Add(annotation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private CollectionModelDescription GenerateCollectionModelDescription(Type modelType, Type elementType) {
            ModelDescription collectionModelDescription = GetOrCreateModelDescription(elementType);
            if (collectionModelDescription != null) {
                return new CollectionModelDescription {
                    Name = ModelNameHelper.GetModelName(modelType),
                    ModelType = modelType,
                    ElementDescription = collectionModelDescription
                };
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private ModelDescription GenerateComplexTypeModelDescription(Type modelType) {
            ComplexTypeModelDescription complexModelDescription = new ComplexTypeModelDescription {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                Documentation = CreateDefaultDocumentation(modelType)
            };

            GeneratedModels.Add(complexModelDescription.Name, complexModelDescription);
            bool hasDataContractAttribute = modelType.GetCustomAttribute<DataContractAttribute>() != null;
            PropertyInfo[] properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties) {
                if (ShouldDisplayMember(property, hasDataContractAttribute)) {
                    ParameterDescription propertyModel = new ParameterDescription {
                        Name = GetMemberName(property, hasDataContractAttribute)
                    };

                    if (DocumentationProvider != null) {
                        propertyModel.Documentation = DocumentationProvider.GetDocumentation(property);
                    }

                    GenerateAnnotations(property, propertyModel);
                    complexModelDescription.Properties.Add(propertyModel);
                    propertyModel.TypeDescription = GetOrCreateModelDescription(property.PropertyType);
                }
            }

            FieldInfo[] fields = modelType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields) {
                if (ShouldDisplayMember(field, hasDataContractAttribute)) {
                    ParameterDescription propertyModel = new ParameterDescription {
                        Name = GetMemberName(field, hasDataContractAttribute)
                    };

                    if (DocumentationProvider != null) {
                        propertyModel.Documentation = DocumentationProvider.GetDocumentation(field);
                    }

                    complexModelDescription.Properties.Add(propertyModel);
                    propertyModel.TypeDescription = GetOrCreateModelDescription(field.FieldType);
                }
            }

            return complexModelDescription;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="keyType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        private DictionaryModelDescription GenerateDictionaryModelDescription(Type modelType, Type keyType, Type valueType) {
            ModelDescription keyModelDescription = GetOrCreateModelDescription(keyType);
            ModelDescription valueModelDescription = GetOrCreateModelDescription(valueType);

            return new DictionaryModelDescription {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                KeyModelDescription = keyModelDescription,
                ValueModelDescription = valueModelDescription
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private EnumTypeModelDescription GenerateEnumTypeModelDescription(Type modelType) {
            EnumTypeModelDescription enumDescription = new EnumTypeModelDescription {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                Documentation = CreateDefaultDocumentation(modelType)
            };
            bool hasDataContractAttribute = modelType.GetCustomAttribute<DataContractAttribute>() != null;
            foreach (FieldInfo field in modelType.GetFields(BindingFlags.Public | BindingFlags.Static)) {
                if (ShouldDisplayMember(field, hasDataContractAttribute)) {
                    EnumValueDescription enumValue = new EnumValueDescription {
                        Name = field.Name,
                        Value = field.GetRawConstantValue().ToString()
                    };
                    if (DocumentationProvider != null) {
                        enumValue.Documentation = DocumentationProvider.GetDocumentation(field);
                    }
                    enumDescription.Values.Add(enumValue);
                }
            }
            GeneratedModels.Add(enumDescription.Name, enumDescription);

            return enumDescription;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="keyType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        private KeyValuePairModelDescription GenerateKeyValuePairModelDescription(Type modelType, Type keyType, Type valueType) {
            ModelDescription keyModelDescription = GetOrCreateModelDescription(keyType);
            ModelDescription valueModelDescription = GetOrCreateModelDescription(valueType);

            return new KeyValuePairModelDescription {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                KeyModelDescription = keyModelDescription,
                ValueModelDescription = valueModelDescription
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private ModelDescription GenerateSimpleTypeModelDescription(Type modelType) {
            SimpleTypeModelDescription simpleModelDescription = new SimpleTypeModelDescription {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                Documentation = CreateDefaultDocumentation(modelType)
            };
            GeneratedModels.Add(simpleModelDescription.Name, simpleModelDescription);

            return simpleModelDescription;
        }
    }
}