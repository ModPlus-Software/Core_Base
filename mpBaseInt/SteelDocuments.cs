namespace mpBaseInt
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;

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