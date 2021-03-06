﻿using DAL;
using Model;
using System.Collections.Generic;

namespace BLL
{
    public class TWindowBLL : BLLBase<TWindowDAL, TWindowModel> 
    {
        public TWindowBLL()
            : base()
        {
        }
        
        public TWindowBLL(string connName)
            : base(connName)
        {
        }

        public TWindowBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.dal.GetGridData();
        }

        /// <summary>
        /// 根据窗口区域获取窗口号列表
        /// </summary>
        /// <param name="aList"></param>
        /// <returns></returns>
        public List<string> GetWindowByArea(string aList)
        {
            return dal.GetWindowByArea(aList);
        }

        //RateService相关

        public object RS_GetWindowList()
        {
            return dal.RS_GetWindowList();
        }

        public object RS_GetUserListByWindowNo(string winNum)
        {
            return dal.RS_GetUserListByWindowNo(winNum);
        }

        public string RS_GetUserPhoto(string userCode)
        {
            return dal.RS_GetUserPhoto(userCode);
        }

        public object RS_GetModel(string winNum, string userCode)
        {
            return dal.RS_GetModel(winNum, userCode);
        }
    }
}
