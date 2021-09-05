using System.Xml;
using System.Collections.Generic;

class XmlDoc
{
    readonly string FilePath;
    readonly XmlDocument Document = new XmlDocument();

    public XmlDoc(string FilePath)
    {
        this.FilePath = FilePath;
    }

    public void CreateDocument(Dictionary<string, string> Parameters)
    {
        Document.RemoveAll();
        Document.AppendChild(Document.CreateXmlDeclaration("1.0", "utf-8", null));
        XmlElement Root = Document.CreateElement("ROOT");

        foreach (string key in Parameters.Keys) {
            XmlElement element = Document.CreateElement(key);
            element.InnerText = Parameters[key];
            Root.AppendChild(element);
        }
        Document.AppendChild(Root);
        Document.Save(FilePath);
    }

    public bool LoadDocument()
    {
        try {
            Document.Load(FilePath);
            return true;
        }
        catch (XmlException) {
            return false;
        }
    }

    public Dictionary<string, string> GetParameters()
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();
        XmlNodeList nodes = Document.DocumentElement.ChildNodes;
        foreach (XmlNode node in nodes) Parameters.Add(node.Name, node.InnerText);
        return Parameters;
    }

    public bool ChangeNodeValue(string Key, string Value)
    {
        XmlElement root = Document.DocumentElement;
        foreach (XmlNode node in root.ChildNodes)
            if (node.Name == Key) {
                node.InnerText = Value;
                Document.Save(FilePath);
                return true;
            }
        return false;
    }
}

