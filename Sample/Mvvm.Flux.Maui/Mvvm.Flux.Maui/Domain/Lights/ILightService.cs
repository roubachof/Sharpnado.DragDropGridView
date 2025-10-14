namespace Mvvm.Flux.Maui.Domain.Lights
{
    public interface ILightService
    {
        event EventHandler<Light> LightUpdated;

        Task<List<Light>> GetLightsAsync();

        Task<Light> GetLightAsync(int lightId);

        Task UpdateLightAsync(Light light);
    }
}