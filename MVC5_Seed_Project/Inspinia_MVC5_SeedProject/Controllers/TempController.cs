using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5_SeedProject.Models;
using ImageMagick;
using System.IO;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class TempController : Controller
    {
        public Entities db = new Entities();
       
        public bool ImageCompression()
        {




            // Read from file.
            using (MagickImage image = new MagickImage("Snakeware.jpg"))
            {
                FileInfo snakewareLogo = new FileInfo("Snakeware.jpg");
                ImageOptimizer optimizer = new ImageOptimizer();
                optimizer.LosslessCompress(snakewareLogo);
               // snakewareLogo
            }
            return true;
        }
        public string imageCompression1()    //Done
        {
            using (MagickImage sprite = new MagickImage(@"C:\Users\Irfan\Desktop\test.jpg"))
            {
                var width = sprite.Width;
                var height = sprite.Height;
                sprite.Format = MagickFormat.Jpeg;
                sprite.Quality = 80;
                sprite.Resize(width, height);
                sprite.Write(@"C:\Users\Irfan\Desktop\test1.jpg");
            }
            return "Done";
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
