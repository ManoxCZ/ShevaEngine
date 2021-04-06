using System.Xml;


namespace ShevaEngine.UI
{
    public interface INodeImporter
    {
        object Import(XmlNode node);
    }
}
