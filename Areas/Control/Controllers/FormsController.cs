using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using NuGet.Packaging;
using Ramallah.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Microsoft.AspNetCore.Html;
using Ramallah.Helpers;
using AutoMapper.Execution;
using Microsoft.Extensions.Localization;

namespace Ramallah.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    public class FormsController : Controller
    {
        private readonly DataContext _context;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public FormsController(DataContext context,IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _context = context;
        }

        // GET: FormsController
        public async Task<IActionResult> Index(string? keyword, int? stype, int? slang, Boolean? sType, int PageNumber = 1)
        {
			Boolean IsSuper = false;
			if (HttpContext.Session.GetString("is_super_admin") == "True") IsSuper = true;
			Nullable<int> UserId = null;
			UserId = int.Parse(HttpContext.Session.GetString("id"));

			int PageSize = 20;
			int dbPages = 1;

			List<Forms> formsList = null;
			
			if (!IsSuper && UserId != null)
            {
				formsList = await _context.Forms.Where(a => a.Deleted == false
			    && (keyword == null || (a.Title.Contains(keyword) || a.ArTitle.Contains(keyword)))
			    && (slang == null || a.LangId == slang)
			    && (sType == null || a.IsJobForm == sType)
			    )
			    .Include(a => a.Language)
			    .Include(a => a.User)
			    .Include(a => a.FormsEntries)
			    //.Skip((PageNumber - 1) * PageSize)
			    //.Take(PageSize)
			    .OrderByDescending(a => a.Id)
			    .ToListAsync();

				
				var formsToRemove = new List<Forms>();
				foreach (var form in formsList) // Loop to all forms
                {
                    bool found = false;                   
                    
                    //if Not Exist remove
                    if (!found) formsToRemove.Add(form);
                }
                foreach(var form in formsToRemove)
                {
                    formsList.Remove(form);
                }
			}
            else //Super admin user
            { 
				formsList = await _context.Forms.Where(a => a.Deleted == false
				&& (keyword == null || (a.Title.Contains(keyword) || a.ArTitle.Contains(keyword)))
				&& (slang == null || a.LangId == slang)
				&& (sType == null || a.IsJobForm == sType)
				)
				.Include(a => a.Language)
				.Include(a => a.User)
				.Include(a => a.FormsEntries)
				.Skip((PageNumber - 1) * PageSize)
				.Take(PageSize)
				.OrderByDescending(a => a.Id)
				.ToListAsync();

				dbPages = _context.Forms.Where(a => a.Deleted == false
			    && (keyword == null || (a.Title.Contains(keyword) || a.ArTitle.Contains(keyword)))
			    && (slang == null || a.LangId == slang)
			    && (sType == null || a.IsJobForm == sType)
			    )
			    .Count();
			}

            float paging = (float) dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            ViewBag.keyword = keyword;
            ViewBag.Language = slang;
            ViewBag.PagesCount = TotalPages;
            ViewBag.CurrentPage = PageNumber;

            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            return View(formsList);
        }

        // GET: FormsController/Questions/Id
        public async Task<IActionResult> Fields(int id)
        {           
            var Fields = await _context.FormsFields.Where(a => a.Deleted == false && a.FormId == id)
                .OrderBy(a => a.Priority)
                .ThenBy(a => a.Id)
                .ToListAsync();

            var form = await _context.Forms.FindAsync(id);
           
            if (form == null)
            {
                TempData["error"] = _localizer["Not Found"].Value;
                return RedirectToAction("Fields");
            }

            ViewBag.Form = form;

            string fields_list = "";
            if(Fields.Count() >0)
            {
                
                foreach (FormsFields item in Fields)
                {
                    string required = "false";
                    string multiple = "false";
                    string other = "false";
                    string toggle = "false";
                    string inline = "false";
                    string values = "";

                    fields_list += "{";

                    var Options = _context.FormsFieldsOptions.Where(a => a.FieldId == item.Id).ToList();
                    if (Options != null)
                    {
                        int OpCount = 0;
                        foreach(FormsFieldsOptions op in Options)
                        {
                            string selected = "false";
                            if (op.Selected) selected = "true";
                            if (OpCount != 0) values += ",";

                            values += "{";
                            values += "label: " + JsonConvert.SerializeObject(op.Label) + ",";
                            values += "value: " + JsonConvert.SerializeObject(op.Value) + ",";
                            values += "selected: " + selected + "";
                            values += "}";
                            OpCount++;
                        }
                    }
                    if (values !="") fields_list += "values :[" + values + "],";
                    //fields_list += "values: '',";
                    fields_list += "field_id: '" + item.Id + "',";
                    fields_list += "name: "+ JsonConvert.SerializeObject(item.Title) + ",";
                    fields_list += "label: "+ JsonConvert.SerializeObject(item.Label) + ",";
                    fields_list += "placeholder: "+ JsonConvert.SerializeObject(item.PlaceHolder) + ",";
                    fields_list += "helptxt: "+ JsonConvert.SerializeObject(item.PlaceHolder) + ",";
                    fields_list += "type: '"+item.Type+"',";
                    fields_list += "subtype: '"+item.SubType+"',";
                    fields_list += "description: " + JsonConvert.SerializeObject(item.Description) + ",";
                    //"name:"+$ff->fi_min_answer_number;
                    if (item.Required) required = "true";
                    else required = "false";
                    fields_list += "required: "+required+",";
                    fields_list += "value: "+ JsonConvert.SerializeObject(item.DefaultValue) + ",";
                    fields_list += "min: '"+item.MinAnsNumber+"',";
                    fields_list += "max: '"+item.MaxAnsNumber+"',";
                    fields_list += "maxlength: '"+item.MaxLength+"',";
                    fields_list += "step: '" + item.Step + "',";
                    fields_list += "rows: '"+item.Rows+"',";
                    if (item.AllowMultiple) multiple = "true";
                    else multiple = "false";
                    fields_list += "multiple: "+multiple+",";
                    if (item.EnableOther) other = "true";
                    else other = "false";
                    fields_list += "other: "+other+",";
                    if (item.Toggle) toggle = "true";
                    else toggle = "false";
                    fields_list += "fi_toggle: "+toggle+",";

                    if (item.Inline) inline = "true";
                    else inline = "false";
                    fields_list += "inline: "+inline+",";
                    fields_list += "className: '"+item.Class+"'";
                    fields_list += "},";
                }                
            }
            ViewBag.Fields = fields_list;
            return View(Fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fields(int? id,string all_fields)
        {
            if(id == null)
            {
                TempData["error"] = _localizer["Not Found"].Value;
                return RedirectToAction("Fields");

            }

            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                TempData["error"] = _localizer["Not Found"].Value;
                return RedirectToAction("Fields");
            }
         
            List<FormsFields> Fields = new List<FormsFields>();
            if (all_fields != null)
            {
                TempData["success"] = _localizer["Data Saved"].Value;
                dynamic dynJson = JsonConvert.DeserializeObject(all_fields);
                int i = 1;

                //Check for deleted items and remove from DB
                List<FormsFields> currentFields = _context.FormsFields.Where(a => a.FormId == id && a.Deleted == false).ToList();
                foreach(FormsFields formField in currentFields)
                {
                    Boolean found = false;
                    foreach(var item in dynJson)
                    {
                        if(item.name == formField.Title)
                        {
                            //Item exist in form
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        _context.FormsFields.Remove(formField);//Remove the field as it is not exist with submitted fields
                    }                   
                }

                foreach (var item in dynJson)
                {
                    //try
                    //{
                    Random rnd = new Random();
                    string title = "";
                    string label = "";
                        

                    Boolean required = false;
                    Boolean allowmultiple = false;
                    Boolean inline = false;
                    Boolean other = false;

                    if (item.required == "true") required = true;
                    if (item.multiple == "true") allowmultiple = true;
                    if (item.inline == "true") inline = true;
                    if (item.other == "true") other = true;
                    int minLength = 0;
                    int maxLength = 500;
                    int rows = 3;

                    double step = 1.0;

                    Nullable<int> minAnsNumber = null;
                    Nullable<int> maxAnsNumber = null;
                        

                    if (item.min !=null) minAnsNumber = item.min;
                    if (item.max != null) maxAnsNumber = item.max;

                    if (item.step != null) step = item.step;

                    if (item.maxlength != null) maxLength = item.maxlength;
                    if (item.rows != null) rows = item.rows;

                    string tName = item.name;
                    string tLabel = item.label;
                    if(!string.IsNullOrEmpty(tName)) title = Regex.Replace(tName, "<.*?>", ""); //To remove all html from the title and label
                    if(!string.IsNullOrEmpty(tLabel)) label = Regex.Replace(tLabel, "<.*?>", "");//To remove all html from the title and label

                    if (string.IsNullOrEmpty(title)) title = item.type+"-"+ rnd.Next(1000000,9999999)+"-"+i;
                    if (string.IsNullOrEmpty(label)) label = item.type;

                    var Field = new FormsFields
                    {
                            FormId = form.Id,
                            Title = title,
                            ArTitle = title,
                            Label = label,
                            ArLabel = label,
                            PlaceHolder = item.placeholder,
                            ArPlaceHolder = item.placeholder,
                            Type = item.type,
                            SubType = item.subtype,
                            Description = item.description,
                            MinAnsNumber = minAnsNumber,
                            MaxAnsNumber = maxAnsNumber,
                            Step = step,
                            Required = required,
                            Priority = i,
                            DefaultValue = item.value,
                            MinLength = minLength,
                            MaxLength = maxLength,
                            Rows = rows,
                            AllowMultiple = allowmultiple,
                            EnableOther = other,
                            Toggle = false,
                            Inline = inline,
                            Class = item.className,
                            Style = "",
                            AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1"),
                            AddedTime = DateTime.Now,
                            Active = true,
                            Deleted = false
                        };

                        int op_priority = 1;
                        Field.Options = new List<FormsFieldsOptions>();
                        //To insert the options if have
                        if (item.values != null)
                        {
                            //IEnumerable<FormsFieldsOptions> op_list = _context.FormsFieldsOptions.Where(u => u.FieldId == Field.Id);
                            //_context.FormsFieldsOptions.RemoveRange(op_list);
                            foreach (var option in item.values)
                            {
                                Boolean opSelected = false;
                                if (option.selected == "true") opSelected = true;

                                var FieldOption = new FormsFieldsOptions
                                {                 
                                    FieldId = Field.Id,
                                    Label = option.label,
                                    ArLabel = option.label,
                                    Value = option.value,
                                    ArValue = option.value,
                                    Selected = opSelected,
                                    Priority = op_priority,
                                    AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1"),
                                    AddedTime = DateTime.Now,
                                    Active = true,
                                    Deleted = false
                                };
                                //_context.FormsFieldsOptions.Add(FieldOption);
                                Field.Options.Add(FieldOption);
                                op_priority++;
                            }
                        }
                        //else
                        //{
                        //    IEnumerable<FormsFieldsOptions> op_list = _context.FormsFieldsOptions.Where(u => u.FieldId == Field.Id);
                        //    _context.FormsFieldsOptions.RemoveRange(op_list);
                        //}

                        //Check if field already exist and being update
                        if(!_context.FormsFields.Where(a=> a.Title == title).Any())
                        {
                            //New Item will be added
                            Fields.Add(Field);
                        }
                        else
                        {
                            //Edit existing Item
                            FormsFields EditField = _context.FormsFields.Where(a => a.Title == Field.Title).FirstOrDefault();
                            if (EditField != null)
                            {
                                //Edit all fields according to sumitted data
                                EditField.Label = Field.Label;
                                EditField.ArLabel = Field.ArLabel;
                                EditField.PlaceHolder = Field.PlaceHolder;
                                EditField.ArPlaceHolder = Field.ArPlaceHolder;
                                EditField.Type = Field.Type;
                                EditField.SubType = Field.SubType;
                                EditField.MinAnsNumber = Field.MinAnsNumber;
                                EditField.MaxAnsNumber = Field.MaxAnsNumber;
                                EditField.Step = Field.Step;
                                EditField.Required = Field.Required;
                                EditField.Priority = Field.Priority;
                                EditField.DefaultValue = Field.DefaultValue;
                                EditField.MinLength = Field.MinLength;
                                EditField.MaxLength = Field.MaxLength;
                                EditField.Rows = Field.Rows;
                                EditField.AllowMultiple = Field.AllowMultiple;
                                EditField.EnableOther = Field.EnableOther;
                                EditField.Toggle = Field.Toggle;
                                EditField.Inline = Field.Inline;
                                EditField.Class = Field.Class;
                                EditField.Description = Field.Description;

                                //Remove all previous options
                                _context.FormsFieldsOptions.RemoveRange(_context.FormsFieldsOptions.Where(a => a.FieldId == EditField.Id));
                            //Insert all newely submitted options

                                if (Field.Options != null)
                                {
                                    EditField.Options = new List<FormsFieldsOptions>();
                                    foreach(var item1 in Field.Options)
                                    {
                                    //Insert all options of field
                                    EditField.Options.Add(item1);
                                    }
                                    //EditField.Options.AddRange(Field.Options);
                                }
                                _context.FormsFields.Update(EditField);
                            }
                            
                        }
                       // Console.WriteLine(Field);
                        i++;

                    //}
                    //catch(Exception ex)
                    //{
                    //    Console.Write(ex.Message);
                    //}
                    
                }
                if (Fields.Count() > 0) { //Some fields will be inserted
                    _context.FormsFields.RemoveRange(_context.FormsFields.Where(a => a.FormId == id && (a.Type=="paragraph" || a.Type == "header")));//Remove Old Fields
                    _context.FormsFields.AddRange(Fields);
                }
                await _context.SaveChangesAsync();
                TempData["success"] = _localizer["Field Inserted"].Value;
                return RedirectToAction("Index");
            }
            else
            {
                //Remove All fields
                _context.FormsFields.RemoveRange(_context.FormsFields.Where(a => a.FormId == id));
                TempData["error"] = _localizer["Empty Fields"].Value;
            }            

            ViewBag.Form = form;
            ViewBag.Fields = "";
            return View();
        }

        public ActionResult Preview(int id)
        {
            var form = _context.Forms.Where(a => a.Deleted == false && a.Id == id && a.IsJobForm == false).FirstOrDefault();
            if (form == null) return RedirectToAction("Index");

            ViewBag.Form = form;
            ViewBag.FormFields = _context.FormsFields.Where(a => a.Deleted == false && a.FormId == id)
            .Include(a => a.Options)
            .OrderBy(a => a.Priority).ThenBy(a => a.Id).ToList();

            return View(form);
        }

        // GET: FormsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public IActionResult ResultDetails(int resId)
        {
            List<FormsEntriesFields> entries = _context.FormsEntriesFields.Where(a => a.Deleted == false && a.EntryId == resId).Include(a => a.Field)
                .OrderBy(a => a.Field.Priority).ThenBy(a => a.Id)
                .ToList();
            if (entries == null)
            {
                return Json(new
                {
                    result = false,
                    msg = _localizer["No Results"].Value
                });
            }
            else
            {
                return Json(new
                {
                    result = true,
                    answers = entries
                });
            }
        }

    public ActionResult Results(int? formId,Boolean? type,int PageNumber = 1)
    {           
        int PageSize = 20;

        var apps = _context.FormsEntries.Where(a => a.Deleted == false 
        && (type==null || a.Form.IsJobForm == type)
        && (formId==null || a.FormId == formId)
        )               
        .Include(a=> a.Form)
        .OrderByDescending(a => a.Id)
        .Skip((PageNumber - 1) * PageSize)
        .Take(PageSize)
        .ToList();

        var dbPages = _context.FormsEntries.Where(a => a.Deleted == false && a.FormId == formId
        && (type == null || a.Form.IsJobForm == type)
        && (formId == null || a.FormId == formId)
        ).Count();


        float paging = (float) dbPages / PageSize;
        double TotalPages = Math.Round(paging);
        ViewBag.PagesCount = TotalPages;
        ViewBag.DBPages = dbPages;
        if (formId != null)
        {
            Forms form = _context.Forms.Where(a => a.Id == formId).FirstOrDefault();
            ViewBag.Form = form;
            ViewBag.FormTitle = form.Title;
        }

        ViewBag.CurrentPage = PageNumber;
        ViewBag.TotalApps = apps.Count();

        ViewBag.Type = type;
        ViewBag.FormId = formId;
        ViewBag.Forms = _context.Forms.Where(a => a.Deleted == false).OrderBy(a => a.Title).ToList();

        return View(apps);
    }

    public ActionResult EntryDetails(int entryId)
    {
    return View();
    }

    // GET: FormsController/Create
    public ActionResult Create()
    {
        ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
        return View();
    }

    // POST: FormsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,ArTitle,Description,ArDescription,Direction,LangId,SubmitLabel,ArSubmitLabel,AddedBy,AddedTime,Type,IsJobForm,IsPublic,StartDate,ExpireDate")] Forms form)
    {
        form.IsJobForm = true;
        form.IsPublic = false;

        if (ModelState.IsValid)
        {
        form.AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1");
        form.AddedTime = DateTime.Now;

        form.IsJobForm = false;
        form.IsPublic = false;

        //Filter Against Vulnerable content
        form.Title = Functions.RemoveHtml(form.Title);
        form.ArTitle = Functions.RemoveHtml(form.ArTitle);

        form.Description = Functions.FilterHtml(form.Description);
        form.ArDescription = Functions.FilterHtml(form.ArDescription);

        await _context.AddAsync(form);
        await _context.SaveChangesAsync();

        TempData["success"] = "Form added successfully...";                
            return RedirectToAction("Create");
        }

        TempData["error"] = "Cannot add form...";            
        ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();

        return View(form);
    }

    // GET: Control/Forms/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
    if (id == null || _context.Forms == null)
    {
        TempData["error"] = "Form not found...";
        return RedirectToAction("Index");
    }

    var form = await _context.Forms.FindAsync(id);

    if (form == null)
    {
        TempData["error"] = "Form not found...";
        return RedirectToAction("Index");
    }
        ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
        return View(form);
    }

    // POST: Control/Menus/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ArTitle,Description,ArDescription,Direction,LangId,SubmitLabel,ArSubmitLabel,AddedBy,AddedTime,IsJobForm,IsPublic,Type,StartDate,ExpireDate")] Forms form)
    {

        if (id != form.Id)
        {
        TempData["error"] = "Form not found...";
        return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            try
            {
            _context.Forms.Attach(form);
            _context.Entry(form).State = EntityState.Modified;
            _context.Entry(form).Property(p => p.AddedBy).IsModified = false;
            _context.Entry(form).Property(p => p.AddedTime).IsModified = false;
            _context.Entry(form).Property(p => p.IsJobForm).IsModified = false;
            _context.Entry(form).Property(p => p.IsPublic).IsModified = false;
            //_context.Entry(form).Property(p => p.IsJobForm).IsModified = false;
            //_context.Entry(form).Property(p => p.IsPublic).IsModified = false;

            //Filter Against Vulnerable content
            form.Title = Functions.RemoveHtml(form.Title);
            form.ArTitle = Functions.RemoveHtml(form.ArTitle);

            form.Description = Functions.FilterHtml(form.Description);
            form.ArDescription = Functions.FilterHtml(form.ArDescription);

            //_context.Update(form);
            await _context.SaveChangesAsync();
            TempData["success"] = "Form edited successfully...";
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["error"] = "Form not found...";
            return RedirectToAction("Index");
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
        return View(form);
    }

    // GET: FormsController/Delete/5
    public ActionResult Delete(int id)
    {
        return View();
    }

    // POST: FormsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
        return RedirectToAction(nameof(Index));
        }
        catch
        {
        return View();
        }
    }
}
}
