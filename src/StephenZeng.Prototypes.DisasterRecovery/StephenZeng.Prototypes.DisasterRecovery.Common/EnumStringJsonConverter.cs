using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StephenZeng.Prototypes.DisasterRecovery.Common
{
    public class EnumStringJsonConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();

            if (!type.IsEnum) throw new InvalidOperationException("Only type Enum is supported");
            foreach (var field in type.GetFields())
            {
                if (field.Name == value.ToString())
                {
                    var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    writer.WriteValue(attribute != null ? attribute.Description : field.Name);

                    return;
                }
            }

            throw new ArgumentException("Enum not found");
        }
    }
}