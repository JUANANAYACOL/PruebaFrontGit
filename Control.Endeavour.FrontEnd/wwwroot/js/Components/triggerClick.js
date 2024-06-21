window.triggerClick = (elementId) => {
    document.getElementById(elementId).click();
};

window.initializeFilePaste = (dropContainer, inputFile) => {
    function onPaste(event) {
        inputFile.files = event.clipboardData.files;
        const changeEvent = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(changeEvent);
    }

    function onDragOver(event) {
        event.preventDefault();
    }

    function onDrop(event) {
        event.preventDefault();
        inputFile.files = event.dataTransfer.files;
        const changeEvent = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(changeEvent);
    }

    function onDropNew(event) {
        event.preventDefault();

        // Verificar si ya hay una imagen presente
        if (inputFile.files.length > 0) {
            // Eliminar la imagen existente
            inputFile.files = null; // o inputFile.files = new FileList();
        }

        // Establecer los archivos soltados en el elemento de entrada de archivo
        inputFile.files = event.dataTransfer.files;

        // Disparar un evento de cambio en el elemento de entrada de archivo
        const changeEvent = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(changeEvent);
    }


    dropContainer.addEventListener('paste', onPaste);
    dropContainer.addEventListener('dragover', onDragOver);
    dropContainer.addEventListener('dropnew', onDropNew);
    dropContainer.addEventListener('drop', onDrop);

    return {
        dispose: () => {
            dropContainer.removeEventListener('paste', onPaste);
            dropContainer.removeEventListener('dragover', onDragOver);
            dropContainer.removeEventListener('dropnew', onDropNew);
            dropContainer.removeEventListener('drop', onDrop);
        }
    };
};


