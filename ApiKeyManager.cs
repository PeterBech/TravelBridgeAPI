namespace TravelBridgeAPI
{
    public class ApiKeyManager
    {
        private readonly List<string> _apiKeys;
        private int _currentIndex = -1;
        private readonly object _lock = new object();

        public ApiKeyManager(IEnumerable<string> apiKeys)
        {
            _apiKeys = new List<string>(apiKeys);
        }

        public string GetNextApiKey()
        {
            lock (_lock)
            {
                if (_apiKeys.Count == 0)
                    throw new Exception("No API keys available.");

                _currentIndex = (_currentIndex + 1) % _apiKeys.Count;
                return _apiKeys[_currentIndex];
            }
        }
    }
}
