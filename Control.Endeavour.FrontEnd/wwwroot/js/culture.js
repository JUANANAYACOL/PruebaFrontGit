window.culture = {
    get: () => window.localStorage['culture'],
    set: (value) => window.localStorage['culture'] = value
};
navigator.serviceWorker.register('service-worker.js');