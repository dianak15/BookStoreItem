using System.Globalization;

[assembly: CLSCompliant(true)]

namespace BookStoreItem
{
    public class BookStoreItem
    {
        private readonly string authorName;
        private readonly string? isni;
        private readonly bool hasIsni;
        private decimal price;
        private string currency = "USD";
        private int amount;

        public BookStoreItem(string author, string title, string publisher, string isbn)
            : this(author, title, publisher, isbn, 0m, "USD", 0, string.Empty, null, null)
        {
        }

        public BookStoreItem(string author, string title, string publisher, string isbn, string? isni)
            : this(author, title, publisher, isbn, 0m, "USD", 0, string.Empty, isni, null)
        {
        }

        public BookStoreItem(string author, string title, string publisher, string isbn, decimal price, string currency, int amount, string bookBinding = " ", DateTime? published = null)
            : this(author, title, publisher, isbn, price, currency, amount, bookBinding, null, published)
        {
        }

        public BookStoreItem(string author, string title, string publisher, string isbn, decimal price, string currency, int amount, string bookBinding = " ", string? isni = null, DateTime? published = null)
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Author name must have at least one letter character.");
            }

            this.authorName = author;

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title must have at least one letter character.");
            }

            this.Title = title;

            if (string.IsNullOrWhiteSpace(publisher))
            {
                throw new ArgumentException("Publisher must have at least one letter character.");
            }

            this.Publisher = publisher;

            if (!ValidateIsbnFormat(isbn))
            {
                throw new ArgumentException("Invalid ISBN format. An ISBN code must have ten characters, each being a digit or 'X'.");
            }

            if (!ValidateIsbnChecksum(isbn))
            {
                throw new ArgumentException("Invalid ISBN checksum. The ISBN code is not valid.");
            }

            this.Isbn = isbn;

            if (isni != null)
            {
                if (!ValidateIsni(isni))
                {
                    throw new ArgumentException("Invalid ISNI format. An ISNI code must have sixteen characters, each being a digit or 'X'.");
                }

                this.isni = isni;
                this.hasIsni = true;
            }

            this.Published = published;

            this.BookBinding = bookBinding;

            this.Price = price;

            this.Currency = currency;

            this.Amount = amount;
        }

        public string AuthorName 
        { 
            get { return this.authorName; } 
        }

        public string Title 
        { 
            get; 
            private set; 
        }

        public string Publisher 
        { 
            get; 
            private set; 
        }

        public string? Isni 
        { 
            get { return this.isni; } 
        }

        public bool HasIsni 
        { 
            get { return this.hasIsni; } 
        }

        public string Isbn 
        { 
            get; 
            private set; 
        }

        public DateTime? Published 
        { 
            get; 
            set; 
        }

        public string? BookBinding 
        { 
            get; 
            set; 
        }

        public decimal Price
        {
            get { return this.price; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Price), "Price must be greater or equal to zero.");
                }

                this.price = value;
            }
        }

        public string Currency
        {
            get { return this.currency; }          
            set
            {
                ThrowExceptionIfCurrencyIsNotValid(value);
                this.currency = value;
            }
        }

        public int Amount
        {
            get { return this.amount; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Amount), "Amount must be greater or equal to zero.");
                }

                this.amount = value;
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                var bookstoreitem = new BookStoreItem("Joe", "Book Title", "Publisher Name", "0306406152", 25.99m, "USD", 10, string.Empty, "123456789012345X", DateTime.Now);

                Console.WriteLine(bookstoreitem.AuthorName + ", " + bookstoreitem.Title + ", " + bookstoreitem.Publisher + ", " + bookstoreitem.Isni + ", " + bookstoreitem.HasIsni + ", Valid ISBN: " + bookstoreitem.Isbn + ", Published: " + bookstoreitem.Published + ", Binding: " + bookstoreitem.BookBinding + ", Price:" + bookstoreitem.Price + ", Currency: " + bookstoreitem.Currency + ", Amount: " + bookstoreitem.Amount);

                bookstoreitem.DisplayIsbnWithChecksum();

                Console.WriteLine("Is ISBN-10 checksum valid? " + bookstoreitem.IsValidIsbnChecksum());

                Console.WriteLine("ISNI URI: " + bookstoreitem.GetIsniUri());
                Console.WriteLine("ISBN Search URI: " + bookstoreitem.GetIsbnSearchUri());

                Console.WriteLine("Book ToString(): " + bookstoreitem.ToString());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static bool ValidateIsni(string isni)
        {
            if (isni == null || isni.Length != 16)
            {
                return false;
            }

            foreach (char c in isni)
            {
                if (!char.IsDigit(c) && c != 'X')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValidateIsbnFormat(string isbn)
        {
            if (isbn == null || isbn.Length != 10)
            {
                return false;
            }

            foreach (char c in isbn)
            {
                if (!char.IsDigit(c) && c != 'X')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValidateIsbnChecksum(string isbn)
        {
            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                int digit = (isbn[i] == 'X') ? 10 : int.Parse(isbn[i].ToString());
                sum += (10 - i) * digit;
            }

            return sum % 11 == 0;
        }

        public static int CalculateIsbnChecksum(string isbn)
        {
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int digit = (isbn[i] == 'X') ? 10 : int.Parse(isbn[i].ToString());
                sum += (10 - i) * digit;
            }

            int lastDigit = (isbn[9] == 'X') ? 10 : int.Parse(isbn[9].ToString());
            int checksum = (sum + lastDigit) % 11;
            return checksum == 10 ? 0 : checksum;
        }

        public void DisplayIsbnWithChecksum()
        {
            Console.WriteLine($"ISBN with checksum: {this.Isbn}-{CalculateIsbnChecksum(this.Isbn)}");
        }

        public bool IsValidIsbnChecksum()
        {
            return CalculateIsbnChecksum(this.Isbn) == 0;
        }

        public Uri GetIsniUri()
        {
            if (this.isni == null)
            {
                throw new InvalidOperationException("ISNI is not set.");
            }

            return new Uri($"https://isni.org/isni/{this.isni}");
        }

        public Uri GetIsbnSearchUri()
        {
            return new Uri($"https://isbnsearch.org/search?s={this.Isbn}");
        }

        public override string ToString()
        {
            if (this.isni == null)
            {
                return $"{this.Title}, {this.AuthorName}, \"{this.Price.ToString(CultureInfo.InvariantCulture)}\", {this.Currency}, {this.Amount}";
            }
            else
            {
                return $"{this.Title}, {this.AuthorName}, {this.isni}, \"{this.Price.ToString(CultureInfo.InvariantCulture)}\", {this.Currency}, {this.Amount}";
            }
        }

        private static void ThrowExceptionIfCurrencyIsNotValid(string currency)
        {
            if (currency == null || currency.Length != 3)
            {
                throw new ArgumentException("Currency must have three characters.");
            }

            foreach (char c in currency)
            {
                if (!char.IsLetter(c))
                {
                    throw new ArgumentException("Currency must contain only letters.");
                }
            }
        }
    }
}
