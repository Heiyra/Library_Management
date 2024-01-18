using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int CopyCount { get; set; }
    public int BorrowedCount { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }

    public Book(string title, string author, string isbn, int copyCount)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        CopyCount = copyCount;
        BorrowedCount = 0;
        BorrowDate = DateTime.MaxValue;
        DueDate = DateTime.MaxValue; 
    }

    
    public bool IsAvailable()// Kitap ödünç alınmaya uygun mu kontrol edilir
    {
        return CopyCount > BorrowedCount;
    }

    
    public void Borrow()//ödünç alma
    {
        if (IsAvailable())
        {
            BorrowedCount++;
            BorrowDate = DateTime.Today;
            DueDate = BorrowDate.AddDays(15);
        }
        else
        {
            Console.WriteLine("Bu kitap şu anda mevcut değil.");
        }
    }

    public void Return()//iade etme
    {
        if (BorrowedCount > 0)
        {
            BorrowedCount--;
            BorrowDate = DateTime.MinValue;
            DueDate = DateTime.MinValue;
        }
        else
        {
            Console.WriteLine("Bu kitabın iade edilecek kopyası yok.");
        }
    }
    public bool IsOverdue()//teslim tarihi geçmiş mi kontrol edilir
    {
        return DueDate < DateTime.Today;
    }

    public void PrintInfo()//kitap bilgilerini yazdırma
    {
        Console.WriteLine();
        Console.WriteLine("                         Başlık: " + Title);
        Console.WriteLine("                         Yazar: " + Author);
        Console.WriteLine("                         ISBN: " + ISBN);
        Console.WriteLine("                         Kopya Sayısı: " + CopyCount);
        Console.WriteLine("                         Ödünç Alınan Kopyalar: " + BorrowedCount);
        Console.WriteLine("                         Mevcut Kopyalar: " + (CopyCount - BorrowedCount));
        Console.WriteLine("                         Ödünç Alınma Tarihi: " + BorrowDate.ToShortDateString());
        Console.WriteLine("                         İade Tarihi: " + DueDate.ToShortDateString());
        Console.WriteLine();
    }
}

