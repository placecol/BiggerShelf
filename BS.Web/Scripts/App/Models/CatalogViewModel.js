/// <reference path="../../_references.js" />

(function () {
    var BS = window.BS = window.BS || {};

    BS.CatalogViewModel = function (options) {
        var self = this;

        // setup history callbacks
        //        self.history = window.History;
        //        if (!self.history.enabled) alert('history not enabled');

        //        // Bind to StateChange Event
        //        self.history.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
        //            var State = self.history.getState(); // Note: We are using History.getState() instead of event.state
        //            self.history.log(State.data, State.title, State.url);
        //        });



        // retrieve data and apply mappings
        var mapping = {
            'ReadingList': {
                key: function (d) {
                    return ko.utils.unwrapObservable(d.Id);
                }
            },
            'Friends': {
                key: function (d) {
                    return ko.utils.unwrapObservable(d.Id);
                }
            },
            'books': {
                create: function (options) {
                    var readingListDetails = self.getBookFromReadingList(options.data.Id);
                    var book = new BS.Book(options.data, readingListDetails);
                    book.IsSelected.subscribe(self.bookSelectionChanged, book);
                    book.UserRating.subscribe(self.bookRatingChanged, book);
                    return book;
                }
            }
        };


        ko.mapping.fromJS({
            books: [],
             profile: {Email: "", ReadingList: [], Friends: []}
        }, mapping, self);

//        self.books = ko.mapping.fromJS([], mapping);
//        self.profile = ko.mapping.fromJS({ }, mapping);
        self.availableRatings = [0, 1, 2, 3, 4, 5];
        self.filters = new BS.Filters(options);

        // get the current profile and books
        var booksReq = $.ajax({
            type: "GET",
            url: "api/books",
            data: self.filters.query(),
            dataType: "json",
            cache: false
        });

        var profileReq = $.ajax({
            type: "GET",
            url: "api/" + options.profileId,
            dataType: "json"
        });

        $.when(profileReq, booksReq)
            .then(
                function (a1, a2) {

                    var profileData = a1[0];
                    ko.mapping.fromJS({ profile: profileData }, mapping, self);

                    var bookData = a2[0];
                    ko.mapping.fromJS({ books: bookData }, mapping, self);
                },
                function (a1, a2, a3) { debugger; });

        self.queryChanged = function (newValue) {

            $.ajax({
                type: "GET",
                url: "api/books",
                data: self.filters.query(),
                dataType: "json",
                cache: false
            }).success(function (data) {
                self.books.removeAll();
                ko.mapping.fromJS({ books: data }, mapping, self);
            }).error(function (res) {
                debugger;
            });
        };

        self.filters.query.subscribe(self.queryChanged);

        self.bookSelectionChanged = function (newValue) {
            if (!newValue) {
                self.removeBookFromReadingList(this);
            } else {
                self.addBookToReadingList(this);
            }
        };

        self.bookRatingChanged = function (newRating) {
            if (newRating == 0) return;

            var book = this;
            if (!book.IsSelected()) {
                this.IsSelected(true);
            } else {
                self.updateBookOnReadingList(book);
            }
        };

        self.addBookToReadingList = function (book) {
            return self.updateBookOnReadingList(book)
                .success(function (res) {
                    self.profile.ReadingList.push(res);
                });
        };

        self.updateBookOnReadingList = function (book) {
            return $.ajax({
                type: "PUT",
                url: "api/" + self.profile.Id() + "/" + book.Id(),
                data: { Rating: book.UserRating() },
                dataType: "json"
            }).error(function (res) { debugger; });
        };

        self.removeBookFromReadingList = function (book) {
            $.ajax({
                type: "DELETE",
                url: "api/" + self.profile.Id() + "/" + book.Id(),
                dataType: "json",
                success: function (data) {
                    self.profile.ReadingList.mappedRemove({ Id: book.Id });
                    book.UserRating(0);
                },
                error: function (req, status, error) { debugger; }
            });
        };

        self.getBookFromReadingList = function (id) {
            var index = self.profile.ReadingList.mappedIndexOf({ Id: id });
            if (index == -1) return null;
            return self.profile.ReadingList()[index];
        };

        self.NotOnLastPage = ko.computed(function () {
            return self.books().length == self.filters.pageSize();
        }).extend({throttle: 1});

        self.NotOnFirstPage = ko.computed(function () {
            return self.filters.page() > 1;
        }).extend({ throttle: 1 });
    };
})();