using Microsoft.Ajax.Utilities;
using NiboChallenge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace NiboChallenge.Controllers
{
    public class NiboController : Controller
    {

        public ActionResult Error()
        {
            return View();
        }

        [System.Web.Http.HttpPost]
        public ActionResult ReadOfx()
        {
            String parametro = "";
            bool primeiro = false;
            List<ofxData> lista = new List<ofxData>();
            if (HttpContext.Request.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in HttpContext.Request.Files)
                {
                    var postedFile = HttpContext.Request.Files[file];
                    
                    if (!postedFile.FileName.Contains(".ofx") && postedFile.FileName != "")
                    {
                        return RedirectToAction("Error");
                    }
                    if (postedFile.FileName == "")
                    {
                        if (primeiro)
                        {
                            return RedirectToAction("Error");
                        }

                        primeiro = true;
                    }

                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        postedFile.InputStream.CopyTo(stream);
                        bytes = stream.ToArray();
                    }

                    NiboChallenge.Util.NiboChallenge nibo = new NiboChallenge.Util.NiboChallenge();
                    nibo.ReadOfx(bytes, lista);

                }

                System.Web.UI.WebControls.GridView gView = new System.Web.UI.WebControls.GridView();
                gView.DataSource = lista.ToList();
                gView.DataBind();
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        gView.RenderControl(htw);
                        ViewBag.GridViewString = sw.ToString();
                        ViewBag.totalItens = lista.Count();
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}