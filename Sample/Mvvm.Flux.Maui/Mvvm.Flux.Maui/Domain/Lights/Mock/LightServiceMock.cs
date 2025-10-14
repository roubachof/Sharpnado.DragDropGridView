using MetroLog;

using Mvvm.Flux.Maui.Infrastructure.Logging;
using Mvvm.Flux.Maui.Infrastructure.Mocking;

namespace Mvvm.Flux.Maui.Domain.Lights.Mock
{
    public class LightServiceMock : ILightService
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(LightServiceMock));

        private readonly Dictionary<int, Light> _lights = new()
            {
                { 1, new Light(1, "Stroopwafel Lounge", "Gerald's primary filming studio, featuring a 4K camera setup and a suspicious amount of caramel waffles", false) },
                { 2, new Light(2, "Grand Living Room", "Where Gerald hosts his legendary 'stroopwafel tasting' livestreams for 5 million subscribers", true) },
                { 3, new Light(3, "Hall of Stormtroopers", "A 50-meter corridor lined with 200 life-size Stormtrooper statues. Gerald knows each by name", false) },
                { 14, new Light(14, "Imperial Ballroom", "Hosts Gerald's annual Dutch Empire Ball. Dress code: Orange or Stormtrooper armor only", false) },
                { 4, new Light(4, "Master Bedroom", "Features a king-size bed shaped like a giant stroopwafel. The sheets are orange, naturally", false) },
                { 5, new Light(5, "Trophy Room", "Houses Gerald's YouTube Play Buttons, crypto mining rig, and a tulip-shaped hot tub", false) },
                { 6, new Light(6, "Guest Suite 'De Windmill'", "For when Gerald's fellow Dutch YouTube millionaires visit. Has a working miniature windmill", false) },
                { 7, new Light(7, "Bathroom of Mirrors", "72 mirrors for practicing Gerald's iconic thumbnail expressions. Echo chamber for yodeling", true) },
                { 8, new Light(8, "Spa Bathroom", "Marble everything. The bathtub is shaped like a wooden clog. Pure class, Dutch style", true) },
                { 9, new Light(9, "Supercar Garage", "Contains Gerald's orange Lamborghini, orange Tesla, and a life-size Stormtrooper speeder bike replica", false) },
                { 10, new Light(10, "Tulip Garden", "10,000 orange tulips arranged to spell 'SUBSCRIBE' when viewed from Gerald's drone", true) },
                { 11, new Light(11, "Zen Garden", "Raked gravel in the pattern of stroopwafel grids. Very calming, very Dutch", false) },
                { 12, new Light(12, "Infinity Pool", "Gerald films his 'Dutch Millionaire Lifestyle' videos here. The pool tiles form a QR code to his Patreon", false) },
                { 13, new Light(13, "Pool House Cinema", "A private theater with stroopwafel-scented air and all 12 Star Wars movies on repeat", false) },
            };

        private readonly RemoteCallEmulator _remoteCallEmulator;

        public LightServiceMock()
        {
            _remoteCallEmulator = new RemoteCallEmulator(exceptionProbability: 0, exceptionCycle: true);
        }

        public event EventHandler<Light> LightUpdated;

        public async Task<List<Light>> GetLightsAsync()
        {
            Log.Info("GetLightsAsync()");

            await _remoteCallEmulator.EmulateRemoteCallDefault()
                .ConfigureAwait(false);
            var result = _remoteCallEmulator.Clone(_lights.Values.ToList());

            Log.Info($"returning {result.Count} lights");
            return result;
        }

        public async Task<Light> GetLightAsync(int lightId)
        {
            Log.Info($"GetLightAsync( lightId: {lightId} )");

            await _remoteCallEmulator.EmulateRemoteCallDefault()
                .ConfigureAwait(false);
            var result = _remoteCallEmulator.Clone(_lights[lightId]);

            return result;
        }

        public async Task UpdateLightAsync(Light light)
        {
            Log.Info($"UpdateLightAsync( light: {light.Name} )");

            await _remoteCallEmulator.EmulateRemoteCallDefault()
                .ConfigureAwait(false);
            _lights[light.Id] = light;
            var result = _remoteCallEmulator.Clone(_lights[light.Id]);
            DispatchLightUpdated(result);
        }

        private void DispatchLightUpdated(Light updatedLight)
        {
            Log.Info("DispatchLightUpdated()");
            Device.InvokeOnMainThreadAsync(() => LightUpdated?.Invoke(this, updatedLight));
        }
    }
}