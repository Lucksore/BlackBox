using System;
using System.IO;
using System.Xml;

namespace BlackBox
{
    class ConfigureFileXML
    {
        public XmlDocument doc = new XmlDocument();

        readonly string FilePath;
        public int Width;
        public int Height;
        public string Title;
        public string UserName;

        public ConfigureFileXML(string FilePath)
        {
            this.FilePath = FilePath;
            if (File.Exists(FilePath)) doc.Load(FilePath);
            else {
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                XmlElement Root = doc.CreateElement("Settings");
                XmlElement Width = doc.CreateElement("Width");
                XmlElement Height = doc.CreateElement("Height");
                XmlElement Title = doc.CreateElement("Title");
                XmlElement UserName = doc.CreateElement("UserName");

                Width.InnerText = Console.WindowWidth.ToString();
                Height.InnerText = Console.WindowHeight.ToString();
                Title.InnerText = "BlackBox";
                UserName.InnerText = Environment.UserName;

                Root.AppendChild(Width);
                Root.AppendChild(Height);
                Root.AppendChild(Title);
                Root.AppendChild(UserName);

                doc.AppendChild(declaration);
                doc.AppendChild(Root);
                doc.Save(FilePath);
            }

            InitializeValues();
        }

        void InitializeValues()
        {
            Width = int.Parse(doc.GetElementsByTagName("Width")[0].InnerText);
            Height = int.Parse(doc.GetElementsByTagName("Height")[0].InnerText);
            Title = doc.GetElementsByTagName("Title")[0].InnerText;
            UserName = doc.GetElementsByTagName("UserName")[0].InnerText;
        }

        public void SetWidth(int Value)
        {
            doc.GetElementsByTagName("Width")[0].InnerText = Value.ToString();
            Width = Value;
            doc.Save(FilePath);
        }
        public void SetHeight(int Value)
        {
            doc.GetElementsByTagName("Height")[0].InnerText = Value.ToString();
            Height = Value;
            doc.Save(FilePath);
            
        }
        public void SetSize(int H, int W)
        {
            SetHeight(H);
            SetWidth(W);
        }
        public void SetTitle(string Value)
        {
            doc.GetElementsByTagName("Title")[0].InnerText = Value;
            Title = Value;
            doc.Save(FilePath);
        }
    }
}
