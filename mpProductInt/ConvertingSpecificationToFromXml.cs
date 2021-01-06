namespace mpProductInt
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Xml.Linq;
    using mpBaseInt;
    using mpConcrete;
    using mpMaterial;
    using mpMetall;
    using mpOther;
    using mpWood;

    [Obsolete]
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
                mpProduct.Length = TryParseInvariant(productXel.Attribute("Length")?.Value, out double dnum) ? dnum : 0;
                mpProduct.Diameter = TryParseInvariant(productXel.Attribute("Diameter")?.Value, out dnum) ? dnum : 0;
                mpProduct.Width = TryParseInvariant(productXel.Attribute("Width")?.Value, out dnum) ? dnum : 0;
                mpProduct.Height = TryParseInvariant(productXel.Attribute("Height")?.Value, out dnum) ? dnum : 0;
                mpProduct.Mass = TryParseInvariant(productXel.Attribute("Mass")?.Value, out dnum) ? dnum : 0;
                mpProduct.WMass = TryParseInvariant(productXel.Attribute("WMass")?.Value, out dnum) ? dnum : 0;
                mpProduct.CMass = TryParseInvariant(productXel.Attribute("CMass")?.Value, out dnum) ? dnum : 0;
                mpProduct.SMass = TryParseInvariant(productXel.Attribute("SMass")?.Value, out dnum) ? dnum : 0;
                mpProduct.ItemTypes = mpProduct.BaseDocument.ItemTypes;

                var indexOfItem = int.TryParse(productXel.Attribute("IndexOfItem")?.Value, out inum) ? inum : -1;
                var productItemXel = indexOfItem != -1 ? mpProduct.BaseDocument.Items.Elements("Item").ElementAt(indexOfItem) : null;
                mpProduct.Item = productItemXel;

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
            if (TryParseInvariant(specificationItemXel.Attribute("Mass")?.Value, out double d))
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
                handMass);

            specificationItem.Position = specificationItemXel.Attribute("Position")?.Value;
            specificationItem.AfterName = specificationItemXel.Attribute("AfterName")?.Value;
            specificationItem.BeforeName = specificationItemXel.Attribute("BeforeName")?.Value;
            specificationItem.Count = specificationItemXel.Attribute("Count")?.Value;
            specificationItem.DbIndex = int.TryParse(specificationItemXel.Attribute("DbIndex")?.Value, out int integer) ? integer : -1;
            specificationItem.Designation = specificationItemXel.Attribute("Designation")?.Value;
            specificationItem.HasSteel = bool.TryParse(specificationItemXel.Attribute("HasSteel")?.Value, out bool flag) & flag;

            if (TryParseInvariant(specificationItemXel.Attribute("Mass")?.Value, out double dnumber))
                specificationItem.Mass = dnumber;
            else
                specificationItem.Mass = null;

            specificationItem.Note = specificationItemXel.Attribute("Note")?.Value;
            specificationItem.TopName = specificationItemXel.Attribute("TopName")?.Value;
            specificationItem.SteelDoc = steelDoc;
            specificationItem.SteelType = steelType;
            specificationItem.AfterName = specificationItemXel.Attribute("AfterName")?.Value;
            specificationItem.SteelVisibility =
                Enum.TryParse(specificationItemXel.Attribute("SteelVisibility")?.Value, out Visibility visibility)
                    ? visibility : Visibility.Collapsed;

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
                if (!item.Product.BaseDocument.Items?.Elements("Item").Any() ?? true)
                {
                    xElement1.SetAttributeValue("IndexOfItem", -1);
                }
                else
                {
                    xElement1.SetAttributeValue("IndexOfItem", item.Product.BaseDocument.Items.Elements("Item").ToList().IndexOf(item.Product.Item));
                }

                if (item.Product.ItemTypes == null)
                {
                    xElement1.SetAttributeValue("ItemTypesValues", string.Empty);
                }
                else
                {
                    string str = item.Product.ItemTypes.Aggregate(string.Empty, (current, itemType) => string.Concat(current, itemType.SelectedItem, "$"));
                    xElement1.SetAttributeValue("ItemTypesValues", str.TrimEnd('$'));
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
                    Func<BaseDocument, bool> func7 = baseDocument => baseDocument.Id.Equals(id);
                    func = func7;
                    func1 = func7;
                    func6 = func;
                }

                using (IEnumerator<BaseDocument> enumerator = documentCollection.Where(func6).GetEnumerator())
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
                    Func<BaseDocument, bool> func9 = baseDocument => baseDocument.Id.Equals(id);
                    func = func9;
                    func2 = func9;
                    func8 = func;
                }

                using (IEnumerator<BaseDocument> enumerator1 = baseDocuments.Where(func8).GetEnumerator())
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
                    Func<BaseDocument, bool> func11 = baseDocument => baseDocument.Id.Equals(id);
                    func = func11;
                    func3 = func11;
                    func10 = func;
                }

                using (IEnumerator<BaseDocument> enumerator2 = documentCollection1.Where(func10).GetEnumerator())
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
                    Func<BaseDocument, bool> func13 = baseDocument => baseDocument.Id.Equals(id);
                    func = func13;
                    func4 = func13;
                    func12 = func;
                }

                using (IEnumerator<BaseDocument> enumerator3 = baseDocuments1.Where(func12).GetEnumerator())
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
                    Func<BaseDocument, bool> func15 = baseDocument => baseDocument.Id.Equals(id);
                    func = func15;
                    func5 = func15;
                    func14 = func;
                }

                using (IEnumerator<BaseDocument> enumerator4 = documentCollection2.Where(func14).GetEnumerator())
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
                specificationItemInputType = !type.Equals("SubSection") ? SpecificationItemInputType.HandInput : SpecificationItemInputType.SubSection;
            }
            else
            {
                specificationItemInputType = SpecificationItemInputType.DataBase;
            }

            return specificationItemInputType;
        }

        private static bool TryParseInvariant(string v, out double d)
        {
            if (double.TryParse(v?.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                return true;
            d = double.NaN;
            return false;
        }
    }
}