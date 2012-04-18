
module('Book tests.');

test('Determine if book is on a user\'s Reading List.',
    function () {
        var profile = new BS.Profile(profileData);
        var book1 = new BS.Book(bookData[0], profile);
        var book4 = new BS.Book(bookData[3], profile);

        equal(true, book1.isSelected(), 'book books/1 should be on reading list.');
        equal(false, book4.isSelected(), 'book books/4 should not be on reading list.');
    }
);

    test('Change the rating of a book on the Reading List.',
    function () {
        var profile = new BS.Profile(profileData);
        var book1 = new BS.Book(bookData[0], profile);
        var server = this.sandbox.useFakeServer();
        server.respondWith("PUT", "api/profiles/1/books/1",
            [200, { "Content-Type": "application/json" }, '']);

        // was 4 changing to 1
        book1.UserRating(1);
        server.respond();

        equal(1, server.requests.length, 'one PUT request should have been submitted to the server');
        equal('Rating=1', server.requests[0].requestBody, 'request body should include rating');
        equal(1, book1.UserRating(), 'book book/1 should have a rating of 1');
    }
);

    test('Setting the rating of a book not on the Reading List adds it to the Reading List.',
    function () {
        var profile = new BS.Profile(profileData);
        var book4 = new BS.Book(bookData[3], profile);
        var server = this.sandbox.useFakeServer();
        server.respondWith("POST", "api/profiles/1/books/4",
            [200, { "Content-Type": "application/json" },
                '{"Id": "books/4", "Title": "Book \#4", "Rating": 3}']);

        // was 4 changing to 1
        book4.UserRating(3);
        server.respond();

        equal(1, server.requests.length, 'one POST request should have been sent to server.');
        equal('Rating=3', server.requests[0].requestBody, 'request body should include rating');
        equal(true, book4.isSelected(), 'book books/4 should have been added to the reading list.');
        equal(3, book4.UserRating(), 'book book/4 should have a rating of 3');
    }
);
