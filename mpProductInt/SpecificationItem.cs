namespace mpProductInt
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Класс для позиции в строительной спецификации
    /// </summary>
    public class SpecificationItem : INotifyPropertyChanged
    {
        private string _position;
        private string _count;
        private string _note;

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
        private void InitSpecificationItem(string steelDoc, string steelType)
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
                    return string.Empty;
                default: return string.Empty;
            }
        }

        public SpecificationItemInputType InputType { get; set; }

        /// <summary>
        /// Позиция
        /// </summary>
        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

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
        public string Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        /// <summary>
        /// Масса
        /// </summary>
        public double? Mass { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged("Note");
            }
        }

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
}