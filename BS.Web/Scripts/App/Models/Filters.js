/// <reference path="../../_references.js" />

(function () {
    var BS = window.BS = window.BS || {};

    BS.Filters = function (options) {
        var self = this;

        self.search = ko.observable(options.search || '');
        self.pageSize = ko.observable(options.pageSize || 10);
        self.page = ko.observable(1);
        self.friends = ko.observable([]);
        self.orderBy = ko.observable(options.orderBy || "Default");

        self.nextPage = function () {
            self.page(self.page() + 1);
        };

        self.prevPage = function () {
            if (self.page() == 1) return;

            self.page(self.page() - 1);
        };

        self.query = ko.computed(function () {
            var data = {
                skip: (self.page() - 1) * self.pageSize(),
                take: self.pageSize(),
                orderBy: self.orderBy(),
                profileIds: self.friends().map(function(friend) {
                    return friend.Id();
                }).join(',')
            };

            var search = self.search();
            if (search != null && search != '')
                data.search = self.search();

            return data;
        }).extend({ throttle: 1 });

        ko.bindingHandlers.resetPage = {
            init: function (element, valueAccessor, allBindingsAccessor) {
                ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
            },
            update: function (element, valueAccessor) {
                self.page(1);
                ko.bindingHandlers.value.update(element, valueAccessor);
            }
        };
    };
})();