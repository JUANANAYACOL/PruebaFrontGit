// CodeInput.js

function openPdfFromBytes(byteArray) {
    // Crear Blob y URL
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);

    // Abrir URL en nueva pestaña
    window.open(url, '_blank');
}



//#region Visualizar un archivo
 function abrirNuevoTabUrl(FileName, ext, base64Data, tipoDescarga) {
    if (tipoDescarga === true) {
        const byteCharacters = atob(base64Data);
        const byteNumbers = new Array(byteCharacters.length);

        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: 'application/octet-stream' });

        const urlBlob = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = urlBlob;
        a.download = (FileName ? FileName : 'file') + (ext ? '.' + ext : '');
        a.click();
        URL.revokeObjectURL(urlBlob);
    } else {
       
        const dataURL = 'data:application/octet-stream;base64,' + base64Data;
        window.open(dataURL, '_blank');
    }
}
//#endregion

function moveFocusToNextField(currentIndex) {
    var nextInput = document.querySelectorAll('.code-input')[currentIndex + 1];
    if (nextInput) {
        nextInput.focus();
    }
}

function moveFocusToPreviousField(currentIndex) {
    if (currentIndex > 0) {
        var previousInput = document.querySelectorAll('.code-input')[currentIndex - 1];
        previousInput.focus();
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const inputs = document.querySelectorAll('.code-input');
    inputs.forEach((element, index) => {
        element.addEventListener('input', (event) => {
            if (event.target.value.length === 1) {
                moveFocusToNextField(index);
            }
        });

        element.addEventListener('keydown', (event) => {
            if (event.key === 'Backspace' && event.target.value === '') {
                moveFocusToPreviousField(index);
            }
        });
    });
});
