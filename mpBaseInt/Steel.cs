using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace mpBaseInt
{
    public class Steel
    {
        public string Document { get; set; }
        public string DocumentName { get; set; }
        public List<string> Values { get; set; }
    }

    public static class SteelDocuments
    {
        public static ObservableCollection<Steel> GetSteels()
        {
            var coll = new ObservableCollection<Steel>();
            // Парсим документ из ресурсов
            var resDoc = XElement.Parse(Properties.Resources.Steel);
            foreach (var element in resDoc.Elements("steel"))
            {
                var steel = new Steel
                {
                    Document = element.Attribute("doc").Value,
                    DocumentName = element.Attribute("name").Value,
                    Values = element.Attribute("marks").Value.Split(',').ToList()
                };
                coll.Add(steel);
            }
            return coll;
        }
    }
}
