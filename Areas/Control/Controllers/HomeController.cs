using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;
using Shared.Service.Extensions;
using System.Drawing;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
//using Microsoft.Extensions.Localization;
//using Microsoft.AspNetCore.Localization;

namespace Ramallah.Areas.Control.Controllers
{
    //[Authorize]    
    [Area("Control")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<HomeController> _localizer;
        // GET: HomeController
        //[Authorize(Roles = "Admin")]
        public HomeController(DataContext context, IWebHostEnvironment webHostEnvironment, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
        }


        [HttpGet()]
        //[Route("GetFile/{*path}")]
        public IActionResult GetFile(string? path)
        {

            //var allowedDirectory = "/files/file";
            //var fullPath = Path.Combine(allowedDirectory, path);
            //!fullPath.StartsWith(allowedDirectory) || 
            //Console.WriteLine("Find the File");
            if (path == null)
            {
                return View();
            }

            path = path.Replace("~/", "wwwroot/");
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", Path.GetFileName(path));
            //return View();

        }

        public ActionResult Index(string? culture)
        {
            if (culture != null)
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
                );
            }

            Boolean IsSuper = false;
            if (HttpContext.Session.GetString("is_super_admin") == "True") IsSuper = true;
            Nullable<int> UserId = null;
            UserId = int.Parse(HttpContext.Session.GetString("id"));

               
            ViewBag.PagesCount = _context.Pages.Where(a => a.Deleted == false).Count();
            ViewBag.FilesCount = _context.Files.Where(a => a.Deleted == 0).Count();
            ViewBag.CatsCount = _context.Categories.Where(a => a.Deleted == 0).Count();

            ViewBag.mostVisited = _context.Pages.Where(a => a.Deleted == false).OrderByDescending(a => a.Views).ToList();

