namespace mpProductInt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using mpBaseInt;

    /// <summary>
    /// Экземпляр продукта базы
    /// </summary>
    public class DbProduct : IEquatable<DbProduct>
    {
        /// <summary>
        /// Документ в базе данных для этого изделия
        /// </summary>
        public DbDocument BaseDocument { get; set; }

        /// <summary>
        /// Длина изделия. Возможно Null
        /// </summary>
        public double? Length { get; set; }

        /// <summary>
        /// Диаметр изделия. Возможно Null
        /// </summary>
        public double? Diameter { get; set; }

        /// <summary>
        /// Высота изделия. Возможно Null
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Ширина изделия. Возможно Null
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Позиция изделия. Может не быть
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Документ на сталь. Может не быть
        /// </summary>
        public string SteelDoc { get; set; }

        /// <summary>
        /// Марка стали. Может не быть
        /// </summary>
        public string SteelType { get; set; }

        /// <summary>
        /// Масса элемента в кг
        /// </summary>
        public double? Mass { get; set; }

        /// <summary>
        /// Масса погонного метра, кг/п.м
        /// </summary>
        public double? WMass { get; set; }

        /// <summary>
        /// Масса кубического метра, кг/куб.м (плотность)
        /// </summary>
        public double? CMass { get; set; }

        /// <summary>
        /// Масса квадратного метра, кг/кв.м
        /// </summary>
        public double? SMass { get; set; }

        /// <summary>
        /// Выбранный элемент в таблице. Может не быть (например, если размеры задаются)
        /// </summary>
        public XElement Item { get; set; }

        /// <summary>
        /// Коллекция дополнительных свойств, которые не входят в таблицу, но могут быть выбраны
        /// Могут не быть
        /// </summary>
        public List<ItemType> ItemTypes { get; set; }

        /// <summary>
        /// Получение данных для отрисовки
        /// Данные будем сразу конвертировать в нужные значения
        /// </summary>
        /// <returns>Список данных для отрисовки или Null</returns>
        public List<object> DrawData()
        {
            var typeFromAtt = new List<string>();
            switch (BaseDocument.DataBaseName)
            {
                case "DbConcrete":
                    typeFromAtt = BaseDocument.CoType.Split(',').ToList(); break;
                case "DbMetall":
                    typeFromAtt = BaseDocument.MeType.Split(',').ToList(); break;
                case "DbWood":
                    typeFromAtt = BaseDocument.WdType.Split(',').ToList(); break;
                case "DbMaterial":
                    typeFromAtt = BaseDocument.MaType.Split(',').ToList(); break;
                case "DbOther":
                    typeFromAtt = BaseDocument.OtType.Split(',').ToList(); break;
            }

            var drawData = new List<object>();

            // Получили список значений в файле изделия
            // Этот список может содержать число, ссылку на атрибут или значение указанного введенного размера
            for (var k = 0; k < typeFromAtt.Count; k++)
            {
                // Первое значение в списке - это ВСЕГДА вариант отрисовки!
                if (k == 0)
                {
                    drawData.Insert(k, typeFromAtt[k]);
                }
                else
                {
                    if (double.TryParse(typeFromAtt[k], out var d)) //// Если это число
                    {
                        drawData.Insert(k, d);
                    }
                    else if (typeFromAtt[k].Equals("D")) //// Если это диаметр
                    {
                        drawData.Insert(k, Diameter);
                    }
                    else if (typeFromAtt[k].Equals("L")) //// Если это длина
                    {
                        drawData.Insert(k, Length);
                    }
                    else if (typeFromAtt[k].Equals("B")) //// Если это ширина
                    {
                        drawData.Insert(k, Width);
                    }
                    else if (typeFromAtt[k].Equals("H")) //// Если это высота (толщина)
                    {
                        drawData.Insert(k, Height);
                    }
                    else if (typeFromAtt[k].Contains("ItemType")) //// Если размер берется по доп. свойству
                    {
                        foreach (ItemType itemType in ItemTypes)
                        {
                            if (itemType.TypeName.Equals(typeFromAtt[k]))
                            {
                                drawData.Insert(k, double.TryParse(itemType.SelectedValue, out d) ? d : 0.0);
                            }
                        }
                    }
                    else
                    {
                        // ЗАМЕНА ЗАПЯТОЙ НА ТОЧКУ!!!
                        // Иначе это имя атрибута
                        // Значение атрибута может содержать число со звездочкой или ничего не содержать (или знак "-")
                        var propValue = Item.Attribute(typeFromAtt[k])?.Value.Replace(',', '.').Replace("*", string.Empty);
                        drawData.Insert(k, double.TryParse(propValue, out d) ? d : 0.0);
                    }
                }
            }

            return drawData;
        }

        /// <summary>
        /// Получение марки изделия согласно правила
        /// </summary>
        /// <returns></returns>
        public string GetNameByRule()
        {
            var brkResult = BreakString(BaseDocument.Rule, '[', ']');
            var sb = new StringBuilder();

            // Проходим по списку знаков сверяя его с атрибутами (и не только)
            foreach (var c in brkResult)
            {
                // Добавляем вспомогательную переменную
                var appended = false;

                // Проходим по атрибутам в документе
                foreach (var docAttr in BaseDocument.XmlDocument.Attributes())
                {
                    if (docAttr.Name.ToString().Equals(c) && !docAttr.Name.ToString().Contains("ItemType"))
                    {
                        sb.Append(docAttr.Value);
                        appended = true;
                        break;
                    }
                }

                // проходим по ItemTypes
                if (BaseDocument.ItemTypes.Count > 0)
                {
                    foreach (var itemType in ItemTypes)
                    {
                        if (itemType.TypeName.Equals(c))
                        {
                            sb.Append(itemType.SelectedValue);
                            appended = true;
                            break;
                        }
                    }
                }

                if (Item != null) //// Если выбран табличный элемент
                {
                    foreach (var attribute in Item.Attributes())
                    {
                        if (attribute.Name.ToString().Equals(c))
                        {
                            sb.Append(attribute.Value);
                            appended = true;
                            break;
                        }
                    }
                }

                // Если это указанный размер
                if (c.Equals("B"))
                {
                    sb.Append(Width);
                    appended = true;
                }

                if (c.Equals("H"))
                {
                    sb.Append(Height);
                    appended = true;
                }

                if (c.Equals("L"))
                {
                    sb.Append(Length);
                    appended = true;
                }

                if (c.Equals("D"))
                {
                    sb.Append(Diameter);
                    appended = true;
                }

                // Если предыдущие проверки не дали результат, значит это просто текст
                if (!appended)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Вспомогательная функция разбивки строки на список
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="symbol1">Первый ограничивающий символ</param>
        /// <param name="symbol2">Второй ограничивающий символ</param>
        /// <returns></returns>
        private static IEnumerable<string> BreakString(string str, char symbol1, char symbol2)
        {
            var result = new List<string>();
            var k = -1;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i].Equals(symbol1))
                {
                    if (sb.Length > 0)
                    {
                        result.Insert(k, sb.ToString());
                    }

                    sb = new StringBuilder();
                    if (i > 1)
                    {
                        if (!str[i - 1].Equals(symbol2))
                        {
                            k++;
                        }
                    }
                }
                else if (str[i].Equals(symbol2))
                {
                    result.Insert(k, sb.ToString());
                    sb = new StringBuilder();
                    k++;
                }
                else
                {
                    if (k == -1)
                    {
                        k++;
                    }

                    sb.Append(str[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает экземпляр класса <see cref="MpProductToSave"/>
        /// </summary>
        public MpProductToSave SetProductToSave()
        {
            var prToSave = new MpProductToSave
            {
                DocumentId = BaseDocument.Id,
                DbName = BaseDocument.DataBaseName,
                Length = Length,
                Diameter = Diameter,
                Width = Width,
                Height = Height,
                SteelDoc = SteelDoc,
                SteelType = SteelType,
                Position = Position,
                Mass = Mass,
                WMass = WMass,
                CMass = CMass,
                SMass = SMass
            };
            if (BaseDocument.Items != null && BaseDocument.Items.Elements("Item").Any())
            {
                prToSave.IndexOfItem = BaseDocument.Items.Elements("Item").ToList().IndexOf(Item);
            }
            else
            {
                prToSave.IndexOfItem = -1;
            }

            if (ItemTypes != null)
            {
                var values = ItemTypes.Aggregate(
                    string.Empty,
                    (current, itemType) => current + (itemType.SelectedValue + "$"));
                prToSave.ItemTypesValues = values.TrimEnd('$');
            }
            else
            {
                prToSave.ItemTypesValues = string.Empty;
            }

            return prToSave;
        }

        /// <summary>
        /// Создание экземпляра <see cref="DbProduct"/> по данным экземпляра класса хранения данных <see cref="MpProductToSave"/>
        /// </summary>
        /// <param name="savedProduct">Экземпляр <see cref="MpProductToSave"/></param>
        public static DbProduct GetProductFromSaved(MpProductToSave savedProduct)
        {
            var product = new DbProduct
            {
                BaseDocument = Utils.GetBaseDocumentById(savedProduct.DbName, savedProduct.DocumentId),
                Length = savedProduct.Length,
                Diameter = savedProduct.Diameter,
                Width = savedProduct.Width,
                Height = savedProduct.Height,
                SteelType = savedProduct.SteelType,
                SteelDoc = savedProduct.SteelDoc,
                Position = savedProduct.Position,
                Mass = savedProduct.Mass,
                WMass = savedProduct.WMass,
                CMass = savedProduct.CMass,
                SMass = savedProduct.SMass
            };

            // Может быть вариант, что продукт "сделан" из атрибутов, тогда ссылки на базу не будет! И дальнейшие действия не нужны
            if (product.BaseDocument != null)
            {
                product.ItemTypes = product.BaseDocument.ItemTypes;
                if (savedProduct.IndexOfItem != null)
                {
                    product.Item = savedProduct.IndexOfItem != -1
                          ? product.BaseDocument.Items.Elements("Item").ElementAt(savedProduct.IndexOfItem.Value)
                          : null;
                }

                if (!string.IsNullOrEmpty(savedProduct.ItemTypesValues))
                {
                    var itv = savedProduct.ItemTypesValues.Split('$').ToList();
                    for (var i = 0; i < itv.Count; i++)
                    {
                        product.ItemTypes[i].SelectedValue = itv[i];
                    }
                }
            }

            return product;
        }

        /// <summary>
        /// Получение массы изделия
        /// </summary>
        public double? GetProductMass()
        {
            double? mass = null;

            // Первый случай: масса элемента есть
            if (Mass != null)
            {
                mass = Mass;
            }
            else
            {
                // Второй случай: массы нет, есть масса в кг/п.м    
                if (WMass != null & Length != null)
                {
                    mass = WMass * Length / 1000;
                }

                // Если есть масса в кг/кв.м
                else if (SMass != null & Width != null & Length != null)
                {
                    mass = Width / 1000 * Length / 1000 * SMass;
                }

                // Если есть масса в кг/куб.м
                else if (CMass != null)
                {
                    // Если цилиндр
                    if (Diameter != null & Length != null)
                    {
                        mass = Math.PI * Math.Pow(Diameter.Value / 2 / 1000, 2) * Length / 1000 * CMass;
                    }

                    // Если прямоугольник
                    if (Width != null & Height != null & Length != null)
                    {
                        mass = Width / 1000 * Height / 1000 * Length / 1000 * CMass;
                    }
                }
            }

            return mass;
        }

        /// <summary>
        /// Конвертирование продукта в класс <see cref="SpecificationItem"/> для заполнения спецификации
        /// </summary>
        /// <param name="count">Свойство "Количество"</param>
        public Specification.SpecificationItem GetSpecificationItem(double? count)
        {
            Specification.SpecificationItem specificationItem;
            var dbIndex = 0;
            var dataBaseName = BaseDocument.DataBaseName;
            switch (dataBaseName)
            {
                case "DbMetall":
                    dbIndex = 0;
                    break;
                case "DbConcrete":
                    dbIndex = 1;
                    break;
                case "DbWood":
                    dbIndex = 2;
                    break;
                case "DbMaterial":
                    dbIndex = 3;
                    break;
                case "DbOther":
                    dbIndex = 4;
                    break;
            }

            var dimension = string.Empty;

            if (Length.HasValue)
            {
                var dimensionAttribute = BaseDocument.XmlDocument.Attribute("DimType");
                if (dimensionAttribute != null)
                {
                    if (dimensionAttribute.Value.Contains("Длина"))
                    {
                        dimension = "Длина";
                    }
                }
            }

            if (!BaseDocument.HasSteel)
            {
                specificationItem = new Specification.SpecificationItem(this, string.Empty, string.Empty, dimension)
                {
                    DbIndex = dbIndex
                };
            }
            else
            {
                specificationItem = new Specification.SpecificationItem(this, SteelDoc, SteelType, dimension)
                {
                    DbIndex = dbIndex
                };
            }

            specificationItem.Mass = GetProductMass();

            if (count.HasValue)
            {
                specificationItem.Count = count.ToString();
            }

            specificationItem.Position = Position;
            
            return specificationItem;
        }

        /// <summary>
        /// Сравнение двух экземпляров класса
        /// </summary>
        /// <param name="other">Сравниваемый экземпляр <see cref="DbProduct"/></param>
        public bool Equals(DbProduct other)
        {
            if (other != null && 
                Length.Equals(other.Length) &&
                Width.Equals(other.Width) &&
                Height.Equals(other.Height) &&
                Diameter.Equals(other.Diameter) &&
                Position.Equals(other.Position) &&
                SteelDoc.Equals(other.SteelDoc) &&
                SteelType.Equals(other.SteelType) &&
                ItemTypesEqual(ItemTypes, other.ItemTypes) &&
                Mass.Equals(other.Mass) &&
                CMass.Equals(other.CMass) &&
                WMass.Equals(other.WMass) &&
                SMass.Equals(other.SMass))
            {
                return true;
            }

            return false;
        }

        private static bool ItemTypesEqual(IList<ItemType> itemTypes1, IList<ItemType> itemTypes2)
        {
            if (itemTypes1.Count != itemTypes2.Count)
            {
                return false;
            }

            for (var i = 0; i < itemTypes1.Count; i++)
            {
                if (!itemTypes1[i].Equals(itemTypes2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
