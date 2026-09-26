document.addEventListener("DOMContentLoaded", () => {

    const user = getCurrentUser();

    const welcomeElement =
        document.getElementById("welcomeMessage");

    if (welcomeElement && user) {
        welcomeElement.textContent =
            `Dobrodošla, ${user.firstName}!`;
    }

});