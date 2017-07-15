using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace SouthlandMetals.Reporting.Helpers
{
    public static class DataTableConverter
    {
        public static DataTable ToDataTable<T>(T item)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                if (Props[i].GetIndexParameters().Count() > 0)
                {
                    values[i] = Props[i].GetValue(item, new object[] { 0 });
                }
                else
                {
                    values[i] = Props[i].GetValue(item, null);
                }
            }
            dataTable.Rows.Add(values);

            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ToDataTable<T>(List<T> itemList)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (var item in itemList)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    if (Props[i].GetIndexParameters().Count() > 0)
                    {
                        values[i] = Props[i].GetValue(item, new object[] { 0 });
                    }
                    else
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> itemList)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (var item in itemList)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    if (Props[i].GetIndexParameters().Count() > 0)
                    {
                        values[i] = Props[i].GetValue(item, new object[] { 0 });
                    }
                    else
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}

