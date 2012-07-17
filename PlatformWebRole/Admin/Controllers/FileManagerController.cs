using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Admin.Models;
using Newtonsoft.Json;
using System.IO;
using System.Web.Script.Serialization;

namespace Admin.Controllers {
    public class FileManagerController : BaseController {

        private string[] hiddenContainers = { "vsdeploy", "wad-control-container" };

        public ActionResult Index(string name = "") {

            DiscountBlobContainer cont = new DiscountBlobContainer();
            List<IListBlobItem> blobs = new List<IListBlobItem>();
            if (name != "") {
                if (name.Contains("/")) {
                    cont = BlobManagement.GetFolder(name);
                    blobs = BlobManagement.GetDirectoryBlobs(name);
                } else {
                    cont = BlobManagement.GetContainer(name);
                    blobs = BlobManagement.GetBlobs(name);
                }
            } else {
                cont = new DiscountBlobContainer {
                    BlobCount = 0,
                    Container = null,
                    parent = null,
                    uri = null,
                    SubContainers = BlobManagement.GetContainers().Where(x => !hiddenContainers.Contains(x.Container.Name)).ToList<DiscountBlobContainer>()
                };
            }
            string[] folders = name.Split('/');
            List<string> usernames = Profiles.GetAllUsernames();
            List<IListBlobItem> files = new List<IListBlobItem>();
            foreach (IListBlobItem blob in blobs) {
                if (!blob.GetType().Equals(typeof(CloudBlobDirectory))) {
                    files.Add(blob);
                }
            }
            ViewBag.container = cont;
            ViewBag.files = files;
            ViewBag.folders = folders;
            ViewBag.usernames = usernames;

            return View();
        }

        [NoValidation]
        public ActionResult FileBrowser(string name = "", string CKEditor = "", string CKEditorFuncNum = "", string langCode = "") {
            DiscountBlobContainer cont = new DiscountBlobContainer();
            List<IListBlobItem> blobs = new List<IListBlobItem>();
            if (name != "") {
                if (name.Contains("/")) {
                    cont = BlobManagement.GetFolder(name);
                    blobs = BlobManagement.GetDirectoryBlobs(name);
                } else {
                    cont = BlobManagement.GetContainer(name);
                    blobs = BlobManagement.GetBlobs(name);
                }
            } else {
                cont = new DiscountBlobContainer {
                    BlobCount = 0,
                    Container = null,
                    parent = null,
                    uri = null,
                    SubContainers = BlobManagement.GetContainers().Where(x => !hiddenContainers.Contains(x.Container.Name)).ToList<DiscountBlobContainer>()
                };
            }
            string[] folders = name.Split('/');
            List<string> usernames = Profiles.GetAllUsernames();
            List<IListBlobItem> files = new List<IListBlobItem>();
            foreach (IListBlobItem blob in blobs) {
                if (!blob.GetType().Equals(typeof(CloudBlobDirectory))) {
                    files.Add(blob);
                }
            }
            ViewBag.container = cont;
            ViewBag.files = files;
            ViewBag.folders = folders;
            ViewBag.usernames = usernames;

            ViewBag.CKEditor = CKEditor;
            ViewBag.CKEditorFuncNum = CKEditorFuncNum;
            ViewBag.langCode = langCode;

            return View();
        }

        /* AJAXified mother fucker */

        public string GetContainers() {
            try {
                List<DiscountBlobContainer> cons = BlobManagement.GetContainers();
                cons = cons.Where(x => !hiddenContainers.Contains(x.Container.Name)).ToList<DiscountBlobContainer>();
                return JsonConvert.SerializeObject(cons);
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string CreateContainer(string name = "", string parent = "", bool make_public = true) {
            try {
                DiscountBlobContainer con = BlobManagement.CreateContainer(name, parent, true);
                return JsonConvert.SerializeObject(con);
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteContainer(string name = "") {
            try {
                BlobManagement.DeleteContainer(name);
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string RenameContainer(string old_name = "", string new_name = "") {
            try {
                throw new Exception("Temporarily Disabled.");
                //return JsonConvert.SerializeObject(BlobManagement.RenameContainer(old_name, new_name));
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string Upload(string container = "") {
            try {
                string filename = HttpContext.Request.Headers["X-File-Name"];
                Stream input = Request.InputStream;
                CloudBlob blob = BlobManagement.CreateBlob(container, filename, input);

                return blob.Uri.ToString();
            } catch (Exception e) {
                return e.Message + e.Source;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteBlob(string uri = "") {
            try {
                BlobManagement.DeleteBlob(uri);
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

        [NoValidation]
        public string AjaxFileBrowser(string name = "") {

            DiscountBlobContainer cont = new DiscountBlobContainer();
            List<IListBlobItem> blobs = new List<IListBlobItem>();
            if (name != "") {
                if (name.Contains("/")) {
                    cont = BlobManagement.GetFolderForSerialization(name);
                    blobs = BlobManagement.GetDirectoryBlobs(name);
                } else {
                    cont = BlobManagement.GetContainer(name);
                    blobs = BlobManagement.GetBlobs(name);
                }
            } else {
                cont = new DiscountBlobContainer {
                    BlobCount = 0,
                    Container = null,
                    parent = null,
                    uri = null,
                    SubContainers = BlobManagement.GetContainers().Where(x => !hiddenContainers.Contains(x.Container.Name)).ToList<DiscountBlobContainer>()
                };
            }
            List<BlobFile> files = new List<BlobFile>();
            foreach (IListBlobItem blob in blobs) {
                if (!blob.GetType().Equals(typeof(CloudBlobDirectory))) {
                    BlobFile bf = new BlobFile {
                        uri = blob.Uri
                    };
                    files.Add(bf);
                }
            }
            cont.files = files;

            return JsonConvert.SerializeObject(cont);
        }
        
        [NoValidation]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CKUpload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode) {
            string message = "";
            string url = "";
            try {
                string filename = upload.FileName;
                Stream input = upload.InputStream;
                CloudBlob blob = BlobManagement.CreateBlob("miscellaneous", filename, input);
                url = blob.Uri.ToString();
                message = "File Uploaded";
            } catch (Exception e) {
                message = String.Format("Failed to upload image: {0}", e.Message);
            }
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\",\"" + message + "\");</script></body></html>";
            return Content(output);
        }

    }
}
