module('Profile tests.');

test('Determine if book is on a user\'s Reading List',
    function () {
        var profile = new BS.Profile(profileData);

        equal(true, profile.isBookOnReadingList('books/1'), 'book books/1 should be on reading list.');
        equal(false, profile.isBookOnReadingList('books/4'), 'book books/4 should not be on reading list.');
    }
);

    test('Add a book to a user\'s Reading List.',
    function () {
        var profile = new BS.Profile(profileData);
        var server = this.sandbox.useFakeServer();
        server.respondWith("POST", "api/profiles/1/books/4",
            [200, { "Content-Type": "application/json" },
                '{"Id": "books/4", "Title": "Book \#4", "Rating": 3}']);

        profile.addBookToReadingList(ko.observable('books/4'), 3);
        server.respond();

        equal(1, server.requests.length, 'one POST request should have been sent to server.');
        equal('Rating=3', server.requests[0].requestBody, 'request body should include rating');
        equal(true, profile.isBookOnReadingList('books/4'), 'book books/4 should have been added to the reading list.');
        equal(3, profile.getBookFromReadingList('books/4').Rating(), 'book books/4 should have a rating of 3');
    }
);

    test('Update ratings on books that have already been added.',
    function () {
        var profile = new BS.Profile(profileData);
        var server = this.sandbox.useFakeServer();
        server.respondWith("PUT", "api/profiles/1/books/3",
            [200, { "Content-Type": "application/json" }, '']);

        // update books/3 rating from 0 to 3
        profile.addBookToReadingList(ko.observable('books/3'), 3);
        server.respond();

        equal(1, server.requests.length, 'one PUT request should have been sent to server.');
        equal('Rating=3', server.requests[0].requestBody, 'request body should include rating');
        equal(true, profile.isBookOnReadingList('books/3'), 'book books/3 should have been added to the reading list.');
        equal(3, profile.getBookFromReadingList('books/3').Rating(), 'book books/3 should have a rating of 3');
    }
);

    test('Remove a book from the user\'s reading list.',
    function () {
        var profile = new BS.Profile(profileData);
        var server = this.sandbox.useFakeServer();
        server.respondWith("DELETE", "api/profiles/1/books/1",
            [200, { "Content-Type": "application/json" }, ""]);

        profile.removeBookFromReadingList(ko.observable('books/1'));
        server.respond();

        equal(1, server.requests.length, 'DELETE request to remove book from list required.');
        equal(false, profile.isBookOnReadingList('books/1'), 'book books/1 should no longer be on the reading list.');
    }
);

    test('Ignore requests to remove books that aren\'t on the reading list.',
    function () {
        var profile = new BS.Profile(profileData);
        var server = this.sandbox.useFakeServer();

        profile.removeBookFromReadingList(ko.observable('books/4'));
        server.respond();

        equal(0, server.requests.length, 'no DELETE request to remove book from list should have been submitted.');
        equal(false, profile.isBookOnReadingList('books/4'), 'book books/1 should no longer be on the reading list.');
    }
);