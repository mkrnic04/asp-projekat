//console.log("Auth");
document.addEventListener("DOMContentLoaded", () => {

    // LOGIN
    const loginForm = document.getElementById("loginForm");

    if (loginForm) {
        loginForm.addEventListener("submit", async function (e) {
            e.preventDefault();

            const username = document.getElementById("username").value;
            const password = document.getElementById("password").value;

            try {
                const response = await fetch("/api/Auth", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        username,
                        password
                    })
                });

                if (!response.ok) {
                    showMessage(
                        "loginMessage",
                        "Pogrešno korisničko ime ili lozinka."
                    );
                    return;
                }

                const data = await response.json();

                saveTokens(data);

                window.location.href = "/index.html";

            } catch (error) {
                showMessage(
                    "loginMessage",
                    "Došlo je do greške prilikom prijave."
                );
            }
        });
    }


    // REGISTER
    const registerForm = document.getElementById("registerForm");

    if (registerForm) {
        registerForm.addEventListener("submit", async function (e) {
            e.preventDefault();

            const data = {
                firstName: document.getElementById("firstName").value,
                lastName: document.getElementById("lastName").value,
                username: document.getElementById("username").value,
                email: document.getElementById("email").value,
                password: document.getElementById("password").value
            };

            try {
                const response = await fetch("/api/Users", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(data)
                });

                if (!response.ok) {
                    let message = "Registracija nije uspela.";

                    try {
                        const result = await response.json();

                        if (result.errors) {
                            message = Object.values(result.errors)
                                .flat()
                                .join("<br>");
                        }
                    } catch {
                    }

                    showMessage("registerMessage", message);
                    return;
                }

                showMessage(
                    "registerMessage",
                    "Registracija je uspešna! Možeš se prijaviti.",
                    "success"
                );

                registerForm.reset();

            } catch {
                showMessage(
                    "registerMessage",
                    "Došlo je do greške prilikom registracije."
                );
            }
        });
    }
});