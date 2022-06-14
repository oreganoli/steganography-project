var encodeForm = document.getElementById("encodeForm");
var outputDiv = document.getElementById("output");
/**
 * 
 * @param {SubmitEvent} e 
 */
async function encodeFormOnSubmit(e) {
    e.preventDefault();
    let response = await fetch("/stegano/encode", {
        body: new FormData(e.target),
        method: "post"
    })
    if (response.ok) {
        let imgBlob = await response.blob();
        console.log(response.type);
        console.log(imgBlob.size);
        console.log(imgBlob.type)
        console.log(URL.createObjectURL(imgBlob));
        const reader = new FileReader();
        reader.onloadend = function () {
            outputDiv.replaceChildren(document.createElement("img", {
                src: reader.result
            }));
        };
        reader.readAsDataURL(imgBlob);

    } else {
        response.text().then(text => {
            outputDiv.replaceChildren(document.createElement("h2", `Error: ${text}`));
        });
    }
}
encodeForm.onsubmit = encodeFormOnSubmit;