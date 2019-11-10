namespace mpProductInt
{
    using System;

    /// <summary>
    /// Класс изделия для хранения в расширенных данных
    /// </summary>
    [Serializable]
    public class MpProductToSave
    {
        /// <summary>
        /// Для регистрации в автокаде уникального имени
        /// </summary>
        public string AppName = "ModPlusProduct";

        /// <summary>
        /// Имя базы данных для более быстрого поиска
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// Id документа
        /// </summary>
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
}