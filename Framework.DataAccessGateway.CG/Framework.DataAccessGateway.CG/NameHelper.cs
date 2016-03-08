using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.DataAccessGateway.CG
{
    internal static class NameHelper
    {
        private static string ModelNamingFormat = "{0}";
        private static string ModelExtNamingFormat = "{0}_Ext";
        private static string ModelKeyNamingFormat = "{0}_Key";
        private static string ModelListFormat = "IList<{0}>";
        private static string RepositoryNamingFormat = "{0}_Repository";

        public static string ModelParameterName = "model";
        public static string ModelListParameterName = "models";
        public static string ModelColumnParameterName = "parameter";
        
        public static string ToModelName(this string value)
        {
            return String.Format(ModelNamingFormat, value);
        }

        public static string ToModelExtName(this string value)
        {
            return String.Format(ModelExtNamingFormat, value.ToModelName());
        }

        public static string ToModelKeyName(this string value)
        {
            return String.Format(ModelKeyNamingFormat, value.ToModelName());
        }

        public static string ToModelListName(this string value)
        {
            return String.Format(ModelListFormat, value.ToModelName());
        }       

        public static string ToRepositoryName(this string name)
        {
            return String.Format(RepositoryNamingFormat, name);
        }
    }
}