class Library
{
    private List<Book> books;
    private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "lib_data.json"); // Belgeler klasörüne lib_data.json isimli kütüphanelerin datasını kaydedin bir dosya oluşturuldu silebilirsiniz
    public Library()
    {
        books = new List<Book>();

        LoadData();
    }

    public void AddBook(Book book)
    {
        books.Add(book);
        SaveData();
    }

    public void ListBooks()//tüm kitapları listeleyip yazdırır
    {
        Console.WriteLine("Kütüphanedeki kitaplar:");
        foreach (Book book in books)
        {
            book.PrintInfo();
        }
    }
    public void ListPopularBooks()
    {
        var sortedBooks = books.OrderByDescending(b => b.BorrowedCount).ToList();

        Console.WriteLine("En popüler 5 kitap:");
        for (int i = 0; i < 5; i++)
        {
            sortedBooks[i].PrintInfo();
        }
    }
    public void SearchBook(string query)//yazar ya da başlık aratma
    {
        Console.WriteLine(query + " ile eşleşen kitaplar:");
        foreach (Book book in books)
        {
            if (book.Title.Contains(query) || book.Author.Contains(query))
            {
                book.PrintInfo();
            }
        }
    }
    public bool CheckIsbn(string isbn)
    {
        int n = isbn.Length;

        bool isNumeric = long.TryParse(isbn, out long result);

        bool isDublicate = !IsDuplicateISBN(isbn);

        if (n == 13 && isNumeric && isDublicate)
        {
            return true;
        }
        else
        {
            if (!isDublicate)
            {
                Console.WriteLine("-ISBN daha önce kullanılmış.");
            }
            if (!isNumeric)
            {
                Console.WriteLine("-ISBN rakamlardan oluşmalıdır.");
            }
            if(n != 13)
            {
                Console.WriteLine("-ISBN 13 haneli olmalıdır.");
            }
            return false;
        }
    }
    public bool IsDuplicateISBN(string isbn)
    {
        return books.Any(book => book.ISBN == isbn);
    }
    public void BorrowBook(string isbn)//ödünç alma
    {
        Book book = books.Find(b => b.ISBN == isbn);
        if (book != null)
        {
            book.Borrow();
            SaveData();
            Console.WriteLine(book.Title + " başlıklı kitap ödünç alındı.");
        }
        else
        {
            Console.WriteLine("Bu ISBN ile eşleşen bir kitap bulunamadı.");
        }
    }
    public void ReturnBook(string isbn)//iade etme
    {
        Book book = books.Find(b => b.ISBN == isbn);
        if (book != null)
        {
            book.Return();
            SaveData();
            Console.WriteLine(book.Title + " başlıklı kitap iade edildi.");
        }
        else
        {
            Console.WriteLine("Bu ISBN ile eşleşen bir kitap bulunamadı.");
        }
    }
    public void ListOverdueBooks()//süresi geçmiş kitapları yazdırma
    {
        Console.WriteLine("Süresi geçmiş kitaplar:");
        foreach (Book book in books)
        {
            if (book.IsOverdue())
            {
                book.PrintInfo();
            }
        }
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            books = JsonSerializer.Deserialize<List<Book>>(json);
        }
        else { SaveData();
            // Var Olan Kitaplar
            Console.WriteLine("Belgeler klasörüne 'lib_data.json' isimli kütüphanelerin datasını kaydedildiği bir dosya oluşturuldu silebilirsiniz.");

            AddBook(new Book("Sineklerin Tanrısı", "William Golding", "9789750719383", 10));
            AddBook(new Book("1984", "George Orwell", "9789753429986", 15));
            AddBook(new Book("Suç ve Ceza", "Fyodor Dostoyevski", "9789754584085", 20));
            AddBook(new Book("Savaş ve Barış", "Lev Tolstoy", "9789754584092", 25));
            AddBook(new Book("Yüzüklerin Efendisi", "J.R.R. Tolkien", "9789754584108", 30));
            AddBook(new Book("Harry Potter ve Felsefe Taşı", "J.K. Rowling", "9789754584115", 15));

        }
    }

    public void SaveData()
    {
        string json = JsonSerializer.Serialize(books);
        File.WriteAllText(filePath, json);
    }
}

