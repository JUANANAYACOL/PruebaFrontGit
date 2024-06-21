function pemToPublicKey(pem) {
    const binaryDerString = atob(pem.replace(/(-----(BEGIN|END) PUBLIC KEY-----|[\n\r])/g, ''));
    const binaryDer = new Uint8Array(binaryDerString.length);
    for (let i = 0; i < binaryDerString.length; i++) {
        binaryDer[i] = binaryDerString.charCodeAt(i);
    }
    return window.crypto.subtle.importKey(
        'spki',
        binaryDer.buffer,
        {
            name: 'RSA-OAEP',
            hash: 'SHA-512',
        },
        true,
        ['encrypt']
    );
}

function pemToPrivateKey(pem) {
    const binaryDerString = atob(pem.replace(/(-----(BEGIN|END) PRIVATE KEY-----|[\n\r])/g, ''));
    const binaryDer = new Uint8Array(binaryDerString.length);
    for (let i = 0; i < binaryDerString.length; i++) {
        binaryDer[i] = binaryDerString.charCodeAt(i);
    }
    return window.crypto.subtle.importKey(
        'pkcs8',
        binaryDer.buffer,
        {
            name: 'RSA-OAEP',
            hash: 'SHA-512',
        },
        true,
        ['decrypt']
    );
}

function encryptData(publicKey, data) {
    const encoder = new TextEncoder();
    const userString = JSON.stringify(data);
    const encodedUser = encoder.encode(userString);

    return window.crypto.subtle.encrypt(
        {
            name: 'RSA-OAEP'
        },
        publicKey,
        encodedUser
    );
}

function setData(value, data) {
    localStorage.setItem(value, data);
}

function encryptDataReturn(data, value, pem) {
    pemToPublicKey(pem).then(publicKey => {
        encryptData(publicKey, data).then(encrypted => {
            let uint8Array = new Uint8Array(encrypted);
            let base64String = btoa(String.fromCharCode.apply(null, uint8Array));

            if (typeof (Storage) !== "undefined") {
                sessionStorage.setItem(value, base64String);
            } else {
                console.error("sessionStorage no se encuentra definido")
            }
        }).catch(ex => console.error(ex.data));
    }).catch(ex => console.error(ex));
}

function decryptData(privateKey, encryptedData) {
    return window.crypto.subtle.decrypt(
        {
            name: 'RSA-OAEP'
        },
        privateKey,
        encryptedData
    ).then(decryptedBuffer => {
        const decoder = new TextDecoder();
        const decryptedString = decoder.decode(decryptedBuffer);
        return JSON.parse(decryptedString);
    });
}

function decryptDataReturn(value, pem) {
    let keyValue = sessionStorage.getItem(value);
    let binaryString = atob(keyValue);
    let len = binaryString.length;
    let arrayBuffer = new ArrayBuffer(len);
    let uint8Array = new Uint8Array(arrayBuffer);

    for (let i = 0; i < len; i++) {
        uint8Array[i] = binaryString.charCodeAt(i);
    }

    return pemToPrivateKey(pem).then(privateKey => {
        return decryptData(privateKey, uint8Array).then(decryptedUser => {
            return decryptedUser;
        });
    }).catch(ex => console.error(ex));
}