function ToggleRegisterForm() {
    let normalForm = document.getElementById("normal");
    let dealershipForm = document.getElementById("dealership");


    let heading = document.getElementsByTagName("h4")[0];
    let toggle = document.getElementById("toggle");

    if (toggle.textContent == `register as dealer?`) {
        // TODO : hide dealership stuff
        normalForm.style.display = "none";
        dealershipForm.style.display = "block";
        toggle.textContent = "register as normal account?";
        heading.textContent = "Create a new dealership account.";
    } else {
        normalForm.style.display = "block";
        dealershipForm.style.display = "none";
        toggle.textContent = "register as dealer?";
        heading.textContent = "Create a new account.";
        // TODO : hide normal user stuff
    }
}