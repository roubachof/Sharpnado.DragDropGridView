using Newtonsoft.Json;

namespace Mvvm.Flux.Maui.Infrastructure.Mocking
{
    public class RemoteCallEmulator
    {
        protected static readonly Random Randomizer = new Random();

        protected static readonly object RandomizerLock = new object();

        private static readonly Exception[] ExceptionRing = new Exception[]
            {
                new NetworkException(),
                new ServerException(),
                new InvalidOperationException(),
            };

        private readonly int _minDelay;

        private readonly int _maxDelay;

        private readonly double _exceptionProbability;

        private readonly bool _exceptionCycle;

        private int _currentExceptionIndex;

        public RemoteCallEmulator(int minDelay = 2, int maxDelay = 3, double exceptionProbability = 0, bool exceptionCycle = false)
        {
            _minDelay = minDelay;
            _maxDelay = maxDelay;
            _exceptionProbability = exceptionProbability;
            _exceptionCycle = exceptionCycle;

            lock (RandomizerLock)
            {
                _currentExceptionIndex = Randomizer.Next(0, 2);
            }
        }

        public virtual Task EmulateRemoteCallDefault() =>
            EmulateRemoteCall(_minDelay, _maxDelay, _exceptionProbability, _exceptionCycle);

        public virtual async Task EmulateRemoteCall(
            int minDelay = 2,
            int maxDelay = 3,
            double exceptionProbability = 0,
            bool exceptionCycle = false)
        {
            int delayMilliseconds;
            lock (RandomizerLock)
            {
                delayMilliseconds = Randomizer.Next(minDelay * 1000, maxDelay * 1000);
            }

            await Task.Delay(delayMilliseconds);

            double exceptionDraw;
            lock (RandomizerLock)
            {
                exceptionDraw = Randomizer.NextDouble();
            }

            if (exceptionDraw <= exceptionProbability)
            {
                int next;
                if (exceptionCycle)
                {
                    next = _currentExceptionIndex++ % 3;
                }
                else
                {
                    lock (RandomizerLock)
                    {
                        next = Randomizer.Next(0, 2);
                    }
                }

                throw ExceptionRing[next];
            }
        }

        /// <summary>
        /// To emulate a remote call, objects are serialized and then deserialized.
        /// We get different instance with each call.
        /// </summary>
        public T Clone<T>(T @object)
        {
            var serializedObject = JsonConvert.SerializeObject(@object);
            return JsonConvert.DeserializeObject<T>(serializedObject);
        }
    }
}
