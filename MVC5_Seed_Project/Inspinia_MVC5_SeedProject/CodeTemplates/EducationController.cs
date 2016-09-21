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
using Inspinia_MVC5_SeedProject.CodeTemplates;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class EducationController : Controller
    {
        private Entities db = new Entities();

        // GET: /Education/
        public async Task<ActionResult> Index()
        {
            var ads = db.Ads.Include(a => a.AdsLocation).Include(a => a.LaptopAd).Include(a => a.MobileAd).Include(a => a.AspNetUser).Include(a => a.CompanyAd).Include(a => a.JobAd).Include(a => a.CarAd).Include(a => a.BikeAd).Include(a => a.House).Include(a => a.Camera);
            return View(await ads.ToListAsync());
        }
     //   [Route("Education-Learning")]
     ////  [Route("Services")]
     //   //[Route("abcdef")]
     //   public ActionResult Categories()
     //   {
     //       ViewBag.category = "Education-Learning";
     //       return View();
     //      // return RedirectToAction("notFound","Home");
     //   }
        [Route("Electronics",Name ="Electronics")]
        public ActionResult Categories1()
        {
            ViewBag.category = "Electronics";

            return View("Categories");
        }
        [Route("Fashion",Name = "Fashion")]
        public async Task<ActionResult> homeL(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Fashion";
            // ViewBag.subcategory = "Beauty products";
            var subcategories = new string[] { "Watches", "Clothes", "Footwear",/* "Jewellery", "Beauty Products",*/ "Others in Fashion" };
            ViewBag.subcategories = subcategories;
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Fashion", null, q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }

        //[Route("Books-Study-Material")]
        //public ActionResult abc()
        //{
        //    ViewBag.category = "Education-Learning";
        //    ViewBag.subcategory = "Books & Study Material";
        //    return View("Index");
        //}

        [Route("Musical-Instruments",Name = "Musical-Instruments")]
        //public ActionResult abc4()
        //{
        //    ViewBag.category = "Education-Learning";
        //    ViewBag.subcategory = "Musical Instruments";

        //    return View("Index");
        //}
        public async Task<ActionResult> abc4(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Education-Learning";
            ViewBag.subcategory = "Musical Instruments";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Animals", null, q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Sports-Equipment")]
        public ActionResult abc3()
        {
            ViewBag.category = "Education-Learning";
            ViewBag.subcategory = "Sports Equipment";
            return View("Index");
        }
        [Route("Gym-Fitness")]
        public ActionResult abc2()
        {
            ViewBag.category = "Education-Learning";
            ViewBag.subcategory = "Gym & Fitness";
            return View("Index");
        }
        [Route("other-hobbies")]
        public ActionResult abc1()
        {
            ViewBag.category = "Education-Learning";
            ViewBag.subcategory = "other hobbies";
            return View("Index");
        }
        [Route("Watches",Name ="Watches")]
        public async Task<ActionResult> watches(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Watches";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Fashion", "Watches", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }
        
        [Route("Clothes",Name ="Clothes")]
        public async Task<ActionResult> clothes(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Clothes";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Fashion", "Clothes", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }
        [Route("Footwear",Name ="Footwear")]
        public async Task<ActionResult> Footwear(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Footwear";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Fashion", "Footwear", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }
        [Route("Jewellery",Name ="Jewellery")]
        public async Task<ActionResult> jewellery(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Jewellery";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Fashion", "Jewellery", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }
        
        [Route("Baby-Products",Name ="Baby-Products")]
        public ActionResult babyProducts()
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Baby Products";
            return View("Index");
        }
        [Route("Beauty-products",Name ="Beauty-products")]
        public async Task<ActionResult> healthBeautyProducts(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Beauty products";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Fashion", "Beauty products", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }
        [Route("Furniture")]
        public ActionResult furniture()
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Furniture";
            return View("Index");
        }
        [Route("HouseHold")]
        public ActionResult houseHold()
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "HouseHold";
            return View("Index");
        }
        [Route("Home-Decoration")]
        public ActionResult HomeDecoration()
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "Home Decoration";
            return View("Index");
        }
        [Route("Others-in-Fashion",Name = "Others-in-Fashion")]
        public async Task<ActionResult> othersInHome(string q = "", string tags = null, int minPrice = 0, int maxPrice = 50000, bool accessories = false, string condition = null, int? page = null)
        {
            ViewBag.category = "Fashion";
            ViewBag.subcategory = "others in Fashion";
            string city = Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString();
            var result = await StudyController.searchResults("Fashion", "Others in Fashion", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Study/Index", viewModel);
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Education/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);
            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);
            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);
            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);
            return View(ad);
        }

        // GET: /Education/Edit/5
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
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);
            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);
            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);
            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);
            return View(ad);
        }

        // POST: /Education/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);
            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);
            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);
            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);
            return View(ad);
        }

        // GET: /Education/Delete/5
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

        // POST: /Education/Delete/5
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
