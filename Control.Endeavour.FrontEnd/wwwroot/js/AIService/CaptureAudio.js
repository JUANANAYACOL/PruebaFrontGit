var BlazorAudioRecorder = {};

(function () {
    var stream;
    var mAudioChunks;
    var mediaRecorder;
    var mCaller;

    BlazorAudioRecorder.Initialize = function (vCaller) {
        mCaller = vCaller;
    };

    BlazorAudioRecorder.StartRecord = async function () {
        stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        mediaRecorder = new MediaRecorder(stream);
        mediaRecorder.addEventListener('dataavailable', vEvent => {
            mAudioChunks.push(vEvent.data);
        });

        mediaRecorder.addEventListener('error', vError => {
            console.warn('media recorder error: ' + vError);
        });

        mediaRecorder.addEventListener('stop', () => {
            var audioBlob = new Blob(mAudioChunks, { type: "audio/mp3;" });
            var audioUrl = URL.createObjectURL(audioBlob);
            ConvertBlobToBase64(audioBlob).then(base64 => {
                mCaller.invokeMethodAsync('TakeAudio', audioUrl, base64);
            });
        });

        mAudioChunks = [];
        mediaRecorder.start();
    };

    BlazorAudioRecorder.StopRecord = function () {
        mediaRecorder.stop();
        stream.getTracks().forEach(pTrack => pTrack.stop());
    };
})();

function ConvertBlobToBase64(blob) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onloadend = () => resolve(reader.result);
        reader.onerror = reject;
        reader.readAsDataURL(blob);
    });
}