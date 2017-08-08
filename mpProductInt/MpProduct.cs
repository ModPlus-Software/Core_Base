using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Visibility = System.Windows.Visibility;
using mpBaseInt;
using mpConcrete;
using mpMaterial;
using mpMetall;
using mpOther;
using mpWood;

namespace mpProductInt
{
    /// <summary>
    /// Класс для изделия
    /// </summary>
    public class MpProduct : IEquatable<MpProduct>
    {
        /// <summary>
        /// Документ в базе данных для этого изделия
        /// </summary>
        public BaseDocument BaseDocument { get; set; }
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
        public ObservableCollection<BaseDocument.ItemType> ItemTypes { get; set; }
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
                    drawData.Insert(k, typeFromAtt[k]);
                else
                {
                    double d;
                    if (double.TryParse(typeFromAtt[k], out d)) // Если это число
                        drawData.Insert(k, d);
                    else if (typeFromAtt[k].Equals("D")) // Если это диаметр
                        drawData.Insert(k, Diameter);
                    else if (typeFromAtt[k].Equals("L")) // Если это длина
                        drawData.Insert(k, Length);
                    else if (typeFromAtt[k].Equals("B")) // Если это ширина
                        drawData.Insert(k, Width);
                    else if (typeFromAtt[k].Equals("H")) // Если это высота (толщина)
                        drawData.Insert(k, Height);
                    else if (typeFromAtt[k].Contains("ItemType"))// Если размер берется по доп. свойству
                    {
                        foreach (BaseDocument.ItemType itemType in ItemTypes)
                        {
                            if (itemType.TypeName.Equals(typeFromAtt[k]))
                            {
                                drawData.Insert(k, double.TryParse(itemType.SelectedItem, out d) ? d : 0.0);
                            }
                        }
                    }
                    else
                    {
                        // ЗАМЕНА ЗАПЯТОЙ НА ТОЧКУ!!!
                        // Иначе это имя атрибута
                        // Значение атрибута может содержать число со звездочкой
                        // или ничего не содержать (или знак "-")
                        var propValue = Item.Attribute(typeFromAtt[k]).Value.Replace(',', '.').Replace("*", "");
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
            foreach (var _char in brkResult)
            {
                // Добавляем вспомогательгую переменную
                var appended = false;
                // Проходим по атрибутам в документе
                foreach (var docAttr in BaseDocument.XmlDocument.Attributes())
                {
                    if (docAttr.Name.ToString().Equals(_char) && !docAttr.Name.ToString().Contains("ItemType"))
                    {
                        sb.Append(docAttr.Value);
                        appended = true;
                        break;
                    }
                }
                // проходим по ItemTypes
                if (BaseDocument.ItemTypes.Count > 0)
                    foreach (var itemType in ItemTypes)
                    {
                        if (itemType.TypeName.Equals(_char))
                        {
                            sb.Append(itemType.SelectedItem);
                            appended = true;
                            break;
                        }
                    }

                if (Item != null) // Если выбран табличный элемент
                {
                    foreach (var attribute in Item.Attributes())
                    {
                        if (attribute.Name.ToString().Equals(_char))
                        {
                            sb.Append(attribute.Value);
                            appended = true;
                            break;
                        }
                    }
                }
                // Если это указанный размер
                if (_char.Equals("B"))
                {
                    sb.Append(Width);
                    appended = true;
                }
                if (_char.Equals("H"))
                {
                    sb.Append(Height);
                    appended = true;
                }
                if (_char.Equals("L"))
                {
                    sb.Append(Length);
                    appended = true;
                }
                if (_char.Equals("D"))
                {
                    sb.Append(Diameter);
                    appended = true;
                }
                // Если предыдущие проверки не дали результат, значит это просто текст
                if (!appended) sb.Append(_char);
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
                        result.Insert(k, sb.ToString());
                    sb = new StringBuilder();
                    if (i > 1)
                        if (!str[i - 1].Equals(symbol2))
                            k++;
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
                        k++;
                    sb.Append(str[i]);
                }
            }
            return result;
        }

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
                prToSave.IndexOfItem = BaseDocument.Items.Elements("Item").ToList().IndexOf(Item);
            else prToSave.IndexOfItem = -1;
            if (ItemTypes != null)
            {
                var values = ItemTypes.Aggregate(string.Empty,
                    (current, itemType) => current + (itemType.SelectedItem + "$"));
                prToSave.ItemTypesValues = values.TrimEnd('$');
            }
            else prToSave.ItemTypesValues = string.Empty;

