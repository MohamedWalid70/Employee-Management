using System.Reflection;
using System.Runtime.CompilerServices;

namespace Internship.StringParser
{
    public class StringParser<T>
    {
        public static List<T> Parse(string input)
        {
            string endOfLineMarker = "\r\n";
            List<T> result;

            ArgumentNullException.ThrowIfNullOrWhiteSpace(input);

            var inputStrings = input.Split(endOfLineMarker);

            var objectLinesExecludingHeader = inputStrings[1..];

            var dersiredProperties = GetInputDesiredProperties<T>(inputBlueprint: inputStrings[0]);

            result = ParseAndConvertRawInput(objectLinesExecludingHeader, dersiredProperties);
            return result;

        }

        private static List<PropertyInfo> GetInputDesiredProperties<T>(string inputBlueprint)
        {
            Type type = typeof(T);

            bool noPropertyMatch = true;

            List<PropertyInfo> dersiredProperties = new();

            var objectPropertyNames = inputBlueprint.Split(',');

            foreach (var property in type.GetProperties())
            {
                foreach (var inputType in objectPropertyNames)
                {
                    if (property.Name.Equals(inputType.Trim()))
                    {
                        noPropertyMatch = false;
                        dersiredProperties.Add(property);
                        break;
                    }

                }

                if (noPropertyMatch && property.GetCustomAttribute<RequiredAttributeAttribute>() != null)
                    throw new FormatException();
            }

            return dersiredProperties;
        }


        private static List<T> ParseAndConvertRawInput(string[] objectsRawLines, List<PropertyInfo> targetProperties)
        {

            List<T> result = new();

            T? item;
            string[] objectPropertiesValues;
            object? convertedPropertyValue;

            foreach (var objectLine in objectsRawLines)
            {

                objectPropertiesValues = objectLine.Split(',');

                if (objectPropertiesValues.Length == targetProperties.Count)
                {

                    item = Activator.CreateInstance<T>();

                    for (int i = 0; i < objectPropertiesValues.Length; i++)
                    {
                        convertedPropertyValue = ConvertType(objectPropertiesValues[i], targetProperties[i]);
                        targetProperties[i].SetValue(item, convertedPropertyValue);
                    }

                    result.Add(item);
                }
                else
                {
                    throw new FormatException();
                }

            }
            return result;
        }

        private static object? ConvertType(string value, PropertyInfo propertyInfo)
        {
            string trimmedValue;
            object? convertedPropertyValue;
            trimmedValue = value.Trim().ToLower();

            if (propertyInfo.PropertyType.IsValueType)
            {

                Type? nullableUnderlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);

                if (trimmedValue == "null" && nullableUnderlyingType == null)
                {
                    throw new FormatException();
                }
                else if (trimmedValue == "null" && nullableUnderlyingType != null)
                {
                    convertedPropertyValue = null;
                }
                else if (trimmedValue != "null" && nullableUnderlyingType != null)
                {
                    convertedPropertyValue = Convert.ChangeType(value, nullableUnderlyingType);
                }
                else
                {
                    convertedPropertyValue = Convert.ChangeType(value, propertyInfo.PropertyType);
                }
            }
            else
            {
                var nullabilityInfoContext = new NullabilityInfoContext();
                var info = nullabilityInfoContext.Create(propertyInfo);

                bool isRefTypeNullable = info.ReadState == NullabilityState.Nullable || info.WriteState == NullabilityState.Nullable;

                if (trimmedValue == "null" && !isRefTypeNullable)
                {
                    throw new FormatException();
                }
                else if (trimmedValue == "null" && isRefTypeNullable)
                {
                    convertedPropertyValue = null;
                }
                else
                {
                    convertedPropertyValue = Convert.ChangeType(value, propertyInfo.PropertyType);
                }

            }
            return convertedPropertyValue;
        }

    }
}
