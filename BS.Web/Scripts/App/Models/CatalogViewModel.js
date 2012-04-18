/// <reference path="../../_references.js" />

(function () {
    var BS = window.BS = window.BS || {};

    BS.CatalogViewModel = function (options) {
        var self = this;

        self.books = ko.mapping.fromJS([]);
        self.profile = ko.mapping.fromJS({ Email: "", ReadingList: [] });

        // get the current profile and books asynchronously!
        var booksReq = $.ajax({
            type: "GET",
            url: "api/books",
            dataType: "json",
            cache: false,
            statusCode: {
                304: function (args) {
                    alert(args);
                }
            }
        });

        var profileReq = $.ajax({
            type: "GET",
            url: "api/" + options.profileId,
            dataType: "json",
            statusCode: {
                304: function (args) {
                    alert("304 error");
                    alert(args);
                }
            }
        });

        $.when(profileReq, booksReq)
            .then(
                function (a1, a2) {
                    var profileData = a1[0];
                    ko.mapping.fromJS({ profile: new BS.Profile(profileData) }, {}, self);

                    var bookData = a2[0];
                    var mapping = {
                        'books': {
                            create: function (options) {
                                return new BS.Book(options.data, self.profile);
                            }
                        }
                    };
                    ko.mapping.fromJS({ books: bookData }, mapping, self);
                },
                function (a1, a2, a3) { debugger; });
    }
})();