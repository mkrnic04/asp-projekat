//console.log("Favorites ready");
async function loadFavorites() {
    const container = document.getElementById("favoritesContainer");

    if (!container) {
        return;
    }

    if (!isLoggedIn()) {
        container.innerHTML = `
            <div class="alert alert-info">
                Moraš biti prijavljen da bi video favorite.
                <a href="/login.html">Prijavi se</a>.
            </div>
        `;
        return;
    }

    container.innerHTML = `
        <div class="text-center py-5">
            <div class="spinner-border"></div>
            <p class="mt-2">Učitavanje favorita...</p>
        </div>
    `;

    try {
        const response = await apiRequest(
            "/Favorites?page=1&pageSize=100"
        );

        if (!response.ok) {
            container.innerHTML = `
                <div class="alert alert-danger">
                    Nije moguće učitati favorite.
                </div>
            `;
            return;
        }

        const data = await response.json();

        if (!data.items || data.items.length === 0) {
            container.innerHTML = `
                <div class="alert alert-info">
                    Još nemaš sačuvane favorite.
                </div>
            `;
            return;
        }

        container.innerHTML = data.items.map(favorite => `
            <div class="col-md-4 mb-4">

                <div class="card shadow-sm h-100">

                    <div class="card-body d-flex flex-column">

                        <h5 class="card-title">
                            ${escapeHtml(favorite.bookTitle)}
                        </h5>

                        <p class="text-muted">
                            Dodato:
                            ${new Date(
            favorite.createdAt
        ).toLocaleDateString()}
                        </p>

                        <div class="mt-auto">

                            <a
                                href="/book-details.html?id=${favorite.bookId}"
                                class="btn btn-primary"
                            >
                                Pogledaj knjigu
                            </a>

                            <button
                                class="btn btn-outline-danger"
                                onclick="deleteFavorite(${favorite.id})"
                            >
                                Ukloni
                            </button>

                        </div>

                    </div>

                </div>

            </div>
        `).join("");

    } catch {
        container.innerHTML = `
            <div class="alert alert-danger">
                Došlo je do greške.
            </div>
        `;
    }
}


async function deleteFavorite(id) {

    if (!confirm("Da li želiš da ukloniš ovu knjigu iz favorita?")) {
        return;
    }

    try {
        const response = await apiRequest(
            `/Favorites/${id}`,
            {
                method: "DELETE"
            }
        );

        if (!response.ok) {
            alert("Nije moguće ukloniti favorit.");
            return;
        }

        await loadFavorites();

    } catch {
        alert("Došlo je do greške.");
    }
}


document.addEventListener("DOMContentLoaded", () => {
    loadFavorites();
});