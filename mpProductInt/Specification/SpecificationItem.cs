namespace mpProductInt.Specification
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Позиция спецификации
    /// </summary>
    public class SpecificationItem : IEquatable<SpecificationItem>, INotifyPropertyChanged
    {
        private string _position;
        private string _count;
        private string _note;
        
        /// <summary>
        /// Инициализация экземпляра <see cref="SpecificationItem"/> по экземпляру <see cref="DbProduct"/>
        /// </summary>
        /// <param name="product">Экземпляр <see cref="DbProduct"/>, связанный с позицией спецификации</param>
        /// <param name="steelDoc">Документ на сталь</param>
        /// <param name="steelType">Тип стали</param>
        /// <param name="dimension">Вариант измерения</param>
        public SpecificationItem(
            DbProduct product,
            string steelDoc,
            string steelType,
            string dimension)
        {
            Product = product;
            Dimension = dimension;
            InputType = SpecificationItemInputType.DataBase;
            InitSpecificationItem(steelDoc, steelType);
        }

        /// <summary>
        /// Инициализация экземпляра <see cref="SpecificationItem"/>, представляющего подраздел
        /// </summary>
        /// <param name="subsection">Название подраздела</param>
        public SpecificationItem(string subsection)
        {
            Product = null;
            InputType = SpecificationItemInputType.SubSection;
            BeforeName = subsection;
            AfterName = TopName = SteelDoc = SteelType = Dimension = Note = string.Empty;
            Mass = null;
            HasSteel = false;
            IsVisibleSteel = false;
        }

        /// <summary>
        /// Инициализация экземпляра <see cref="SpecificationItem"/>, представляющего позицию с ручным вводом
        /// </summary>
        /// <param name="steelDoc">Документ на сталь</param>
        /// <param name="steelType">Тип стали</param>
        /// <param name="handBeforeName">Составная часть наименования при ручном вводе: перед дробью</param>
        /// <param name="handTopName">Составная часть наименования при ручном вводе: числитель</param>
        /// <param name="handAfterName">Составная часть наименования при ручном вводе: после дроби</param>
        /// <param name="handMass">Составная часть наименования при ручном вводе: масса</param>
        public SpecificationItem(
            string steelDoc,
            string steelType,
            string handBeforeName,
            string handTopName,
            string handAfterName,
            double? handMass)
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
                IsVisibleSteel = true;
            }
            else
            {
                HasSteel = false;
                IsVisibleSteel = false;
            }
        }

        /// <summary>
        /// Событие изменения свойства экземпляра
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Продукт, связанный с текущим элементом
        /// </summary>
        public DbProduct Product { get; set; }

        /// <summary>
        /// Тип позиции спецификации
        /// </summary>
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
                OnPropertyChanged();
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

        /// <summary>
        /// Видимость стали
        /// </summary>
        public bool IsVisibleSteel { get; set; }

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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                    Designation = $"{Product.BaseDocument.DocumentType} {Product.BaseDocument.DocumentNumber}";

                    // has steel
                    HasSteel = Product.BaseDocument.HasSteel;
                    IsVisibleSteel = HasSteel;
                }
                else
                {
                    Designation = string.Empty;
                    HasSteel = false;
                    IsVisibleSteel = false;
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
                    BeforeName = $"{Product.GetNameByRule()} {GetAfterName()}";
                    TopName = AfterName = SteelDoc = SteelType = string.Empty;
                }
            }
        }

        private string GetAfterName()
        {
            switch (Dimension)
            {
                case "Длина":
                    return $"L={Product.Length}";
                case "п.м":
                    return ", п.м";
                case "":
                    return string.Empty;
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Вызвать событие изменения свойства экземпляра
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <inheritdoc />
        public bool Equals(SpecificationItem other)
        {
            if (other != null && Position == other.Position &&
                BeforeName == other.BeforeName &&
                Designation == other.Designation &&
                Note == other.Note)
            {
                if (Mass != null && other.Mass != null)
                    return Math.Abs(Mass.Value - other.Mass.Value) < 0.0001;
                return true;
            }

            return false;
        }
    }
}
