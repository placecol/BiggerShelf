/// <reference path="../../_references.js" />

(function () {
    var BS = window.BS = window.BS || {};

    BS.Profile = function (data) {
        var self = this;

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
            }
        };

        ko.mapping.fromJS(data, mapping, self);

        self.getBookFromReadingList = function (id) {
            var index = self.ReadingList.mappedIndexOf({ Id: id });
            return self.ReadingList()[index];
        }

        self.isBookOnReadingList = function (id) {
            return self.ReadingList.mappedIndexOf({ Id: id }) != -1;
        };

        self.addBookToReadingList = function (id, rating) {
            var req = {};
            if (!self.isBookOnReadingList(id)) {
                req = $.ajax({
                    type: "POST",
                    url: "api/" + self.Id() + "/" + id(),
                    data: { Rating: rating },
                    dataType: "json"
                }).success(function (res) { self.ReadingList.push(new BS.Book(res, self)); });
            } else {
                req = $.ajax({
                    type: "PUT",
                    url: "api/" + self.Id() + "/" + id(),
                    data: { Rating: rating },
                    dataType: "json"
                }).success(function (res) { self.getBookFromReadingList(id).Rating(rating); });
            }

            req.error(function (res) { debugger; });
        }

        self.removeBookFromReadingList = function (id) {
            if (!self.isBookOnReadingList(id)) return;

            $.ajax({
                type: "DELETE",
                url: "api/" + self.Id() + "/" + id(),
                dataType: "json",
                success: function (data) {
                    self.ReadingList.mappedRemove({ Id: id });
                },
                error: function (req, status, error) { debugger; }
            });
        }
    }
})();