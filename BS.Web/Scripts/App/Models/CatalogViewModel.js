/// <reference path="../../_references.js" />

(function () {
    var BS = window.BS = window.BS || {};

    BS.CatalogViewModel = function (options) {
        var self = this;

        self.books = ko.mapping.fromJS([]);
        self.profile = ko.mapping.fromJS({ Email: "", ReadingList: [] });

        // get the current profile, then get the books
        $.ajax({
            type: "GET",
            url: "api/" + options.profileId,
            dataType: "json",
            statusCode: {
                304: function (args) {
                    alert("304 error");
                    alert(args);
                }
            },
            success: function (data) {
                ko.mapping.fromJS({ profile: new BS.Profile(data) }, {}, self);

                $.ajax({
                    type: "GET",
                    url: "api/books",
                    dataType: "json",
                    cache: false,
                    statusCode: {
                        304: function (args) {
                            alert(args);
                        }
                    },
                    success: function (data) {
                        var mapping = {
                            'books': {
                                create: function (options) {
                                    return new BS.Book(options.data, self.profile);
                                }
                            }
                        };
                        ko.mapping.fromJS({ books: data }, mapping, self);
                    },
                    error: function (req, status, error) {
                        debugger;
                    }
                });
            },
            error: function (req, status, error) {
                debugger;
            }
        });
    }
})();