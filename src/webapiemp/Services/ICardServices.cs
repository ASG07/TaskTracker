using webapiemp.DTOs.RequestDTO;
using webapiemp.Models;

namespace webapiemp.Services;

public interface ICardServices
{
    Task UpdateCardAsync(NewCardDTO newCardDTO, Card card);
    Task<Card> GetCardAsync(int cardId);
    Task<GroupMembership> GetMemberAsync(int userId, int groupId);
    Task CreateCardAsync(Card card);
}
