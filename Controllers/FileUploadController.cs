using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;

namespace PDFUpload.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly string wwwrootDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadedPdfs");

        //public IActionResult Index()
        //{
        //    Console.WriteLine("From Index");
        //    System.Diagnostics.Debug.WriteLine("Index");
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile uploadedFile)
        {
            ViewData.Clear();
            //To Create directory if directory is not found
            if (!Directory.Exists(wwwrootDirectory))
            {
                Directory.CreateDirectory(wwwrootDirectory);
            }

            if (uploadedFile == null)
            {
                ViewBag.Message = "Choose File to upload";
                // to get msg in console
                System.Diagnostics.Debug.WriteLine("File need to upload");
            }
            else
            {
                var checkThisPath = wwwrootDirectory + "/" + uploadedFile.FileName;
                FileInfo file = new FileInfo(checkThisPath);
                System.Diagnostics.Debug.WriteLine("Check File Existes: " + file.Exists);

                if (!file.Exists)
                {
                    //To check the file extention
                    if (Path.GetExtension(uploadedFile.FileName) == ".pdf")
                    {

                        // To get msg in console for validating file type
                        System.Diagnostics.Debug.WriteLine("File extention is " + (Path.GetExtension(uploadedFile.FileName) == ".pdf"));

                        //To check file type (Just for ref)
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

                        ViewBag.Message = uploadedFile.FileName + " Uploaded " + s;

                        //return console
                        System.Diagnostics.Debug.WriteLine("Uploaded");
                        //TO valid PDF
                        try
                        {
                            checkPages(uploadedFile);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                            System.Diagnostics.Debug.WriteLine("Not a Valid PDF");
                            string checkError = e + "";
                            if (checkError.Contains("InvalidPdfException"))
                            {
                                ViewBag.Message = "Not A valid PDF. Try Valid PDF";
                                file.Delete();

                            }
                        }
                    }
                    else
                    {
                        ViewBag.Message = "allowed only pdf but given file is " + Path.GetExtension(uploadedFile.FileName);
                        // To get console message
                        System.Diagnostics.Debug.WriteLine("Uploaded file is " + Path.GetExtension(uploadedFile.FileName) + " Not Allowed");
                    }
                }
                else
                {
                    ViewBag.Message = "File alredy Exists. Try another one.";
                    System.Diagnostics.Debug.WriteLine("File already Present");
                }
            }

            return View("Views/Home/Index.cshtml");
        }

        //To verify the uploaded file is pdf or not by readinng the pages
        private void checkPages(IFormFile uploadedFile)
        {

            string pdfPath = wwwrootDirectory + "/" + uploadedFile.FileName;
            PdfReader pdfReader = new PdfReader(pdfPath);
            int numberOfPages = pdfReader.NumberOfPages;
            System.Diagnostics.Debug.WriteLine(numberOfPages);

        }

        // returns file content type
        private string checkFileType(IFormFile FileName)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(FileName.FileName, out contentType);
            // return contentType ?? "application/octet-stream";
            return contentType;
        }
    }
}
