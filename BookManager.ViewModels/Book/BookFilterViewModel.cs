using System;
using System.Collections.Generic;
using BookManager.ViewModels.Book;

namespace BookManager.ViewModels.Books
{
    public class BookFilterViewModel
    {
        public string? SearchTitle { get; set; }
        public Guid? SelectedAuthorId { get; set; }
        public Guid? SelectedGenreId { get; set; }
        public Guid? SelectedPublisherId { get; set; }

        public List<AuthorDropdownViewModel> Authors { get; set; } = new();
        public List<GenreDropdownViewModel> Genres { get; set; } = new();
        public List<PublisherDropdownViewModel> Publishers { get; set; } = new();

        public List<BookViewModel> Books { get; set; } = new();
    }
}