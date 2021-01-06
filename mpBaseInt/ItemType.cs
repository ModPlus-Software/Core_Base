namespace mpBaseInt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Дополнительный выбираемый тип в документе. Например класс арматуры, класс бетона и т.п.
    /// </summary>
    public class ItemType : IEquatable<ItemType>, INotifyPropertyChanged
    {
        private string _selectedValue = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemType"/> class.
        /// </summary>
        /// <param name="typeName">Имя типа (имя атрибута)</param>
        /// <param name="typeHeader">Заголовок (то, что отображается пользователю)</param>
        public ItemType(string typeName, string typeHeader)
        {
            TypeValues = new List<string>();
            TypeName = typeName;
            TypeHeader = typeHeader;
        }

        /// <summary>
        /// Событие изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Имя типа (имя атрибута)
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Заголовок (то, что отображается пользователю)
        /// </summary>
        public string TypeHeader { get; }

        /// <summary>
        /// Коллекция значений типа
        /// </summary>
        public List<string> TypeValues { get; }

        /// <summary>
        /// Выбранное значение
        /// </summary>
        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue == value)
                    return;
                _selectedValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дополнительное примечание для типов
        /// </summary>
        public string TypeToolTip { get; private set; }

        /// <summary>
        /// Видимость дополнительного примечания
        /// </summary>
        public bool IsVisibleTypeToolTip { get; private set; }

        /// <summary>
        /// Установить значение и видимость дополнительного примечания
        /// </summary>
        /// <param name="typeToolTip">Значение примечания</param>
        /// <param name="isVisibleTypeToolTip">Видимость примечания</param>
        internal void SetToolTip(string typeToolTip, bool isVisibleTypeToolTip)
        {
            TypeToolTip = typeToolTip;
            IsVisibleTypeToolTip = isVisibleTypeToolTip;
        }

        /// <inheritdoc/>
        public bool Equals(ItemType other)
        {
            return other != null && 
                   TypeValuesEqual(TypeValues, other.TypeValues) &&
                   TypeHeader.Equals(other.TypeHeader) &&
                   TypeName.Equals(other.TypeName) &&
                   SelectedValue.Equals(other.SelectedValue);
        }

        private bool TypeValuesEqual(IEnumerable<string> typeValues1, IList<string> typeValues2)
        {
            return !typeValues1.Where((t, i) => !t.Equals(typeValues2[i])).Any();
        }

        /// <summary>
        /// Вызов события изменения свойства
        /// </summary>
        /// <param name="propertyName">Свойство</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
