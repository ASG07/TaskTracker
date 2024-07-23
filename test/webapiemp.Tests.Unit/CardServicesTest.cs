using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using webapiemp.Models;
using webapiemp.Services;

namespace webapiemp.Tests.Unit;
public class CardServicesTest
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Card> _dbSet;
    private readonly CardServices _cardService;

    public CardServicesTest()
    {
        _context = Substitute.For<ApplicationDbContext>();
        _dbSet = Substitute.For<DbSet<Card>>();
        _context.Set<Card>().Returns(_dbSet);

        _cardService = new CardServices(_context);
    }

    [Fact]
    public async Task GetCardAsync_ShouldReturnCard_WhenCardExists()
    {
        // Arrange
        var cardId = 1;
        var expectedCard = new Card { Id = cardId, title = "Test Card" };

        _dbSet.FirstOrDefaultAsync(Arg.Any<Expression<Func<Card, bool>>>())
              .Returns(Task.FromResult(expectedCard));

        // Act
        var result = await _cardService.GetCardAsync(cardId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedCard);


    }
}
