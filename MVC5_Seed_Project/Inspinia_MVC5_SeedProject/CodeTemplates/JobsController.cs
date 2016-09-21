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
    public class JobsController : Controller
    {
        private Entities db = new Entities();
        ElectronicsController electronicController = new ElectronicsController();
        // GET: /Jobs/
        [Route("Jobs", Name = "Jobs")]
        public async Task<ActionResult> Index(string q = "", string tags = null, int minSalary = 0, int maxSalary = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategories = new string[] { "Customer Service", "It", "Online Jobs", "Marketing", "Advertising & PR", "Sales", "Clerical & Administration", "Human Resources", "Education","Hotels & Tourism","Accounting & Finance","Manufacturing", "Part time Jobs", "Other Jobs" };

            var result = await searchResults("Jobs", null, q, tags, minSalary, maxSalary, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View(viewModel);
        }
        [Route("Customer-Service", Name = "Customer-Service")]
        public async Task<ActionResult> CustomerService(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Customer Service";
            var result = await searchResults("Jobs", "Customer Service", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("It", Name = "It")]
        public async Task<ActionResult> It(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "It";
            var result = await searchResults("Jobs", "It", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Online-Jobs", Name = "Online-Jobs")]
        public async Task<ActionResult> OnlineJobs(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Online Jobs";
            var result = await searchResults("Jobs", "Online Jobs", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Marketing", Name = "Marketing")]
        public async Task<ActionResult> marketing(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Marketing";
            var result = await searchResults("Jobs", "Marketing", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Advertising-PR", Name = "Advertising-PR")]
        public async Task<ActionResult> avdertisingPR(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Advertising & PR";
            var result = await searchResults("Jobs", "Advertising & PR", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Sales", Name = "Sales")]
        public async Task<ActionResult> Sales(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Sales";
            var result = await searchResults("Jobs", "Sales", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Clerical-Administration", Name = "Clerical-Administration")]
        public async Task<ActionResult> ClericalAdministration(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Clerical & Administration";
            var result = await searchResults("Jobs", "Clerical & Administration", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Human-Resources", Name = "Human-Resources")]
        public async Task<ActionResult> HumanResources(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Human Resources";
            var result = await searchResults("Jobs", "Human Resources", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Education", Name = "Education")]
        public async Task<ActionResult> Education(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Education";
            var result = await searchResults("Jobs", "Education", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Hotels-Tourism", Name = "Hotels-Tourism")]
        public async Task<ActionResult> HotelsTourism(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Hotels & Tourism";
            var result = await searchResults("Jobs", "Hotels & Tourism", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Accounting-Finance", Name = "Accounting-Finance")]
        public async Task<ActionResult> AccountingFinance(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Accounting & Finance";
            var result = await searchResults("Jobs", "Accounting & Finance", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Manufacturing", Name = "Manufacturing")]
        public async Task<ActionResult> Manufacturing(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Manufacturing";
            var result = await searchResults("Jobs", "Manufacturing", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Part-time-Jobs", Name = "Part-time-Jobs")]
        public async Task<ActionResult> ParttimeJobs(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Part time Jobs";
            var result = await searchResults("Jobs", "Part time Jobs", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Other-Jobs", Name = "Other-Jobs")]
        public async Task<ActionResult> OtherJobs(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Jobs";
            ViewBag.subcategory = "Other Jobs";
            var result = await searchResults("Jobs", "Other Jobs", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count,
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }


        public static async Task<List<ListAdView>> searchResults(string category, string subcategory, string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, string city = null, string pp = null, int fixedMaxPrice = 10000, string condition = "n", string subsubcategory = null)
        {
            Entities db = new Entities();
            if (subcategory == "Books ")
            {
                subcategory = "Books & Study Material";
            }
            else if (subcategory == "Others in Education ")
            {
                subcategory = "Others in Education & Learning";
            }
            else if (subcategory == "Games ")
            {
                subcategory = "Games";
            }
            if (tags == null || tags == "")
            {
                List<ListAdView> ads = await (from ad in db.Ads
                                              where ((q == null || q == "" || ad.title.Contains(q) || ad.title.StartsWith(q)) && (category == null || category == "" || ad.category.Equals(category)) && (subcategory == null || subcategory == "" || subcategory == "undefined" || ad.subcategory.Equals(subcategory)) && (subsubcategory == null || subsubcategory == "" || subsubcategory == "undefined" || ad.subsubcategory.Equals(subsubcategory)) && ad.status.Equals("a") && (minPrice == 0 || ad.price > minPrice) && (maxPrice == fixedMaxPrice || ad.price < maxPrice) && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
                                              orderby ad.time descending
                                              select new ListAdView()
                                              {
                                                  Id = ad.Id,
                                                  name = ad.name,
                                                  phoneNumber = ad.phoneNumber,
                                                  category = ad.category,
                                                  postedBy = ad.postedBy,
                                                  description = ad.description,
                                                  time = ad.time,
                                                  price = ad.price,
                                                  maxPrice = ad.JobAd.maxPrice,
                                                  isnegotiable = ad.isnegotiable,
                                                  type = ad.type,
                                                  condition = ad.condition,
                                                  status = ad.status,
                                                  views = ad.views,
                                                  subcategory = ad.subcategory,
                                                  title = ad.title,
                                                  //   subsubcategory = ad.subsubcategory,
                                                  AdImages = ad.AdImages.ToList(),
                                                  AdsLocation = ad.AdsLocation,
                                                  AdTags = ad.AdTags.ToList(),
                                                  bidsCount = ad.Bids.Count,
                                                  maxBid = ad.Bids.OrderByDescending(x => x.price).FirstOrDefault()
                                              }).ToListAsync();
                return ads;
            }
            else
            {
                string[] tagsArray = null;
                if (tags != null)
                {
                    tagsArray = tags.Split(',');
                }
                List<ListAdView> ads = await (from ad in db.Ads
                                              where (ad.category.Equals(category) && (!tagsArray.Except(ad.AdTags.Select(x => x.Tag.name)).Any()) && (subcategory == null || subcategory == "undefined" || ad.subcategory.Equals(subcategory)) && ad.status.Equals("a") && (minPrice == 0 || ad.price > minPrice) && (maxPrice == fixedMaxPrice || ad.price < maxPrice) && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
                                              orderby ad.time descending
                                              select new ListAdView()
                                              {
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
                                                  title = ad.title,
                                                  //   subsubcategory = ad.subsubcategory,
                                                  AdImages = ad.AdImages.ToList(),
                                                  AdsLocation = ad.AdsLocation,
                                                  AdTags = ad.AdTags.ToList(),
                                                  bidsCount = ad.Bids.Count,
                                                  maxBid = ad.Bids.OrderByDescending(x => x.price).FirstOrDefault()
                                              }).ToListAsync();
                return ads;
            }
        }

        // GET: /Jobs/Details/5
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

        // GET: /Jobs/Create
        public ActionResult Create()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: /Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( Ad ad)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                   await SaveAd(ad);
                    electronicController.SaveTags(Request["tags"], ad);
                   await SaveSkills(Request["skills"],  ad);
                    electronicController.PostAdByCompanyPage(ad.Id);
                    electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                   await db.SaveChangesAsync();

                    return RedirectToAction("Details","Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly( ad.title) });
                }
            }
            return View(ad);
        }
        public async Task<object> SaveSkills(string s,Ad ad, bool update = false)
        {
            if (update)
            {
                var adid = ad.Id;
                var adtags = db.JobSkills.Where(x => x.adId.Equals(adid)).ToList();
                foreach (var cc in adtags)
                {
                    db.JobSkills.Remove(cc);
                }
                await db.SaveChangesAsync();
            }
            if(s == "" || s == null)
            {
                return true;
            }
            string[] values = s.Split(',');
            Skill[] tags = new Skill[values.Length];
            JobSkill[] qt = new JobSkill[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
                string ss = values[i];
                if (ss != "")
                {
                    var data = db.Skills.FirstOrDefault(x => x.name.Equals(ss, StringComparison.OrdinalIgnoreCase));

                    tags[i] = new Skill();
                    if (data != null)
                    {
                        tags[i].Id = data.Id;
                    }
                    else
                    {
                        tags[i].name = values[i];
                        tags[i].time = DateTime.UtcNow;
                        tags[i].addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        db.Skills.Add(tags[i]);
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
                    qt[i] = new JobSkill();
                    qt[i].adId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.JobSkills.Add(qt[i]);
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
            return true;
        }
        public async Task<bool> SaveAd(Ad ad,bool update = false)
        {
            JobAd jobAd = new JobAd();
            if(update){
              //  jobAd =await db.JobAds.FindAsync(ad.Id);
                jobAd.adId = ad.Id;
            }
            //else{
            //    jobAd = new JobAd();
            //}
             
            ad.status = "a";
            ad.category = "Jobs";
            var seats = System.Web.HttpContext.Current.Request["seats"];
            if ( seats != null && seats != "")
            {
                jobAd.seats = int.Parse(seats);
            } 
            jobAd.qualification = System.Web.HttpContext.Current.Request["qualification"];
            jobAd.exprience = System.Web.HttpContext.Current.Request["exprience"];
            if(jobAd.exprience != "" && jobAd.exprience != null)
            {
                jobAd.exprience = jobAd.exprience.Trim();
            }
            if(System.Web.HttpContext.Current.Request["salary"] != null && System.Web.HttpContext.Current.Request["salary"] != "")
            {
                ad.price = int.Parse(System.Web.HttpContext.Current.Request["salary"]);
            }
            if (System.Web.HttpContext.Current.Request["maxSalary"] != null && System.Web.HttpContext.Current.Request["maxSalary"] != "")
            {
               jobAd.maxPrice = int.Parse(System.Web.HttpContext.Current.Request["maxSalary"]);
            }
            ad.isnegotiable = System.Web.HttpContext.Current.Request["gender"];
            var skills = System.Web.HttpContext.Current.Request["skills"];
            jobAd.careerLevel = System.Web.HttpContext.Current.Request["careerLevel"];
            var lastDateToApply = System.Web.HttpContext.Current.Request["lastDateToApply"];
            if (lastDateToApply != null && lastDateToApply != "") 
            {
                jobAd.lastDateToApply = DateTime.Parse(lastDateToApply);
            }
            ad.condition = System.Web.HttpContext.Current.Request["shift"];
            jobAd.salaryType = System.Web.HttpContext.Current.Request["salaryType"];
            
           // ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
            ad.postedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();



            if (!update)
            {
                jobAd.category1 = System.Web.HttpContext.Current.Request["category"];
                ad.subcategory = System.Web.HttpContext.Current.Request["subcategory"];
                ad.time = DateTime.UtcNow;

                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                jobAd.adId = ad.Id;
                db.JobAds.Add(jobAd);
                await db.SaveChangesAsync();
            }
            else if (update)
            {
                ad.time = DateTime.Parse(System.Web.HttpContext.Current.Request["time"]);
                jobAd.category1 = System.Web.HttpContext.Current.Request["category"];
                ad.subcategory = System.Web.HttpContext.Current.Request["subcategory"];
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                db.Entry(jobAd).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            

            

            return true;
        }
        // GET: /Jobs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Request.IsAuthenticated)
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
            Ad ad1 = await db.Ads.FindAsync(id);
            return RedirectToAction("Details", "Electronics", new { id = ad1.Id, title = ElectronicsController.URLFriendly(ad1.title) });

        }

        // POST: /Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    var ab = Request["postedBy"];
                    var iddd = User.Identity.GetUserId();
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                       await SaveAd(ad, true);

                       
                        electronicController.SaveTags(Request["tags"],  ad, "update");
                       await SaveSkills(Request["skills"],ad,true);
                        //asp.Ads.Add(ad);

                        db.Entry(ad).State = EntityState.Modified;
                        db.SaveChanges();

                        electronicController.PostAdByCompanyPage(ad.Id, true);
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
                        electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly( ad.title) });
                    }
                }
              //  return View("Edit", ad);
            }
            return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
        }

        // GET: /Jobs/Delete/5
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

        // POST: /Jobs/Delete/5
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
