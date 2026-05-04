using MyGateway.DTOs.Downstream;

namespace MyGateway.HttpClients;

public class BillingClient : IBillingClient
{
    private readonly HttpClient _httpClient;

    public BillingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BillDto?> GetBillByReservationAsync(Guid reservationId)
    {
        var response = await _httpClient.GetAsync($"/bills/reservation/{reservationId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BillDto>();
    }
}
