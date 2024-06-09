using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using webapiemp.DTOs.RequestDTO;
using webapiemp.Models;
using webapiemp.Services;
namespace webapiemp.Services
{
    public class CardServices : ICardServices
    {
        private readonly ApplicationDbContext _context;
        public CardServices(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task UpdateCardAsync(NewCardDTO newCardDTO, Card card)
        {
            card.title = newCardDTO.title;
            card.Description = newCardDTO.Description;
            card.Priority = newCardDTO.Priority;
            card.State = newCardDTO.State;

            await _context.SaveChangesAsync();
        }

        public async Task CreateCardAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
        }

        public async Task<Card> GetCardAsync(int cardId)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
            if (card == null) return null;
            return card;
        }

        public async Task<GroupMembership> GetMemberAsync(int userId, int groupId)
        {
            var member = await _context.GroupMemberships.FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);
            if (member == null) return null;

            return member;
        }



        




        //public static async Task<List<Card>> GetCards(ApplicationDbContext context)
        //{

        //    return await context.Cards.AsNoTracking().ToListAsync();
        //}

        //public static async Task AddCard(Card card, ApplicationDbContext context)
        //{
        //    Card newCard = new Card() { title = card.title, Description = card.Description, Priority = card.Priority, State = card.State, Date = DateTime.Now, AuthorId = card.AuthorId, ResponderId = card.ResponderId };

        //    await context.Cards.AddAsync(newCard);
        //    await context.SaveChangesAsync();
        //}

        //public static async Task<Card> GetCardById(int id, ApplicationDbContext context)
        //{
        //    return await context.Cards.FirstOrDefaultAsync(card => card.Id == id);
        //}

        //public static async Task DeleteCardById(int id, ApplicationDbContext context)
        //{
        //    Card card = await GetCardById(id, context);
        //    if (card != null)
        //    {
        //        context.Cards.Remove(card);
        //        await context.SaveChangesAsync();
        //    }
        //}

        //public static async Task UpadateCard(Card card, ApplicationDbContext context)
        //{
        //    var dbcard = await GetCardById(card.Id, context);
        //    if (dbcard == null) throw new IndexOutOfRangeException();

        //    dbcard.title = card.title;
        //    dbcard.Description = card.Description;
        //    dbcard.Priority = card.Priority;
        //    dbcard.State = card.State;

        //    await context.SaveChangesAsync();
        //}

    }
}
