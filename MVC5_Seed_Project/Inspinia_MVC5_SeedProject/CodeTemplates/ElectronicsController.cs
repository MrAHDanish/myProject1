using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
using System.Text;
using Newtonsoft.Json;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.IO;
using System.Xml;
using System.Data.Entity.Validation;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using ImageMagick;
using System.Text.RegularExpressions;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class FileName
    {
        public string fileName;
        public string fileId;
    }
    public class IdStatus
    {
        public int id;
        public string status;
    }
    
    public class ElectronicsController : Controller
    {
       
        private Entities db = new Entities();
        //public string subdomainName
        //{
        //    get
        //    {
        //        string s = Request.Url.Host;
        //        var index = s.IndexOf(".");

        //        if (index < 0)
        //        {
        //            return null;
        //        }
        //        var sub = s.Split('.')[0];
        //        if (sub == "www" || sub == "localhsot")
        //        {
        //            return null;
        //        }
        //        return sub;
        //    }
        //}
        // GET: /Electronics/
        //public ActionResult Index()
        //{
        //   /// var ads = db.Ads.Include(a => a.AspNetUser);
        //    return View();
        //}
        public static bool checkCategory(string category, string subcategory = null)
        {
            if (category == "Education-Learning")
            {
                //if (subcategory == "Courses & classes" || subcategory == "Books & Study Material")
                //{
                //    return true;
                //}
                return true;
            }
            if(category == "Services")
            {
                return true;
            }
            if(category == "Fashion")
            {
                return true;
            }
            if (category == "Vehicles")
            {
                if (subcategory == "commercial-vehicles" || subcategory == "vehicles-for-rent" || subcategory == "other-vehicles" || subcategory == "spare-parts")
                {
                    return true;
                }
            }
            else if (category == "RealEstate")
            {
                string[] subcategories = { "apartment", "house", "plot & land", "Shop", "Office", "PG & Flatmates", "other commercial places" }; //reference over Realestate/create page + RealEstateSearch.js
                foreach (var subcat in subcategories)
                {
                    if (subcategory == subcat)
                    {
                        return true;
                    }
                }
            }
            else if (category == "Electronics")
            {
                string[] subcategories = { "TV - Video - Audio","Camera", "Games", "Home-Appliances", "Other-Electronics" };
                foreach (var subcat in subcategories)
                {
                    if (subcategory == subcat)
                    {
                        return true;
                    }
                }
            }
            else if (category == "Animals")
            {
                return true;
            }
            return false;
        }
        public static void sendEmail(string to,string subject, string body){
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("dealkar.pk@gmail.com");
           // mail.From = new System.Net.Mail.MailAddress("notification@dealkar.pk");
            // The important part -- configuring the SMTP client
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;   // [1] You can try with 465 also, I always used 587 and got success 587
             smtp.EnableSsl = true; //commented for godaddy
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // [2] Added this
            smtp.UseDefaultCredentials = false; // [3] Changed this
            smtp.Credentials = new NetworkCredential("dealkar.pk@gmail.com", "birthday2Wishirfan");  // [4] Added this. Note, first parameter is NOT string.
                                                // smtp.Credentials = new NetworkCredential("notification@dealkar.pk", "birthdaywish");
             smtp.Host = "smtp.gmail.com";
           // smtp.Host = "relay-hosting.secureserver.net";
            //recipient address
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            //Formatted mail body
            mail.IsBodyHtml = true;
            // string st = "Test";

            mail.Body = body;
            smtp.Send(mail);
            //try
            //{
            //    smtp.Send(mail);
            //}
            //catch (Exception e)
            //{
            //    string s = e.ToString(); 
            //}
        }

        [Route("Laptops-Computers",Name ="Laptops-Computers")]
        public async Task<ActionResult> ComputersLaptops(string c, string brand, string model, string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = c;
            if (c == null || c == "")
            {
                ViewBag.category = "Laptops-Computers";
            }else if(c == "Other Accessories")
            {
                ViewBag.category = "Laptops-Computers Accessories";
            }

            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await searchLaptops("Laptops-Computers", c, brand, model, q, tags, minPrice, maxPrice, city, null, 50000, accessories, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Laptops",viewModel);
        }
         [Route("Cameras",Name ="Cameras")]
        public async Task<ActionResult> Cameras(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000,  string condition = null, int? page = null)
        {
            ViewBag.category = "Cameras";
            ViewBag.subcategories = "";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Electronics", "Cameras",q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }
        public async Task<List<ListAdView>> searchLaptops(string category, string subcategory, string brand, string model, string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, string city = null, string pp = null, int fixedMaxPrice = 10000, bool isAccessories = false, string newOrUsed = null)
        {
            if (tags != null && tags != "")
            {
                q = q + " " + tags;
            }
            if (model != null && model != "")
            {
                q = q + " " + model;
            }
            if(brand != null && brand != "")
            {
                q = q + " " + brand;
            }
            q = Regex.Replace(q, @"\b(" + string.Join("|", "sale") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "is") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "for") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "i") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "a") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "an") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "want") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "buy") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "sell") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "who") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "and") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "or") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "are") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "you") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "me") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            q = Regex.Replace(q, @"\b(" + string.Join("|", "my") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            string[] q_array = q.Split(' ');
          //  if (tags == null || tags == "")
            {
                List<ListAdView> temp = await (from ad in db.LaptopAds
                                               where ((q == null || q == "" || q_array.Any(x => ad.Ad.title.Contains(x))) && ( category == null || category == "" || ad.Ad.subcategory.Equals(category)) && ( subcategory == null || subcategory == "null" || subcategory == "" || ad.Ad.subsubcategory.Equals(subcategory)) && (newOrUsed == null || newOrUsed == "" || newOrUsed == ad.Ad.condition) && ad.Ad.status.Equals("a") && (model == null || model == "" || model == "undefined" || /*ad.LaptopModel.model.Equals(model)*/ true) && (brand == null || brand == "" || brand == "undefined" || /*ad.LaptopModel.LaptopBrand.brand.Equals(brand)*/ true) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "" || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "" || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
                                               orderby ad.Ad.time descending
                                               select new ListAdView
                                               {
                                                   title = ad.Ad.title,
                                                   description = ad.Ad.description,
                                                   Id = ad.Ad.Id,
                                                   time = ad.Ad.time,
                                                   category = ad.Ad.category,
                                                   subcategory = ad.LaptopModel.LaptopBrand.brand,
                                                   isnegotiable = ad.Ad.isnegotiable,
                                                   price = ad.Ad.price,
                                                   subsubcategory = ad.LaptopModel.model,
                                                   views = ad.Ad.views,
                                                   condition = ad.Ad.condition,
                                                   AdTags = ad.Ad.AdTags,
                                                   AdImages = ad.Ad.AdImages,
                                                   AdsLocation = ad.Ad.AdsLocation,
                                                   bidsCount = ad.Ad.Bids.Count,
                                                   maxBid = ad.Ad.Bids.OrderByDescending(x => x.price).FirstOrDefault()

                                               }).ToListAsync();
                return temp;
            }
          //  else
            {
                string[] tagsArray = null;
                if (tags != null)
                {
                    tagsArray = tags.Split(',');
                }
                List<ListAdView> ads = await (from ad in db.LaptopAds
                                              where ((category == null || category == "" || ad.Ad.subcategory.Equals(category)) && (subcategory == null || subcategory == "" || ad.Ad.subsubcategory.Equals(subcategory)) && (newOrUsed == null || newOrUsed == "" || newOrUsed == ad.Ad.condition) && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()) && ad.Ad.status.Equals("a") && (model == null || model == "" || model == "undefined" || ad.LaptopModel.model.Equals(model)) && (brand == null || brand == "" || brand == "undefined" || ad.LaptopModel.LaptopBrand.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "" || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "" || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
                                              orderby ad.Ad.time descending
                                              select new ListAdView
                                              {
                                                  title = ad.Ad.title,
                                                  description = ad.Ad.description,
                                                  Id = ad.Ad.Id,
                                                  time = ad.Ad.time,
                                                  category = ad.Ad.category,
                                                  subcategory = ad.LaptopModel.LaptopBrand.brand,
                                                  isnegotiable = ad.Ad.isnegotiable,
                                                  price = ad.Ad.price,
                                                  subsubcategory = ad.LaptopModel.model,
                                                  views = ad.Ad.views,
                                                  condition = ad.Ad.condition,
                                                  AdTags = ad.Ad.AdTags,
                                                  AdImages = ad.Ad.AdImages,
                                                  AdsLocation = ad.Ad.AdsLocation,
                                                  bidsCount = ad.Ad.Bids.Count,
                                                  maxBid = ad.Ad.Bids.OrderByDescending(x => x.price).FirstOrDefault()

                                              }).ToListAsync();
                return ads;
            }
        }

        public void PostAdByCompanyPage(int adId,bool update = false)
        {
            var postAdUsing = System.Web.HttpContext.Current.Request["postAdUsing"];
            if (postAdUsing != null)
            {
                if (update)
                {
                    var comads = db.CompanyAds.Find(adId);
                    if(comads != null)
                    {
                        db.CompanyAds.Remove(comads);
                        db.SaveChanges();
                    }
                }
                if (postAdUsing != "on")
                {
                    var companyId = db.Companies.FirstOrDefault(x => x.title.Equals(postAdUsing)).Id;
                    CompanyAd comAd = new CompanyAd();
                    comAd.companyId = companyId;
                    comAd.adId = adId;
                    db.CompanyAds.Add(comAd);
                    db.SaveChanges();
                }
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLaptopAd([Bind(Include = "Id,category,subcategory,subsubcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = MyAd(ad, "Save", "Electronics","Laptops-Computers");
                    LaptopAd mobileAd = new LaptopAd();
                    mobileAd.color = Request["color"];
                    IdStatus ids = SaveLaptopBrandModel(ad);
                    mobileAd.laptopId = ids.id;
                    ad.status = ids.status;
                    //tags
                    SaveTags(Request["tags"], ad);
                    mobileAd.Id = ad.Id;
                    // ad.LaptopAd.Add(mobileAd);
                    db.LaptopAds.Add(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string ss = e.ToString();
                    }
                    PostAdByCompanyPage(ad.Id);
                    ReplaceAdImages(  ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }

        public ActionResult CreateLaptopAccessoriesAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult CreateCameraAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult CreateCameraAccessoriesAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult EditCameraAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult EditLaptopAccessoriesAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult EditCameraAccessoriesAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Register", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void SaveCameraAd(int adId, bool update = false)
        {
            Camera cam = new Camera();
            cam.category = System.Web.HttpContext.Current.Request["cameraCategory"];
            cam.brand = System.Web.HttpContext.Current.Request["brand"];
            cam.adId = adId;
            if (update)
            {
                db.Entry(cam).State = EntityState.Modified;
            }
            else
            {
                db.Cameras.Add(cam);
            }
            db.SaveChanges();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCameraAd([Bind(Include = "Id,category,subcategory,subsubcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = MyAd(ad, "Save", "Electronics", "Cameras");
                    

                    SaveCameraAd(ad.Id);
                    PostAdByCompanyPage(ad.Id);
                    
                    //tags
                    SaveTags(Request["tags"],  ad);

                    ReplaceAdImages( ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCameraAd([Bind(Include = "Id,category,subcateogry,subsubcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        ad = MyAd(ad, "Update");


                        SaveCameraAd(ad.Id, true);

                        //tags
                        SaveTags(Request["tags"],  ad, "update");
                        //location

                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        ReplaceAdImages( ad, fileNames);
                        db.SaveChanges();
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }

                }
                return RedirectToAction("Register", "Account");
            }
            return View("EditLaptopAccessoriesAd", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCameraAccessoriesAd([Bind(Include = "Id,category,subcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = MyAd(ad, "Save", "Electronics", "CamerasAccessories");
                    

                    SaveCameraAd(ad.Id);
                    PostAdByCompanyPage(ad.Id);

                    //tags
                    SaveTags(Request["tags"],  ad);

                    ReplaceAdImages( ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCameraAccessoriesAd([Bind(Include = "Id,category,subcateogry,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        ad = MyAd(ad, "Update");

                        SaveCameraAd(ad.Id, true);

                        //tags
                        SaveTags(Request["tags"],  ad, "update");
                        //location

                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        ReplaceAdImages( ad, fileNames);
                        db.SaveChanges();
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }

                }
                return RedirectToAction("Register", "Account");
            }
            return View("EditLaptopAccessoriesAd", ad);
        }
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
        public void ReplaceAdImages( Ad ad, FileName[] filenames)
        {
            string newFileName = "";
            int count = 1;
            var id = ad.Id;
            var imaa = db.AdImages.Where(x => x.adId.Equals(id)).Count();
            count = imaa + 1;
            for (int i = 1; i < filenames.Length; i++)
            {
                 IAmazonS3 client;
                 try
                 {
                     using (client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
                     {
                         GetObjectRequest request = new GetObjectRequest
                         {
                             BucketName = _bucketName,
                             Key = _folderName + filenames[i].fileName
                         };
                         using (GetObjectResponse response = client.GetObject(request))
                         {
                             string filename = filenames[i].fileName;
                             if (!System.IO.File.Exists(filename))
                             {
                                 string extension = System.IO.Path.GetExtension(filenames[i].fileName);
                                 newFileName = ad.Id.ToString() + "_" + count + extension;

                                 client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);

                                 CopyObjectRequest request1 = new CopyObjectRequest()
                                 {
                                     SourceBucket = _bucketName,
                                     SourceKey = _folderName + filename,
                                     DestinationBucket = _bucketName,
                                     CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                                     DestinationKey = _folderName + newFileName
                                 };
                                 CopyObjectResponse response1 = client.CopyObject(request1);

                                 AdImage image = new AdImage();
                                 image.imageExtension = extension;
                                 image.adId = ad.Id;
                                 db.AdImages.Add(image);
                                 db.SaveChanges();
                                 count++;



                                 DeleteObjectRequest deleteObjectRequest =
                                 new DeleteObjectRequest
                                 {
                                     BucketName = _bucketName,
                                     Key = _folderName + filenames[i].fileName
                                 };
                                 AmazonS3Config config = new AmazonS3Config();
                                 config.ServiceURL = "https://s3.amazonaws.com/";
                                 using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(
                                      _awsAccessKey, _awsSecretKey, config))
                                 {
                                     client.DeleteObject(deleteObjectRequest);
                                 }
                             }
                         }
                     }
                 }
                 catch (Exception e)
                 {

                 }
            }
        }

        


        [HttpPost]
        public ActionResult FileUploadHandler()
        {
            string[] fileNames = null;
            bool canpass = true;
            string filename = "";
            
            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (canpass)
                {
                    fileNames = new string[Request.Files.Count];
                    canpass = false;
                }
                try
                {
                    IAmazonS3 client;
                    //string logo = "logo2.png";
                    //using (client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
                    //{
                    //    GetObjectRequest request = new GetObjectRequest
                    //         {
                    //             BucketName = _bucketName,
                    //             Key = logo
                    //         };
                    //    using (GetObjectResponse response = client.GetObject(request))
                    //    {
                         //   string dest =System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), logo);
                            //if (!System.IO.File.Exists(logo))
                            //{
                            //    response.WriteResponseStreamToFile(dest);
                            //}
                            Image imgg = Image.FromFile(Server.MapPath(@"\Images\others\WaterMark.png"));
                            float f = float.Parse("1"); //0.5
                            Image img = SetImageOpacity(imgg, f);

                            HttpPostedFileBase file = Request.Files[i];
                    using (Image image = Image.FromStream(file.InputStream, true, true))
                    //  using (Image watermarkImage = Image.FromFile(Server.MapPath(@"\Images\others\WaterMark.png")))
                    using (Image watermarkImage = img)
                    {
                       //Image  image = ResizeImage(image12, 600, 600);
                        using (Graphics imageGraphics = Graphics.FromImage(image))
                        using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
                        {
                            // int x = (image.Width / 2 - watermarkImage.Width / 2);
                            int x = 4;
                            int y = image.Height - watermarkImage.Height-30;
                         //   int y = (image.Height / 2 - watermarkImage.Height / 2);
                            watermarkBrush.TranslateTransform(x, y);
                            imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                            
                            
                            string extension = System.IO.Path.GetExtension(file.FileName);
                            filename = "temp" + DateTime.UtcNow.Ticks + extension;
                            var i2 = new Bitmap(image);
                            //  image.Save(Server.MapPath(@"~\Images\Ads\" + filename));
                            System.IO.Directory.CreateDirectory(Server.MapPath(@"~\Images\Ads\"));

                            i2.Save(Server.MapPath(@"~\Images\Ads\" + filename));
                            //apply Compression
                            using (MagickImage sprite = new MagickImage(System.Web.HttpContext.Current.Server.MapPath(@"~\Images\Ads\" + filename)))
                            {
                                var width = sprite.Width;
                                var height = sprite.Height;
                                sprite.Format = MagickFormat.Jpeg;
                                sprite.Quality = 80;
                                sprite.Resize(width, height);
                                sprite.Write(System.Web.HttpContext.Current.Server.MapPath(@"~\Images\Ads\" + filename));
                            }

                            //upload on aws
                            if (file.ContentLength > 0) // accept the file
                            {
                                AmazonS3Config config = new AmazonS3Config();
                                config.ServiceURL = "https://s3.amazonaws.com/";
                                Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

                                var request2 = new PutObjectRequest()
                                {
                                    BucketName = _bucketName,
                                    CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                                    Key = _folderName + filename,
                                    //InputStream = file.InputStream//SEND THE FILE STREAM
                                    FilePath = Server.MapPath(@"~\Images\Ads\" + filename)
                                };
                                s3Client.PutObject(request2);
                            }
                            if (System.IO.File.Exists(Server.MapPath(@"~\Images\Ads\" + filename)))
                            {
                                System.IO.File.Delete(Server.MapPath(@"~\Images\Ads\" + filename));
                            }
                        }
                    }
                    //  }
                    fileNames[i] = filename;
                    }
               // }
                catch (Exception ex)
                {
                    return Json("Error");
                }
            }
            return Json(fileNames);
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static void UploadDPToAWS(string path, string filename)
        {
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = "https://s3.amazonaws.com/";
            Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

            var request2 = new PutObjectRequest()
            {
                BucketName = _bucketName,
                CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                Key = _userFolder + filename,
                //InputStream = file.InputStream//SEND THE FILE STREAM
                FilePath = path
            };
            s3Client.PutObject(request2);
        }
        public Image SetImageOpacity(Image image, float opacity)
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
        public IdStatus SaveLaptopBrandModel(Ad ad)
        {
            IdStatus ids = new IdStatus();
            ids.status = "a";
            var company = Request["brand"];
            var model = Request["model"];
            if (company != null && company != "")
            {
                company = company.Trim();
                if(model == null)
                {
                    model = "";
                }
              //  model = model.Trim();
            }
            if (true) //company != null
            {
                bool isOldBrand = db.LaptopBrands.Any(x => x.brand.Equals(company));
                if (!isOldBrand)
                {
                    if(company != "" && company != null && company != "undefined")
                    {
                        ids.status = "p";
                        LaptopBrand mob = new LaptopBrand();
                        mob.brand = company;
                        mob.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        mob.time = DateTime.UtcNow;
                        mob.status = "p";
                        db.LaptopBrands.Add(mob);
                        db.SaveChanges();
                        //if(model != "" && model != null && model != "undefined")
                        {
                            LaptopModel mod = new LaptopModel();
                            mod.model = model;
                            mod.brandId = mob.Id;
                            mod.time = DateTime.UtcNow;
                            mod.status = "p";
                            mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            if (model == null || model == "" || model == "undefined")
                            {
                               // ids.status = "a"; because brand is new
                                mod.status = "a";
                            }
                            db.LaptopModels.Add(mod);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    //if (model != "" && model != null && model != "undefined")
                    {
                        bool isOldModel = db.LaptopModels.Any(x => x.model.Equals(model) && x.LaptopBrand.brand.Equals(company));
                        if (!isOldModel)
                        {
                            ad.status = "p";
                            var brandId = db.LaptopBrands.FirstOrDefault(x => x.brand.Equals(company));
                            LaptopModel mod = new LaptopModel();
                            mod.brandId = brandId.Id;
                            mod.model = model;
                            mod.status = "p";
                            mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            mod.time = DateTime.UtcNow;
                            if (model == null || model == "" || model == "undefined")
                            {
                                ids.status = "a";
                                mod.status = "a";
                            }
                            db.LaptopModels.Add(mod);
                            db.SaveChanges();
                            
                        }
                    }
                }
                ids.id = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model).Id;
                return ids;
            }
        }
        
        
        public ActionResult Home_Appliances()
        {
            return View();
        }
        public ActionResult CreateHomeAppliancesAd()
        {
            if (Request.IsAuthenticated)
            {

                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public Ad MyAd( Ad ad,string SaveOrUpdate,string cateogry = null,string subcategory = null)
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
                //var nn = System.Web.HttpContext.Current.Request["isNegotiable"];
                //if (nn == "on")
                //{
                //    ad.isnegotiable = "y";
                //}
                //else
                //{
                //    ad.isnegotiable = "n";
                //}
                ad.isnegotiable = "n";
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
                ad.price = int.Parse(pp);
            }
            //ad.description = ad.description.Replace("'", "`");
            //  ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
            ad.postedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (SaveOrUpdate == "Save")
            {
                ad.category = cateogry;
                if(ad.subcategory == null | ad.subcategory == "")
                {
                    ad.subcategory = subcategory == null ? "" : subcategory;
                }
                ad.time = DateTime.UtcNow;
                db.Ads.Add(ad);
                
            }
            else if (SaveOrUpdate == "Update")
            {
                ad.time =DateTime.Parse(System.Web.HttpContext.Current.Request["time"]);
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
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHomeAppliancesAd([Bind(Include = "Id,category,subcategory,subsubcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                   ad =  MyAd( ad,"Save","Electronics","HomeAppliances");


                    //tags
                    SaveTags(Request["tags"], ad);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("CreateHomeAppliancesAd", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }
        
        public void MyAdLocation(string city, string popularPlace, string exectLocation, Ad ad,string SaveOrUpdate)
        {
            AdsLocation loc = new AdsLocation();
            if (city != null)
            {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    cit.status = "p";
                    db.Cities.Add(cit);
                    db.SaveChanges();
                    loc.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        try
                        {
                            Coordinates co = GetLongitudeAndLatitude(popularPlace,city);
                            if (co.status)
                            {
                                pop.longitude = co.longitude;
                                pop.latitude = co.latitude;
                            }
                        }
                        catch (Exception e)
                        {

                        }
                        
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        pop.addedOn = DateTime.UtcNow;
                        pop.status = "p";
                        db.popularPlaces.Add(pop);
                        db.SaveChanges();
                        loc.popularPlaceId = pop.Id;
                    }
                }
                else
                {
                    loc.cityId = citydb.Id;
                    if (popularPlace != null)
                    {
                        var ppp = db.popularPlaces.FirstOrDefault(x => x.City.cityName.Equals(city, StringComparison.OrdinalIgnoreCase) && x.name.Equals(popularPlace, StringComparison.OrdinalIgnoreCase));
                        if (ppp == null)
                        {
                            popularPlace pop = new popularPlace();
                            try
                            {
                                Coordinates co = GetLongitudeAndLatitude(popularPlace,city);
                                if (co.status)
                                {
                                    pop.longitude = co.longitude;
                                    pop.latitude = co.latitude;
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            pop.cityId = citydb.Id;
                            pop.name = popularPlace;
                            pop.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            pop.addedOn = DateTime.UtcNow;
                            pop.status = "p";
                            db.popularPlaces.Add(pop);
                            
                            db.SaveChanges();
                            loc.popularPlaceId = pop.Id;
                        }
                        else
                        {
                            loc.popularPlaceId = ppp.Id;
                        }
                    }
                }
                loc.exectLocation = exectLocation == "undefined"?null:exectLocation;
                loc.Id = ad.Id;
                if (SaveOrUpdate == "Save")
                {
                    ad.AdsLocation = loc;
                   // db.AdsLocations.Add(loc);
                }
                else if (SaveOrUpdate == "Update")
                {
                    if(ad.AdsLocation == null)
                    {
                        var ada = db.Ads.Where(x => x.Id.Equals(ad.Id)).Include(x => x.AdsLocation).FirstOrDefault();
                        if(ada != null)
                        {
                            ad.AdsLocation = ada.AdsLocation;
                        }
                       //ad.AdsLocation = db.Ads.Find(ad.Id).AdsLocation;
                    }
                    if (ad.AdsLocation == null) {
                        ad.AdsLocation = loc;
                       // db.AdsLocations.Add(loc);
                    }
                    else { 
                    ad.AdsLocation.cityId = loc.cityId;
                    ad.AdsLocation.popularPlaceId = loc.popularPlaceId;
                    ad.AdsLocation.exectLocation = loc.exectLocation;
                    }
                    //db.Entry(loc).State = EntityState.Modified;
                }
                    db.SaveChanges();
                
            }
        }
        [Route("TV-Video-Audio",Name = "TV-Video-Audio")]
        public async Task<ActionResult> tv(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.subcategory = "TV-Video-Audio";
            ViewBag.category = "Electronics";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Electronics", "TV - Video - Audio", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Vehicles/Index", viewModel);
        }
        [Route("Other-Electronics",Name ="Other-Electronics")]
        public async Task<ActionResult> otherelectronics(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.subcategory = "Other-Electronics";
            ViewBag.category = "Electronics";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Electronics", "Other-Electronics", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Vehicles/Index", viewModel);
        }
        [Route("Home-Appliances",Name ="Home-Appliances")]
        public async Task<ActionResult> homeappliances(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.subcategory = "Home-Appliances";
            ViewBag.category = "Electronics";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Electronics", "Home-Appliances", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Vehicles/Index", viewModel);
        }
        [Route("Games",Name ="Games")]
        public async Task<ActionResult> games(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.subcategory = "Games";
            ViewBag.category = "Electronics";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Electronics", "Games", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Vehicles/Index", viewModel);
        }

        //[Route("Electronics/{category?}")]
        //public ActionResult Index(string category)
        //{
        //    ViewBag.subcategory = category;
        //    ViewBag.category = "Vehicles";
        //    return View("Index");
        //}
        //[HttpPost]
        
        public ActionResult EditLaptopAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }

            }
            return RedirectToAction("Register", "Account");
        }
        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
        public static string URLFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }
        public void SaveTags(string s, Ad ad,string addOrUpdate = "add")
        {
            if(addOrUpdate == "update")
            {
                var adid = ad.Id;
                var adtags = db.AdTags.Where(x => x.adId.Equals(adid)).ToList();
                foreach (var cc in adtags)
                {
                    db.AdTags.Remove(cc);
                }
                 db.SaveChanges();
            }
            string[] values = s.Split(',');
            Inspinia_MVC5_SeedProject.Models.Tag[] tags = new Inspinia_MVC5_SeedProject.Models.Tag[values.Length];
            AdTag[] qt = new AdTag[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
                string ss = values[i];
                if (ss != "")
                {
                    var data = db.Tags.FirstOrDefault(x => x.name.Equals(ss, StringComparison.OrdinalIgnoreCase));

                    tags[i] = new Inspinia_MVC5_SeedProject.Models.Tag();
                    if (data != null)
                    {
                        tags[i].Id = data.Id;
                    }
                    else
                    {
                        tags[i].name = values[i];
                        tags[i].time = DateTime.UtcNow;
                        tags[i].createdBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        db.Tags.Add(tags[i]);
                    }
                }
                else
                {
                    tags[i] = null;
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string sb = e.ToString();
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (tags[i] != null)
                {
                    qt[i] = new AdTag();
                    qt[i].adId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.AdTags.Add(qt[i]);
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string sb = e.ToString();
            }
        }
        public ActionResult UpdateLaptopAd([Bind(Include = "Id,category,subcategory,subsubcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        ad = MyAd(ad, "Update");
                        LaptopAd mobileAd = new LaptopAd();

                        mobileAd.color = Request["color"];
                        var company = Request["brand"];
                        var model = Request["model"];

                        //tags
                        SaveTags(Request["tags"], ad,"update");
                        IdStatus ids = SaveLaptopBrandModel(ad);
                        //  var mobileModel = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model);
                        mobileAd.laptopId = ids.id;
                        ad.status = ids.status;

                        //db.Ads.Add(ad);
                        mobileAd.Id = ad.Id;
                        db.Entry(mobileAd).State = EntityState.Modified;
                        //ad.MobileAds.Add(mobileAd);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location
                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Update");
                        ReplaceAdImages( ad, fileNames);
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        [Route("Details/{id?}/{title?}")]
        public async Task<ActionResult> Details(int? id,string title = null)
        {
            Ad add = await db.Ads.FindAsync(id);
            if (add == null)
            {
                return RedirectToAction("../not-found");
            }
            //await AdViews(id);
            string loginUserId = Request.IsAuthenticated ? User.Identity.GetUserId() : null;
            add.views++;
            db.Entry(add).State = EntityState.Modified;
            bool isadmin = false;
            if(loginUserId != null)
            {
                var user =await db.AspNetUsers.FindAsync(loginUserId);
                if(user != null)
                {
                    if(user.status != null)
                    {
                        isadmin = user.status.Equals("admin");
                    }
                    
                }
                
            }
            await db.SaveChangesAsync();
            AdDetailViewModel ads = await (from ad in db.Ads
                                           where ad.Id == id
                                           select new AdDetailViewModel()
                                           {
                                               loginUserId = loginUserId,
                                               isAdmin = isadmin,
                                               Id = ad.Id,
                                               name = ad.name,
                                               phoneNumber = ad.phoneNumber,
                                               category = ad.category,
                                               postedBy = ad.postedBy,
                                               description = ad.description,
                                               time = ad.time,
                                               price = ad.price,
                                               isnegotiable = ad.isnegotiable,
                                               type = ad.type,
                                               condition = ad.condition,
                                               status = ad.status,
                                               views = ad.views,
                                               subcategory = ad.subcategory,
                                               subsubcategory = ad.subsubcategory,
                                               title = ad.title,
                                               AdImages = ad.AdImages.ToList(),
                                               AdsLocation = ad.AdsLocation,
                                               AdTags = ad.AdTags.ToList(),
                                               Bids = ad.Bids.ToList(),
                                               Comments = ad.Comments.ToList(),
                                               JobAd = ad.JobAd,
                                               JobSkills = ad.JobSkills.ToList(),
                                               AspNetUser = ad.AspNetUser,
                                               SaveAdsCount = ad.SaveAds.Count,
                                               BikeAd = ad.BikeAd,
                                               Camera = ad.Camera,
                                               CarAd = ad.CarAd,
                                               CompanyAd = ad.CompanyAd,
                                               House = ad.House,
                                               LaptopAd = ad.LaptopAd,
                                               MobileAd = ad.MobileAd,
                                               ReportedAdsCount = ad.Reporteds.Count,
                                               isReported = ad.Reporteds.Any(x=>x.reportedBy.Equals(loginUserId)),
                                               isSaved = ad.SaveAds.Any(x=>x.savedBy.Equals(loginUserId)),
                                               maxBid = ad.Bids.OrderByDescending(x => x.price).FirstOrDefault()
                                           }).FirstOrDefaultAsync();
            return View(ads);
        }
        
        // GET: /Electronics/Create
        
        public ActionResult CreateLaptopAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        
        
        // POST: /Electronics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        

        // POST: /Electronics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        

        // GET: /Electronics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }
        public Coordinates test(string abc,string city)
        {
            return GetLongitudeAndLatitude(abc,city);
           // return GetCoordinates(abc);
        }
        public class Coordinates
        {
            public decimal longitude;
            public decimal latitude;
            public bool status;
        }
        public static Coordinates GetLongitudeAndLatitude(string famousPlace,string city)
        {
            Coordinates co = new Coordinates();
            co.status = false;
            //string urlAddress = "https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyBilH9FSqKqoahGM2ImsDB4XAMNiQASPsQ&address=" + HttpUtility.UrlEncode(famousPlace) + "&region=" + city + "&sensor=false";
            string urlAddress = "https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyBilH9FSqKqoahGM2ImsDB4XAMNiQASPsQ&address=" + HttpUtility.UrlEncode(famousPlace + " " +city)  + "&sensor=false";
            try
            {
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(urlAddress);
                XmlNodeList objXmlNodeList = objXmlDocument.SelectNodes("/GeocodeResponse/result/geometry/location");
                if (objXmlNodeList.Count == 1)
                {
                    co.status = true;
                }
                foreach (XmlNode objXmlNode in objXmlNodeList)
                {
                    // GET LONGITUDE 
                    co.longitude = decimal.Parse ( objXmlNode.ChildNodes.Item(0).InnerText);
                    // GET LATITUDE 
                    co.latitude= decimal.Parse ( objXmlNode.ChildNodes.Item(1).InnerText);
                }
            }
            catch
            {
                // Process an error action here if needed  
            }
            return co;
        }
        
        
        // POST: /Electronics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ad ad = db.Ads.Find(id);
            db.Ads.Remove(ad);
            db.SaveChanges();
            return RedirectToAction("Index");
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
