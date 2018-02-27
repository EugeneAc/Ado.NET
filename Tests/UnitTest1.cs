using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using My_Task;

namespace Tests
{
    /// <summary>
    /// Тестирование всех методов из задания
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        
        [TestMethod]
        public void TestGetOrders()
        {
            var myDal = new Dal(ConfigurationManager.ConnectionStrings["myDB"].ConnectionString);
            var res = myDal.GetOrders("ShipVia=1");
            Assert.IsTrue(res.Count>0);
        }

        [TestMethod]
        public void TestGetOrderInfo()
        {
            var myDal = new Dal(ConfigurationManager.ConnectionStrings["myDB"].ConnectionString);
            Assert.IsNotNull(myDal.GetOrderInfo(10248));

        }
        [TestMethod]
        public void TestCreateAndDeleteOrder()
        {
            var myDal = new Dal(ConfigurationManager.ConnectionStrings["myDB"].ConnectionString);
            var result = myDal.CreateNewOrder(new My_Task.Models.Order { CustomerID = "VINET", EmployeeID = 1 });
            Assert.IsTrue(result);
            var res1 = myDal.DeleteOrder(myDal.GetOrders(null).Last().OrderID);
            Assert.IsTrue(res1);
            
        }

        [TestMethod]
        public void TestStroredProc()
        {
            var myDal = new Dal(ConfigurationManager.ConnectionStrings["myDB"].ConnectionString);
            Assert.IsTrue(myDal.GetCustOrderHist("ALFKI").Count > 0);
            foreach (var el in myDal.GetCustOrderHist("ALFKI"))
            {
                Console.WriteLine("{0} {1}", el.Key, el.Value);
            }

            Assert.IsNotNull(myDal.GetCustOrdersDetail(10248));
        }

        [TestMethod]
        public void TestSetOrderInProcess()
        {
            var myDal = new Dal(ConfigurationManager.ConnectionStrings["myDB"].ConnectionString);
            var result = myDal.CreateNewOrder(new My_Task.Models.Order { CustomerID = "VINET", EmployeeID = 1 });
            Assert.IsTrue(result);
            var testorder = myDal.GetOrders(null).Last();
            var res1 = myDal.SetOrderInProcess(testorder.OrderID);
            Assert.IsTrue(res1);
            testorder = myDal.GetOrders(null).Last();
            Assert.IsTrue(testorder.OrderStatus == My_Task.Models.OrderStatus.InProcess);
            foreach (var p in testorder.GetType().GetProperties())
            {
                Console.WriteLine("{0} - {1}", p.Name, p.GetValue(testorder));
            }
            myDal.DeleteOrder(testorder.OrderID);
        }

        [TestMethod]
        public void TestSetOrderCompleted()
        {
            var myDal = new Dal(ConfigurationManager.ConnectionStrings["myDB"].ConnectionString);
            var result = myDal.CreateNewOrder(new My_Task.Models.Order { CustomerID = "VINET", EmployeeID = 1 });
            Assert.IsTrue(result);
            var testorder = myDal.GetOrders(null).Last();
            var res1 = myDal.SetOrderInProcess(testorder.OrderID);
            Assert.IsTrue(res1);
            Assert.IsTrue(myDal.SetOrderCompleted(testorder.OrderID));
            testorder = myDal.GetOrders(null).Last();
            Assert.IsTrue(testorder.OrderStatus == My_Task.Models.OrderStatus.Completed);
            foreach (var p in testorder.GetType().GetProperties())
            {
                Console.WriteLine("{0} - {1}", p.Name, p.GetValue(testorder));
            }
            
        }

        [TestMethod]
        public void TesChangeOrder()
        {
            var myDal = new Dal(ConfigurationManager.ConnectionStrings["myDB"].ConnectionString);
            Assert.IsTrue(myDal.CreateNewOrder(new My_Task.Models.Order { CustomerID = "VINET", EmployeeID = 1 }));
            var testorder = myDal.GetOrders(null).Last();
            foreach (var p in testorder.GetType().GetProperties())
            {
                Console.WriteLine("{0} - {1}", p.Name, p.GetValue(testorder));
            }

            Assert.IsTrue(myDal.ChangeOrder(myDal.GetOrders(null).Last().OrderID, "CustomerID", "SAVEA"));
            Assert.IsTrue(myDal.ChangeOrder(myDal.GetOrders(null).Last().OrderID, "EmployeeID", "2"));

            Console.WriteLine("");
            testorder = myDal.GetOrders(null).Last();
            Assert.IsTrue(testorder.CustomerID == "SAVEA");
            Assert.IsTrue(testorder.EmployeeID == 2);
            foreach (var p in testorder.GetType().GetProperties())
            {
                Console.WriteLine("{0} - {1}", p.Name, p.GetValue(testorder));
            }
            myDal.DeleteOrder(testorder.OrderID);
        }
    }
}
