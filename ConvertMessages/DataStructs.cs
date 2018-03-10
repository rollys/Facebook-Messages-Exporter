using System;
using System.Collections.Generic;
namespace DataStructs
{
    struct sMessage
    {
        public string user;
        public string date;
        public string text;

        public sMessage(string us, string dat, string tex)
        {
            user = us;
            date = dat;
            text = tex;
        }
    }

    class SimpleTable
    {
        public IList<string> columnanames { get; }
        public IList<string[]> data { get; }

        public SimpleTable(IList<string> ColumnNames)
        {
            columnanames = ColumnNames;
            data = new List<string[]>();
        }

        public SimpleTable(IList<sMessage> Messages)
        {
            columnanames = new List<string>();
            data = new List<string[]>();
            columnanames.Add("user");
            columnanames.Add("date");
            columnanames.Add("message");

            for (int i = 0; i < Messages.Count; i++)
            {
                string[] con = new string[] { Messages[i].user, Messages[i].date, Messages[i].text };
                data.Add(con);
            }
        }

        public SimpleTable()
        {
            columnanames = new List<string>();
            data = new List<string[]>();
        }

        public void ArrColumn(string ColumnName)
        {
            if (string.IsNullOrEmpty(ColumnName)) throw new Exception("Name cannot be empty.");

            columnanames.Add(ColumnName);
        }

        public void AddRow(string[] row)
        {
            if (row.GetLength(0) > columnanames.Count) throw new ArgumentOutOfRangeException("Can not be more than columns number");

            data.Add(row);
        }
    }
}