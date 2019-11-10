namespace mpBaseInt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Представление документа базы
    /// </summary>
    public class BaseDocument
    {
        /// <summary>
        /// Имя базы данных к которому относится элемент
        /// нужно для работы из других функций
        /// </summary>
        public string DataBaseName { get; set; }

        /// <summary>
        /// ID документа
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Текущий документ в виде Xml-элемента
        /// </summary>
        public XElement XmlDocument { get; set; }
        
        /// <summary>
        /// Тип документа (ГОСТ, Серия и т.п.)
        /// </summary>
        public string DocumentType { get; set; }
        
        /// <summary>
        /// Номер документа
        /// </summary>
        public string DocumentNumber { get; set; }
        
        /// <summary>
        /// Название документа
        /// </summary>
        public string DocumentName { get; set; }
        
        /// <summary>
        /// Статус документа
        /// </summary>
        public bool? DocStatus { get; set; }
        
        /// <summary>
        /// Краткое название документа для обобщения (Двутавры, Прокат листовой, Сваи железобетонные и т.д.)
        /// </summary>
        public string DocumentShortName { get; set; }
        
        /// <summary>
        /// Короткое название изделия (Уголок, двутавр, швеллер и т.п.)
        /// </summary>
        public string ShortName { get; set; }
        
        /// <summary>
        /// Группа (балки, колонны, профили, прокат и т.д.)
        /// </summary>
        public string Group { get; set; }
        
        /// <summary>
        /// Кол-во описывающих символов = кол-во столбцов в таблице
        /// </summary>
        public int SymbolCount { get; set; }
        
        /// <summary>
        /// Описывающие символы (заголовки таблицы)
        /// </summary>
        public IEnumerable<string> Symbols { get; set; }
        
        /// <summary>
        /// Значения в файле в виде xml (для биндинга)
        /// </summary>
        public XElement Items { get; set; }
        
        /// <summary>
        /// Название файла изображения (без расширения). Может быть пустым
        /// </summary>
        public string Image { get; set; }
        
        /// <summary>
        /// Выбран-ли этот документ. Нужно для работы с контролами окна и биндингом
        /// </summary>
        public bool IsSelected { get; set; }
        
        /// <summary>
        /// Коллекция дополнительных свойств, которые не входят в таблицу, но могут быть выбраны
        /// </summary>
        public ObservableCollection<ItemType> ItemTypes { get; set; }
        
        /// <summary>
        /// Имеет ли элемент свойство "Сталь"
        /// </summary>
        public bool HasSteel { get; set; }
        
        /// <summary>
        /// Правило написания наименования
        /// представляет собой строку с именами атрибутов, взятых в []
        /// </summary>
        public string Rule { get; set; }
        
        /// <summary>
        /// Является-ли элемент Железобетонным (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string CoType { get; set; }
        
        /// <summary>
        /// Является-ли элемент Металлическим (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string MeType { get; set; }
        
        /// <summary>
        /// Является-ли элемент Деревянным (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string WdType { get; set; }
        
        /// <summary>
        /// Является-ли элемент Материалом (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string MaType { get; set; }
        
        /// <summary>
        /// Является-ли элемент Прочим (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string OtType { get; set; }
        
        /// <summary>
        /// Масса элемента, кг
        /// </summary>
        public string Mass { get; set; }
        
        /// <summary>
        /// Масса погонного метра, кг/п.м
        /// </summary>
        public string WMass { get; set; }
        
        /// <summary>
        /// Масса кубического метра, кг/куб.м (плотность)
        /// Может иметь несколько значений, через запятую
        /// </summary>
        public string CMass { get; set; }
        
        /// <summary>
        /// Масса квадратного метра, кг/кв.м
        /// </summary>
        public string SMass { get; set; }
        
        /// <summary>
        /// Значение, указывающее на нужность ввода размеров для элемента
        /// Может не быть!
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Парсинг документа из Xml в класс BaseDocument
        /// </summary>
        /// <param name="dbName">Имя базы данных</param>
        /// <returns>Документ вида BaseDocument</returns>
        public bool InitCurrentElement(string dbName)
        {
            try
            {
                if (XmlDocument != null)
                {
                    IsSelected = false;
                    DataBaseName = dbName;
                    Id = int.Parse(XmlDocument.Attribute("Id").Value);
                    DocumentType = XmlDocument.Attribute("DocType")?.Value;
                    DocumentNumber = XmlDocument.Attribute("DocNum")?.Value;
                    DocumentName = XmlDocument.Attribute("DocName")?.Value;
                    DocumentShortName = XmlDocument.Attribute("DocNameShort")?.Value;
                    
                    // Статус документа: 3 варианта - действует, не действует, нет данных
                    bool? docStatus = null;
                    if (XmlDocument.Attribute("DocStatus") != null)
                    {
                        if (bool.TryParse(XmlDocument.Attribute("DocStatus")?.Value, out var b))
                            docStatus = b;
                    }

                    DocStatus = docStatus;
                    ShortName = XmlDocument.Attribute("ShortName")?.Value;
                    Group = XmlDocument.Attribute("Group")?.Value;
                    Image = XmlDocument.Attribute("Image")?.Value;
                    Rule = XmlDocument.Attribute("Rule")?.Value;
                    var size = XmlDocument.Attribute("Size");
                    Size = size?.Value ?? "нет";
                    ItemTypes = GeTypes(XmlDocument);

                    // Типы (для отрисовки)
                    var coType = XmlDocument.Attribute("CoType");
                    CoType = coType?.Value ?? string.Empty;
                    var meType = XmlDocument.Attribute("MeType");
                    MeType = meType?.Value ?? string.Empty;
                    var maType = XmlDocument.Attribute("MaType");
                    MaType = maType?.Value ?? string.Empty;
                    var wdType = XmlDocument.Attribute("WdType");
                    WdType = wdType?.Value ?? string.Empty;
                    var otType = XmlDocument.Attribute("OtType");
                    OtType = otType?.Value ?? string.Empty;

                    // Массы
                    var mass = XmlDocument.Attribute("Mass");
                    Mass = mass?.Value ?? string.Empty;
                    var wMass = XmlDocument.Attribute("WMass");
                    WMass = wMass?.Value ?? string.Empty;
                    var cMass = XmlDocument.Attribute("CMass");
                    CMass = cMass?.Value ?? string.Empty;
                    var sMass = XmlDocument.Attribute("SMass");
                    SMass = sMass?.Value ?? string.Empty;

                    // Steel
                    var hasSteelAtt = XmlDocument.Attribute("HasSteel");
                    HasSteel = hasSteelAtt != null && bool.Parse(hasSteelAtt.Value);

                    SymbolCount = GetSymbolCount(XmlDocument);
                    if (SymbolCount != 0)
                    {
                        Symbols = GetSymbols(XmlDocument, SymbolCount);
                        Items = new XElement("Items");
                        foreach (var xElement in XmlDocument.Elements("Item"))
                        {
                            Items.Add(xElement);
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Получение количества описывающих символов
        /// </summary>
        /// <param name="doc">Xml-документ</param>
        /// <returns>Кол-во символов. 0, если нет атрибута</returns>
        private static int GetSymbolCount(XElement doc)
        {
            var symbAttr = doc.Attribute("SymbolSum");
            if (symbAttr != null)
            {
                int cnt;
                return int.TryParse(symbAttr.Value, out cnt) ? cnt : 0;
            }

            return 0;
        }

        /// <summary>
        /// Получение описывающих символов (заголовки таблицы)
        /// </summary>
        /// <param name="doc">Xml-документ</param>
        /// <param name="symbolCount">Кол-во описывающих элементов</param>
        /// <returns>Коллекция строковых значений. Null - если колв-во в коллекции 0</returns>
        private static IEnumerable<string> GetSymbols(XElement doc, int symbolCount)
        {
            var coll = new List<string>();
            for (var i = 1; i <= symbolCount; i++)
            {
                var att = doc.Attribute("Symbol" + i);
                if (att != null) 
                    coll.Add(att.Value);
            }

            return !coll.Any() ? null : coll;
        }
        
        /// <summary>
        /// Класс, описывающий дополнительные типы в документе
        /// например класс арматуры, класс бетона и т.п.
        /// т.е. типы - это свойства, которые не являются табличными
        /// но их можно задавать
        /// </summary>
        public class ItemType : IEquatable<ItemType>
        {
            public ItemType()
            {
                TypeValues = new ObservableCollection<string>();
            }

            /// <summary>
            /// Имя типа (имя атрибута)
            /// </summary>
            public string TypeName { get; set; }

            /// <summary>
            /// Заголовок (то, что отображается пользователю)
            /// </summary>
            public string TypeHeader { get; set; }

            /// <summary>
            /// Коллекция значений типа
            /// </summary>
            public ObservableCollection<string> TypeValues { get; set; }

            public string SelectedItem { get; set; }

            public bool Equals(ItemType other)
            {
                return TypeValuesEqual(TypeValues, other?.TypeValues) &&
                       TypeHeader.Equals(other?.TypeHeader) &&
                       TypeName.Equals(other?.TypeName) &&
                       SelectedItem.Equals(other?.SelectedItem);
            }

            private static bool TypeValuesEqual(IEnumerable<string> typeValues1, IList<string> typeValues2)
            {
                return !typeValues1.Where((t, i) => !t.Equals(typeValues2[i])).Any();
            }

            /// <summary>
            /// Дополнительное примечание для типов и видимость
            /// </summary>
            public string TypeToolTip { get; set; }

            public bool TypeToolTipVisibility { get; set; }
        }
        
        /// <summary>
        /// Получение типов для документа
        /// </summary>
        /// <param name="doc">Xml-документ</param>
        /// <returns>Коллекция типов. Может быть пустой</returns>
        private static ObservableCollection<ItemType> GeTypes(XElement doc)
        {
            var coll = new ObservableCollection<ItemType>();

            // Проходим по атрибутам в документе
            foreach (var attribute in doc.Attributes())
            {
                // Если имя атрибута содержит ItemType
                if (attribute.Name.ToString().Contains("ItemType") & !attribute.Name.ToString().Contains("ToolTip"))
                {
                    // Значение атрибута - это список, разделенный знаком $, в котором первое значение - имя типа
                    var values = attribute.Value.Split('$');

                    // Создаем новое значение ItemType, заполняем его и добавляем в коллекцию
                    var newItemType = new ItemType { TypeHeader = values[0], TypeName = attribute.Name.ToString() };
                    for (var i = 1; i < values.Count(); i++)
                    {
                        newItemType.TypeValues.Add(values[i]);
                    }

                    // Ищем для этого типа примечания
                    newItemType.TypeToolTipVisibility = GetTypeToolTip(doc, attribute.Name.ToString(), out string itemTypeToolTip);
                    newItemType.TypeToolTip = itemTypeToolTip;

                    // adding
                    coll.Add(newItemType);
                }
            }

            return coll;
        }

        private static bool GetTypeToolTip(XElement doc, string itemType, out string itemTypeToolTip)
        {
            itemTypeToolTip = string.Empty;
            try
            {
                // Проходим по атрибутам в документе
                foreach (var attribute in doc.Attributes())
                {
                    // Если имя атрибута содержит имя атрибута типа ItemType
                    if (attribute.Name.ToString().Contains(itemType) & attribute.Name.ToString().Contains("ToolTip"))
                    {
                        // Значение атрибута разделено знаком $, который означает переход на строчку
                        if (attribute.Value.Contains("$"))
                        {
                            var tooltip = attribute.Value.Split('$');
                            foreach (var s in tooltip)
                            {
                                itemTypeToolTip += s + Environment.NewLine;
                            }

                            itemTypeToolTip = itemTypeToolTip.TrimEnd(Environment.NewLine.ToCharArray());
                            return true;
                        }

                        itemTypeToolTip = attribute.Value;
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
