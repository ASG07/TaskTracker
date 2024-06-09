using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapiemp.Services;
using webapiemp.Models;
using webapiemp.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Collections;
using webapiemp.DTOs.RequestDTO;
using webapiemp.Extensions;

namespace webapiemp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CardsController : ControllerBase
{

    private readonly ApplicationDbContext _context;
    private readonly IAuthServices _authServices;
    private readonly ICardServices _cardServices;

    public CardsController(ApplicationDbContext context, IAuthServices authServices, ICardServices cardServices)
    {
        _context = context;
        _authServices = authServices;
        _cardServices = cardServices;
    }

    //[HttpGet]
    //public async Task<ActionResult<List<Card>>> GetAll()
    //{
    //    return await CardServices.GetCards(_context);
    //}

    //[HttpGet("{id}")]
    //public async Task<ActionResult<Card>> GetsCard(int id)
    //{


    //    var Card = await CardServices.GetCardById(id, _context);

    //    if (Card == null) return NotFound();

    //    return Card;
    //}

    [HttpDelete("{cardId}")]
    [Authorize]
    public async Task<ActionResult> Delete(int cardId)
    {
        //check if user exist
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Unauthorized();

        //check if card exist
        var card = await _cardServices.GetCardAsync(cardId);
        if (card == null) return NotFound("card not found");


        //check if the user is the author or the admin
        if (!await _authServices.IsAuthorAsync(userId, cardId) && !_authServices.IsAdmin(userId, card.GroupId)) return Unauthorized("you are not the Author or Group's Admin");

        //---user Authorization is done---//


        _context.Cards.Remove(card);
        await _context.SaveChangesAsync();
        return Ok();
    }



    [HttpPost("{groupId}")]
    [Authorize]
    public async Task<ActionResult> Create(NewCardDTO card, int groupId)
    {
        if (card == null) BadRequest();

        //Get userId
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Unauthorized("Token is invalid");


        //check if Author is a memeber of specified group
        var authorMembership = await _cardServices.GetMemberAsync(userId, groupId);
        if (authorMembership == null) return BadRequest("member does not exist");

        if (card.Priority != "low" && card.Priority != "normal" && card.Priority != "high" && card.Priority != "very-high") return BadRequest("priorty is incorrect");
        if (card.State != "analysis" && card.State != "in_progress" && card.State != "finished") return BadRequest("state is incorrect");

        //check if Assignee is a memeber of specified group
        var assigneeMembership = await _cardServices.GetMemberAsync(card.AssigneeId, groupId);


        //check if no assignee was set by the author
        if (card.AssigneeId != 0)
        {
            if (assigneeMembership == null) return BadRequest("member does not exist");
        }

        var newCard = new Card
        {
            title = card.title,
            Description = card.Description,
            Priority = card.Priority,
            State = card.State,
            Date = DateTime.Now,
            AuthorId = authorMembership.UserId,
            ResponderId = assigneeMembership != null ? assigneeMembership.UserId : null,
            GroupId = groupId,
        };

        authorMembership.AmountBugsSubmitted++;
        if (assigneeMembership != null) assigneeMembership.AmountWorkingOn++;
        await _context.SaveChangesAsync();

        await _cardServices.CreateCardAsync(newCard);

        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var location = baseUrl + "/" + newCard.Id;
        return Created(location, new { newCard.Id, newCard.title, newCard.Description, newCard.Priority, newCard.AuthorId });
    }



    [HttpPut("{cardId}")]
    [Authorize]
    public async Task<ActionResult> Update(NewCardDTO newCardDTO, int cardId)
    {
        if (newCardDTO == null) BadRequest();

        //check if user exist
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Unauthorized();

        //check if card exist
        var card = await _cardServices.GetCardAsync(cardId);
        if (card == null) return NotFound("card not found");

        //check if the user is author/assignee/admin
        if (!await _authServices.IsAuthorAsync(userId, cardId) &&
            !await _authServices.IsAssigneeAsync(userId, cardId)&& 
            !_authServices.IsAdmin(userId, card.GroupId)) 
                return Unauthorized("you are not the Author or Group's Admin");

        if (newCardDTO.Priority != "low" && newCardDTO.Priority != "normal" && newCardDTO.Priority != "high" && newCardDTO.Priority != "very-high") return BadRequest("priorty is incorrect");
        if (newCardDTO.State != "analysis" && newCardDTO.State != "in_progress" && newCardDTO.State != "finished") return BadRequest("state is incorrect");

        await _cardServices.UpdateCardAsync(newCardDTO, card);
        return Ok();

    }



