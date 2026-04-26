using MyGateway.DTOs.Downstream;

namespace MyGateway.HttpClients;

public class BookingClient : IBookingClient
{
    private readonly HttpClient _httpClient;

    public BookingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ReservationDto?> GetReservationAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/reservations/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ReservationDto>();
    }
}
