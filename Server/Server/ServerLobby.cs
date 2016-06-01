using System;
using System.Collections.Generic;
using System.Xml;

namespace Server
{
    static class ServerLobby
    {
        private static List<Table> tables = new List<Table>();
        public static XmlDocument reader = new XmlDocument();

        public static List<Table> Tables
        {
            get { return tables; }
        }

        public static void Initialize()
        {
            reader.Load(@"C:\Users\Богдан\Desktop\Poker\Server\Server\Tables.xml");
            XmlNodeList a = reader.SelectNodes("/Tables/Table");
            Table t;
            for (int i = 0; i < a.Count; i++)
            {
                t = new Table(a.Item(i).FirstChild.InnerText, int.Parse(a.Item(i).FirstChild.NextSibling.InnerText), int.Parse(a.Item(i).LastChild.InnerText));
                tables.Add(t);
            }
        }
    }
}