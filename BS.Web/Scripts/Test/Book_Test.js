
module('Book tests.');

test('Unselected book defaults to IsSelected = false and UserRating = 0',
    function () {
        var book = new BS.Book(bookData[0], null);

        equal(false, book.IsSelected());
        equal(0, book.UserRating());
    }
);

    test('Selected book defaults to IsSelected = true and UserRating = Selected books rating',
    function () {
        var book = new BS.Book(bookData[0], ko.mapping.fromJS({ Id: 'books/1', Rating: 3 }));

        equal(true, book.IsSelected());
        equal(3, book.UserRating());
    }
);