            //ViewBag.Title = _stringLocalizer["page.title"].Value;
            //ViewBag.PageTitle = _stringLocalizer["page.title"].Value;
            return View();
                      
        }

        public IActionResult getAreas(int cityId)
        {
            var areas = _context.Villages.Where(a => a.Deleted == false && (a.CityId == cityId || a.Id == 9999)).OrderBy(a => a.ArName).ToList();

            return Json(new
            {
                result = true,
                areas = areas
            });
        }

        
        //public async Task<IActionResult> ImportSpec()
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    // Path to the Excel file on the server
        //    //string serverFilePath = @"\\~\files\jerusalem.xlsx";
        //    string webRootPath = _webHostEnvironment.WebRootPath;
        //    string filePath = Path.Combine(webRootPath, "files", "Specialize.xlsx");

        //    // Name of the Excel worksheet where the data is located
        //    //String sheetName = "Academic";
        //    string sheetName = "Profession";

        //    // Create a list to hold the imported data
        //    List<LookupSpecialize> importedData = new List<LookupSpecialize>();

        //    // Load the Excel data into the list
        //    using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
        //    {
        //        //ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];
        //        foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
        //        {

        //            if (worksheet != null && worksheet.Name == sheetName)
        //            {
        //                int totalRows = worksheet.Dimension.Rows;
        //                // Loop through the rows in the Excel data
        //                for (int i = 2; i <= totalRows; i++)
        //                {
        //                    LookupSpecialize myModel = new LookupSpecialize();

        //                    if (!string.IsNullOrEmpty(worksheet.Cells[i, 1].Value?.ToString()))
        //                    {
        //                        // Set the properties of the MyModel object based on the Excel data
        //                        myModel.Name = worksheet.Cells[i, 1].Value?.ToString();
        //                        myModel.ArName = worksheet.Cells[i, 1].Value?.ToString();
        //                        //myModel.CityId = int.Parse(worksheet.Cells[i, 3].Value?.ToString());
        //                        myModel.Type = 1;

        //                        // Add the MyModel object to the list
        //                        importedData.Add(myModel);
        //                    }
        //                    else
        //                    {
        //                        //Null value
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // Validate the imported data using data annotations
        //    List<ValidationResult> validationResults = new List<ValidationResult>();
        //    foreach (LookupSpecialize myModel in importedData)
        //    {
        //        ValidationContext validationContext = new ValidationContext(myModel, null, null);
        //        Validator.TryValidateObject(myModel, validationContext, validationResults, true);
        //    }

        //    // If any validation errors occurred, display them in the view
        //    if (validationResults.Count > 0)
        //    {
        //        foreach (ValidationResult validationResult in validationResults)
        //        {
        //            ModelState.AddModelError("", validationResult.ErrorMessage);
        //        }
        //        return View("ImportExcel");
        //    }

        //    // Save the imported data to the database using Entity Framework
        //    try
        //    {
        //        _context.LookupSpecialize.AddRange(importedData);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any exceptions that occur during the database save operation
        //        ModelState.AddModelError("", "An error occurred while saving the imported data to the database: " + ex.Message);
        //        return View("ImportExcel");
        //    }

        //    // Redirect to the action method that displays the updated data in the view
        //    TempData["success"] = "Specialization Imported";
        //    return RedirectToAction("Index", "Home");
        //}

        //public async Task<IActionResult> ImportFromExcel()
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    // Path to the Excel file on the server
        //    //string serverFilePath = @"\\~\files\jerusalem.xlsx";
        //    string webRootPath = _webHostEnvironment.WebRootPath;
        //    string filePath = Path.Combine(webRootPath, "files", "Jerusalem.xlsx");

        //    // Name of the Excel worksheet where the data is located
        //    string sheetName = "Jerusalem";

        //    // Create a list to hold the imported data
        //    List<Villages> importedData = new List<Villages>();

        //    // Load the Excel data into the list
        //    using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
        //    {
        //        //ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];
        //        foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
        //        {

        //            if (worksheet != null)
        //            {
        //                int totalRows = worksheet.Dimension.Rows;

        //                // Loop through the rows in the Excel data
        //                for (int i = 2; i <= totalRows; i++)
        //                {
        //                    Villages myModel = new Villages();

        //                    if (!string.IsNullOrEmpty(worksheet.Cells[i, 1].Value?.ToString()))
        //                    {
        //                        // Set the properties of the MyModel object based on the Excel data
        //                        myModel.Name = worksheet.Cells[i, 1].Value?.ToString();
        //                        myModel.ArName = worksheet.Cells[i, 2].Value?.ToString();
        //                        //myModel.CityId = int.Parse(worksheet.Cells[i, 3].Value?.ToString());
        //                        myModel.CityId = int.Parse(worksheet.Cells[i, 3].Value?.ToString());

        //                        // Add the MyModel object to the list
        //                        importedData.Add(myModel);
        //                    }
        //                    else
        //                    {
        //                        //Null value
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // Validate the imported data using data annotations
        //    List<ValidationResult> validationResults = new List<ValidationResult>();
        //    foreach (Villages myModel in importedData)
        //    {
        //        ValidationContext validationContext = new ValidationContext(myModel, null, null);
        //        Validator.TryValidateObject(myModel, validationContext, validationResults, true);
        //    }

        //    // If any validation errors occurred, display them in the view
        //    if (validationResults.Count > 0)
        //    {
        //        foreach (ValidationResult validationResult in validationResults)
        //        {
        //            ModelState.AddModelError("", validationResult.ErrorMessage);
        //        }
        //        return View("ImportExcel");
        //    }

        //    // Save the imported data to the database using Entity Framework
        //    try
        //    {
        //        _context.Villages.AddRange(importedData);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any exceptions that occur during the database save operation
        //        ModelState.AddModelError("", "An error occurred while saving the imported data to the database: " + ex.Message);
        //        return View("ImportExcel");
        //    }

        //    // Redirect to the action method that displays the updated data in the view
        //    TempData["success"] = "Villages Imported";
        //    return RedirectToAction("Index", "Home");
        //}
        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View(id);
        }

        public ActionResult Techs()
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
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

        public ActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;
            if (!upload.IsImage())
            {
                var NotImageMessage = "هذه ليست صورة الرجاء اختيار صورة";
                var NotImage = new { uploaded = 0, error = new { message = NotImageMessage } };
                return Json(NotImage);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            Image image = Image.FromStream(upload.OpenReadStream());
            int width = image.Width;
            int height = image.Height;
            if ((width > 1600) || (height > 2000))
            {
                var DimensionErrorMessage = "الصورة بحجم كبير جدا الرجاء اختيار صوره بحجم اصغر";
                var NotImage = new { uploaded = 0, error = new { message = DimensionErrorMessage } };
                return Json(NotImage);
            }

            //if (upload.Length > 500 * 1024)
            //{
            //    var LengthErrorMessage = "الصورة بحجم كبير جدا الرجاء اختيار صورة بحجم اصغر";
            //    var NotImage = new { uploaded = 0, error = new { message = LengthErrorMessage } };
            //    return Json(NotImage);
            //}

            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/files/image/NewsContentImg",
                fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);

            }

            var url = $"{"/files/image/NewsContentImg/"}{fileName}";
            var successMessage = "تم تحميل الصورة بنجاح !";
            //dynamic success = JsonConvert.DeserializeObject("{ 'uploaded': 1,'fileName': \"" + fileName + "\",'url': \"" + url + "\", 'error': { 'message': \"" + successMessage + "\"}}");
            var success = new { uploaded = 1, fileName, url, error = new { message = successMessage } };
            return Json(success);
        }

        public async Task<IActionResult> Uploadfile(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {

            if (upload == null || upload.Length == 0)
                return null;


            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            string extension = System.IO.Path.GetExtension(fileName).Replace(".", "").ToLower();

            var path = "";
            bool allowedType = false;
            String url_dir = "/files/file/pdf/";


            if (extension == "xlsx" || extension == "xltx" || extension == "xls")
            {
                path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/files/file/excel",
                fileName);
                allowedType = true;
                url_dir = "/files/file/excel/";
            }
            else if (extension == "doc" || extension == "docx")
            {
                path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/files/file/word",
                fileName);
                allowedType = true;
                url_dir = "/files/file/word/";
            }

            else if (extension == "pdf")
            {
                path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/files/file/pdf",
                fileName);
                allowedType = true;
                url_dir = "/files/file/pdf/";
            }

            if (allowedType)
            {
                using (FileStream fs = System.IO.File.Create(path))
                {
                    upload.CopyTo(fs);
                    fs.Flush();
                }


                var url = $"{url_dir}{fileName}";
                var successMessage = "تم تحميل الملف بنجاح !";
                var success = new { uploaded = 1, fileName, url, error = new { message = successMessage } };
                return Json(success);
            }

            else
            {
                var LengthErrorMessage = "فشل التحميل غير مسموح بتحميل ملفات من هذا النوع!";
                var Notallowed = new { uploaded = 0, error = new { message = LengthErrorMessage } };
                return Json(Notallowed);
            }

        }
    }
}
