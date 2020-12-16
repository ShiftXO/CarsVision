function AddToWatchlist(id) {
    var hearts = document.querySelectorAll(".fa-heart");
    let arr = Array.from(hearts);
    var heart = arr.filter(x => x.attributes[1].nodeValue == id);
    let xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            let isAdded = this.responseText;
            if (isAdded == "created") {
                heart[0].className = "fas fa-heart float-right zoom text-primary";
            }
            else {
                heart[0].className = "far fa-heart float-right zoom text-primary";
            }
        }
    };
    xhr.open("GET", "/Home/Add?id=" + id);
    xhr.send();
}