/// <reference path="../../_references.js" />

(function () {
    var BS = window.BS = window.BS || {};

    BS.Book = function (data, readingListDetails) {
        var self = this;

        ko.mapping.fromJS(data, {}, self);

        self.UserRating = ko.observable(0);
        self.IsSelected = ko.observable(false);
        if (readingListDetails != null) {
            self.UserRating(readingListDetails.Rating());
            self.IsSelected(true);
        }
    }
})();