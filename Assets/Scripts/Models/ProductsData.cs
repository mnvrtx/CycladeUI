using System.Collections.Generic;

namespace CycladeUIExample.Models
{
    public class ProductsData
    {
        public List<IapProduct> IapProducts;
        public List<Product> InGameProducts;

        public static ProductsData GetMock()
        {
            return new ProductsData()
            {
                IapProducts = new List<IapProduct>()
                {
                    new()
                    {
                        Count = 10,
                        // AdditionalInfo = "SALE",
                        IapType = IapProductType.Diamond1,
                        Price = 11
                    },
                    new()
                    {
                        Count = 12,
                        // AdditionalInfo = "SALE",
                        IapType = IapProductType.Diamond2,
                        Price = 13
                    },
                    new()
                    {
                        Count = 14,
                        AdditionalInfo = "SALE",
                        IapType = IapProductType.Diamond3,
                        Price = 15
                    },
                    new()
                    {
                        Count = 16,
                        // AdditionalInfo = "SALE",
                        IapType = IapProductType.Diamond4,
                        Price = 17
                    },
                    new()
                    {
                        Count = 18,
                        AdditionalInfo = "SALE",
                        IapType = IapProductType.Diamond5,
                        Price = 19
                    },
                    new()
                    {
                        Count = 20,
                        AdditionalInfo = "SALE",
                        IapType = IapProductType.Diamond6,
                        Price = 21
                    },
                },
                InGameProducts = new List<Product>()
                {
                    new()
                    {
                        Count = 1,
                        // AdditionalInfo = "SALE",
                        InGameType = InGameProductType.Chest,
                        Price = 2
                    },
                    new()
                    {
                        Count = 3,
                        AdditionalInfo = "SALE",
                        InGameType = InGameProductType.ChestBig,
                        Price = 4
                    },
                    new()
                    {
                        Count = 5,
                        // AdditionalInfo = "SALE",
                        InGameType = InGameProductType.Credits1,
                        Price = 6
                    },
                    new()
                    {
                        Count = 7,
                        // AdditionalInfo = "SALE",
                        InGameType = InGameProductType.Credits2,
                        Price = 8
                    },
                    new()
                    {
                        Count = 9,
                        AdditionalInfo = "SALE",
                        InGameType = InGameProductType.Credits3,
                        Price = 10
                    },
                    new()
                    {
                        Count = 11,
                        AdditionalInfo = "SALE",
                        InGameType = InGameProductType.Credits4,
                        Price = 12
                    }
                },
            };
        }
    }
    
    public class BaseProduct
    {
        public int Count;
        public string AdditionalInfo;
    }
    
    public class IapProduct : BaseProduct
    {
        public IapProductType IapType;
        public int Price;
    }
    
    public class Product : BaseProduct
    {
        public bool IsChest => InGameType is InGameProductType.Chest or InGameProductType.ChestBig;
        public InGameProductType InGameType;
        public int Price;
    }

    public enum IapProductType
    {
        Diamond1,
        Diamond2,
        Diamond3,
        Diamond4,
        Diamond5,
        Diamond6,
    }
    public enum InGameProductType
    {
        Credits1,
        Credits2,
        Credits3,
        Credits4,
        Chest,
        ChestBig,
    }
}