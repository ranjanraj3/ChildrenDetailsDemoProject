using ChildrenDetails.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ChildrenDetails.Models.ChildrenDetail;

namespace ChildrenDetails.Controllers
{
    public class ChildrenDetailsController : Controller
    {
        // GET: ChildrenDetails
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Save all the Children Details in Database if record is newely created and Update the children details in database if that records is already exist
        /// </summary>
        /// <param name="objChildrenDetail"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Index(ChildrenDetail objChildrenDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ChildrenDetailsEntities db = new ChildrenDetailsEntities();

                    if (TempData["ID"] != null)
                    {
                        int ID = Convert.ToInt32(TempData["ID"]);
                        ChildrenDetail objChildrenDetails = db.ChildrenDetails.FirstOrDefault(x => x.ID == ID);
                        if (objChildrenDetails != null)
                        {
                            objChildrenDetail.ID = ID;
                            db.ChildrenDetails.AddOrUpdate(objChildrenDetail);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        db.ChildrenDetails.Add(objChildrenDetail);
                        db.SaveChanges();
                    }
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }

                return View(objChildrenDetail);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// This ActionResult is used for populating the data of all Childrens from database and show it on ChildrenLists page and also searched the children details 
        /// based on First Name and Last Name
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public ActionResult ChildrenLists(string searchString)
        {

            try
            {

                HttpPostedFileBase[] files;
                files = null;
                ViewBag.files = files;

                ChildrenDetailsEntities objChildrenDetailsEntities = new ChildrenDetailsEntities();

                var data = from item in objChildrenDetailsEntities.ChildrenDetails
                           //orderby item.ID
                           select item;

                if (!String.IsNullOrEmpty(searchString))
                {
                    data = data.Where(s => s.Child_Last_Name.ToUpper().Contains(searchString.ToUpper())
                                           || s.Child_First_Name.ToUpper().Contains(searchString.ToUpper()));
                }

                return View(data.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Select existing records of children from ChildrenList page to Update in database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                using (ChildrenDetailsEntities db = new ChildrenDetailsEntities())
                {
                    ChildrenDetail model = new ChildrenDetail();
                    model.SelectedCustomer = db.ChildrenDetails.Find(id);
                    model.DisplayMode = "ReadOnly";
                    TempData["ID"] = id;
                    return View("Index", model.SelectedCustomer);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Delete existing records of childredn from ChildrenList page and from database as well
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public ActionResult Delete(int id)
        {
            try
            {
                ChildrenDetailsEntities db = new ChildrenDetailsEntities();
                ChildrenDetail objChildrenDetail = db.ChildrenDetails.Find(id);
                db.ChildrenDetails.Remove(objChildrenDetail);
                db.SaveChanges();
                return RedirectToAction("ChildrenLists");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult UploadFiles()
        {
            HttpPostedFileBase[] files;
            files = null;
            ViewBag.files = files;

            return View();
        }

        /// <summary>
        /// Upload Childrens files on local directory folder
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] files)
        {

            try
            {
                ChildrenDetailsEntities db = new ChildrenDetailsEntities();

                //if (files != null)
                //{
                //    //Ensure model state is valid  
                //    if (ModelState.IsValid)
                //    {   //iterating through multiple file collection   
                //        foreach (HttpPostedFileBase file in files)
                //        {
                //            //Checking file is available to save.  
                //            if (file != null)
                //            {
                //                var InputFileName = Path.GetFileName(file.FileName);
                //                var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                //                //Save file to server folder  
                //                file.SaveAs(ServerSavePath);
                //                //assigning file uploaded status to ViewBag for showing message to user.  
                //                ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                //            }

                //        }
                //    }
                //}

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        // extract only the filename
                        var fileName = Path.GetFileName(file.FileName);

                        // extract the file content to byte array
                        var content = new byte[file.ContentLength];
                        // reads the content from stream
                        file.InputStream.Read(content, 0, file.ContentLength);

                        //get file extesion
                        var fileExtension = Path.GetExtension(fileName);
                        //save file name as uniqe
                        var uniqueFileName = Guid.NewGuid().ToString();

                        ChildrenFileUpload objChildrenFileUpload = new ChildrenFileUpload
                        {
                            File_Name = uniqueFileName,
                            Upload_Date = DateTime.Now,
                            File_Content = content
                        };

                        db.ChildrenFileUploads.Add(objChildrenFileUpload);
                        db.SaveChanges();
                    } 
                }

                return RedirectToAction("ChildrenLists");
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}