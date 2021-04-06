using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace ShevaEngine.UI
{
    public class XamlImporter 
    {

        private static Dictionary<Type, INodeImporter> _importers = CreateImporters();


        /// <summary>
        /// Create importers.
        /// </summary>        
        private static Dictionary<Type, INodeImporter> CreateImporters()
        {
            return new Dictionary<Type, INodeImporter>
            {
                { typeof(ColumnDefinition), new ColumnDefinitionImporter() },
                { typeof(RowDefinition), new RowDefinitionImporter() }
            };
        }

        /// <summary>
        /// Import layer.
        /// </summary>
        public static Control Import(IUIStyleGenerator styleGenerator, string data)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(data);

            return ImportXaml(styleGenerator, document);            
        }

        /// <summary>
        /// Import xaml.
        /// </summary>        
        private static Control ImportXaml(IUIStyleGenerator styleGenerator, XmlDocument document)
        {
            return ImportXamlNode(styleGenerator, document.FirstChild.FirstChild);
        }

        /// <summary>
        /// Import xaml node.
        /// </summary>        
        private static Control ImportXamlNode(IUIStyleGenerator styleGenerator, XmlNode node)
        {
            Control control = CreateControl(styleGenerator, node);

            if (control != null)
            {
                SetAttributes(styleGenerator, control, node);

                foreach (XmlNode child in node.ChildNodes)
                {
                    string[] parts = child.Name.Split('.');

                    if (parts.Length > 1)
                    {
                        string attributeName = FixAttributeName(parts[1]);

                        if (control.HasProperty(attributeName))
                            SetAttribute(control, attributeName, child);
                    }
                    else
                        control.Children.Add(ImportXamlNode(styleGenerator, child));
                }
            }
            
            return control;
        }

        /// <summary>
        /// Create control.
        /// </summary>
        private static Control CreateControl(IUIStyleGenerator styleGenerator, XmlNode node)
        {
            switch (node.Name.ToLowerInvariant())
            {                
                case "grid":
                    return styleGenerator.Create<Grid>();                                    
                case "button":
                    return styleGenerator.Create<Button>();
                case "textblock":
                    return styleGenerator.Create<Label>();
                case "image":
                    return styleGenerator.Create<Image>();
                default:
                    throw new NotImplementedException();
            }           
        }        

        /// <summary>
        /// Set attributes.
        /// </summary>
        private static void SetAttributes(IUIStyleGenerator styleGenerator, Control control, XmlNode node)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                string attributeName = FixAttributeName(attribute.Name);

                if (control.HasProperty(attributeName))
                    SetAttribute(control, attributeName, attribute.Value);
                else if (attributeName == "content")
                {
                    Label newLabel = styleGenerator.Create<Label>();
                    newLabel.Text.OnNext(attribute.Value);

                    control.Children.Add(newLabel);
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// Parse value.
        /// </summary>
        private static void SetAttribute(Control control, string propertyName, string value)
        {
            Type propertyType = control.GetPropertyType(propertyName);

            if (propertyType == typeof(int))
                control.SetPropertyValue(propertyName, int.Parse(value));
            else
                control.SetPropertyValue(propertyName, value);
        }

        /// <summary>
        /// Parse value.
        /// </summary>
        private static void SetAttribute(Control control, string propertyName, XmlNode node)
        {
            Type propertyType = control.GetPropertyType(propertyName);
           
            if (propertyType.IsGenericType && propertyType.GenericTypeArguments.Length > 0)
            {
                Type itemType = propertyType.GenericTypeArguments[0];
                Type containerType = typeof(List<>).MakeGenericType(itemType);                               
                IList container = (IList)Activator.CreateInstance(containerType);

                if (_importers.ContainsKey(itemType) && _importers.TryGetValue(itemType, out INodeImporter importer))
                    foreach (XmlNode child in node.ChildNodes)
                        container.Add(importer.Import(child));

                control.SetPropertyValue(propertyName, container);
            }
        }

        /// <summary>
        /// Fix attribute name.
        /// </summary>
        private static string FixAttributeName(string attributeName)
        {
            return attributeName.ToLower().Replace(".", "");
        }

        
    }
}
