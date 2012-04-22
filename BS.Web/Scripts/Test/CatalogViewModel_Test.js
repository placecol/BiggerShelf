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
                catalog.books()[0].IsSelected() &&
                catalog.books()[1].IsSelected() &&
                catalog.books()[2].IsSelected(),
                'books/1 books/2 and books/3 should be selected');
        equal(false,
                catalog.books()[3].IsSelected() &&
                catalog.books()[4].IsSelected(),
                'books/4 and books/5 should not be selected');
    }
);

    test('Selecting a book adds it to the user\'s reading list.',
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

        // books/4 is not selected, so let's select it
        server.respondWith("PUT", "api/profiles/1/books/4",
            [200, { "Content-Type": "application/json" },
                '{"Id": "books/4", "Title": "Book \#4", "Rating": 3}']);
        catalog.books()[3].IsSelected(true);
        server.respond();

        equal(true, catalog.books()[3].IsSelected(), 'books/4 should be marked as selected.');
        equal(true, catalog.getBookFromReadingList(catalog.books()[3].Id()) != null, 'books/4 should be on the reading list.');
        equal(3, server.requests.length, 'three requests should have been made to the server');
    }
);

    test('Deselecting a book removes it from the user\'s reading list.',
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

        // books/1 is selected, so let's deselect it
        server.respondWith("DELETE", "api/profiles/1/books/1",
            [200, { "Content-Type": "application/json" }, '']);
        catalog.books()[0].IsSelected(false);
        server.respond();

        equal(false, catalog.books()[0].IsSelected(), 'books/1 should no longer be marked as selected.');
        equal(0, catalog.books()[0].UserRating(), 'books/1 should have a rating of zero.');
        equal(null, catalog.getBookFromReadingList(catalog.books()[0].Id()), 'books/1 should no longer be on the reading list.');
        equal(3, server.requests.length, 'three requests should have been made to the server');
    }
);

        test('Change the rating of a book on the Reading List.',
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


            // was 4 changing to 1
            server.respondWith("PUT", "api/profiles/1/books/1",
                [200, { "Content-Type": "application/json" }, '']);
            catalog.books()[0].UserRating(1);
            server.respond();

            equal(3, server.requests.length, 'three requests should have been submitted to the server');
            equal('Rating=1', server.requests[2].requestBody, 'PUT request body should include rating');
            equal(1, catalog.books()[0].UserRating(), 'book book/1 should have a rating of 1');
        }
    );

        test('Setting the rating of a book not on the Reading List adds it to the Reading List.',
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

            // was 4 changing to 1
            server.respondWith("PUT", "api/profiles/1/books/4",
                [200, { "Content-Type": "application/json" },
                    '{"Id": "books/4", "Title": "Book \#4", "Rating": 3}']);
            var book4 = catalog.books()[3];
            book4.UserRating(3);
            server.respond();

            equal(3, server.requests.length, 'three requests should have been sent to server.');
            equal('Rating=3', server.requests[2].requestBody, 'POST request body should include rating');
            equal(true, book4.IsSelected(), 'book books/4 should have been added to the reading list.');
            equal(3, book4.UserRating(), 'book book/4 should have a rating of 3');
        }
    );
