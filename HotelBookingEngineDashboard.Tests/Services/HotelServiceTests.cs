using FluentAssertions;
using HotelBookingEngineDashboard.Application.Exceptions;
using HotelBookingEngineDashboard.Application.Interfaces;
using HotelBookingEngineDashboard.Application.Services;
using HotelBookingEngineDashboard.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Tests.Services
{
    public class HotelServiceTests
    {
        private readonly Mock<IHotelRepository> _hotelRepoMock;
        private readonly HotelService _sut;

        public HotelServiceTests()
        {
            _hotelRepoMock = new Mock<IHotelRepository>();
            var logger = new Mock<ILogger<HotelService>>();
            _sut = new HotelService(_hotelRepoMock.Object, logger.Object);
        }

        // Helpers 

        private void SetupHotels(string? city, List<Hotel> hotels, int page = 1, int pageSize = 6) =>
    _hotelRepoMock
        .Setup(r => r.GetAllAsync(city, page, pageSize))
        .ReturnsAsync((hotels, hotels.Count));

        private void SetupHotel(int id, Hotel? hotel) =>
            _hotelRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(hotel);

        private static List<Hotel> SampleHotels() => new()
        {
            new Hotel { HotelId = 1, Name = "The Grand Oberoi", City = "Mumbai", StarRating = 5, PricePerNight = 12000 },
            new Hotel { HotelId = 2, Name = "Lemon Tree", City = "Pune", StarRating = 4, PricePerNight = 5500 }
        };

        private static Hotel SampleHotel() => new()
        {
            HotelId = 1,
            Name = "The Grand Oberoi",
            City = "Mumbai",
            StarRating = 5,
            PricePerNight = 12000,
            Description = "Luxury hotel in Mumbai",
            IsAvailable = true
        };

        // GetAllHotelsAsync Tests 

        [Fact]
        public async Task GetAllHotelsAsync_GivenNoFilter_ReturnsAllHotels()
        {
            // Arrange
            SetupHotels(null, SampleHotels());

            // Act
            var result = await _sut.GetAllHotelsAsync();

            // Assert
            result.Items.Should().HaveCount(2);
            result.Items.First().Name.Should().Be("The Grand Oberoi");
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetAllHotelsAsync_GivenCityFilter_ReturnsFilteredHotels()
        {
            // Arrange
            SetupHotels("Mumbai", new List<Hotel> { SampleHotel() });

            // Act
            var result = await _sut.GetAllHotelsAsync("Mumbai");

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().City.Should().Be("Mumbai");
        }

        [Fact]
        public async Task GetAllHotelsAsync_GivenNoHotelsExist_ReturnsEmptyList()
        {
            // Arrange
            SetupHotels(null, new List<Hotel>());

            // Act
            var result = await _sut.GetAllHotelsAsync();

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        // GetHotelByIdAsync Tests 

        [Fact]
        public async Task GetHotelByIdAsync_GivenValidId_ReturnsHotelDetail()
        {
            // Arrange
            SetupHotel(1, SampleHotel());

            // Act
            var result = await _sut.GetHotelByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.HotelId.Should().Be(1);
            result.Name.Should().Be("The Grand Oberoi");
            result.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task GetHotelByIdAsync_GivenInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            SetupHotel(99, null);

            // Act
            var act = async () => await _sut.GetHotelByIdAsync(99);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Fact]
        public async Task GetHotelByIdAsync_GivenValidId_CallsRepositoryOnce()
        {
            // Arrange
            SetupHotel(1, SampleHotel());

            // Act
            await _sut.GetHotelByIdAsync(1);

            // Assert
            _hotelRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }
    }
}
