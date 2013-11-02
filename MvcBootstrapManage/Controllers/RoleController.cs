﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBootstrapManage.Models;
using MvcBootstrapManage.ViewModel;

namespace MvcBootstrapManage.Controllers
{
    public class RoleController : ManageController
    {
        protected override int TotalCount
        {
            get { return db.Role.Count(); }
        }

        public override ActionResult Index()
        {
            var result = db.Role.Take(base.PageSize).ToList();
            ViewData["ParentModule"] = db.Module.Where(m => m.ParentId == null).ToList();
            return View(result);
        }

        public override ActionResult Index(int? pageIndex)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Modify(RoleEditViewModel viewModel)
        {
            Role role = db.Role.Where(m => m.ID == viewModel.ID).Single();
            role.Name = viewModel.Name;
            role.Remark = viewModel.Remark;
            role.IsEnable = int.Parse(viewModel.IsEnable) == 1 ? true : false;
            db.SaveChanges();
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Update(FormCollection formInfo)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Delete(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Create(FormCollection formInfo)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Search(string name)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult SetPermission(int id, FormCollection formInfo)
        {
            int controllerId;
            int actionId;
            foreach (string item in formInfo.AllKeys)
            {
                controllerId = Convert.ToInt32(item.Split('-')[0]);
                actionId = Convert.ToInt32(item.Split('-')[1]);
                db.Permission.AddObject(new Permission() { RoleID = id, ControllerID = controllerId, ActionID = actionId });
            }
            db.SaveChanges();
            return new EmptyResult();
        }
    }
}