            return prToSave;
        }

        public static MpProduct GetProductFromSaved(MpProductToSave savedProduct)
        {
            var product = new MpProduct
            {
                BaseDocument = GetBaseDocumentById(savedProduct.DbName, savedProduct.DocumentId),
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
                    product.Item = savedProduct.IndexOfItem != -1
                        ? product.BaseDocument.Items.Elements("Item").ElementAt(savedProduct.IndexOfItem.Value)
                        : null;
                if (!string.IsNullOrEmpty(savedProduct.ItemTypesValues))
                {
                    var itv = savedProduct.ItemTypesValues.Split('$').ToList();
                    for (var i = 0; i < itv.Count; i++)
                    {
                        product.ItemTypes[i].SelectedItem = itv[i];
                    }
                }
            }
            return product;
        }

        private static BaseDocument GetBaseDocumentById(string dbName, int id)
        {
            switch (dbName)
            {
                case "DbConcrete":
                    {
                        mpConcrete.Concrete.LoadAllDocument();
                        foreach (var baseDocument in mpConcrete.Concrete.DocumentCollection.Where(baseDocument => baseDocument.Id.Equals(id)))
                        {
                            return baseDocument;
                        }
                    }
                    break;
                case "DbMetall":
                    {
                        mpMetall.Metall.LoadAllDocument();
                        foreach (var baseDocument in mpMetall.Metall.DocumentCollection.Where(baseDocument => baseDocument.Id.Equals(id)))
                        {
                            return baseDocument;
                        }
                    }
                    break;
                case "DbWood":
                    {
                        mpWood.Wood.LoadAllDocument();
                        foreach (var baseDocument in mpWood.Wood.DocumentCollection.Where(baseDocument => baseDocument.Id.Equals(id)))
                        {
                            return baseDocument;
                        }
                    }
                    break;
                case "DbMaterial":
                    {
                        mpMaterial.Material.LoadAllDocument();
                        foreach (var baseDocument in mpMaterial.Material.DocumentCollection.Where(baseDocument => baseDocument.Id.Equals(id)))
                        {
                            return baseDocument;
                        }
                    }
                    break;
                case "DbOther":
                    {
                        mpOther.Other.LoadAllDocument();
                        foreach (var baseDocument in mpOther.Other.DocumentCollection.Where(baseDocument => baseDocument.Id.Equals(id)))
                        {
                            return baseDocument;
                        }
                    }
                    break;
            }
            return null;
        }
        /// <summary>
        /// Получение массы изделия
        /// </summary>
        /// <returns></returns>
        public double? GetProductMass()
        {
            double? mass = null;
            // Первый случай: масса элемента есть
            if (Mass != null) mass = Mass;
            else
            {
                // Второй случай: массы нет, есть масса в кг/п.м    
                if (WMass != null & Length != null)
                    mass = WMass * Length / 1000;
                // Если есть масса в кг/кв.м
                else if (SMass != null & Width != null & Length != null)
                    mass = Width / 1000 * Length / 1000 * SMass;
                // Если есть масса в кг/куб.м
                else if (CMass != null)
                {
                    // Если цилиндр
                    if (Diameter != null & Length != null)
                        mass = Math.PI * Math.Pow(Diameter.Value / 2 / 1000, 2) * Length / 1000 * CMass;
                    // Если прямоугольник
                    if (Width != null & Height != null & Length != null)
                        mass = Width / 1000 * Height / 1000 * Length / 1000 * CMass;
                }
            }
            return mass;
        }
        /// <summary>
        /// Конвертирование продукта в класс SpecificationItem для заполнения спецификации
        /// </summary>
        /// <returns></returns>
        public SpecificationItem GetSpecificationItem(double? count)
        {
            SpecificationItem str;
            SpecificationItem specificationItem;
            int num = 0;
            string dataBaseName = this.BaseDocument.DataBaseName;
            if (dataBaseName == "DbMetall")
            {
                num = 0;
            }
            else if (dataBaseName == "DbConcrete")
            {
                num = 1;
            }
            else if (dataBaseName == "DbWood")
            {
                num = 2;
            }
            else if (dataBaseName == "DbMaterial")
            {
                num = 3;
            }
            else if (dataBaseName == "DbOther")
            {
                num = 4;
            }
            var dimension = string.Empty;
            
            if (Length.HasValue)
            {
                var dimensionAttribute = BaseDocument.XmlDocument.Attribute("DimType");
                if (dimensionAttribute != null)
                    if(dimensionAttribute.Value.Contains("Длина"))
                        dimension = "Длина";
            }
            if (!this.BaseDocument.HasSteel)
            {
                specificationItem = new SpecificationItem(
                    this, string.Empty, string.Empty, dimension, string.Empty, 
                    SpecificationItemInputType.DataBase, string.Empty, string.Empty, string.Empty, GetProductMass())
                {
                    DbIndex = num
                };
                str = specificationItem;
            }
            else
            {
                specificationItem = new SpecificationItem(
                    this, this.SteelDoc, this.SteelType, dimension, string.Empty, 
                    SpecificationItemInputType.DataBase, string.Empty, string.Empty, string.Empty, GetProductMass())
                {
                    DbIndex = num
                };
                str = specificationItem;
            }
            if (count.HasValue)
            {
                str.Count = count.ToString();
            }
            str.Position = this.Position;
            return str;
        }
        /// <summary>
        /// Сравнение двух экземпляров класса
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MpProduct other)
        {
            if (Length.Equals(other.Length) &&
                Width.Equals(other.Width) &&
                Height.Equals(other.Height) &&
                Diameter.Equals(other.Diameter) &&
                Position.Equals(other.Position) &&
                SteelDoc.Equals(other.SteelDoc) &&
                SteelType.Equals(other.SteelType) &&
                ItemTypesUqual(ItemTypes, other.ItemTypes) &&
                //ItemTypes.Equals(other.ItemTypes) &&
                Mass.Equals(other.Mass) &&
                CMass.Equals(other.CMass) &&
                WMass.Equals(other.WMass) &&
                SMass.Equals(other.SMass))
                return true;
            return false;
        }

        private static bool ItemTypesUqual(IList<BaseDocument.ItemType> itemTypes1, IList<BaseDocument.ItemType> itemTypes2)
        {
            if (itemTypes1.Count != itemTypes2.Count) return false;
            for (var i = 0; i < itemTypes1.Count; i++)
            {
                if (!itemTypes1[i].Equals(itemTypes2[i])) return false;
            }
            return true;
        }
    }
    /// <summary>
    /// Класс изделия для хранения в расширенных данных
    /// </summary>
    [Serializable]
    public class MpProductToSave
    {
        // Для регистрации в автокаде уникального имени
        public string AppName = "ModPlusProduct";
        // Имя базы данных для более быстрого поиска
        public string DbName { get; set; }
        // Id документа
        public int DocumentId { get; set; }
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
        /// Индекс позиции в списке элементов. Может не быть, если нет элементов (например все размеры вводятся)
        /// </summary>
        public int? IndexOfItem { get; set; }
        /// <summary>
        /// Значения для ItemTypes, сохраненные в виде строки. Разделение знаком $
        /// </summary>
        public string ItemTypesValues { get; set; }
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
    }
    /// <summary>
    /// Класс для позиции в строительной спецификации
    /// </summary>
    public class SpecificationItem : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product">MpProduct, связанный с элементом спецификации</param>
        /// <param name="steelDoc">Документ на сталь</param>
        /// <param name="steelType">Марка стали</param>
        /// <param name="dimension">Вариант измерения</param>
        /// <param name="subsection">Название подраздела. product должен быть null</param>
        /// <param name="inputType">Вид элемента спецификации: из БД, подраздел или ручной ввод</param>
        /// <param name="handBeforeName"></param>
        /// <param name="handTopName"></param>
        /// <param name="handAfterName"></param>
        /// <param name="handMass"></param>
        public SpecificationItem(
            MpProduct product,
            string steelDoc,
            string steelType,
            string dimension,
            string subsection,
            SpecificationItemInputType inputType,
            string handBeforeName,
            string handTopName,
            string handAfterName,
            double? handMass)
        {
            if (product != null)
            {
                Product = product;
                Dimension = dimension;
                InputType = SpecificationItemInputType.DataBase;
                InitSpecificationItem(steelDoc, steelType);
            }
            else
            {
                if (inputType == SpecificationItemInputType.SubSection)
                {
                    Product = null;
                    InputType = SpecificationItemInputType.SubSection;
                    BeforeName = subsection;
                    AfterName = TopName = SteelDoc = SteelType = Dimension = Note = string.Empty;
                    Mass = null;
                    HasSteel = false;
                    SteelVisibility = Visibility.Collapsed;
                }
                else if (inputType == SpecificationItemInputType.HandInput)
                {
                    Product = null;
                    InputType = SpecificationItemInputType.HandInput;
                    BeforeName = handBeforeName;
                    AfterName = handAfterName;
                    TopName = handTopName;
                    Mass = handMass;
                    Dimension = string.Empty;
                    if (!string.IsNullOrEmpty(steelDoc) && !string.IsNullOrEmpty(steelType))
                    {
                        SteelDoc = steelDoc;
                        SteelType = steelType;
                        HasSteel = true;
                        SteelVisibility = Visibility.Visible;
                    }
                    else
                    {
                        HasSteel = false;
                        SteelVisibility = Visibility.Collapsed;
                    }
                }
            }
        }
        /// <summary>
        /// Продукт, связанный с текущим элементом
        /// </summary>
        public MpProduct Product { get; set; }
        /// <summary>
        /// Инициализация элемента спецификации
        /// </summary>
        private void InitSpecificationItem(
            string steelDoc, string steelType
            )
        {
            if (Product != null)
            {
                if (Product.BaseDocument != null)
                {
                    // Designation
                    Designation = Product.BaseDocument.DocumentType + " " + Product.BaseDocument.DocumentNumber;
                    // has steel
                    HasSteel = Product.BaseDocument.HasSteel;
                    SteelVisibility = HasSteel ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    Designation = string.Empty;
                    HasSteel = false;
                    SteelVisibility = Visibility.Collapsed;
                }
                Mass = Product.GetProductMass();

                if (HasSteel)
                {
                    BeforeName = Product.BaseDocument.ShortName;
                    TopName = Product.GetNameByRule();
                    AfterName = GetAfterName();
                    SteelDoc = steelDoc;
                    SteelType = steelType;
                }
                else
                {
                    BeforeName = Product.GetNameByRule() + " " + GetAfterName();
                    TopName = AfterName = SteelDoc = SteelType = string.Empty;
                }
            }
        }

        private string GetAfterName()
        {
            switch (Dimension)
            {
                case "Длина":
                    return "L=" + Product.Length;
                case "п.м":
                    return ", п.м";
                case "":
                    return "";
                default: return string.Empty;
            }
        }

        public SpecificationItemInputType InputType { get; set; }
        /// <summary>
        /// Позиция
        /// </summary>
        private string _position;
        public string Position { get { return _position; } set { _position = value; OnPropertyChanged("Position"); } }
        /// <summary>
        /// Обозначение
        /// </summary>
        public string Designation { get; set; }
        /// <summary>
        /// Первая строка наименования
        /// </summary>
        public string BeforeName { get; set; }
        /// <summary>
        /// Наименование (то, что записано в числителе, если есть сталь. Иначе - все в BeforeName)
        /// </summary>
        public string TopName { get; set; }
        /// <summary>
        /// Вторая строка наименования
        /// </summary>
        public string AfterName { get; set; }
        /// <summary>
        /// Документ на сталь
        /// </summary>
        public string SteelDoc { get; set; }
        /// <summary>
        /// Марка стали
        /// </summary>
        public string SteelType { get; set; }

        /// <summary>
        /// Есть ли сталь
        /// </summary>
        public bool HasSteel { get; set; }

        public Visibility SteelVisibility { get; set; }
        /// <summary>
        /// Вариант измерения
        /// </summary>
        public string Dimension { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        private string _count;
        public string Count { get { return _count; } set { _count = value; OnPropertyChanged("Count"); } }
        /// <summary>
        /// Масса
        /// </summary>
        public double? Mass { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        private string _note;
        public string Note { get { return _note; } set { _note = value; OnPropertyChanged("Note"); } }
        /// <summary>
        /// Индекс базы данных для редактирования элемента
        /// 0 - Металл
        /// 1 - Железобетон
        /// 2 - Дерево
        /// 3 - Материалы
        /// 4 - Прочее
        /// </summary>
        public int DbIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum SpecificationItemInputType
    {
        DataBase = 0, // Элемент из базы данных
        SubSection = 1, // Подраздел
        HandInput = 2 // Ручной ввод
    }

    public static class ConvertingSpecificationToFromXml
    {
        public static SpecificationItem ConvertFromXml(XElement specificationItemXel)
        {
            MpProduct mpProduct = null;
            var productXel = specificationItemXel.Element("Product");
            // Если есть элемент, описывающий Изделие из БД
            if (productXel != null)
            {
                mpProduct = new MpProduct()
                {
                    BaseDocument = GetBaseDocumentById(
                        productXel.Attribute("BaseDocument.DataBaseName")?.Value,
                        int.TryParse(productXel.Attribute("BaseDocument.Id")?.Value, out int inum) ? inum : -1)
                };
                mpProduct.SteelType = productXel.Attribute("SteelType")?.Value;
                mpProduct.Position = productXel.Attribute("Position")?.Value;
                mpProduct.Position = productXel.Attribute("Position")?.Value;
                mpProduct.Length = double.TryParse(productXel.Attribute("Length")?.Value, out double dnum) ? dnum : 0;
                mpProduct.Diameter = double.TryParse(productXel.Attribute("Diameter")?.Value, out dnum) ? dnum : 0;
                mpProduct.Width = double.TryParse(productXel.Attribute("Width")?.Value, out dnum) ? dnum : 0;
                mpProduct.Length = double.TryParse(productXel.Attribute("Height")?.Value, out dnum) ? dnum : 0;
                mpProduct.Mass = double.TryParse(productXel.Attribute("Mass")?.Value, out dnum) ? dnum : 0;
                mpProduct.WMass = double.TryParse(productXel.Attribute("WMass")?.Value, out dnum) ? dnum : 0;
                mpProduct.CMass = double.TryParse(productXel.Attribute("CMass")?.Value, out dnum) ? dnum : 0;
                mpProduct.SMass = double.TryParse(productXel.Attribute("SMass")?.Value, out dnum) ? dnum : 0;
                mpProduct.ItemTypes = mpProduct.BaseDocument.ItemTypes;
                // 
                var indexOfItem = int.TryParse(productXel.Attribute("IndexOfItem")?.Value, out inum) ? inum : -1;
                var productItemXel = indexOfItem != -1 ? mpProduct.BaseDocument.Items.Elements("Item").ElementAt(indexOfItem) : null;
                mpProduct.Item = productItemXel;
                //
                if (!string.IsNullOrEmpty(productXel.Attribute("ItemTypesValues")?.Value))
                {
                    var list = productXel.Attribute("ItemTypesValues")?.Value.Split('$').ToList();
                    for (var i = 0; i < list?.Count; i++)
                    {
                        mpProduct.ItemTypes[i].SelectedItem = list[i];
                    }
                }
            }
            // Остальные значения
            var steelDoc = specificationItemXel.Attribute("SteelDoc")?.Value;
            var steelType = specificationItemXel.Attribute("SteelType")?.Value;
            var dimension = specificationItemXel.Attribute("Dimension")?.Value;

            double? handMass = null;
            if (double.TryParse(specificationItemXel.Attribute("Mass")?.Value, out double d))
                handMass = d;

            var specificationItem = new SpecificationItem(
                mpProduct,
                steelDoc,
                steelType,
                dimension,
                string.Empty,
                GetInputType(specificationItemXel.Attribute("InputType")?.Value),
                specificationItemXel.Attribute("BeforeName")?.Value,
                specificationItemXel.Attribute("TopName")?.Value,
                specificationItemXel.Attribute("AfterName")?.Value,
                handMass
                );

            specificationItem.Position = specificationItemXel.Attribute("Position")?.Value;
            specificationItem.AfterName = specificationItemXel.Attribute("AfterName")?.Value;
            specificationItem.BeforeName = specificationItemXel.Attribute("BeforeName")?.Value;
            specificationItem.Count = specificationItemXel.Attribute("Count")?.Value;
            specificationItem.DbIndex = (int.TryParse(specificationItemXel.Attribute("DbIndex")?.Value, out int integer) ? integer : -1);
            specificationItem.Designation = specificationItemXel.Attribute("Designation")?.Value;
            specificationItem.HasSteel = bool.TryParse(specificationItemXel.Attribute("HasSteel")?.Value, out bool flag) & flag;

            if (double.TryParse(specificationItemXel.Attribute("Mass")?.Value, out double dnumber))
                specificationItem.Mass = dnumber;
            else specificationItem.Mass = null;

            specificationItem.Note = specificationItemXel.Attribute("Note")?.Value;
            specificationItem.TopName = specificationItemXel.Attribute("TopName")?.Value;
            specificationItem.SteelDoc = steelDoc;
            specificationItem.SteelType = steelType;
            specificationItem.AfterName = specificationItemXel.Attribute("AfterName")?.Value;
            specificationItem.SteelVisibility = Enum.TryParse(specificationItemXel.Attribute("SteelVisibility")?.Value, out Visibility visibility) ? visibility : Visibility.Collapsed;

            return specificationItem;
        }

        public static XElement ConvertToXml(SpecificationItem item)
        {
            double? mass;
            XElement xElement = new XElement("SpecificationItem");
            xElement.SetAttributeValue("InputType", item.InputType.ToString());
            xElement.SetAttributeValue("Position", item.Position);
            xElement.SetAttributeValue("AfterName", item.AfterName);
            xElement.SetAttributeValue("BeforeName", item.BeforeName);
            xElement.SetAttributeValue("Count", item.Count);
            xElement.SetAttributeValue("DbIndex", item.DbIndex);
            xElement.SetAttributeValue("Designation", item.Designation);
            xElement.SetAttributeValue("Dimension", item.Dimension);
            xElement.SetAttributeValue("HasSteel", item.HasSteel);
            if (item.Mass.HasValue)
            {
                XName xName = "Mass";
                mass = item.Mass;
                xElement.SetAttributeValue(xName, mass.Value);
            }
            xElement.SetAttributeValue("Note", item.Note);
            xElement.SetAttributeValue("TopName", item.TopName);
            xElement.SetAttributeValue("SteelDoc", item.SteelDoc);
            xElement.SetAttributeValue("SteelType", item.SteelType);
            xElement.SetAttributeValue("SteelVisibility", item.SteelVisibility);
            if (item.Product != null)
            {
                XElement xElement1 = new XElement("Product");
                xElement1.SetAttributeValue("BaseDocument.Id", item.Product.BaseDocument.Id);
                xElement1.SetAttributeValue("BaseDocument.DataBaseName", item.Product.BaseDocument.DataBaseName);
                if (item.Product.Length.HasValue)
                {
                    XName xName1 = "Length";
                    mass = item.Product.Length;
                    xElement1.SetAttributeValue(xName1, mass.Value);
                }
                if (item.Product.Diameter.HasValue)
                {
                    XName xName2 = "Diameter";
                    mass = item.Product.Diameter;
                    xElement1.SetAttributeValue(xName2, mass.Value);
                }
                if (item.Product.Width.HasValue)
                {
                    XName xName3 = "Width";
                    mass = item.Product.Width;
                    xElement1.SetAttributeValue(xName3, mass.Value);
                }
                if (item.Product.Height.HasValue)
                {
                    XName xName4 = "Height";
                    mass = item.Product.Height;
                    xElement1.SetAttributeValue(xName4, mass.Value);
                }
                xElement1.SetAttributeValue("SteelDoc", item.Product.SteelDoc);
                xElement1.SetAttributeValue("SteelType", item.Product.SteelType);
                xElement1.SetAttributeValue("Position", item.Product.Position);
                xElement1.SetAttributeValue("Mass", item.Product.Mass);
                xElement1.SetAttributeValue("WMass", item.Product.WMass);
                xElement1.SetAttributeValue("CMass", item.Product.CMass);
                xElement1.SetAttributeValue("SMass", item.Product.SMass);
                if ((item.Product.BaseDocument.Items == null ? true : !item.Product.BaseDocument.Items.Elements("Item").Any<XElement>()))
                {
                    xElement1.SetAttributeValue("IndexOfItem", -1);
                }
                else
                {
                    xElement1.SetAttributeValue("IndexOfItem", item.Product.BaseDocument.Items.Elements("Item").ToList<XElement>().IndexOf(item.Product.Item));
                }
                if (item.Product.ItemTypes == null)
                {
                    xElement1.SetAttributeValue("ItemTypesValues", string.Empty);
                }
                else
                {
                    string str = item.Product.ItemTypes.Aggregate<BaseDocument.ItemType, string>(string.Empty, (string current, BaseDocument.ItemType itemType) => string.Concat(current, itemType.SelectedItem, "$"));
                    xElement1.SetAttributeValue("ItemTypesValues", str.TrimEnd(new char[] { '$' }));
                }
                xElement.Add(xElement1);
            }
            return xElement;
        }

        private static BaseDocument GetBaseDocumentById(string dbName, int id)
        {
            Func<BaseDocument, bool> func;
            BaseDocument current;
            Func<BaseDocument, bool> func1 = null;
            Func<BaseDocument, bool> func2 = null;
            Func<BaseDocument, bool> func3 = null;
            Func<BaseDocument, bool> func4 = null;
            Func<BaseDocument, bool> func5 = null;
            string str = dbName;
            if (str == "DbConcrete")
            {
                Concrete.LoadAllDocument();
                ICollection<BaseDocument> documentCollection = Concrete.DocumentCollection;
                Func<BaseDocument, bool> func6 = func1;
                if (func6 == null)
                {
                    Func<BaseDocument, bool> func7 = (BaseDocument baseDocument) => baseDocument.Id.Equals(id);
                    func = func7;
                    func1 = func7;
                    func6 = func;
                }
                using (IEnumerator<BaseDocument> enumerator = documentCollection.Where<BaseDocument>(func6).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        return current;
                    }
                }
            }
            else if (str == "DbMetall")
            {
                Metall.LoadAllDocument();
                ICollection<BaseDocument> baseDocuments = Metall.DocumentCollection;
                Func<BaseDocument, bool> func8 = func2;
                if (func8 == null)
                {
                    Func<BaseDocument, bool> func9 = (BaseDocument baseDocument) => baseDocument.Id.Equals(id);
                    func = func9;
                    func2 = func9;
                    func8 = func;
                }
                using (IEnumerator<BaseDocument> enumerator1 = baseDocuments.Where<BaseDocument>(func8).GetEnumerator())
                {
                    if (enumerator1.MoveNext())
                    {
                        current = enumerator1.Current;
                        return current;
                    }
                }
            }
            else if (str == "DbWood")
            {
                Wood.LoadAllDocument();
                ICollection<BaseDocument> documentCollection1 = Wood.DocumentCollection;
                Func<BaseDocument, bool> func10 = func3;
                if (func10 == null)
                {
                    Func<BaseDocument, bool> func11 = (BaseDocument baseDocument) => baseDocument.Id.Equals(id);
                    func = func11;
                    func3 = func11;
                    func10 = func;
                }
                using (IEnumerator<BaseDocument> enumerator2 = documentCollection1.Where<BaseDocument>(func10).GetEnumerator())
                {
                    if (enumerator2.MoveNext())
                    {
                        current = enumerator2.Current;
                        return current;
                    }
                }
            }
            else if (str == "DbMaterial")
            {
                Material.LoadAllDocument();
                ICollection<BaseDocument> baseDocuments1 = Material.DocumentCollection;
                Func<BaseDocument, bool> func12 = func4;
                if (func12 == null)
                {
                    Func<BaseDocument, bool> func13 = (BaseDocument baseDocument) => baseDocument.Id.Equals(id);
                    func = func13;
                    func4 = func13;
                    func12 = func;
                }
                using (IEnumerator<BaseDocument> enumerator3 = baseDocuments1.Where<BaseDocument>(func12).GetEnumerator())
                {
                    if (enumerator3.MoveNext())
                    {
                        current = enumerator3.Current;
                        return current;
                    }
                }
            }
            else if (str == "DbOther")
            {
                Other.LoadAllDocument();
                ICollection<BaseDocument> documentCollection2 = Other.DocumentCollection;
                Func<BaseDocument, bool> func14 = func5;
                if (func14 == null)
                {
                    Func<BaseDocument, bool> func15 = (BaseDocument baseDocument) => baseDocument.Id.Equals(id);
                    func = func15;
                    func5 = func15;
                    func14 = func;
                }
                using (IEnumerator<BaseDocument> enumerator4 = documentCollection2.Where<BaseDocument>(func14).GetEnumerator())
                {
                    if (enumerator4.MoveNext())
                    {
                        current = enumerator4.Current;
                        return current;
                    }
                }
            }
            current = null;
            return current;
        }

        private static SpecificationItemInputType GetInputType(string type)
        {
            SpecificationItemInputType specificationItemInputType;
            if (!type.Equals("DataBase"))
            {
                specificationItemInputType = (!type.Equals("SubSection") ? SpecificationItemInputType.HandInput : SpecificationItemInputType.SubSection);
            }
            else
            {
                specificationItemInputType = SpecificationItemInputType.DataBase;
            }
            return specificationItemInputType;
        }
    }

    public static class SpecificationItemHelpers
    {
        /// <summary>
        /// Компаратор для сортировка по значению Позиция
        /// </summary>
        public class AlphanumComparatorFastToSortByPosition : IComparer<SpecificationItem>
        {
            public int Compare(SpecificationItem x, SpecificationItem y)
            {
                string s1 = x.Position as string;
                if (s1 == null)
                {
                    return 0;
                }
                string s2 = y.Position as string;
                if (s2 == null)
                {
                    return 0;
                }

                int len1 = s1.Length;
                int len2 = s2.Length;
                int marker1 = 0;
                int marker2 = 0;

                // Walk through two the strings with two markers.
                while (marker1 < len1 && marker2 < len2)
                {
                    char ch1 = s1[marker1];
                    char ch2 = s2[marker2];

                    // Some buffers we can build up characters in for each chunk.
                    char[] space1 = new char[len1];
                    int loc1 = 0;
                    char[] space2 = new char[len2];
                    int loc2 = 0;

                    // Walk through all following characters that are digits or
                    // characters in BOTH strings starting at the appropriate marker.
                    // Collect char arrays.
                    do
                    {
                        space1[loc1++] = ch1;
                        marker1++;

                        if (marker1 < len1)
                        {
                            ch1 = s1[marker1];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                    do
                    {
                        space2[loc2++] = ch2;
                        marker2++;

                        if (marker2 < len2)
                        {
                            ch2 = s2[marker2];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                    // If we have collected numbers, compare them numerically.
                    // Otherwise, if we have strings, compare them alphabetically.
                    string str1 = new string(space1);
                    string str2 = new string(space2);

                    int result;

                    if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                    {
                        int thisNumericChunk = int.Parse(str1);
                        int thatNumericChunk = int.Parse(str2);
                        result = thisNumericChunk.CompareTo(thatNumericChunk);
                    }
                    else
                    {
                        result = str1.CompareTo(str2);
                    }

                    if (result != 0)
                    {
                        return result;
                    }
                }
                return len1 - len2;
            }
        }
        /// <summary>
        /// Компаратор для сравнения двух элементов спецификации
        /// </summary>
        public class EqualSpecificationItem : IEqualityComparer<SpecificationItem>
        {
            public bool Equals(SpecificationItem x, SpecificationItem y)
            {
                if (x.Position == y.Position &
                    x.BeforeName == y.BeforeName &
                    x.Designation == y.Designation &
                    x.Mass == y.Mass &
                    x.Note == y.Note) return true;
                return false;
            }

            public int GetHashCode(SpecificationItem obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
