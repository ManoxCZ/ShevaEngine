using System.Xml;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Column definition importer.
    /// </summary>
    public class ColumnDefinitionImporter : INodeImporter
    {
        /// <summary>
        /// Import method.
        /// </summary>
        public object Import(XmlNode node)
        {
            if (node.Name == nameof(ColumnDefinition))
            {
                XmlAttribute attribute = node.Attributes["Width"];

                string attributeValue = attribute.Value.Trim();

                string[] parts = attributeValue.Split('*');
                int value = int.Parse(parts[0]);

                return new ColumnDefinition()
                {
                    Units = parts.Length > 1 ? Units.Relative : Units.Absolute,
                    Value = value
                };
            }

            return null;
        }
    }
}
