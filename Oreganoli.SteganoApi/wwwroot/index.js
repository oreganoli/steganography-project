var encodeForm = document.getElementById("encodeForm");
var decodeForm = document.getElementById("decodeForm");
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
        // console.log(response.type);
        // console.log(imgBlob.size);
        // console.log(imgBlob.type)
        var imgUrl = URL.createObjectURL(imgBlob);
        let img = document.createElement("img");
        img.classList.add("w-75");
        img.classList.add("img-fluid");
        img.classList.add("border");
        img.classList.add("border-success");
        img.classList.add("rounded");
        img.src = imgUrl;
        outputDiv.replaceChildren(img);

    } else {
        response.text().then(text => {
            outputDiv.replaceChildren(document.createElement("h2", `Error: ${text}`));
        });
    }
}
/**
 * 
 * @param {SubmitEvent} e 
 */
async function decodeFormOnSubmit(e) {
    e.preventDefault();
    let response = await fetch("/stegano/decode", {
        body: new FormData(e.target),
        method: "post"
    })
    if (response.ok) {
        let text = await response.text();
        let p = document.createElement("p");
        p.innerText = text;
        p.classList.add("alert");
        p.classList.add("alert-success");
        outputDiv.replaceChildren(p);
    } else {
        response.text().then(text => {
            outputDiv.replaceChildren(document.createElement("h2", `Error: ${text}`));
        });
    }
}
encodeForm.onsubmit = encodeFormOnSubmit;
decodeForm.onsubmit = decodeFormOnSubmit;