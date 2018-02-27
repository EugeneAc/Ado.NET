using My_Task.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Task
{
    /// <summary>
    /// Максимально абстактный класс, не зависящий от конкретного провайдера
    /// </summary>
    public abstract class DalFather
    {
        protected IDbConnection _connection;
        protected string _connectionString;

        protected DalFather(string connectionString, IDbConnection connection)
        {
            _connectionString = connectionString;
            _connection = connection;

        }

        protected string ExecuteScalarCommand(string commandText)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            var retstring = command.ExecuteScalar().ToString();
            _connection.Close();
            return retstring;
        }

        protected int ExecuteNonQuery(IDbCommand command)
        {
            _connection.Open();
            int updatedRows = (int)command.ExecuteNonQuery();
            _connection.Close();
            return updatedRows;
        }

        protected List<T> ExecuteCommand<T>(IDbCommand command, T model)
        {
            _connection.Open();
            List<T> list = new List<T>();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (model.GetType() == typeof(Order))
                    {
                        SetOrderModelProperties(reader, (Order)(object)model);
                    }
                    else
                    {
                        SetModelProperties(reader, model);
                    }

                    list.Add(model);
                }
            }
            _connection.Close();
            return list;
        }

        protected List<T> ExecuteCommand<T>(string commandText, T model)
        {
           var command = _connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            return ExecuteCommand<T>(command, model);
        }

        protected static void SetOrderModelProperties(IDataReader reader, Order model)
        {
            foreach (var item in model.GetType().GetProperties().Where(m => m.PropertyType != typeof(OrderStatus)))
            {
                if (reader[item.Name].GetType() == typeof(System.DBNull))
                {
                    if (item.Name == "OrderDate")
                    {
                        model.OrderStatus = OrderStatus.New;
                    }
                    else if (item.Name == "ShippedDate")
                    {
                        model.OrderStatus = OrderStatus.InProcess;
                    }
                    model.GetType().GetProperty(item.Name).SetValue(model, null, null);
                }
                else
                {
                    if (item.Name == "ShippedDate")
                    {
                        model.OrderStatus = OrderStatus.Completed;
                    }
                    model.GetType().GetProperty(item.Name).SetValue(model, reader[item.Name], null);
                }
            }
        }

        protected static void SetModelProperties(IDataReader reader, object model)
        {
            foreach (var item in model.GetType().GetProperties())
            {
                if (reader[item.Name].GetType() == typeof(System.DBNull))
                {
                    model.GetType().GetProperty(item.Name).SetValue(model, null, null);
                }
                else
                {
                    model.GetType().GetProperty(item.Name).SetValue(model, reader[item.Name], null);
                }
            }
        }

    }
}
