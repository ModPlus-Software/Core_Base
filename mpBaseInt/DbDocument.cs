namespace mpBaseInt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Документ базы
    /// </summary>
    public class DbDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbDocument"/> class.
        /// </summary>
        /// <param name="xmlDocument">Текущий документ в виде Xml-элемента</param>
        public DbDocument(XElement xmlDocument)
        {
            XmlDocument = xmlDocument;
        }

        /// <summary>
        /// Текущий документ в виде Xml-элемента
        /// </summary>
        public XElement XmlDocument { get; }

        /// <summary>
        /// Имя базы данных к которому относится элемент
        /// нужно для работы из других функций
        /// </summary>
        public string DataBaseName { get; private set; }

        /// <summary>
        /// ID документа
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// Тип документа (ГОСТ, Серия и т.п.)
        /// </summary>
        public string DocumentType { get; private set; }
        
        /// <summary>
        /// Номер документа
        /// </summary>
        public string DocumentNumber { get; private set; }
        
        /// <summary>
        /// Название документа
        /// </summary>
        public string DocumentName { get; private set; }
        
        /// <summary>
        /// Статус документа
        /// </summary>
        public DbDocumentStatus DocStatus { get; private set; }
        
        /// <summary>
        /// Краткое название документа для обобщения (Двутавры, Прокат листовой, Сваи железобетонные и т.д.)
        /// </summary>
        public string DocumentShortName { get; private set; }
        
        /// <summary>
        /// Короткое название изделия (Уголок, двутавр, швеллер и т.п.)
        /// </summary>
        public string ShortName { get; private set; }
        
        /// <summary>
        /// Группа (балки, колонны, профили, прокат и т.д.)
        /// </summary>
        public string Group { get; private set; }
        
        /// <summary>
        /// Кол-во описывающих символов = кол-во столбцов в таблице
        /// </summary>
        public int SymbolCount { get; private set; }
        
        /// <summary>
        /// Описывающие символы (заголовки таблицы)
        /// </summary>
        public IEnumerable<string> Symbols { get; private set; }
        
        /// <summary>
        /// Значения в файле в виде xml
        /// </summary>
        public XElement Items { get; private set; }
        
        /// <summary>
        /// Название файла изображения (без расширения). Может быть пустым
        /// </summary>
        public string Image { get; private set; }
        
        /// <summary>
        /// Коллекция дополнительных свойств, которые не входят в таблицу, но могут быть выбраны
        /// </summary>
        public List<ItemType> ItemTypes { get; private set; }
        
        /// <summary>
        /// Имеет ли элемент свойство "Сталь"
        /// </summary>
        public bool HasSteel { get; private set; }
        
        /// <summary>
        /// Правило написания наименования
        /// представляет собой строку с именами атрибутов, взятых в []
        /// </summary>
        public string Rule { get; private set; }
        
        /// <summary>
        /// Является-ли элемент Железобетонным (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string CoType { get; private set; }
        
        /// <summary>
        /// Является-ли элемент Металлическим (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string MeType { get; private set; }
        
        /// <summary>
        /// Является-ли элемент Деревянным (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string WdType { get; private set; }
        
        /// <summary>
        /// Является-ли элемент Материалом (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string MaType { get; private set; }
        
        /// <summary>
        /// Является-ли элемент Прочим (для отрисовки)
        /// Если свойства нет, значит не является или нет варианта отрисовки
        /// </summary>
        public string OtType { get; private set; }
        
        /// <summary>
        /// Масса элемента, кг
        /// </summary>
        public string Mass { get; private set; }
        
        /// <summary>
        /// Масса погонного метра, кг/п.м
        /// </summary>
        public string WMass { get; private set; }
        
        /// <summary>
        /// Масса кубического метра, кг/куб.м (плотность)
        /// Может иметь несколько значений, через запятую
        /// </summary>
        public string CMass { get; private set; }
        
        /// <summary>
        /// Масса квадратного метра, кг/кв.м
        /// </summary>
        public string SMass { get; private set; }
        
        /// <summary>
        /// Значение, указывающее на нужность ввода размеров для элемента
        /// Может не быть!
        /// </summary>
        public string Size { get; private set; }

        /// <summary>
        /// Имеются ли данные для отрисовки. Метод проверяет значение свойств <see cref="CoType"/>, <see cref="MeType"/>,
        /// <see cref="MaType"/>, <see cref="WdType"/>, <see cref="OtType"/> в зависимости от значения свойства <see cref="DataBaseName"/>
        /// </summary>
        public bool HasDrawType()
        {
            switch (DataBaseName)
            {
                case "DbConcrete":
                    return !string.IsNullOrEmpty(CoType);
                case "DbMetall":
                    return !string.IsNullOrEmpty(MeType);
                case "DbMaterial":
                    return !string.IsNullOrEmpty(MaType);
                case "DbWood":
                    return !string.IsNullOrEmpty(WdType);
                case "DbOther":
                    return !string.IsNullOrEmpty(OtType);
                default:
                    throw new ArgumentException($"Unknown value of {DataBaseName}");
            }
        }

        /// <summary>
        /// Инициализация документа из Xml
        /// </summary>
        /// <param name="dbSection">Раздел базы</param>
        internal bool InitCurrentElement(IDbSection dbSection)
        {
            try
            {
                if (XmlDocument != null)
                {
                    DataBaseName = dbSection.Name;
                    Id = int.Parse(XmlDocument.Attribute("Id")?.Value ?? string.Empty);
                    DocumentType = XmlDocument.Attribute("DocType")?.Value;
                    DocumentNumber = XmlDocument.Attribute("DocNum")?.Value;
                    DocumentName = XmlDocument.Attribute("DocName")?.Value;
                    DocumentShortName = XmlDocument.Attribute("DocNameShort")?.Value;
                    
                    // Статус документа: 3 варианта - действует, не действует, нет данных
                    var docStatus = DbDocumentStatus.NoData;
                    if (XmlDocument.Attribute("DocStatus") != null)
                    {
                        if (bool.TryParse(XmlDocument.Attribute("DocStatus")?.Value, out var b))
                            docStatus = b ? DbDocumentStatus.Valid : DbDocumentStatus.NotValid;
                    }

                    DocStatus = docStatus;
                    ShortName = XmlDocument.Attribute("ShortName")?.Value;
                    Group = XmlDocument.Attribute("Group")?.Value;
                    Image = XmlDocument.Attribute("Image")?.Value;
                    Rule = XmlDocument.Attribute("Rule")?.Value;
                    var size = XmlDocument.Attribute("Size");
                    Size = size?.Value ?? "нет";
                    ItemTypes = GeItemTypes();

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

                    SymbolCount = GetSymbolCount();
                    if (SymbolCount != 0)
                    {
                        Symbols = GetSymbols(SymbolCount);
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
        /// <returns>Кол-во символов. 0, если нет атрибута</returns>
        private int GetSymbolCount()
        {
            var symbolsAttr = XmlDocument.Attribute("SymbolSum");
            if (symbolsAttr != null)
            {
                return int.TryParse(symbolsAttr.Value, out var cnt) ? cnt : 0;
            }

            return 0;
        }

        /// <summary>
        /// Получение описывающих символов (заголовки таблицы)
        /// </summary>
        /// <param name="symbolCount">Кол-во описывающих элементов</param>
        /// <returns>Коллекция строковых значений. Null - если количество в коллекции 0</returns>
        private IEnumerable<string> GetSymbols(int symbolCount)
        {
            var coll = new List<string>();
            for (var i = 1; i <= symbolCount; i++)
            {
                var att = XmlDocument.Attribute("Symbol" + i);
                if (att != null) 
                    coll.Add(att.Value);
            }

            return !coll.Any() ? null : coll;
        }

        /// <summary>
        /// Получение типов для документа
        /// </summary>
        /// <returns>Коллекция типов. Может быть пустой</returns>
        private List<ItemType> GeItemTypes()
        {
            var coll = new List<ItemType>();

            // Проходим по атрибутам в документе
            foreach (var attribute in XmlDocument.Attributes())
            {
                // Если имя атрибута содержит ItemType
                if (attribute.Name.ToString().Contains("ItemType") & !attribute.Name.ToString().Contains("ToolTip"))
                {
                    // Значение атрибута - это список, разделенный знаком $, в котором первое значение - имя типа
                    var values = attribute.Value.Split('$');

                    // Создаем новое значение ItemType, заполняем его и добавляем в коллекцию
                    var newItemType = new ItemType(attribute.Name.ToString(), values[0]);

                    for (var i = 1; i < values.Length; i++)
                    {
                        newItemType.TypeValues.Add(values[i]);
                    }

                    // Ищем для этого типа примечания
                    var isVisibleTypeTooltip = GetTypeToolTip(attribute.Name.ToString(), out var itemTypeToolTip);
                    newItemType.SetToolTip(itemTypeToolTip, isVisibleTypeTooltip);

                    // adding
                    coll.Add(newItemType);
                }
            }

            return coll;
        }

        private bool GetTypeToolTip(string itemType, out string itemTypeToolTip)
        {
            itemTypeToolTip = string.Empty;
            try
            {
                // Проходим по атрибутам в документе
                foreach (var attribute in XmlDocument.Attributes())
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
