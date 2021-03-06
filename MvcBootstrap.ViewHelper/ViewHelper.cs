﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections;
using MvcBootstrap.EFModel;
using MvcBootstrap.DAO;
using MvcBootstrap.IDAO;

public static class ViewHelper
{
    /// <summary>
    /// 生成用户功能菜单
    /// </summary>
    public static MvcHtmlString CreateMenu(this HtmlHelper helper)
    {
        using (DBEntity db = new DBEntity())
        {
            int roleID = 1;
            IEnumerable<UserBrowseViewModel> modules = db.GetUserBrowse(roleID).AsEnumerable();

            string parentMenu = "<a href=\"#{0}\" class=\"nav-header\" data-toggle=\"collapse\"><i class=\"ico-menu ico-{1}\"></i>{2}</a>";
            string childMenu = "<ul id=\"{0}\" class=\"nav nav-list collapse in\">{1}</ul>";
            string childContent = "<li><a target=\"content\" href=\"/{0}\"><i class=\"ico-menu ico-{1}\"></i>{2}</a></li>";

            IList<UserBrowseViewModel> parentModules = modules.GetEntities(m => m.ParentId == null).ToList();
            IEnumerable<Module> childModules = null;
            StringBuilder strBuilder = new StringBuilder();
            StringBuilder childBuilder = new StringBuilder();
            foreach (var parent in parentModules)
            {
                strBuilder.AppendFormat(parentMenu, parent.Name + "-menu", parent.Code, parent.Name);
                childModules = db.Module.GetEntities(m => m.ParentId == parent.ID);
                foreach (var child in childModules)
                {
                    childBuilder.AppendFormat(childContent, child.Url, child.Code, child.Name);
                }

                strBuilder.AppendFormat(childMenu, parent.Name + "-menu", childBuilder.ToString());
                childBuilder.Clear();
            }

            return MvcHtmlString.Create(strBuilder.ToString());
        }
    }

    /// <summary>
    /// 生成权限操作选项
    /// </summary>
    public static MvcHtmlString Operations(this HtmlHelper helper)
    {
        using (DBEntity db = new DBEntity())
        {
            IDictionary<int, string> operations = db.Operation
                                                    .Select(s => new { s.ID, s.Name })
                                                    .AsEnumerable()
                                                    .ToDictionary(k => k.ID, k => k.Name);
            string label = "<label class=\"checkbox inline ml10\"><input type=\"checkbox\" id=\"op{0}\" name=\"op{0}\" />{1}</label>";
            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyValuePair<int, string> item in operations)
            {
                strBuilder.AppendFormat(label, item.Key, item.Value);
            }

            return MvcHtmlString.Create(strBuilder.ToString());
        }
    }

    /// <summary>
    /// 权限分配时，生成每个资源对应的权限
    /// </summary>
    public static MvcHtmlString DistributeOptions(this HtmlHelper helper, int moduleId)
    {
        StringBuilder strBuilder = new StringBuilder();
        string label = "<form class=\"js-form-permission\" name=\"setPermission\"><input type=\"checkbox\" class=\"js-checkall-permission\" style=\"margin-top:-2px\" data-toggle=\"tooltip\" data-placement=\"top\" data-original-title=\"全选\" /><label class=\"inline mr40 pl20\">{0}</label>";
        string checkbox = "<input type=\"checkbox\" name=\"{0}-{1}\" style=\"margin:-2px 8px 0 8px\" />{2}";
        using (DBEntity db = new DBEntity())
        {
            IEnumerable<Module> modules = db.Module.GetEntities(m => m.ParentId == moduleId);
            string[] operations = null;
            int actionId = 0;
            Operation operation = null;
            foreach (Module module in modules)
            {
                if (!string.IsNullOrWhiteSpace(module.Operations))
                {
                    strBuilder.AppendFormat(label, module.Name);
                    operations = module.Operations.Split(',');
                    foreach (string op in operations)
                    {
                        actionId = Convert.ToInt32(op);
                        operation = db.Operation.GetEntity(o => o.ID == actionId);
                        strBuilder.AppendFormat(checkbox, module.ID, operation.ID, operation.Name);
                    }
                    strBuilder.Append("</form><p></p>");
                }
            }
        }

        return MvcHtmlString.Create(strBuilder.ToString());
    }
}