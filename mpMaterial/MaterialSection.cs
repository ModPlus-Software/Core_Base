namespace mpMaterial
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using mpBaseInt;

    /// <summary>
    /// Раздел "Материалы"
    /// </summary>
    public class MaterialSection : IDbSection
    {
        private IEnumerable<BaseDocument> _documents;

        /// <inheritdoc/>
        public string Name => "DbMaterial";

        /// <inheritdoc/>
        public string Code => "Ma";

        /// <inheritdoc/>
        public IEnumerable<BaseDocument> Documents => _documents ?? (_documents = DbSectionUtils.GetDocuments(
            Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true), Name));

        /// <inheritdoc />
        public List<string> GetDocumentNames()
        {
            return Documents.Select(doc => $"{doc.DocumentType} {doc.DocumentNumber}").ToList();
        }

        /// <inheritdoc/>
        public string GetImagePath(BaseDocument element)
        {
            var str = string.Empty;
            if (!string.IsNullOrEmpty(element.Image))
                str = $@"pack://application:,,,/mpMaterial;component/Resources/Images/{element.Image}.png";
            return str;
        }
    }
}
