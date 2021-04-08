using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using ShevaEngine.UI.Brushes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace ShevaEngine.UI
{
    public class XamlImporter 
    {
        private readonly static Log _log = new Log(typeof(XamlImporter));
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
        public static T Import<T>(IUIStyleGenerator styleGenerator, string data)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(data);

            if (typeof(T) == typeof(Control))
            {
                Control control = ImportXaml(styleGenerator, document);
                control.InitializeComponent();

                return (T)(object)control;
            }
            else if (typeof(T) == typeof(Layer))
            {
                Control control = ImportXaml(styleGenerator, document);
                control.InitializeComponent();

                return (T)(object)new Layer()
                {
                    Control = control
                };
            }
            else
            {
                _log.Error($"Can't import {typeof(T)}");

                return default;
            }
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
                case "checkbox":
                    return styleGenerator.Create<Checkbox>();
                case "combobox":
                    return styleGenerator.Create<Combobox>();
                case "slider":
                    return styleGenerator.Create<Slider>();
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
                    SetAttribute(newLabel, "text", attribute.Value);

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

            Regex r = new Regex(@"{\s*Binding\s+(?<name>.+)\s*}");
            Match match = r.Match(value);

            if (match.Success)
            {
                control.SetPropertyBinding(propertyName, new Binding()
                {
                    PropertyName = match.Groups[1].Value
                });
            }
            else
            {
                if (propertyType == typeof(int))
                    control.SetPropertyValue(propertyName, int.Parse(value));
                else if (propertyType == typeof(double))
                    control.SetPropertyValue(propertyName, double.Parse(value));
                else if (propertyType == typeof(Margin))
                    control.SetPropertyValue(propertyName, new Margin(value));
                else if (propertyType == typeof(Brush))
                    control.SetPropertyValue(propertyName, CreateBrush(value));
                else if (propertyType == typeof(FontSize))
                    control.SetPropertyValue(propertyName, CreateFontSize(value));
                else if (propertyType == typeof(HorizontalAlignment))
                {
                    if (Enum.TryParse(value, true, out HorizontalAlignment alignment))
                        control.SetPropertyValue(propertyName, alignment);
                }
                else if (propertyType == typeof(VerticalAlignment))
                {
                    if (Enum.TryParse(value, true, out VerticalAlignment alignment))
                        control.SetPropertyValue(propertyName, alignment);
                }
                else if (propertyType == typeof(Texture2D))
                {
                    Texture2D texture = ShevaGame.Instance.Content.Load<Texture2D>(System.IO.Path.ChangeExtension(value, null));

                    control.SetPropertyValue(propertyName, texture);
                }
                else
                    control.SetPropertyValue(propertyName, value);
            }
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

        /// <summary>
        /// Create brush.
        /// </summary>
        private static Brush CreateBrush(string value)
        {
            Type colorType = typeof(Color);

            System.Reflection.PropertyInfo property = typeof(Color).GetProperty(value);

            if (property != null)
                return new SolidColorBrush((Color)property.GetValue(null));

            return null;
        }

        /// <summary>
        /// Create font size.
        /// </summary>
        private static FontSize CreateFontSize(string value)
        {
            if (Enum.TryParse<FontSize>($"Size{value}", out FontSize size))
                return size;

            return FontSize.Size12;
        }
    }
}
