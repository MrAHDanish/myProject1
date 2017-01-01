using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using Quartz;
using Quartz.Impl;
using System.Web.Mvc;
//using System.Web.Http;
using AngleSharp;
using System.Configuration;
using System.Data.Entity.Validation;
using Amazon;
using Amazon.S3;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using ImageMagick;
using Amazon.S3.Model;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ElectronicsController.sendEmail("irfanyusanif@gmail.com", "I am job scheduler v 2.0 ", ".I am running at " + DateTime.UtcNow);
        }
    }
    public class AdminController : Controller
    {
        private Entities db = new Entities();
        public ActionResult ManageScraping()
        {
            return View();
        }
        public bool compressFilesOnAmazon()
        {
            // Create a client
            string lastFileCompressed = "";
            string error = "";
            bool start = false;
            try {
                AmazonS3Config config = new AmazonS3Config();
                config.ServiceURL = "https://s3.amazonaws.com/";
                AmazonS3Client client = new AmazonS3Client(_awsAccessKey, _awsSecretKey, config);
                
                // List all objects
                ListObjectsRequest listRequest = new ListObjectsRequest
                {
                    BucketName = "dealkar.pk",
                    Prefix = "Items"
                };

                ListObjectsResponse listResponse;
                do
                {
                    // Get a list of objects
                    listResponse = client.ListObjects(listRequest);
                    foreach (S3Object obj in listResponse.S3Objects)
                    {
                        if (obj.Key.EndsWith(".jpg") || obj.Key.EndsWith(".jpeg") || obj.Key.EndsWith(".png") )
                        {
                            if (obj.Key.EndsWith("16132_1.jpg"))
                            {
                                start = true;
                            }
                            if (start)
                            {
                                var webClient = new WebClient();
                                byte[] imageBytes = webClient.DownloadData("https://s3.amazonaws.com/dealkar.pk/" + obj.Key);
                                //apply Compression
                                using (MagickImage sprite = new MagickImage(imageBytes))
                                {
                                    var width = sprite.Width;
                                    var height = sprite.Height;
                                    // sprite.Format = MagickFormat.Jpeg;
                                    sprite.Quality = 70;
                                    sprite.Resize(width, height);
                                    System.IO.Directory.CreateDirectory(Server.MapPath(@"~\admin\Items\"));
                                    sprite.Write(System.Web.HttpContext.Current.Server.MapPath(obj.Key));

                                    Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

                                    var request2 = new Amazon.S3.Model.PutObjectRequest()
                                    {
                                        BucketName = "dealkar.pk",
                                        CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                                        Key = obj.Key,
                                        FilePath = System.Web.HttpContext.Current.Server.MapPath(obj.Key)
                                    };
                                    s3Client.PutObject(request2);
                                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(obj.Key)))
                                    {
                                        System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(obj.Key));
                                        lastFileCompressed = obj.Key;
                                    }
                                }
                            }
                        }
                    }

                    // Set the marker property
                    listRequest.Marker = listResponse.NextMarker;
                } while (listResponse.IsTruncated);
            }catch(Exception e)
            {
                error = e.ToString();
            }
            ElectronicsController.sendEmail("irfanyusanif@gmail.com", "File Compression status", "Last file compressed was " + lastFileCompressed + " and error is " + error + "/n and time is " + DateTime.UtcNow);
            return true;
        }
        //[HttpPost]
        public ActionResult ExecuteJob()
        {
            if (!isSuperAdmin())
            {
                return HttpNotFound();
            }
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();

            sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
            //  .WithIdentity("myTrigger", "group1")
            //  .StartNow()
            //  .WithSimpleSchedule(x => x
            //      .WithIntervalInSeconds(86400)
            //      .RepeatForever())
            //  .Build();
            //.ForJob(job)
                                    .WithIdentity("myTrigger", "group1")
                                    .StartAt(DateTime.UtcNow)
                                    .WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24))
                                    //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(4, 20))
                                    .Build();

            sched.ScheduleJob(job, trigger);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        public bool isSuperAdmin()
        {
            if (Request.IsAuthenticated)
            {
                if (User.Identity.GetUserId() == "c1239071-cf6d-4cec-9da4-4b2871250143" || User.Identity.GetUserId() == "7234b5b0-2cb5-4d4a-bc18-98e17c460221")
                {
                    return true;
                }
            }
            return false;
        }
        public string GetModel (string title,string brand)
        {
            // string[] titles = title.Split(' ');
            var allModels = db.MobileModels.Where(x => x.Mobile.brand.Equals(brand)).Select(x=>x.model);
            // allModels.OrderByDescending(model => titles.Where(tit => tit.Equals(model, StringComparison.OrdinalIgnoreCase)).Count()).FirstOrDefault();
             var mod = allModels.OrderByDescending(x => x.Length)
             .FirstOrDefault(x => title.Contains(x));
            return mod;
        }
        public async Task<ActionResult> detailPageData(string link)
        {
            NewMobileViewModel mobile = new NewMobileViewModel();
            try {
                //change url in mvc controller
                var config = AngleSharp.Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync("http://www.justprice.pk/" + link);

                var imageCells = document.QuerySelectorAll(".owl-lazy");
                string imageLink = imageCells.Select(m => m.GetAttribute("src")).FirstOrDefault();

                var namecells = document.QuerySelectorAll("#product-name");
                var name = namecells.Select(x => x.TextContent).FirstOrDefault();

                var storeNameCells = document.QuerySelectorAll(".pb-product-lprice-list-item a img");
                var storeImage = storeNameCells.Select(x => x.GetAttribute("src")).ToArray();

                var storePriceCells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                var storePrices = storePriceCells.Select(x => x.TextContent).ToArray();

                var itemSpecCells = document.QuerySelectorAll(".c-spec");
                var storespecs = itemSpecCells.Select(x => x.InnerHtml).FirstOrDefault();

                var itemMinPriceCells = document.QuerySelectorAll(".pb-product-price span");
                var MinPrice = itemMinPriceCells.Select(x => x.InnerHtml).ToArray();
                var price = MinPrice[2];

                var warrantyCells = document.QuerySelectorAll(".pb-product-lprice-list-item .medium-3.columns:nth-child(1)");
                var warranty = warrantyCells.Select(x => x.InnerHtml).ToArray();

                MobileStoreInfo[] store = new MobileStoreInfo[storePrices.Count()];
                for (int i = 0; i < store.Count(); i++)
                {
                    store[i] = new MobileStoreInfo();
                }
                int index = 0;
                int index2 = 1;
                foreach (var st in store)
                {
                    st.price = storePrices[index];
                    st.storeImage = storeImage[index];
                    st.warranty = warranty[index2].Split(new[] { "<br>" }, StringSplitOptions.None)[0].Trim();
                    st.inStock = warranty[index2].Split(new[] { "<br>" }, StringSplitOptions.None)[2].Trim();
                    index++;
                    index2 = index2 + 2;
                }

                mobile.imageUrl = imageLink;
                mobile.storeInfo = store;
                mobile.price = price;
                mobile.spec = storespecs;
                mobile.name = name;
                //var InstockCells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                //var instock = InstockCells.Select(x => x.TextContent);

                //var conditioncells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                //var condition = conditioncells.Select(x => x.TextContent);

                //var deliveryCells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                //var delivery = deliveryCells.Select(x => x.TextContent);

                //var phone1cells = document.QuerySelectorAll(phone1Selector);
                //string[] titles = phone1cells.Select(m => m.GetAttribute("title")).ToArray();
                return View("../MobilesTablets/NewMobiles", mobile);
                //return Json(mob, JsonRequestBehavior.AllowGet);
            }catch(Exception e)
            {
                return View("../MobilesTablets/NewMobiles", mobile);
            }
        }
        public async Task<PartialViewResult> NewMobilePartialView(string link)
        {
            NewMobileViewModel mobile = new NewMobileViewModel();
            try
            {
                //change url in mvc controller
                var config = AngleSharp.Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync("http://www.justprice.pk/" + link);

                var imageCells = document.QuerySelectorAll(".owl-lazy");
                string imageLink = imageCells.Select(m => m.GetAttribute("src")).FirstOrDefault();

                var namecells = document.QuerySelectorAll("#product-name");
                var name = namecells.Select(x => x.TextContent).FirstOrDefault();

                var storeNameCells = document.QuerySelectorAll(".pb-product-lprice-list-item a img");
                var storeImage = storeNameCells.Select(x => x.GetAttribute("src")).ToArray();

                var storePriceCells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                var storePrices = storePriceCells.Select(x => x.TextContent).ToArray();

                var itemSpecCells = document.QuerySelectorAll(".c-spec");
                var storespecs = itemSpecCells.Select(x => x.InnerHtml).FirstOrDefault();

                var itemMinPriceCells = document.QuerySelectorAll(".pb-product-price span");
                var MinPrice = itemMinPriceCells.Select(x => x.InnerHtml).ToArray();
                var price = MinPrice[2];

                var warrantyCells = document.QuerySelectorAll(".pb-product-lprice-list-item .medium-3.columns:nth-child(1)");
                var warranty = warrantyCells.Select(x => x.InnerHtml).ToArray();

                MobileStoreInfo[] store = new MobileStoreInfo[storePrices.Count()];
                for (int i = 0; i < store.Count(); i++)
                {
                    store[i] = new MobileStoreInfo();
                }
                int index = 0;
                int index2 = 1;
                foreach (var st in store)
                {
                    st.price = storePrices[index];
                    st.storeImage = storeImage[index];
                    st.warranty = warranty[index2].Split(new[] { "<br>" }, StringSplitOptions.None)[0].Trim();
                    st.inStock = warranty[index2].Split(new[] { "<br>" }, StringSplitOptions.None)[2].Trim();
                    index++;
                    index2 = index2 + 2;
                }

                mobile.imageUrl = imageLink;
                mobile.storeInfo = store;
                mobile.price = price;
                mobile.spec = storespecs;
                mobile.name = name;
                //var InstockCells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                //var instock = InstockCells.Select(x => x.TextContent);

                //var conditioncells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                //var condition = conditioncells.Select(x => x.TextContent);

                //var deliveryCells = document.QuerySelectorAll(".pb-product-lprice-list-item .c-price.s-price");
                //var delivery = deliveryCells.Select(x => x.TextContent);

                //var phone1cells = document.QuerySelectorAll(phone1Selector);
                //string[] titles = phone1cells.Select(m => m.GetAttribute("title")).ToArray();
                return PartialView("../MobilesTablets/_NewMobileData",mobile);
                //return Json(mob, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return PartialView("../MobilesTablets/_NewMobileData", mobile);
            }
        }

        public async Task<JsonResult> scrapeJustPrice(string query = null, string brand = null, string model = null)
        {
            if (brand == null || brand == "")
            {
            //    string[] querySplit = query.Split(new Char[] { ' ', '-' },
            //                     StringSplitOptions.RemoveEmptyEntries);
                var allBrands = db.Mobiles.Select(x => x.brand);
                brand = allBrands.FirstOrDefault(x => query.Contains(x));
                model = GetModel(query, brand);
            }
            else if (model == null || model == "")
            {
                model = GetModel(query, brand);
            }
            int adsCount = 0;
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            
            //if(model == null || model == "")
            //{
            //    return Json("Error", JsonRequestBehavior.AllowGet);
            //}

            query = brand + " " + model;
         //   query = query.Replace(' ', '+');
            var document = await BrowsingContext.New(config).OpenAsync("http://www.justprice.pk/search_products.htm?searchWithin=&searchText=" + query);


            var phone1Selector = ".pb-pitem-details-cnt .c-name";

            var phone1cells = document.QuerySelectorAll(phone1Selector);
            string [] titles = phone1cells.Select(m => m.GetAttribute("title")).ToArray();
            
            var phone1Selector1 = ".pb-pitem-details-cnt a";

            var phone1cells1 = document.QuerySelectorAll(phone1Selector1);
            string [] links = phone1cells1.Select(m => m.GetAttribute("href")).ToArray();


            var phone1Selector2 = ".pb-pitem-details-cnt .c-price";

            var phone1cells2 = document.QuerySelectorAll(phone1Selector2);
            string [] prices = phone1cells2.Select(m => m.TextContent).ToArray();

            var imagesSelector = ".img-responsive.center-block";
            var imagesCells = document.QuerySelectorAll(imagesSelector);
            string[] images = imagesCells.Select(m => m.GetAttribute("src")).ToArray();
            var error = fileUploadforJustPrice(images,"NewMobiles/");
            

            string[][] Tablero = new string[titles.Count()][];
            for (int ix = 0; ix < titles.Count(); ++ix)
            {
                Tablero[ix] = new string[4];
            }
            int indexing = 0;
         foreach(var index in Tablero)
            {
                int count = 0;
                
                index[count++] = titles[indexing];
                index[count++] = links[indexing].Split('/').Last();
                index[count++] = prices[indexing];
                index[count++] = images[indexing].Split('/').Last();
                indexing++;
            }

            return Json(Tablero , JsonRequestBehavior.AllowGet);
        }
        [Route("Mobile-Prices/{url?}")]
        public async Task<ActionResult> MobilePrices(string url)
        {
            string brand = url.Split('-')[0];
            string completeUrl = "http://www.justprice.pk/" + brand + "-mobile-price-in-pakistan/" + url;
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(completeUrl);


            var storeImageSelector = ".pb-product-lprice-list-item .img-responsive";
            var storeImageCells = document.QuerySelectorAll(storeImageSelector);
            string[] storeImages = storeImageCells.Select(m => m.GetAttribute("src")).ToArray();

            var priceSelector = ".pb-pstore-price .c-price";
            var priceCells = document.QuerySelectorAll(priceSelector);
            string[] prices = priceCells.Select(m => m.TextContent).ToArray();

            //var storeWarrantySelector = ".pb-product-lprice-list-item .fa-refresh";
            //var storeWarrantyCells = document.QuerySelectorAll(storeWarrantySelector);
            //string[] storeWarranty = storeWarrantyCells.Select(m => m.GetAttribute("src")).ToArray();

            fileUploadforJustPrice(storeImages, "StoreImages/");

            return View("../Home/temp2");
        }
        public object fileUploadforJustPrice(string [] paths,string folderName)
        {
            string[] fileNames = null;
            bool isFound = false;
         //   string filename = "";
            int count = 1;
            foreach(var path in paths)
                if (path != "" && path != null)
                {
                    string fileName = path.Split('/').Last();
                    AmazonS3Config config = new AmazonS3Config();
                    config.ServiceURL = "https://s3.amazonaws.com/";
                    AmazonS3Client client = new AmazonS3Client(_awsAccessKey, _awsSecretKey, config);

                    // List all objects
                    ListObjectsRequest listRequest = new ListObjectsRequest
                    {
                        BucketName = "dealkar.pk",
                        Prefix = folderName+ fileName,
                        MaxKeys = 1
                    };
                    ListObjectsResponse listResponse = client.ListObjects(listRequest);
                    isFound = false;
                    foreach (S3Object obj in listResponse.S3Objects)
                    {
                        if (obj.Key.EndsWith(fileName))
                        {
                            isFound = true;
                        }
                    }
                    if (!isFound) {
                        string localfilePath = @"\Images\" + folderName + fileName;
                        System.IO.Directory.CreateDirectory(Server.MapPath(@"\Images\" + folderName));
                        string completeFilePath = System.Web.HttpContext.Current.Server.MapPath(localfilePath);
                        try
                        {
                            using (WebClient client1 = new WebClient())
                            {
                                client1.DownloadFile(path, System.Web.HttpContext.Current.Server.MapPath(localfilePath));
                            }
                            {
                                {
                                    //upload to aws
                                    config.ServiceURL = "https://s3.amazonaws.com/";
                                    Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

                                    var request2 = new Amazon.S3.Model.PutObjectRequest()
                                    {
                                        BucketName = _bucketName,
                                        CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                                        Key = "NewMobiles/" + fileName,
                                        // InputStream = file.InputStream//SEND THE FILE STREAM
                                        FilePath = System.Web.HttpContext.Current.Server.MapPath(localfilePath)
                                    };
                                    s3Client.PutObject(request2);
                                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(localfilePath)))
                                    {
                                        System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(localfilePath));
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            string s = e.ToString();
                            return Json(s, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
            return true;
        }
        public async Task<JsonResult> runScrapingCode(string special = null,string specialLink = null)
        {
           // deleteAds();
            int adsCount = 0;
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync("https://www.olx.com.pk/all-results/?search%5Bphotos%5D=false&page=2");
            if (special != null && special != "undefined")
            {
                if (specialLink == null || specialLink == "undefined" || specialLink == "")
                {
                    document = await BrowsingContext.New(config).OpenAsync("https://www.olx.com.pk/all-results/?search%5Bphotos%5D=false&page=2");
                }
                else
                {
                    document = await BrowsingContext.New(config).OpenAsync(specialLink);
                }
            }
            //ad link
            var titleSelector = "a.detailsLink";
            var titlecells = document.QuerySelectorAll(titleSelector);
            var titles = titlecells.Select(m => m.GetAttribute("href"));

            string previousAdLink = null;
            foreach (var link in titles)
            {
                if (previousAdLink == null)
                {
                    previousAdLink = link;
                }
                else if (previousAdLink == link)
                {
                    previousAdLink = link;
                    continue;
                }
                previousAdLink = link;
                Ad ad = new Ad();
                ad = await GetAdFromOlx(ad, link, special);
                adsCount++;
                if(adsCount == 39)
                {
                    break;
                }
            }
            
            return Json(titles.Count() + " total Links" + adsCount + " posted", JsonRequestBehavior.AllowGet);

            //    var address = "https://www.olx.com.pk/item/islamic-kalma-old-coin-IDUFlmP.html#afd73d07ac";
            //var address = specialLink;
            //Ad ad1 = new Ad();
            //ad1 = await GetAdFromOlx(ad1, address, null);

            //db.Ads.Add(ad1);
            //return Json("Done" + " New Ads Posted", JsonRequestBehavior.AllowGet);
            return Json(adsCount + " New Ads Posted", JsonRequestBehavior.AllowGet);
        }
        public async Task<Ad> GetAdFromOlx(Ad ad, string url,string special)
        {
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(url);
            //ad id
            var aSelector = ".pdingleft10.brlefte5 .rel.inlblk";
            var acells = document.QuerySelectorAll(aSelector);
            var a = acells.Select(m => m.TextContent);
            string adId = a.FirstOrDefault();
            adId = adId.Trim();
            //title
            var titleSelector = ".offerheadinner .brkword";
            var titlecells = document.QuerySelectorAll(titleSelector);
            var titles = titlecells.Select(m => m.TextContent);
            ad.title = titles.FirstOrDefault();
            ad.title = ad.title.Trim();
            //description
            var dSelector = ".pding10.lheight20.large"; //.pding10 + .lheight20 + .large
            var dcells = document.QuerySelectorAll(dSelector);
            var d = dcells.Select(m => m.TextContent);
            ad.description = d.FirstOrDefault();
            ad.description = ad.description.Trim();
            //price
            var pSelector = ".pricelabel strong";
            var pcells = document.QuerySelectorAll(pSelector);
            var p = pcells.Select(m => m.TextContent);
            string price = p.FirstOrDefault();
            try {
                price = price.Split(' ')[1];
                ad.price = decimal.Parse(price.ToString());
            }catch(Exception e) { }
            //brand
            var bSelector = ".brbottdashc8 a";
            var bcells = document.QuerySelectorAll(bSelector);
            var b = bcells.Select(m => m.TextContent);
            string brand = b.FirstOrDefault();
            try { 
            brand = brand.Trim();
            }catch(Exception e) { string s = e.ToString(); }
            //city
           
            var cSelector = ".c2b.small";
            var ccells = document.QuerySelectorAll(cSelector);
            var c = ccells.Select(m => m.TextContent);
            string city = c.FirstOrDefault();
            var city1 = db.Cities.FirstOrDefault(x => city.Contains(x.cityName)).cityName;
          //  city = city.Split(',')[0];
            city = city1;
            //name
            var nSelector = ".block.color-5.brkword.xx-large";
            var ncells = document.QuerySelectorAll(nSelector);
            var names = ncells.Select(m => m.TextContent);
            ad.name = names.FirstOrDefault();
            try {
                ad.name = ad.name.Trim();
            }catch(Exception e) { }
            //phone number
            var phSelector = ".xx-large.lheight20.fnormal";
            var phcells = document.QuerySelectorAll(phSelector);
            var ph = phcells.Select(m => m.TextContent);
            string phone = ph.FirstOrDefault();
            if (phone != null && phone != "")
            {
                ad.phoneNumber = "0" + phone;
                ad.phoneNumber = ad.phoneNumber.Trim();
            }
            
            //category
            var catSelector = ".link.nowrap span";
            var catcells = document.QuerySelectorAll(catSelector);
            var cat = catcells.Select(m => m.TextContent);
            ad.category = cat.FirstOrDefault();
            ad.category = ad.category.Split(new string[] {city}, StringSplitOptions.None)[0];
            ad.category = ad.category.Trim();
            //subCategory
            var subSelector = ".middle ul li:nth-child(3)";
            var subcells = document.QuerySelectorAll(subSelector);
            var sub = subcells.Select(m => m.TextContent);
            ad.subcategory = sub.FirstOrDefault();
            ad.subcategory = ad.subcategory.Split(new string[] { city }, StringSplitOptions.None)[0];
            //ad.subcategory = ad.subcategory.Remove(ad.subcategory.LastIndexOf(' ') + 1);
            ad.subcategory = ad.subcategory.Split('»')[1];
            ad.subcategory = ad.subcategory.Trim();
            //subCategory
            string subsubcategory = null;
            try { 
            var subsubSelector = ".middle ul li:nth-child(4)";
            var subsubcells = document.QuerySelectorAll(subsubSelector);
            var subsub = subsubcells.Select(m => m.TextContent);
             subsubcategory = subsub.FirstOrDefault();
            subsubcategory = subsubcategory.Split(new string[] { city }, StringSplitOptions.None)[0];
             //   subsubcategory = subsubcategory.Remove(subsubcategory.LastIndexOf(' ') + 1);
            subsubcategory = subsubcategory.Split('»')[1];
            subsubcategory = subsubcategory.Trim();
            }catch(Exception e) { }

            string year = null;
            string petrol = null;
            string km = null;
            string rooms = null;
            string area = null;
            decimal? priceTo = null;
            //Year
            var yearSelector = ".brbottdashc8 td:nth-child(2) .block";
            var yearcells = document.QuerySelectorAll(yearSelector);
            var years = yearcells.Select(m => m.TextContent);
            year = years.FirstOrDefault();
            try
            {
                year = year.Trim();
            }
            catch (Exception e) { string s = e.ToString(); }
            //Rooms
            var roomSelector = ".brbottdashc8 td:nth-child(2) .block a";
            var roomcells = document.QuerySelectorAll(roomSelector);
            var room = roomcells.Select(m => m.TextContent);
            rooms = room.FirstOrDefault();
            try
            {
                rooms = rooms.Trim();
            }
            catch (Exception e) { string s = e.ToString(); }
            //Fuel type
            var petrolSelector = ".brbottdashc8 td:nth-child(3) a";
            var petrolcells = document.QuerySelectorAll(petrolSelector);
            var petrols = petrolcells.Select(m => m.TextContent);
            petrol = petrols.FirstOrDefault();
            try
            {
                petrol = petrol.Trim();
            }
            catch (Exception e) {  }
            //area
            var areaSelector = ".brbottdashc8 td:nth-child(3)";
            var areacells = document.QuerySelectorAll(areaSelector);
            var areas = areacells.Select(m => m.TextContent);
            area = areas.FirstOrDefault();
            try
            {
                area = area.Split(':')[1];
                area = area.Trim();
                area = area.Split(' ')[0];
            }
            catch (Exception e) { string s = e.ToString(); }
            //KM Driven
            var kmSelector = "table.details tr:nth-child(3) .block";
            var kmcells = document.QuerySelectorAll(kmSelector);
            var kms = kmcells.Select(m => m.TextContent);
            km = kms.FirstOrDefault();
            try
            {
                km = km.Trim();
                km = km.Split(' ')[0];
            }
            catch (Exception e) { string s = e.ToString(); }
           //price from == ad.price
            //price to
            var ptSelector = ".x-large.margintop7.block.not-arranged span:nth-child(2)";
            var ptcells = document.QuerySelectorAll(ptSelector);
            var pts = ptcells.Select(m => m.TextContent);
            String pt = pts.FirstOrDefault();
            try
            {
                pt = pt.Split(' ')[1];
                priceTo = decimal.Parse(pt.ToString());
            }
            catch (Exception e) { }
            //pictures
            var pictureSelector = "#bigGallery a[href]";
            var pictureCells = document.QuerySelectorAll(pictureSelector);
            var picutes = pictureCells.Select(m =>m.GetAttribute("href"));

            ad.time = DateTime.UtcNow;
            ad.views = 0;
            ad.status = "a";
            ad.isnegotiable = "y";
            ad.type = true;
            ad.condition = "u";
            ad.postedBy = User.Identity.GetUserId();
            db.Ads.Add(ad);
            try { 
            await db.SaveChangesAsync();
            }catch(Exception e)
            {
                string s = e.ToString();
            }
            if (city != null && city != "")
            {
                AdsLocation loc = new AdsLocation();
                loc.Id = ad.Id;
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    cit.status = "p";
                    db.Cities.Add(cit);
                    await db.SaveChangesAsync();
                    loc.cityId = cit.Id;
                }
                else
                {
                    loc.cityId = citydb.Id;
                }
                db.AdsLocations.Add(loc);
                await db.SaveChangesAsync();
            }
            
            await SaveCategoriesData(ad, ad.category, ad.subcategory, subsubcategory,brand,year,petrol,km,rooms,area,priceTo,special);
            try
            {
                await db.SaveChangesAsync();
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
           fileUpload(picutes.Take(3), ad.Id);
            //string subsubcategory = subsub.FirstOrDefault();
            //subsubcategory = subsubcategory.TrimEnd(' ');
            //subsubcategory = subsubcategory.Remove(subsubcategory.LastIndexOf(' ') + 1);
            //subsubcategory = subsubcategory.Split('»')[1];
            //subsubcategory = subsubcategory.Trim();

            return ad;
        }
        public object deleteAds()
        {
            //var li = from aa in db.Ads
            //         from city in db.Cities
            //         where aa.category.Contains(city.cityName)
            //         select new
            //         {
            //             category = aa.category,
            //         };

            var ddd = db.Ads.Where(x => db.Cities.Any(c => x.subcategory.Contains(c.cityName)));
            int total = ddd.Count();
            foreach(var dd in ddd)
            {
                db.Ads.Remove(dd);
            }
            db.SaveChanges();
          return total;
        }
        public async Task<bool> SaveCategoriesData(Ad ad, string category, string subcategory,string subsubcategory,string brand, string year, string fuelType, string Km,string rooms, string area,decimal? priceTo,string special = null)
        {
            
            if (special == "Qurbani")
            {
                if (category == "Pets")
                {
                    ad.category = "Animals";
                    ad.subcategory = subcategory;
                    ad.subsubcategory = "Qurbani";
                    return true;
                }
                
            }
            if (category == "Mobiles")
            {
                await SaveMobileAd(ad, ad.title);
            }
            else if (category == "Electronics & Appliances")
            {
                ad.category = "Electronics";
                if (subcategory == "Computers & Accessories")
                {
                    ad.subcategory = "Laptops-Computers";
                    ad.subsubcategory = subsubcategory;
                    await SaveLaptopAd(ad, ad.title);
                }
                else if (subcategory == "Fridge - AC - Washing Machine")
                {
                    ad.subcategory = "Home-Appliances";
                    ad.subsubcategory = subsubcategory;
                }
                else if (subcategory == "Kitchen & Other Appliances")
                {
                    ad.subcategory = "Other-Electronics";
                    ad.subsubcategory = subsubcategory;
                }
                else if (subcategory == "Cameras & Accessories")
                {
                    ad.subcategory = "Cameras";
                    ad.subsubcategory = subsubcategory;
                    await SaveCameraAd(ad);
                }
                else if (subcategory == "Games & Entertainment")
                {
                    ad.subcategory = "Games";
                    ad.subsubcategory = subsubcategory;
                }
            }
            else if (category == "Cars")
            {
                ad.category = "Vehicles";
                ad.subsubcategory = subsubcategory;
                if (subcategory == "Cars")
                {
                    await SaveCarAd(ad, brand, year, fuelType, Km);
                }
                else if (subcategory == "Commercial Vehicles")
                {
                    ad.subcategory = "Commercial-Vehicles";
                }
                else if (subcategory == "Other Vehicles")
                {
                    ad.subcategory = "Other-Vehicles";
                }
                else if (subcategory == "Spare Parts")
                {
                    ad.subcategory = "Spare-Parts";
                }
            }
            else if (category == "Bikes")
            {
                ad.category = "Vehicles";
                ad.subcategory = "Bikes";
                ad.subsubcategory = subcategory;
                await SaveBikeAd(ad, brand, year, fuelType, Km);
            }
            else if (category == "Pets")
            {
                ad.category = "Animals";
                ad.subcategory = subcategory;
                ad.subsubcategory = subsubcategory;
            }
            else if (category.StartsWith("Books,"))
            {
                if (subcategory == "Books & Magazines")
                {
                    ad.category = "Education-Learning";
                    ad.subcategory = "Books & Study Material";
                    ad.subsubcategory = subsubcategory;
                }
                else
                {
                    ad.category = "Sports-Hobbies";
                    ad.subcategory = subcategory;
                    ad.subsubcategory = subsubcategory;
                }
            }
            else if (category == "Fashion") {
                if(subcategory == "Accessories")
                {
                    ad.subsubcategory = subsubcategory;
                    if (ad.title.IndexOf("watch", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ad.subcategory = "Watches";
                    }
                    else if (ad.title.IndexOf("jewellery", StringComparison.OrdinalIgnoreCase) >= 0) {
                        ad.subcategory = "Jewellery";
                    }
                    else
                    {
                        ad.subcategory = "Others in Fashion";
                    }
                }
            }else if(category == "Kids")
            {
                ad.category = "Kids-Toys";
            }
            else if(category == "Property")
            {
                if (subcategory == "Apartments")
                {
                    ad.subcategory = "apartment";
                } else if (subcategory == "Houses")
                {
                    ad.subcategory = "house";
                } else if (subcategory == "Land & Plots")
                {
                    ad.subcategory = "plot & land";
                } else if (subcategory == "PG & Roommates")
                {
                    ad.subcategory = "PG & Flatmates";
                } else if (subcategory == "Shops - Offices - Commercial Space")
                {
                    if (ad.title.IndexOf("shop", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ad.subcategory = "Shop";
                    }
                    else
                    {
                        ad.subcategory = "Office";
                    }
                } else if (subcategory == "Vacation Rentals - Guest Houses") {
                    ad.type = false; //for rent
                    ad.subcategory = "other commercial places";
                }
                else { ad.subcategory = "other commercial places"; }
               
                ad.subsubcategory = subsubcategory;
                await SaveRealEstateAd(ad, brand, area, rooms,subsubcategory);
            }
            else if(category == "Jobs")
            {
                if (subcategory == "Part time")
                {
                    ad.subcategory = "Part time Jobs";
                }else if(subcategory == "Online")
                {
                    ad.subcategory = "Online Jobs";
                }
                else { ad.subcategory = subcategory; }
                
                ad.subsubcategory = subsubcategory;
               // ad.isnegotiable = "d"; //gender does not matter
                await SaveJobAd(ad, brand, area, rooms, priceTo);
            }else if(category == "Services")
            {
                ad.subcategory = subcategory;
                ad.subsubcategory = subsubcategory;
            }
            else if(category == "Furniture")
            {
                ad.category = "Furniture & Decor";
                ad.subcategory = subcategory;
                ad.subsubcategory = subsubcategory;
            }
            await db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SaveJobAd(Ad ad, string brand, string area, string rooms, decimal? priceTo)
        {
            JobAd mob = new  JobAd();
            mob.maxPrice = priceTo;
            ad.subsubcategory = brand;
            db.SaveChanges();
            
            try
            {
                mob.salaryType = rooms[0].ToString().ToLower();
                mob.adId = ad.Id;
               
                db.JobAds.Add(mob);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();

            }
            return true;
        }

        public async Task<bool> SaveRealEstateAd(Ad ad, string brand, string area, string rooms,string rentOrSale)
        {
            House mob = new  House();
            db.SaveChanges();
            try
            {
                if(rentOrSale == "Rent")
                {
                    ad.type = false;
                }
                mob.adId = ad.Id;
                try
                {
                mob.area = decimal.Parse( area);
                }
                catch (Exception e) { }
                
                // ad.status = idstatus.status;
                db.Houses.Add(mob);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();

            }
            return true;
        }

        public async Task<bool> SaveBikeAd(Ad ad, string brand, string year, string fuelType, string km)
        {
            BikeAd mob = new  BikeAd();
            db.SaveChanges();
            try
            {
                mob.adId = ad.Id;
                try {
                    mob.year = short.Parse(year);
                }catch(Exception e) { }
                mob.bikeModel = await SaveBikesBrandModel(brand);
                // ad.status = idstatus.status;
                db.BikeAds.Add(mob);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();

            }
            return true;
        }
        public async Task<int?> SaveBikesBrandModel(String brand)
        {
           // IdStatus idStatus = new IdStatus();
          //  idStatus.status = "a";
            var company = brand;
            var model = "";
            if (company != null && company != "")
            {
                company = company.Trim();
                model = model.Trim();
            }
            if (company != null) //company != null
            {

                bool isOldBrand = db.BikeBrands.Any(x => x.brand.Equals(company));
                if (!isOldBrand)
                {
                    BikeBrand mob = new BikeBrand();
                    mob.brand = company;
                    mob.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    mob.time = DateTime.UtcNow;
                    if (company == null || company == "")
                    {
                        mob.status = "a";
                    }
                    else
                    {
                        mob.status = "p";
                    }
                    db.BikeBrands.Add(mob);
                    db.SaveChanges();

                    BikeModel mod = new BikeModel();
                    mod.model = model;
                    mod.brandId = mob.Id;
                    mod.time = DateTime.UtcNow;
                    if (model == null || model == "")
                    {
                        mod.status = "a";
                    }
                    else
                    {
                        mod.status = "p";
                    }
                    mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    db.BikeModels.Add(mod);
                    db.SaveChanges();
                    //ad.status = "p";
                    //idStatus.status = "p";
                }
                else
                {
                    bool isOldModel = db.BikeModels.Any(x => x.model.Equals(model) && x.BikeBrand.brand.Equals(company));
                    if (!isOldModel)
                    {
                        //idStatus.status = "p";
                        //ad.status = "p";
                        var brandId = db.BikeBrands.FirstOrDefault(x => x.brand.Equals(company));
                        BikeModel mod = new BikeModel();
                        mod.brandId = brandId.Id;
                        mod.model = model;
                        if (model == null || model == "")
                        {
                            mod.status = "a";
                        }
                        else
                        {
                            mod.status = "p";
                        }
                        mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        mod.time = DateTime.UtcNow;
                        db.BikeModels.Add(mod);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string s = e.ToString();
                        }
                    }
                }
                var mobileModel = db.BikeModels.FirstOrDefault(x => x.BikeBrand.brand == company && x.model == model);
                return mobileModel.Id;
            }
            return null;
        }


        public async Task<bool> SaveCarAd(Ad ad,string brand,string year, string fuelType, string km)
        {
            CarAd mob = new CarAd();
            db.SaveChanges();
            try
            {
                mob.adId = ad.Id;
            mob.fuelType = fuelType;
                km = km.Replace(",", "");
            mob.kmDriven = int.Parse(km);
            mob.year = short.Parse(year);
            IdStatus ids = await SaveCarsBrandModel(brand);
            mob.carModel = ids.id;
            // ad.status = idstatus.status;
            db.CarAds.Add(mob);
           
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();

            }
            return true;
        }
        public async Task<IdStatus> SaveCarsBrandModel(string brand)
        {
            IdStatus adStatus = new IdStatus();
            adStatus.status = "a";
            var company = brand;
            var model = "";
            if (company != null && company != "")
            {
                company = company.Trim();
                model = model.Trim();
            }
            if (true) //company != null
            {
                bool isOldBrand = db.CarBrands.Any(x => x.brand.Equals(company));

                if (!isOldBrand)
                {
                    CarBrand mob = new CarBrand();
                    mob.brand = company;
                    mob.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    mob.time = DateTime.UtcNow;
                    if (company == null || company == "")
                    {
                        mob.status = "a";
                    }
                    else
                    {
                        mob.status = "p";
                    }
                    db.CarBrands.Add(mob);
                  await  db.SaveChangesAsync();

                    CarModel mod = new CarModel();
                    mod.model = model;
                    mod.brandId = mob.Id;
                    mod.time = DateTime.UtcNow;
                    if (model == null || model == "")
                    {
                        mod.status = "a";
                    }
                    else
                    {
                        mod.status = "p";
                    }
                    mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    db.CarModels.Add(mod);
                    await db.SaveChangesAsync();
                    //ad.status = "p";
                    adStatus.status = "p";
                }
                else
                {
                    bool isOldModel = db.CarModels.Any(x => x.model.Equals(model) && x.CarBrand.brand.Equals(company));
                    if (!isOldModel)
                    {
                        adStatus.status = "a";
                        //   ad.status = "p";
                        var brandId = db.CarBrands.FirstOrDefault(x => x.brand.Equals(company));
                        CarModel mod = new CarModel();
                        mod.brandId = brandId.Id;
                        mod.model = model;
                        if (model == null || model == "")
                        {
                            mod.status = "a";
                        }
                        else
                        {
                            mod.status = "p";
                        }
                        mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        mod.time = DateTime.UtcNow;
                        db.CarModels.Add(mod);
                        try
                        {
                            await db.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            string s = e.ToString();
                        }
                    }
                }
                var mobileModel = db.CarModels.FirstOrDefault(x => x.CarBrand.brand == company && x.model == model);
                adStatus.id = mobileModel.Id;
                return adStatus;
            }
        }

        public async Task<bool> SaveLaptopAd(Ad ad, string title)
        {
            LaptopAd mob = new LaptopAd();
           // IdStatus idstatus = SaveMobileBrandModel(title);
           // mob.laptopId = idstatus.id;
            mob.Id = ad.Id;
           // ad.status = idstatus.status;
            db.LaptopAds.Add(mob);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();
                
            }
            return true;
        }
        public async Task<bool> SaveCameraAd(Ad ad)
        {
            Camera mob = new  Camera();
            // IdStatus idstatus = SaveMobileBrandModel(title);
            // mob.laptopId = idstatus.id;
            mob.adId = ad.Id;
            // ad.status = idstatus.status;
            db.Cameras.Add(mob);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();

            }
            return true;
        }

        public string detectMobileBrand(string title)
        {
            string[] ssize = title.Split(new char[0]);
            foreach (var s in ssize)
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
        public async Task<bool> SaveMobileAd(Ad ad,string title)
        {
            MobileAd mob = new MobileAd();
            IdStatus idstatus = SaveMobileBrandModel(title);
            mob.mobileId = idstatus.id;
            mob.Id = ad.Id;
            ad.status = idstatus.status;
            db.MobileAds.Add(mob);
            try {
                await db.SaveChangesAsync();
            }catch(Exception e)
            {
                string s = e.ToString();
            }
            return true;
        }
        public IdStatus SaveMobileBrandModel(string title)
        {
            try
            {
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
            catch (Exception e)
            {
                throw;
            }
        }
        public bool fileUpload(IEnumerable<string> paths, int adId)
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


                                
                               // filename1 = @"\Images\Ads\tt" + adId + "_" + count + ".jpg";
                                filename = @"\Images\Ads\" + adId + "_" + count + ".jpg";
                                var i2 = new Bitmap(image);

                                i2.Save(System.Web.HttpContext.Current.Server.MapPath(filename));
                                //apply Compression
                                using (MagickImage sprite = new MagickImage(System.Web.HttpContext.Current.Server.MapPath(filename)))
                                {
                                    var width = sprite.Width;
                                    var height = sprite.Height;
                                    sprite.Format = MagickFormat.Jpeg;
                                    sprite.Quality = 80;
                                    sprite.Resize(width, height);
                                    sprite.Write(System.Web.HttpContext.Current.Server.MapPath(filename));
                                }


                                //upload to aws
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
            for (int i = 1; i <= paths.Count(); i++)
            {
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(@"\Images\Ads\" + adId + "_" + i + ".jpg")))
                {
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(@"\Images\Ads\" + adId + "_" + i + ".jpg"));
                }
            }
            return true;
        }

        public void setWidth(string width)
        {
            Session["BrowserWidth"] = width;
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
        public async Task<object> ApplyDealkarLogo()
        {
           System.Drawing.Image imgg = System.Drawing.Image.FromFile(Server.MapPath(@"\Images\others\logo.png"));
            float f = float.Parse("1"); //0.5
            System.Drawing.Image img = SetImageOpacity(imgg, f);
            string filename = null;
            HttpPostedFileBase file = Request.Files[0];
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(file.InputStream, true, true))
            //  using (Image watermarkImage = Image.FromFile(Server.MapPath(@"\Images\others\WaterMark.png")))
            using (System.Drawing.Image watermarkImage = img)
            {
                //Image  image = ResizeImage(image12, 600, 600);
                using (Graphics imageGraphics = Graphics.FromImage(image))
                using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
                {
                    // int x = (image.Width / 2 - watermarkImage.Width / 2);
                    int x = image.Width - watermarkImage.Width - 20; ;
                    int y = 2;
                   // int y = image.Height - watermarkImage.Height - 30;
                    //   int y = (image.Height / 2 - watermarkImage.Height / 2);
                    watermarkBrush.TranslateTransform(x, y);
                    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));

                    //upload on aws
                    string extension = System.IO.Path.GetExtension(file.FileName);
                     filename = "temp" + DateTime.UtcNow.Ticks + extension;
                    var i2 = new Bitmap(image);
                //    GetImage(i2,)
                    //  image.Save(Server.MapPath(@"~\Images\Ads\" + filename));
                    System.IO.Directory.CreateDirectory(Server.MapPath(@"~\Images\others\"));

                    i2.Save(Server.MapPath(@"~\Images\others\" + filename));
                    
                    //if (System.IO.File.Exists(Server.MapPath(@"~\Images\Ads\" + filename)))
                    //{
                    //    System.IO.File.Delete(Server.MapPath(@"~\Images\Ads\" + filename));
                    //}
                }
            }
            //call compression function here
            Response.Redirect(@"~\images\others\"+ filename );
            return filename; //View("../Home/temp2", new { fileName = filename});
        }
        [Route("Apply-logo-on-images")]
        public ActionResult ApplyLogoToImages()
        {
            return View("applylogotoimage");
        }
        public  ImageSource GetImage(byte[] imageData, System.Windows.Media.PixelFormat format, int width = 640, int height = 480)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Interlace = PngInterlaceOption.On;
                encoder.Frames.Add(BitmapFrame.Create(BitmapSource.Create(width, height, 96, 96, format, null, imageData, width * format.BitsPerPixel / 8)));
                encoder.Save(memoryStream);
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = memoryStream;
                imageSource.EndInit();
                return imageSource;
            }
        }
        public bool isAdmin()
        {
            if (Request.IsAuthenticated)
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
        // GET: /Admin/
        public async Task<ActionResult> Index()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> SuperAdmin()
        {
            if (User.Identity.GetUserId() == "c1239071-cf6d-4cec-9da4-4b2871250143" || User.Identity.GetUserId() == "7234b5b0-2cb5-4d4a-bc18-98e17c460221")
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Models()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> ManageUsers()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Ads()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> LatestAds()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Feedback()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Location()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        // GET: /Admin/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // GET: /Admin/Create
        public ActionResult Create()
        {
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId");
            return View();
        }

        // POST: /Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            return View(ad);
        }

        // GET: /Admin/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            return View(ad);
        }

        // POST: /Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            return View(ad);
        }

        // GET: /Admin/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: /Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            db.Ads.Remove(ad);
            await db.SaveChangesAsync();
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
