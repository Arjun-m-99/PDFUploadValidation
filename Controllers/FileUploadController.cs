using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;

namespace PDFUpload.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly string wwwrootDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadedPdfs");

        public IActionResult Index()
        {
            Console.WriteLine("From Index");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile uploadedFile)
        {
            //To Create directory if directory is not found
            if (!Directory.Exists(wwwrootDirectory))
            {
                Directory.CreateDirectory(wwwrootDirectory);
            }

            if ((Path.GetExtension(uploadedFile.FileName) == ".pdf"))
            {
                if (uploadedFile != null)
                {
                    string s = checkFileType(uploadedFile);
                    var path = Path.Combine(
                        wwwrootDirectory,
                        //uploadedFile.FileName + Path.GetExtension(uploadedFile.FileName) // Save file name+extention
                        uploadedFile.FileName //Save only file name
                        );

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(stream);
                    }



                    ViewBag.Message = uploadedFile.FileName + " Uploded " + s;
                    //return View();
                }
            }
            else
            {
                ViewBag.Message = "Allowed only PDF but given file is "+ Path.GetExtension(uploadedFile.FileName);
            }
            if (uploadedFile == null)
            {
                ViewBag.Message = "Choose File to upload";
            }
            return View("Index");
        }

        private string checkFileType(IFormFile FileName)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(FileName.FileName, out contentType);
            // return contentType ?? "application/octet-stream";
            return contentType;
        }
    }
}
