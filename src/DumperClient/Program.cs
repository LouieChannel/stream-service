using DumperClient.Properties;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DumperClient
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient client = new UdpClient();

            try
            {
                var csvTable = new DataTable();

                using (var csvReader = new CsvReader(new StringReader(Resources.Terra_D1_multi_labeled_interpolated), true))
                {
                    csvTable.Load(csvReader);
                }

                foreach (DataRow element in csvTable.Rows)
                {
                    byte[] data = Encoding.UTF8.GetBytes($"1,{element.ItemArray[1]},{element.ItemArray[2]},{element.ItemArray[3]},{element.ItemArray[4]},{element.ItemArray[5]},{element.ItemArray[6]},{element.ItemArray[7]},{element.ItemArray[8]}");
                    client.Send(data, data.Length, "127.0.0.1", 5005);
                    Thread.Sleep(1);
                }

                client.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
