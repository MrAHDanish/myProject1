using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Inspinia_MVC5_SeedProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Drawing;
using Inspinia_MVC5_SeedProject.CodeTemplates;
using System.Data.Entity.Validation;
using Amazon.S3;
using System.Drawing;
using Amazon;
using System.IO;
using AngleSharp;
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class AdminController : ApiController
    {
        private Entities db = new Entities();
        public AdminController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {

        }

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        private static readonly string _awsAccessKey =
            ConfigurationManager.AppSettings["AWSAccessKey"];

        private static readonly string _awsSecretKey =
            ConfigurationManager.AppSettings["AWSSecretKey"];

        private static readonly string _bucketName =
            ConfigurationManager.AppSettings["Bucketname"];
        private static readonly string _folderName =
            ConfigurationManager.AppSettings["FolderName"];
        private static readonly string _userFolder =
            ConfigurationManager.AppSettings["UserFolder"];
        //public async Task<IHttpActionResult> runScrapingCode()
        //{
        //    var config =  Configuration.Default.WithDefaultLoader();
        //    // Load the names of all The Big Bang Theory episodes from Wikipedia
        //    var address = "https://en.wikipedia.org/wiki/List_of_The_Big_Bang_Theory_episodes";
        //    // Asynchronously get the document in a new context using the configuration
        //    var document = await BrowsingContext.New(config).OpenAsync(address);
        //    // This CSS selector gets the desired content
        //    var cellSelector = "tr.vevent td:nth-child(3)";
        //    // Perform the query to get all cells with the content
        //    var cells = document.QuerySelectorAll(cellSelector);
        //    // We are only interested in the text - select it with LINQ
        //    var titles = cells.Select(m => m.TextContent);
        //    return Ok("done");
        //}
        [HttpPost]
        public async Task<IHttpActionResult> runscraping()
        {
            var data = db.mobiledatas.Take(40).OrderBy(x=>x.id).Skip(20);
            foreach (var ad in data.ToList())
            {
                try {
                    Ad add = new Ad();
                    add.category = "Mobiles";
                    add.title = ad.title;
                    add.description = ad.description;
                    add.condition = "u";
                    add.isnegotiable = "n";
                    add.name = ad.usr;
                    add.phoneNumber = ad.cell;
                    add.postedBy = User.Identity.GetUserId();
                    if (ad.price != null)
                    {
                        add.price = decimal.Parse(ad.price.Split(' ')[1]);
                    }
                    // add.price = ad.price;
                    add.status = "a";
                    add.subcategory = null;
                    add.time = DateTime.UtcNow;
                    add.type = true;
                    add.views = 0;
                    AdsLocation loc = new AdsLocation();
                    try
                    {
                        db.Ads.Add(add);
                    
                        await db.SaveChangesAsync();
                        ElectronicsController e = new ElectronicsController();
                        //  MobilesTabletsController m = new MobilesTabletsController();
                        // m.SaveMobileBrandModel();
                        var mobileModelId = SaveMobileBrandModel(ad.title);
                        MobileAd mm = new MobileAd();
                        mm.Id = add.Id;
                        if (mobileModelId != null)
                        {
                            mm.mobileId = mobileModelId.id;
                        }
                        db.MobileAds.Add(mm);
                        await db.SaveChangesAsync();
                        if (ad.location != null)
                        {
                            string city = ad.location.Split(',')[0];

                            e.MyAdLocation(city, null, null, add, "Save");
                        }
                        string[] paths = { ad.img1, ad.img2, ad.img3 };
                        fileUpload(paths, add.Id);
                        db.mobiledatas.Remove(ad);
                        await db.SaveChangesAsync();
                    } catch (Exception e)
                    {
                        db.mobiledatas.Remove(ad);
                        await db.SaveChangesAsync();
                        string s = e.ToString();
                        return Ok(s);
                    }
                }catch(Exception e)
                {
                    db.mobiledatas.Remove(ad);
                    await db.SaveChangesAsync();
                    string s = e.ToString();
                    return Ok(s);
                }
                }
            
            return Ok("Done");
        }

        public bool getCategory()
        {
            return true;
        }
        public bool fileUpload(String[] paths, int adId)
        {
            string[] fileNames = null;
            bool canpass = true;
            string filename = "";
            int count = 1;
            foreach (var path in paths)
            {
                if (path != "" && path != null)
                {
                    IAmazonS3 client;
                    System.Drawing.Image imgg = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(@"\Images\others\WaterMark.png"));
                    float f = float.Parse("1"); //0.5
                    System.Drawing.Image img = SetImageOpacity(imgg, f);
                    string localFilename = @"\Images\Ads\temp" + adId + "_" + count + ".jpg";
                    try
                    {
                        using (WebClient client1 = new WebClient())
                        {
                            client1.DownloadFile(path, System.Web.HttpContext.Current.Server.MapPath(localFilename));
                        }

                        //changing extension
                        // path = Path.ChangeExtension(path, ".jpg");
                        //  File.Move(path, Path.ChangeExtension(path, ".jpg"));

                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(localFilename)))
                        using (System.Drawing.Image watermarkImage = img)
                        {
                            using (Graphics imageGraphics = Graphics.FromImage(image))
                            using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
                            {
                                // int x = (image.Width / 2 - watermarkImage.Width / 2);
                                int x = 4;
                                int y = image.Height - watermarkImage.Height - 30;
                                //   int y = (image.Height / 2 - watermarkImage.Height / 2);
                                watermarkBrush.TranslateTransform(x, y);
                                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));


                                //upload to aws
                                filename = @"\Images\Ads\" + adId + "_" + count + ".jpg";
                                var i2 = new Bitmap(image);
     
                                i2.Save(System.Web.HttpContext.Current.Server.MapPath(filename));
                                
                                AmazonS3Config config = new AmazonS3Config();
                                config.ServiceURL = "https://s3.amazonaws.com/";
                                Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

                                var request2 = new Amazon.S3.Model.PutObjectRequest()
                                {
                                    BucketName = _bucketName,
                                    CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                                    Key = _folderName + adId + "_" + count++ + ".jpg",
                                   // InputStream = file.InputStream//SEND THE FILE STREAM
                                    FilePath = System.Web.HttpContext.Current.Server.MapPath(filename)
                                };
                                s3Client.PutObject(request2);
                                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(localFilename)))
                                { 
                                    imageGraphics.Dispose();
                                    watermarkImage.Dispose();
                                    ((Bitmap)i2).Dispose();
                                    image.Dispose();
                                    watermarkImage.Dispose();
                                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(localFilename));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string s = e.ToString();
                    }
                    AdImage adimg = new AdImage();
                    adimg.adId = adId;
                    adimg.imageExtension = ".jpg";
                    db.AdImages.Add(adimg);
                    db.SaveChanges();

                }
            }
            for (int i = 1; i < 4; i++)
            {
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(@"\Images\Ads\" + adId + "_" + i + ".jpg")))
                {
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(@"\Images\Ads\" + adId + "_" + i + ".jpg"));
                }
            }
            return true;
        }
        //i'm using System.Drawing.Image instead of Image because this is already a class in ElectronicsController
        public System.Drawing.Image SetImageOpacity(System.Drawing.Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Ad MyAd(Ad ad, string SaveOrUpdate, string cateogry = null, string subcategory = null)
        {
            ad.name = System.Web.HttpContext.Current.Request["name"];
            ad.phoneNumber = System.Web.HttpContext.Current.Request["phoneNumber"];
            if (ad.status == null || ad.status == "")
            {
                ad.status = "a";
            }
            var type = System.Web.HttpContext.Current.Request["type"];
            var isbiding = System.Web.HttpContext.Current.Request["bidingAllowed"];
            var condition = System.Web.HttpContext.Current.Request["condition"];
            var pp = System.Web.HttpContext.Current.Request["price"];
            string[] prices = pp.Split(',');   //exception: object reference not set to instance of object
            if (type == "sell")
            {
                ad.type = true;
            }
            else
            {
                ad.type = false;
            }
            if (isbiding == "fixedPrice")
            {
                pp = prices[0];
                var nn = System.Web.HttpContext.Current.Request["isNegotiable"];
                if (nn == "on")
                {
                    ad.isnegotiable = "y";
                }
                else
                {
                    ad.isnegotiable = "n";
                }
            }
            else if (isbiding == "allowBiding")
            {
                pp = prices[1];
                ad.isnegotiable = "b";
            }

            if (condition == "new")
            {
                ad.condition = "n";
            }
            else if (condition == "unboxed")
            {
                ad.condition = "b";
            }
            else
            {
                ad.condition = "u";
            }
            if (condition == null)
            {
                ad.condition = "z";
            }
            if (pp != null && pp != "")
            {
                ad.price = decimal.Parse(pp);
            }
            //ad.description = ad.description.Replace("'", "`");
            //  ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
            ad.postedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (SaveOrUpdate == "Save")
            {
                ad.category = cateogry;
                ad.subcategory = subcategory == null ? "" : subcategory;
                ad.time = DateTime.UtcNow;
                db.Ads.Add(ad);

            }
            else if (SaveOrUpdate == "Update")
            {
                ad.time = DateTime.Parse(System.Web.HttpContext.Current.Request["time"]);
                if (ad.category == null)
                {
                    ad.category = System.Web.HttpContext.Current.Request["category"];
                }
                if (ad.subcategory == null)
                {
                    ad.subcategory = System.Web.HttpContext.Current.Request["subcategory"];
                }
                db.Entry(ad).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                string s = e.ToString();
                List<string> errorMessages = new List<string>();
                foreach (DbEntityValidationResult validationResult in e.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
            }
            return ad;
        }
        public string detectMobileBrand(string title)
        {
            string[] ssize = title.Split(new char[0]);
            foreach(var s in ssize)
            {
                if (s.Equals("Iphone", StringComparison.OrdinalIgnoreCase))
                {
                    return "Apple";
                }
            }
            var brand = db.Mobiles.Select(x => x.brand).Intersect(ssize);

            foreach (var br in brand)
            {
                if (br != "")
                {
                    return br;
                }
            }
            //(!tagsArray.Except(ad.AdTags.Select(x => x.Tag.name)).Any());
            return "";
        }
        public IdStatus SaveMobileBrandModel(string title)
        {
            try {
                string adStatus = "a";
                var company = detectMobileBrand(title);
                var model = "";
                if (company != null && company != "")
                {
                    company = company.Trim();
                    model = model.Trim();
                }
                if (true) //company != null
                {
                    bool isOldBrand = db.Mobiles.Any(x => x.brand.Equals(company));

                    if (!isOldBrand)
                    {
                        if (company != null && company != "" && company != "undefined")
                        {
                            adStatus = "p";
                            Mobile mob = new Mobile();
                            mob.brand = company;
                            mob.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            mob.time = DateTime.UtcNow;
                            mob.status = "p";
                            db.Mobiles.Add(mob);
                            db.SaveChanges();
                            if (model != null && model != "" && model != "undefined")
                            {
                                MobileModel mod = new MobileModel();
                                mod.model = model;
                                mod.brandId = mob.Id;
                                mod.time = DateTime.UtcNow;
                                mod.status = "p";
                                mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                                db.MobileModels.Add(mod);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        bool isOldModel = db.MobileModels.Any(x => x.model.Equals(model));
                        if (!isOldModel)
                        {
                            if (model != null && model != "undefined")
                            {
                                adStatus = "p";
                                var brandId = db.Mobiles.FirstOrDefault(x => x.brand.Equals(company));
                                MobileModel mod = new MobileModel();
                                mod.brandId = brandId.Id;
                                mod.model = model;
                                mod.status = "p";
                                mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                                mod.time = DateTime.UtcNow;
                                db.MobileModels.Add(mod);
                                db.SaveChanges();
                            }
                        }
                        else if (model == "")
                        {
                            adStatus = "a";
                            var brandId = db.Mobiles.FirstOrDefault(x => x.brand.Equals(company));
                            MobileModel mod = new MobileModel();
                            mod.brandId = brandId.Id;
                            mod.model = model;
                            mod.status = "a";
                            mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            mod.time = DateTime.UtcNow;
                            db.MobileModels.Add(mod);
                            db.SaveChanges();
                        }
                    }
                    var mobileModel = db.MobileModels.FirstOrDefault(x => x.Mobile.brand == company && x.model == model);
                    if (model == null)
                    {
                        mobileModel = db.MobileModels.FirstOrDefault(x => x.Mobile.brand.Equals(company));
                    }
                    IdStatus idstatus = new IdStatus();
                    if (mobileModel != null)
                    {
                        idstatus.id = mobileModel.Id;
                        idstatus.status = adStatus;
                    }
                    else
                    {
                        idstatus = null;
                    }
                    return idstatus;
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }
        public async Task<bool> isAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    return true;
                }
            }
            return false;
        }
        [HttpPost]
        public async Task<IHttpActionResult> MakeAdmin(string email)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var makeAdmin = await db.AspNetUsers.FirstOrDefaultAsync(x => x.UserName.Equals(email));
                    if (makeAdmin != null)
                    {
                        if (makeAdmin.status != "admin")
                        {
                            makeAdmin.status = "admin";
                        }
                        else
                        {
                            makeAdmin.status = "active";
                        }
                        await db.SaveChangesAsync();
                        return Ok("Done");
                    }
                    return NotFound();
                }
            }
            return BadRequest();
        }

        public async Task<IHttpActionResult> PostScrapingAds()
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IHttpActionResult> SendEmail()
        {
            ElectronicsController.sendEmail("irfanyusanif@gmail.com", "just for test review", "hi are you there");
            return Ok("Done");
        }
        [HttpPost]
        public bool deleteImage(string path)
        {
                if (path != "" && path != null)
                {
                    IAmazonS3 client;
                   // System.Drawing.Image imgg = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(@"\Images\others\WaterMark.png"));
                  //  float f = float.Parse("1"); //0.5
                  //  System.Drawing.Image img = SetImageOpacity(imgg, f);
                 //   string localFilename = @"\Images\Ads\temp" + adId + "_" + count + ".jpg";
                    try
                    {
                        
                        AmazonS3Config config = new AmazonS3Config();
                        config.ServiceURL = "https://s3.amazonaws.com/";
                        Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

                        var request2 = new Amazon.S3.Model.DeleteObjectRequest()
                        {
                            BucketName = _bucketName,
                         //   CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                            Key = _folderName +path,
                            // InputStream = file.InputStream//SEND THE FILE STREAM
                         //   FilePath = System.Web.HttpContext.Current.Server.MapPath(filename)
                        };
                        s3Client.DeleteObjectAsync(request2);
                                
                    }
                    catch (Exception e)
                    {
                        string s = e.ToString();
                    }
                    AdImage adimg = new AdImage();
                //  adimg.adId = path;
                string adId = path.Split('_')[0];
                int adkiId = int.Parse(adId);
                  //  var adid = db.AdImages.Where(x => x.adId == adkiId);
                var adid = db.AdImages.Where(x=>x.adId.Equals(adkiId)).FirstOrDefault();
                db.AdImages.Remove(adid);
                    db.SaveChanges();

                }
            return true;
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetAllAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var ret = from admin in db.AspNetUsers
                              where admin.status.Equals("admin")
                              select new
                              {
                                  id = admin.Id,
                                  name = admin.Email
                              };
                    return Ok(ret);

                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> BlockUser(string id) //block on the basis of id
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var makeAdmin = await db.AspNetUsers.FirstOrDefaultAsync(x => x.Id.Equals(id));
                    if (makeAdmin != null)
                    {
                        makeAdmin.status = "blocked";
                        await db.SaveChangesAsync();
                        await UserManager.UpdateSecurityStampAsync(makeAdmin.Id);
                        await UserManager.SetLockoutEnabledAsync(makeAdmin.Id, true);
                        await UserManager.SetLockoutEndDateAsync(makeAdmin.Id, DateTime.Today.AddYears(10));
                        return Ok("Done");

                    }

                    return NotFound();
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> BlockUserEmail(string email) //block on the basis of email
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var makeAdmin = await db.AspNetUsers.FirstOrDefaultAsync(x => x.UserName.Equals(email));
                    if (makeAdmin != null)
                    {
                        makeAdmin.status = "blocked";
                        await db.SaveChangesAsync();
                        await UserManager.UpdateSecurityStampAsync(makeAdmin.Id);
                        await UserManager.SetLockoutEnabledAsync(makeAdmin.Id, true);
                        await UserManager.SetLockoutEndDateAsync(makeAdmin.Id, DateTime.Today.AddYears(10));
                        return Ok("Done");

                    }

                    return NotFound();
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> openUser(string email) //block on the basis of email
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var id = db.AspNetUsers.FirstOrDefault(x => x.UserName.Equals(email)).Id;
                    return Ok(id);
                    return NotFound();
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<bool> addNewBrandModel(string brand, string model, string category)
        {
            if (brand != null)
            {
                brand.Trim();
            }
            if (model != null)
            {
                model.Trim();
            }
            if (brand == "" || brand == "undefined" || brand == null)
            {
                return true;
            }
            if (category == "Mobiles")
            {
                var isNew = db.Mobiles.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    Mobile mob = new Mobile();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.Mobiles.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        MobileModel mod = new MobileModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.MobileModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.MobileModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            var brandId = db.Mobiles.First(x => x.brand.Equals(brand));
                            MobileModel mod = new MobileModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = brandId.Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.MobileModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            else if (category == "Laptops")
            {
                var isNew = db.LaptopBrands.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    LaptopBrand mob = new LaptopBrand();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.LaptopBrands.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        LaptopModel mod = new LaptopModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.LaptopModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.LaptopModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            LaptopModel mod = new LaptopModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = db.LaptopBrands.First(x => x.brand.Equals(brand)).Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.LaptopModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            else if (category == "Bikes")
            {
                var isNew = db.BikeBrands.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    BikeBrand mob = new BikeBrand();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.BikeBrands.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        BikeModel mod = new BikeModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.BikeModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.BikeModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            BikeModel mod = new BikeModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = db.BikeBrands.First(x => x.brand.Equals(brand)).Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.BikeModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            else if (category == "Cars")
            {
                var isNew = db.CarBrands.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    CarBrand mob = new CarBrand();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.CarBrands.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        CarModel mod = new CarModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.CarModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.CarModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            CarModel mod = new CarModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = db.CarBrands.First(x => x.brand.Equals(brand)).Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.CarModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            return true;
        }
        public async Task<IHttpActionResult> GetFeedbacks()
        {
            var ret = from feed in db.Feedbacks
                      orderby feed.time descending
                      select new
                      {
                          id = feed.Id,
                          type = feed.type,
                          description = feed.description,
                          time = feed.time,
                          givenById = feed.givenBy,
                          givenByName = feed.AspNetUser.Email,
                      };
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteFeedback(int id)
        {
            var feedback = await db.Feedbacks.FindAsync(id);
            db.Feedbacks.Remove(feedback);
            await db.SaveChangesAsync();
            return Ok("Done");
        }
        public async Task<IHttpActionResult> GetItems(int limit)
        {
            // await AdViews(id);
            string islogin = "";
            string loginUserProfileExtension = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
                var ide = await db.AspNetUsers.FindAsync(islogin);
                loginUserProfileExtension = ide.dpExtension;
            }
            var ret = ((from ad in db.Ads
                        where ad.status != "a" || ad.Reporteds.Count > 0
                        orderby ad.time descending
                        select new
                        {
                            title = ad.title,
                            postedById = ad.AspNetUser.Id,
                            postedByName = ad.AspNetUser.Email,
                            description = ad.description,
                            id = ad.Id,
                            time = ad.time,
                            status = ad.status,
                            islogin = islogin,
                            loginUserProfileExtension = loginUserProfileExtension,
                            isNegotiable = ad.isnegotiable,
                            price = ad.price,
                            reportedCount = ad.Reporteds.Count,
                            isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                            //views = ad.AdViews.Count,
                            views = ad.views,
                            condition = ad.condition,
                            type = ad.type,
                            isSaved = ad.SaveAds.Any(x => x.savedBy == islogin),
                            savedCount = ad.SaveAds.Count,
                            mobilead = new
                            {
                                color = ad.MobileAd.color,
                                sims = ad.MobileAd.sims,
                                brand = ad.MobileAd.MobileModel.Mobile.brand,
                                model = ad.MobileAd.MobileModel.model
                            },
                            laptopad = new
                            {
                                color = ad.LaptopAd.color,
                                brand = ad.LaptopAd.LaptopModel.LaptopBrand.brand,
                                model = ad.LaptopAd.LaptopModel.model,
                            },
                            location = new
                            {
                                cityName = ad.AdsLocation.City.cityName,
                                cityId = ad.AdsLocation.cityId,
                                popularPlaceId = ad.AdsLocation.popularPlaceId,
                                popularPlace = ad.AdsLocation.popularPlace.name,
                                exectLocation = ad.AdsLocation.exectLocation,
                            },
                            adTags = from tag in ad.AdTags.ToList()
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                         followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                         //info = tag.Tag.info,
                                     },
                            bid = from biding in ad.Bids.ToList()
                                  select new
                                  {
                                      postedByName = biding.AspNetUser.Email,
                                      postedById = biding.AspNetUser.Id,
                                      price = biding.price,
                                      time = biding.time,
                                      id = biding.Id,
                                  },
                            comment = from comment in ad.Comments.ToList()
                                      orderby comment.time
                                      select new
                                      {
                                          description = comment.description,
                                          postedById = comment.postedBy,
                                          postedByName = comment.AspNetUser.Email,
                                          imageExtension = comment.AspNetUser.dpExtension,
                                          time = comment.time,
                                          id = comment.Id,
                                          adId = comment.adId,
                                          islogin = islogin,
                                          loginUserProfileExtension = loginUserProfileExtension,
                                          voteUpCount = comment.CommentVotes.Where(x => x.isup == true).Count(),
                                          voteDownCount = comment.CommentVotes.Where(x => x.isup == false).Count(),
                                          isUp = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup),
                                          isDown = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup == false),
                                          commentReply = from commentreply in comment.CommentReplies.ToList()
                                                         orderby commentreply.time
                                                         select new
                                                         {
                                                             id = commentreply.Id,
                                                             description = commentreply.description,
                                                             postedById = commentreply.postedBy,
                                                             postedByName = commentreply.AspNetUser.Email,
                                                             imageExtension = commentreply.AspNetUser.dpExtension,
                                                             loginUserProfileExtension = loginUserProfileExtension,
                                                             time = commentreply.time,
                                                             voteUpCount = commentreply.CommentReplyVotes.Where(x => x.isup == true).Count(),
                                                             voteDownCount = commentreply.CommentReplyVotes.Where(x => x.isup == false).Count(),
                                                             isUp = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup),
                                                             isDown = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup == false)
                                                         }
                                      }
                        }).Take(limit)).AsEnumerable();
            return Ok(ret);
        }
        public async Task<IHttpActionResult> GetLatestItems(int limit)
        {
            // await AdViews(id);
            string islogin = "";
            string loginUserProfileExtension = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
                var ide = await db.AspNetUsers.FindAsync(islogin);
                loginUserProfileExtension = ide.dpExtension;
            }
            var ret = ((from ad in db.Ads
                        orderby ad.time descending
                        select new
                        {
                            title = ad.title,
                            postedById = ad.AspNetUser.Id,
                            postedByName = ad.AspNetUser.Email,
                            description = ad.description,
                            id = ad.Id,
                            time = ad.time,
                            status = ad.status,
                            islogin = islogin,
                            loginUserProfileExtension = loginUserProfileExtension,
                            isNegotiable = ad.isnegotiable,
                            price = ad.price,
                            reportedCount = ad.Reporteds.Count,
                            isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                            //views = ad.AdViews.Count,
                            views = ad.views,
                            condition = ad.condition,
                            type = ad.type,
                            isSaved = ad.SaveAds.Any(x => x.savedBy == islogin),
                            savedCount = ad.SaveAds.Count,
                            mobilead = new
                            {
                                color = ad.MobileAd.color,
                                sims = ad.MobileAd.sims,
                                brand = ad.MobileAd.MobileModel.Mobile.brand,
                                model = ad.MobileAd.MobileModel.model
                            },
                            laptopad = new
                            {
                                color = ad.LaptopAd.color,
                                brand = ad.LaptopAd.LaptopModel.LaptopBrand.brand,
                                model = ad.LaptopAd.LaptopModel.model,
                            },
                            location = new
                            {
                                cityName = ad.AdsLocation.City.cityName,
                                cityId = ad.AdsLocation.cityId,
                                popularPlaceId = ad.AdsLocation.popularPlaceId,
                                popularPlace = ad.AdsLocation.popularPlace.name,
                                exectLocation = ad.AdsLocation.exectLocation,
                            },
                            adTags = from tag in ad.AdTags.ToList()
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                         followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                         //info = tag.Tag.info,
                                     },
                            bid = from biding in ad.Bids.ToList()
                                  select new
                                  {
                                      postedByName = biding.AspNetUser.Email,
                                      postedById = biding.AspNetUser.Id,
                                      price = biding.price,
                                      time = biding.time,
                                      id = biding.Id,
                                  },
                            comment = from comment in ad.Comments.ToList()
                                      orderby comment.time
                                      select new
                                      {
                                          description = comment.description,
                                          postedById = comment.postedBy,
                                          postedByName = comment.AspNetUser.Email,
                                          imageExtension = comment.AspNetUser.dpExtension,
                                          time = comment.time,
                                          id = comment.Id,
                                          adId = comment.adId,
                                          islogin = islogin,
                                          loginUserProfileExtension = loginUserProfileExtension,
                                          voteUpCount = comment.CommentVotes.Where(x => x.isup == true).Count(),
                                          voteDownCount = comment.CommentVotes.Where(x => x.isup == false).Count(),
                                          isUp = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup),
                                          isDown = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup == false),
                                          commentReply = from commentreply in comment.CommentReplies.ToList()
                                                         orderby commentreply.time
                                                         select new
                                                         {
                                                             id = commentreply.Id,
                                                             description = commentreply.description,
                                                             postedById = commentreply.postedBy,
                                                             postedByName = commentreply.AspNetUser.Email,
                                                             imageExtension = commentreply.AspNetUser.dpExtension,
                                                             loginUserProfileExtension = loginUserProfileExtension,
                                                             time = commentreply.time,
                                                             voteUpCount = commentreply.CommentReplyVotes.Where(x => x.isup == true).Count(),
                                                             voteDownCount = commentreply.CommentReplyVotes.Where(x => x.isup == false).Count(),
                                                             isUp = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup),
                                                             isDown = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup == false)
                                                         }
                                      }
                        }).Take(limit)).AsEnumerable();
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveAd(int id)
        {
            Ad mobile = await db.Ads.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveMobileBrand(int id)
        {
            Mobile mobile = await db.Mobiles.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveMobileModel(int id)
        {
            MobileModel mobile = await db.MobileModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveLaptopBrand(int id)
        {
            LaptopBrand mobile = await db.LaptopBrands.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveCarBrand(int id)
        {
            CarBrand mobile = await db.CarBrands.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveLaptopModel(int id)
        {
            LaptopModel mobile = await db.LaptopModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveCarModel(int id)
        {
            CarModel mobile = await db.CarModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveBikeBrand(int id)
        {
            BikeBrand mobile = await db.BikeBrands.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveBikeModel(int id)
        {
            BikeModel mobile = await db.BikeModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }

        // PUT api/Admin/5
        public async Task<IHttpActionResult> PutMobile(int id, Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mobile.Id)
            {
                return BadRequest();
            }

            db.Entry(mobile).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MobileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Admin
        [ResponseType(typeof(Mobile))]
        public async Task<IHttpActionResult> PostMobile(Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Mobiles.Add(mobile);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = mobile.Id }, mobile);
        }

        // DELETE api/Admin/5
        [ResponseType(typeof(Mobile))]
        public async Task<IHttpActionResult> DeleteMobile(int id)
        {
            Mobile mobile = await db.Mobiles.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }

            db.Mobiles.Remove(mobile);
            await db.SaveChangesAsync();

            return Ok(mobile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MobileExists(int id)
        {
            return db.Mobiles.Count(e => e.Id == id) > 0;
        }
    }
}