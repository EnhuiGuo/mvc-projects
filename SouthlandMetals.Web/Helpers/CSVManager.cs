using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace SouthlandMetals.Web.Helpers
{
    public class CSVManager
    {
        /// <summary>
        /// Process the file supplied and process the CSV to a dynamic datatable
        /// </summary>
        /// <param name="fileName">String</param>
        /// <returns>DataTable</returns>
        public DataTable ProcessCSV(HttpPostedFileBase fileName)
        {
            DataTable dataTable = new DataTable();

            // work out where we should split on comma, but not in a sentance
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            //Set the filename in to our stream
            StreamReader reader = new StreamReader(fileName.InputStream);

            //Read the first line and split the string at , with our regular express in to an array
            string[] columnHeaders = r.Split(reader.ReadLine());
            foreach (string header in columnHeaders)
            {
                dataTable.Columns.Add(header);
            }

            //Read each line in the CVS file until it's empty
            while (!reader.EndOfStream)
            {
                string[] rows = r.Split(reader.ReadLine());
                if (rows.Length > 1)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < columnHeaders.Length; i++)
                    {
                        //add our current value to our data row
                        dataRow[i] = rows[i].Trim();
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            //Tidy Streameader up
            reader.Dispose();

            //return a the new DataTable
            return dataTable;
        }
    }
}