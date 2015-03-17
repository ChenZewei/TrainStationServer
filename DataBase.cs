using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;


namespace TrainStationServer
{
    class DataBase
    {
        private string cmd;
        public static MySqlConnection MySQLConnect = new MySqlConnection("server=192.168.80.13; user id=ivms; Password=ivmspwd; database=opensips; persist security info=False");
        public DataBase()
        {
            try
            {
                MySQLConnect.Open();
            }
            catch(MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public void test()
        {
            List<string[]> result = new List<string[]>();
            cmd = "select id,name,location,custom from ivms_resources;";
            try
            {
                MySqlCommand Cmd = new MySqlCommand(cmd, MySQLConnect);
                MySqlDataReader DataReader = Cmd.ExecuteReader();
                while (DataReader.Read())
                {
                    string[] temp = new string[2];
                    temp[0] = DataReader[0].ToString();
                    temp[1] = DataReader[1].ToString();
                    result.Add(temp);
                    Console.WriteLine(temp[0] + " " + temp[1]);
                }
            }
            catch(MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
