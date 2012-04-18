
module('CatalogViewModel tests');

test('Retrieve current profile and books',
    function () {
        var server = this.sandbox.useFakeServer();
        server.respondWith("GET", "api/profiles/1",
            [200, { "Content-Type": "application/json" },
                JSON.stringify(profileData)]);
        server.respondWith("GET", /api\/books\?.*/,
            [200, { "Content-Type": "application/json" },
                JSON.stringify(bookData)]);

        var catalog = new BS.CatalogViewModel({ profileId: 'profiles/1' });
        server.respond();

        equal('profiles/1', catalog.profile.Id(), 'Catalog should have retrieved "profile/1"');
        equal(5, catalog.books().length, 'There are five books in the catalog.');
        equal(true,
                catalog.books()[0].isSelected() &&
                catalog.books()[1].isSelected() &&
                catalog.books()[2].isSelected(),
                'books/1 books/2 and books/3 should be selected');
        equal(false,
                catalog.books()[3].isSelected() &&
                catalog.books()[4].isSelected(),
                'books/4 and books/5 should not be selected');
    }
);