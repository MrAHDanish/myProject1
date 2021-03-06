﻿using System;
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
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Inspinia_MVC5_SeedProject.Models;
using Microsoft.Owin.Host.SystemWeb;
//below are from stackoverflow
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Inspinia_MVC5_SeedProject.Controllers
{
    
    public class UserController : ApiController
    {
        private Entities db = new Entities();
        public UserController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {

        }

        public UserController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }
        // GET api/User
        public IQueryable<AspNetUser> GetAspNetUsers()
        {
            return db.AspNetUsers;
        }
        public async Task<IHttpActionResult> GetLoginUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(System.Web.HttpContext.Current.User.Identity.GetUserId());
            }
            return Ok("Visitor");
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetPayments()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var ret = from user in db.AspNetUsers
                          where user.Id.Equals(userId)
                          select new
                          {
                              id = user.Id,
                              name = user.Email,
                              email = user.UserName,
                              phoneNumber = user.PhoneNumber,
                              phoneNumberConfirmed = user.PhoneNumberConfirmed
                          };
                return Ok(ret);
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetLoginUserId12(string email)
        {
            var ret = db.AspNetUsers.FirstOrDefault(x => x.UserName.Equals(email)).Id;
            return Ok(ret);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Getuser1(string id)
        {
                var ret = await (from u in db.AspNetUsers
                                 where (u.Id.Equals(id))
                                 select new
                                 {
                                     id = u.Id,
                                     name = u.Email,
                                     email = u.UserName,
                                     dpExtension = u.dpExtension,
                                 }).FirstOrDefaultAsync();
                return Ok(ret);
       
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetSimpleUser(string id)
        {
                var ret =await (from u in db.AspNetUsers
                          where (u.Id.Equals(id))
                          select new
                          {
                              id = u.Id,
                              name = u.Email,
                              email = u.UserName,
                              dpExtension = u.dpExtension,
                          }).FirstOrDefaultAsync();
                return Ok(ret);
           
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            if (user.SecurityStamp == null)
            {
                user.SecurityStamp = "abcdjadfasd34ads";
            }
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Extracted the part that has been changed in SignInAsync for clarity.

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(string email, string password = "aa")
        {

            var ab = email.Split('@');

            var user = new ApplicationUser() { UserName = email };
            user.Email = ab[0];
            UserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = -1
            };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                try
                {
                    await SignInAsync(user, isPersistent: true);
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                var id = user.Id;
                var data = await db.AspNetUsers.FindAsync(id);
                if (password == "aa")
                {
                    data.IsPasswordSaved = false;
                }
                else
                {
                    data.IsPasswordSaved = true;
                }
                data.hideEmail = true;
                data.hidePhoneNumber = true;
                data.hideDateOfBirth = true;
                data.since = DateTime.UtcNow;
                data.status = "active";
                data.EmailConfirmed = false;
                db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();


                return Ok("Done");
            }
            return BadRequest();
        }
        

        [HttpPost]
        public async Task<IHttpActionResult> login(string email,string password)
        {
            var data = db.AspNetUsers.First(x => x.UserName.Equals(email));
            bool isSaved = true;
            try
            {
                isSaved = (bool)data.IsPasswordSaved;
            }
            catch (Exception e)
            {
                isSaved = false;
            }
            if (!isSaved)
            {
                UserManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = -1
                };
                IdentityResult result = await UserManager.ChangePasswordAsync(data.Id, "aa", password);
                if (!result.Succeeded)
                {
                    return Ok("Error");
                }
                data.IsPasswordSaved = true;
                await db.SaveChangesAsync();
            }
            var user = await UserManager.FindAsync(email, password);

            if (user != null)
            {
                await SignInAsync(user, true);
                return Ok("Done");
            }
            else
            {
                return BadRequest("Error");
            }
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> CheckEmail(string email){
            if (!User.Identity.IsAuthenticated)
            {
                var ret = await db.AspNetUsers.FirstOrDefaultAsync(x => x.UserName.Equals(email));
                //var dat = from u in db.AspNetUsers
                //          where u.Email.Equals(email)
                //          select new
                //          {
                //              isPasswordSaved = u.IsPasswordSaved,
                //          };
                if (ret == null)
                {
                    return Ok("NewUser");
                }
                try {
                    if (await UserManager.GetLockoutEnabledAsync(ret.Id))
                    {
                        return Ok("Blocked");
                    } }
                catch(Exception e)
                {
                    string s = e.ToString();
                }
                var data = new
                {
                    name = ret.Email,
                    isPasswordSaved = ret.IsPasswordSaved,
                };
                
                return Ok(data);
            }
            return BadRequest("Already Login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckLoginUserPassword(string email,string password)
        {
            ApplicationDbContext ctx = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(ctx);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            password = UserManager.PasswordHasher.HashPassword(password);  
            //var ret = await db.AspNetUsers.FirstOrDefaultAsync(x => x.UserName.Equals(email) && x.PasswordHash.Equals(password));
            var user = await UserManager.FindAsync(email,password);
            if (user != null)
            {
                //await HttpContext
               // await HttpContext.GetOwinContext()
    //.Get<ApplicationSignInManager>().SignInAsync(user, true, false); 
           //    await  SignInAsync(user, true,true); 
                return Ok("Incorrect");
            }
            return Ok("Ok");
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> SaveProfilePic()
        {
            //save extension in database.
            return Ok();
        }
        [HttpPost]
        public async Task<IHttpActionResult> AddFriend(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == User.Identity.GetUserId())
                {
                    return BadRequest("You are already friend of yourself :) ");
                }
                Friend friend = new Friend();
                friend.friendId = id;
                friend.userId = User.Identity.GetUserId();
                friend.time = DateTime.UtcNow;
                db.Friends.Add(friend);
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> UnFriend(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var fr =await db.Friends.FirstOrDefaultAsync(x=>x.friendId.Equals(id) || x.userId.Equals(id)) ;
                if (fr.userId == User.Identity.GetUserId() || fr.friendId == User.Identity.GetUserId())
                {
                    db.Friends.Remove(fr);
                    await db.SaveChangesAsync();
                    return Ok("Done");
                }
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> SaveItem(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                SaveAd save =  db.SaveAds.FirstOrDefault(x=>x.savedBy.Equals(userId) && x.adId.Equals(id));
                if(save != null){
                    db.SaveAds.Remove(save);
                    await db.SaveChangesAsync();
                    var ret =await db.SaveAds.CountAsync(x => x.adId.Equals(id));
                    var obj = new { text = "Deleted", count = ret };
                    return Ok(obj);
                }
                SaveAd ad = new SaveAd();
                ad.time = DateTime.UtcNow;
                ad.adId = id;
                ad.savedBy = User.Identity.GetUserId();
                db.SaveAds.Add(ad);
                await db.SaveChangesAsync();
                var retu =await db.SaveAds.CountAsync(x => x.adId.Equals(id));
                var obje = new { text = "Saved", count = retu };
                return Ok(obje);
            }
            return BadRequest("Not login");
        }
        //public async Task<IHttpActionResult> SendMessageTo(string id)
        //{}

        public async Task<IHttpActionResult> GetUser(string id)
        {
            AspNetUser aspnetuser = await db.AspNetUsers.FindAsync(id);
           // AspNetUser aspnetuser = db.AspNetUsers.FirstOrDefault(id);
            if (aspnetuser == null)
            {
                return NotFound();
            }
            string loginUserId = "";
            if (User.Identity.IsAuthenticated)
            {
                loginUserId = User.Identity.GetUserId();
            }
            if (loginUserId != id)
            {
                var user = (from u in db.AspNetUsers
                            where u.Id.Equals(id)
                            select new
                            {
                                UserName = u.UserName,
                                Email = u.Email,
                                isEmailConfirmed = u.EmailConfirmed,
                                Id = u.Id,
                                dateOfBirth = u.dateOfBirth,
                                gender = u.gender,
                                hideEmail = u.hideEmail,
                                hidePhoneNumber = u.hidePhoneNumber,
                                hideDateOfBirth = u.hideDateOfBirth,
                                phoneNumber = u.PhoneNumber,
                                about = u.about,
                                dpExtension = u.dpExtension,
                                lastSeen = Membership.UserIsOnlineTimeWindow,
                                reputation = u.reputation,
                                since = u.since,
                                city = u.city,
                                isFriend = u.Friends.Any(x => x.friendId.Equals(loginUserId) || x.userId.Equals(loginUserId)),
                                loginUserId = loginUserId,
                                companies = from company in u.Companies
                                            select new
                                            {
                                                id = company.Id,
                                                name = company.title,
                                                logoExtension = company.logoextension,
                                            },
                                followingTags = from tag in u.FollowTags
                                                select new
                                                {
                                                    id = tag.Tag.Id,
                                                    name = tag.Tag.name,
                                                },
                                activeads = from ad in u.Ads
                                            where ad.CompanyAd == null
                                            orderby ad.time descending
                                            select new
                                            {
                                                title = ad.title,
                                                postedById = ad.AspNetUser.Id,
                                                postedByName = ad.AspNetUser.Email,
                                                description = ad.description,
                                                id = ad.Id,
                                                time = ad.time,
                                                islogin = loginUserId,
                                                isNegotiable = ad.isnegotiable,
                                                price = ad.price,
                                                reportedCount = ad.Reporteds.Count,
                                                isReported = ad.Reporteds.Any(x => x.reportedBy == loginUserId),
                                                category = ad.category,
                                                subcategory = ad.subcategory,
                                                views = ad.views,
                                                adTags = from tag in ad.AdTags.ToList()
                                                         select new
                                                         {
                                                             id = tag.tagId,
                                                             name = tag.Tag.name,
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
                                                },
                                            },
                                savedads = "",

                                notificationads = "",
                            }).FirstOrDefault();
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            var adnotifications = from followtag in db.FollowTags
                                  where followtag.followedBy.Equals(id)
                                  from adtag in db.AdTags
                                  where followtag.tagId.Equals(adtag.tagId)
                                  from ad in db.Ads
                                  where ad.Id.Equals(adtag.adId)
                                  orderby ad.time descending
                                  select new
                                  {
                                      title = ad.title,
                                      postedById = ad.AspNetUser.Id,
                                      postedByName = ad.AspNetUser.Email,
                                      description = ad.description,
                                      id = ad.Id,
                                      time = ad.time,
                                      islogin = loginUserId,
                                      isNegotiable = ad.isnegotiable,
                                      price = ad.price,
                                      reportedCount = ad.Reporteds.Count,
                                      isReported = ad.Reporteds.Any(x => x.reportedBy == loginUserId),
                                      category = ad.category,
                                      subcategory = ad.subcategory,
                                      views = ad.views,
                                      adTags = from tag in ad.AdTags.ToList()
                                               select new
                                               {
                                                   id = tag.tagId,
                                                   name = tag.Tag.name,
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
                                      },
                                  };
            adnotifications = adnotifications.GroupBy(x => x.id).Select(x => x.FirstOrDefault()).OrderByDescending(x=>x.time);
                var user1 = (from u in db.AspNetUsers
                            where u.Id.Equals(id)
                            select new
                            {
                                UserName = u.UserName,
                                Email = u.Email,
                                isEmailConfirmed = u.EmailConfirmed,
                                Id = u.Id,
                                dateOfBirth = u.dateOfBirth,
                                gender = u.gender,
                                hideEmail = u.hideEmail,
                                hidePhoneNumber = u.hidePhoneNumber,
                                hideDateOfBirth = u.hideDateOfBirth,
                                phoneNumber = u.PhoneNumber,
                                about = u.about,
                                dpExtension = u.dpExtension,
                                lastSeen = Membership.UserIsOnlineTimeWindow,
                                reputation = u.reputation,
                                since = u.since,
                                city = u.city,
                                isFriend = u.Friends.Any(x => x.friendId.Equals(loginUserId) || x.userId.Equals(loginUserId)),
                                loginUserId = loginUserId,
                                companies = from company in u.Companies
                                            select new
                                            {
                                                id = company.Id,
                                                name = company.title,
                                                logoExtension = company.logoextension,
                                            },
                                followingTags = from tag in u.FollowTags
                                                select new
                                                {
                                                    id = tag.Tag.Id,
                                                    name = tag.Tag.name,
                                                },
                                activeads = (from ad in u.Ads
                                            where ad.CompanyAd == null
                                            orderby ad.time descending
                                            select new
                                            {
                                                title = ad.title,
                                                postedById = ad.AspNetUser.Id,
                                                postedByName = ad.AspNetUser.Email,
                                                description = ad.description,
                                                id = ad.Id,
                                                time = ad.time,
                                                islogin = loginUserId,
                                                isNegotiable = ad.isnegotiable,
                                                price = ad.price,
                                                reportedCount = ad.Reporteds.Count,
                                                isReported = ad.Reporteds.Any(x => x.reportedBy == loginUserId),
                                                category = ad.category,
                                                subcategory = ad.subcategory,
                                                views = ad.views,
                                                adTags = from tag in ad.AdTags.ToList()
                                                         select new
                                                         {
                                                             id = tag.tagId,
                                                             name = tag.Tag.name,
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
                                                },
                                            }).Take(40),
                                savedads = from ad in u.SaveAds
                                           orderby ad.time descending
                                           select new
                                           {
                                               title = ad.Ad.title,
                                               postedById = ad.Ad.AspNetUser.Id,
                                               postedByName = ad.Ad.AspNetUser.Email,
                                               description = ad.Ad.description,
                                               id = ad.Ad.Id,
                                               time = ad.Ad.time,
                                               islogin = loginUserId,
                                               isNegotiable = ad.Ad.isnegotiable,
                                               price = ad.Ad.price,
                                               reportedCount = ad.Ad.Reporteds.Count,
                                               isReported = ad.Ad.Reporteds.Any(x => x.reportedBy == loginUserId),
                                               category = ad.Ad.category,
                                               subcategory = ad.Ad.subcategory,
                                               views = ad.Ad.views,
                                               adTags = from tag in ad.Ad.AdTags.ToList()
                                                        select new
                                                        {
                                                            id = tag.tagId,
                                                            name = tag.Tag.name,
                                                        },
                                               bid = from biding in ad.Ad.Bids.ToList()
                                                     select new
                                                     {
                                                         price = biding.price,
                                                     },
                                               adImages = from image in ad.Ad.AdImages.ToList()
                                                          select new
                                                          {
                                                              imageExtension = image.imageExtension,
                                                          },
                                               location = new
                                               {
                                                   cityName = ad.Ad.AdsLocation.City.cityName,
                                                   cityId = ad.Ad.AdsLocation.cityId,
                                                   popularPlaceId = ad.Ad.AdsLocation.popularPlaceId,
                                                   popularPlace = ad.Ad.AdsLocation.popularPlace.name,
                                               },
                                           },

                                notificationads = adnotifications,
                            }).FirstOrDefault();

                if (user1 == null)
                {
                    return NotFound();
                }
                return Ok(user1);
        }
        //public async Task<IHttpActionResult> IsUserOnline(string id)
        //{
        //    var ret = "false";
        //    if (Membership.GetUser(id).IsOnline)
        //    {
        //        ret = "true";
        //        return Ok(ret);
        //    }
        //    return Ok(ret);
        //}
        public async Task<int> cityId(string city)
        {
            if (city != null)
            {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    db.Cities.Add(cit);
                    await db.SaveChangesAsync();
                    return cit.Id;
                }
                return citydb.Id;
            }
            return -1;
        }
         public async Task<IHttpActionResult> UpdateProfile(AspNetUser user)
         {
             if (User.Identity.IsAuthenticated)
             {
                 if (User.Identity.GetUserId() == user.Id)
                 {
                     if (!ModelState.IsValid)
                     {
                         return BadRequest();
                     }
                     await cityId(user.city);
                     AspNetUser u =await db.AspNetUsers.FindAsync(user.Id);
                     u.UserName = user.UserName;
                     u.Email = user.Email;
                     u.about = user.about;
                     u.city = user.city;
                     u.dateOfBirth = user.dateOfBirth;
                     u.gender = user.gender;
                     u.hideDateOfBirth = user.hideDateOfBirth;
                     u.hideEmail = user.hideEmail;
                    // u.hideFriends = user.hideFriends;
                     u.hidePhoneNumber = user.hidePhoneNumber;
                     u.PhoneNumber = user.PhoneNumber;
                     
                     //user.status = u.status;
                     //user.since = u.since;
                     //user.dpExtension = u.dpExtension;
                     //user.IsPasswordSaved = u.IsPasswordSaved;
                     //user.EmailConfirmed = u.EmailConfirmed;
                     //user.reputation = u.reputation;
                    
                    // user.
                    // db.Entry(user).State = EntityState.Modified;

                     try
                     {
                         await db.SaveChangesAsync();
                     }
                     catch (DbUpdateConcurrencyException)
                     {
                         if (!AspNetUserExists(user.Id))
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
             }
             return BadRequest();
        }
        // PUT api/User/5
        public async Task<IHttpActionResult> PutAspNetUser(string id, AspNetUser aspnetuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aspnetuser.Id)
            {
                return BadRequest();
            }

            db.Entry(aspnetuser).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserExists(id))
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

        // POST api/User
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> PostAspNetUser(AspNetUser aspnetuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AspNetUsers.Add(aspnetuser);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AspNetUserExists(aspnetuser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aspnetuser.Id }, aspnetuser);
        }

        // DELETE api/User/5
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> DeleteAspNetUser(string id)
        {
            AspNetUser aspnetuser = await db.AspNetUsers.FindAsync(id);
            if (aspnetuser == null)
            {
                return NotFound();
            }

            db.AspNetUsers.Remove(aspnetuser);
            await db.SaveChangesAsync();

            return Ok(aspnetuser);
        }

        protected override void Dispose(bool disposing)
       {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AspNetUserExists(string id)
        {
            return db.AspNetUsers.Count(e => e.Id == id) > 0;
        }
    }
}