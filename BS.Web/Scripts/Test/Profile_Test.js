
var profileData = {
    Id: 'profiles/1',
    Email: 'an@email.com',
    Name: 'My Name',
    ReadingList: [
                { Id: 'books/1', Title: 'Book #1', Rating: 4 },
                { Id: 'books/2', Title: 'Book #2', Rating: 2 },
                { Id: 'books/3', Title: 'Book #3', Rating: 0 }
            ],
    Friends: [
                { Id: 'profiles/2', Name: 'Bob' },
                { Id: 'profiles/3', Name: 'Jacky' }
            ]
};

test('Determine if book is on a user\'s Reading List',
    function () {
        var profile = new BS.Profile(profileData);

        equal(true, profile.isBookOnReadingList('books/1'), 'book books/1 should be on reading list.');
        equal(false, profile.isBookOnReadingList('books/4'), 'book books/4 should not be on reading list.');
    }
);