
function SearchingLocation() {
    var self = this;
    self._isSearchCityLoading = ko.observable(false);
    self._isSearchPPLoading = ko.observable(false);
    console.log($.cookie("searchCity"));
    console.log($.cookie("searchPP"));
    searchingCity = ko.observable();
    searchingPP = ko.observable();
    searchingCities = ko.observableArray();
    searchingPPs = ko.observableArray();
    self._searchingLocationError = ko.observable();
    self.searchLocationBtn = function () {
        $("#setLocation").modal('hide');
    }
    //searchingCity.subscribe(function () {
        
    //})
    self.isLocationClicked = ko.observable(false);
    self.setLocationClicked = function () {
        self.isLocationClicked(true);

        var sub = searchingCity.subscribe(function (value) {
            $.cookie("searchCity", searchingCity(), { path: '/' });
            if (searchingCity() != null && searchingCity() != "") {
                $("#setLocation").submit();
               // window.location.href = "/Shared/SetLocation?city=" + searchingCity();
                //searchingPP("");
            }

            // self._loadPopularPlaces();
        })

        //var iddd = $("#setLocation select");
        //iddd.click();

        var sel = $('#searching-city').selectize();
        sel[0].selectize.focus();

      //  var sel = $('#searching-city').selectize();
//sel[0].sel.focus();
      //  $(this).parent().siblings('div.bottom').find("input.post").focus();
    }

    self.loadCities = function () {
        self._isSearchCityLoading(true);
        $.ajax({
            url: '/api/Location/GetCities',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                self._isSearchCityLoading(false);
                $.each((data), function (i, item) { searchingCities.push(item) });
                searchingCities.push("All Pakistan");
                self._searchingLocationError("");
                searchingCity($.cookie("searchCity"));
                var sel = $('#searching-city').selectize({
                    sortField: {
                        field: 'text',
                        direction: 'asc'
                    },
                });
            },
            error: function (jqXHR, status, thrownError) {
                self._searchingLocationError("Error! Check your internet connection and refresh page");
                self._isSearchCityLoading(false);
                toastr.error("failed to load Cities.Please refresh page and try again", "Error");
            }
        });
    }
    self._loadPopularPlaces = function () {
        self._isSearchPPLoading(true);
        searchingPPs.removeAll();
        $.ajax({
            url: '/api/Location/GetPopularPlaces?city=' + searchingCity(),
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                self._isSearchPPLoading(false);
                $.each((data), function (i, item) { searchingPPs.push(item) });
                self._searchingLocationError("");
                searchingPP($.cookie("searchPP"));
                $('#searching-popularPlace').selectize({
                    sortField: {
                        field: 'text',
                        direction: 'asc'
                    },
                });

                //--------------------
               
                
               

                //-----------------
            },
            error: function (jqXHR, status, thrownError) {
                self._searchingLocationError("Error! Check your internet connection and refresh page");
                self._isSearchPPLoading(false);
                toastr.error("failed to load Famous Places.Please refresh page and try again", "Error");
            }
        });
    }
    self.loadCities();
   
    searchingPP.subscribe(function (value) {
        $.cookie("searchPP", searchingPP(), { path: '/' });
    })
}