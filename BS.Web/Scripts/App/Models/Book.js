/// <reference path="../../_references.js" />

(function () {
    var BS = window.BS = window.BS || {};

    BS.Book = function (data, profile) {
        var self = this;

        ko.mapping.fromJS(data, {}, self);

        self.profile = profile;
        self.isSelected = ko.computed({
            read: function () {
                return self.profile.isBookOnReadingList(self.Id);
            },
            write: function (val) {
                if (val == true) {
                    self.profile.addBookToReadingList(self.Id);
                } else {
                    self.profile.removeBookFromReadingList(self.Id);
                }
            }
        });
    }
})();