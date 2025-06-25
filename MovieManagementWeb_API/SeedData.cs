using MovieManagementWeb_API.Models.Entities;
using MovieManagementWeb_API.Data;

namespace MovieManagementWeb_API
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            DateTime Utc(int y, int m, int d, int h = 0, int min = 0, int s = 0)
            {
                return DateTime.SpecifyKind(new DateTime(y, m, d, h, min, s), DateTimeKind.Utc);
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(new List<User>
        {
            new User
            {
                Username = "admin",
                Password = "admin123",
                FullName = "Admin User",
                BirthDate = Utc(1990, 1, 1),
                Gender = "Nam",
                Email = "admin@example.com",
                IdCard = "0123456789",
                PhoneNumber = "0901234567",
                Address = "Admin Street",
                Role = "Admin",
                IsLocked = false
            },
            new User
            {
                Username = "user1",
                Password = "user123",
                FullName = "Normal User",
                BirthDate = Utc(2000, 12, 12),
                Gender = "Nam",
                Email = "user@example.com",
                IdCard = "1122334455",
                PhoneNumber = "0911223344",
                Address = "User Street",
                Role = "User",
                IsLocked = false
            }
        });
                context.SaveChanges();
            }
            
        //    var genres = new List<Genre>();
        //    var genreNames = new[] { "Action", "Sci-Fi" };
        //    foreach (var name in genreNames)
        //    {
        //        var genre = context.Genres.FirstOrDefault(g => g.Name == name);
        //        if (genre == null)
        //        {
        //            genre = new Genre { Name = name };
        //            context.Genres.Add(genre);
        //        }
        //        genres.Add(genre);
        //    }

        //    var actors = new List<Actor>();
        //    var actorNames = new[] { "Robert Downey Jr.", "Chris Evans" };
        //    foreach (var name in actorNames)
        //    {
        //        var actor = context.Actors.FirstOrDefault(a => a.Name == name);
        //        if (actor == null)
        //        {
        //            actor = new Actor { Name = name };
        //            context.Actors.Add(actor);
        //        }
        //        actors.Add(actor);
        //    }

        //    context.SaveChanges();

        //    if (!context.Movies.Any())
        //    {
        //        var movie = new Movie
        //        {
        //            Title = "Avengers: Endgame",
        //            Image = "http://localhost:7239/uploads/abc.jpg",
        //            CarouselImage = "http://localhost:7239/uploads/abc.jpg",
        //            TrailerLink = "https://youtube.com/watch?v=abc",
        //            Subtitle = "Hồi kết của Avengers",
        //            RatingCode = "P",
        //            Duration = 120,
        //            Director = "Anthony Russo",
        //            Cast = "Robert Downey Jr., Chris Evans",
        //            Description = "Cuộc chiến chống lại Thanos",
        //            ProductionCompany = "Marvel Studios",
        //            Format = new List<string> { "2D", "IMAX" },
        //            ReleaseDate = Utc(2025, 6, 24),
        //            CreatedByUsername = "admin",
        //            EditedByUsername = "admin",
        //            Showtimes = new List<Showtime>
        //    {
        //        new Showtime
        //        {
        //            Date = Utc(2025, 6, 24),
        //            Time = "18:30:00",
        //            RoomName = "Room 1"
        //        }
        //    },
        //            MovieGenres = genres.Select(g => new MovieGenre { GenreId = g.Id }).ToList(),
        //            ActorMovies = actors.Select(a => new ActorMovie { ActorId = a.Id }).ToList()
        //        };

        //        context.Movies.Add(movie);
        //        context.SaveChanges();
        //    }
        }
      }
    }