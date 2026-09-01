using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using SqlSugar;
using System.Configuration;
using Cardio.Util;

namespace Cardio.DAL
{
    public class BaseDAL<T>  where T : BaseEntity, new() 
    {
        public SqlSugarScope Db = null;
        #region 构造函数
        public BaseDAL()
        {
            try
            {
                Db = new SqlSugarScope(new ConnectionConfig()
                {
                    ConnectionString = @"Data Source = ./Resources/Data/BroshareCADB.db",
                    DbType = SqlSugar.DbType.Sqlite,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                }, db => {
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(sql + pars);
                    };
                });
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
            }

        }
        #endregion

        /// <summary>
        /// 插入用户注册信息
        /// </summary>
        /// <param name="userInfo">用户信息实例化对象</param>
        /// <returns></returns>
        public int Insert(T o)
        {
            try
            {
                int flag = Db.Insertable(o).ExecuteReturnIdentity();
                if(flag>0)
                    return flag;
                //LogUtil.Info("SqlSugarHelper", flag.ToString());
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
            }

            return 0;
        }


        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userInfo">用户信息实体类</param>
        /// <returns></returns>
        public bool Update(T o)
        {
            try
            {
                int flag = Db.Updateable(o).ExecuteCommand();
                LogUtil.Info("SqlSugarHelper", flag.ToString());
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
                return false;
            }

            return true;
        }
        public bool Update(T o, string s)
        {
            try
            {
                int flag = Db.Updateable(o).Where(s, new { }).ExecuteCommand();
                if (flag > 0) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogUtil.Error("SqlSugarHelperUpdateString", ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 根据id删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            try
            {
                int flag = Db.Deleteable<T>().Where(it=>it.Id==id).ExecuteCommand();
                if (flag > 0)
                    return true;
                LogUtil.Info("SqlSugarHelper", flag.ToString());
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
            }

            return false;
        }

        public T FindByID(int id)
        {
            try
            {
                var entity = Db.Queryable<T>().Where(it=>it.Id == id).First();
                return (T)entity;
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
                return null;
            }

        }

        public T Find(string s)
        {
            try
            {
                var entity = Db.Queryable<T>().Where(s, new { }).First();
                return (T)entity;
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
                return null;
            }
      
        }

        public int FindCount(string s)
        {
            try
            {
                var count = Db?.Queryable<T>().Where(s, new { }).Count() ?? 0;
                return count;
            }
            catch (Exception ex)
            {
                LogUtil.Error("SqlSugarHelper", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="count"></param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public List<T> Finds(int page,int size, ref int count,string where,string order)
        {
            try
            {
                var list = Db.Queryable<T>().Where(where, new { }).OrderBy(order).ToPageList(page,size,ref count);
                return list;
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
                return null;
            }

        }

        public List<T> Finds(string s)
        {
            try
            {
                var list = Db.Queryable<T>().Where(s, new { }).ToList();
                return list;
            }
            catch (Exception ex)
            {
                LogUtil.Info("SqlSugarHelper", ex.Message);
                return null;
            }

        }
    }
}
