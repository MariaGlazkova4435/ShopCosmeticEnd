using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopCosmetic
{
    public class OrdersDataGrid
    {
        public List<Order1> ListOrder(List<Order> orders)
        {
            List<Order1> ordersListViews = new List<Order1>();
            foreach (Order o in orders)
            {
                Order1 order1 = new Order1()
                {
                    idOrder = o.idOrder,
                    status = o.status,
                    date = o.date,
                    fullnameClient = o.fullnameClient,
                    emailClient = o.emailClient,
                    address = o.address,
                    total = o.total,
                    phoneClient = o.phoneClient,
                    delivery = o.delivery,
                    payment = o.payment,
                    Basket = o.Basket,
                    OrderStatus = o.OrderStatus,
                    Payment1 = o.Payment1,
                    dateHand = o.dateHand,
                    dateCollect = o.dateCollect
                };
                ordersListViews.Add(order1);
            }
            return ordersListViews;
        }

        public Order1 ReturnOrder(Order order)
        {
            Order1 order1 = new Order1()
            {
                idOrder = order.idOrder,
                status = order.status,
                date = order.date,
                fullnameClient = order.fullnameClient,
                emailClient = order.emailClient,
                address = order.address,
                total = order.total,
                phoneClient = order.phoneClient,
                delivery = order.delivery,
                payment = order.payment,
                Basket = order.Basket,
                OrderStatus = order.OrderStatus,
                Payment1 = order.Payment1,
                dateHand = order.dateHand,
                dateCollect = order.dateCollect
            };
            return order1;
        }
    }

    public class Order1
    {
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

        public string Datehand
        {
            get
            {
                return (dateHand == Convert.ToDateTime("01.01.2001"))||(dateHand==null) ? "" : dateHand.Value.ToString("dd.MM.yyyy"); 
            }
        }

        public string DateCollect
        {
            get
            {
                return (dateCollect == Convert.ToDateTime("01.01.2001")) || (dateCollect == null) ? "" : dateCollect.Value.ToString("dd.MM.yyyy");
            }
        }
        public string DeliveryValue
        {
            get
            {
                return delivery ? address : "без доставки";
            }
        }

        public string DeliveryBool
        {
            get
            {
                return delivery ? "с доставкой" : "без доставки";
            }
        }

        public int CountProducts
        {
            get
            {
                return Basket.Sum(x => x.amount);
            }
        }


    }
}