    [HttpGet("{GroupId}")]
    [Authorize]
    public async Task<ActionResult<List<CardDTO>>> GetCardsWithUsersInfo(int GroupId)
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Unauthorized();

        if (!await _authServices.IsMember(userId, GroupId)) return Forbid();

        var query = from card in _context.Cards

                    join assignee_member in _context.GroupMemberships
                      on new { 
                          key1 = (int)card.ResponderId,
                          key2 = card.GroupId
                      } equals new { 
                          key1 = assignee_member.UserId,
                          key2 = assignee_member.GroupId
                      } into card_assi_mem
                    from assignee_member in card_assi_mem.DefaultIfEmpty()

                    join author_member in _context.GroupMemberships
                      on new { key1 = card.AuthorId, key2 = card.GroupId } equals new { key1 = author_member.UserId, key2 = author_member.GroupId } into card_auth_mem
                    from author_member in card_auth_mem.DefaultIfEmpty()

                    where card.GroupId == GroupId
                    //where author_member.GroupId == GroupId

                    //where assignee_member.GroupId == GroupId
                    select new CardDTO
                    {
                        Id = card.Id,
                        Title = card.title,
                        Description = card.Description,
                        Priority = card.Priority,
                        Date = card.Date,
                        State = card.State,
                        AuthorId = card.Author == null ? 0 : card.Author.Id,
                        AuthorName = card.Author == null ? "Removed" : card.Author.UserName,
                        AuthorAmountBugsSubmitted = author_member == null ? 0 : author_member.AmountBugsSubmitted,
                        AuthorAmountFinished = author_member == null ? 0 : author_member.AmountFinished,
                        AuthorAmountWorkingOn = author_member == null ? 0 : author_member.AmountWorkingOn,
                        AssigneeId = card.Responder == null ? 0 : card.Responder.Id,
                        AssigneeName = card.Responder == null ? "Not assigned yet" : card.Responder.UserName,
                        AssigneeAmountBugsSubmitted = assignee_member == null ? 0 : assignee_member.AmountBugsSubmitted,
                        AssigneeAmountFinished = assignee_member == null ? 0 : assignee_member.AmountFinished,
                        AssigneeAmountWorkingOn = assignee_member == null ? 0 : assignee_member.AmountWorkingOn,
                    };

        var list = query.ToList();
        return list;
    }

    [HttpPost("{cardId}/updateState")]
    [Authorize]
    public async Task<IActionResult> UpdateState(int cardId, [FromBody] string newState)
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Unauthorized();

        var card = await _cardServices.GetCardAsync(cardId);
        if (card == null) return NotFound("card not found");

        if (!await _authServices.IsAuthorAsync(userId, card.GroupId) && !_authServices.IsAdmin(userId, card.GroupId)) return NotFound();

        if (newState != "analysis" && newState != "in_progress" && newState != "finished") return BadRequest("state is incorrect");
        card.State = newState;

        await _context.SaveChangesAsync();
        return Ok(card);
    }

    [HttpPut("{cardId}/addAssignee/{assigneeId}")]
    [Authorize]
    public async Task<IActionResult> AddAssignee(int cardId, int assigneeId)
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Unauthorized();

        var card = await _cardServices.GetCardAsync(cardId);
        if (card == null) return NotFound("card not found");

        var assignee = await _authServices.GetMemberAsync(assigneeId,card.GroupId);
        if (assignee == null) return NotFound();

        if (!await _authServices.IsAuthorAsync(userId, card.Id) && !_authServices.IsAdmin(userId, card.GroupId)) return NotFound();
        card.ResponderId = assigneeId;
        //card.Responder = assignee;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{cardId}/archive")]
    [Authorize]
    public async Task<IActionResult> ArchiveCard(int cardId)
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Unauthorized();

        var card = await _cardServices.GetCardAsync(cardId);
        if (card == null) return NotFound("card not found");

        if (!_authServices.IsAdmin(userId, card.GroupId)) return NotFound();

        card.State = "archived";
        await _context.SaveChangesAsync();
        return Ok();
    }

}