class Program
{
    public static void InputProcess(Library library)
    {
        string Fchoice;
        int choice = -1;
        while (choice != 0)
        {
            // Menü
            Console.WriteLine();
            Console.WriteLine("                         Kütüphane Yönetim Sistemi");
            Console.WriteLine();
            Console.WriteLine("         (Kitaplar ödünç alındıktan 15 gün içersinde iade edilmelidir)");
            Console.WriteLine();
            Console.WriteLine("         1- Kitap Ekle                       2- Kitapların Listesi");
            Console.WriteLine("         3- Başlığı ya da Yazarı Arat        4- Bir kitap ödünç al");
            Console.WriteLine("         5- Bir kitabı iade et               6- Süresi geçmiş kitaplarlar");
            Console.WriteLine();
            Console.WriteLine("         7- Popüler 5 kitap(Ödünç alınma)    0- Çıkış Yap");
            Console.WriteLine();
            Console.WriteLine("         (İşlem yapmak için yapmak istediğiniz işlemin başındaki sayıyı giriniz)");
            Console.WriteLine();
            Console.Write("Seçiminiz: ");
            Console.WriteLine();

            Fchoice = Console.ReadLine();
            bool valid = false;
            while (!valid)
            {
                try
                {
                    choice = Convert.ToInt32(Fchoice);
                    valid = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine();
                    Console.WriteLine("Lütfen geçerli bir rakam giriniz.");

                    Console.WriteLine();
                    Console.Write("Seçiminiz: ");
                    Fchoice = Console.ReadLine();
                }
            }
            Console.WriteLine();

            switch (choice)
            {
                case 1: // Kitap Ekle

                    InputAddBook(library);

                    
                    break;
                case 2: // Kitap Listesi Görüntüle

                    library.ListBooks();

                    InputProcess(library);

                    break;
                case 3: // Başlık ya da Yazar Arat

                    Console.Write("Aramak istediğiniz kitabın başlığını veya yazarını girin: ");
                    string query = Console.ReadLine();

                    library.SearchBook(query);

                    InputProcess(library);

                    break;
                case 4: // Kitap ödünç al

                    Console.Write("Ödünç almak istediğiniz kitabın ISBN numarasını girin: ");
                    string isbnBorrow = Console.ReadLine();

                    library.BorrowBook(isbnBorrow);

                    InputProcess(library);

                    break;
                case 5: // Kitabı iade et
                    Console.Write("İade etmek istediğiniz kitabın ISBN numarasını girin: ");
                    string isbnReturn = Console.ReadLine();

                    library.ReturnBook(isbnReturn);

                    InputProcess(library);

                    break;
                case 6: // Süresi geçmiş kitapları listele
                    library.ListOverdueBooks();

                    InputProcess(library);
                    break;
                case 7: // Popüler ilk 5 kitap ödünç alınma sayısına göre
                    library.ListPopularBooks();

                    InputProcess(library);
                    break;
                case 0: // Kapat
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Lütfen geçerli bir seçim yapın.");
                    InputProcess(library);
                    break;
            }
        }
    }

    public static void InputAddBook(Library Lib)
    {


        Console.WriteLine("Kitap eklemek için aşağıdaki bilgileri girin:");
        Console.Write("Başlık: ");
        string title = Console.ReadLine();


        Console.WriteLine();
        Console.Write("Yazar: ");
        string author = Console.ReadLine();


        Console.WriteLine();
        Console.WriteLine("(ISBN 13 basamaklı bir sayı olmalıdır ve daha önce kullanılmamış ISBN giriniz)");
        Console.Write("ISBN: ");
        string isbn = Console.ReadLine();

        Console.WriteLine();

        bool isVerifed = Lib.CheckIsbn(isbn);
        while (!isVerifed)
        {
            Console.WriteLine();
            Console.WriteLine("Bu ISBN hatalı lütfen gereksinimlere uyarak tekrar yazınız.");
            Console.WriteLine();
            Console.Write("ISBN: ");
            isbn = Console.ReadLine();
            Console.WriteLine();

            isVerifed = Lib.CheckIsbn(isbn);
        }


        Console.WriteLine();
        Console.Write("Kopya Sayısı: ");
        string copyCountInput = Console.ReadLine();
        int copyCount = -1;

        
        bool valid = false;
        while (!valid)
        {
            try
            {
                copyCount = Convert.ToInt32(copyCountInput);
                valid = true;
            }
            catch (FormatException)
            {
                Console.WriteLine();
                Console.WriteLine("Lütfen geçerli bir sayı girin.");

                Console.WriteLine();
                Console.Write("Kopya Sayısı: ");
                copyCountInput = Console.ReadLine();
            }
        }


        Console.WriteLine();
        Book book = new Book(title, author, isbn, copyCount);

        bool added = false;
        try
        {
            Lib.AddBook(book);
            added = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Kitap eklenirken bir hata oluştu: " + ex.Message);

            InputProcess(Lib);
        }

        if (added)
        {
            Console.WriteLine();

            Console.WriteLine(title + " başlıklı kitap kütüphaneye eklendi.");

            Console.WriteLine();

            InputProcess(Lib);
        }

    }
    static void Main(string[] args)
    {

        Library library = new Library();
        InputProcess(library);
        
    }
}
