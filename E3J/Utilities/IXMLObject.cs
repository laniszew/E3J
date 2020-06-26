using System.Xml;

namespace E3J.Utilities
{
    /// <summary>
    /// IXMLObject class
    /// </summary>
    public interface IXMLObject
    {
        /// <summary>
        /// To the XML.
        /// </summary>
        /// <returns></returns>
        XmlElement ToXML();
    }
}
