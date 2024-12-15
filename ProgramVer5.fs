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

    //Ver3

// Function to return a book
let returnBook title =
    match library.TryFind(title) with
    | Some book when book.IsBorrowed -> 
        let updatedBook = { book with IsBorrowed = false; BorrowDate = None }
        library <- library.Add(title, updatedBook)
        saveLibraryToFile ()
        "Book returned successfully"
    | Some book -> "This book was not borrowed"
    | None -> "Book not found"

// Function to delete a book
let deleteBook title =
    match library.TryFind(title) with
    | Some _ -> 
        library <- library.Remove(title)
        saveLibraryToFile ()
        "Book deleted successfully"
    | None -> "Book not found"

// Create form for the Windows Form Application
let form = new Form(Text = "Library Management System", Width = 1000, Height = 700)

// Create panels for layout
let addBookPanel = new Panel(Top = 20, Left = 20, Width = 400, Height = 250, BackColor = System.Drawing.Color.LightGray)
let searchPanel = new Panel(Top = 280, Left = 20, Width = 400, Height = 300, BackColor = System.Drawing.Color.LightGray)
let booksDisplayPanel = new Panel(Top = 20, Left = 450, Width = 500, Height = 500, BackColor = System.Drawing.Color.LightGray)

// Add book panel components
let addBookLabel = new Label(Text = "AddBook", Top = 10, Left = 10, Font = new System.Drawing.Font("Arial", 14.0f, System.Drawing.FontStyle.Bold))
let titleLabel = new Label(Text = "Title:", Top = 50, Left = 10, Font = new System.Drawing.Font("Arial", 10.0f))
let titleTextBox = new TextBox(Top = 50, Left = 80, Width = 300, Font = new System.Drawing.Font("Arial", 10.0f), RightToLeft = RightToLeft.No)
let authorLabel = new Label(Text = "Author:", Top = 90, Left = 10, Font = new System.Drawing.Font("Arial", 10.0f))
let authorTextBox = new TextBox(Top = 90, Left = 80, Width = 300, Font = new System.Drawing.Font("Arial", 10.0f), RightToLeft = RightToLeft.No)
let genreLabel = new Label(Text = "Genre:", Top = 130, Left = 10, Font = new System.Drawing.Font("Arial", 10.0f))
let genreTextBox = new TextBox(Top = 130, Left = 80, Width = 300, Font = new System.Drawing.Font("Arial", 10.0f), RightToLeft = RightToLeft.No)
let addButton = new Button(Text = "Add Book", Top = 170, Left = 120, Width = 160, Height = 40, BackColor = System.Drawing.Color.MediumSeaGreen, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Arial", 12.0f, System.Drawing.FontStyle.Bold))

// Search panel components
let searchLabel = new Label(Text = "Search", Top = 10, Left = 10, Font = new System.Drawing.Font("Arial", 14.0f, System.Drawing.FontStyle.Bold))
let searchTextBox = new TextBox(Top = 50, Left = 10, Width = 300, Font = new System.Drawing.Font("Arial", 10.0f), RightToLeft = RightToLeft.No)
let searchButton = new Button(Text = "Search", Top = 90, Left = 10, Width = 90, Height = 40, BackColor = System.Drawing.Color.CornflowerBlue, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Arial", 12.0f))
let borrowButton = new Button(Text = "Borrow", Top = 90, Left = 110, Width = 90, Height = 40, BackColor = System.Drawing.Color.CornflowerBlue, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Arial", 12.0f))
let returnButton = new Button(Text = "Return", Top = 90, Left = 210, Width = 90, Height = 40, BackColor = System.Drawing.Color.CornflowerBlue, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Arial", 12.0f))
let deleteButton = new Button(Text = "Delete Book", Top = 140, Left = 10, Width = 90, Height = 40, BackColor = System.Drawing.Color.IndianRed, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Arial", 12.0f))

// Display panel components
let booksLabel = new Label(Text = "BooksList", Top = 10, Left = 10, Font = new System.Drawing.Font("Arial", 14.0f, System.Drawing.FontStyle.Bold))
let booksListBox = new ListBox(Top = 50, Left = 10, Width = 460, Height = 400, Font = new System.Drawing.Font("Arial", 10.0f))

// Status Label (Message area)
let statusLabel = new Label(Top = 550, Left = 20, Width = 950, Height = 100, BackColor = System.Drawing.Color.LightGray, Font = new System.Drawing.Font("Arial", 14.0f), TextAlign = System.Drawing.ContentAlignment.MiddleCenter)

// Load library data from file when starting the program
loadLibraryFromFile ()

//Ver4


// Function to refresh the ListBox
let refreshBooksList () =
    booksListBox.Items.Clear()
    library
    |> Map.toList
    |> List.iter (fun (title, book) -> 
        let status = if book.IsBorrowed then "Borrowed" else "Available"
        booksListBox.Items.Add(sprintf "%s - %s" title status) |> ignore
    )

// Event Handlers
addButton.Click.Add(fun _ ->
    let title = titleTextBox.Text
    let author = authorTextBox.Text
    let genre = genreTextBox.Text
    if title = "" || author = "" || genre = "" then
        statusLabel.Text <- "Please fill all fields!"
    else
        let result = addBook title author genre
        statusLabel.Text <- result
        refreshBooksList ()
)

searchButton.Click.Add(fun _ ->
    let title = searchTextBox.Text
    let result = searchBook title
    statusLabel.Text <- result
)
//Ver5
borrowButton.Click.Add(fun _ ->
    let title = searchTextBox.Text
    let result = borrowBook title
    statusLabel.Text <- result
    refreshBooksList ()
)

returnButton.Click.Add(fun _ ->
    let title = searchTextBox.Text
    let result = returnBook title
    statusLabel.Text <- result
    refreshBooksList ()
)

deleteButton.Click.Add(fun _ ->
    let title = searchTextBox.Text
    let result = deleteBook title
    statusLabel.Text <- result
    refreshBooksList ()
)

// Add components to panels
addBookPanel.Controls.AddRange([| addBookLabel; titleLabel; titleTextBox; authorLabel; authorTextBox; genreLabel; genreTextBox; addButton |])
searchPanel.Controls.AddRange([| searchLabel; searchTextBox; searchButton; borrowButton; returnButton; deleteButton |])
booksDisplayPanel.Controls.AddRange([| booksLabel; booksListBox |])

// Add panels to form
form.Controls.AddRange([| addBookPanel; searchPanel; booksDisplayPanel; statusLabel |])

// Run the application
refreshBooksList ()
Application.Run(form)