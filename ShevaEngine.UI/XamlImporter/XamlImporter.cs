using System;
using System.Xml;

namespace ShevaEngine.UI.XamlImporter
{
    public class XamlImporter 
    {
        /// <summary>
        /// Import layer.
        /// </summary>
        public static Control Import(string data)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(data);

            return ImportXaml(document);            
        }

        /// <summary>
        /// Import xaml.
        /// </summary>        
        private static Control ImportXaml(XmlDocument document)
        {
            return ImportXamlNode(document.FirstChild.FirstChild);
        }

        /// <summary>
        /// Import xaml node.
        /// </summary>        
        private static Control ImportXamlNode(XmlNode node)
        {
            Control control = CreateControl(node);

            if (control != null)
            {
                SetAttributes(control, node);

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
                        control.Children.Add(ImportXamlNode(child));
                }
            }
            
            return control;
        }

        /// <summary>
        /// Create control.
        /// </summary>
        private static Control CreateControl(XmlNode node)
        {
            switch (node.Name.ToLowerInvariant())
            {
                case "usercontrol":
                    return CreateUserControl(node);
                case "grid":
                    return CreateGrid(node);
                case "button":
                    return CreateButton(node);
                case "stackpanel":
                    break;
                case "textblock":
                    return CreateLabel(node);
                case "image":
                    break;
                default:
                    throw new NotImplementedException();
            }

            return null;
        }

        /// <summary>
        /// Create user control.
        /// </summary>
        /// <returns></returns>
        private static Control CreateUserControl(XmlNode node)
        {
            return new Grid();
        }

        /// <summary>
        /// Create grid control.
        /// </summary>
        /// <returns></returns>
        private static Control CreateGrid(XmlNode node)
        {
            return new Grid();
        }

        /// <summary>
        /// Create button control.
        /// </summary>
        /// <returns></returns>
        private static Button CreateButton(XmlNode node)
        {
            return new Button();
        }

        /// <summary>
        /// Create label control.
        /// </summary>
        /// <returns></returns>
        private static Label CreateLabel(XmlNode node)
        {           
            return new Label();
        }

        /// <summary>
        /// Set attributes.
        /// </summary>
        private static void SetAttributes(Control control, XmlNode node)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                string attributeName = FixAttributeName(attribute.Name);

                if (control.HasProperty(attributeName))
                    SetAttribute(control, attributeName, attribute.Value);
            }
        }

        /// <summary>
        /// Parse value.
        /// </summary>
        private static void SetAttribute(Control control, string propertyName, string value)
        {
            Type propertyType = control.GetPropertyType(propertyName);

            if (propertyType == typeof(Int32))
                control.SetPropertyValue<Int32>(propertyName, int.Parse(value));
            else
                control.SetPropertyValue(propertyName, value);
        }

        /// <summary>
        /// Parse value.
        /// </summary>
        private static void SetAttribute(Control control, string propertyName, XmlNode node)
        {
            Type propertyType = control.GetPropertyType(propertyName);            
        }

        /// <summary>
        /// Fix attribute name.
        /// </summary>
        private static string FixAttributeName(string attributeName)
        {
            return attributeName.Replace(".", "");
        }

        
    }
}
