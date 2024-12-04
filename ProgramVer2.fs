//Ver1
open System
open System.Windows.Forms
open Newtonsoft.Json
open System.IO

// Define the Book Record type
type Book = {
    Title: string
    Author: string
    Genre: string
    IsBorrowed: bool
    BorrowDate: DateTime option  // Add BorrowDate as an optional field
}

// Map to store books by title
let mutable library = Map.empty<string, Book>

// Path to store the JSON file
let filePath = "library.json"

// Function to save books to a file
let saveLibraryToFile () =
    let json = JsonConvert.SerializeObject(library)
    File.WriteAllText(filePath, json)

// Function to load books from a file
let loadLibraryFromFile () =
    if File.Exists(filePath) then
        let json = File.ReadAllText(filePath)
        library <- JsonConvert.DeserializeObject<Map<string, Book>>(json)
    else
        library <- Map.empty

        //Ver2

// Function to add a new book
let addBook title author genre =
    let book = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowDate = None }
    library <- library.Add(title, book)
    saveLibraryToFile ()
    "Book added successfully"

// Function to search for a book by title
let searchBook title =
    match library.TryFind(title) with
    | Some book -> 
        let status = if book.IsBorrowed then 
                         match book.BorrowDate with
                         | Some date -> sprintf "Borrowed on: %s" (date.ToString("yyyy-MM-dd HH:mm"))
                         | None -> "Borrowed"
                     else "Available"
        sprintf "Title: %s\nAuthor: %s\nGenre: %s\nStatus: %s" 
                book.Title book.Author book.Genre status
    | None -> "Book not found"

// Function to borrow a book and automatically record the borrow date
let borrowBook title =
    match library.TryFind(title) with
    | Some book when not book.IsBorrowed -> 
        let updatedBook = { book with IsBorrowed = true; BorrowDate = Some DateTime.Now }
        library <- library.Add(title, updatedBook)
        saveLibraryToFile ()
        "Book borrowed successfully"
    | Some book -> "This book is already borrowed"
    | None -> "Book not found"