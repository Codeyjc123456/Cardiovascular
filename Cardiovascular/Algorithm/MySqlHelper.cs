using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sql = MySql.Data.MySqlClient;

namespace Cardio.Algorithm
{
    public abstract class MySqlHelper
    {
        #region 字符串处理
        /// <summary>
        /// 模糊查询字符处理
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="id"></param>
        /// <returns>数值型返回True，字符型返回False</returns>
        public static bool GetLikeStrOrNum(ref string userName, ref int id)
        {
            //int id;
            if (!int.TryParse(userName.Substring(0, 1), out id))
            {
                id = userName.Length;
                for (int i = 0; i <= id; i++)
                {
                    userName = userName.Insert(2 * i, "%");
                }
                return false;
            }
            else
                return true;
        }
        public static string GetLikeStr(string userName)
        {
            int id = userName.Length;
            for (int i = 0; i <= id; i++)
            {
                userName = userName.Insert(2 * i, "%");
            }
            return userName;
        }
        /// <summary>
        /// 规范化查询的字符串
        /// </summary>
        /// <param name="objStr"></param>
        /// <returns>返回不含特殊字符的字符串</returns>
        public static string GetPureStr(string objStr)
        {
            return objStr.Replace(" ", "").Replace("'", "");
        }
        public static DataSet SearchTotalPageAndResult(string tableName, string colName, string condition, int beginRow,
            int endRow, ref double totalCount, MySqlParameter[] paras = null)
        {
            System.Text.StringBuilder strs = new System.Text.StringBuilder();
            if (string.IsNullOrEmpty(condition))
                condition = "1=1";
            if (totalCount <= 0)//需要查询页数
            {
                strs.Append("select count(*) from " + tableName);
                strs.Append(" where " + condition);

                totalCount = double.Parse(sql.MySqlHelper.ExecuteDataRow(DBConnectionString, strs.ToString(), paras)[0].ToString());
            }
            if (totalCount > 0)
            {
                strs.Length = 0;
                strs.Append("select " + colName + " from " + tableName);
                strs.Append(" where " + condition);
                strs.Append(" limit " + beginRow + "," + endRow);
                return sql.MySqlHelper.ExecuteDataset(DBConnectionString, strs.ToString(), paras);
            }
            else
            {
                return null;
            }
        }
        #endregion
        #region 数据库连接字符串
        public static readonly string DBConnectionString = ""; //System.Configuration.ConfigurationManager.ConnectionStrings["SqlConStr"].ToString();
        #endregion
        #region PrepareCommand
        /// <summary>
        /// Command预处理
        /// </summary>
        /// <param name="conn">MySqlConnection对象</param> 
        /// <param name="trans">MySqlTransaction对象，可为null</param>
        /// <param name="cmd">MySqlCommand对象</param>
        /// <param name="cmdType">CommandType，存储过程或命令行</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">MySqlCommand参数数组，可为null</param>
        private static void PrepareCommand(MySqlConnection conn, MySqlTransaction trans, MySqlCommand cmd, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary> 
        /// 执行命令 
        /// </summary> 
        /// <param name="connectionString">数据库连接字符串</param> 
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param> 
        /// <param name="cmdText">SQL语句或存储过程名</param> 
        /// <param name="cmdParms">MySqlCommand参数数组</param> 
        /// <returns>返回受引响的记录行数</returns>

        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="conn">Connection对象</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">MySqlCommand参数数组</param>
        /// <returns>返回受引响的记录行数</returns>

        public static int ExecuteNonQuery(MySqlConnection conn, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary> 
        /// 执行事务 
        /// </summary> 
        /// <param name="trans">MySqlTransaction对象</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">MySqlCommand参数数组</param> 
        /// <returns>返回受引响的记录行数</returns>

        public static int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(trans.Connection, trans, cmd, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        #endregion
        #region ExecuteScalar
        /// <summary>
        /// 执行命令，返回第一行第一列的值 
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">MySqlCommand参数数组</param>
        /// <returns>返回Object对象</returns>

        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                PrepareCommand(connection, null, cmd, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 执行命令，返回第一行第一列的值
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">MySqlCommand参数数组</param>
        /// <returns>返回Object对象</returns>

        public static object ExecuteScalar(MySqlConnection conn, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
        #endregion
        #region ExecuteReader
        /// <summary>
        /// 执行命令或存储过程，返回MySqlDataReader对象 
        /// 注意MySqlDataReader对象使用完后必须Close以释放MySqlConnection资源
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">MySqlCommand参数数组</param>
        /// <returns></returns>

        public static MySqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);
                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return dr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        #endregion
        #region ExecuteDataSet
        /// <summary>
        /// 执行命令或存储过程，返回DataSet对象
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型(存储过程或SQL语句)</param>
        /// <param name="cmdText">SQL语句或存储过程名</param> 
        /// <param name="cmdParms">MySqlCommand参数数组(可为null值)</param>
        /// <returns></returns>

        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                conn.Close();
                cmd.Parameters.Clear();
                return ds;
            }
        }
        #endregion
    }
}
