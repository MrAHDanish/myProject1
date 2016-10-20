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
using System.Text;
using System.Text.RegularExpressions;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class StudyController : Controller
    {
        public Entities db = new Entities();
        //public bool insertParam(string key, string value)
        //{
        //   // key = HttpServerUtility.UrlTokenEncode(key); value = encodeURI(value);

        //    var kvp = document.location.search.substr(1).split('&');

        //    var i = kvp.length; var x; while (i--)
        //    {
        //        x = kvp[i].split('=');

        //        if (x[0] == key)
        //        {
        //            x[1] = value;
        //            kvp[i] = x.join('=');
        //            break;
        //        }
        //    }

        //    if (i < 0) { kvp[kvp.length] = [key, value].join('='); }

        //    //this will reload the page, it's likely better to store this until finished
        //    document.location.search = kvp.join('&');
        //}
        // GET: /Study/
        [Route("Services", Name = "Services")]
        public async Task<ActionResult> Services(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            string city = null;
            string pp = null;
            ViewBag.category = "Services";
            ViewBag.subcategories = new string[] { "Education & Classes", "Web Development", "Electronics & Computer Repair", "Maids & Domestic Help", "Health & Beauty", "Movers & Packers", "Drivers & Taxi", "Event Services", "Other Services" };

            var result = await searchResults("Services", null, q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Property", Name = "Property")]
        public async Task<ActionResult> Property(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            string city = null;
            string pp = null;
            ViewBag.category = "Property";
            ViewBag.subcategories = new string[] { "Apartment", "House", "Plot & Land", "Shop", "Office", "PG & Flatmates", "other commercial places" };

            var result = await searchResults("Property", null, q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("House", Name = "House")]
        public async Task<ActionResult> house(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Property";
            ViewBag.subcategory = "House";
            var result = await searchResults("Property", "House", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Apartment", Name = "Apartment")]
        public async Task<ActionResult> apartment(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Property";
            ViewBag.subcategory = "Apartment";
            var result = await searchResults("Property", "Apartment", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Plot-Land", Name = "Plot-Land")]
        public async Task<ActionResult> plotland(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Property";
            ViewBag.subcategory = "Plot & Land";
            var result = await searchResults("Property", "Plot & Land", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Shop", Name = "Shop")]
        public async Task<ActionResult> shop(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Property";
            ViewBag.subcategory = "Shop";
            var result = await searchResults("Property", "Shop", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Office", Name = "Office")]
        public async Task<ActionResult> Office(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Property";
            ViewBag.subcategory = "Office";
            var result = await searchResults("Property", "Office", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Other-commercial-places", Name = "Other-commercial-places")]
        public async Task<ActionResult> Othercommericalplaces(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Property";
            ViewBag.subcategory = "Other commercial places";
            var result = await searchResults("Property", "Other commercial places", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("PG-Flatmates", Name = "PG-Flatmates")]
        public async Task<ActionResult> pgflatmates(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Property";
            ViewBag.subcategory = "PG & Flatmates";
            var result = await searchResults("Property", "PG & Flatmates", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        //  [Route("Search/{s?}")]
        [Route("q/{s?}", Name = "q")]
        public async Task<ActionResult> Search(string s = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            string city = null;
            string pp = null;
            ViewBag.category = null;
            ViewBag.subcategories = null;
            var result = await searchResults(null, null, s, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("../Home/Search", viewModel);
        }
        [Route("Education-Classes", Name = "Education-Classes")]
        public async Task<ActionResult> educationClasses(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Education & Classes";
            var result = await searchResults("Services", "Education & Classes", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }   

        [Route("Education-Learning",Name = "Education-Learning")]
        public async Task<ActionResult> EducationLearning(string q="",string tags = null,int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            string city = null;
            string pp = null;
            ViewBag.category = "Education & Learning"; 
            var subcategories = new string[] { "Books & Study Material"/*, "Home Tuitions", "Classes", "Courses", "Others in Education & Learning"*/ };
            ViewBag.subcategories = subcategories;
            var result = await searchResults("Education-Learning", null, q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            // = await StudyController.searchResults("Fashion", "Beauty products", q, tags, minPrice, maxPrice, city, null, 50000, condition);

            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Books-Study-Material",Name ="Books-Study-Material")]
        public async Task<ActionResult> Books(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Education & Learning";
            //var subcategories = new string[] { "Books & Study Material", "Home Tuitions", "Classes", "Courses", "Others in Education & Learning" };
            //ViewBag.subcategories = subcategories;
            ViewBag.subcategory = "Books & Study Material";
            var result = await searchResults("Education-Learning", "Books & Study Material", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        //change hard code values to fixed min price and fixed max price
        public static async Task<List<ListAdView>> searchResults(string category, string subcategory, string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, string city = null, string pp = null,int fixedMaxPrice = 10000 ,string condition = "n",string subsubcategory = null)
        {
            Entities db = new Entities();
            


            if(tags != null && tags != "")
            {
                q = q + " "+ tags;
            }
             q = Regex.Replace(q, @"\b(" + string.Join("|", "sale") + @")\b", string.Empty,RegexOptions.IgnoreCase);
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
            q = Regex.Replace(q, @"\b(" + string.Join("|", "in") + @")\b", string.Empty, RegexOptions.IgnoreCase);
         //   q = Regex.Replace(q, @"\b(" + string.Join("|", "or") + @")\b", string.Empty, RegexOptions.IgnoreCase);
            //  q =  Regex.Replace(q, "[ a | i | the |?|want | to | buy | sell | sale| for | is ]",string.Empty, RegexOptions.IgnoreCase);

            string[] q_array = q.Split(' ');
            
            if(subcategory == "Books ")
            {
                subcategory = "Books & Study Material";
            }else if(subcategory == "Others in Education ")
            {
                subcategory = "Others in Education & Learning";
            }
            else if(subcategory == "Games ")
            {
                subcategory = "Games";
            }
          //  if (tags == null || tags == "")
            {
                //var data = Select * from 
                List<ListAdView> ads = (from ad in db.Ads
                                              where ((q == null || q == "" || q_array.Any(x=>ad.title.Contains(x))) && (category == null || category == "" || ad.category.Equals(category)) && (subcategory == null || subcategory == "" || subcategory == "undefined" || ad.subcategory.Equals(subcategory)) && (subsubcategory == null || subsubcategory == "" || subsubcategory == "undefined" || ad.subsubcategory.Equals(subsubcategory)) && ad.status.Equals("a") && (minPrice == 0 || ad.price > minPrice) && (maxPrice == fixedMaxPrice || ad.price < maxPrice) && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                              }).ToList();
                return ads;
            }
            //else
            {
                string[] tagsArray = null;
                if (tags != null)
                {
                    tagsArray = tags.Split(',');
                }
                List<ListAdView> ads =  (from ad in db.Ads
                                               where ((q == null || q == "" || q_array.Any(x => ad.title.Contains(x))) && (!tagsArray.Except(ad.AdTags.Select(x => x.Tag.name)).Any()) && (category == null || category == "" || ad.category.Equals(category)) && (subcategory == null || subcategory == "" || subcategory == "undefined" || ad.subcategory.Equals(subcategory)) && (subsubcategory == null || subsubcategory == "" || subsubcategory == "undefined" || ad.subsubcategory.Equals(subsubcategory)) && ad.status.Equals("a") && (minPrice == 0 || ad.price > minPrice) && (maxPrice == fixedMaxPrice || ad.price < maxPrice) && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                                  maxBid = ad.Bids.OrderByDescending(x=>x.price).FirstOrDefault()
                                              }).ToList();
                return ads;
            }
        }

        [Route("Classes",Name ="Classes")]
        public async Task<ActionResult> classes(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Education & Learning";
            ViewBag.subcategory = "Classes";
            var result = await searchResults("Education-Learning", "Classes", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Courses",Name ="Courses")]
        public async Task<ActionResult> Courses(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Education & Learning";
            ViewBag.subcategory = "Courses";
            var result = await searchResults("Education-Learning", "Courses", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Home-Tuitions",Name ="Home-Tuitions")]
        public async Task<ActionResult> Hometuition(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Education & Learning";
            ViewBag.subcategory = "Home Tuitions";
            var result = await searchResults("Education-Learning", "Home Tuitions", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Others-in-Education-Learning",Name = "Others-in-Education-Learning")]
        public async Task<ActionResult> others(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Education & Learning";
            ViewBag.subcategory = "Others in Education & Learning";
            var result = await searchResults("Education-Learning", "Others in Education & Learning", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }

        //services
        [Route("Web-Development", Name = "Web-Development")]
        public async Task<ActionResult> webdevelopment(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Web Development";
            var result = await searchResults("Services", "Web Development", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }           // GET: /Study/Details/5
        [Route("Electronics-Computer-Repair", Name = "Electronics-Computer-Repair")]
        public async Task<ActionResult> electronicsComputerRepair(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Electronics & Computer Repair";
            var result = await searchResults("Services", "Electronics & Computer Repair", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }
        [Route("Maids-Domestic-Help", Name = "Maids-Domestic-Help")]
        public async Task<ActionResult> MaidsDomesticHelp(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Maids  & Domestic Help";
            var result = await searchResults("Services", "Maids & Domestic Help", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }           // GET: /Study/Details/5

        [Route("Health-Beauty", Name = "Health-Beauty")]
        public async Task<ActionResult> HealthBeauty(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Health & Beauty";
            var result = await searchResults("Services", "Health & Beauty", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }           // GET: /Study/Details/5

        [Route("Movers-Packers", Name = "Movers-Packers")]
        public async Task<ActionResult> MoversPackers(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Movers & Packers";
            var result = await searchResults("Services", "Movers & Packers", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }           // GET: /Study/Details/5

        [Route("Drivers-Taxi", Name = "Drivers-Taxi")]
        public async Task<ActionResult> DriversTaxi(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Drivers & Taxi";
            var result = await searchResults("Services", "Drivers & Taxi", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }           // GET: /Study/Details/5

        [Route("Event-Services", Name = "Event-Services")]
        public async Task<ActionResult> EventServices(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Event Services";
            var result = await searchResults("Services", "Event Services", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }           // GET: /Study/Details/5
        [Route("Other-Services", Name = "Other-Services")]
        public async Task<ActionResult> otherServices(string q = "", string tags = null, int minPrice = 0, int maxPrice = 10000, int? page = null)
        {
            ViewBag.category = "Services";
            ViewBag.subcategory = "Other Services";
            var result = await searchResults("Services", "Other Services", q, tags, minPrice, maxPrice, Session["City"] == null || Session["City"].ToString() == "All Pakistan" ? null : Session["City"].ToString());
            var pager = new Pager(result.Count(), page);

            var viewModel = new ListViewModel
            {
                ItemsCount = result.Count(),
                Items = result.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            return View("Index", viewModel);
        }           // GET: /Study/Details/5

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

        // GET: /Study/Create
        public ActionResult Create()
        {
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation");
            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId");
            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand");
            ViewBag.Id = new SelectList(db.CarAds, "adId", "color");
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating");
            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom");
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification");
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color");
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color");
            return View();
        }

        // POST: /Study/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views,name,phoneNumber")] Ad ad)
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
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // GET: /Study/Edit/5
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
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // POST: /Study/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views,name,phoneNumber")] Ad ad)
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
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // GET: /Study/Delete/5
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

        // POST: /Study/Delete/5
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
