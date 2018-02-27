using My_Task.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Task
{
    /// <summary>
    /// Чтобы можно было мОкать dal
    /// </summary>
    interface IDal
    {
        List<Order> GetOrders(string criteria);
        OdrerInfo GetOrderInfo(int orderID);
        bool DeleteOrder(int orderID);
        bool CreateNewOrder(Order order);
        bool SetOrderInProcess(int orderID);
        bool SetOrderCompleted(int orderID);
        bool ChangeOrder(int orderID, string parmName, string parmaValue);
        Dictionary<string, int> GetCustOrderHist(string customerID);
        List<CustOrderDetail> GetCustOrdersDetail(int orderID);
    }
}
