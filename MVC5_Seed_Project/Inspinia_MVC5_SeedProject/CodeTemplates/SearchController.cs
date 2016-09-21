using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

using System.Threading.Tasks;

using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;


namespace Inspinia_MVC5_SeedProject.CodeTemplates
{

    public class SearchController : Controller
    {
        private Entities db = new Entities();

        [HttpGet]
        public async Task<JsonResult> GetSimilarData(int id, string tags, string category, string subcategory, string subsubcategory)
        {
            var cit = System.Web.HttpContext.Current.Session["City"];
            string city = cit.ToString();
            string pp = null;
            if (subcategory == "Books ")
            {
                subcategory = "Books & Study Material";
            }
            else if (subcategory == "Others in Education ")
            {
                subcategory = "Others in Education & Learning";
            }
            else if (subcategory == "Gym ")
            {
                subcategory = "Gym & Fitness";
            }
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null || tags == "null" || tags == "undefined")
            {
                var temp1 = from ad in db.Ads
                            where (ad.Id != id && ad.category.Equals(category) && (subcategory == null || subcategory == "null" || subcategory == "undefined" || ad.subcategory.Equals(subcategory)) && ad.status.Equals("a") && (city == null || city == "All Pakistan" || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
                            orderby ad.time descending
                            select new
                            {
                                title = ad.title,
                                postedById = ad.AspNetUser.Id,
                                postedByName = ad.AspNetUser.Email,
                                description = ad.description,
                                id = ad.Id,
                                time = ad.time,
                                islogin = islogin,
                                isNegotiable = ad.isnegotiable,
                                price = ad.price,
                                reportedCount = ad.Reporteds.Count,
                                isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                                category = ad.category,
                                subcategory = ad.subcategory,
                                views = ad.views,
                                condition = ad.condition,
                                savedCount = ad.SaveAds.Count,
                                adTags = from tag1 in ad.AdTags.ToList()
                                         select new
                                         {
                                             id = tag1.tagId,
                                             name = tag1.Tag.name,
                                             //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                             //info = tag.Tag.info,
                                         },
                                bid = from biding in ad.Bids.ToList()
                                      select new
                                      {
                                          price = biding.price,
                                      },
                                adImages = from image in ad.AdImages.ToList()
                                           select new
                                           {
                                               imageExtension = image.imageExtension,
                                           },
                                location = new
                                {
                                    cityName = ad.AdsLocation.City.cityName,
                                    cityId = ad.AdsLocation.cityId,
                                    popularPlaceId = ad.AdsLocation.popularPlaceId,
                                    popularPlace = ad.AdsLocation.popularPlace.name,
                                    exectLocation = ad.AdsLocation.exectLocation,
                                },

                            };
                return Json(temp1.Take(8),JsonRequestBehavior.AllowGet);
            }
            string[] tagsArray = null;
            if (tags != null)
            {
                tagsArray = tags.Split(',');
            }



            var temp = from ad in db.Ads
                       where (ad.category.Equals(category) && (subcategory == null || subcategory == "undefined" || ad.subcategory.Equals(subcategory)) && (!tagsArray.Except(ad.AdTags.Select(x => x.Tag.name)).Any()) && ad.status.Equals("a") && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))

                       orderby ad.time descending
                       select new
                       {
                           title = ad.title,
                           postedById = ad.AspNetUser.Id,
                           postedByName = ad.AspNetUser.Email,
                           description = ad.description,
                           id = ad.Id,
                           time = ad.time,
                           islogin = islogin,
                           isNegotiable = ad.isnegotiable,
                           price = ad.price,
                           reportedCount = ad.Reporteds.Count,
                           isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                           category = ad.category,
                           subcategory = ad.subcategory,
                           views = ad.views,
                           condition = ad.condition,
                           savedCount = ad.SaveAds.Count,
                           adTags = from tag1 in ad.AdTags.ToList()
                                    select new
                                    {
                                        id = tag1.tagId,
                                        name = tag1.Tag.name,
                                        //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                        //info = tag.Tag.info,
                                    },
                           bid = from biding in ad.Bids.ToList()
                                 select new
                                 {
                                     price = biding.price,
                                 },
                           adImages = from image in ad.AdImages.ToList()
                                      select new
                                      {
                                          imageExtension = image.imageExtension,
                                      },
                           location = new
                           {
                               cityName = ad.AdsLocation.City.cityName,
                               cityId = ad.AdsLocation.cityId,
                               popularPlaceId = ad.AdsLocation.popularPlaceId,
                               popularPlace = ad.AdsLocation.popularPlace.name,
                               exectLocation = ad.AdsLocation.exectLocation,
                           },

                       };
            return Json(temp.Take(8),JsonRequestBehavior.AllowGet);
        }

        // GET: /Search/Details/5

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

        // GET: /Search/Create
        public ActionResult Create()
        {

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");

            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation");

            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId");

            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand");

            ViewBag.Id = new SelectList(db.CarAds, "adId", "color");

            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating");

            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom");

            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color");

            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color");

            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification");

            return View();
        }

        // POST: /Search/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views,name,phoneNumber,subsubcategory")] Ad ad)

        {
            if (ModelState.IsValid)
            {

                db.Ads.Add(ad);

                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);

            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);

            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);

            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);

            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);

            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);

            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);

            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);

            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);

            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);

            return View(ad);
        }

        // GET: /Search/Edit/5

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

            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);

            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);

            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);

            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);

            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);

            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);

            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);

            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);

            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);

            return View(ad);
        }

        // POST: /Search/Edit/5

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Edit([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views,name,phoneNumber,subsubcategory")] Ad ad)

        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;

                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);

            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);

            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);

            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);

            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);

            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);

            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);

            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);

            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);

            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);

            return View(ad);
        }

        // GET: /Search/Delete/5

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

        // POST: /Search/Delete/5
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
