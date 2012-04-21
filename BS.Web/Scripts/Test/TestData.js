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

var bookData = [
    { Id: 'books/1', Title: 'Book #1', Author: 'Author #1', ASIN: '12345', Description: 'A "Good" Book' },
    { Id: 'books/2', Title: 'Book #2', Author: 'Author #1', ASIN: '54321', Description: 'A "Really Good" Book' },
    { Id: 'books/3', Title: 'Book #3', Author: 'Author #2', ASIN: '13245', Description: 'A Book about VAMPIRES' },
    { Id: 'books/4', Title: 'Book #4', Author: 'Author #3', ASIN: '53421', Description: 'Another book about vampires' },
    { Id: 'books/5', Title: 'Book #5', Author: 'Author #3', ASIN: '21435', Description: 'A book that\'s not about vampires.' },
];