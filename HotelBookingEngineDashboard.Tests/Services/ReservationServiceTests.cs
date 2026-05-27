using FluentAssertions;
using HotelBookingEngineDashboard.Application.DTOs.Reservations;
using HotelBookingEngineDashboard.Application.Exceptions;
using HotelBookingEngineDashboard.Application.Interfaces;
using HotelBookingEngineDashboard.Application.Services;
using HotelBookingEngineDashboard.Domain.Entities;
using HotelBookingEngineDashboard.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace HotelBookingEngineDashboard.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly Mock<IHotelRepository> _hotelRepoMock;
        private readonly ReservationService _sut;

        public ReservationServiceTests()
        {
            _reservationRepoMock = new Mock<IReservationRepository>();
            _hotelRepoMock = new Mock<IHotelRepository>();
            var logger = new Mock<ILogger<ReservationService>>();
            _sut = new ReservationService(
                _reservationRepoMock.Object,
                _hotelRepoMock.Object,
                logger.Object);
        }

        // ── Helpers ───────────────────────────────────────────────────

        private void SetupHotel(int id, Hotel? hotel) =>
            _hotelRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(hotel);

        private void SetupReservation(int id, Reservation? reservation) =>
            _reservationRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(reservation);

        private static Hotel ValidHotel() => new()
        {
            HotelId = 1,
            Name = "Test Hotel"
        };

        private static CreateReservationDto ValidCreateDto(
            int hotelId = 1,
            int guestAge = 25,
            int numberOfGuests = 2,
            DateTime? checkIn = null,
            DateTime? checkOut = null) => new()
            {
                HotelId = hotelId,
                GuestName = "Kunal",
                GuestAge = guestAge,
                NumberOfGuests = numberOfGuests,
                CheckInDate = checkIn ?? DateTime.Today.AddDays(1),
                CheckOutDate = checkOut ?? DateTime.Today.AddDays(3)
            };

        private static UpdateReservationDto ValidUpdateDto(
            int guestAge = 25,
            int numberOfGuests = 2,
            DateTime? checkIn = null,
            DateTime? checkOut = null) => new()
            {
                GuestName = "Kunal",
                GuestAge = guestAge,
                NumberOfGuests = numberOfGuests,
                CheckInDate = checkIn ?? DateTime.Today.AddDays(1),
                CheckOutDate = checkOut ?? DateTime.Today.AddDays(3)
            };

        private static Reservation ConfirmedReservation() => new()
        {
            ReservationId = 1,
            Status = ReservationStatus.CONFIRMED,
            Hotel = ValidHotel(),
            HotelId = 1,
            GuestName = "Kunal",
            GuestAge = 25,
            NumberOfGuests = 2,
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3)
        };

        private static Reservation CancelledReservation() => new()
        {
            ReservationId = 1,
            Status = ReservationStatus.CANCELLED
        };

        // ── CreateAsync Tests ─────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_GivenHotelNotFound_ThrowsNotFoundException()
        {
            SetupHotel(99, null);

            var act = async () => await _sut.CreateAsync(ValidCreateDto(hotelId: 99));

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(17)]
        public async Task CreateAsync_GivenGuestAgeBelowMinimum_ThrowsBadRequestException(int age)
        {
            SetupHotel(1, ValidHotel());

            var act = async () => await _sut.CreateAsync(ValidCreateDto(guestAge: age));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*18*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_GivenInvalidNumberOfGuests_ThrowsBadRequestException(int numberOfGuests)
        {
            SetupHotel(1, ValidHotel());

            var act = async () => await _sut.CreateAsync(ValidCreateDto(numberOfGuests: numberOfGuests));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*guest*");
        }

        [Fact]
        public async Task CreateAsync_GivenPastCheckInDate_ThrowsBadRequestException()
        {
            SetupHotel(1, ValidHotel());

            var act = async () => await _sut.CreateAsync(
                ValidCreateDto(checkIn: DateTime.Today.AddDays(-1)));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Check-in*");
        }

        [Fact]
        public async Task CreateAsync_GivenCheckOutBeforeCheckIn_ThrowsBadRequestException()
        {
            SetupHotel(1, ValidHotel());

            var act = async () => await _sut.CreateAsync(
                ValidCreateDto(
                    checkIn: DateTime.Today.AddDays(5),
                    checkOut: DateTime.Today.AddDays(2)));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Check-out*");
        }

        [Fact]
        public async Task CreateAsync_GivenValidData_ReturnsConfirmedReservation()
        {
            // Arrange
            var hotel = ValidHotel();
            var reservation = new Reservation
            {
                ReservationId = 1,
                BookingRef = "BK-A1B2C3D4",
                HotelId = 1,
                Hotel = hotel,
                GuestName = "Kunal",
                GuestAge = 25,
                NumberOfGuests = 2,
                CheckInDate = DateTime.Today.AddDays(1),
                CheckOutDate = DateTime.Today.AddDays(3),
                Status = ReservationStatus.CONFIRMED
            };

            SetupHotel(1, hotel);

            _reservationRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _reservationRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(reservation);

            // Act
            var (success, message, data) = await _sut.CreateAsync(ValidCreateDto());

            // Assert
            success.Should().BeTrue();
            data.Should().NotBeNull();
            data!.GuestName.Should().Be("Kunal");
        }

        [Fact]
        public async Task CreateAsync_GivenValidData_CallsCreateRepositoryOnce()
        {
            // Arrange
            var hotel = ValidHotel();
            var reservation = ConfirmedReservation();
            reservation.BookingRef = "BK-A1B2C3D4";

            SetupHotel(1, hotel);

            _reservationRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _reservationRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(reservation);

            // Act
            await _sut.CreateAsync(ValidCreateDto());

            // Assert
            _reservationRepoMock.Verify(
                r => r.CreateAsync(It.IsAny<Reservation>()),
                Times.Once);
        }

        // ── CancelAsync Tests ─────────────────────────────────────────

        [Fact]
        public async Task CancelAsync_GivenReservationNotFound_ThrowsNotFoundException()
        {
            SetupReservation(99, null);

            var act = async () => await _sut.CancelAsync(99);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Fact]
        public async Task CancelAsync_GivenAlreadyCancelledReservation_ThrowsBadRequestException()
        {
            SetupReservation(1, CancelledReservation());

            var act = async () => await _sut.CancelAsync(1);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*already cancelled*");
        }

        [Fact]
        public async Task CancelAsync_GivenConfirmedReservation_ReturnsSuccess()
        {
            // Arrange
            SetupReservation(1, ConfirmedReservation());

            _reservationRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);

            // Act
            var (success, message) = await _sut.CancelAsync(1);

            // Assert
            success.Should().BeTrue();
            message.Should().Be("Reservation cancelled successfully.");
        }

        [Fact]
        public async Task CancelAsync_GivenConfirmedReservation_SetsStatusToCancelled()
        {
            // Arrange
            SetupReservation(1, ConfirmedReservation());

            _reservationRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.CancelAsync(1);

            // Assert
            _reservationRepoMock.Verify(
                r => r.UpdateAsync(
                    It.Is<Reservation>(r => r.Status == ReservationStatus.CANCELLED)),
                Times.Once);
        }

        // ── UpdateAsync Tests ─────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_GivenReservationNotFound_ThrowsNotFoundException()
        {
            SetupReservation(99, null);

            var act = async () => await _sut.UpdateAsync(99, ValidUpdateDto());

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Fact]
        public async Task UpdateAsync_GivenCancelledReservation_ThrowsBadRequestException()
        {
            SetupReservation(1, CancelledReservation());

            var act = async () => await _sut.UpdateAsync(1, ValidUpdateDto());

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Cancelled*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        public async Task UpdateAsync_GivenInvalidAge_ThrowsBadRequestException(int age)
        {
            SetupReservation(1, ConfirmedReservation());

            var act = async () => await _sut.UpdateAsync(1, ValidUpdateDto(guestAge: age));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*18*");
        }

        [Fact]
        public async Task UpdateAsync_GivenValidData_ReturnsSuccess()
        {
            // Arrange
            SetupReservation(1, ConfirmedReservation());

            _reservationRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);

            // Act
            var (success, message) = await _sut.UpdateAsync(1, ValidUpdateDto());

            // Assert
            success.Should().BeTrue();
            message.Should().Be("Reservation updated successfully.");
        }

        [Fact]
        public async Task UpdateAsync_GivenValidData_CallsUpdateRepositoryOnce()
        {
            // Arrange
            SetupReservation(1, ConfirmedReservation());

            _reservationRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.UpdateAsync(1, ValidUpdateDto());

            // Assert
            _reservationRepoMock.Verify(
                r => r.UpdateAsync(It.IsAny<Reservation>()),
                Times.Once);
        }

        // ── GetAllAsync Tests ─────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_GivenReservationsExist_ReturnsAllReservations()
        {
            // Arrange
            var reservations = new List<Reservation>
            {
                new Reservation { ReservationId = 1, BookingRef = "BK-10001", Hotel = ValidHotel(), Status = ReservationStatus.CONFIRMED },
                new Reservation { ReservationId = 2, BookingRef = "BK-10002", Hotel = ValidHotel(), Status = ReservationStatus.CANCELLED }
            };

            _reservationRepoMock
                .Setup(r => r.GetAllAsync(1, 10))
                .ReturnsAsync((reservations, reservations.Count));

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Items.Should().HaveCount(2);
            result.Items.First().BookingRef.Should().Be("BK-10001");
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetAllAsync_GivenNoReservations_ReturnsEmptyList()
        {
            // Arrange
            _reservationRepoMock
                .Setup(r => r.GetAllAsync(1, 10))
                .ReturnsAsync((new List<Reservation>(), 0));

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task GetAllAsync_GivenReservationsExist_CallsRepositoryOnce()
        {
            // Arrange
            _reservationRepoMock
                .Setup(r => r.GetAllAsync(1, 10))
                .ReturnsAsync((new List<Reservation>(), 0));

            // Act
            await _sut.GetAllAsync();

            // Assert
            _reservationRepoMock.Verify(r => r.GetAllAsync(1, 10), Times.Once);
        }

        // ── GetByIdAsync Tests ────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_GivenValidId_ReturnsReservation()
        {
            // Arrange
            var reservation = ConfirmedReservation();
            reservation.BookingRef = "BK-10001";

            SetupReservation(1, reservation);

            // Act
            var result = await _sut.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.BookingRef.Should().Be("BK-10001");
            result.Status.Should().Be("CONFIRMED");
        }

        [Fact]
        public async Task GetByIdAsync_GivenInvalidId_ThrowsNotFoundException()
        {
            SetupReservation(99, null);

            var act = async () => await _sut.GetByIdAsync(99);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Fact]
        public async Task GetByIdAsync_GivenValidId_CallsRepositoryOnce()
        {
            // Arrange
            SetupReservation(1, ConfirmedReservation());

            // Act
            await _sut.GetByIdAsync(1);

            // Assert
            _reservationRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        // ── GetByUserIdAsync Tests ────────────────────────────────────

        [Fact]
        public async Task GetByUserIdAsync_GivenValidUserId_ReturnsUserReservations()
        {
            // Arrange
            var reservations = new List<Reservation>
            {
                new Reservation { ReservationId = 1, BookingRef = "BK-10001", Hotel = ValidHotel(), Status = ReservationStatus.CONFIRMED, UserId = 1 },
                new Reservation { ReservationId = 2, BookingRef = "BK-10002", Hotel = ValidHotel(), Status = ReservationStatus.CONFIRMED, UserId = 1 }
            };

            _reservationRepoMock
                .Setup(r => r.GetByUserIdAsync(1, 1, 10))
                .ReturnsAsync((reservations, reservations.Count));

            // Act
            var result = await _sut.GetByUserIdAsync(1);

            // Assert
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetByUserIdAsync_GivenUserWithNoReservations_ReturnsEmptyList()
        {
            // Arrange
            _reservationRepoMock
                .Setup(r => r.GetByUserIdAsync(99, 1, 10))
                .ReturnsAsync((new List<Reservation>(), 0));

            // Act
            var result = await _sut.GetByUserIdAsync(99);

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task GetByUserIdAsync_GivenValidUserId_CallsRepositoryOnce()
        {
            // Arrange
            _reservationRepoMock
                .Setup(r => r.GetByUserIdAsync(1, 1, 10))
                .ReturnsAsync((new List<Reservation>(), 0));

            // Act
            await _sut.GetByUserIdAsync(1);

            // Assert
            _reservationRepoMock.Verify(
                r => r.GetByUserIdAsync(1, 1, 10),
                Times.Once);
        }
    }
}