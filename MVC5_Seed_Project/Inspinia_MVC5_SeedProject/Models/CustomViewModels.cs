using System;
using System.Collections.Generic;
using System.Web;

namespace Inspinia_MVC5_SeedProject.Models
{
    public class ListViewModel
    {
        public IEnumerable<ListAdView> Items { get; set; }
        public Pager Pager { get; set; }
        public int ItemsCount { get; set; }
    }

    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 20)
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 3;
            var endPage = currentPage + 2;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 20)
                {
                    startPage = endPage - 19;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
        //public Pager(int totalItems, int? page, int pageSize = 10)
        //{
        //    // calculate total, start and end pages
        //    var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
        //    var currentPage = page != null ? (int)page : 1;
        //    var startPage = currentPage - 5;
        //    var endPage = currentPage + 4;
        //    if (startPage <= 0)
        //    {
        //        endPage -= (startPage - 1);
        //        startPage = 1;
        //    }
        //    if (endPage > totalPages)
        //    {
        //        endPage = totalPages;
        //        if (endPage > 10)
        //        {
        //            startPage = endPage - 9;
        //        }
        //    }

        //    TotalItems = totalItems;
        //    CurrentPage = currentPage;
        //    PageSize = pageSize;
        //    TotalPages = totalPages;
        //    StartPage = startPage;
        //    EndPage = endPage;
        //}
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }
    public class ListAdView
    {
        public int Id { get; set; }
        public string category { get; set; }
        public string postedBy { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public System.DateTime time { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> maxPrice { get; set; }
        public string isnegotiable { get; set; }
        public string subcategory { get; set; }
        public Nullable<bool> type { get; set; }
        public string condition { get; set; }
        public string status { get; set; }
        public int views { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string subsubcategory { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string area { get; set; }
        public string bedroom { get; set; }
        public string exprience { get; set; }
        public string qualification { get; set; }
        public int bidsCount { get; set; }

        public virtual Bid maxBid { get; set; }

       // public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<AdImage> AdImages { get; set; }
        public virtual AdsLocation AdsLocation { get; set; }
        public virtual ICollection<AdTag> AdTags { get; set; }
      //  public virtual ICollection<Bid> Bids { get; set; }
      //  public virtual BikeAd BikeAd { get; set; }
      //  public virtual Camera Camera { get; set; }
      //  public virtual CarAd CarAd { get; set; }
      //  public virtual ICollection<Comment> Comments { get; set; }
      //  public virtual CompanyAd CompanyAd { get; set; }
      //  public virtual House House { get; set; }
      //  public virtual JobAd JobAd { get; set; }
      //  public virtual ICollection<JobSkill> JobSkills { get; set; }
      //  public virtual LaptopAd LaptopAd { get; set; }
       // public virtual MobileAd MobileAd { get; set; }
       // public virtual ICollection<Reported> Reporteds { get; set; }
       // public virtual ICollection<SaveAd> SaveAds { get; set; }
    }
    public  class AdDetailViewModel
    {

        //public AdViewModel()
        //{

        //    this.AdImages = new HashSet<AdImage>();

        //    this.AdTags = new HashSet<AdTag>();

        //    this.Bids = new HashSet<Bid>();

        //    this.Comments = new HashSet<Comment>();

        //    this.JobSkills = new HashSet<JobSkill>();

        //    this.Reporteds = new HashSet<Reported>();

        //    this.SaveAds = new HashSet<SaveAd>();

        //}

            public string loginUserId { get; set; }
        public bool isAdmin { get; set; }
        public Bid maxBid { get; set; }
        public int Id { get; set; }

        public string category { get; set; }

        public string postedBy { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public System.DateTime time { get; set; }

        public Nullable<decimal> price { get; set; }

        public string isnegotiable { get; set; }

        public string subcategory { get; set; }

        public Nullable<bool> type { get; set; }

        public string condition { get; set; }

        public string status { get; set; }

        public int views { get; set; }

        public string name { get; set; }

        public string phoneNumber { get; set; }

        public string subsubcategory { get; set; }
        public int SaveAdsCount { get; set; }
        public int ReportedAdsCount { get; set; }
        public bool isReported { get; set; }
        public bool isSaved { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }

        public virtual ICollection<AdImage> AdImages { get; set; }

        public virtual AdsLocation AdsLocation { get; set; }

        public virtual ICollection<AdTag> AdTags { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        public virtual BikeAd BikeAd { get; set; }

        public virtual Camera Camera { get; set; }

        public virtual CarAd CarAd { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual CompanyAd CompanyAd { get; set; }

        public virtual House House { get; set; }

        public virtual ICollection<JobSkill> JobSkills { get; set; }

        public virtual LaptopAd LaptopAd { get; set; }

        public virtual MobileAd MobileAd { get; set; }

        public virtual ICollection<Reported> Reporteds { get; set; }

        public virtual ICollection<SaveAd> SaveAds { get; set; }

        public virtual JobAd JobAd { get; set; }

    }
}
