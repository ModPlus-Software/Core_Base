namespace mpMetall
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using mpBaseInt;

    /// <summary>
    /// Раздел "Металл"
    /// </summary>
    public class MetalSection : IDbSection
    {
        private IEnumerable<DbDocument> _documents;

        /// <inheritdoc/>
        public string Name => "DbMetall";

        /// <inheritdoc/>
        public string Code => "Me";

        /// <inheritdoc/>
        public IEnumerable<DbDocument> Documents => _documents ?? (_documents = DbSectionUtils.GetDocuments(
            Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true), this));

        /// <inheritdoc/>
        public List<string> GetDocumentNames()
        {
            return Documents.Select(doc => $"{doc.DocumentType} {doc.DocumentNumber}").ToList();
        }

        /// <inheritdoc/>
        public string GetImagePath(DbDocument document)
        {
            var str = string.Empty;
            if (!string.IsNullOrEmpty(document.Image))
                str = $@"pack://application:,,,/mpMetall;component/Resources/Images/{document.Image}.png";
            return str;
        }
        
        /// <inheritdoc />
        public IEnumerable<DbDocument> FindDocuments(string searchValue)
        {
            return DbSectionUtils.FindDocuments(this, searchValue);
        }
    }
}
