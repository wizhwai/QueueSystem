﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class DALBase<T> : AbstractDALBase<T> where T : ModelBase
    {
        protected DbContext db;
        protected string connName = "MySQL", areaNo;

        public DALBase()
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
        }

        public DALBase(string connName)
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
            this.connName = connName;
        }

        public DALBase(string connName, string areaNo)
        {
            this.db = Factory.Instance.CreateDbContext(connName);
            this.areaNo = areaNo;
            this.connName = connName;
        }

        public DALBase(DbContext db)
        {
            this.db = db;
            this.areaNo = ConfigurationManager.AppSettings["AreaNo"];
        }

        public DALBase(DbContext db, string areaNo)
        {
            this.db = db;
            this.areaNo = areaNo;
        }

        #region CommonMethods

        public override IQuery<T> GetQuery()
        {
            return db.Query<T>().Where(p => p.AreaNo == this.areaNo);
        }

        public override List<T> GetModelList()
        {
            return this.GetQuery().ToList();
        }

        public override List<T> GetModelList(Expression<Func<T, bool>> predicate)
        {
            return this.GetQuery().Where(predicate).ToList();
        }

        public override T GetModel(int id)
        {
            return db.Query<T>().Where(p => p.ID == id && p.AreaNo == this.areaNo).FirstOrDefault();
        }

        public override T GetModel(Expression<Func<T, bool>> predicate)
        {
            return this.GetQuery().Where(predicate).FirstOrDefault();
        }

        public override T Insert(T model)
        {
            model.ID = this.GetMaxId();
            model.AreaNo = this.areaNo;
            return db.Insert(model);
        }

        public override int Update(T model)
        {
            if (model.AreaNo != this.areaNo)
                return -1;
            return this.db.Update(model);
        }

        public override int Delete(T model)
        {
            if (model.AreaNo != this.areaNo)
                return -1;
            return this.db.Delete(model);
        }

        public override int GetMaxId()
        {
            int maxId = -1;
            using (var maxDb = Factory.Instance.CreateDbContext(this.connName))
            {
                try
                {
                    var paraList = new DbParam[]{
                    new DbParam("areaNo",this.areaNo),
                    new DbParam("tableName",this.tableName),
                    new DbParam("maxId", null)
                    };
                    maxDb.Session.CommandTimeout = 60;
                    maxDb.Session.BeginTransaction();
                    var maxObj = maxDb.Session.ExecuteScalar("select maxId+1 from f_maxid where areaNo=@areaNo and tableName=@tableName FOR UPDATE;", paraList);
                    if (maxObj == null)
                    {
                        maxId = 1;
                        paraList[2].Value = maxId;
                        var insertSql = "insert into f_maxid(areaNo,tableName,maxId) values(@areaNo,@tableName,@maxId) ";
                        maxDb.Session.ExecuteNonQuery(insertSql, paraList);
                    }
                    else
                    {
                        maxId = Convert.ToInt32(maxObj);
                        paraList[2].Value = maxId;
                        var updateSql = "update f_maxid set maxId=@maxId where areaNo=@areaNo and tableName=@tableName";
                        maxDb.Session.ExecuteNonQuery(updateSql, paraList);
                    }
                    maxDb.Session.CommitTransaction();
                    return maxId;
                }
                catch (Exception ex)
                {
                    maxDb.Session.RollbackTransaction();
                    throw ex;
                }
            }
        }

        #endregion
    }
}
