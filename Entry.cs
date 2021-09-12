using System;

namespace DuplicateDrugIdentifier
{
    public class Entry
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string PrimarySupplier { get; set; }
        public string PrimarySupplierProductCode { get; set; }
        public string Barcode { get; set; }
        public string ProductGroup { get; set; }
        public string ProductGroupName { get; set; }
        public double? RetailPrice { get; set; }
        public double QtyOnHand { get; set; }
        public int QtyOnOrder { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Status { get; set; }
        public DateTime? LastSellPriceChange { get; set; }

        public Entry(string[] values)
        {

            ItemCode = values[0];
            Description = values[1];
            PrimarySupplier = values[2];
            PrimarySupplierProductCode = values[3];
            Barcode = values[4];
            ProductGroup = values[5];
            ProductGroupName = values[6];

            if (double.TryParse(values[7], out double retailPrice))
            {
                RetailPrice = retailPrice;
            }
            else
            {
                RetailPrice = null;
            }

            QtyOnHand = double.Parse(values[8]);
            QtyOnOrder = int.Parse(values[9]);

            if (DateTime.TryParse(values[10], out DateTime creationDate))
            {
                CreationDate = creationDate;
            }
            else
            {
                CreationDate = null;
            }

            Status = values[11];

            if (DateTime.TryParse(values[12], out DateTime lastSellPriceChange))
            {
                LastSellPriceChange = lastSellPriceChange;
            }
            else
            {
                LastSellPriceChange = null;
            }
        }
    }
}
