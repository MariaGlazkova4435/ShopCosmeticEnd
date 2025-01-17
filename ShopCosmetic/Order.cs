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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.Basket = new HashSet<Basket>();
        }
    
        public int idOrder { get; set; }
        public int status { get; set; }
        public System.DateTime date { get; set; }
        public string fullnameClient { get; set; }
        public string emailClient { get; set; }
        public string address { get; set; }
        public decimal total { get; set; }
        public string phoneClient { get; set; }
        public bool delivery { get; set; }
        public int payment { get; set; }
        public Nullable<System.DateTime> dateHand { get; set; }
        public Nullable<System.DateTime> dateCollect { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Basket> Basket { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public virtual Payment Payment1 { get; set; }
    }
}
