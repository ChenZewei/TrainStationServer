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
        private MySqlDataReader DataReader;
        private MySqlCommand Cmd;
        public static MySqlConnection MySQLConnect = new MySqlConnection("server=localhost; user id=root; Password=123456789; database=opensips; persist security info=False");
        //public static MySqlConnection MySQLConnect = new MySqlConnection("server=192.168.80.13; user id=ivms; Password=ivmspwd; database=opensips; persist security info=False");
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

        ~DataBase()
        {
            try
            {
                MySQLConnect.Close();
            }
            catch (MySqlException e)
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

        public void Update(string table, string[] columes, string[] values, string[] locColumes, string[] locValues, int length, int locLength)
        {
            int state;
            if (values.Length != length || columes.Length != length || length <= 1)
                return;
            cmd = "update " + table + " set " + columes[0] + "='" + values[0] + "'";
            for (int i = 1; i < length; i++)
            {
                cmd += ", " + columes[i] + "='" + values[i] + "'";
            }

            cmd += " where " + locColumes[0] + "='" + locValues[0] + "'";

            for (int i = 1; i < locLength; i++)
            {
                cmd += ", " + locColumes[i] + "='" + locValues[i] + "'";
            }

            try
            {
                MySqlCommand Cmd = new MySqlCommand(cmd, MySQLConnect);
                state = Cmd.ExecuteNonQuery();
                if (state == 1)
                    Console.WriteLine("数据插入成功！");

            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void Insert(string database, string[] columes, string[] values)
        {

            int len = columes.Length;
            int state;
            if (values.Length != len || len <= 1)
                return;
            cmd = buildInsertCmd(database, columes, values);

            try
            {
                Cmd = new MySqlCommand(cmd, MySQLConnect);
                state = Cmd.ExecuteNonQuery();
                if (state == 1)
                    Console.WriteLine("数据插入成功！");

            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

        }
        /// <summary>
        /// 向数据库插入n列n行数据（重载方法）
        /// </summary>
        /// <example>
        /// Eg1:
        /// string database = “ivms_sourse”;
        /// string[] columes = {"id", "name", "location"};
        /// List<string> resId = new List<string> { "6100011201000102", "6100011201000103" };
        /// List<string> name = new List<string> { "通道2", "通道3" };
        /// List<string> location = new List<string> { "外勤", "内勤" };
        /// Insert(database, columes, resId, name, location);
        /// ALSO: 
        /// List<string>[] values = {resId,name,location};
        /// Insert(database, columes, values);
        /// 
        /// Eg2:
        /// string resId = "6100011201000102";
        /// string name = "通道2";
        /// string location = "外勤"；
        /// Insert(database, columes, resId, name, location);
        /// ALSO:
        /// string[] value = {resId, name, location};
        /// Insert(database, columes, value)
        /// </example>
        /// <param name="database"></param>
        /// <param name="columes"></param>
        /// <param name="values"></param>
        public void Insert(string database, string[] columes,params List<string>[] values)
        {
            int len = columes.Length;
            int state;
            if (values.Length != len || len <= 1)
                return;
            
            try
            {
                cmd = buildInsertCmd(database, columes, values);
                Cmd = new MySqlCommand(cmd, MySQLConnect);
                state = Cmd.ExecuteNonQuery();
                if (state == 1)
                    Console.WriteLine("数据插入成功！");

            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// 构造sql语句方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <param name="database"></param>
        /// <param name="columes"></param>
        /// <param name="values"></param>
        /// <returns>返回构造好的sql语句</returns>
        //public static string buildCmd<T>(string operation, string database, string[] columes, params T[] values)
        //{
        //    string cmdText = operation.ToLower();
            
        //    switch (cmdText)
        //    {
        //        case "insert into":
        //            return buildInsertCmd(database, columes, values);
        //        case "update":
        //            break;
        //        case "select":
        //            break;
        //        case "replace":
        //            break;
        //        default:
        //            break;
        //    }
        //    return null;
        //}
        //public static string buildCmd<T>(string operation, string database, string[] columes, T value)
        //{
        //    T[] values = {value};
        //    return buildCmd(operation, database, columes, values);
        //}

        //public static string buildInsertCmd<T>(string database, string[] coluemes, params T[] values)
        //{
        //    Console.WriteLine("未处理参数类型values：");
        //    Console.WriteLine(values.GetType().ToString());
        //    return null;

        //}

        /// <summary>
        /// 构造向数据库插入n列1行数据的sql语句
        /// </summary>
        /// <param name="database"></param>
        /// <param name="columes"></param>
        /// <param name="values"></param>
        /// <returns>构造完成的insert语句</returns>

        public static string buildInsertCmd(string database, string[] columes, params string[] values)
        {
            string cmdText = "insert into " + database + "(";
            for (int i = 0; i < columes.Length; i++)
            {
                cmdText += columes[i];
                if (i == columes.Length - 1)
                {
                    cmdText += ")";
                    break;
                }
                cmdText += ",";
            }
            cmdText += " values";
            cmdText += "(";
            for (int i = 0; i < values.Length; i++)
            {

                cmdText += ("'" + values[i] + "'");
                if (i == values.Length - 1)
                {
                    break;
                }
                cmdText += ",";
            }
            cmdText += ");";
            return cmdText;
        }
        //public static string buildInsertCmd(string database, string colume, string value)
        //{
        //    string[] columes = { colume };
        //    string[] values = { value };
        //    return buildInsertCmd(database, colume, value);
        //}
        /// <summary>
        /// 构造向数据库插入n列n行数据的sql语句（重载方法）
        /// </summary>
        /// <param name="database"></param>
        /// <param name="columes"></param>
        /// <param name="values"></param>
        /// <returns>构造完成的insert语句</returns>
        public static string buildInsertCmd(string database, string[] columes, params List<string>[] values)
        {

            string cmdText = "insert into " + database + "(";
            for (int i = 0; i < columes.Length; i++)
            {
                cmdText += columes[i];
                if (i == columes.Length - 1)
                {
                    cmdText += ")";
                    break;
                }
                cmdText += ",";
            }
            cmdText += " values";
            for (int i = 0; i < values[0].Count; i++)
            {
                cmdText += "(";
                for (int j = 0; j < values.Length; j++)
                {

                    cmdText += ("'" + values[j][i] + "'");
                    if (j == values.Length - 1)
                    {
                        break;
                    }
                    cmdText += ",";
                }
                cmdText += ")";
                if (i == values[0].Count - 1)
                {
                    break;
                }
                cmdText += ",";
            }
            cmdText += ";";
            return cmdText;
        }

        //public static string cmdInsertBuilder(string database, string colume, List<string> value)
        //{
        //    string[] columes = {colume};
        //    List<string>[] values = { value };
        //    return cmdInsertBuilder(database, columes, values);
        //}
        //public void Insert(string database,string[] columes,string[] values)
        //{
            
        //    int len = columes.Length;
        //    int state;
        //    if (values.Length != len || len <= 1)
        //        return;
        //    cmd = "insert into " + database + "(" + columes[0];
        //    for (int i = 1; i < columes.Length;i++ )
        //    {
        //        cmd += "," + columes[i];
        //    }
        //    cmd += ") values('" + values[0] + "'";
        //    for (int i = 1; i < values.Length;i++ )
        //    {
        //        cmd += ",'" + values[i] + "'";
        //    }
        //    cmd += ");";
            
        //    try
        //    {
        //        Cmd = new MySqlCommand(cmd, MySQLConnect);
        //        state = Cmd.ExecuteNonQuery();
        //        if (state == 1)
        //            Console.WriteLine("数据插入成功！");

        //    }
        //    catch (MySqlException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }

        //}

        //public void InsertList(string database, List<string> columes, List<string> values)
        //{

        //    int len = columes.Count;
        //    if (values.Count != len || len <= 1)
        //        return;
        //    cmd = "insert into " + database + "(" + columes[0];
        //    for (int i = 1; i < columes.Length; i++)
        //    {
        //        cmd += "," + columes[i];
        //    }
        //    cmd += ") values('" + values[0] + "'";
        //    for (int i = 1; i < values.Length; i++)
        //    {
        //        cmd += ",'" + columes[i] + "'";
        //    }
        //    cmd += ");";
        //    try
        //    {
        //        MySqlCommand Cmd = new MySqlCommand(cmd, MySQLConnect);
        //        MySqlDataReader DataReader = Cmd.ExecuteReader();
        //        if (Cmd.ExecuteNonQuery() > 0)
        //        {
        //            Console.WriteLine("数据插入成功！");
        //        }
        //    }
        //    catch (MySqlException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }

        //}
    }
}
