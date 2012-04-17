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

        self.isBookOnReadingList = function (id) {
            return self.ReadingList.mappedIndexOf({ Id: id }) != -1;
        };

        self.addBookToReadingList = function (id) {
            $.ajax({
                type: "POST",
                url: "api/" + self.Id + "/books",
                data: { Id: id },
                dataType: "json",
                success: function (data) {
                    self.ReadingList.push(data);
                },
                error: function (req, status, error) { debugger; }
            });
        }

        self.removeBookFromReadingList = function (id) {
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