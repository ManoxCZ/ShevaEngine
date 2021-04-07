using System.Globalization;
using System.Xml;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Row definition importer.
    /// </summary>
    public class RowDefinitionImporter : INodeImporter
    {
        /// <summary>
        /// Import method.
        /// </summary>
        public object Import(XmlNode node)
        {
            if (node.Name == nameof(RowDefinition))
            {
                XmlAttribute attribute = node.Attributes["Height"];

                string attributeValue = attribute.Value.Trim();

                string[] parts = attributeValue.Split('*');
                double value = double.Parse(parts[0], CultureInfo.InvariantCulture);

                return new RowDefinition()
                {
                    Units = parts.Length > 1 ? Units.Relative : Units.Absolute,
                    Value = value
                };
            }

            return null;
        }
    }
}
