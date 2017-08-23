using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.Enums.PascalCaseWordSplittingEnumConverter
{
    public class PascalCaseWordSplittingEnumConverter : EnumConverter
    {
        public PascalCaseWordSplittingEnumConverter(Type type)
            : base(type)
        {

        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string stringValue = (string)base.ConvertTo(context, culture, value, destinationType);

                stringValue = SplitString(stringValue);

                return stringValue;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public string SplitString(string stringValue)
        {
            StringBuilder buff = new StringBuilder(stringValue);

            //assume the first letter is upper

            bool lastWasUpper = true;
            int lastSpaceIndex = -1;

            for(int i = 1; i < buff.Length; i++){
                bool isUpper = char.IsUpper(buff[i]);

                if(isUpper & !lastWasUpper){
                    buff.Insert(i, ' ');
                    lastSpaceIndex = i;
                }

                if (!isUpper && lastWasUpper)
                {
                    if (lastSpaceIndex != i-2)
                    {
                        buff.Insert(i - 1, ' ');
                        lastSpaceIndex = i = 1;
                    }
                }

                lastWasUpper = isUpper;
            }
            return buff.ToString();
        }
    }
}
