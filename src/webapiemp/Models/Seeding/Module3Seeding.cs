using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace webapiemp.Models.Seeding;

public static class Module3Seeding
{
    public static void Seed(ModelBuilder modelBuilder)
    {

        //var group1 = new Group { Id = 1, Name = "hospital devs group", InvitationCode = "a" };
        //var group2 = new Group { Id = 2, Name = "college project", InvitationCode = "s" };

        //modelBuilder.Entity<Group>().HasData(group1, group2);



        //var ahmed = new User { Id = 1, Name = "ahmed", Email = "ahmed@hotmail.com" };
        //var ahmed = new User
        //{
        //    Id = 1,
        //    Email = “frankofoedu@gmail.com”,
        //    EmailConfirmed = true,
        //    FirstName = “Frank”,
        //    LastName = “Ofoedu”,
        //    UserName = “frankofoedu@gmail.com",
        //    NormalizedUserName = “FRANKOFOEDU@GMAIL.COM"
        //};
        //PasswordHasher<User> ph = new PasswordHasher<ahmed>();
        //ahmed.PasswordHash = ph.HashPassword(ahmed, "mypassword_ ?");

        //var fahad = new User { Id = 2, Name = "fahad", Email = "fahad@hotmail.com", password = "12345" };
        //var abdullah = new User { Id = 3, Name = "abdullah", Email = "abdullah@hotmail.com", password = "12345" };
        //var saad = new User { Id = 4, Name = "saad", Email = "saad@hotmail.com", password = "12345" };
        //var hamed = new User { Id = 5, Name = "hamed", Email = "hamed@hotmail.com", password = "12345" };

        //modelBuilder.Entity<User>().HasData(ahmed, fahad, abdullah, saad, hamed);



        //var membership1 = new GroupMembership { UserId = ahmed.Id, GroupId = group1.Id, Role = "admin", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0 };
        //var membership2 = new GroupMembership { UserId = fahad.Id, GroupId = group1.Id, Role = "member", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0 };
        //var membership3 = new GroupMembership { UserId = fahad.Id, GroupId = group2.Id, Role = "admin", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0 };
        //var membership4 = new GroupMembership { UserId = abdullah.Id, GroupId = group2.Id, Role = "member", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0 };
        //var membership5 = new GroupMembership { UserId = saad.Id, GroupId = group2.Id, Role = "member", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0 };
        //var membership6 = new GroupMembership { UserId = hamed.Id, GroupId = group2.Id, Role = "member", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0 };

        //modelBuilder.Entity<GroupMembership>().HasData(membership1, membership2, membership3, membership4, membership5, membership6);



        //var card1 = new Card { Id = 1, title = "app launching issue", Description = "app does not respond after launch sometimes", State = "inProgress", Date = new DateTime(2022, 4, 3), Priority = "high", GroupId = group1.Id, AuthorId = ahmed.Id, ResponderId = fahad.Id };
        //var card2 = new Card { Id = 2, title = "app screen issue", Description = "screen does not respond after launch sometimes", State = "finished", Date = new DateTime(2022, 4, 3), Priority = "normal", GroupId = group1.Id, AuthorId = fahad.Id, ResponderId = ahmed.Id };
        //var card3 = new Card { Id = 3, title = "iphone does not unlock after swiping", Description = "sometimes when the user disable the bluetooth functiuality then lock the phone", State = "finished", Date = new DateTime(2022, 4, 3), Priority = "very-high", GroupId = group2.Id, AuthorId = fahad.Id, ResponderId = abdullah.Id };
        //var card4 = new Card { Id = 4, title = "app launching issue", Description = "app does not respond after launch sometimes", State = "inProgress", Date = new DateTime(2022, 4, 3), Priority = "low", GroupId = group2.Id, AuthorId = abdullah.Id, ResponderId = hamed.Id };
        //var card5 = new Card { Id = 5, title = "app launching issue", Description = "app does not respond after launch sometimes", State = "analysis", Date = new DateTime(2022, 4, 3), Priority = "normal", GroupId = group2.Id, AuthorId = hamed.Id, ResponderId = fahad.Id };
        //var card6 = new Card { Id = 6, title = "app launching issue", Description = "app does not respond after launch sometimes", State = "inProgress", Date = new DateTime(2022, 4, 3), Priority = "high", GroupId = group2.Id, AuthorId = abdullah.Id, ResponderId = saad.Id };

        //modelBuilder.Entity<Card>().HasData(card1, card2, card3, card4, card5, card6);
    }
}
