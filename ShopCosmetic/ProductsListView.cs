using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ShopCosmetic
{
    public class ProductsListView
    {
        public List<Product1> ListProd(List<Product> products)
        {
            List<Product1> productsListViews = new List<Product1>();
            foreach (Product product in products)
            {
                Product1 product1 = new Product1()
                {
                    id = product.id,
                    name = product.name,
                    promotion = product.promotion,
                    brand = product.brand,
                    Brand1 = product.Brand1,
                    description = product.description,
                    pack = product.pack,
                    price = product.price,
                    photo = product.photo,
                    promotionPercent = product.promotionPercent,
                    sclad = product.sclad,
                    typePurpose = product.typePurpose,
                    TypePurpose1 = product.TypePurpose1,
                    Basket = product.Basket,
                    photoBinary = product.photoBinary
                };
                productsListViews.Add(product1);
            }
            return productsListViews;
        }

        public Product1 ReturnProduct(Product product)
        {
            Product1 product1 = new Product1()
            {
                id = product.id,
                name = product.name,
                promotion = product.promotion,
                brand = product.brand,
                Brand1 = product.Brand1,
                description = product.description,
                pack = product.pack,
                price = product.price,
                photo = product.photo,
                promotionPercent = product.promotionPercent,
                sclad = product.sclad,
                typePurpose = product.typePurpose,
                TypePurpose1 = product.TypePurpose1,
                Basket = product.Basket,
                photoBinary = product.photoBinary
            };
            return product1;
        }
    }

    public class Product1
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string photo { get; set; }
        public int typePurpose { get; set; }
        public int brand { get; set; }
        public Nullable<int> sclad { get; set; }
        public bool promotion { get; set; }
        public Nullable<int> promotionPercent { get; set; }
        public Nullable<int> pack { get; set; }
        public virtual Brand Brand1 { get; set; }
        public byte[] photoBinary { get; set; }
        public virtual TypePurpose TypePurpose1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Basket> Basket { get; set; }
        public string IsPromotionAdminText
        {
            get
            {
                return (bool)promotion ? "действует скидка" : "Нет скидки";
            }
        }
        public string PriceClientText
        {
            get
            {
                double p = (double)price;
                double per = promotionPercent != null ? (double)promotionPercent : 0;
                double tot = p * (1 - (per / 100));
                if (tot < 1)
                    return "1 руб";
                else
                    return (bool)promotion ? $"{Math.Round(tot, 2)} руб" : Math.Round(price, 2).ToString() + " руб";
            }
        }
        public Visibility VisibleBool
        {
            get
            {
                return (bool)promotion ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Color ColorFont
        {
            get
            {
                return (bool)promotion ? Color.FromRgb(255, 0, 0) : Color.FromRgb(0, 0, 0);
            }
        }
        public string Percent
        {
            get
            {
                return (bool)promotion ? "-" + promotionPercent.ToString() + "%" : "";
            }
        }
        public Visibility VisibleDesc
        {
            get
            {
                return !String.IsNullOrEmpty(description) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public int missAmount
        {
            get
            {
                return Basket.Sum(x => x.lack);
            }
        }
    }
}
