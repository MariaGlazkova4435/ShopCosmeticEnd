//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopCosmetic
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Basket = new HashSet<Basket>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string photo { get; set; }
        public int typePurpose { get; set; }
        public int brand { get; set; }
        public int pack { get; set; }
        public Nullable<int> sclad { get; set; }
        public bool promotion { get; set; }
        public Nullable<int> promotionPercent { get; set; }
        public byte[] photoBinary { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Basket> Basket { get; set; }
        public virtual Brand Brand1 { get; set; }
        public virtual TypePurpose TypePurpose1 { get; set; }
    }
}
