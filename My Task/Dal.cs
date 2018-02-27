using Microsoft.VisualStudio.TestTools.UnitTesting;
using My_Task.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Task
{
    /// <summary>
    /// Конкретный класс для взаимодействия с БД
    /// Все методы из задания здесь
    /// </summary>
    public class Dal : DalFather, IDal
    {
        public Dal(string connectionString) : base(connectionString, new SqlConnection(connectionString))
        {

        }

        public List<Order> GetOrders (string criteria)
        {
            var commandText = "SELECT [OrderID] ,[CustomerID],[EmployeeID],[OrderDate],[RequiredDate]" +
                ",[ShippedDate] ,[ShipVia] ,[Freight] ,[ShipName] ,[ShipAddress] ,[ShipCity]  ,[ShipRegion]" +
                ",[ShipPostalCode] ,[ShipCountry] FROM[Northwind].[dbo].[Orders]";
                if (!string.IsNullOrEmpty(criteria))
                commandText+=" where "  + criteria;
            return ExecuteCommand(commandText, new Order());
        }

        public bool CreateNewOrder(Order order)
        {
            var colNames = "";
            var prms = "";
            var command = _connection.CreateCommand();

            foreach (var prop in order.GetType().GetProperties().Where(m => m.PropertyType != typeof(OrderStatus) && m.Name != "OrderID"&&m.GetValue(order)!=null))
            {
                var queryParam = command.CreateParameter();
                queryParam.ParameterName = "@" + prop.Name;
                queryParam.Value = prop.GetValue(order);
                command.Parameters.Add(queryParam);

                prms += queryParam.ParameterName + ",";
                colNames += prop.Name + ",";
            }
            prms = prms.Remove(prms.Length - 1);
            colNames = colNames.Remove(colNames.Length - 1);

            var cmdText = "INSERT INTO Northwind.dbo.Orders (" + colNames + ") Values (" + prms+")";
            command.CommandText = cmdText;
            var stringaffected = ExecuteNonQuery(command);
            return Convert.ToBoolean(stringaffected);
        }

        public bool DeleteOrder(int orderID)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM Northwind.dbo.Orders " +
                "where OrderID="+orderID+ " and (OrderDate IS NULL or ShippedDate IS NULL)";
            
            var stringaffected = ExecuteNonQuery(command);
            return Convert.ToBoolean(stringaffected); ;
        }

        public OdrerInfo GetOrderInfo (int orderID)
        {
            var odrerInfo = new OdrerInfo();
            odrerInfo.Order = GetOrders("OrderID = "+orderID).FirstOrDefault();

            var getOrdDetailscommandText = "SELECT [OrderID],[ProductID],[UnitPrice]" +
                              ",[Quantity],[Discount]" +
                              " FROM[Northwind].[dbo].[Order Details] where OrderID ="+ orderID;
            odrerInfo.OrderDetails = ExecuteCommand(getOrdDetailscommandText, new OrderDetail());
            odrerInfo.Products = new List<Product>();

            foreach (var od in odrerInfo.OrderDetails)
            {
                var getProductscmdText = "SELECT [ProductID],[ProductName],[SupplierID]" +
                    ",[CategoryID],[QuantityPerUnit],[UnitPrice],[UnitsInStock]" +
                    ",[UnitsOnOrder],[ReorderLevel],[Discontinued]" +
                    "FROM[Northwind].[dbo].[Products] where ProductID = "+od.ProductID;
                odrerInfo.Products.AddRange(ExecuteCommand(getProductscmdText, new Product()));
            }
            return odrerInfo;
        }

        public bool SetOrderInProcess(int orderID)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "Update Northwind.dbo.Orders " +
                "set OrderDate = '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "'"+
                " where OrderID=" + orderID + " and OrderDate IS NULL";

            return Convert.ToBoolean(ExecuteNonQuery(command));
        }

        public bool SetOrderCompleted(int orderID)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "Update Northwind.dbo.Orders " +
                "set ShippedDate = '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "'"+
                " where OrderID=" + orderID + " and (ShippedDate IS NULL and OrderDate IS NOT NULL)";

            return Convert.ToBoolean(ExecuteNonQuery(command));
        }

        public Dictionary<string, int> GetCustOrderHist(string customerID)
        {
            SqlCommand command = (SqlCommand)_connection.CreateCommand();
            command.CommandText = "[Northwind].dbo.[CustOrderHist]";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@CustomerID", customerID);

            _connection.Open();
            Dictionary<string, int> retDict = new Dictionary<string, int>();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    retDict.Add((string)reader[0], (int)reader[1]);
                }
            }
            _connection.Close();
            
            return retDict;
        }

        public List<CustOrderDetail> GetCustOrdersDetail(int orderID)
        {
            SqlCommand command = (SqlCommand)_connection.CreateCommand();
            command.CommandText = "[Northwind].dbo.[CustOrdersDetail]";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@OrderID", orderID);
            ExecuteCommand(command, new CustOrderDetail());

            return ExecuteCommand(command, new CustOrderDetail());
        }

        public bool ChangeOrder(int orderID, string parmName, string parmaValue)
        {
            SqlCommand command = (SqlCommand)_connection.CreateCommand();
            command.CommandText = "Update Northwind.dbo.Orders " +
               "set "+ parmName +"= @paramValue"+
               " where OrderID=" + orderID + " and OrderDate IS NULL";
            command.Parameters.AddWithValue("@paramValue", parmaValue);

            return Convert.ToBoolean(ExecuteNonQuery(command));
        }
    }
}
