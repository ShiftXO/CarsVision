function LoadModels() {
    let makeName = document.getElementById("makes").value;
    let modelsSelect = document.getElementById("models");

    let xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            modelsSelect.innerHTML = "<option value\"All\"></option>";
            var models = JSON.parse(this.responseText);
            for (let i = 0; i < models.length; i++) {
                const element = models[i];
                modelsSelect.innerHTML += `<option value"${element}">${element}</option>`;
            }
        }
    };
    xhr.open("GET", "/Home/Ajax?makeName=" + makeName);
    xhr.send();
}