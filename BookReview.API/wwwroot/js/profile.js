async function loadProfile() {
    const user = getCurrentUser();

    if (!user) {
        window.location.href = "/login.html";
        return;
    }

    try {
        const response = await apiRequest(`/Users/${user.id}`);

        if (!response.ok) {
            showMessage(
                "profileMessage",
                "Nije moguće učitati profil."
            );
            return;
        }

        const data = await response.json();

        document.getElementById("firstName").value =
            data.firstName || "";

        document.getElementById("lastName").value =
            data.lastName || "";

        document.getElementById("email").value =
            data.email || "";

        // Ako korisnik nije Admin,
        // polja su samo za čitanje.
        if (user.role !== "Admin") {

            document.getElementById("firstName").readOnly = true;
            document.getElementById("lastName").readOnly = true;
            document.getElementById("email").readOnly = true;

            // Sakrivamo dugme za čuvanje.
            const saveButton =
                document.querySelector("#profileForm button[type='submit']");

            if (saveButton) {
                saveButton.style.display = "none";
            }
        }

    } catch {
        showMessage(
            "profileMessage",
            "Greška prilikom učitavanja profila."
        );
    }
}


async function updateProfile(event) {
    event.preventDefault();

    const user = getCurrentUser();

    if (!user) {
        window.location.href = "/login.html";
        return;
    }

    // User ne može da menja profil.
    if (user.role !== "Admin") {
        return;
    }

    try {

        // Uzimamo postojeće podatke
        const getResponse = await apiRequest(
            `/Users/${user.id}`
        );

        if (!getResponse.ok) {
            showMessage(
                "profileMessage",
                "Nije moguće učitati postojeće podatke."
            );
            return;
        }

        const currentUser = await getResponse.json();

        const data = {
            username: currentUser.username,
            email: document.getElementById("email").value,
            firstName: document.getElementById("firstName").value,
            lastName: document.getElementById("lastName").value,
            role: currentUser.role
        };

        const response = await apiRequest(
            `/Users/${user.id}`,
            {
                method: "PUT",
                body: JSON.stringify(data)
            }
        );

        if (!response.ok) {
            showMessage(
                "profileMessage",
                "Nije moguće sačuvati izmene."
            );
            return;
        }

        showMessage(
            "profileMessage",
            "Profil je uspešno izmenjen.",
            "success"
        );

    } catch {
        showMessage(
            "profileMessage",
            "Došlo je do greške."
        );
    }
}


document.addEventListener("DOMContentLoaded", () => {

    if (!document.getElementById("profileForm")) {
        return;
    }

    loadProfile();

    document
        .getElementById("profileForm")
        .addEventListener(
            "submit",
            updateProfile
        );
});